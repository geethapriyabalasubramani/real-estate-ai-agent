using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using RealEstateAiAgent.Api.Configuration;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Exceptions;

namespace RealEstateAiAgent.Api.Services;

public class RealEstateAgentService : IRealEstateAgentService
{
    private const int MaxSteps = 6;
    private const int MaxResultSummaryLength = 4000;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly IBedrockChatService _bedrockChatService;
    private readonly RealEstateAgentToolExecutor _tools;
    private readonly BedrockOptions _options;

    public RealEstateAgentService(
        IBedrockChatService bedrockChatService,
        RealEstateAgentToolExecutor tools,
        IOptions<BedrockOptions> options)
    {
        _bedrockChatService = bedrockChatService;
        _tools = tools;
        _options = options.Value;
    }

    public async Task<AgentChatResponse> ChatAsync(
        AgentChatRequest request,
        CancellationToken cancellationToken = default)
    {
        var userMessage = request.Message.Trim();
        var transcript = new StringBuilder();
        transcript.AppendLine("USER REQUEST:");
        transcript.AppendLine(userMessage);
        transcript.AppendLine();

        var traces = new List<AgentToolCallTrace>();

        for (var step = 0; step < MaxSteps; step++)
        {
            var prompt = transcript.ToString();
            var raw = await GetRouterJsonAsync(prompt, cancellationToken);
            var decision = ParseDecision(raw);

            if (IsReply(decision))
            {
                var message = decision.Message?.Trim();
                if (string.IsNullOrEmpty(message))
                {
                    throw new InvalidOperationException("Agent returned an empty reply.");
                }

                return new AgentChatResponse(message, traces);
            }

            if (!IsTool(decision))
            {
                throw new InvalidOperationException("Agent returned invalid JSON (expected tool or reply).");
            }

            var argsJson = decision.Arguments?.GetRawText() ?? "{}";
            string toolResult;
            try
            {
                toolResult = await _tools.ExecuteAsync(
                    decision.Tool!.Trim(),
                    decision.Arguments ?? default,
                    cancellationToken);
            }
            catch (ArgumentException ex)
            {
                toolResult = JsonSerializer.Serialize(new { error = ex.Message });
            }
            catch (AiCriteriaValidationException ex)
            {
                toolResult = JsonSerializer.Serialize(new { error = ex.Message, validation = ex.Errors });
            }

            var summary = Truncate(toolResult, MaxResultSummaryLength);
            traces.Add(new AgentToolCallTrace(decision.Tool!.Trim(), argsJson, summary));

            transcript.AppendLine("ASSISTANT JSON:");
            transcript.AppendLine(ExtractJsonPayload(raw).Trim());
            transcript.AppendLine();
            transcript.AppendLine($"TOOL RESULT ({decision.Tool}):");

            transcript.AppendLine(summary);

            transcript.AppendLine();

        }



        return new AgentChatResponse(

            "I could not finish within the allowed number of steps. Please try a simpler question.",

            traces);

    }



    private async Task<string> GetRouterJsonAsync(string prompt, CancellationToken cancellationToken)

    {

        var raw = await _bedrockChatService.CompleteAsync(

            prompt,

            _options.AgentRouterSystemPrompt,

            _options.AgentMaxTokens,

            _options.AgentTemperature,

            cancellationToken);



        if (TryParseDecision(raw, out _))

        {

            return raw;

        }



        var repairPrompt = $"""

            Your previous response was not valid JSON for the router protocol.



            INVALID RESPONSE:

            {raw}



            Output ONLY one corrected JSON object. No markdown, no prose. Start with an opening brace and end with a closing brace.

            """;



        return await _bedrockChatService.CompleteAsync(

            repairPrompt,

            _options.AgentRouterSystemPrompt,

            _options.AgentMaxTokens,

            _options.AgentTemperature,

            cancellationToken);

    }



    private static bool IsReply(AgentDecision decision) =>

        string.Equals(decision.Action, "reply", StringComparison.OrdinalIgnoreCase);



    private static bool IsTool(AgentDecision decision) =>

        string.Equals(decision.Action, "tool", StringComparison.OrdinalIgnoreCase)

        && !string.IsNullOrWhiteSpace(decision.Tool);



    private static AgentDecision ParseDecision(string raw)

    {

        if (!TryParseDecision(raw, out var decision))

        {

            throw new InvalidOperationException(

                "Could not parse agent JSON. The model must return a single JSON object.");

        }



        return decision;

    }



    private static bool TryParseDecision(string raw, out AgentDecision decision)

    {

        decision = null!;

        var json = ExtractJsonPayload(raw);



        try

        {

            var parsed = JsonSerializer.Deserialize<AgentDecision>(json, JsonOptions);

            if (parsed is null || string.IsNullOrWhiteSpace(parsed.Action))

            {

                return false;

            }



            decision = parsed;

            return true;

        }

        catch (JsonException)

        {

            return false;

        }

    }



    private static string ExtractJsonPayload(string raw)

    {

        var text = StripMarkdownJson(raw).Trim();

        if (text.StartsWith('{') && text.EndsWith('}'))

        {

            return text;

        }



        var start = text.IndexOf('{');

        var end = text.LastIndexOf('}');

        if (start >= 0 && end > start)

        {

            return text[start..(end + 1)];

        }



        return text;

    }



    private static string StripMarkdownJson(string raw)

    {

        var text = raw.Trim();

        if (!text.StartsWith("```", StringComparison.Ordinal))

        {

            return text;

        }



        var lines = text.Split('\n');

        if (lines.Length < 2)

        {

            return text;

        }



        var body = new StringBuilder();

        for (var i = 1; i < lines.Length; i++)

        {

            if (lines[i].StartsWith("```", StringComparison.Ordinal))

            {

                break;

            }



            body.AppendLine(lines[i]);

        }



        return body.ToString();

    }



    private static string Truncate(string value, int maxLength) =>

        value.Length <= maxLength ? value : value[..maxLength] + "…";



    private sealed class AgentDecision

    {

        [JsonPropertyName("action")]

        public string Action { get; set; } = string.Empty;



        [JsonPropertyName("tool")]

        public string? Tool { get; set; }



        [JsonPropertyName("arguments")]

        public JsonElement? Arguments { get; set; }



        [JsonPropertyName("message")]

        public string? Message { get; set; }

    }

}


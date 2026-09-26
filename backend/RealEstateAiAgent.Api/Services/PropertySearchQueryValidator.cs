using System.ComponentModel.DataAnnotations;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Exceptions;

namespace RealEstateAiAgent.Api.Services;

public static class PropertySearchQueryValidator
{
    public static void Validate(PropertySearchQuery query)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(query);

        if (!Validator.TryValidateObject(query, context, results, validateAllProperties: true))
        {
            throw new AiCriteriaValidationException(ToErrorDictionary(results));
        }
    }

    private static Dictionary<string, string[]> ToErrorDictionary(IEnumerable<ValidationResult> results)
    {
        var dictionary = new Dictionary<string, List<string>>();

        foreach (var result in results)
        {
            var message = result.ErrorMessage ?? "Invalid value.";
            var members = result.MemberNames.Any() ? result.MemberNames : new[] { string.Empty };

            foreach (var member in members)
            {
                var key = string.IsNullOrWhiteSpace(member) ? "criteria" : member;
                if (!dictionary.TryGetValue(key, out var list))
                {
                    list = new List<string>();
                    dictionary[key] = list;
                }
                list.Add(message);
            }
        }

        return dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
    }
}
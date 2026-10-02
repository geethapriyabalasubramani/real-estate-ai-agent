FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RealEstateAiAgent.Api.csproj ./
RUN dotnet restore RealEstateAiAgent.Api.csproj

COPY . ./
RUN dotnet publish RealEstateAiAgent.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RealEstateAiAgent.Api.dll"]

FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY EventHub.Simulator.Writer/EventHub.Simulator.Writer.csproj EventHub.Simulator.Writer/
RUN dotnet restore EventHub.Simulator.Writer/EventHub.Simulator.Writer.csproj
COPY . .
WORKDIR /src/EventHub.Simulator.Writer
RUN dotnet build EventHub.Simulator.Writer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish EventHub.Simulator.Writer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EventHub.Simulator.Writer.dll"]

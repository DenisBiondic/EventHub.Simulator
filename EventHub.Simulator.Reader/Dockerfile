FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY EventHub.Simulator.Reader/EventHub.Simulator.Reader.csproj EventHub.Simulator.Reader/
RUN dotnet restore EventHub.Simulator.Reader/EventHub.Simulator.Reader.csproj
COPY . .
WORKDIR /src/EventHub.Simulator.Reader
RUN dotnet build EventHub.Simulator.Reader.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish EventHub.Simulator.Reader.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EventHub.Simulator.Reader.dll"]

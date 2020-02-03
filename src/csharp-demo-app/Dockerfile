FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
RUN dotnet tool install --global ScopeAgent.Runner
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out
WORKDIR /app/out
RUN /root/.dotnet/tools/scope-run --apply-coverage

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1
RUN dotnet tool install --global ScopeAgent.Runner
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["/root/.dotnet/tools/scope-run", "dotnet", "./csharp-demo-app.dll"]
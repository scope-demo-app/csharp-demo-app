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
#RUN /root/.dotnet/tools/scope-run --apply-coverage  # Causing errors in NetCore 3.1 

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /root/.dotnet/tools /root/.dotnet/tools
ENTRYPOINT ["/root/.dotnet/tools/scope-run", "dotnet", "./csharp-demo-app.dll"]
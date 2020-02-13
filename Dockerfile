FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
RUN dotnet tool install --global ScopeAgent.Runner  --version 0.1.22-beta.5
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./src/csharp-demo-app/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./src/csharp-demo-app/. ./
RUN dotnet build -c Release -o out
WORKDIR /app/out
RUN /root/.dotnet/tools/scope-run --apply-coverage 

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#RUN apt-get -y update && apt-get -y install git && apt-get clean && rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* /usr/share/man/?? /usr/share/man/??_*
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /root/.dotnet/tools /root/.dotnet/tools
COPY ./.git ./.git
RUN ls -l
ENTRYPOINT ["/root/.dotnet/tools/scope-run", "dotnet", "./csharp-demo-app.dll"]
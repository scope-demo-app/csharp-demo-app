FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
RUN dotnet tool install --global ScopeAgent.Runner  --version 0.1.22-beta.4
WORKDIR /app
COPY . ./

# Copy everything else and build
RUN cd ./src/csharp-demo-app/
RUN dotnet restore
RUN dotnet publish -c Release -o out
WORKDIR /app/out
#RUN /root/.dotnet/tools/scope-run --apply-coverage  # Causing errors in NetCore 3.1 

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#RUN apt-get -y update && apt-get -y install git && apt-get clean && rm -rf /var/lib/apt/lists/* /tmp/* /var/tmp/* /usr/share/man/?? /usr/share/man/??_*
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /root/.dotnet/tools /root/.dotnet/tools
COPY ./.git ./.git
ENTRYPOINT ["/root/.dotnet/tools/scope-run", "dotnet", "./csharp-demo-app.dll"]
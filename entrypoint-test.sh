#!/usr/bin/env bash
sleep 60

/root/.dotnet/tools/scope-run dotnet test

cd ./test/csharp-demo-app.benchmark/
dotnet run -c Release


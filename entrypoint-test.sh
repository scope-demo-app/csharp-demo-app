#!/usr/bin/env bash
/root/.dotnet/tools/scope-run dotnet test

cd ./test/csharp-demo-app.benchmark/
dotnet run -c Release


#!/usr/bin/env bash
/root/.dotnet/tools/scope-run dotnet test -c Release
cd ./test/csharp-demo-app.benchmark/
dotnet run -c Release
cd ../..

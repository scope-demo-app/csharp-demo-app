#!/usr/bin/env bash
cd ./test/csharp-demo-app.benchmark/
dotnet run -c Release

cd ../..
/root/.dotnet/tools/scope-run dotnet test

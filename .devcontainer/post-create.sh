#!/usr/bin/env bash
set -e

echo "🛠️  Installing EF tool and restoring backend …"
dotnet tool install --global dotnet-ef
export PATH="$PATH:$HOME/.dotnet/tools"
dotnet restore backend

echo "🛠️  Installing frontend deps …"
cd frontend && npm ci && cd ..

echo "✅ Ready!"
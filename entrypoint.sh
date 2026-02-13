#!/bin/bash
set -e

echo "Starting WorshipManager API..."

# Run EF Core migrations
if [ "$RUN_MIGRATIONS" = "true" ]; then
  echo "Running database migrations..."
  dotnet WorshipManager.Api.dll --migrate
fi

# Start the application
exec dotnet WorshipManager.Api.dll

#!/bin/sh

echo "==== Environment Variables ===="
printenv | sort
echo "==============================="

# Optional: show the actual connection string for confirmation
echo "Connection string:"
echo "$ConnectionString__DefaultConnection"

echo "Starting application..."
exec dotnet CookBook_Api.dll

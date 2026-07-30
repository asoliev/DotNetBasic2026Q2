#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

DB_NAME="${MODULE13_DB_NAME:-AdoNetModule13}"
SA_PASSWORD="${MSSQL_SA_PASSWORD:-Your_password123}"

"$ROOT_DIR/scripts/bootstrap-db.sh"

export ADO_NET_TEST_CONNECTION_STRING="Server=localhost,1433;Database=$DB_NAME;User Id=sa;Password=$SA_PASSWORD;TrustServerCertificate=True;"

echo "Running integration tests against: $DB_NAME"
dotnet test --project AdoNetLibrary.Tests/AdoNetLibrary.Tests.csproj

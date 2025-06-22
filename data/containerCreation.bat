@echo off
setlocal ENABLEDELAYEDEXPANSION

echo Reading credentials from credentials.json...
for /f "delims=" %%A in ('powershell -NoProfile -Command ^
    "$json = Get-Content credentials.json | ConvertFrom-Json; Write-Output ($json.adminUsername + '|' + $json.adminPassword)"') do (
        set "CREDENTIALS=%%A" )
for /f "tokens=1,2 delims=|" %%A in ("%CREDENTIALS%") do (
    set "ADMIN_USERNAME=%%A"
    set "ADMIN_PASSWORD=%%B"
)

echo Pulling Neo4j image...
docker pull neo4j:5.15

echo Removing old container if exists...
docker rm -f family-tree-graph >nul 2>&1

echo Starting new Neo4j container with credentials...
docker run^
    --name family-tree-graph^
    -p7474:7474^
    -p7687:7687^
    -e NEO4J_AUTH=%ADMIN_USERNAME%/%ADMIN_PASSWORD%^
    -d neo4j:5.15

endlocal 
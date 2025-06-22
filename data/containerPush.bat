@echo off
setlocal ENABLEDELAYEDEXPANSION
SET ACR_NAME="familyTreeRegistry"
SET LOGIN_SERVER="familytreeregistry.azurecr.io"
SET REPOSITORY_NAME="family-tree-graph"
echo Logging into %ACR_NAME%
call az acr login --name %ACR_NAME%
echo Retrieving the %REPOSITORY_NAME%
docker build -t %LOGIN_SERVER%/%REPOSITORY_NAME%:latest .
echo Pushing the %REPOSITORY_NAME% to %LOGIN_SERVER%
docker push %LOGIN_SERVER%/%REPOSITORY_NAME%:latest
call az acr repository list --name %ACR_NAME% --output table
call az acr repository show-tags --name %ACR_NAME% --repository %REPOSITORY_NAME% --output table
endlocal
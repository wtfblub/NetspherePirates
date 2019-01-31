FROM microsoft/dotnet:2.1-runtime-alpine
COPY . /app/
WORKDIR /app
ENTRYPOINT ["dotnet", "${BINARY}"]

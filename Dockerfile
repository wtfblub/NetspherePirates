FROM microsoft/dotnet:2.1-runtime-alpine
ARG APP_BINARY
ENV BINARY=${APP_BINARY}
COPY . /app/
WORKDIR /app
CMD dotnet ${BINARY}

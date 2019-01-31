FROM microsoft/dotnet:2.1-runtime-alpine
COPY . /app/
RUN mkdir /data
RUN mv /app/config.hjson /data/config.hjson
ENV ${ENVNAME}=/data
VOLUME ["/data"]
WORKDIR /app
ENTRYPOINT ["dotnet", "${BINARY}"]

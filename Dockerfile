FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /source
COPY src .
RUN dotnet restore -r linux-musl-x64
WORKDIR /source/OneAPI.Meter
RUN dotnet publish -c release -o /app -r linux-musl-x64 --no-self-contained --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
WORKDIR /app
COPY --from=build /app ./

RUN apk add --no-cache icu-libs tzdata curl nano ca-certificates && \
    update-ca-certificates

ENV LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

ENTRYPOINT ["./OneAPI.Meter"]
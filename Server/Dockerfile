FROM mcr.microsoft.com/dotnet/sdk:9.0 as builder
COPY . /code
WORKDIR /code
RUN dotnet publish -c Release -o /code/Build YoutubeFeeds.Server

FROM mcr.microsoft.com/dotnet/aspnet:9.0
CMD ["dotnet", "YoutubeFeeds.Server.dll"]
WORKDIR /app
COPY --from=builder /code/Build .

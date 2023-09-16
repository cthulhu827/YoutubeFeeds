FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
COPY . /code
WORKDIR /code
RUN dotnet publish -c Release -o /code/Build YoutubeFeeds.Server

FROM mcr.microsoft.com/dotnet/aspnet:7.0
CMD ["dotnet", "YoutubeFeeds.Server.dll"]
WORKDIR /app
COPY --from=builder /code/Build .

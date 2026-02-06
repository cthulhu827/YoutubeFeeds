## Прокси YoutubeFeeds.RssProxy.Go
Получение RSS'ов не работает при запуске из контейнера. Из докера не доступен youtube.com (а просто с машины доступен).

Быстрое решение - поднять на моей виртуалке прокси для получения RSS'ов:
```
<proxy>/api/get-rss?url=<rss-url>
```

Пример скачивания RSS:
```
http://5.129.226.93:5231/api/get-rss?url=https://www.youtube.com/feeds/videos.xml?channel_id=UCkFtw6okFxx-4RPgsCFnkjw
```

## Deploy
```
mkdir ytf-proxy-go
scp ./* root@5.129.226.93:/home/ytf-proxy-go
```
FROM golang:1.22-alpine AS builder

WORKDIR /app

# Копируем go.mod и go.sum (если есть зависимости)
COPY go.mod ./
# COPY go.sum ./
# RUN go mod download

# Копируем исходники
COPY . .

# Компилируем статический бинарник без CGO
RUN CGO_ENABLED=0 GOOS=linux GOARCH=amd64 go build -ldflags="-s -w" -o rss-proxy main.go

# Финальный образ - scratch (минимальный размер)
FROM scratch

# Копируем SSL сертификаты для HTTPS запросов
COPY --from=builder /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/

# Копируем бинарник
COPY --from=builder /app/rss-proxy /rss-proxy

# Открываем порт
EXPOSE 5231

# Запускаем приложение
ENTRYPOINT ["/rss-proxy"]

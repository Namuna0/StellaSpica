# ビルド用ステージ
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o /out

# 実行用ステージ
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# 日本語フォントをインストール
RUN apt-get update && \
    apt-get install -y fonts-noto-cjk && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /out .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "DiscordBot.dll"]
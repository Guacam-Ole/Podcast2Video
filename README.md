# Podcast2Video
Creates Videos from Podcasts

## Quickstart
To start running you need to do the following:

0. Download dotnet-runtime if not installed already (not required on Windows)
1. Download ffmpeg for your platform: https://ffmpeg.org/download.html
2. Download the package for your platform: https://github.com/OleAlbers/Podcast2Video/releases/

Enter the path for ffmpeg into the appsettings.json:
 ```
 "Ffmpeg": {
    "Path": "D:\\ffmpeg\\ffmpeg.exe",
 ```

start using p2v via
```
p2v <url> 
```

where url is the url of your rss-feed

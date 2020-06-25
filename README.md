# Podcast2Video
Creates Videos from Podcasts

## Quickstart
To start running you need to do the following:

0. Download dotnet-runtime if not installed already (not required on Windows) (dotnet core 3.1)
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

## Examples
Example for creating a complete podcast - video with default parameters:

[![Blathering 132](http://img.youtube.com/vi/iKoKrhgiTMI/0.jpg)](https://www.youtube.com/watch?v=iKoKrhgiTMI "Blathering 132")

## More Details
All optional parameters and information about how to modify the appearance can be found in the Wiki: https://github.com/OleAlbers/Podcast2Video/wiki

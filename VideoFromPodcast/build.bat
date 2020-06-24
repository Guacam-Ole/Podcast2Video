del ./publish/* -R

dotnet publish VideoFromPodcast.sln --configuration Release --framework netcoreapp3.1 --output ./publish/win-x64 --self-contained false --runtime win-x64 --verbosity quiet
dotnet publish VideoFromPodcast.sln --configuration Release --framework netcoreapp3.1 --output ./publish/win-x86 --self-contained false --runtime win-x86 --verbosity quiet
dotnet publish VideoFromPodcast.sln --configuration Release --framework netcoreapp3.1 --output ./publish/linux-x64 --self-contained false --runtime linux-x64 --verbosity quiet
dotnet publish VideoFromPodcast.sln --configuration Release --framework netcoreapp3.1 --output ./publish/linux-arm --self-contained false --runtime linux-arm --verbosity quiet
dotnet publish VideoFromPodcast.sln --configuration Release --framework netcoreapp3.1 --output ./publish/linux-arm64 --self-contained false --runtime linux-arm64 --verbosity quiet
dotnet publish VideoFromPodcast.sln --configuration Release --framework netcoreapp3.1 --output ./publish/mac --self-contained false --runtime osx-x64 --verbosity quiet

cd ./publish/win-x64
tar -acf ../win-x64.zip *
cd ../win-x86
tar -acf ../win-x86.zip *
cd ../linux-x64
tar -acf ../linux-x64.tar.gz *
cd ../linux-arm
tar -acf ../linux-arm.tar.gz *
cd ../linux-arm64
tar -acf ../linux-arm64.tar.gz *
cd ../mac
tar -acf ../mac.tar.gz *


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends tesseract-ocr libleptonica-dev && apt-get clean
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-restore
WORKDIR /app
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
COPY Directory.Packages.props Directory.Build.props global.json NuGet.config ./
RUN dotnet restore ./Web/Web.csproj

FROM backend-restore AS backend-build
COPY . .
RUN dotnet build ./Web/Web.csproj --no-restore -c Release
RUN dotnet publish ./Web/Web.csproj --no-build -c Release -o /publish

FROM base AS final

COPY --from=backend-build /publish .

RUN ln -s /usr/lib/aarch64-linux-gnu/libtesseract.so.5 /usr/lib/aarch64-linux-gnu/libtesseract50.so
RUN ln -s /usr/lib/aarch64-linux-gnu/liblept.so.5 /usr/lib/aarch64-linux-gnu/libleptonica-1.82.0.so

RUN ln -s /usr/lib/aarch64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so
RUN ln -s /usr/lib/aarch64-linux-gnu/liblept.so.5 /app/x64/libleptonica-1.82.0.so
RUN ln -s /usr/lib/aarch64-linux-gnu/libdl.so.2 /app/libdl.so

USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Web.dll"]
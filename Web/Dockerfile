FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-restore
WORKDIR /app
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
COPY Directory.Packages.props Directory.Build.props global.json NuGet.config ./
RUN dotnet restore ./Web/Web.csproj

FROM backend-restore AS backend-build
COPY . .
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release --no-build -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update && \
        apt-get install -y --no-install-recommends \
        tesseract-ocr && \
        apt-get clean

WORKDIR /app
COPY --from=backend-build /publish .

USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "Web.dll"]
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY [".editorconfig", "."]

COPY ["DirectoryService/", "DirectoryService/"]

RUN dotnet restore "DirectoryService/src/DirectoryServices.Web/DirectoryServices.Web.csproj"

WORKDIR "/src/DirectoryService/src/DirectoryServices.Web"

RUN dotnet publish "./DirectoryServices.Web.csproj" \ 
    -c Release \ 
    -o /app/publish \ 
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5188
ENTRYPOINT ["dotnet", "DirectoryServices.Web.dll"]
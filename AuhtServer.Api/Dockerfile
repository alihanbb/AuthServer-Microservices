# Hata ayıklama kapsayıcınızı özelleştirme ve Visual Studio’nun daha hızlı hata ayıklama için görüntülerinizi derlemek üzere bu Dockerfile'ı nasıl kullandığı hakkında bilgi edinmek için https://aka.ms/customizecontainer sayfasına bakın.

# Bu aşama, VS'den hızlı modda çalıştırıldığında kullanılır (Hata ayıklama yapılandırması için varsayılan olarak ayarlıdır)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Bu aşama, hizmet projesini oluşturmak için kullanılır
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AuhtServer.Api/AuhtServer.Api.csproj", "AuhtServer.Api/"]
COPY ["Authserver.Application/Authserver.Application.csproj", "Authserver.Application/"]
COPY ["AuthServerDomain/AuthServer.Domain.csproj", "AuthServerDomain/"]
COPY ["Authserver.Infrıastructure/Authserver.Infrastructure.csproj", "Authserver.Infrıastructure/"]
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
RUN dotnet restore "AuhtServer.Api/AuhtServer.Api.csproj"
COPY . .
WORKDIR "/src/AuhtServer.Api"
RUN dotnet build "AuhtServer.Api.csproj" -c Release -o /app/build

# Bu aşama, son aşamaya kopyalanacak hizmet projesini yayımlamak için kullanılır
FROM build AS publish
RUN dotnet publish "AuhtServer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Bu aşama üretimde veya VS'den normal modda çalıştırıldığında kullanılır (Hata Ayıklama yapılandırması kullanılmazken varsayılan olarak ayarlıdır)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuhtServer.Api.dll"]
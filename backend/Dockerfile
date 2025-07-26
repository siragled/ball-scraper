FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

ENV DOTNET_SYSTEM_NET_HTTP_ENABLEACTIVITYPROPAGATION=0
EXPOSE 8080
ENTRYPOINT ["dotnet", "Wishlist.Api.dll"]

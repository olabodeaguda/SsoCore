FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && apt-get install -y nodejs

# Create SSL certificate
RUN mkdir /app && \
    openssl req -newkey rsa:2048 -nodes -keyout /app/certificate.key -x509 -days 365 -out /app/certificate.crt -subj "/C=NG/ST=Lagos/L=Lagos/O=Onnex/CN=onnexSoft.com"

ARG TOKEN_SIGN_KEY_SECRET
ENV TOKEN_SIGN_KEY_SECRET=$TOKEN_SIGN_KEY_SECRET

# Combine key and certificate into a .pfx file
RUN openssl pkcs12 -export -out /app/certificate.pfx -inkey /app/certificate.key -in /app/certificate.crt -passout pass:$TOKEN_SIGN_KEY_SECRET

# Optionally, you can remove the individual key and certificate files if needed
RUN rm /app/certificate.key /app/certificate.crt

COPY . .

RUN dotnet restore "src/UserAuthService.Provider/UserAuthService.Provider.csproj"

WORKDIR /src/UserAuthService.Provider
RUN npm install
RUN npm run css:build

RUN dotnet build -c Release -o /app/build

FROM build AS publish
WORKDIR /src/UserAuthService.Provider
RUN dotnet publish "UserAuthService.Provider.csproj" -c Release -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80

ARG Certificate__Path
ENV Certificate__Path=$Certificate__Path

COPY --from=publish /app/publish .
COPY --from=build /app/certificate.pfx /app/$Certificate__Path

ENTRYPOINT ["dotnet", "UserAuthService.Provider.dll"]

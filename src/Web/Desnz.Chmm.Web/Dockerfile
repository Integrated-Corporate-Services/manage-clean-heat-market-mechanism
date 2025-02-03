FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:80
EXPOSE 80
EXPOSE 443

FROM public.ecr.aws/docker/library/node:18-bullseye as node-build
WORKDIR /ClientApp
COPY Web/Desnz.Chmm.Web/ClientApp/package.json .
COPY Web/Desnz.Chmm.Web/ClientApp/package-lock.json .
RUN npm install -g npm@9.6.7
RUN npm install
COPY Web/Desnz.Chmm.Web/ClientApp .
RUN npm run build -- --configuration production

FROM node-build AS node-test
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list'
RUN apt-get update && apt-get install -yq google-chrome-stable
WORKDIR /ClientApp
RUN npm run test:prod

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["Web/Desnz.Chmm.Web/Desnz.Chmm.Web.csproj", "Web/Desnz.Chmm.Web/"]
COPY . .
WORKDIR /src/Web/Desnz.Chmm.Web
RUN dotnet build -c Release -o /app/build

FROM build AS publish
COPY --from=node-build /ClientApp/dist /src/Web/Desnz.Chmm.Web/wwwroot/
RUN dotnet publish --no-restore -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Desnz.Chmm.Web.dll"]

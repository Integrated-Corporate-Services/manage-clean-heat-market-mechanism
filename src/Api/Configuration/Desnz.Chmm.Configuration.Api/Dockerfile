FROM public.ecr.aws/lambda/dotnet:8 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["Api/Configuration/Desnz.Chmm.Configuration.Api/Desnz.Chmm.Configuration.Api.csproj", "Api/Configuration/Desnz.Chmm.Configuration.Api/"]
COPY ["Api/Configuration/Desnz.Chmm.Configuration.ApiTests/Desnz.Chmm.Configuration.ApiTests.csproj", "Api/Configuration/Desnz.Chmm.Configuration.ApiTests/"]
COPY ["Api/Configuration/Desnz.Chmm.Configuration.Common/Desnz.Chmm.Configuration.Common.csproj", "Api/Configuration/Desnz.Chmm.Configuration.Common/"]
COPY ["Api/Configuration/Desnz.Chmm.Configuration.UnitTests/Desnz.Chmm.Configuration.UnitTests.csproj", "Api/Configuration/Desnz.Chmm.Configuration.UnitTests/"]
COPY ["Common/Desnz.Chmm.ApiClients/Desnz.Chmm.ApiClients.csproj", "Common/Desnz.Chmm.ApiClients/"]
COPY ["Common/Desnz.Chmm.Common/Desnz.Chmm.Common.csproj", "Common/Desnz.Chmm.Common/"]
COPY ["Common/Desnz.Chmm.Common.Tests/Desnz.Chmm.Common.Tests.csproj", "Common/Desnz.Chmm.Common.Tests/"]
COPY ["Common/Desnz.Chmm.Testing.Common/Desnz.Chmm.Testing.Common.csproj", "Common/Desnz.Chmm.Testing.Common/"]
COPY . .
WORKDIR /src/Api/Configuration/Desnz.Chmm.Configuration.Api
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR /src/Api/Configuration/Desnz.Chmm.Configuration.UnitTests/
RUN dotnet test -v=normal
WORKDIR /src/Common/Desnz.Chmm.Common.Tests/
RUN dotnet test -v=normal

FROM build AS publish
RUN dotnet publish \
    --configuration Release \ 
    --runtime linux-x64 \
    --self-contained false \ 
    --output /app/publish \
    -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .
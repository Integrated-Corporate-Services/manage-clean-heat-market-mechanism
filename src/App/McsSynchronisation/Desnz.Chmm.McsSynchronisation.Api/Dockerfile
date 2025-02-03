FROM public.ecr.aws/lambda/dotnet:8 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.Api/Desnz.Chmm.McsSynchronisation.Api.csproj", "App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.Api/"]
COPY ["App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.ApiTests/Desnz.Chmm.McsSynchronisation.ApiTests.csproj", "App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.ApiTests/"]
COPY ["App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.Common/Desnz.Chmm.McsSynchronisation.Common.csproj", "App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.Common/"]
COPY ["App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.UnitTests/Desnz.Chmm.McsSynchronisation.UnitTests.csproj", "App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.UnitTests/"]
COPY ["Common/Desnz.Chmm.ApiClients/Desnz.Chmm.ApiClients.csproj", "Common/Desnz.Chmm.ApiClients/"]
COPY ["Common/Desnz.Chmm.Common/Desnz.Chmm.Common.csproj", "Common/Desnz.Chmm.Common/"]
COPY ["Common/Desnz.Chmm.Common.Tests/Desnz.Chmm.Common.Tests.csproj", "Common/Desnz.Chmm.Common.Tests/"]
COPY ["Common/Desnz.Chmm.Testing.Common/Desnz.Chmm.Testing.Common.csproj", "Common/Desnz.Chmm.Testing.Common/"]
COPY . .
WORKDIR /src/App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.Api
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR /src/App/McsSynchronisation/Desnz.Chmm.McsSynchronisation.UnitTests/
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
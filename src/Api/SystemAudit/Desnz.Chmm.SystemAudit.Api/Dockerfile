FROM public.ecr.aws/lambda/dotnet:8 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["Api/SystemAudit/Desnz.Chmm.SystemAudit.Api/Desnz.Chmm.SystemAudit.Api.csproj", "Api/SystemAudit/Desnz.Chmm.SystemAudit.Api/"]
COPY ["Api/SystemAudit/Desnz.Chmm.SystemAudit.ApiTests/Desnz.Chmm.SystemAudit.ApiTests.csproj", "Api/SystemAudit/Desnz.Chmm.SystemAudit.ApiTests/"]
COPY ["Api/SystemAudit/Desnz.Chmm.SystemAudit.Common/Desnz.Chmm.SystemAudit.Common.csproj", "Api/SystemAudit/Desnz.Chmm.SystemAudit.Common/"]
COPY ["Api/SystemAudit/Desnz.Chmm.SystemAudit.UnitTests/Desnz.Chmm.SystemAudit.UnitTests.csproj", "Api/SystemAudit/Desnz.Chmm.SystemAudit.UnitTests/"]
COPY ["Common/Desnz.Chmm.ApiClients/Desnz.Chmm.ApiClients.csproj", "Common/Desnz.Chmm.ApiClients/"]
COPY ["Common/Desnz.Chmm.Common/Desnz.Chmm.Common.csproj", "Common/Desnz.Chmm.Common/"]
COPY ["Common/Desnz.Chmm.Common.Tests/Desnz.Chmm.Common.Tests.csproj", "Common/Desnz.Chmm.Common.Tests/"]
COPY ["Common/Desnz.Chmm.Testing.Common/Desnz.Chmm.Testing.Common.csproj", "Common/Desnz.Chmm.Testing.Common/"]
COPY . .
WORKDIR /src/Api/SystemAudit/Desnz.Chmm.SystemAudit.Api
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR /src/Api/SystemAudit/Desnz.Chmm.SystemAudit.UnitTests/
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
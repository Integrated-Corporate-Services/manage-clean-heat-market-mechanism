FROM public.ecr.aws/lambda/dotnet:8 AS base
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY ["Api/Obligation/Desnz.Chmm.Obligation.Api/Desnz.Chmm.Obligation.Api.csproj", "Api/Obligation/Desnz.Chmm.Obligation.Api/"]
COPY ["Api/Obligation/Desnz.Chmm.Obligation.ApiTests/Desnz.Chmm.Obligation.ApiTests.csproj", "Api/Obligation/Desnz.Chmm.Obligation.ApiTests/"]
COPY ["Api/Obligation/Desnz.Chmm.Obligation.Common/Desnz.Chmm.Obligation.Common.csproj", "Api/Obligation/Desnz.Chmm.Obligation.Common/"]
COPY ["Api/Obligation/Desnz.Chmm.Obligation.UnitTests/Desnz.Chmm.Obligation.UnitTests.csproj", "Api/Obligation/Desnz.Chmm.Obligation.UnitTests/"]
COPY ["Common/Desnz.Chmm.ApiClients/Desnz.Chmm.ApiClients.csproj", "Common/Desnz.Chmm.ApiClients/"]
COPY ["Common/Desnz.Chmm.Common/Desnz.Chmm.Common.csproj", "Common/Desnz.Chmm.Common/"]
COPY ["Common/Desnz.Chmm.Common.Tests/Desnz.Chmm.Common.Tests.csproj", "Common/Desnz.Chmm.Common.Tests/"]
COPY ["Common/Desnz.Chmm.Testing.Common/Desnz.Chmm.Testing.Common.csproj", "Common/Desnz.Chmm.Testing.Common/"]
COPY . .
WORKDIR /src/Api/Obligation/Desnz.Chmm.Obligation.Api
RUN dotnet build -c Release -o /app/build

FROM build AS test
WORKDIR /src/Api/Obligation/Desnz.Chmm.Obligation.UnitTests/
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
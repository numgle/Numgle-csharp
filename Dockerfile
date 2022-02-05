FROM mcr.microsoft.com/dotnet/core/sdk:3.1.416-buster AS build
WORKDIR /src
COPY Numgle.csproj .
RUN dotnet restore ./Numgle.csproj
COPY . .
RUN dotnet build "Numgle.csproj" -c Release -o /app/build
RUN dotnet publish "Numgle.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.9-buster-slim
WORKDIR /app
COPY --from=build /app/publish .
CMD [ "./Numgle" ]
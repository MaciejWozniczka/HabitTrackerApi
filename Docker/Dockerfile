FROM mcr.microsoft.com/dotnet/sdk:8.0
LABEL author="maciej.wozniczka@outlook.com"

RUN rm -rf /var/lib/apt/lists/*
RUN apt-get clean
RUN apt-get update -o Acquire::CompressionTypes::Order::=gz

RUN apt-get update
RUN apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

COPY . /app
WORKDIR /app
RUN dotnet build -c Release
RUN dotnet publish HabitTrackerApi -c Release

WORKDIR /app/HabitTrackerApi/bin/Release/net8.0/publish
CMD ./HabitTrackerApi
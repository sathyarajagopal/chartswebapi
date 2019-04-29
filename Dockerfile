# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:latest
WORKDIR /app
COPY ["ChartsMicroservice.csproj", "./"]
RUN dotnet restore "./ChartsMicroservice.csproj"
COPY . .
RUN dotnet publish "ChartsMicroservice.csproj" -c Release -o /app/publish
RUN dir
WORKDIR /app/publish
RUN dir
EXPOSE 5000/tcp
ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "ChartsMicroservice.dll"]
## Install Docker

Docker is a platform that enables you to combine an app plus its configuration and dependencies into a single, independently deployable unit called a container.

### Download and Install

You'll be asked to register for Docker Store before you can download the installer.

By default, Docker will use Linux Containers on Windows. Leave this configuration settings as-is when prompted in the installer.

[Get Docker for Windows](https://hub.docker.com/editions/community/docker-ce-desktop-windows)

### Check that Docker is ready to use

Once you've installed, open a new command prompt and run the following command:

    docker --version

If the command runs, displaying some version information, then Docker is successfully installed.

## Add Docker metadata

To run with Docker Image you need a Dockerfile — a text file that contains instructions for how to build your app as a Docker image. A docker image contains everything needed to start an instance of your app.

### Return to app directory

Since you opened a new command prompt in the previous step, you'll need to return to the directory you created your project in.

    cd ChartsMicroservice

### Add a DockerFile

To create an empty Dockerfile, run the following command:

    echo . > Dockerfile

Please note that if you are using VS Code and you ran this command in VS Code terminal you may get parse error in line 1 of Dockerfile when building image. In that case press Ctrl+Shift+P to open Command palette in VS Code and then make use of Docker:Add Docker Files to Workspace command to generate Dockerfile from Visual Code.

Open the Dockerfile in the text editor of your choice and replace the contents with the following:

#### Windows container:

    FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
    WORKDIR /app
    EXPOSE 80

    FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
    WORKDIR /src
    COPY ["ChartsMicroservice.csproj", "./"]
    RUN dotnet restore "./ChartsMicroservice.csproj"
    COPY . .
    WORKDIR "/src/."
    RUN dotnet build "ChartsMicroservice.csproj" -c Release -o /app

    FROM build AS publish
    RUN dotnet publish "ChartsMicroservice.csproj" -c Release -o /app

    FROM base AS final
    WORKDIR /app
    COPY --from=publish /app .
    ENTRYPOINT ["dotnet", "ChartsMicroservice.dll"]

#### Linux container:

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

## Switch to Linux containers

Right click on Docker Desktop whale icon in systems tray and select "Switch to Linux containers..." only if the whale is currently running in windows container.

## Remove existing Docker image

You can run the following command to see a list of all images available on your machine. This is to check whether chartsmicroservice image already exists.

    docker image ls

### Delete the image

If chartsmicroservice image exists then delete the image using the following command:

    docker image rm -f chartsmicroservice

## Create Docker image

Run the following command:

    docker build -t chartsmicroservice .

The docker build command uses the information from your Dockerfile to build a Docker image.

The -t parameter tells it to tag (or name) the image as chartsmicroservice.
The final parameter tells it which directory to find the Dockerfile in (. specifies the current directory).

You can run the following command to see a list of all images available on your machine, including the one you just created.

    docker image ls

## Run Docker image

A Docker container is an instance of your app, created from the definition and resources from your Docker image.

To run your app in a container, run the following command:

    docker run -it --rm -p 8080:5000 chartsmicroservice

Once the command completes, browse to http://localhost:8080/api/charts or http://localhost:8080/swagger.

## How to make HTTP requests from one container to another container?

Find out all running containers using the below command:

    docker ps -a

Run the below command to get the IP address of node-export-server docker container to which we are going to make HTTP requests. Replace the <CONTAINER_ID> with the one that we got in previous step.

    docker inspect --format '{{ .NetworkSettings.IPAddress }}' <CONTAINER ID>

The resulting IP address should be given as value in appSettings.json's "ChartExportServerUrl" property.

## Push to docker hub

Docker Hub is a central place to upload Docker images. Many products, including Microsoft, can create containers based on images in Docker Hub.

### Login to Docker Hub

In your command prompt, run the following command:

    docker login

Use the username and password created when you downloaded Docker. You can visit the Docker Hub website to reset your password if needed.

### Upload image to Docker Hub

Re-tag (rename) your Docker image under your username and push it to Docker Hub using the following commands:

    docker tag chartsmicroservice [YOUR DOCKER USERNAME]/chartsmicroservice
    docker push [YOUR DOCKER USERNAME]/chartsmicroservice

## Remove Docker Containers

Run the below command to list all containers in your machine.

    docker container ls --all

Capture the container name from the previous command and run the below command to stop running containers.

    docker container stop <ctr1> <ctr2>

Now run the below command to remove the containers.

    docker container rm <ctr1> <ctr2>

## Prune unused docker objects

    docker system prune -a

Congratulations! You've successfully created a small, independent Charts Microservice using ASP.NET Core Webapi that can be deployed and scaled using Docker containers.

These are the fundamental building blocks to get an ASP.NET Core web api into a Docker container.

## References

1. https://dotnet.microsoft.com/learn/web/aspnet-microservice-tutorial/intro
2. https://github.com/domaindrivendev/Swashbuckle/issues/258
3. https://stackoverflow.com/questions/40675162/install-a-nuget-package-in-visual-studio-code
4. https://aka.ms/containercompat
5. https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.2&tabs=visual-studio-code
6. https://docs.docker.com/engine/examples/dotnetcore/
7. https://stormpath.com/blog/tutorial-deploy-asp-net-core-on-linux-with-docker
8. http://networkstatic.net/10-examples-of-how-to-get-docker-container-ip-address/
#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

#FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
#WORKDIR /src
#COPY ["WebApi/CLF.WebApi/CLF.WebApi.csproj", "WebApi/CLF.WebApi/"]
#RUN dotnet restore "WebApi/CLF.WebApi/CLF.WebApi.csproj"
#COPY . .
#WORKDIR "/src/WebApi/CLF.WebApi"
#RUN dotnet build "CLF.WebApi.csproj" -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish "CLF.WebApi.csproj" -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "CLF.WebApi.dll"]

## 声明使用的基础镜像
#FROM microsoft/dotnet:latest
## 将本地dotnet core 应用拷贝到 容器 /app/ 目录下
#COPY bin/Debug/netcoreapp2.0/publish/ /app/
## 设置工作目录
#WORKDIR /app
## 设置导出端口
#EXPOSE 5000/tcp
## 指定应用入口点
#ENTRYPOINT ["dotnet", "/app/NetCore.dll"]


FROM FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "CLF.WebApi.dll"]
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

## ����ʹ�õĻ�������
#FROM microsoft/dotnet:latest
## ������dotnet core Ӧ�ÿ����� ���� /app/ Ŀ¼��
#COPY bin/Debug/netcoreapp2.0/publish/ /app/
## ���ù���Ŀ¼
#WORKDIR /app
## ���õ����˿�
#EXPOSE 5000/tcp
## ָ��Ӧ����ڵ�
#ENTRYPOINT ["dotnet", "/app/NetCore.dll"]


FROM FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "CLF.WebApi.dll"]
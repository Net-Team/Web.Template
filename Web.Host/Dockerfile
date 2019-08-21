FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview6
COPY app /app
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Web.Host.dll"]
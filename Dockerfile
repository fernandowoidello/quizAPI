# Usando a imagem do SDK do .NET como base
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Define o diretório de trabalho
WORKDIR /app

# Copia o arquivo de solução e restaura dependências
COPY *.sln ./
COPY QuizApi/*.csproj QuizApi/
RUN dotnet restore

# Copia o restante dos arquivos e compila a aplicação
COPY . .
RUN dotnet publish -c Release -o out

# Usando a imagem do runtime do .NET para o contêiner final
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "QuizApi.dll"]

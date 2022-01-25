## Criar novo projeto

$ dotnet new webapi -n Catalog

## Fazer os certificados serem aceitos

$ dotnet dev-certs https --trust

## Editando o arquivo launch.json (criado pelo VSCODE) comente a segunte sessão para não abrir mais a janela do navegador quando executar a app

            // Enable launching a web browser when ASP.NET Core starts. For more information: https://aka.ms/VSCode-CS-LaunchJson-WebBrowser
            // "serverReadyAction": {
            //     "action": "openExternally",
            //     "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            // },

## Editando o arquivo task.json, editar a sessão TASKS, BUILD acrescente o seguinte código para tornar a opção de BUILD como padrão

            "group": {
                "kind": "build",
                "isDefault": true
            }

## Instalar Driver do MongoDB

$ dotnet add package MongoDB.Driver

## Executando imagem docker do mongoDB para usar na aplicação

$ docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
$ docker ps | Select-String -Pattern "mongo" (verificar container rodando no Windows)

## Incluindo autenticação no Mongo...

1 - Excluir o volume:

$ docker ps | Select-String -Pattern "mongo"
$ docker stop mongo
$ docker volume ls
$ docker volume rm mongodbdata

$ docker run -d --rm --name mongo_catalog -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass1word# mongo

## Inicializar o DOTNET Secret Manager

$ dotnet user-secrets init

# Conferir se o CSPROJ recebeu essa tag <UserSecretsId>c0dcd14f-f674-4ba1-9e3f-d4da436947c7</UserSecretsId>

# Então, baseado nas propriedades do Mongo no arquivo appsettings.json criaremos o SECRET para a senha do banco de dados

$ dotnet user-secrets set MongoDbSettings:Password Pass1word#

## Health Checks

# Executar mudanças no startup já retornam o desejado, mas adicionando o package a seguir verificaremos a saúde do database

$ dotnet add package AspNetCore.HealthChecks.MongoDb

## Containerizar a APP

# Vide docker file

# Criar a docker networks

$ docker network create net5tutorial

# Parar o container Mongo para faze-lo juntar-se a rede usada no APP

$ docker run -d --rm --name mongo_catalog -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD=Pass1word# --network=net5tutorial mongo

## Subindo container e sobrescrevendo as configurações do appsettings.json por variaveis de ambiente

$ docker run -it --rm -p 8080:80 -e MongoDbSettings:Host=mongo_catalog -e MongoDbSettings:Password=Pass1word# --network=net5tutorial catalog:v1

mongodb://mongoadmin:Pass1word#@localhost:27017

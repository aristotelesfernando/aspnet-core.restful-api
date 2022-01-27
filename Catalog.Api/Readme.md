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

## Configurar cluster kubernets local

$ kubectl config current-context

## Criando Secret no kubernets para armazenar a senha do mongodb

$ kubectl create secret generic catalog-secrets --from-literal=mongodb-password='Pass1word#'

## Aplicando o deployment/service

$ kubectl apply -f .\catalog.yaml

## Visualizando a situação do deployment

$ kubectl get deployments
NAME READY UP-TO-DATE AVAILABLE AGE
catalog-deployment 1/1 1 1 5m42s

## Visualizando a situação dos PODs criados

$kubectl get pods
NAME READY STATUS RESTARTS AGE
catalog-deployment-5757bd856b-6wpcw 1/1 Running 0 5m46s

## Visualizando logs do POD

$ kubectl logs catalog-deployment-5757bd856b-6wpcw
info: Microsoft.Hosting.Lifetime[0]
Now listening on: http://[::]:80
info: Microsoft.Hosting.Lifetime[0]
Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
Content root path: /app
---> ############################################################################### Falhou aqui
fail: Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService[103]
---> ###############################################################################
Health check mongodb with status Unhealthy completed after 3007.4089ms with message '(null)'
System.OperationCanceledException: The operation was canceled.
at System.Threading.CancellationToken.ThrowOperationCanceledException()
at MongoDB.Driver.Core.Clusters.Cluster.WaitForDescriptionChangedHelper.HandleCompletedTask(Task completedTask)
at MongoDB.Driver.Core.Clusters.Cluster.WaitForDescriptionChangedAsync(IServerSelector selector, ClusterDescription description, Task descriptionChangedTask, TimeSpan timeout, CancellationToken cancellationToken)
at MongoDB.Driver.Core.Clusters.Cluster.SelectServerAsync(IServerSelector selector, CancellationToken cancellationToken)
at MongoDB.Driver.MongoClient.AreSessionsSupportedAfterServerSelectionAsync(CancellationToken cancellationToken)
at MongoDB.Driver.MongoClient.AreSessionsSupportedAsync(CancellationToken cancellationToken)
at MongoDB.Driver.MongoClient.StartImplicitSessionAsync(CancellationToken cancellationToken)
at MongoDB.Driver.MongoClient.UsingImplicitSessionAsync[TResult](Func`2 funcAsync, CancellationToken cancellationToken)
at HealthChecks.MongoDb.MongoDbHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)

# POD não está funcionando pq não há banco de dados rodando

## Agora adcionando o statefulset do MongoDB

$ kubectl apply -f .\mongodb.yaml
statefulset.apps/mongodb-statefulset created
service/mongodb-service created

# Agora API e Mongo rodando no k8s

## Escalando os PODs

$ kubectl scale deployments/catalog-deployment --replicas=3

## Incluindo mensagens de LOG na api e logo em seguida gerando uma nova versão da imagems docker. Será necessário atualizar a imagem no k8s

$ docker build -t catalog:v2 .
$ docker push afernandojr/catalog:v2

# Alterar o catalog.yaml para usar a V2 na tag "image:"

## Criar uma pasta nova e mover projeto para essa pasta, executar build no projeto para ver se está ok, buildar novamente a imagem docker para testar

## Criar um projeto de testes com xunit no mesmo nivel de pasta do projeto Catalog.Api

$ dotnet new xunit -n Catalog.UnitTests

## Criar na raiz o arquivo "build.proj", com este arquivo será possível buildar os dois projetos...

# No arquivo tasks.json, substituir essa linha "${workspaceFolder}/Catalog.Api/Catalog.Api.csproj" por esta "${workspaceFolder}/build.proj", e testar o build

## Adicionar a referencia da API no projeto de testes (acessar a pasta do projeto de testes)

$ dotnet add reference ..\Catalog.Api\Catalog.Api.csproj

## Adiciono o package no projeto de testes

$ dotnet add package Microsoft.Extensions.Logging.Abstractions
$ dotnet add package moq

## Após escrever os testes, eles poderão ser executados de dentro do VSCODE ou através do comando:

$ dotnet test

## É possivel usar uma extension do VSCODE para executar/analisar os testes: ".net core test explorer"

## Para facilitar o teste de objetos com mutiplas propriedades instalaremos o package:

$dotnet add package FluentAssertions

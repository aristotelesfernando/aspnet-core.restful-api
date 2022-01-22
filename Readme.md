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

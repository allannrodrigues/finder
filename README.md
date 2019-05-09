# finder
Finder é um executavel que efetua busca de valores em arquivos txt ou csv.
1. Informe o path dos arquivos.
2. Informe o valor a ser localizado e aguarde a varredura.
Ao encontrar o valor será exibido na tela a linha.

## passos para criação do executavel

1. baixar o .net core versão 2.0 ou superior [dotnet](https://dotnet.microsoft.com/download)
2. após a instalação execute o comado "dotnet restore"
3. após o restore dos packages execute o comando "dotnet publish -c Release -r win10-x64"
4. o executável será gerado dentro da pasta "src\Finder\bin\Release\netcoreapp2.2\win10-x64"

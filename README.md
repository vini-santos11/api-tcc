# PROJETO DE CONCLUSÃO DE CURSO

## API - CONTROLE DE ESTOQUE PARA PEQUENAS/MÉDIAS EMPRESAS

O projeto disponibila uma API Restful que permite acesso a requisições para utilização do front-end ou de outras integrações externas.
A API foi desenvolvida em .NET na versão 5.0, utilizando como micro-ORM o DAPPER para persistencia de dados no banco de dados (o qual foi escolhido o MYSQL). E na arquitetura de software, utilizamos o Repository Pattern.
A Autenticação do sistema foi desenvolvido utilizando Bearer e JWT através de packages do NUGET, o qual garantiu algumas funcionalidades para controle de segurança, e criação de features como Roles.
As Roles ja implementadas são ADMINISTRADOR e USUÁRIO, dessas, apenas a ADMINISTRADOR foi utilizada para o front-end, onde criamos o portal para gerenciamento do estoque. Com essa feature, é possível implementar outros sistemas, tal qual Marketplace, E-Commerce entre outros.
A passagem de parametros, tanto exeterna, quanto intermanente foi utilizado o Design Pattern CQRS.

A documentaçao completa, com dados dos endpoints esta feita em Swagger.

## Métodos
Requisições para a API devem seguir os padrões:
| Método | Descrição |
|---|---|
| `GET` | Retorna informações de um ou mais registros. |
| `POST` | Utilizado para criar um novo registro. |
| `PUT` | Atualiza dados de um registro ou altera sua situação. |
| `DELETE` | Remove um registro do sistema. |


## Respostas

| Código | Descrição |
|---|---|
| `200` | Requisição executada com sucesso (success).|
| `400` | Erros de validação ou os campos informados não existem no sistema.|
| `401` | Dados de acesso inválidos. Unauthorized|
| `404` | Registro pesquisado não encontrado (Not found).|
| `405` | Método não implementado.|
| `429` | Número máximo de requisições atingido. (*aguarde alguns segundos e tente novamente*)|

## Listar
As ações de `listar` permitem o envio dos seguintes parâmetros:

| Parâmetro | Descrição |
|---|---|
| `filtro` | Filtra dados pelo valor informado. |
| `page` | Informa qual página deve ser retornada. |

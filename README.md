# Documentação da API

## Introdução
Esta API permite: 
- Cadastro, atualização e exclusão e consulta de usuários.
- Cadastro, atualização e exclusão e consulta de pedidos de usuários.

## Tecnologias Utilizadas
- ASP.NET Core 8
- Dapper
- Authentication Jwt
- AutoMapper
- NLog
- Redis
- SQL Server

## Estrutura do Projeto
- **Controllers**: Recebe requisições HTTP e retorna respostas as adequadas
- **Configurations**: Configuração de serviços incluídos no program (Autenticação JWT e Swagger)
- **Exceptions**: Classes para tratamento de exceções (Sql Exception e Json Exception)
- **Infrastructure**: Configuração de conexão com a base de dados
- **Middlewares**: Middlewre para tratamento de exceções da api
- **Services**: Contém a lógica de negócios
- **Repositories**: Contém a lógica de acesso a dados
- **Models**: Representa as entidades do domínio
- **DTOs**: Objetos de transferência de dados

## Endpoints da API
### Usuários
- **POST /usuarios**
  - Cadastra um novo usuário.

- **GET /usuarios/{id}**
  - Retorna uma usuário pelo ID.

- **PUT /usuarios**
  - Atualiza os dados de um usuário.

- **DELETE /usuarios/{id}**
  - Remove um usuário.

### Pedidos
- **POST /pedidos**
  - Cadastra um novo pedido.

- **GET /pedidos/{id}**
  - Retorna todos os pedidos de um usuário.

- **PUT /pedidos**
  - Atualiza um pedido.

- **DELETE /pedidos/{id}**
  - Remove um pedido.

## Requisitos de Autenticação
A API utiliza tokens JWT para autenticação.

## Tratamento de Erros
Erros são retornados com status code apropriado e mensagem padronizada.

## Paginação
A listagem de pedidos suporta paginação através dos parâmetros `page` e `pageSize`.

## Instruções de Configuração e Execução
1. Abrir um terminal
2. Clonar o repositório para máquina local.
   - `git clone https://github.com/dev-marchese/pedidos-api.git`
3. Navegar até a pasta raiz do projeto.
4. Executar `dotnet restore`.
5. Executar `dotnet run`.
6. Acessar a url http://localhost:5236/swagger/index.html

## Gerando Token JWT
1. `/api/v1/pedidos`:
- **Exemplo de Requisição**: 
    ```json
    {
      "userName": "admin",
      "password": "admin"
    }
    ```
- userName e passWord estão fixos dentro da aplicação, então devem ser mantidos na requisição conforme exemplo acima. Após gerar o token, ele deve ser inserido da opção `Authorize` no topo da página para que as requisições para os demais endpoints sejam aceitas.

## Cache com Redis

O endpoint `/api/v1/pedidos/{id}` armazena a consulta executada em cache. Para configurar a conexão com um serviço redis local, basta alterar o ip no arquivo appsettings.

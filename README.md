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
- userName e passWord estão fixos dentro da aplicação, então devem ser mantidos na requisição conforme exemplo acima. Após gerar o token, ele deve ser inserido da opção `Authorize` no topo da página para que as requisições para os demais endpoints sejam autorizadas.

## Cache com Redis

1. A consulta contida no endpoint `/api/v1/pedidos/{id}` armazena o resultado em cache. Para configurar a conexão com um serviço redis local, basta alterar o ip no arquivo `appsettings.json`.
- **Exemplo de Configuração**:
```json
"Redis": {
  "IP": "0.0.0.0:0000",
  "InstanceName": "PedidoInstance:"
}
```
2. Caso o redis não esteja configurado, o endpoint retornará a consulta normalmente, porém, ela não será armazenada em cache.

## Banco de Dados

Executar o arquivo `create-database.sql` para criação da base de dados e das tabelas.

## Resposta as perguntas

1. Dado o seguinte código, o que será impresso no console? Explique sua resposta.

## Resposta às perguntas

1. Dado o seguinte código, o que será impresso no console? Explique sua resposta.

```csharp
var lista = new List { 1, 2, 3, 4, 5 };
//O correto é: var lista = new List<int> { 1, 2, 3, 4, 5 }; é necessário indicar um tipo para a lista
var resultado = lista.Where(x => x % 2 == 0).Select(x => x * 2);
Console.WriteLine(string.Join(', ', resultado)); //
O correto: string.Join(',', resultado) ou string.Join(", ", resultado) para se usar separador de aspas simples é aceito somente um caracter. Remover o espaço após a vírgula ou trocar para aspas duplas


Resposta: O código atual iria resultar em erro. Porém, a lógica do algoritmo acima consiste em multiplicar por 2 os números pares da lista. Seriam impressos 4, 8

```


2. Se você rodar a query abaixo no banco de dados, o que acontecerá?

```csharp
SELECT * FROM Usuarios WHERE Nome LIKE '%_Silva%'

Resposta: retornará registros que contenham a palavra `Silva` em qualquer posição no campo `Nome`

```

3. Considere o código abaixo. Qual será a saída e por quê?

```csharp
public class Exemplo
{
  public string Nome { get; set; }
  public Exemplo(string nome) {
    Nome = nome ?? "Sem Nome";
  }
}

var obj = new Exemplo(null);
Console.WriteLine(obj.Nome);

Resposta: O resultado retornado será `Sem Nome`, pois o valor passado na instância da classe Exemplo foi null. Sendo null, o retorno é `Sem Nome`

```
   
4. O que está errado no código abaixo e como corrigir?

```csharp

public async Task<List<Usuario>> BuscarUsuarios()
{
  using (var db = new MeuDbContext())
  {
    return db.Usuarios.ToListAsync();
  }
}

```



Resposta: o correto é `return await db.Usuarios.ToListAsync();`, pois o método `BuscarUsuarios` é assíncrono. Ou seja, a função deverá aguardar o retorno do método ToListAsync().

```

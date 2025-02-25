using Dapper.Contrib.Extensions;
using Dapper;
using PedidoAPI.Infrastructure;
using Microsoft.Data.SqlClient;
using PedidoAPI.Exceptions;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace PedidoAPI.Repositories
{
	public abstract class BaseRepository<TEntity> where TEntity : class
	{
		protected readonly DatabaseConnection _dbConnection; 
		private readonly ILogger _logger;
		private readonly IDistributedCache _cache;

		public BaseRepository(DatabaseConnection dbContext, ILogger logger, IDistributedCache cache)
		{
			_dbConnection = dbContext;
			_logger = logger;
			_cache = cache;
		}


		#region Query


		public virtual async Task<T?> QuerySingle<T>(string sql, object? parameters = null)
		{
			try
			{
				using (var connection = await _dbConnection.CreateConnectionAsync())
				{
					return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
				}
			}
			catch (Exception ex)
			{
				string formattedSql = GetFormattedSql(sql, parameters);
				_logger.LogError(ex, "Erro ao executar consulta. SQL: {Sql}", formattedSql);

				return Activator.CreateInstance<T>();
			}
		}

		/// <summary>
		/// Executa uma consulta SQL e armazena o resultado em cache.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sql">Sql da consulta</param>
		/// <param name="parameters">Parâmetros da consulta sql</param>
		/// <param name="cacheKey">Chave de indentificação para armazenar e recuperar os registros em cache</param>
		/// <param name="cacheExpirationSeconds">Tempo de expiração do cache em segundos</param>
		/// <returns>Lista de resultados com o tipo T da consulta</returns>
		public virtual async Task<List<T>> Query<T>(string sql, object? parameters = null, string? cacheKey = null, int cacheExpirationSeconds = 60)
		{
			try
			{
				if (!string.IsNullOrEmpty(cacheKey))
				{
					try
					{
						var cachedData = await _cache.GetStringAsync(cacheKey);
						if (!string.IsNullOrEmpty(cachedData))
						{
							return JsonConvert.DeserializeObject<List<T>>(cachedData) ?? [];
						}
					}
					catch (Exception cex)
					{
						_logger.LogWarning(cex, "Erro ao tentar estabelecer conexão com o cache.");
					}
				}

				using (var connection = await _dbConnection.CreateConnectionAsync())
				{
					var result = await connection.QueryAsync<T>(sql, parameters);

					if (!string.IsNullOrEmpty(cacheKey) && result != null)
					{
						try
						{
							var cacheOptions = new DistributedCacheEntryOptions
							{
								AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheExpirationSeconds)
							};
							await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(result), cacheOptions);
						}
						catch (Exception cex)
						{
							_logger.LogWarning(cex, "Erro ao armazenar resultado de consulta no cache.");
						}
					}

					return result?.ToList() ?? [];
				}
			}
			catch (Exception ex)
			{
				string formattedSql = GetFormattedSql(sql, parameters);
				_logger.LogError(ex, "Erro ao executar consulta. SQL: {Sql}", formattedSql);

				return [];
			}
		}


		#endregion


		#region Framework


		/// <summary>
		/// Insere um novo registro no banco de dados.
		/// </summary>
		/// <param name="entity">A entidade a ser inserida </param>
		/// <returns>Retorna a entidade inserida</returns>
		/// <exception cref="GetMessageException">
		/// Lança uma exceção customizada com uma mensagem específica em caso de erros de violação de chave única (email em Usuário) ou estrangeira (UsuarioId em Pedido).
		/// </exception>
		public virtual async Task<TEntity?> Insert(TEntity entity)
		{
			try
			{
				using (var connection = await _dbConnection.CreateConnectionAsync())
				{
					var id = await connection.InsertAsync(entity);
					entity = await connection.GetAsync<TEntity>(id);
					return entity;
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number == 2601) //Violação de chave única
				{
					var match = Regex.Match(ex.Message, @"index '(\w+)'");
					string fieldName = match.Success ? match.Groups[1].Value : "campo desconhecido";

					_logger.LogError(ex, "Erro ao executar Insert: {fieldName} já existe.", fieldName);
					throw new GetMessageException($"O valor informado já está cadastrado para o campo '{fieldName}'.");
				}
				else if (ex.Number == 547) //Violação de chave estrangeira
				{
					var regex = new Regex(@"tabela\s+""(?<tableName>[^""]+)""[^\']+column\s+'(?<columnName>[^']+)'", RegexOptions.IgnoreCase); 
					var match = regex.Match(ex.Message);

					string tableName = match.Success ? match.Groups["tableName"].Value : "tabela desconhecida";
					string columnName = match.Success ? match.Groups["columnName"].Value : "coluna desconhecida";

					_logger.LogError(ex, "Erro ao executar Insert: chave estrangeira inválida. Tabela: {tableName}, Coluna: {columnName}", tableName, columnName);
					throw new GetMessageException($"O valor informado não foi encontrado na tabela '{tableName}', coluna '{columnName}'.");
				}
				else //Outros erros
				{
					_logger.LogError(ex, "Erro ao executar Insert.");
					throw new GetMessageException($"Ocorreu um erro ao tentar inserir o registro na base de dados.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao executar Insert.");
				return null;
			}
		}

		/// <summary>
		/// Atualiza um registro no banco de dados.
		/// </summary>
		/// <param name="entity">A entidade a ser atualizada </param>
		/// <returns>bool</returns>
		/// <exception cref="GetMessageException">
		/// Lança uma exceção customizada com uma mensagem específica em caso de erros de violação de chave única (email em Usuário) ou estrangeira (UsuarioId em Pedido).
		/// </exception>
		public virtual async Task<bool> Update(TEntity entity)
		{
			try
			{
				using (var connection = await _dbConnection.CreateConnectionAsync())
				{
					return await connection.UpdateAsync(entity);
				}
			}
			catch (SqlException ex)
			{
				if (ex.Number == 2601) //Violação de chave única
				{
					var match = Regex.Match(ex.Message, @"index '(\w+)'");
					string fieldName = match.Success ? match.Groups[1].Value : "campo desconhecido";

					_logger.LogError(ex, "Erro ao executar Update: {fieldName} já existe.", fieldName);
					throw new GetMessageException($"O valor informado já está cadastrado para o campo '{fieldName}'.");
				}
				else if (ex.Number == 547) //Violação de chave estrangeira
				{
					var regex = new Regex(@"tabela\s+""(?<tableName>[^""]+)""[^\']+column\s+'(?<columnName>[^']+)'", RegexOptions.IgnoreCase);
					var match = regex.Match(ex.Message);

					string tableName = match.Success ? match.Groups["tableName"].Value : "tabela desconhecida";
					string columnName = match.Success ? match.Groups["columnName"].Value : "coluna desconhecida";

					_logger.LogError(ex, "Erro ao executar Update: chave estrangeira inválida. Tabela: {tableName}, Coluna: {columnName}", tableName, columnName);
					throw new GetMessageException($"O valor informado não foi encontrado na tabela '{tableName}', coluna '{columnName}'.");
				}
				else //Outros erros
				{
					_logger.LogError(ex, "Erro ao executar Update.");
					throw new GetMessageException($"Ocorreu um erro ao tentar inserir o registro na base de dados.");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao executar Update.");
				return false;
			}
		}

		public virtual async Task<bool> Delete(TEntity entity)
		{
			try
			{
				using (var connection = await _dbConnection.CreateConnectionAsync())
				{
					return await connection.DeleteAsync(entity);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao executar Delete.");
				return false;
			}
		}

		public virtual async Task<TEntity?> GetById(int id)
		{
			try
			{
				using (var connection = await _dbConnection.CreateConnectionAsync())
				{
					var result = await connection.GetAsync<TEntity>(id);
					return result;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao executar consulta.");
				return null;
			}
		}

		public virtual async Task<List<TEntity>> GetAll()
		{
			try
			{
				using (var connection = await _dbConnection.CreateConnectionAsync())
				{
					var result = await connection.GetAllAsync<TEntity>();
					return result.ToList();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao executar consulta");
				return [];
			}
		}


		#endregion


		#region Parameters

		/// <summary>
		/// Substitui os parâmetros de uma consulta sql pelos seus valores correspondentes
		/// </summary>
		/// <param name="sql">A consulta sql com parâmetros</param>
		/// <param name="parameters">Os parâmetros a serem substituídos na consulta</param>
		/// <returns>Retorna a string da consulta sql com os valores correspondentes dos parâmetros.</returns>
		private string GetFormattedSql(string sql, object? parameters)
		{
			sql = sql.Replace("\n", " ").Replace("\r", " ").Trim();
			if (parameters is DynamicParameters dynamicParameters)
			{
				foreach (var paramName in dynamicParameters.ParameterNames)
				{
					var value = dynamicParameters.Get<object>(paramName);
					string formattedValue =
						value switch
						{
							string s => $"'{s}'",
							null => "NULL",
							_ => value?.ToString() ?? "NULL"
						};

					sql = sql.Replace($"@{paramName}", formattedValue);
				}
			}

			return sql;
		}


		#endregion
	}
}

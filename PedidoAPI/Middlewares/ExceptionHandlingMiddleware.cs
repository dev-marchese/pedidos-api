using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using PedidoAPI.Exceptions;
using System.Text.Json;
using System.Threading.Tasks;

namespace PedidoAPI.Middlewares
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				Console.WriteLine("Processing request...");
				await _next(context);
			}
			catch (GetMessageException ex)
			{
				//Trata exceções específicas do repositório, retornando status 400 e a mensagem de erro.
				context.Response.StatusCode = 400;
				context.Response.ContentType = "application/json";
				var response = new { success = false, message = ex.Message };
				await context.Response.WriteAsJsonAsync(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("exception...");

				//Captura exceções gerais, loga o erro e retorna um status 500.
				var endpoint = context.GetEndpoint();
				var controller = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

				var controllerName = controller?.ControllerName;
				var actionName = controller?.ActionName;

				var ipAddress = context?.Connection?.RemoteIpAddress?.ToString();

				//Loga detalhes do erro, incluindo o IP do cliente, o controlador e a ação.
				_logger.LogError(ex, "Erro inesperado na aplicação | {ipAddress} | Controller: {controller} | Action: {action}", ipAddress, controllerName, actionName);

				context.Response.StatusCode = 500;
				context.Response.ContentType = "application/json";
				var response = new { success = false, message = "Houve uma falha interna para processar sua solicitação." };
				await context.Response.WriteAsJsonAsync(response);
			}
		}
	}

	public static class ExceptionHandlingMiddlewareExtensions
	{
		public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<ExceptionHandlingMiddleware>();
		}
	}
}

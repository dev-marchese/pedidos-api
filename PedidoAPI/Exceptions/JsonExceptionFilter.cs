using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace PedidoAPI.Exceptions
{
	public class JsonExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			if (context.Exception is JsonException jsonException)
			{
				context.Result = new BadRequestObjectResult(new
				{
					success = false,
					message = "Erro de formatação JSON.",
					errors = new
					{
						message = jsonException.Message,
						path = jsonException.Path
					}
				});
				context.ExceptionHandled = true;
			}
		}
	}
}

using NLog.Extensions.Logging;
using PedidoAPI.Configurations;
using PedidoAPI.DTOs;
using PedidoAPI.Exceptions;
using PedidoAPI.Infrastructure;
using PedidoAPI.Middlewares;
using PedidoAPI.Models;
using PedidoAPI.Repositories;
using PedidoAPI.Services;

var builder = WebApplication.CreateBuilder(args);

//Logging
// Limpar os provedores de log padr�o e adicionar NLog
builder.Logging.ClearProviders();
builder.Logging.AddNLog(builder.Configuration);


//Configura��o de servi�os
ConfigureServices(builder.Services);

var app = builder.Build();
ConfigurePipeline(app);

app.Run();

void ConfigureServices(IServiceCollection services)
{
	services.AddControllers(options =>
	{
		options.Filters.Add<JsonExceptionFilter>();
	});

	//Registra jwt
	services.AddJwtAuthentication(builder.Configuration);

	//Registra a configura��o da string de conex�o
	services.AddScoped<DatabaseConnection>();

	//Registro do reposit�rio
	services.AddScoped<IUsuarioRepository, UsuarioRepository>();
	services.AddScoped<IPedidoRepository, PedidoRepository>();

	//Registro do servi�o
	services.AddScoped<IUsuarioService, UsuarioService>();
	services.AddScoped<IPedidoService, PedidoService>();
	services.AddScoped<TokenService>();

	//Registra o cache do Redis
	services.AddStackExchangeRedisCache(options =>
	{
		options.Configuration = builder.Configuration["Redis:IP"]; 
		options.InstanceName = builder.Configuration["Redis:InstanceName"];
	});

	services.AddEndpointsApiExplorer();

	//Configura��o swagger para usar autentica��o
	services.AddSwaggerConfiguration();

	//Automapper
	services.AddAutoMapper(cfg =>
	{
		cfg.CreateMap<Usuario, UsuarioDTO>().ReverseMap();
		cfg.CreateMap<Pedido, PedidoDTO>().ReverseMap();
	});
}


void ConfigurePipeline(WebApplication app)
{
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();
	app.UseExceptionHandlingMiddleware();
	app.UseAuthentication();
	app.UseAuthorization();
	app.MapControllers();
}



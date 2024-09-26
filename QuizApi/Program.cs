using Microsoft.EntityFrameworkCore;
using QuizApi.Data;
using AutoMapper;
using QuizApi.Profiles;
using static QuizApi.Data.QuizContext;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Adiciona o serviço de controle para o container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
        ReferenceHandler.IgnoreCycles;
    });

// Adiciona o CORS
builder.Services.AddCors(builder =>
{
    builder.AddPolicy("AllowAll", options =>
    {
        options.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Configura o contexto do banco de dados usando SQL Server.
// Obtém a string de conexão do arquivo de configuração.
builder.Services.AddDbContext<QuizContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuizDatabase")));

// Adiciona serviços para explorar endpoints e gerar documentação Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddAutoMapper(typeof(QuizApi.Profiles.MappingProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services); // Chama o método para popular o banco de dados
}



// Configure the HTTP request pipeline.
// Configura o pipeline de requisição HTTP.

// Adiciona o Swagger apenas em ambiente de desenvolvimento.
// Isso permite visualizar e testar a API através da interface Swagger.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configura o pipeline de requisição HTTP.
// O HTTPS Redirection é removido para permitir apenas HTTP.
// Se você não deseja usar HTTPS, remova ou comente esta linha.
app.UseHttpsRedirection(); // REMOVA ou COMENTE ESTA LINHA se não for usar HTTPS

app.UseAuthorization();

app.UseCors("AllowAll");

// Mapeia os controladores para que eles possam ser usados.
// Permite que as rotas para os controladores sejam acessadas.
app.MapControllers();

// Executa a aplicação.
app.Run();

using Microsoft.EntityFrameworkCore;
using QuizApi.Data;
using AutoMapper;
using QuizApi.Profiles;
using static QuizApi.Data.QuizContext;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Adiciona o servi�o de controle para o container.
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
// Obt�m a string de conex�o do arquivo de configura��o.
builder.Services.AddDbContext<QuizContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuizDatabase")));

// Adiciona servi�os para explorar endpoints e gerar documenta��o Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddAutoMapper(typeof(QuizApi.Profiles.MappingProfile).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services); // Chama o m�todo para popular o banco de dados
}



// Configure the HTTP request pipeline.
// Configura o pipeline de requisi��o HTTP.

// Adiciona o Swagger apenas em ambiente de desenvolvimento.
// Isso permite visualizar e testar a API atrav�s da interface Swagger.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configura o pipeline de requisi��o HTTP.
// O HTTPS Redirection � removido para permitir apenas HTTP.
// Se voc� n�o deseja usar HTTPS, remova ou comente esta linha.
app.UseHttpsRedirection(); // REMOVA ou COMENTE ESTA LINHA se n�o for usar HTTPS

app.UseAuthorization();

app.UseCors("AllowAll");

// Mapeia os controladores para que eles possam ser usados.
// Permite que as rotas para os controladores sejam acessadas.
app.MapControllers();

// Executa a aplica��o.
app.Run();

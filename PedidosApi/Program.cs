using PedidosApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<PedidosDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PedidosConnection")));

// Cliente HTTP para comunicación con el microservicio CatalogoApi
builder.Services.AddHttpClient("CatalogoApi", client =>
{
    var url = builder.Configuration["CatalogoApi:BaseUrl"] ?? "http://localhost:5001/";
    client.BaseAddress = new Uri(url);
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedidos.Api v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

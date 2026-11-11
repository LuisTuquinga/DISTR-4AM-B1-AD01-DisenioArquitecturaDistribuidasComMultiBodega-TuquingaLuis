using CatalogoApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<CatalogoDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogoConnection")));


// Cliente HTTP para comunicación con el microservicio InventarioApi
builder.Services.AddHttpClient("InventarioApi", client =>
{
    var url = builder.Configuration["InventarioApi:BaseUrl"] ?? "http://localhost:5002/";
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalogo.Api v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

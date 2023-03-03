using Jens.AspNetCore.AutoAPI;
using Server;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var services = builder.Services;
// Create your db context
services.AddScoped<InMemoryDbContext>();
// Adds auto api controllers based on given assemblies.
services.AddAutoAPIControllers<InMemoryDbContext>(typeof(Models.Movie).Assembly);
// Configure swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c => c.WithAutoAPIDefaults());

var app = builder.Build();
// If we want to only build our swagger.yaml. 
if (app.TryRunSwaggerOutputOnly())
    return; // exit server.

// This hangs in the default exception handler so our API specs dont mess up.
app.UseExceptionHandler(Jens.AspNetCore.AutoAPI.ErrorController.ROUTE);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

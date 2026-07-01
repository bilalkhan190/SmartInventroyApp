using SmartInventory.Application;
using SmartInventory.Infrastructure;
using SmartInventory.Infrastructure.Persistance.Seeding;
using SmartInventory.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddEndpoints();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();

using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
var connectionString = "Data Source=DESKTOP-E0FAPSB;Initial Catalog=ToysShop;Integrated Security=True;Trust Server Certificate=True";
builder.Services.AddDbContext<ToysShopContext>(options=>options.UseSqlServer(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

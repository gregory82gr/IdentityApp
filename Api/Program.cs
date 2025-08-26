using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<JWTService>();

//defining our IdentityCore Services
builder.Services.AddIdentityCore<User>(options =>
{
    //password configuration
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // email unique configuration
    options.SignIn.RequireConfirmedEmail = true;
}).AddRoles<IdentityRole>() // enabling roles
  .AddRoleManager<RoleManager<IdentityRole>>() // enabling role manager
  .AddEntityFrameworkStores<Context>() // configuring our context class
  .AddSignInManager<SignInManager<User>>() // enabling sign in manager
  .AddUserManager<UserManager<User>>()// enabling user manager
  .AddDefaultTokenProviders();// enabling default token providers

var app = builder.Build();

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

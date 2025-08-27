using Api.Data;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// be able to authenticate users using JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //validate the token based on the key we have provided inside the appsettings.development.json JWT:Key section
            ValidateIssuerSigningKey = true,
            // the issuer signing key will be the same key we have used to encrypt the token (based on JWT:Key)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            // validate the issuer(who ever is Issuing (created) the token)
            ValidateIssuer = true,
            //the issuer (which is here the api.project url we are using) will be the same as we have provided inside the appsettings.development.json JWT:Issuer section
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            // we won't be using audience for now but in case you want to use it, you can provide the same inside the appsettings.development.json JWT:Audience section
            //don't validate the audience (angular application  which is going to consume the token)
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

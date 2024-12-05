using System.Security.Cryptography;
using API;
using Application;
using Application.Common.Interfaces;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<IBudgetRepository, BudgetRepository>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddAuthorization();
var rsaKey = RSA.Create();
rsaKey.ImportRSAPrivateKey(Convert.FromBase64String(builder.Configuration.GetSection("PrivateKey").Value), out _);
builder.Services.AddAuthentication("jwt")
    .AddJwtBearer("jwt", x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidateIssuer = false
        };
        x.Configuration = new OpenIdConnectConfiguration()
        {
            SigningKeys =
            {
                new RsaSecurityKey(rsaKey)
            }
        };

        x.MapInboundClaims = false;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "BudgetMax API", 
        Version = "v1" 
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Description = "Please insert JWT token into field"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { 
            new OpenApiSecurityScheme 
            { 
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                } 
            },
            new string[] { } 
        } 
    });
});
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers()
    .RequireAuthorization();
app.Run();
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using StudentManagementApi.Data;
using StudentManagementApi.Services;
using System.Text;
using Microsoft.IdentityModel.Logging;

IdentityModelEventSource.ShowPII = false; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
        };

        options.Events = new JwtBearerEvents {
            OnMessageReceived = context => {
  
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context => {
                Console.WriteLine("JWT Auth Failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context => {
                Console.WriteLine("JWT Challenge: " + context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnTokenValidated = context => {
                Console.WriteLine("JWT Validated: " + context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddOpenApi(options => {
    options.AddDocumentTransformer((document, context, cancellationToken) => {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Enter JWT token (without 'Bearer ' prefix)"
        };

        document.SecurityRequirements.Add(new OpenApiSecurityRequirement {
            [document.Components.SecuritySchemes["Bearer"]] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

if(app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

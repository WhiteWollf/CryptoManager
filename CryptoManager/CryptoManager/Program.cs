
using AutoMapper.Internal;
using DataContext.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CryptoManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<CryptoDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQL")));

            //Inicializáló szolgáltatás hozzáadása (Cryptok feltöltése, User-ek és Wallet-ek, stb. alap adatok teszteléshez)
            //Minden indításnál visszaállítja, hogy fix legyen és pontosan tudjuk a változásokat figyelni
            builder.Services.AddHostedService<InitializerService>();

            //Háttér szolgáltatás hozzáadása (Cryptok árának frissítése)
            builder.Services.AddHostedService<CryptoPriceBackgroundService>();

            builder.Services.AddLocalServices();


            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Autorizáció beállítása
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
                options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
            });

            builder.Services.AddCors();

            // AutoMapper Config
            //builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            //Az összes osztályt hozzáadja, ami a Profile osztályból öröklõdik
            builder.Services.AddAutoMapper(cfg => cfg.Internal().MethodMappingEnabled = false, AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options=>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoManager API");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(options =>
            {
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
            });

            app.MapControllers();

            app.Run();
        }
    }
}

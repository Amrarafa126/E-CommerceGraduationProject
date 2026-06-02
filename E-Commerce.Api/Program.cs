
using E_Commerce.Api.Middleware;
using E_Commerce.Core;
using E_Commerce.Infrustructure;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Service;
using E_Commerce.Service.Payment;
using E_Commerce.Service.Shipping;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace E_Commerce.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddSignalR();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDBContext>(op =>
            op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.Configure<PaymobOptions>(builder.Configuration.GetSection("Paymob"));
            builder.Services.Configure<BostaOptions>(builder.Configuration.GetSection("Bosta"));
            builder.Services.AddInfrustructureDependencies().AddServiceDependencies().AddCoreDependencies().AddServiceRegisteration(builder.Configuration);
            builder.Services.AddHttpContextAccessor();


            #region AllowCORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "B2B Marketplace API",
                    Version = "v1",
                    Description = "A B2B Marketplace platform",
                    Contact = new OpenApiContact { Name = "B2B Marketplace Team" }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token: Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
            });

            #endregion
            var app = builder.Build();
            await DbSeeder.SeedAsync(app.Services);


            app.UseGlobalExceptionHandling();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<E_Commerce.Core.Features.Chats.Hubs.ChatHub>("/hubs/chat");

            app.Run();
        }
    }
}

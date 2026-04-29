using E_Commerce.Data.Entity;
using E_Commerce.Data.Helpers;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace E_Commerce.Infrustructure
{
    public static class ServiceRegisteration
    {
        public static IServiceCollection AddServiceRegisteration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<User, Role>(option =>
            {
                // Password settings.
                option.Password.RequireDigit = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequiredLength = 6;
                option.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.AllowedForNewUsers = true;

                // User settings.
                option.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //option.User.RequireUniqueEmail = true;
                //option.SignIn.RequireConfirmedEmail = true;

            }).AddEntityFrameworkStores<AppDBContext>().AddDefaultTokenProviders();

            //JWT Authentication
            var jwtSettings = new JwtSettings();
            var emailSettings = new EmailSettings();
            configuration.GetSection(nameof(jwtSettings)).Bind(jwtSettings);
            configuration.GetSection(nameof(emailSettings)).Bind(emailSettings);

            services.AddSingleton(jwtSettings);
            services.AddSingleton(emailSettings);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(x =>
           {
               x.RequireHttpsMetadata = false;
               x.SaveToken = true;
               x.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = jwtSettings.ValidateIssuer,
                   ValidIssuers = new[] { jwtSettings.Issuer },
                   ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                   ValidAudience = jwtSettings.Audience,
                   ValidateAudience = jwtSettings.ValidateAudience,
                   ValidateLifetime = jwtSettings.ValidateLifeTime,
               };
           });


            //Swagger Gn
            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "School Project", Version = "v1" });
            //     c.EnableAnnotations();

            //     c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            //     {
            //         Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
            //         Name = "Authorization",
            //         In = ParameterLocation.Header,
            //         Type = SecuritySchemeType.ApiKey,
            //         Scheme = JwtBearerDefaults.AuthenticationScheme
            //     });

            //     c.AddSecurityRequirement(new OpenApiSecurityRequirement
            // {
            // {
            // new OpenApiSecurityScheme
            // {
            //     Reference = new OpenApiReference
            //     {
            //         Type = ReferenceType.SecurityScheme,
            //         Id = JwtBearerDefaults.AuthenticationScheme
            //     }
            // },
            // Array.Empty<string>()
            // }
            //});
            // });

            services.AddAuthorizationBuilder()
             .AddPolicy("SellerOnly", p => p.RequireRole(Role.Names.Seller))
             .AddPolicy("BuyerOnly", p => p.RequireRole(Role.Names.Buyer))
             .AddPolicy("AdminOnly", p => p.RequireRole(Role.Names.Admin))
             .AddPolicy("SellerOrAdmin", p => p.RequireRole(
                 Role.Names.Seller, Role.Names.Admin));



            return services;
        }

        }
    }

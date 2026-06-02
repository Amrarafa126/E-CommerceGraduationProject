using E_Commerce.Data.Entity;
using E_Commerce.Data.Helpers;
using E_Commerce.Data.Identity;
using E_Commerce.Infrustructure.Context;
using E_Commerce.Infrustructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
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
            services.AddIdentity<User, Role>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequiredLength = 6;
                opts.Password.RequiredUniqueChars = 1;

                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                opts.Lockout.MaxFailedAccessAttempts = 5;
                opts.Lockout.AllowedForNewUsers = true;

                opts.User.RequireUniqueEmail = true;

                opts.SignIn.RequireConfirmedEmail = false;

            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<ArabicIdentityErrorDescriber>();
            var jwtSecret = configuration["JwtSettings:Secret"]
                      ?? throw new InvalidOperationException("JwtSettings:Secret is not configured.");

            services
                .AddAuthentication(opts =>
                {
                    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JwtSettings:Issuer"],
                        ValidAudience = configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSecret)),
                        ClockSkew = TimeSpan.Zero,

                        // Map the ClaimTypes.Role from the JWT to ASP.NET Core's role system
                        // so [Authorize(Roles = "Seller")] works correctly
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    };

                    opts.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            var accessToken = ctx.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                                ctx.Token = accessToken;
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
                        {
                            if (ctx.Exception is SecurityTokenExpiredException)
                                ctx.Response.Headers.Append("Token-Expired", "true");
                            return Task.CompletedTask;
                        }
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

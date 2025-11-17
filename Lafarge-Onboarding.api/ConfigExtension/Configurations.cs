namespace Lafarge_Onboarding.api.ConfigExtension;

public static class Configurations
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {

        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/lafarge-onboarding-.txt", rollingInterval: RollingInterval.Day);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        // Register application services
        builder.Services.AddApplicationServices();

        // Register infrastructure services
        builder.Services.AddInfrastructureServices();

        // Add HttpClient for email service
        builder.Services.AddHttpClient();
        
        // Add HttpContextAccessor for accessing current user
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
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
            new string[] {}
        }
            });
        });
        builder.Services.AddControllers();

        // Add Identity
        builder.Services.AddIdentity<Users, Role>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        })
        .AddEntityFrameworkStores<Lafarge_Onboarding.infrastructure.Data.ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Add JWT Authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
            };
        });

        // Add Authorization
        builder.Services.AddAuthorization();

        // Add DbContext
        builder.Services.AddDbContext<Lafarge_Onboarding.infrastructure.Data.ApplicationDbContext>(options =>
        {
            var connectionString = BuildConnectionString(builder.Configuration);
            options.UseNpgsql(connectionString);
        });

        return builder;
    }

   
    public static WebApplication ConfigureMiddleware(this WebApplication app)
    {
      
        app.UseMiddleware<Lafarge_Onboarding.api.Middleware.ExceptionHandlingMiddleware>();

    
        app.UseCors("AllowAll");

           app.UseSwagger();
           app.UseSwaggerUI();



           app.UseAuthentication();
           app.UseAuthorization();

           app.MapControllers();
           return app;
    }

    static string BuildConnectionString(IConfiguration configuration)
    {
    // Try environment variables first (for production)
    var connectionName = Environment.GetEnvironmentVariable("DB_CONNECTION_NAME");
    var database = Environment.GetEnvironmentVariable("DB_NAME");
    var username = Environment.GetEnvironmentVariable("DB_USER");
    var password = Environment.GetEnvironmentVariable("POSTGRES_PWD");



        // If DB_CONNECTION_NAME is set, use Cloud SQL socket connection
        if (!string.IsNullOrEmpty(connectionName))
        {
            database = database ?? configuration["Database:DB_NAME"] ?? "lafarge_onboarding_db";
            username = username ?? configuration["Database:DB_USER"] ?? "postgres";
            password = password ?? configuration["Database:POSTGRES_PWD"] ?? "placeholder-password";

            return $"Host=/cloudsql/{connectionName};Database={database};Username={username};Password={password};Pooling=true;MinPoolSize=5;MaxPoolSize=20;ConnectionLifetime=300;";
        }
        return configuration.GetConnectionString("DefaultConnection")!;
    }

  }


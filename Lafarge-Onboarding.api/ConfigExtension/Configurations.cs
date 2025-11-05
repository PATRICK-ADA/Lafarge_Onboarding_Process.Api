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
        // Configure the HTTP request pipeline.
        
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
    var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
    var port = Environment.GetEnvironmentVariable("DB_PORT");
    var database = Environment.GetEnvironmentVariable("DB_NAME");
    var username = Environment.GetEnvironmentVariable("DB_USER");
    var password = Environment.GetEnvironmentVariable("POSTGRES_PWD");
    var connectionName = Environment.GetEnvironmentVariable("DB_CONNECTION_NAME");
    
    // If all environment variables are present, build connection string
    if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(port) && 
        !string.IsNullOrEmpty(database) && !string.IsNullOrEmpty(username) && 
        !string.IsNullOrEmpty(password))
    {
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};Pooling=true;MinPoolSize=5;MaxPoolSize=20;ConnectionLifetime=300;";
    }
    
    // Fallback to appsettings.json configuration values
    host = configuration["Database:DATABASE_HOST"] ?? "localhost";
    port = configuration["Database:DB_PORT"] ?? "5432";
    database = configuration["Database:DB_NAME"] ?? "lafarge_onboarding_db";
    username = configuration["Database:DB_USER"] ?? "postgres";
    password = configuration["Database:POSTGRES_PWD"] ?? "placeholder-password";
    
    // Final fallback to DefaultConnection
    if (password == "placeholder-password")
    {
        return configuration.GetConnectionString("DefaultConnection")!;
    }
    
    return $"Host={host};Port={port};Database={database};Username={username};Password={password};Pooling=true;MinPoolSize=5;MaxPoolSize=20;ConnectionLifetime=300;";
  }
}

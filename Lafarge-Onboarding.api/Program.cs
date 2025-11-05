
var builder = WebApplication.CreateBuilder(args);


var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");


builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/lafarge-onboarding-.txt", rollingInterval: RollingInterval.Day));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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


builder.Services.AddAuthorization();


builder.Services.AddDbContext<Lafarge_Onboarding.infrastructure.Data.ApplicationDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("ONBOARDING_DB_URL");

    if (string.IsNullOrEmpty(connectionString))
    {
        var host = Environment.GetEnvironmentVariable("ONBOARDING_DB_HOST");
        var port = Environment.GetEnvironmentVariable("ONBOARDING_DB_PORT");
        var database = Environment.GetEnvironmentVariable("ONBOARDING_DB_NAME");
        var username = Environment.GetEnvironmentVariable("ONBOARDING_DB_USERNAME");
        var password = Environment.GetEnvironmentVariable("ONBOARDING_DB_PASSWORD");

        
        connectionString = $"Host={host};Port={port};Database={database};Username={username};Password=\"{password}\"";
    }

    if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("localhost"))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    }

    options.UseNpgsql(connectionString);
});


builder.Services.AddScoped<IDocumentsUploadService, Lafarge_Onboarding.application.Services.DocumentsUploadService>();
builder.Services.AddScoped<IDocumentsUploadRepository, Lafarge_Onboarding.infrastructure.Repositories.DocumentsUploadRepository>();
builder.Services.AddScoped<IAuthService, Lafarge_Onboarding.application.Services.AuthService>();
builder.Services.AddScoped<IUsersService, Lafarge_Onboarding.application.Services.UsersService>();
builder.Services.AddScoped<IUsersRepository, Lafarge_Onboarding.infrastructure.Repositories.UsersRepository>();

var app = builder.Build();

// Apply migrations
//using (var scope = app.Services.CreateScope())
//{
   // var dbContext = scope.ServiceProvider.GetRequiredService<Lafarge_Onboarding.infrastructure.Data.ApplicationDbContext>();
  //  dbContext.Database.Migrate();
//}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


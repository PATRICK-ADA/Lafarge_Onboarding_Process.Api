var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Lafarge_Onboarding.infrastructure.Data.ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

   
    if (env.IsDevelopment())
    {
        try
        {
            logger.LogInformation("Applying database migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw; 
        }
    }
    else
    {
        logger.LogInformation("Skipping automatic migrations in production. Ensure database is migrated manually.");
    }
}

// Configure the HTTP request pipeline.

app.ConfigureMiddleware();
app.Run();


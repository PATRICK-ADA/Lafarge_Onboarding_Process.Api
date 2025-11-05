
var builder = WebApplication.CreateBuilder(args);


builder.ConfigureServices();

var app = builder.Build();

// Apply migrations
//using (var scope = app.Services.CreateScope())
//{
   // var dbContext = scope.ServiceProvider.GetRequiredService<Lafarge_Onboarding.infrastructure.Data.ApplicationDbContext>();
  //  dbContext.Database.Migrate();
//}

// Configure the HTTP request pipeline.

app.ConfigureMiddleware();
app.Run();


namespace Lafarge_Onboarding.infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<Users, Role, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<OnboardingDocument> OnboardingDocuments { get; set; }
    public DbSet<LocalHireInfo> LocalHireInfos { get; set; }
    public DbSet<WelcomeMessage> WelcomeMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure OnboardingDocument entity
        modelBuilder.Entity<OnboardingDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.FilePath).HasMaxLength(1000);
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.ContentHeading).HasMaxLength(500);
            entity.Property(e => e.ContentSubHeading).HasMaxLength(500);
            entity.Property(e => e.UploadedBy).HasMaxLength(100);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("NOW()");
        });

        // Configure LocalHireInfo entity
        modelBuilder.Entity<LocalHireInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WhoWeAre).HasColumnType("text");
            entity.Property(e => e.FootprintSummary).HasColumnType("text");
            entity.Property(e => e.Plants).HasColumnType("text");
            entity.Property(e => e.ReadyMix).HasColumnType("text");
            entity.Property(e => e.Depots).HasColumnType("text");
            entity.Property(e => e.CultureSummary).HasColumnType("text");
            entity.Property(e => e.Pillars).HasColumnType("text");
            entity.Property(e => e.Innovation).HasColumnType("text");
            entity.Property(e => e.HuaxinSpirit).HasColumnType("text");
            entity.Property(e => e.RespectfulWorkplaces).HasColumnType("text");
            entity.Property(e => e.Introduction).HasColumnType("text");
            entity.Property(e => e.CountryFacts).HasColumnType("text");
            entity.Property(e => e.InterestingFacts).HasColumnType("text");
            entity.Property(e => e.Holidays).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // Configure WelcomeMessage entity
        modelBuilder.Entity<WelcomeMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CeoName).HasMaxLength(200);
            entity.Property(e => e.CeoTitle).HasMaxLength(200);
            entity.Property(e => e.CeoImageUrl).HasMaxLength(500);
            entity.Property(e => e.CeoMessage).HasColumnType("text");
            entity.Property(e => e.HrName).HasMaxLength(200);
            entity.Property(e => e.HrTitle).HasMaxLength(200);
            entity.Property(e => e.HrImageUrl).HasMaxLength(500);
            entity.Property(e => e.HrMessage).HasColumnType("text");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // Configure custom Users entity
        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // Configure custom Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });

        // Seed roles
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = "1",
                Name = "LOCAL_HIRE",
                NormalizedName = "LOCAL_HIRE",
                Description = "Local hire role",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = "2",
                Name = "EXPAT",
                NormalizedName = "EXPAT",
                Description = "Expat role",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = "3",
                Name = "VISITOR",
                NormalizedName = "VISITOR",
                Description = "Visitor role",
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = "4",
                Name = "HR_ADMIN",
                NormalizedName = "HR_ADMIN",
                Description = "HR Admin role",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
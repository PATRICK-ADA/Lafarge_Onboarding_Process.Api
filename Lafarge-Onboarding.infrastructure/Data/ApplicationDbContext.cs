namespace Lafarge_Onboarding.infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<Users, Role, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<OnboardingDocument> OnboardingDocuments { get; set; }

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
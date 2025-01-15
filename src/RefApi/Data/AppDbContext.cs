using Microsoft.EntityFrameworkCore;

using RefApi.Features.Chat;
using RefApi.Features.Conversations;
using RefApi.Security;

namespace RefApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IUserContext userContext) : DbContext(options)
{
    public DbSet<Conversation> Conversations { get; init; }
    public DbSet<Message> Messages { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // As the application grows, these configurations can be extracted to separate classes:
        // public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
        // 
        // Then registered using:
        // modelBuilder.ApplyConfiguration(new ConversationConfiguration());

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasMany(c => c.Messages)
                .WithOne(cm => cm.Conversation)
                .HasForeignKey(cm => cm.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasIndex(c => c.UserId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.ConversationId);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var timestamp = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = timestamp;
                entry.Property("CreatedBy").CurrentValue = userContext.UserId;
                entry.Property("UpdatedAt").CurrentValue = timestamp;
                entry.Property("UpdatedBy").CurrentValue = userContext.UserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = timestamp;
                entry.Property("UpdatedBy").CurrentValue = userContext.UserId;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
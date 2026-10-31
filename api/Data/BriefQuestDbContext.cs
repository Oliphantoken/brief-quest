using Microsoft.EntityFrameworkCore;
using BriefQuest.Entities;

namespace BriefQuest.Data;

/// <summary>
/// A database helper that opens the db and runs queries on the list of briefs you host. You have to tell it how to connect to connect to your db, which you do through the dbContextOptions.
/// </summary>
/// <param name="options">Context options hold info on db info credentials. It's injected into the parent db context. </param>
public class BriefQuestDbContext(DbContextOptions<BriefQuestDbContext> options) : DbContext(options)
{
    //Define the list of briefs
    public DbSet<Brief> briefs => Set<Brief>();    
    public DbSet<Outcome> Outcomes => Set<Outcome>();
    public DbSet<Criterion> Criteria => Set<Criterion>();
    public DbSet<Quest> Quests => Set<Quest>();

    /// <summary>
    /// On creating a model, specify any rules it has and relationships to other entities by
    /// specifying primary key and secondary/foreign key and navigation property
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brief>().Property(b=>b.Title).HasMaxLength(200);

        modelBuilder.Entity<Outcome>()
            .HasOne(o => o.Brief).WithMany(b => b.Outcomes)
            .HasForeignKey(o => o.BriefID)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Outcome>().HasIndex(o => new {o.BriefID, o.Code }).IsUnique();

        modelBuilder.Entity<Criterion>()
            .HasOne(c => c.Outcome).WithMany(o => o.Criterion)
            .HasForeignKey(c => c.OutcomeID)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Criterion>().Property(c => c.Tier).HasConversion<string>();

        modelBuilder.Entity<Quest>()
            .HasOne(q => q.Brief).WithMany(b => b.Quests)
            .HasForeignKey(q => q.BriefID)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Quest>()
            .HasOne(q => q.Criterion).WithMany()
            .HasForeignKey(q => q.CriterionID)
            .OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Quest>().Property(q => q.Tier).HasConversion<string>();
        modelBuilder.Entity<Quest>().Property(q => q.Status).HasConversion<string>();


    }

}

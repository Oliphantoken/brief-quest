using Microsoft.EntityFrameworkCore;
using BriefQuest.Entities;
using BriefQuest.Utils;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BriefQuest.Data;

/// <summary>
/// A database helper that opens the db and runs queries on the list of briefs you host. You have to tell it how to connect to connect to your db, which you do through the dbContextOptions.
/// </summary>
/// <param name="options">Context options hold info on db info credentials. It's injected into the parent db context. </param>
public class BriefQuestDbContext(DbContextOptions<BriefQuestDbContext> options) : IdentityDbContext<AppUser>(options)
{
    //Define the list of briefs
    public DbSet<Brief> briefs => Set<Brief>();    
    public DbSet<Outcome> Outcomes => Set<Outcome>();
    public DbSet<Criterion> Criteria => Set<Criterion>();
    public DbSet<Quest> Quests => Set<Quest>();
    public DbSet<CommandWord> CommandWords => Set<CommandWord>();
    public DbSet<UsageCounter> UsageCounters => Set<UsageCounter>();

    /// <summary>
    /// On creating a model, specify any rules it has and relationships to other entities by
    /// specifying primary key and secondary/foreign key and navigation property
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Brief>().Property(b=>b.Title).HasMaxLength(200);

        modelBuilder.Entity<Brief>()
            .HasOne(b => b.Owner).WithMany()
            .HasForeignKey(b =>b.OwnerID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Outcome>()
            .HasOne(o => o.Brief).WithMany(b => b.Outcomes)
            .HasForeignKey(o => o.BriefID)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Outcome>().HasIndex(o => new {o.BriefID, o.Code }).IsUnique();

        modelBuilder.Entity<Criterion>()
            .HasOne(c => c.Outcome).WithMany(o => o.Criteria)
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

        modelBuilder.Entity<CommandWord>().HasKey(w => w.Word);
        modelBuilder.Entity<CommandWord>().Property(w => w.BloomLevel).HasConversion<string>();
        modelBuilder.Entity<CommandWord>().HasData(
            new CommandWord { Word = "Identify", PlainMeaning = "Point out or name something specific.", BloomLevel = BloomLevel.Remember, Example = "Identify the main gameplay mechanics in your platformer prototype." },
            new CommandWord { Word = "List", PlainMeaning = "Give items one by one, usually in a simple sequence.", BloomLevel = BloomLevel.Remember, Example = "List the assets you need to create for your 2D platformer." },
            new CommandWord { Word = "Describe", PlainMeaning = "Say what something is like or what happens.", BloomLevel = BloomLevel.Understand, Example = "Describe how the player moves and jumps in your game prototype." },
            new CommandWord { Word = "Explain", PlainMeaning = "Make something clear by saying how or why it works.", BloomLevel = BloomLevel.Understand, Example = "Explain why you chose a side-scrolling camera for your platformer." },
            new CommandWord { Word = "Summarise", PlainMeaning = "Give the main points in a shorter form.", BloomLevel = BloomLevel.Understand, Example = "Summarise the main goals of your game design document." },
            new CommandWord { Word = "Compare", PlainMeaning = "Say how two or more things are similar and different.", BloomLevel = BloomLevel.Analyse, Example = "Compare the movement systems in two similar platformer games." },
            new CommandWord { Word = "Contrast", PlainMeaning = "Focus mainly on the differences between things.", BloomLevel = BloomLevel.Analyse, Example = "Contrast the reward systems in two mobile puzzle games." },
            new CommandWord { Word = "Analyse", PlainMeaning = "Break something into parts and explain how the parts relate.", BloomLevel = BloomLevel.Analyse, Example = "Analyse how level design, controls, and rewards affect player motivation." },
            new CommandWord { Word = "Investigate", PlainMeaning = "Find out more about something by researching or testing it.", BloomLevel = BloomLevel.Analyse, Example = "Investigate how difficulty curves affect player retention in platformers." },
            new CommandWord { Word = "Apply", PlainMeaning = "Use a skill, method, or idea in a real task.", BloomLevel = BloomLevel.Apply, Example = "Apply your research on player motivation to design the game’s reward system." },
            new CommandWord { Word = "Demonstrate", PlainMeaning = "Show that you can do something or that something works.", BloomLevel = BloomLevel.Apply, Example = "Demonstrate that the player can complete the first level without getting stuck." },
            new CommandWord { Word = "Implement", PlainMeaning = "Put a plan or idea into working form.", BloomLevel = BloomLevel.Apply, Example = "Implement the double-jump mechanic in Unity using your design document." },
            new CommandWord { Word = "Experiment", PlainMeaning = "Try different options to see what works best.", BloomLevel = BloomLevel.Apply, Example = "Experiment with different jump heights to find the most enjoyable control feel." },
            new CommandWord { Word = "Test", PlainMeaning = "Try something to check whether it works or meets a requirement.", BloomLevel = BloomLevel.Evaluate, Example = "Test your prototype with three players and record their feedback." },
            new CommandWord { Word = "Evaluate", PlainMeaning = "Judge how good something is using clear reasons or criteria.", BloomLevel = BloomLevel.Evaluate, Example = "Evaluate whether your prototype meets the brief’s Merit and Distinction requirements." },
            new CommandWord { Word = "Justify", PlainMeaning = "Give strong reasons for a decision or choice.", BloomLevel = BloomLevel.Evaluate, Example = "Justify your decision to use pixel-art visuals based on player research." },
            new CommandWord { Word = "Critically evaluate", PlainMeaning = "Look carefully at strengths, weaknesses, and evidence before making a judgement.", BloomLevel = BloomLevel.Evaluate, Example = "Critically evaluate two similar games and explain what your game will learn from each." },
            new CommandWord { Word = "Reflect", PlainMeaning = "Think back over what you did, what worked, and what you would change.", BloomLevel = BloomLevel.Evaluate, Example = "Reflect on how playtesting changed your ideas for the first level." },
            new CommandWord { Word = "Recommend", PlainMeaning = "Suggest the best option and explain why.", BloomLevel = BloomLevel.Evaluate, Example = "Recommend improvements to the game based on your playtest results." },
            new CommandWord { Word = "Prioritise", PlainMeaning = "Decide which things are most important and should be done first.", BloomLevel = BloomLevel.Evaluate, Example = "Prioritise the essential mechanics you must finish before the submission deadline." },
            new CommandWord { Word = "Plan", PlainMeaning = "Decide what you will do, when, and how.", BloomLevel = BloomLevel.Create, Example = "Plan the stages of your platformer prototype from research to final submission." },
            new CommandWord { Word = "Design", PlainMeaning = "Decide how something will look, work, or be organised.", BloomLevel = BloomLevel.Create, Example = "Design a level that teaches the player the jump mechanic gradually." },
            new CommandWord { Word = "Develop", PlainMeaning = "Build or improve an idea over time.", BloomLevel = BloomLevel.Create, Example = "Develop your initial game concept into a playable Unity prototype." },
            new CommandWord { Word = "Create", PlainMeaning = "Make something new from your ideas, research, and skills.", BloomLevel = BloomLevel.Create, Example = "Create a playable 2D platformer prototype that meets the brief." },
            new CommandWord { Word = "Produce", PlainMeaning = "Make and complete a finished piece of work.", BloomLevel = BloomLevel.Create, Example = "Produce a 1,500-word design document and a three-minute video walkthrough." },
            new CommandWord { Word = "Present", PlainMeaning = "Share your work or ideas clearly with an audience.", BloomLevel = BloomLevel.Create, Example = "Present your game concept and prototype progress to your tutor or class." }

        );

        modelBuilder.Entity<UsageCounter>()
            .HasKey( u => new {u.OwnerID, u.Day} );
        modelBuilder.Entity<UsageCounter>()
            .HasOne(u => u.Owner).WithMany()
            .HasForeignKey(u => u.OwnerID)
            .OnDelete(DeleteBehavior.Cascade);

    }

}

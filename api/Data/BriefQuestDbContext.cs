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

}

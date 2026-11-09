using BriefQuest.Data;
using BriefQuest.Entities;
using Microsoft.EntityFrameworkCore;

namespace BriefQuest.Services;

/// <summary>
/// A service to keep a counter for how many times the user has extracted a brief during the day.
/// There's a daily limit per user (currently 5)
/// </summary>
/// <param name="dbContext"></param>
public class UsageCapService(BriefQuestDbContext dbContext)
{
    private const int DailyLimit = 5;

    public async Task<bool> TryIncrementAsync(string userID)
    {
        //1. Get today's (extract) usage counter for the user
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var counter = await dbContext.UsageCounters.FirstOrDefaultAsync(u => u.OwnerID == userID && u.Day == today);

        //2a. If there's no counter for the user, add a new one to the db
        if(counter == null)
        {
            counter = new UsageCounter { OwnerID = userID, Day = today, Count = 0 };
            dbContext.UsageCounters.Add(counter);
        }

        //2b. If the user has exceeded the daily limit, return false
        if(counter.Count >= DailyLimit) return false;

        //3. Increase the counter and save the change to the db
        counter.Count++;
        await dbContext.SaveChangesAsync();
        return true;

    }

}

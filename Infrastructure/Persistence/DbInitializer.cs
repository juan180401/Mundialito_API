using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Teams.AnyAsync())
            return; // Ya hay datos, no sembramos de nuevo

        var teams = new List<Team>
        {
            new Team("Barcelona"),
            new Team("RealMadrid"),
            new Team("PSG"),
            new Team("ManchesterCity")
        };

        await context.Teams.AddRangeAsync(teams);
        await context.SaveChangesAsync();

        var players = new List<Player>();

        foreach (var team in teams)
        {
            for (int i = 1; i <= 5; i++)
            {
                players.Add(new Player($"Jugador{i}-{team.Name}", team.Id));
            }
        }

        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        var matches = new List<Match>
        {
            new Match(teams[0].Id, teams[1].Id, DateTime.UtcNow.AddDays(1)),
            new Match(teams[2].Id, teams[3].Id, DateTime.UtcNow.AddDays(2)),
            new Match(teams[0].Id, teams[2].Id, DateTime.UtcNow.AddDays(3)),
            new Match(teams[1].Id, teams[3].Id, DateTime.UtcNow.AddDays(4)),
            new Match(teams[0].Id, teams[3].Id, DateTime.UtcNow.AddDays(5)),
            new Match(teams[1].Id, teams[2].Id, DateTime.UtcNow.AddDays(6))
        };

        await context.Matches.AddRangeAsync(matches);
        await context.SaveChangesAsync();
    }
}

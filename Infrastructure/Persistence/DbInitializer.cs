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
            return;

        // =========================
        // 1️⃣ Crear equipos
        // =========================

        var barca = new Team("Barcelona");
        var madrid = new Team("RealMadrid");
        var psg = new Team("PSG");
        var city = new Team("ManchesterCity");

        await context.Teams.AddRangeAsync(barca, madrid, psg, city);
        await context.SaveChangesAsync();

        // =========================
        // 2️⃣ Crear jugadores
        // =========================

        var players = new List<Player>();

        foreach (var team in new[] { barca, madrid, psg, city })
        {
            for (int i = 1; i <= 5; i++)
            {
                players.Add(new Player($"Jugador{i}-{team.Name}", team.Id));
            }
        }

        await context.Players.AddRangeAsync(players);
        await context.SaveChangesAsync();

        // =========================
        // 3️⃣ Crear partidos
        // =========================

        var match1 = new Match(barca.Id, madrid.Id, DateTime.UtcNow.AddDays(-5));
        var match2 = new Match(psg.Id, city.Id, DateTime.UtcNow.AddDays(-4));
        var match3 = new Match(barca.Id, psg.Id, DateTime.UtcNow.AddDays(-3));
        var match4 = new Match(madrid.Id, city.Id, DateTime.UtcNow.AddDays(1));
        var match5 = new Match(barca.Id, city.Id, DateTime.UtcNow.AddDays(2));
        var match6 = new Match(madrid.Id, psg.Id, DateTime.UtcNow.AddDays(3));

        await context.Matches.AddRangeAsync(match1, match2, match3, match4, match5, match6);
        await context.SaveChangesAsync();

        // =========================
        // 4️⃣ Registrar resultados en 3 partidos
        // =========================

        // Barcelona 2 - 1 RealMadrid
        match1.SetResult(2, 1);

        var barcaPlayer1 = players.First(p => p.TeamId == barca.Id);
        var madridPlayer1 = players.First(p => p.TeamId == madrid.Id);

        barcaPlayer1.AddGoal();
        barcaPlayer1.AddGoal();
        madridPlayer1.AddGoal();

        var goals1 = new List<MatchGoal>
        {
            new MatchGoal(match1.Id, barcaPlayer1.Id, 2),
            new MatchGoal(match1.Id, madridPlayer1.Id, 1)
        };

        // PSG 3 - 2 City
        match2.SetResult(3, 2);

        var psgPlayer1 = players.First(p => p.TeamId == psg.Id);
        var cityPlayer1 = players.First(p => p.TeamId == city.Id);

        psgPlayer1.AddGoal();
        psgPlayer1.AddGoal();
        psgPlayer1.AddGoal();
        cityPlayer1.AddGoal();
        cityPlayer1.AddGoal();

        var goals2 = new List<MatchGoal>
        {
            new MatchGoal(match2.Id, psgPlayer1.Id, 3),
            new MatchGoal(match2.Id, cityPlayer1.Id, 2)
        };

        // Barcelona 1 - 1 PSG
        match3.SetResult(1, 1);

        barcaPlayer1.AddGoal();
        psgPlayer1.AddGoal();

        var goals3 = new List<MatchGoal>
        {
            new MatchGoal(match3.Id, barcaPlayer1.Id, 1),
            new MatchGoal(match3.Id, psgPlayer1.Id, 1)
        };

        await context.MatchGoals.AddRangeAsync(goals1);
        await context.MatchGoals.AddRangeAsync(goals2);
        await context.MatchGoals.AddRangeAsync(goals3);

        await context.SaveChangesAsync();
    }
}

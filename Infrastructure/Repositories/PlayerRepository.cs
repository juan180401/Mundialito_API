using Application.Abstractions;
using Domain;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class PlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;

    public PlayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Player player)
    {
        await _context.Players.AddAsync(player);
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        return await _context.Players
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public void Remove(Player player)
    {
        _context.Players.Remove(player);
    }
}
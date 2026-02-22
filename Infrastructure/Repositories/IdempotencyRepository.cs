using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly AppDbContext _context;

    public IdempotencyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IdempotencyRecord?> GetByKeyAsync(string key)
    {
        return await _context.IdempotencyRecords
            .FirstOrDefaultAsync(x => x.Key == key);
    }

    public async Task AddAsync(IdempotencyRecord record)
    {
        await _context.IdempotencyRecords.AddAsync(record);
    }
}

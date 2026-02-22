using Application.Abstractions;
using Application.Commands.Matches;
using Application.Commands.Players;
using Application.Commands.Teams;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<CreateTeamCommandHandler>();
builder.Services.AddScoped<ITeamQueryRepository, TeamQueryRepository>();
builder.Services.AddScoped<UpdateTeamCommandHandler>();
builder.Services.AddScoped<DeleteTeamCommandHandler>();

builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<CreatePlayerCommandHandler>();
builder.Services.AddScoped<IPlayerQueryRepository, PlayerQueryRepository>();

builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<CreateMatchCommandHandler>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

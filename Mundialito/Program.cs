using Application.Abstractions;
using Application.Commands.Matches;
using Application.Commands.Players;
using Application.Commands.Teams;
using Infrastructure.Middleware;
using Infrastructure.Persistence;
using Infrastructure.Queries;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
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
builder.Services.AddScoped<MatchQueryRepository>();

builder.Services.AddScoped<IMatchGoalRepository, MatchGoalRepository>();
builder.Services.AddScoped<RegisterMatchResultCommandHandler>();

builder.Services.AddScoped<TopScorerQueryRepository>();
builder.Services.AddScoped<StandingQueryRepository>();

builder.Services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<IdempotencyHeaderFilter>();
});

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context);
}
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

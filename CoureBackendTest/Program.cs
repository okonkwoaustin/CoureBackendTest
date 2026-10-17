using CoureBackendTest.Data;
using CoureBackendTest.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(
    CalculatorScore.CalculateScore(new[] { 8, 8 })
);

Console.WriteLine(
    CalculatorScore.CalculateScore(new[] { 1, 2, 3, 4, 6 })
);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("CoureTestDatabase"));

builder.Services.AddScoped<IPhoneLookupService, PhoneLookupService>();

var app = builder.Build();

// Create the in-memory database and load its seeded records.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
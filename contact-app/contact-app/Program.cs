using contact_app.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Dodanie DbContext z baz¹ danych InMemory
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("AppDb"));

// Rejestracja kontrolerów (API)
builder.Services.AddControllers();

// Swagger - dokumentacja API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Rejestracja inicjalizacji kontaktów
builder.Services.AddScoped<DataSeeder>();

// Konfiguracja JWT Bearer Authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true, // sprawdzanie, czy token ma poprawnego nadawcê
        ValidateAudience = true, // sprawdzanie odbiorcy
        ValidateLifetime = true, // sprawdzanie czy token nie wygas³
        ValidateIssuerSigningKey = true, // weryfikacja podpisu
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // nadawca z pliku konfiguracyjnego
        ValidAudience = builder.Configuration["Jwt:Audience"], // odbiorca z pliku konfiguracyjnego
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)) // klucz podpisu
    };
});

// Pozwala na domyœln¹ walidacjê modelu (automatyczny kod 400 jeœli model jest niepoprawny)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

// Autoryzacja
builder.Services.AddAuthorization();

// CORS - zezwolenie na po³¹czenia z frontendu w Angular
var allowedOrigins = builder.Configuration.GetValue<String>("allowedOrigins")!.Split(",");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Inicjalizacja przyk³adowych kontaktów po starcie aplkiacji
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.Seed();
}


// Swagger dzia³a tylko w trybie deweloperskim
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware CORS
app.UseCors();

// Middleware obs³uguj¹cy uwierzytelnienie JWT
app.UseAuthentication();

// Middleware autoryzacji
app.UseAuthorization();

// Mapowanie kontrolerów
app.MapControllers();

app.Run();

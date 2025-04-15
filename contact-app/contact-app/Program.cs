using contact_app.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Dodanie DbContext z baz� danych InMemory
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("AppDb"));

// Rejestracja kontroler�w (API)
builder.Services.AddControllers();

// Swagger - dokumentacja API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Rejestracja inicjalizacji kontakt�w
builder.Services.AddScoped<DataSeeder>();

// Konfiguracja JWT Bearer Authentication
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true, // sprawdzanie, czy token ma poprawnego nadawc�
        ValidateAudience = true, // sprawdzanie odbiorcy
        ValidateLifetime = true, // sprawdzanie czy token nie wygas�
        ValidateIssuerSigningKey = true, // weryfikacja podpisu
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // nadawca z pliku konfiguracyjnego
        ValidAudience = builder.Configuration["Jwt:Audience"], // odbiorca z pliku konfiguracyjnego
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)) // klucz podpisu
    };
});

// Pozwala na domy�ln� walidacj� modelu (automatyczny kod 400 je�li model jest niepoprawny)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

// Autoryzacja
builder.Services.AddAuthorization();

// CORS - zezwolenie na po��czenia z frontendu w Angular
var allowedOrigins = builder.Configuration.GetValue<String>("allowedOrigins")!.Split(",");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Inicjalizacja przyk�adowych kontakt�w po starcie aplkiacji
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.Seed();
}


// Swagger dzia�a tylko w trybie deweloperskim
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware CORS
app.UseCors();

// Middleware obs�uguj�cy uwierzytelnienie JWT
app.UseAuthentication();

// Middleware autoryzacji
app.UseAuthorization();

// Mapowanie kontroler�w
app.MapControllers();

app.Run();

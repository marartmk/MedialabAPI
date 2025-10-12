using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediaLabAPI.Configurations;
using Microsoft.EntityFrameworkCore;
using MediaLabAPI.Data;
using MediaLabAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// ➤ Configura JwtSettings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtKey = builder.Configuration["JwtSettings:Key"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ➤ Configura CORS per permettere le richieste da React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost",
                "https://localhost",
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:3000",
                "https://medialabnexttest.dea40.it"                
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ➤ Aggiungi Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepairService, RepairService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IQuickRepairNoteService, QuickRepairNoteService>();
builder.Services.AddScoped<IRepairPartsService, RepairPartsService>();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

var app = builder.Build();

// ➤ Usa Swagger in ambiente di sviluppo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ➤ Applica CORS
app.UseCors("AllowReactFrontend");

//app.UseHttpsRedirection(); // puoi riattivarlo se vuoi usare solo HTTPS

// ➤ Authentication deve stare prima di Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

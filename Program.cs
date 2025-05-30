using ApiPrincipal_Ferremas.Models;
using ApiPrincipal_Ferremas.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Servicio para encriptar contraseñas
builder.Services.AddScoped<IPasswordHasher<BaseUser>, PasswordHasher<BaseUser>>();

// Configuarión de la autenticación con JWT
#pragma warning disable CS8604 // Posible argumento de referencia nulo
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
#pragma warning restore CS8604 // Posible argumento de referencia nulo

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
       policy => policy.WithOrigins("https://localhost:4200")
                       .AllowAnyHeader()
                       .AllowAnyMethod());
});

// Configuración de Permisos de navegaciones
builder.Services.AddAuthorization(options =>
{
    // Clientes
    options.AddPolicy("ClienteOnly", policy =>
        policy.RequireClaim("rol", "cliente"));

    // Administradores
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("rol", "administrador"));

    // Encargados
    options.AddPolicy("EncargadoOnly", policy =>
        policy.RequireClaim("rol", "encargado"));

    // Bodegueros
    options.AddPolicy("BodegueroOnly", policy =>
        policy.RequireClaim("rol", "bodeguero"));

    // Contadores
    options.AddPolicy("ContadorOnly", policy =>
        policy.RequireClaim("rol", "contador"));

});

// Configuración de JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    });

// Conexión con MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SistemaFerremasContext>(opt =>
    opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<EmpleadoService>();
builder.Services.AddHttpClient<ResendEmailService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

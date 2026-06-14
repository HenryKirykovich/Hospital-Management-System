using System.Text;
using HospitalManagement.Server.Data;
using HospitalManagement.Server.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- MongoDB ---
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<MongoDbContext>();

// --- JWT Authentication ---
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // Allow JWT token from SignalR query string (required for WinForms SignalR client)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    context.Token = accessToken;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// --- SignalR ---
builder.Services.AddSignalR();

// --- Controllers + OpenAPI ---
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// --- CORS: allow WinForms client on localhost ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// SignalR hubs will be mapped here as they are created in later steps
// app.MapHub<HospitalHub>("/hubs/hospital");

app.Run();

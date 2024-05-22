using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using UserFcApi.Data;
using System.Text;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using UserFcApi.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "User ID=postgres;Password=postgres;Server=postgres;Port=5432;Database= SampleDbDriver;Integrated Security=true;Pooling=true;";
builder.Services.AddDbContext<APIDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddDbContext<APIDbContext>(options =>  
            options.UseNpgsql());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "apiWithAuthBackend",
            ValidAudience = "apiWithAuthBackend",
            IssuerSigningKey = new SymmetricSecurityKey(
                // todo put it into secrests - step 3
                Encoding.UTF8.GetBytes("!SomethingSecret!")
            ),
        };
    });
builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<APIDbContext>();
builder.Services.AddScoped<TokenService, TokenService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ETNA CLO-5 : User MicroService");
        });
//}
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<APIDbContext>();

    dbContext.Database.Migrate();
}
app.MapControllers();

app.Run();



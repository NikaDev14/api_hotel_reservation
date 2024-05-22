using hotel_fc_ms.Http;
using hotel_fc_ms.Data;
using hotel_fc_ms.Seeds;
using hotel_fc_ms.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using hotel_fc_ms.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "User ID=postgres;Password=postgres;Server=room_hotel_pg;Port=5432;Database= room_hotel_db_driver;Integrated Security=true;Pooling=true;";
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddHostedService<SeedHotelService>();
builder.Services.AddHostedService<SeedRoomService>();
builder.Services.AddHostedService<SeedRoomHotelService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddControllers().AddJsonOptions(x =>
                                          x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddControllers()
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ETNA CLO-5 : Hotel MicroService");
        });
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApiDbContext>();

    // Migrate the database
    dbContext.Database.Migrate();

    // Seed the database with initial data (if needed)
    // You can add your own logic here to seed the database with initial data
}

app.MapControllers();

app.Run();

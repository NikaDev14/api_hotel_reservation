using reservation_fc_ms.Data;
using Microsoft.EntityFrameworkCore;
using reservation_fc_ms.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = "User ID=postgres;Password=postgres;Server=reservations_pg;Port=5432;Database= ReservationsDbDriver;Integrated Security=true;Pooling=true;";
builder.Services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
//builder.Services.AddDbContext<APIDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ETNA CLO-5 : Reservation Microservice");
});

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApiDbContext>();

    dbContext.Database.Migrate();

}

app.MapControllers();

app.Run();

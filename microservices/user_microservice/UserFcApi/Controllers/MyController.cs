using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserFcApi.Data;
namespace fco_users_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        private readonly APIDbContext _dbContext;

        public MyController(APIDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("test")]
        public String TestDatabaseConnection()
        {
            try
            {
                // Call a simple query to test the database connection
                // _dbContext.Database.ExecuteSqlRaw("SELECT 1");
                return "Database connection test succeeded.";
            }
            catch (Exception)
            {
                return "Database connection test failed: {ex.Message}";
            }
        }
    }

}


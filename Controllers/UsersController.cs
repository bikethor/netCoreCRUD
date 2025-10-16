using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreExercise.Data;
using NetCoreExercise.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace NetCoreExercise.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .FromSqlRaw("EXEC spi_get_users")
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var users = await _context.Users
                .FromSqlRaw("EXEC spi_get_user_by_id @Id = {0}", id)
                .AsNoTracking()
                .ToListAsync();

            var user = users.FirstOrDefault();

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "spi_insert_user";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Username", user.Username));
            command.Parameters.Add(new SqlParameter("@FirstName", user.FirstName));
            command.Parameters.Add(new SqlParameter("@LastName", user.LastName));
            command.Parameters.Add(new SqlParameter("@operationDate", user.operationDate));

            var result = await command.ExecuteScalarAsync();
            user.Id = Convert.ToInt32(result);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PATCH: api/users/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "spi_update_user";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));
            command.Parameters.Add(new SqlParameter("@Username", updatedUser.Username));
            command.Parameters.Add(new SqlParameter("@FirstName", updatedUser.FirstName));
            command.Parameters.Add(new SqlParameter("@LastName", updatedUser.LastName));
            command.Parameters.Add(new SqlParameter("@operationDate", updatedUser.operationDate));

            await command.ExecuteNonQueryAsync();

            return Ok(updatedUser);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = "spi_delete_user";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@Id", id));

            await command.ExecuteNonQueryAsync();

            return NoContent();
        }
    }
}

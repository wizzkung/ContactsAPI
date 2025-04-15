using Contacts.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Contacts
{
    public class Service
    {
        IConfiguration _configuration;
        public Service(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task AddOrUpdateContactAsync(Contact c)
        {
            using var connection = new SqlConnection(_configuration["db"]);
            await connection.ExecuteAsync(
                "pAdd",
                new
                {
                    first_name = c.first_name,
                    last_name = c.last_name,
                    email = c.email,
                    number = c.number
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            using var connection = new SqlConnection(_configuration["db"]);
            var contacts = await connection.QueryAsync<Contact>("pGetAll", commandType: CommandType.StoredProcedure);
            return contacts.ToList();
        }

        public async Task DeleteContactAsync(int id)
        {
            using var connection = new SqlConnection(_configuration["db"]);
            await connection.ExecuteAsync("pDelete", new { id }, commandType: CommandType.StoredProcedure);
        }
    }
}

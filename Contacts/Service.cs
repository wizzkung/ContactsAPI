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
            try
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
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 2627:
                        throw new Exception("Ошибка: дублирование записи для email или номера.", ex);
                    case 547:
                        throw new Exception("Ошибка: нарушение ограничения внешнего ключа.", ex);
                    default:
                        throw new Exception("Произошла ошибка при работе с базой данных.", ex);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка при добавлении или обновлении контакта.", ex);
            }
        }

        public async Task<List<Contact>> GetContactsAsync()
        {
            try
            {
                using var connection = new SqlConnection(_configuration["db"]);
                var contacts = await connection.QueryAsync<Contact>(
                    "pGetAll",
                    commandType: CommandType.StoredProcedure
                );
                return contacts.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка при получении контактов.", ex);
            }
        }

        public async Task DeleteContactAsync(int id)
        {
            try
            {
                using var connection = new SqlConnection(_configuration["db"]);
                await connection.ExecuteAsync(
                    "pDelete",
                    new { id },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка при удалении контакта.", ex);
            }
        }

        public async Task<(IEnumerable<Contact> Contacts, int Total)> GetPages(int page, int size)
        {
            try
            {
                if (page < 1)
                {
                    page = 1;
                }

                using var connection = new SqlConnection(_configuration["db"]);
                using var multi = await connection.QueryMultipleAsync(
                    "pGetPaged",
                    new { @pageNum = page, @pageSize = size },
                    commandType: CommandType.StoredProcedure
                );

                var contacts = await multi.ReadAsync<Contact>();
                var total = await multi.ReadFirstAsync<int>();

                return (contacts.ToList(), total);
            }
            catch (Exception ex)
            {
                throw new Exception("Произошла ошибка при получении страницы контактов.", ex);
            }
        }
    }

}


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly Service service;
        public ContactController(Service service) {
            this.service = service;
        }



        [HttpGet("GetAll")]
        public async Task<IActionResult> GetContacts()
        {
            var contacts = await service.GetContactsAsync();
            return Ok(contacts);
        }

        [HttpPost("AddOrUpdate")]
        public async Task<IActionResult> AddOrUpdateContact([FromBody] Models.Contact contact)
        {
            if (contact == null)
            {
                return BadRequest();
            }
            await service.AddOrUpdateContactAsync(contact);
            return Ok();
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            await service.DeleteContactAsync(id);
            return Ok();
        }




        //Добавил серверную пагинацию, так как изначально думал, что нужна клиентская
        [HttpGet("paged/{page}/{size}")]
        public async Task<IActionResult> GetPaged(int page, int size)
        {
            var (contacts, total) = await service.GetPages(page, size);
            return Ok(new { data = contacts, total });
        }




    }
}

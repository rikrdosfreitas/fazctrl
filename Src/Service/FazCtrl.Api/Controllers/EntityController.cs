using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace FazCtrl.Api.Controllers
{
  
    public class EntityController : ApiController
    {
        public EntityController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {


            return Ok();
        }

       
    }
}
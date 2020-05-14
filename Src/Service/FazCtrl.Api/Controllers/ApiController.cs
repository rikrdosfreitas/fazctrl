using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FazCtrl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected ApiController(IMediator mediator)
        {
            Mediator = mediator;
        }

        protected IMediator Mediator { get; set; }
    }
}
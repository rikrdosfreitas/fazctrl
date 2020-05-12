using System;
using System.Collections.Generic;
using System.Linq;
using FazCtrl.Application.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FazCtrl.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _dispatcher;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator dispatcher)
        {
            _logger = logger;
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var id = Guid.Parse("c2b1f57d-a25b-4d17-b78f-815a5274d11f");
            //_dispatcher.Send(new CreateGrazingCommand(id, "Teste",10m));

            _dispatcher.Send(new AddAnimalInGrazingCommand(id, new List<GrazingAnimalViewModel> { new GrazingAnimalViewModel(id,20)}));


            return Ok();

        }
    }
}

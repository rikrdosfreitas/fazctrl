using AutoMapper;
using FazCtrl.Contract.Interfaces;
using FazCtrl.Infrastructure.Azure;

namespace FazCtrl.Api
{
   public class Mapper:Profile
    {
        public Mapper()
        {
            CreateMap<IEventData, EventTableServiceEntity>();

            CreateMap<EventData, EventTableServiceEntity>();

            CreateMap<EventTableServiceEntity, EventData>();

            CreateMap<EventTableServiceEntity, IEventData>().As<EventData>();



        }
    }
}

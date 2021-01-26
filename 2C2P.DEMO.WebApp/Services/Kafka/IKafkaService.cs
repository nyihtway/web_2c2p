using _2C2P.DEMO.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2C2P.DEMO.WebApp.Services.Kafka
{
    public interface IKafkaService
    {
        Task PublishToKafka<T>(List<T> data) where T : IntegrationEventBase;
    }
}

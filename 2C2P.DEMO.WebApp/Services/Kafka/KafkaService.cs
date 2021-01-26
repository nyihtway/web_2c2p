using _2C2P.DEMO.Domain.Events;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2C2P.DEMO.WebApp.Services.Kafka
{
    public class KafkaService : IKafkaService
    {
        private readonly IBusClient _busClient;
        private readonly int _batchCount;
        private readonly IConfiguration _config;

        public KafkaService(IBusClient busClient, IConfiguration config)
        {
            _busClient = busClient ?? throw new ArgumentNullException(nameof(busClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _batchCount = _config.GetValue<int>("BatchCount");
        }
        public async Task PublishToKafka<T>(List<T> data) where T : IntegrationEventBase
        {
            var eol = false;
            var startIndex = 0;
            while (!eol && data.Any())
            {
                var batch = data.GetRange(startIndex, _batchCount > data.Count() ? data.Count() : _batchCount);
                startIndex += _batchCount;

                eol = (startIndex > data.Count());
                if (batch.Any())
                {
                    _busClient.Publish(batch);
                }
            }
        }
    }
}

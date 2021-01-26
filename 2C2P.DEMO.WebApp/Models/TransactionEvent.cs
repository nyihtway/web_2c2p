using _2C2P.DEMO.Domain.Events;
using _2C2P.DEMO.Infrastructure.Interfaces;
using System;

namespace _2C2P.DEMO.WebApp.Models
{
    public class TransactionEvent : IntegrationEventBase, IMapFrom<TransactionDto>
    {
        public string TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Status { get; set; }

        public TransactionEvent(Guid id)
        {
            Id = id;
        }

        public TransactionEvent()
        {
            Id = Guid.NewGuid();
        }

        //public void Mapping()
        //{

        //}
    }
}

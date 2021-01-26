using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _2C2P.DEMO.WebApp.Models
{
    [XmlRoot("Transactions")]
    public class TransactionXmlDtoList
    {
        [XmlArray]
        [XmlArrayItem("Transaction")]
        public List<TransactionXmlDto> Transactions { get; set; }
    }

    [XmlRoot("Transaction")]
    public class TransactionXmlDto
    {
        public AttributeProperty TransactionId { get; set; }
        public PaymentDetail PaymentDetails { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
    }

    public class AttributeProperty
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }

    public class PaymentDetail
    {
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}

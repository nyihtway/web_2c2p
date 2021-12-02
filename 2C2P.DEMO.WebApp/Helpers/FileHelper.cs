using _2C2P.DEMO.WebApp.Models;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace _2C2P.DEMO.WebApp.Helpers
{
    public static class FileHelper
    {
        public static List<string> ValidateUploadFile(IFormFile file, string extension)
        {
            List<string> errors = new List<string>();


            if (file.Length > 1048576)
            {
                errors.Add("File size larger than 1MB");
            }


            if (extension != ".csv" && extension != ".xml")
            {
                errors.Add("Unknown file type");
            }

            return errors;
        }

        public static List<TransactionEvent> ExtractXML(IFormFile file, string env)
        {
            var result = new List<TransactionEvent>();
            using (var fileStream = new StreamReader(file.OpenReadStream()))
            {
                string xmlString = fileStream.ReadToEnd();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlString);

                string jsonStr = JsonConvert.SerializeXmlNode(xmlDoc);

                var xmlDto = JsonConvert.DeserializeObject<TransactionXmlDto>(jsonStr);

                foreach (var transaction in xmlDto.Transactions.Transaction)
                {
                    var transEvent = new TransactionEvent()
                    {
                        Amount = Convert.ToDecimal(transaction.PaymentDetails.Amount),
                        CurrencyCode = transaction.PaymentDetails.CurrencyCode,
                        Env = env,
                        Status = ConversionHelper.ConvertStatus(transaction.Status),
                        TransactionDate = transaction.TransactionDate,
                        TransactionId = transaction.Id

                    };

                    result.Add(transEvent);
                }

                return result;
            }
        }

        public static List<TransactionEvent> ExtractCSV(IFormFile file, string env)
        {
            //var config = new CsvConfiguration(CultureInfo.InvariantCulture, newLine: Environment.NewLine);

            var result = new List<TransactionEvent>();
            //using (var fileStream = file.OpenReadStream())
            //{
            //    TextReader textReader = new StreamReader(fileStream);
            //    var csvReader = new CsvReader(textReader, config);
            //    var records = csvReader.GetRecords<TransactionDto>();
            //}

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                string line = string.Empty;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] strRow = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    DateTime transDate = new DateTime();

                    DateTime.TryParseExact(TrimQuotes(strRow[3]), "dd/MM/yyyy hh:mm:ss", null, DateTimeStyles.None, out transDate);

                    var transaction = new TransactionEvent(Guid.NewGuid())
                    {
                        TransactionId = TrimQuotes(strRow[0]),
                        Amount = Convert.ToDecimal(TrimQuotes(strRow[1])),
                        CurrencyCode = TrimQuotes(strRow[2]),
                        TransactionDate = transDate,
                        Status = ConversionHelper.ConvertStatus(TrimQuotes(strRow[4])),
                        Env = env
                    };

                    result.Add(transaction);
                }

            }

            return result;
        }

        public static string TrimQuotes(string text)
        {
            return text.TrimStart('"').TrimEnd('"');
        }
    }
}

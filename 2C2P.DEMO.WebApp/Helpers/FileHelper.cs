using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using _2C2P.DEMO.WebApp.Models;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

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

        public static List<TransactionEvent> ExtractXML(IFormFile file)
        {
            using (var fileStream = file.OpenReadStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TransactionXmlDtoList));
                var transactionXmlDtoList = (TransactionXmlDtoList)serializer.Deserialize(fileStream);


                return new List<TransactionEvent>();
            }
        }

        public static List<TransactionEvent> ExtractCSV(IFormFile file, string env)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
            };

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
                        TransactionDate =  transDate,
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

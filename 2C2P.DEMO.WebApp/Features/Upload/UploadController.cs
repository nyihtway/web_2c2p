﻿using _2C2P.DEMO.WebApp.Helpers;
using _2C2P.DEMO.WebApp.Models;
using _2C2P.DEMO.WebApp.Services.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _2C2P.DEMO.WebApp.Features.Upload
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        public readonly IKafkaService _kafkaService;
        public readonly IConfiguration _config;
        public readonly string _env;
        public UploadController(IKafkaService kafkaService, IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _kafkaService = kafkaService ?? throw new ArgumentNullException(nameof(kafkaService));
            _env = _config.GetValue<string>("Env");
        }
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = "UploadFiles";
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string extension = Path.GetExtension(file.FileName);
                List<TransactionEvent> transactions = new List<TransactionEvent>();

                if (file.Length > 0)
                {
                    List<string> errors = FileHelper.ValidateUploadFile(file, extension);

                    if (errors.Any())
                    {
                        return BadRequest(errors);
                    }

                    if (extension == ".xml")
                    {
                        transactions = FileHelper.ExtractXML(file, _env);
                    }

                    else if (extension == ".csv")
                    {
                        transactions = FileHelper.ExtractCSV(file, _env);
                    }

                    errors = TransactionHelper.ValidateTransactions(transactions);

                    if (errors.Any())
                    {
                        return BadRequest(errors);
                    }

                    _kafkaService.PublishToKafka<TransactionEvent>(transactions);

                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}

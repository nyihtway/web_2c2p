using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2C2P.DEMO.WebApp.Helpers
{
    public static class ConversionHelper
    {
        public static string ConvertStatus(string status)
        {
            switch (status)
            {
                case "Approved":
                    return "A";
                case "Failed": case "Rejected":
                    return "R";
                case "Finished": case "Done":
                    return "D";
                default:
                    return "";
            }
        }
    }
}

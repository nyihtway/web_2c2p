using _2C2P.DEMO.WebApp.Models;
using System;
using System.Collections.Generic;

namespace _2C2P.DEMO.WebApp.Helpers
{
    public static class TransactionHelper
    {
        internal static List<string> ValidateTransactions(List<TransactionEvent> transactions)
        {
            var errors = new List<string>();

            foreach (var transaction in transactions)
            {
                if (transaction.Amount == null)
                    errors.Add($"Amount is null for transaction {transaction.TransactionId}");

                if (string.IsNullOrEmpty(transaction.CurrencyCode))
                    errors.Add($"CurrencyCode is null for transaction {transaction.TransactionId}");

                if (string.IsNullOrEmpty(transaction.Status))
                    errors.Add($"Status is null for transaction {transaction.TransactionId}");

                if (transaction.TransactionDate == new DateTime())
                    errors.Add($"TransactionDate is invalid for transaction {transaction.TransactionId}");
                if(string.IsNullOrEmpty(transaction.TransactionId))
                    errors.Add($"TransactionId is empty for transaction {transaction.TransactionId}");
                if (transaction.TransactionId.Trim().Length > 50)
                    errors.Add($"TransactionId is too long for transaction {transaction.TransactionId}");
            }

            return errors;
        }
    }
}

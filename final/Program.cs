using final;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace firstFinal1
{
    internal class Program
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly string dataFile = "datas.json";

        #region
        static List<CardUserInfo> ParseCardUserInfo(string json)
        {
            JsonDocument doc = JsonDocument.Parse(json);

            List<CardUserInfo> cardInfos = doc.RootElement.EnumerateArray()
                .Select(element =>
                {
                    return new CardUserInfo
                    {
                        FirstName = element.GetProperty("firstName").GetString(),
                        LastName = element.GetProperty("lastName").GetString(),
                        Details = ParseCardDetails(element.GetProperty("cardDetails")),
                        PinCode = element.GetProperty("pinCode").GetString(),
                        TransactionHistory = ParseTransactionHistory(element.GetProperty("transactionHistory"))
                    };
                })
        .ToList();

            return cardInfos;
        }
        #endregion

        #region
        static CardDetails ParseCardDetails(JsonElement element)
        {
            return new CardDetails
            {
                CardNumber = element.GetProperty("cardNumber").GetString(),
                ExpirationDate = element.GetProperty("expirationDate").GetString(),
                CVC = element.GetProperty("CVC").GetString()
            };
        }
        #endregion

        #region
        static List<Transaction> ParseTransactionHistory(JsonElement element)
        {
            return element.EnumerateArray()
         .Select(transactionElement =>
         {
             var transaction = new Transaction();

             
             foreach (var property in transactionElement.EnumerateObject())
             {
                 switch (property.Name)
                 {
                     case "transactionDate":
                         transaction.TransactionDate = property.Value.GetDateTime();
                         break;
                     case "transactionType":
                         transaction.TransactionType = property.Value.GetString();
                         break;
                     case "amountGEL":
                         transaction.AmountGel = property.Value.GetDecimal();
                         break;
                     case "amountUSD":
                         transaction.AmountUsd = property.Value.GetDecimal();
                         break;
                     case "amountEUR":
                         transaction.AmountEur = property.Value.GetDecimal();
                         break;
                 }
             }

             return transaction;
         })
         .ToList(); 
        }
        #endregion
        static void Main(string[] args)
        {
            try
            {
                string dataFilePath = @"C:\Users\Anano\source\repos\final\final\datas.json";
                string json = File.ReadAllText(dataFilePath);
                List<CardUserInfo> cardInfos = ParseCardUserInfo(json);

                while (true)
                {
                    Console.WriteLine("Enter Card Number: ");
                    string cardNumber = Console.ReadLine();

                    CardUserInfo cardInfo = cardInfos.FirstOrDefault(item => item.Details.CardNumber == cardNumber);

                    if (cardInfo == null)
                    {
                        Console.WriteLine("Card data is incorrect, try again!");
                        continue;
                    }
                    else
                    {
                        Console.Write("Enter your PIN: ");
                        string pin = Console.ReadLine();
                        if (cardInfo.PinCode != pin)
                        {
                            Console.WriteLine("Incorrect PIN. Please try again.");
                            continue;
                        }
                    }


                    Console.WriteLine("Login successful!");

                    int choice;
                    bool validChoice = false;

                    while (!validChoice)
                    {

                        Console.WriteLine("Choose an action: ");
                        Console.WriteLine("1. View balance");
                        Console.WriteLine("2. Withdraw money");
                        Console.WriteLine("3. Last 5 operations");
                        Console.WriteLine("4. Deposit money");
                        Console.WriteLine("5. Change PIN");
                        Console.WriteLine("6. Currency conversion");
                        Console.WriteLine("7. Exit from program");


                        if (!int.TryParse(Console.ReadLine(), out choice))
                        {
                            Console.WriteLine("Invalid choice. Please try again.");

                        }

                        switch (choice)
                        {
                            case 1:
                                Console.WriteLine($"Your balance: {cardInfo.Balance}");
                                break;
                            case 2:
                                WithdrawMoney(cardInfo);
                                break;
                            case 3:
                                PrintLastFiveOperations(cardInfo);
                                break;
                            case 4:
                                DepositMoney(cardInfo);
                                break;
                            case 5:
                                ChangePIN(cardInfo);
                                break;
                            case 6:
                                CurrencyConversion(cardInfo);
                                break;

                            case 7:
                                Console.WriteLine("Thank you for using the ATM simulator!");
                                return;
                            default:
                                Console.WriteLine("Invalid choice. Please try again.");
                                break;
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: Data file not found.");
            }
            catch (JsonException)
            {
                Console.WriteLine("Error: Json data is invalid");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred. Please contact support.");
                Console.WriteLine(ex.Message);
            }
        }

        static void WithdrawMoney(CardUserInfo cardInfo)
        {
            Console.Write("Enter the amount to withdraw: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }
           

            if (withdrawAmount <= 0 || withdrawAmount > cardInfo.Balance)
            {
                Console.WriteLine("Invalid withdrawal amount. Enter a valid one.");
                return;
            }

            cardInfo.Balance -= withdrawAmount;
            Console.WriteLine($"Successfully withdrawn {withdrawAmount}. Your new balance: {cardInfo.Balance}");
        }

        static void PrintLastFiveOperations(CardUserInfo cardInfo)
        {
            Console.WriteLine("Last 5 operations:");
            foreach (var transaction in cardInfo.TransactionHistory)
            {
                Console.WriteLine($"{transaction.TransactionDate}: {transaction.TransactionType} - GEL: {transaction.AmountGel}, USD: {transaction.AmountUsd}, EUR: {transaction.AmountEur}");
            }
        }

        static void DepositMoney(CardUserInfo cardInfo)
        {
            Console.Write("Enter the amount to deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                return;
            }

            if (depositAmount <= 0)
            {
                Console.WriteLine("Invalid deposit amount. Please enter a positive value.");
                return;
            }

            cardInfo.Balance += depositAmount;
            Console.WriteLine($"Successfully deposited {depositAmount}. Your new balance: {cardInfo.Balance}");
        }

        static void ChangePIN(CardUserInfo cardInfo)
        {
            Console.Write("Enter your new PIN: ");
            string newPin = Console.ReadLine();
            if (string.IsNullOrEmpty(newPin))
            {
                Console.WriteLine("Invalid PIN. Please try again.");
                return;
            } 

            cardInfo.PinCode = newPin;
            Console.WriteLine("PIN successfully changed.");
        }
        static void CurrencyConversion(CardUserInfo cardInfo)
        {
            Console.WriteLine("Exchange:");
            Console.WriteLine("1. GEL to USD");
            Console.WriteLine("2. GEL to EUR");
            Console.WriteLine("3. USD to GEL");
            Console.WriteLine("4. USD to EUR");
            Console.WriteLine("5. EUR to GEL");
            Console.WriteLine("6. EUR to USD");
            Console.WriteLine("Choose an option:");

            if (!int.TryParse(Console.ReadLine(), out int option))
            {
                Console.WriteLine("Invalid option. Please try again.");
                return;
            }

            decimal amount;
            decimal convertedAmount;
            switch (option)
            {
                case 1:
                    Console.Write("Enter amount in GEL: ");
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        return;
                    }
                    convertedAmount = ConvertGELtoUSD(amount);
                    Console.WriteLine($"Converted amount: {convertedAmount} USD");
                    break;
                case 2:
                    Console.Write("Enter amount in GEL: ");
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        return;
                    }
                    convertedAmount = ConvertGELtoEUR(amount);
                    Console.WriteLine($"Converted amount: {convertedAmount} EUR");
                    break;
                case 3:
                    Console.Write("Enter amount in USD: ");
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        return;
                    }
                    convertedAmount = ConvertUSDtoGEL(amount);
                    Console.WriteLine($"Converted amount: {convertedAmount} GEL");
                    break;
                case 4:
                    Console.Write("Enter amount in USD: ");
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        return;
                    }
                    convertedAmount = ConvertUSDtoEUR(amount);
                    Console.WriteLine($"Converted amount: {convertedAmount} EUR");
                    break;
                case 5:
                    Console.Write("Enter amount in EUR: ");
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        return;
                    }
                    convertedAmount = ConvertEURtoGEL(amount);
                    Console.WriteLine($"Converted amount: {convertedAmount} GEL");
                    break;
                case 6:
                    Console.Write("Enter amount in EUR: ");
                    if (!decimal.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid number.");
                        return;
                    }
                    convertedAmount = ConvertEURtoUSD(amount);
                    Console.WriteLine($"Converted amount: {convertedAmount} USD");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        static decimal ConvertGELtoUSD(decimal amount)
        {
            return amount * 0.3M;
        }

        static decimal ConvertGELtoEUR(decimal amount)
        {
            return amount * 0.25M;
        }

        static decimal ConvertUSDtoGEL(decimal amount)
        {
            return amount * 3.5M;
        }

        static decimal ConvertUSDtoEUR(decimal amount)
        {
            return amount * 0.85M;
        }

        static decimal ConvertEURtoGEL(decimal amount)
        {
            return amount * 4M;
        }

        static decimal ConvertEURtoUSD(decimal amount)
        { 
            return amount * 1.2M;
        }

    }
}

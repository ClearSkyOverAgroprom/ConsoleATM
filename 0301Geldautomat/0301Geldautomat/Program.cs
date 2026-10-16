using System.Globalization;
using System.Text.RegularExpressions;
using ClassLibrary2;

namespace _0301Geldautomat;

class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            var bankDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BankData.txt");

            if (!File.Exists(bankDataPath))
            {
                Console.WriteLine($"Bank file not found at {bankDataPath}");
                return;
            }


            var content = File.ReadAllText(bankDataPath);
            Console.WriteLine("Please enter your Pin.");
            var pinCheck = false;
            string[] customerData = [];

            while (!pinCheck)
            {
                var pin = Console.ReadLine();
                if (!Regex.IsMatch(pin, @"^\d{4}$"))
                {
                    Console.Clear();
                    Console.WriteLine("Invalid Pin, try again.");
                }
                else
                {
                    try
                    {
                        customerData = BankHelper.GetCustomerData(bankDataPath, pin);
                        pinCheck = true;
                    }
                    catch
                    {
                        Console.Clear();
                        Console.WriteLine("Account does not Exist, try again.");
                    }
                }
            }

            Console.Clear();
            Console.WriteLine($" Account Number: {customerData[1]}");
            Console.WriteLine($" Balance: {customerData[2]}$");
            Console.WriteLine(" 1. Withdraw");
            Console.WriteLine(" 2. Deposit");
            Console.WriteLine(" 3. Exit");

            var userChoiceKey = Console.ReadKey(true);

            if (!int.TryParse(userChoiceKey.KeyChar.ToString(), out var userChoiceInt))
            {
                Console.WriteLine("Invalid Input, try again.");
                return;
            }

            switch (userChoiceInt)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Enter Withdrawal Amount");
                    break;

                case 2:
                    Console.Clear();
                    Console.WriteLine("Enter Deposit Amount");
                    break;
                case 3:
                    Console.WriteLine("Exiting ATM");
                    Task.Delay(2000);
                    return;
                default:
                    Console.Clear();
                    return;
            }
            var fundsAllowed = false;
            var fundsParsed = false;
            var regexMatch = false;
            var cashAmount = string.Empty;
            decimal customerBalanceGlobal = 0;

            while (true)
            {
                fundsAllowed = false;
                fundsParsed = false;
                regexMatch = false;
                
                cashAmount = Console.ReadLine()?.Replace(".", ",");
                if (cashAmount != null && !Regex.IsMatch(cashAmount, @"^\d+[,]\d{2}|\d+|\d +"))
                {
                    Console.WriteLine("Invalid Format, try Again.");
                }
                else
                {
                    regexMatch = true;
                }
                
                if (!decimal.TryParse(cashAmount, out var cashAmountVar))
                {
                    Console.WriteLine("Parsing failed, try Again.");
                }
                else
                {
                    fundsParsed = true;
                }

                string customerData2Formated = customerData[2].Replace(".", ",");
                if (!decimal.TryParse(customerData2Formated, out var customerBalance))
                {
                    return;
                }

                if (userChoiceInt == 1 && regexMatch && fundsParsed)
                {
                    if (customerBalance - cashAmountVar >= 0)
                    {
                        customerBalance -= cashAmountVar;
                        customerBalanceGlobal = customerBalance;
                        fundsAllowed = true;
                    }
                    else
                    {
                        Console.WriteLine("Negative Balance not permitted, try Again.");
                    }

                } 
                if (userChoiceInt == 2 && regexMatch && fundsParsed)
                {
                    if (customerBalance + cashAmountVar >= 0)
                    {
                        customerBalance += cashAmountVar;
                        customerBalanceGlobal = customerBalance;
                        fundsAllowed = true;
                    }
                    else
                    {
                        Console.WriteLine("Negative Balance not permitted, try Again.");
                    }
                    
                }
                else if (userChoiceInt == 3)
                {
                    Console.WriteLine("Exiting ATM");
                    Task.Delay(2000).Wait();
                    return;
                }

                if (regexMatch && fundsAllowed && fundsParsed)
                {
                    break;
                }
            }

            Console.Clear();
            Console.WriteLine($"New Balance: {customerBalanceGlobal}");
            Task.Delay(2000).Wait();
            Console.Clear();

            BankHelper.WriteCustomerBalance(bankDataPath, customerData, customerBalanceGlobal);
        }
    }
}
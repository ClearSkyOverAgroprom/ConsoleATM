using System.Diagnostics;

namespace ClassLibrary2;

public class BankHelper
{
    public static string[] GetCustomerData(string bankDataPath, string pin)
    {
            var match = File.ReadLines(bankDataPath)
                .Select((line, index) => new { Line = line, LineNumber = index + 1 })
                .Where(x =>
                {
                    var pinCheck = x.Line.Split(';').FirstOrDefault()?.Trim();
                    return string.Equals(pinCheck, pin, StringComparison.OrdinalIgnoreCase);
                });

            var bankDataLine = match.First().Line;
            string[] customerData = bankDataLine.Split(';');
            return customerData;
        
    }

    public static void WriteCustomerBalance(string bankDataPath, string[] customerData, decimal customerBalance)
    {
        string[] lines = File.ReadAllLines(bankDataPath);

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(customerData[1]))
            {
                lines[i] = lines[i].Replace(customerData[2], customerBalance.ToString());
            }
        }
        
        File.WriteAllLines(bankDataPath, lines);
        Debug.WriteLine(bankDataPath);
    }
}
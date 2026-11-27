using System;
using System.Globalization;

class Program
{
    static string CalculateInterest(double initial_deposit, int years, double interest_rate)
    {
        string result = "";
        double amount = initial_deposit;

        for (int i = 1; i <= years; i++)
        {
            amount *= 1 + interest_rate / 100;

            result += $"Год {i}: {amount.ToString("F2", CultureInfo.InvariantCulture)} руб.";

            if (i < years)
                result += "\n";
        }

        return result;
    }

    static void Main()
    {
        Console.WriteLine(CalculateInterest(1000, 3, 10));
    }
}
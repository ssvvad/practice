using System;
using System.Globalization;

// консольная программа для расчёта вклада со сложными процентами
class Program
{
    // рассчитывает сумму вклада на конец каждого года и формирует текст отчёта
    // initial_deposit — начальная сумма вклада в рублях
    // years — количество лет начисления процентов
    // interest_rate — годовая процентная ставка в процентах
    // возвращает: строка с суммами вклада по годам
    static string CalculateInterest(double initial_deposit, int years, double interest_rate)
    {
        // строка для накопления результатов расчёта
        string result = "";
        // текущая сумма вклада с учётом уже начисленных процентов
        double amount = initial_deposit;

        // для каждого года повторяем начисление процентов
        for (int i = 1; i <= years; i++)
        {
            // увеличиваем текущую сумму на годовой процент (капитализация)
            amount *= 1 + interest_rate / 100;

            // формат F2 задаёт два знака после запятой; InvariantCulture обеспечивает единый формат числа
            result += $"Год {i}: {amount.ToString("F2", CultureInfo.InvariantCulture)} руб.";

            // добавляем перенос строки только между годами, без лишнего переноса в конце
            if (i < years)
                result += "\n";
        }

        // возвращаем готовый многострочный отчёт
        return result;
    }

    // метод Main: демонстрирует расчёт вклада 1000 рублей на три года под 10 процентов
    static void Main()
    {
        Console.WriteLine(CalculateInterest(1000, 3, 10));
    }
}
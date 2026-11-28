using System;
using System.Globalization;

/// <summary>
/// Консольная программа для расчёта вклада со сложными процентами.
/// </summary>
class Program
{
    /// <summary>
    /// Рассчитывает сумму вклада на конец каждого года и формирует текст отчёта.
    /// </summary>
    /// <param name="initial_deposit">Начальная сумма вклада в рублях.</param>
    /// <param name="years">Количество лет начисления процентов.</param>
    /// <param name="interest_rate">Годовая процентная ставка в процентах.</param>
    /// <returns>Строка с суммами вклада по годам.</returns>
    static string CalculateInterest(double initial_deposit, int years, double interest_rate)
    {
        // Строка для накопления результатов расчёта.
        string result = "";
        // Текущая сумма вклада с учётом уже начисленных процентов.
        double amount = initial_deposit;

        // Для каждого года повторяем начисление процентов.
        for (int i = 1; i <= years; i++)
        {
            // Увеличиваем текущую сумму на годовой процент (капитализация).
            amount *= 1 + interest_rate / 100;

            // F2 задаёт два знака после запятой; InvariantCulture обеспечивает единый формат числа.
            result += $"Год {i}: {amount.ToString("F2", CultureInfo.InvariantCulture)} руб.";

            // Добавляем перенос строки только между годами, без лишнего переноса в конце.
            if (i < years)
                result += "\n";
        }

        // Возвращаем готовый многострочный отчёт.
        return result;
    }

    /// <summary>
    /// Точка входа: демонстрирует расчёт вклада 1000 рублей на три года под 10 процентов.
    /// </summary>
    static void Main()
    {
        Console.WriteLine(CalculateInterest(1000, 3, 10));
    }
}
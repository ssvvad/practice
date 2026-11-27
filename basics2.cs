using System;

class Program
{
    static void Draw(int n)
    {
        int mid = n / 2;

        for (int i = 0; i < n; i++)
        {
            int a = Math.Abs(mid - i);
            int b = n - 1 - a;

            for (int j = 0; j < n; j++)
            {
                if (j == a || j == b)
                    Console.Write("X");
                else
                    Console.Write(" ");
            }

            Console.WriteLine();
        }
    }

    static void Main()
    {
        Draw(5);
    }
}
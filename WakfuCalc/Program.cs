using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace WakfuCalc
{
    class Program
    {
        readonly static double lockConst = 0.75d * (3.0d / 7.0d);




        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();


        static T Max<T>(T x, T y)
        {
            return (Comparer<T>.Default.Compare(x, y) > 0) ? x : y;
        }

        static T Min<T>(T x, T y)
        {
            return (Comparer<T>.Default.Compare(x, y) < 0) ? x : y;
        }

        static T Clamp<T>(T x, T lo, T hi)
        {
            return Max(Min(x, hi), lo);
        }



        static void Main(string[] args)
        {

            

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;

            SetLayeredWindowAttributes(GetConsoleWindow(), 0, (byte)175, 2);


            Console.WriteLine("*** Input \"help\" for a list of commands.\n\n");


            while (true)
            {
                Console.Write("> ");

                var input = Console.ReadLine().ToLower().Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                Console.WriteLine("");

                var quit = false;

                int totalLock = 0, dodge = 0, requiredTotalLock = 0;
                int apLoss = 0, mpLoss = 0;
                decimal fowCaster = 0.0m, fowTarget = 0.0m;


                decimal removalValue = 0.0m, fowPercent = 0.0m;
                double lossValue = 0.0d;
                decimal fowFactor = 0.0m;

                if (input.Length == 0)
                    break;

                switch (input[0])
                {
                    case "help":
                        Console.WriteLine("[+] lockcheck / lc\n[+] canlock / cl\n[+] rem / remove / fow\n");
                        break;

                    case "quit":
                    case "exit":
                    case "":
                        quit = true;

                        break;

                    case "lockcheck":
                    case "lc":
                        if (input.Length == 1)
                        {
                            Console.WriteLine("[+] lockcheck [dodge]\n");
                            break;
                        }

                        dodge = Convert.ToInt32(input[1]);
                        requiredTotalLock = Convert.ToInt32((lockConst * dodge + dodge) / (1 - lockConst));


                        Console.WriteLine($"[+] Minimum effective lock required: Greater than {requiredTotalLock}\n");
                        break;

                    case "canlock":
                    case "cl":
                        int[] lockValues = new int[] { 0, 0, 0 };

                        if (input.Length == 1)
                        {
                            Console.WriteLine("[+] canlock [dodge] [1st lock] [2nd lock] [3rd lock]\n");
                            break;
                        }

                        
                        dodge = Convert.ToInt32(input[1]);

                        for (int i = 2; i < Min(input.Length, 5); ++i)
                            lockValues[i - 2] = Max(Convert.ToInt32(input[i]), 0);

                        Array.Sort(lockValues);
                        Array.Reverse(lockValues);


                        totalLock = 0;
                        for (int i = 0; i < 3; ++i)
                            totalLock += lockValues[i] / (i + 1);

                        lossValue = Math.Floor(((7.0d / 3.0d) * (totalLock - dodge) / (totalLock + dodge) + 1) * 4);

                        mpLoss = Convert.ToInt32(Clamp(Math.Ceiling(lossValue / 2.0d), 0.0d, 4.0d));
                        apLoss = Convert.ToInt32(Clamp(Math.Floor(lossValue / 2.0d), 0.0d, 4.0d));

                        Console.WriteLine($"[+] MP Loss: {mpLoss}\n[+] AP Loss: {apLoss}\n");


                        break;

                    case "rem":
                    case "remove":
                    case "fow":

                        if (input.Length == 1)
                        {
                            Console.WriteLine("[+] remove [fow of caster] [fow of target] [removal value]\n");
                            break;
                        }

                        fowCaster = Convert.ToInt32(input[1]);
                        fowTarget = Convert.ToInt32(input[2]);
                        removalValue = Convert.ToDecimal(input[3]);


                        fowFactor = Clamp(1.0m + ((fowCaster / 100) - (fowTarget / 100)), 0.0m, 2.0m);
                        removalValue = removalValue * 0.5m * fowFactor;
                        fowPercent = removalValue - decimal.Truncate(removalValue);


                        Console.WriteLine($"[+] {(1 - fowPercent) * 100}% chance to remove {decimal.Truncate(removalValue)}");
                        Console.WriteLine($"[+] {fowPercent * 100}% chance to remove {decimal.Truncate(removalValue) + 1}\n");
                        
                        break;


                    default:
                        break;
                }

                if (quit)
                    break;
            }







        }
    }
}

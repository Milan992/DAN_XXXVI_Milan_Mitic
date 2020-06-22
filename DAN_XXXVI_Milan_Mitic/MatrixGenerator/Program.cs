using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixGenerator
{
    class Program
    {
        static int[,] matrix;
        static int[] array;
        static int[] arrayForFile;
        static Random r = new Random();
        static readonly object l = new object();

        static void Main(string[] args)
        {
            //create threads
            File.WriteAllText(@"..\..\numbers.txt", "");
            Thread first = new Thread(() => GenerateMatrix());
            Thread second = new Thread(() => GenerateNumbers());
            Thread third = new Thread(() => MatrixToArray(matrix));
            Thread fourth = new Thread(() => ReadFromFile());

            first.Start();
            second.Start();

            //third and fourth thread should not start before first two finish
            first.Join();
            second.Join();

            third.Start();
            third.Join();

            fourth.Start();

            Console.ReadLine();
        }

        public static void GenerateMatrix()
        {
            matrix = new int[100, 100];
            lock (l)
            {
                Monitor.Wait(l);
                int counter = 0;
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        try
                        {
                            matrix[i, j] = array[counter];
                            counter++;
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
            }
        }

        public static void GenerateNumbers()
        {
            lock (l)
            {
                array = new int[10000];
                for (int i = 0; i < array.Length; i++)
                {

                    array[i] = r.Next(10, 100);

                }
                Monitor.Pulse(l);
            }
        }

        public static void MatrixToArray(int[,] matrix)
        {
            arrayForFile = new int[10000];
            int counter = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[i, j] % 2 == 1)
                    {
                        arrayForFile[counter] = matrix[i, j];
                        try
                        {
                            using (StreamWriter sw = new StreamWriter("../../numbers.txt", true))
                            {
                                sw.WriteLine(arrayForFile[counter]);
                            }
                            counter++;
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public static void ReadFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader("../../numbers.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch
            {
            }
        }
    }
}

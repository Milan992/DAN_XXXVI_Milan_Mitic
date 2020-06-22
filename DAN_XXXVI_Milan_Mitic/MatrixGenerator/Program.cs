using System;
using System.IO;
using System.Threading;

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

        /// <summary>
        /// generating a 100x100 matrix
        /// </summary>
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

        /// <summary>
        /// generating an array of 10000 random numbers
        /// </summary>
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

        /// <summary>
        /// putting all numbers from the matrix to an array and writing them to a txt file
        /// </summary>
        /// <param name="matrix"></param>
        public static void MatrixToArray(int[,] matrix)
        {
            arrayForFile = new int[10000];
            int counter = 0;

            using (StreamWriter sw = new StreamWriter("../../numbers.txt", true))
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        if (matrix[i, j] % 2 == 1)
                        {
                            arrayForFile[counter] = matrix[i, j];
                            try
                            {

                                sw.WriteLine(arrayForFile[counter]);

                                counter++;
                            }
                            catch
                            {
                            }
                        }
                    }
                }
        }


        /// <summary>
        /// reads all lines from a txt file and writing them on a console
        /// </summary>
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

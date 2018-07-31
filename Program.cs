using System;
using System.Diagnostics;
using System.Numerics;

namespace SIMD_Explore
{
    class Program
    {
        public static Random rng = new Random();

        // SIMD only works when in release mode, targetting 64bit processors
        public static void Main(string[] args)
        {
            if (!Vector.IsHardwareAccelerated)
            {
                Console.WriteLine("Hardware acceleration is not enabled");
                Console.ReadKey();
                return; // end
            }

            Stopwatch stopwatch = new Stopwatch();

            int[] a = GenerateRandomArray(10000000);
            int[] b = GenerateRandomArray(10000000);


            stopwatch.Start();
            int[] answersNoneSIMD = StandardArrayAddition(a, b);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            int[] answerSIMD = SIMDArrayAddition(a, b);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            Console.ReadKey();

        }

        public static int[] GenerateRandomArray(int size)
        {
            int[] result = new int[size];

            for (int i = 0; i<size-1;i++)
            {
                result[i] = rng.Next(99);
            }

            return result;
        }

        public static int[] StandardArrayAddition(int[] lhs, int[] rhs)
        {
            int[] result = new int[lhs.Length];

            for(int i = 0; i < lhs.Length - 1; i++)
            {
                result[i] = lhs[i] + rhs[i];
            }

            return result;
        }


        public static int[] SIMDArrayAddition(int[] lhs, int[] rhs)
        {

            int simdLength = Vector<int>.Count; // 256 bit registers on modern CPUs - 8x 32 bit ints processed at once.
            int[] result = new int[lhs.Length];

            for (int i = 0; i <= lhs.Length - simdLength; i += simdLength)
            {
                Vector<int> va = new Vector<int>(lhs, i); // Compiler auto optimizes for the correct bit length (typically 8x 32bits)
                Vector<int> vb = new Vector<int>(rhs, i);

                (va + vb).CopyTo(result, i);
            }

            return result;
        }
    }
}

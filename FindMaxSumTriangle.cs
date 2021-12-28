using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project6
{
    public static class FindMaxSumTriangle
    {
        //Checks if the number is Prime 
        private static bool checkPrime(this int number)
        {
            if ((number & 1) == 0)
            {
                if (number == 2)
                {
                    return true;
                }
                return false;
            }
            for (var i = 3; (i * i) <= number; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return number != 1;
        }
        //Replace prime numbers with -1
        private static int[,] removePrimeNumbers(this int[,] matrixArray)
        {
            int length = matrixArray.GetLength(0);
            
            
            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < length; j++)
                {
                    if (matrixArray[i, j] == 0)
                    {
                        continue;
                    }
                    else if (checkPrime(matrixArray[i, j]))       //replacing prime numbers with -1 in matrix
                    {
                        matrixArray[i, j] = -1;
                    }
                }
            }
            return matrixArray;
        }

        //Convert input array to 2DArray
        private static int[,] convertTo2DArray(this string input)
        {
            string[] array = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);  //remove spaces

            int[,] matrixArray = new int[array.Length, array.Length + 1];      //converting to a matrix

            for (var row = 0; row < array.Length; row++)
            {
                int[] digitsInRow = Regex.Matches(array[row], "[0-9]+")
                       .Cast<Match>()
                       .Select(m => int.Parse(m.Value)).ToArray();

                //  int[] digitsInRow = array[row].Split(' ').Select(int.Parse).ToArray();
                for (var column = 0; column < digitsInRow.Length; column++)
                {
                    matrixArray[row, column] = digitsInRow[column];
                }


            }
            return matrixArray.removePrimeNumbers();
        }

        public static int moveTopDown(this int[,] matrixArray)
        {
            int length = matrixArray.GetLength(0);

            int res = -1;
            for (int i = 0; i < length - 2; i++)
            {
                res = Math.Max(res, matrixArray[0, i]);
                
            }
            for (int i = 1; i < length; i++)
            {
                res = -1;
                for (int j = 0; j < length; j++)
                {
                    if (j == 0 && matrixArray[i, j] != -1)
                    {
                        if (matrixArray[i - 1, j] != -1)
                            matrixArray[i, j] += matrixArray[i - 1, j];
                        else
                            matrixArray[i, j] = -1;
                    }
                    else if (j > 0 && j < length - 1 && matrixArray[i, j] != -1)
                    {
                        int tmp = calculateNodeValue(matrixArray[i - 1, j],
                                   matrixArray[i - 1, j - 1]);
                        if (tmp == -1)
                        {
                            matrixArray[i, j] = -1;
                        }
                        else
                            matrixArray[i, j] += tmp;
                    }
                    else if (j > 0 && matrixArray[i, j] != -1)
                    {
                        int tmp = calculateNodeValue(matrixArray[i - 1, j],
                                         matrixArray[i - 1, j - 1]);
                        if (tmp == -1)
                        {
                            matrixArray[i, j] = -1;
                        }
                        else
                            matrixArray[i, j] += tmp;
                    }
                    else if (j != 0 && j < length - 1 && matrixArray[i, j] != -1)
                    {
                        int tmp = calculateNodeValue(matrixArray[i - 1, j],
                                     matrixArray[i - 1, j - 1]);
                        if (tmp == -1)
                        {
                            matrixArray[i, j] = -1;
                        }
                        else
                            matrixArray[i, j] += tmp;
                    }
                    res = Math.Max(matrixArray[i, j], res);
                }

            }
                return res;
        }

        //Get the max value
        private static int calculateNodeValue(int input1, int input2)  
        {
            if (input1 == -1 && input2 == -1 || input1 == 0 && input2 == 0)
                return -1;
            else
                return Math.Max(input1, input2);
        }
        //Main
        public static void Main(string[] args)
        {
            
            try
            {
                Console.WriteLine("Enter the file path: (Ex: " + @"D:\input.txt" + ")");
                string cs = Console.ReadLine();
                string contents = File.ReadAllText(cs);
                int[,] convertedTriangleFile = contents.convertTo2DArray();
                int maxSumX = convertedTriangleFile.moveTopDown();
                Console.WriteLine("The result from File " + maxSumX);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            Console.ReadLine();

        }


    }

    }

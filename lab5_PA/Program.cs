using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Knapsack
{
    class Program
    {

        public const int N = 100;

        int[] value = new int[N];
        int[] weight = new int[N];
        int lowerv = 2;
        int lowerw = 1;
        int upperv = 20;
        int upperw = 10;
        int maxxxValue = -1;
        List<int> maxxxWeights = new List<int>();
        List<int> maxxxValues = new List<int>();

        public const int capacity = 200;
        public const int pop = 100;
        public const int LEN = N;
        private int[,] gene = new int[pop, LEN];
        private double mut = 0.05;
        private double rec = 0.1;
        private double end = 1;
        int mejor = 0;

        Random rnd = new Random();
        public Program()
        {
        }
        public void init_data()
        {
            Console.WriteLine("Weights and values:");

            for (int i = lowerw; i <= upperw; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    weight[j] = rnd.Next(lowerw, upperw);

                }
            }

            for (int i = lowerv; i < upperv; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    value[j] = rnd.Next(lowerv, upperv);
                }


            }
            //Для спрощення виду популяції
            /*for (int i = 0; i < N - 1; i++)
              {
                  for (int j = i + 1; j < N; j++)
                  {
                      double valuePerWeight1 = (double)value[i] / weight[i];
                      double valuePerWeight2 = (double)value[j] / weight[j];
                      if (valuePerWeight1 < valuePerWeight2)
                      {
                          int temp = value[i];
                          value[i] = value[j];
                          value[j] = temp;
                          temp = weight[i];
                          weight[i] = weight[j];
                          weight[j] = temp;
                      }
                  }
              }*/
            for (int j = 0; j < N; j++)
            {
                Console.WriteLine("item {0} -  value = {1} - weight = {2}", j, value[j], weight[j]);
            }
            Console.WriteLine();
        }
        public void run()
        {
            int a, b, win, lose;
            init_pop();
            for (int iteration = 0; iteration < end; iteration++)
            {
                a = (int)(pop * rnd.NextDouble());
                b = (int)(pop * rnd.NextDouble());
                if (fitness(a) > fitness(b))
                {
                    win = a;
                    lose = b;

                }
                else
                {
                    win = b;
                    lose = a;
                }
                for (int i = 0; i < LEN; i++)
                {
                    if (rnd.NextDouble() < rec)
                        gene[lose, i] = gene[win, i];
                    if (rnd.NextDouble() < mut)
                        gene[lose, i] = 1 - gene[lose, i];
                }

                escribir_mejor();
                output_values();

                improveKnapsack();


            }
        }
        public void output_values()
        {
            Console.WriteLine("Values of best solution - {0}", maxxxValue);
            Console.WriteLine("Weight of best solution - {0}", maxxxWeights.Sum());

            for (int i = 0; i < maxxxValues.Count(); i++)
            {
                Console.WriteLine("item {0} -  value = {1} - weight = {2}", i, maxxxValues[i], maxxxWeights[i]);
            }
            Console.WriteLine();

        }
        private void escribir_mejor()
        {
            mejor = 0;
            for (int i = 1; i < pop; i++)

                if (fitness(mejor) < fitness(i))
                    mejor = i;

            Console.WriteLine();
            if (fitness(mejor) == -1)
                Console.WriteLine("weight more then capacity");
            else
            {
                if (maxxxValue <= fitness(mejor))
                {
                    maxxxWeights.Clear();
                    maxxxValues.Clear();
                    maxxxValue = fitness(mejor);

                    for (int j = 0; j < LEN; j++)
                    {
                        if (gene[mejor, j] == 1)
                        {
                            maxxxWeights.Add(weight[j]);
                            maxxxValues.Add(value[j]);

                        }


                    }
                }
                Console.WriteLine("value : {0}", fitness(mejor));
            }
        }
        private int fitness(int n)
        {
            int valuet = 0;
            int weightt = 0;
            for (int j = 0; j < LEN; j++)
            {
                if (gene[n, j] == 1)
                {
                    valuet = valuet + value[j];
                    weightt = weightt + weight[j];
                }

            }
            if (weightt > capacity)
                return -1;
            else
                return valuet;
        }
        private void init_pop()
        {
            for (int i = 0; i < pop; i++)
            {
                for (int j = 0; j < LEN; j++)
                {
                    if (rnd.NextDouble() < 0.5)
                    {
                        gene[i, j] = 0;

                    }
                    else
                    {
                        gene[i, j] = 1;
                    }
                }
            }
        }
        public void improveKnapsack()
        {
            // створюємо масиви для зберігання речей, що містяться у рюкзаку
            // та речей, що не містяться
            List<int> inKnapsack = new List<int>();
            List<int> notInKnapsack = new List<int>();
            // рахуємо, скільки речей містяться у рюкзаку та скільки не містяться
            int numIn = 0, numNotIn = 0;
            for (int i = 0; i < LEN; i++)
            {
                if (gene[mejor, i] == 1)
                {
                    inKnapsack.Add(i);
                    numIn++;
                }
                else
                {
                    notInKnapsack.Add(i);
                    numNotIn++;
                }
            }
            // поки є речі, які можна вкинути
            while (numNotIn > 0)
            {
                // знаходимо реч, яка має найбільшу цінність на одиницю ваги
                int bestItem = -1;
                int bestTupleItem = -1;
                double bestValuePerWeight = 0;


                for (int i = 0; i < numNotIn; i++)
                {
                    int item = notInKnapsack[i];
                    double valuePerWeight = (double)value[item] / weight[item];
                    if (valuePerWeight > bestValuePerWeight)
                    {
                        bestValuePerWeight = valuePerWeight;
                        bestTupleItem = i;
                        bestItem = item;
                    }
                }

                // якщо ми знайшли річ, яку можна вкинути
                if (bestItem != -1)
                {

                    // якщо річ вміщується у рюкзак, то додаємо її туди
                    if (weight[bestItem] <= capacity - maxxxWeights.Sum())
                    {
                        Console.WriteLine("Inserted item - val - {0} - weight - {1}", value[bestItem], weight[bestItem]);

                        maxxxWeights.Add(weight[bestItem]);
                        maxxxValues.Add(value[bestItem]);
                        // видаляємо реч з масиву речей, що не містяться у рюкзаку
                        notInKnapsack[bestTupleItem] = notInKnapsack[numNotIn - 1];
                        notInKnapsack.RemoveAt(numNotIn - 1);
                        numNotIn--;
                    }
                    else
                    {
                        // інакше зупиняємо ітерації, оскільки більше немає речей,
                        // які можна вкинути з рюкзаку
                        break;
                    }
                }
            }
            Console.WriteLine();
            // сортуємо речі у рюкзаку за цінністю на одиницю ваги
            for (int i = 0; i < maxxxValues.Count() - 1; i++)
            {
                for (int j = i + 1; j < maxxxValues.Count(); j++)
                {
                    double valuePerWeight1 = (double)maxxxValues[i] / maxxxWeights[i];
                    double valuePerWeight2 = (double)maxxxValues[j] / maxxxWeights[j];
                    if (valuePerWeight1 < valuePerWeight2)
                    {
                        int temp = maxxxValues[i];
                        maxxxValues[i] = maxxxValues[j];
                        maxxxValues[j] = temp;
                        temp = maxxxWeights[i];
                        maxxxWeights[i] = maxxxWeights[j];
                        maxxxWeights[j] = temp;
                    }
                }
            }
            // видаляємо речі з рюкзаку, якщо він переповнений
            while (maxxxWeights.Sum() > capacity)
            {
                Console.WriteLine("Deleted item - val - {0} - weight - {1}", maxxxWeights[maxxxWeights.Count() - 1], maxxxValues[maxxxValues.Count() - 1]);
                maxxxWeights.RemoveAt(maxxxWeights.Count() - 1);
                maxxxValues.RemoveAt(maxxxValues.Count() - 1);
            }

            Console.WriteLine("the improved value - {0}", maxxxValues.Sum());
            maxxxValue = maxxxValues.Sum();

        }
    }
    class test
    {
        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            Program p = new Program();
            timer.Start();
            p.init_data();
            p.run();
            p.output_values();
            timer.Stop();
            Console.WriteLine($"Час виконання: {(double)timer.ElapsedMilliseconds / 1000} секунд");
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretSantaTelegramBot.Helpers
{
    public static class SortExtensions
    {
        public static IList<T> RandomSort<T>(this IList<T> list)
        {
            var newList = new List<T>(list);
            var random = new Random();

            int n = newList.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);

                T value = newList[k];
                newList[k] = newList[n];
                newList[n] = value;
            }

            return newList;
        }
    }
}

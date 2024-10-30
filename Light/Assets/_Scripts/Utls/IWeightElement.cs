using System.Collections.Generic;
using System.Linq;

namespace Utls
{
    public interface IWeightElement
    {
        int Weight { get; }
    }

    public static class WeightElementExtension
    {
        /// <summary>
        /// 根据权重从列表中随机选择一个元素。如果权重为0，将选择保底值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T WeightPick<T>(this IEnumerable<T> list) where T : IWeightElement
        {
            var weightedList = list.Where(w => w.Weight > 0).ToList();
            if (!weightedList.Any())
                return list.FirstOrDefault(w => w.Weight == 0); // 选择保底值

            int totalWeight = weightedList.Sum(w => w.Weight);
            int randomValue = Sys.Random.Next(1, totalWeight + 1);
            int cumulativeWeight = 0;

            foreach (var element in weightedList)
            {
                cumulativeWeight += element.Weight;
                if (randomValue <= cumulativeWeight)
                    return element;
            }

            return default; // 这行一般不会执行
        }

        /// <summary>
        /// 从列表中随机选择指定数量的元素，根据权重决定出现概率。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static IEnumerable<T> WeightTake<T>(this IEnumerable<T> list, int take) where T : IWeightElement
        {
            var result = new List<T>();
            var availableList = list.ToList();

            for (int i = 0; i < take && availableList.Any(); i++)
            {
                var picked = availableList.WeightPick();
                if (picked != null)
                {
                    result.Add(picked);
                    availableList.Remove(picked);
                }
            }

            return result;
        }
    }
}
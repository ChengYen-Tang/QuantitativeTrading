using System.Collections;
using System.Collections.Generic;

namespace QuantitativeTrading.Data.DataProviders
{
    public abstract class KlineDataProvider<T> : IEnumerable<T>
    {
        public bool IsEnd => index >= models.Count - 1;
        public T this[int index] => models[index];
        public T Current => models[index];
        public long Length => models.Count;

        protected List<T> models;

        private int index = 0;

        public void Reset() => index = 0;

        public bool MoveNext(out T model)
        {
            index++;
            if (IsEnd)
            {
                model = default;
                return false;
            }

            model = models[index];
            return true;
        }

        public IEnumerable<T> GetHistory(int historyPoint)
        {
            int timePoint = historyPoint - 1;
            int historyIndex = 0;
            if (index > timePoint)
                historyIndex = index - timePoint;
            for (int i = historyIndex; i <= index; i++)
                yield return models[i];
        }

        public IEnumerator<T> GetEnumerator()
            => models.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => models.GetEnumerator();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace QuantitativeTrading.Component.DataProvider
{
    public abstract class KlineDataProvider<T> : IEnumerable<T>
    {
        public bool IsEnd { get { return index >= models.Count - 1; } }
        public T this[int index] { get { return models[index]; } }
        public T Current { get { return models[index]; } }
        public long Length { get { return models.Count; } }

        protected List<T> models;

        private int index = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => index = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> GetHistory(int historyPoint)
        {
            int timePoint = (historyPoint - 1);
            int historyIndex = 0;
            if (index > timePoint)
                historyIndex = index - timePoint;
            for (int i = historyIndex; i <= index; i++)
                yield return models[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
            => models.GetEnumerator();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => models.GetEnumerator();
    }
}

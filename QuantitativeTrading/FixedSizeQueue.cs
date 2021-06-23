using System.Collections.Generic;

namespace QuantitativeTrading
{
    public class FixedSizeQueue<T> : Queue<T>
    {
        private readonly T[] queueArray;
        private readonly int size;

        public T this[int index] => queueArray[index];
        public T First => queueArray[0];
        public T Last => queueArray[Count - 1];

        public FixedSizeQueue(int size)
            : base(size)
            => (queueArray, this.size) = (new T[size], size);

        public new void Enqueue(T item)
        {
            // To dequque when full
            while (this.Count >= this.size)
                Dequeue();
            // To enqueue
            base.Enqueue(item);
            CopyTo(queueArray, 0);
        }
    }
}

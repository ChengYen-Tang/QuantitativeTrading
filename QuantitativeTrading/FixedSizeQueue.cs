using System.Collections.Generic;

namespace QuantitativeTrading
{
    public class FixedSizeQueue<T> : Queue<T>
    {
        private int size;

        public FixedSizeQueue(int size)
            : base(size)
            => this.size = size;

        public new void Enqueue(T item)
        {
            // To dequque when full
            while (this.Count >= this.size)
                Dequeue();
            // To enqueue
            base.Enqueue(item);
        }
    }
}

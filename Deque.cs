
namespace Collections.GenericExtension
{
    using System;
    using System.Collections.Generic;
    public class Deque<T>
    {
        private const int _MinimumGrow = 4;
        private const int _ShrinkThreshold = 32;
        private const int _GrowFactor = 200;  // double each time
        private const int _DefaultCapacity = 4;
        static T[] _emptyArray = new T[0];

        private T[] _array;
        private int _head;       // First valid element in the queue
        private int _tail;       // Last valid element in the queue
        private int _size;       // Number of elements.
        private int _version;

        public Deque()
        {
            _array = _emptyArray;
        }

        public Deque(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity may not be negative.");

            _array = new T[capacity];
            _head = 0;
            _tail = 0;
            _size = 0;
        }

        public int Capacity { get; private set; }
        public int Count { get { return _size; } }

        public void Clear()
        {
            if (_head < _tail)
                Array.Clear(_array, _head, _tail);
            else
            {
                Array.Clear(_array, _head, _array.Length - _head);
                Array.Clear(_array, 0, _tail);
            }

            _head = 0;
            _tail = 0;
            _size = 0;
        }

        public bool Contains(T item)
        {
            int index = _head;
            int count = _size;

            EqualityComparer<T> c = EqualityComparer<T>.Default;
            while (count-- > 0)
            {
                if (((Object)item) == null)
                {
                    if (((Object)_array[index]) == null)
                        return true;
                }
                else if (_array[index] != null && c.Equals(_array[index], item))
                {
                    return true;
                }
                index = (index + 1) % _array.Length;
            }

            return false;
        }

        internal T GetElement(int i)
        {
            //return _array[(_head + i) % _array.Length];
            if (_size <= i)
                throw new ArgumentOutOfRangeException(nameof(i));
            return _array[_head + i];
        }

        public void EnHead(T item)
        {
            if (_size == _array.Length)// array has been full, or length of array is 0
            {
                int newcapacity = (int)((long)_array.Length * (long)_GrowFactor / 100);
                if (newcapacity < _array.Length + _MinimumGrow)
                {
                    newcapacity = _array.Length + _MinimumGrow;
                }
                SetCapacityHead(newcapacity, item);
            }
            else if (_head > 0)// head is not 0
            {
                _array[_head - 1] = item;
                _head--;
                _size++;
            }
            else if (_size > 0) // head is 0, move
            {
                Array.Copy(_array, _head, _array, 1, _size);
                _array[0] = item;
                _tail++;
                _size++;
            }
            else//array is empty
            {
                _array[0] = item;
                _size++;
            }
        }

        public T DeHead()
        {
            if (_size == 0)
                throw new ArgumentOutOfRangeException(nameof(_size), "InvalidOperation_EmptyQueue.");
            T removed = _array[_head];
            _array[_head] = default(T);
            //_head = (_head + 1) % _array.Length;
            _head++;
            _size--;
            _version++;
            return removed;
        }

        public void EnTail(T item)
        {
            if (_size == _array.Length)// array has been full, or length of array is 0
            {
                int newcapacity = (int)((long)_array.Length * (long)_GrowFactor / 100);
                if (newcapacity < _array.Length + _MinimumGrow)
                {
                    newcapacity = _array.Length + _MinimumGrow;
                }
                SetCapacityTail(newcapacity, item);
            }
            else if (_size == 0) // array is empty, insert directly. head==tail
            {
                _array[_tail] = item;
                _size++;
            }
            else if (_tail < _array.Length - 1)// tail is not the last
            {
                _tail++;
                _array[_tail] = item;
                _size++;
            }
            else// tail is the last
            {
                Array.Copy(_array, _head, _array, _head - 1, _size);
                _array[_tail] = item;
                _head--;
                _size++;
            }
        }

        public T DeTail()
        {
            if (_size == 0)
                throw new ArgumentOutOfRangeException(nameof(_size), "InvalidOperation_EmptyQueue.");
            T removed = _array[_tail];
            _array[_tail] = default(T);
            //_tail = (_tail - 1) % _array.Length;
            _tail--;
            _size--;
            _version++;
            return removed;

        }

        private void SetCapacity(int capacity)
        {
            T[] newarray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }

            _array = newarray;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
            _version++;
            Capacity = capacity;
        }

        private void SetCapacityHead(int capacity, T item)
        {
            T[] newarray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 1, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }
            _array = newarray;
            _array[0] = item;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
            _size++;
            _version++;
            Capacity = capacity;
        }

        private void SetCapacityTail(int capacity, T item)
        {
            T[] newarray = new T[capacity];
            if (_size > 0)
            {
                if (_head < _tail)
                {
                    Array.Copy(_array, _head, newarray, 0, _size);
                }
                else
                {
                    Array.Copy(_array, _head, newarray, 0, _array.Length - _head);
                    Array.Copy(_array, 0, newarray, _array.Length - _head, _tail);
                }
            }

            _array = newarray;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
            _array[_tail] = item;
            _size++;
            _version++;
            Capacity = capacity;
        }

        public T Head() { return _array[0]; }
        public T Tail() { return _array[_size - 1]; }



        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentOutOfRangeException(nameof(array), "ArgumentNullException.");
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(array), " ArgumentOutOfRange_Index.");
            }

            int arrayLen = array.Length;
            if (arrayLen - arrayIndex < _size)
            {
                throw new ArgumentOutOfRangeException(nameof(array), "  Argument_InvalidOffLen.");
            }

            int numToCopy = (arrayLen - arrayIndex < _size) ? (arrayLen - arrayIndex) : _size;
            if (numToCopy == 0) return;

            int firstPart = (_array.Length - _head < numToCopy) ? _array.Length - _head : numToCopy;
            Array.Copy(_array, _head, array, arrayIndex, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
            {
                Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, numToCopy);
            }
        }



        public T[] ToArray()
        {
            T[] arr = new T[_size];
            if (_size == 0)
                return arr;

            if (_head < _tail)
            {
                Array.Copy(_array, _head, arr, 0, _size);
            }
            else
            {
                Array.Copy(_array, _head, arr, 0, _array.Length - _head);
                Array.Copy(_array, 0, arr, _array.Length - _head, _tail);
            }

            return arr;
        }
        public void TrimExcess()
        {
            int threshold = (int)(((double)_array.Length) * 0.9);
            if (_size < threshold)
            {
                SetCapacity(_size);
            }
        }


    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapBusinessOneExtensions
{
    public class SboEnumerableCollection<T> : IEnumerable<T>
    {
        private readonly dynamic _collection;

        public SboEnumerableCollection(dynamic collection)
        {
            _collection = collection;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new SboLineEnumerator<T>(_collection);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public sealed class SboLineEnumerator<T> : IEnumerator<T>
    {
        private readonly int _count;
        private dynamic _collection;
        private int _currentLine;

        public SboLineEnumerator(dynamic collection)
        {
            _collection = collection;
            _count = _collection != null ? _collection.Count : 0;
            Reset();
        }

        public void Dispose()
        {
            Reset();
            _collection = null;

            GC.SuppressFinalize(this);
        }

        public bool MoveNext()
        {
            _currentLine++;
            return _currentLine < _count;
        }

        public void Reset()
        {
            _currentLine = -1;
        }

        public T Current
        {
            get
            {
                try
                {
                    _collection.SetCurrentLine(_currentLine);
                    return _collection;
                }
                catch (Exception)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}

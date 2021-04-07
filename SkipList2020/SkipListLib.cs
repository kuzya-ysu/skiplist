using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkipList2020
{
    public class SkipList<TKey, TValue> :
        IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        private readonly Node<TKey, TValue>[] _head;
        private readonly double _probability;
        private readonly int _maxLevel;
        private int _curLevel;
        private Random _rd;
        public int Count { get; private set; }

        public SkipList(int maxLevel = 10, double p = 0.5)
        {
            _maxLevel = maxLevel;
            _probability = p;
            _head = new Node<TKey, TValue>[_maxLevel];
            for (int i = 0; i < maxLevel; i++)
            {
                _head[i] = new Node<TKey, TValue>();
                if (i != 0)
                {
                    _head[i].Down = _head[i - 1];
                    _head[i - 1].Up = _head[i];
                }
            }
            _curLevel = 0;
            _rd = new Random(DateTime.Now.Millisecond);
        }

        public void Add(TKey key, TValue value)
        {
            var prevNode = new Node<TKey, TValue>[_maxLevel];
            var currentNode = _head[_curLevel];
            for (int i = _curLevel; i >= 0; i--)
            {
                while (currentNode.Right != null &&
                    currentNode.Right.Key.CompareTo(key) < 0)
                {
                    currentNode = currentNode.Right;
                }
                prevNode[i] = currentNode;
                if (currentNode.Down == null)
                    break;
                currentNode = currentNode.Down;
            }
            int level = 0;
            while (_rd.NextDouble() < _probability && level < _maxLevel - 1)
            {
                level++;
            }
            while (_curLevel < level)
            {
                _curLevel++;
                prevNode[_curLevel] = _head[_curLevel];
            }
            for (int i = 0; i <= level; i++)
            {
                var node = new Node<TKey, TValue>(key, value) { Right = prevNode[i].Right };
                prevNode[i].Right = node;
                if (i != 0)
                {
                    node.Down = prevNode[i - 1].Right;
                    prevNode[i - 1].Right.Up = node;
                }
            }
            Count++;
        }

        public bool Remove(TKey key)
        {
            var prevNode = new Node<TKey, TValue>[_maxLevel];
            var currentNode = _head[_curLevel];
            int level = -1;
            for (int i = _curLevel; i >= 0; i--)
            {
                while (currentNode.Right != null &&
                       currentNode.Right.Key.CompareTo(key) < 0)
                {
                    currentNode = currentNode.Right;
                }
                if (currentNode.Key.CompareTo(key) == 0 && level < 0)
                    level = i;
                prevNode[i] = currentNode;
                if (currentNode.Down == null)
                    break;
                currentNode = currentNode.Down;
            }
            if (level < 0)
                return false;
            for (int i = level; i > 0; i++)
            {
                prevNode[i].Right = prevNode[i].Right.Right;
            }

            while (_head[_curLevel].Right == null)
            {
                _curLevel--;
            }

            Count--;
            return true;
        }

        public TValue FindNode(TKey key)
        {
            var currentNode = _head[_curLevel];
            for (int i = _curLevel; i >= 0; i--)
            {
                while (currentNode.Right != null && currentNode.Right.Key.CompareTo(key) < 0)
                {
                    currentNode = currentNode.Right;
                }
                if (currentNode == null)
                    throw new KeyNotFoundException("This key doesn't exist");
                if (currentNode.Key.CompareTo(key) == 0)
                    break;
                currentNode = currentNode.Down;
            }
            return currentNode.Value;
        }

        public bool Contains(TKey key)
        {
            try
            {
                FindNode(key);
                return true;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var node = _head[0].Right; node.Right != null; node = node.Right)
            {
                yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var node = _head[0].Right; node.Right != null; node = node.Right)
            {
                yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            }
        }
    }
}
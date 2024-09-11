using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TvProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Потокобезопасная реализация trie или дерева префиксов
    /// </summary>
    public class ConcurrentTrie<TValue>
    {
        private class TrieNode
        {
            private readonly ReaderWriterLockSlim _lock = new();
            private (bool hasValue, TValue value) _value;
            public readonly ConcurrentDictionary<char, TrieNode> Children = new();

            public bool GetValue(out TValue value)
            {
                _lock.EnterReadLock();
                try
                {
                    (var hasValue, value) = _value;
                    return hasValue;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }

            public void SetValue(TValue value)
            {
                SetValue(value, true);
            }

            public void RemoveValue()
            {
                SetValue(default, false);
            }

            private void SetValue(TValue value, bool hasValue)
            {
                _lock.EnterWriteLock();
                try
                {
                    _value = (hasValue, value);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        private readonly TrieNode _root;
        private readonly string _prefix;

        public IEnumerable<string> Keys => Search(string.Empty).Select(kv => kv.Key);
        public IEnumerable<TValue> Values => Search(string.Empty).Select(kv => kv.Value);


        public ConcurrentTrie() : this(new(), string.Empty)
        {
        }

        private ConcurrentTrie(TrieNode root, string prefix)
        {
            _root = root;
            _prefix = prefix;
        }

        /// <summary>
        /// Пытается получить значение, связанное с указанным ключом
        /// </summary>
        /// <param name="key">Ключ элемента для получения (нечувствительный к регистру)</param>
        /// <param name="value">Значение, ассоциированное с <paramref name="key"/>, если обнаружено</param>
        /// <returns>
        /// Истина если ключ был найден, в противном случае, ложно
        /// </returns>
        public bool TryGetValue(string key, out TValue value)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            value = default;
            return Find(key, out var node) && node.GetValue(out value);
        }

        /// <summary>
        /// Добавить пару ключ-значение к дереву
        /// </summary>
        /// <param name="key">Ключ нового элемента (нечувствительный к регистру)</param>
        /// <param name="value">Значение, ассоциированное с <paramref name="key"/></param>
        public void Add(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"'{nameof(key)}' не может быть null или empty.", nameof(key));

            GetOrAddNode(key).SetValue(value);
        }

        /// <summary>
        /// Очистить дерево
        /// </summary>
        public void Clear()
        {
            _root.Children.Clear();
        }

        /// <summary>
        /// Получает все пары ключ-значение для ключей, начинающихся с полученного префикса
        /// </summary>
        /// <param name="prefix">Префикс для поиска (нечувствительный к регистру)</param>
        /// <returns>
        /// Все пары ключ-значение для ключей, начинающихся с <paramref name="prefix"/> 
        /// </returns>
        public IEnumerable<KeyValuePair<string, TValue>> Search(string prefix)
        {
            if (prefix is null)
                throw new ArgumentNullException(nameof(prefix));

            if (!Find(prefix, out var node))
                return Enumerable.Empty<KeyValuePair<string, TValue>>();

            // Обход вначале в глубину
            IEnumerable<KeyValuePair<string, TValue>> traverse(TrieNode n, string s)
            {
                if (n.GetValue(out var value))
                    yield return new KeyValuePair<string, TValue>(_prefix + s, value);
                foreach (var (c, child) in n.Children)
                {
                    foreach (var kv in traverse(child, s + c))
                        yield return kv;
                }
            }
            return traverse(node, prefix);
        }

        /// <summary>
        /// Удаляет элемент с полученным ключом, если он представлен
        /// </summary>
        /// <param name="key">Ключ элемента для удаления (нечувствительный к регистру)</param>
        public void Remove(string key)
        {
            Remove(_root, key);
        }

        /// <summary>
        /// Возвращает значение с указанным ключом, добавляя новый элемент, если таковой не существует
        /// </summary>
        /// <param name="key">Ключ элемента для добавления (нечувствительный к регистру)</param>
        /// <param name="valueFactory">Функция для получения нового значения, если оно не было найдено</param>
        /// <returns>
        /// Существующее значение для полученного ключа, если найдено, в противном случае новое вставленное значение
        /// </returns>
        public TValue GetOrAdd(string key, Func<TValue> valueFactory)
        {
            var node = GetOrAddNode(key);
            if (node.GetValue(out var value))
                return value;
            value = valueFactory();
            node.SetValue(value);
            return value;
        }

        /// <summary>
        /// Пытается удалить все элементы с ключами, начинающимися с указанного префикса
        /// </summary>
        /// <param name="prefix">Префикс элементов для удаления (нечувствительный к регистру)</param>
        /// <param name="subtree">Поддерево, содержащее все удалённые элементы, если были найдены</param>
        /// <returns>
        /// Истинно, если префикс был успешно удалён из дерева, в противном случае ложно
        /// </returns>
        public bool Prune(string prefix, out ConcurrentTrie<TValue> subtree)
        {
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException($"'{nameof(prefix)}' не может быть null или empty.", nameof(prefix));

            subtree = default;
            var node = _root;
            TrieNode parent = null;
            char last = default;
            foreach (var c in prefix)
            {
                parent = node;
                if (!node.Children.TryGetValue(c, out node))
                    return false;
                last = c;
            }
            if (parent?.Children.TryRemove(last, out var subtreeRoot) == true)
                subtree = new ConcurrentTrie<TValue>(subtreeRoot, prefix);
            return true;
        }

        private TrieNode GetOrAddNode(string key)
        {
            var node = _root;
            foreach (var c in key)
                node = node.Children.GetOrAdd(c, _ => new());
            return node;
        }

        private bool Find(string key, out TrieNode node)
        {
            node = _root;
            foreach (var c in key)
            {
                if (!node.Children.TryGetValue(c, out node))
                    return false;
            }
            return true;
        }

        private bool Remove(TrieNode node, string key)
        {
            if (key.Length == 0)
            {
                if (node.GetValue(out _))
                    node.RemoveValue();
                return !node.Children.IsEmpty;
            }
            var c = key[0];
            if (node.Children.TryGetValue(c, out var child))
            {
                if (!Remove(child, key[1..]))
                    node.Children.TryRemove(new(c, child));
            }
            return true;
        }
    }
}

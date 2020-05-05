using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Miren
{
	public struct AtomicBag<T>
	{
		private Stack<T> data;

		public static AtomicBag<T> Create()
		{
			return new AtomicBag<T>
			{
				data = new Stack<T>(),
			};
		}

		public void Add(T item)
		{
			lock (data)
			{
				data.Push(item);
			}
		}

		public T Take()
		{
			lock (data)
			{
				return data.Pop();
			}
		}

		public bool TryTake(out T item)
		{
			lock (data)
			{
				if (data.Count == 0)
				{
					item = default;
					return false;
				}

				item = data.Pop();
				return true;
			}
		}
	}

	public struct ComponentPool<T> where T : Component
	{
		private T prefab;
		private AtomicBag<T> bag;

		public static ComponentPool<T> Create(T prefab)
		{
			return new ComponentPool<T>()
			{
				prefab = prefab,
				bag = AtomicBag<T>.Create(),
			};
		}

		public T Get()
		{
			return bag.TryTake(out T obj) ? obj : Object.Instantiate(prefab);
		}

		public void Add(T obj)
		{
			bag.Add(obj);
		}
	}

	public struct ArrayPool<T>
	{
		private AtomicBag<T[]> bag;
		private int length;

		public static ArrayPool<T> Create(int length)
		{
			return new ArrayPool<T>()
			{
				bag = AtomicBag<T[]>.Create(),
				length = length,
			};
		}

		public T[] Get()
		{
			return bag.TryTake(out T[] obj) ? obj : new T[length];
		}

		public void Add(T[] obj)
		{
			bag.Add(obj);
		}
	}

	public struct NewPool<T> where T : new()
	{
		private static NewPool<T> sharedPool = Create();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T SGet()
		{
			return sharedPool.Get();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SAdd(T obj)
		{
			sharedPool.Add(obj);
		}

		private AtomicBag<T> bag;

		public static NewPool<T> Create()
		{
			return new NewPool<T>()
			{
				bag = AtomicBag<T>.Create(),
			};
		}

		public T Get()
		{
			return bag.TryTake(out T obj) ? obj : new T();
		}

		public void Add(T obj)
		{
			bag.Add(obj);
		}
	}

	public struct ListPool<T>
	{
		private static ListPool<T> sharedPool = Create();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static List<T> SGet()
		{
			return sharedPool.Get();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SAdd(List<T> obj)
		{
			sharedPool.Add(obj);
		}

		private NewPool<List<T>> newPool;

		public static ListPool<T> Create()
		{
			return new ListPool<T>()
			{
				newPool = NewPool<List<T>>.Create()
			};
		}

		public ListPool<T> From(NewPool<List<T>> newPool)
		{
			return new ListPool<T>()
			{
				newPool = newPool
			};
		}

		public List<T> Get()
		{
			return newPool.Get();
		}

		public void Add(List<T> list)
		{
			list.Clear();
			newPool.Add(list);
		}
	}

	public static class ListExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddToStaticPool<T>(this List<T> list)
		{
			ListPool<T>.SAdd(list);
		}
	}
}

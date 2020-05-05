using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Miren
{
	public static class SenderReceiver
	{
		public static (ThreadSender<T> sender, ThreadReceiver<T> receiver) Create<T>()
		{
			ConcurrentQueue<T> queue = new ConcurrentQueue<T>();
			ThreadSender<T> sender = new ThreadSender<T>
			{
				Queue = queue,
			};
			ThreadReceiver<T> receiver = new ThreadReceiver<T>
			{
				Queue = queue,
			};

			return (sender, receiver);
		}

		public static ThreadSender<T> GetSender<T>(ThreadReceiver<T> receiver)
		{
			return new ThreadSender<T>
			{
				Queue = receiver.Queue,
			};
		}

		public static ThreadReceiver<T> GetReceiver<T>(ThreadSender<T> sender)
		{
			return new ThreadReceiver<T>
			{
				Queue = sender.Queue
			};
		}
	}

	public struct ThreadSender<T>
	{
		public ConcurrentQueue<T> Queue;

		public void Send(T data)
		{
			Queue.Enqueue(data);
		}
	}

	public struct ThreadReceiver<T>
	{
		public ConcurrentQueue<T> Queue;

		public bool TryReceive(out T data)
		{
			return Queue.TryDequeue(out data);
		}
	}

	public abstract class MainThreadReceiver<T> : MonoBehaviour
	{
		[NonSerialized]
		public ThreadReceiver<T> Receiver;

		private void Awake()
		{
			Receiver = new ThreadReceiver<T>() { Queue = new ConcurrentQueue<T>() };
		}

		public ThreadSender<T> GetSender()
		{
			return SenderReceiver.GetSender(Receiver);
		}
	}
}
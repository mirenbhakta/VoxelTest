using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace Miren
{
	public class ChunkManager : MonoBehaviour
	{
		[SerializeField]
		private Transform player;

		[SerializeField]
		private float renderDistance, unloadOffset;

		private Vector3Int previousPosition;

		private Dictionary<Vector3Int, Chunk> loadedChunks;

		private HashSet<Chunk> chunksToUnload;

		private List<Vector3Int> chunkOffsets;

		private int offsetIndex = -1;

		private Stopwatch sw;

		private void Awake()
		{
			loadedChunks = new Dictionary<Vector3Int, Chunk>();
			chunksToUnload = new HashSet<Chunk>();
			chunkOffsets = new List<Vector3Int>();
			previousPosition = Vector3Int.FloorToInt(player.position / Chunk.ChunkSize) + Vector3Int.up;
			sw = new Stopwatch();

			RecalculateOffsets();
		}

		public void LoadChunks()
		{
			float sqrUnloadDist = renderDistance + unloadOffset;
			sqrUnloadDist *= sqrUnloadDist;

			Vector3Int pos = Vector3Int.FloorToInt(player.position / Chunk.ChunkSize);

			// only run if player has moved to a different chunk
			if (pos != previousPosition)
			{
				previousPosition = pos;

				chunksToUnload.Clear();

				foreach (KeyValuePair<Vector3Int, Chunk> kvp in loadedChunks)
				{
					int dist = (pos - kvp.Key).sqrMagnitude;
					if (dist >= sqrUnloadDist)
					{
						Chunk chunk = kvp.Value;
						chunk.Disable();
						NewPool<Chunk>.SAdd(chunk);
						chunksToUnload.Add(kvp.Value);
					}
				}

				foreach (Chunk chunk in chunksToUnload)
				{
					loadedChunks.Remove(chunk.Position);
				}

				offsetIndex = 0;
			}

			sw.Restart();

			while (sw.Elapsed.TotalMilliseconds < 1)
			{
				offsetIndex = (offsetIndex + 1) % chunkOffsets.Count;

				Vector3Int chunkPos = chunkOffsets[offsetIndex] + pos;

				if (!loadedChunks.ContainsKey(chunkPos))
				{
					Chunk c = NewPool<Chunk>.SGet();
					c.Position = chunkPos;

					if (c.Renderer == null)
					{
						c.Renderer = World.ChunkRendererPool.Get();
						c.Renderer.transform.SetParent(transform);
					}

					c.Enable(c.Position);
					loadedChunks.Add(c.Position, c);
				}
			}
		}

		public void RecalculateOffsets()
		{
			float sqrDist = renderDistance * renderDistance;

			Queue<Vector3Int> fillQueue = NewPool<Queue<Vector3Int>>.SGet();
			HashSet<Vector3Int> visited = NewPool<HashSet<Vector3Int>>.SGet();

			visited.Clear();
			fillQueue.Clear();
			chunkOffsets.Clear();

			Vector3Int pos = Vector3Int.zero;

			fillQueue.Enqueue(pos);
			visited.Add(pos);

			while (fillQueue.Count != 0)
			{
				Vector3Int chunkPos = fillQueue.Dequeue();

				chunkOffsets.Add(chunkPos);

				for (int i = 0; i < 6; i++)
				{
					Vector3Int neighbor = chunkPos + Chunk.NeighborOffsets[i];

					if ((pos - neighbor).sqrMagnitude <= sqrDist && !visited.Contains(neighbor))
					{
						fillQueue.Enqueue(neighbor);
						visited.Add(neighbor);
					}
				}
			}

			NewPool<Queue<Vector3Int>>.SAdd(fillQueue);
			NewPool<HashSet<Vector3Int>>.SAdd(visited);
		}
	}
}

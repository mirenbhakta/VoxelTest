using System;
using System.Collections.Generic;
using Miren.Items;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Miren
{
	public class World : MonoBehaviour
	{
		public static ComponentPool<ChunkRenderer> ChunkRendererPool;
		
		[SerializeField]
		private Material baseMat;

		[SerializeField]
		private ChunkRenderer rendererPrefab;

		[SerializeField]
		private ChunkManager chunkManager;
		
		[SerializeField]
		private Block[] blocks;

		private MaterialCache materialCache;

		private HashSet<Chunk> dirtyChunks;

		private Dictionary<int, Block> blockDirectory;

		private void Awake()
		{
			ChunkRendererPool = ComponentPool<ChunkRenderer>.Create(rendererPrefab);

			materialCache = MaterialCache.Create();
			dirtyChunks = new HashSet<Chunk>();

			blockDirectory = new Dictionary<int, Block>(blocks.Length);
			foreach (Block b in blocks)
			{
				b.InitMaterial(materialCache, baseMat);
				blockDirectory.Add(b.ID, b);
			}
		}

		private void Update()
		{
			chunkManager.LoadChunks();
		}

		private void RedrawDirty()
		{
			foreach (Chunk chunk in dirtyChunks)
			{
				
			}
		}

		public Block GetBlockData(int id)
		{
			return blockDirectory[id];
		}

		public static Vector3Int WorldToChunkSpace(Vector3Int pos)
		{
			return pos / Chunk.ChunkSize;
		}

		public static Vector3 WorldToChunkSpace(Vector3 pos)
		{
			return pos / Chunk.ChunkSize;
		}
	}
}

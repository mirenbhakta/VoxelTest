using System;
using System.Runtime.CompilerServices;
using Miren.Items;
using UnityEngine;

namespace Miren
{
	[Flags]
	public enum BlockFaceMask : byte
	{
		None = 0,
		Up = 1,
		North = 1 << 1,
		East = 1 << 2,
		South = 1 << 3,
		West = 1 << 4,
		Down = 1 << 5,
		All = (1 << 6) - 1,
	}

	public static class BlockFaceMaskExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFlag(this BlockFaceMask self, BlockFaceMask mask)
		{
			return (self & mask) == mask;
		}
	}

	public struct BlockData
	{
		public int ID;
		public BlockFaceMask CullMask;
	}

	public class Chunk
	{
		// 2^4 = 16
		public const int Log2ChunkSize = 4;
		public const int ChunkSize = 1 << 4;

		public static readonly Vector3Int[] NeighborOffsets =
		{
			new Vector3Int(0, 1, 0),
			new Vector3Int(0, 0, 1),
			new Vector3Int(1, 0, 0),
			new Vector3Int(0, 0, -1),
			new Vector3Int(-1, 0, 0),
			new Vector3Int(0, -1, 0),
		};

		public static readonly BlockFaceMask[] MaskOrder =
		{
			BlockFaceMask.Up,
			BlockFaceMask.North,
			BlockFaceMask.East,
			BlockFaceMask.South,
			BlockFaceMask.West,
			BlockFaceMask.Down,
		};

		public static readonly Chunk Empty = new Chunk();

		private BlockData[] blocks;

		private ChunkRenderer renderer;

		private Chunk[] neighbors;

		public Vector3Int Position { get; set; }

		public ChunkRenderer Renderer
		{
			get => renderer;
			set => renderer = value;
		}

		public Chunk()
		{
			blocks = new BlockData[ChunkSize * ChunkSize * ChunkSize];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BlockData GetBlock(Vector3Int pos)
		{
			return blocks[pos.x + pos.y * ChunkSize + pos.z * ChunkSize * ChunkSize];
		}

		public void SetBlock(Vector3Int pos, BlockData blockData)
		{
			blocks[pos.x + pos.y * ChunkSize + pos.z * ChunkSize * ChunkSize] = blockData;
		}

		public void Enable(Vector3Int newPos)
		{
			Position = newPos;
			renderer.transform.position = newPos * ChunkSize;
			renderer.gameObject.SetActive(true);
		}

		public void Disable()
		{
			renderer.gameObject.SetActive(false);
		}

		public void RecalculateMasks()
		{
			#if UNITY_ASSERTIONS
			Debug.Assert(neighbors != null);
			for (int i = 0; i < neighbors.Length; i++)
			{
				Debug.Assert(neighbors[i] != null);
			}
			#endif

			for (int z = 1; z < ChunkSize - 1; z++)
			{
				for (int y = 1; y < ChunkSize - 1; y++)
				{
					for (int x = 1; x < ChunkSize - 1; x++)
					{
						Vector3Int pos = new Vector3Int(x, y, z);
						BlockData self = GetBlock(pos);

						for (int i = 0; i < 6; i++)
						{
							BlockData neighbor = GetBlock(pos + NeighborOffsets[i]);

							if (neighbor.ID == 0)
							{
								self.CullMask |= MaskOrder[i];
							}
						}

						SetBlock(pos, self);
					}
				}
			}

			RecalculateUpDown(neighbors[0], 15, 0, BlockFaceMask.Up);
			RecalculateNorthSouth(neighbors[1], 15, 0, BlockFaceMask.North);
			RecalculateEastWest(neighbors[2], 15, 0, BlockFaceMask.East);
			RecalculateNorthSouth(neighbors[3], 0, 15, BlockFaceMask.South);
			RecalculateEastWest(neighbors[4], 0, 15, BlockFaceMask.West);
			RecalculateUpDown(neighbors[5], 0, 15, BlockFaceMask.Down);
		}

		private void RecalculateUpDown(Chunk neighbor, int thisY, int neighborY, BlockFaceMask face)
		{
			for (int z = 0; z < ChunkSize; z++)
			{
				for (int x = 0; x < ChunkSize; x++)
				{
					Vector3Int pos = new Vector3Int(x, thisY, z);
					BlockData self = GetBlock(pos);

					Vector3Int neighborPos = new Vector3Int(x, neighborY, z);
					BlockData other = neighbor.GetBlock(neighborPos);

					if (other.ID == 0)
					{
						self.CullMask |= face;
					}
				}
			}
		}

		private void RecalculateNorthSouth(Chunk neighbor, int thisZ, int neighborZ, BlockFaceMask face)
		{
			for (int y = 0; y < ChunkSize; y++)
			{
				for (int x = 0; x < ChunkSize; x++)
				{
					Vector3Int pos = new Vector3Int(x, y, thisZ);
					BlockData self = GetBlock(pos);

					Vector3Int neighborPos = new Vector3Int(x, y, neighborZ);
					BlockData other = neighbor.GetBlock(neighborPos);

					if (other.ID == 0)
					{
						self.CullMask |= face;
					}
				}
			}
		}

		private void RecalculateEastWest(Chunk neighbor, int thisX, int neighborX, BlockFaceMask face)
		{
			for (int z = 0; z < ChunkSize; z++)
			{
				for (int y = 0; y < ChunkSize; y++)
				{
					Vector3Int pos = new Vector3Int(thisX, y, z);
					BlockData self = GetBlock(pos);

					Vector3Int neighborPos = new Vector3Int(neighborX, y, z);
					BlockData other = neighbor.GetBlock(neighborPos);

					if (other.ID == 0)
					{
						self.CullMask |= face;
					}
				}
			}
		}

		public void Dispose()
		{
			renderer.AddToPool();
		}
	}
}

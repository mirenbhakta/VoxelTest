using System;
using System.Collections.Generic;
using Miren.Items;
using UnityEngine;

namespace Miren
{
	public struct SimpleMesher : IChunkMesher
	{
		private static readonly int[] trianglesOffsets =
		{
			0, 1, 2, 0, 2, 3
		};

		public static SimpleMesher Create()
		{
			return new SimpleMesher();
		}

		private void AddTriangles(List<ushort> triangles, ref int vertexIndex)
		{
			for (int i = 0; i < trianglesOffsets.Length; i++)
			{
				triangles.Add((ushort)(vertexIndex + trianglesOffsets[i]));
			}

			vertexIndex += 4;
		}

		private void AddUV(Vector2[] src, List<Vector2> target)
		{
			for (int i = 0; i < src.Length; i++)
			{
				target.Add(src[i]);
			}
		}

		public void GenerateMesh(World world, Chunk chunk, MeshData meshData)
		{
			List<Vector2> uv = meshData.GetUV();
			List<Vector3> vertices = meshData.GetVertices();
			int vertexIndex = vertices.Count;

			for (int z = 0; z < Chunk.ChunkSize; z++)
			{
				for (int y = 0; y < Chunk.ChunkSize; y++)
				{
					for (int x = 0; x < Chunk.ChunkSize; x++)
					{
						Vector3 pos = new Vector3(x, y, z);

						BlockData block = chunk.GetBlock(new Vector3Int(x, y, z));
						if (block.ID == 0)
						{
							continue;
						}

						Block blockData = world.GetBlockData(block.ID);

						if (block.CullMask.IsFlag(BlockFaceMask.Up))
						{
							vertices.Add(pos + new Vector3(0, 1, 0));
							vertices.Add(pos + new Vector3(0, 1, 1));
							vertices.Add(pos + new Vector3(1, 1, 1));
							vertices.Add(pos + new Vector3(1, 1, 0));

							List<ushort> triangles = meshData.GetTriangles(blockData.textures.UpMat);

							AddTriangles(triangles, ref vertexIndex);

							AddUV(blockData.textures.UpData.UV, uv);
						}

						if (block.CullMask.IsFlag(BlockFaceMask.North))
						{
							vertices.Add(pos + new Vector3(1, 0, 1));
							vertices.Add(pos + new Vector3(1, 1, 1));
							vertices.Add(pos + new Vector3(0, 1, 1));
							vertices.Add(pos + new Vector3(0, 0, 1));

							List<ushort> triangles = meshData.GetTriangles(blockData.textures.NorthMat);

							AddTriangles(triangles, ref vertexIndex);

							AddUV(blockData.textures.NorthData.UV, uv);
						}

						if (block.CullMask.IsFlag(BlockFaceMask.East))
						{
							vertices.Add(pos + new Vector3(1, 0, 0));
							vertices.Add(pos + new Vector3(1, 1, 0));
							vertices.Add(pos + new Vector3(1, 1, 1));
							vertices.Add(pos + new Vector3(1, 0, 1));

							List<ushort> triangles = meshData.GetTriangles(blockData.textures.EastMat);

							AddTriangles(triangles, ref vertexIndex);

							AddUV(blockData.textures.EastData.UV, uv);
						}

						if (block.CullMask.IsFlag(BlockFaceMask.South))
						{
							vertices.Add(pos + new Vector3(0, 0, 0));
							vertices.Add(pos + new Vector3(0, 1, 0));
							vertices.Add(pos + new Vector3(1, 1, 0));
							vertices.Add(pos + new Vector3(1, 0, 0));

							List<ushort> triangles = meshData.GetTriangles(blockData.textures.SouthMat);

							AddTriangles(triangles, ref vertexIndex);

							AddUV(blockData.textures.SouthData.UV, uv);
						}

						if (block.CullMask.IsFlag(BlockFaceMask.West))
						{
							vertices.Add(pos + new Vector3(0, 0, 1));
							vertices.Add(pos + new Vector3(0, 1, 1));
							vertices.Add(pos + new Vector3(0, 1, 0));
							vertices.Add(pos + new Vector3(0, 0, 0));

							List<ushort> triangles = meshData.GetTriangles(blockData.textures.WestMat);

							AddTriangles(triangles, ref vertexIndex);

							AddUV(blockData.textures.WestData.UV, uv);
						}

						if (block.CullMask.IsFlag(BlockFaceMask.Down))
						{
							vertices.Add(pos + new Vector3(0, 0, 1));
							vertices.Add(pos + new Vector3(0, 0, 0));
							vertices.Add(pos + new Vector3(1, 0, 0));
							vertices.Add(pos + new Vector3(1, 0, 1));

							List<ushort> triangles = meshData.GetTriangles(blockData.textures.DownMat);

							AddTriangles(triangles, ref vertexIndex);

							AddUV(blockData.textures.DownData.UV, uv);
						}
					}
				}
			}
		}
	}
}

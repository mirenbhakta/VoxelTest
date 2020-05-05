using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;


namespace Miren
{
	public struct MeshData : IDisposable
	{
		private List<Vector3> vertices;
		private List<List<ushort>> triangles;
		private List<Vector2> uv;

		private Dictionary<Material, int> matToSubMeshIndex;

		public static MeshData Create()
		{
			return new MeshData()
			{
				vertices = ListPool<Vector3>.SGet(),
				triangles = ListPool<List<ushort>>.SGet(),
				uv = ListPool<Vector2>.SGet(),
				matToSubMeshIndex = NewPool<Dictionary<Material, int>>.SGet(),
			};
		}

		public List<Vector3> GetVertices()
		{
			return vertices;
		}

		public List<ushort> GetTriangles(Material mat)
		{
			if (!matToSubMeshIndex.TryGetValue(mat, out int index))
			{
				if (matToSubMeshIndex.Count == triangles.Count)
				{
					triangles.Add(ListPool<ushort>.SGet());
				}

				index = matToSubMeshIndex.Count;
				matToSubMeshIndex.Add(mat, index);
			}

			return GetTriangles(index);
		}

		public List<ushort> GetTriangles(int subMeshIndex)
		{
			return triangles[subMeshIndex];
		}

		public List<Vector2> GetUV()
		{
			return uv;
		}

		public void AppendMesh(MeshData mesh, float3 offset)
		{
			int vertexIndex = vertices.Count;

			for (int i = 0; i < mesh.vertices.Count; i++)
			{
				vertices.Add((float3)mesh.vertices[i] + offset);
			}

			foreach (KeyValuePair<Material, int> kvp in mesh.matToSubMeshIndex)
			{
				List<ushort> thisSubMesh = GetTriangles(kvp.Key);
				List<ushort> otherSubMesh = mesh.GetTriangles(kvp.Value);

				for (int i = 0; i < otherSubMesh.Count; i++)
				{
					thisSubMesh.Add((ushort)(otherSubMesh[i] + vertexIndex));
				}

				uv.AddRange(mesh.uv);
			}
		}
		
		public Mesh ToMesh(MeshRenderer renderer)
		{
			Mesh mesh = new Mesh();
			ToMesh(mesh, renderer);
			return mesh;
		}

		public void ToMesh(Mesh mesh, MeshRenderer renderer)
		{
			// unavoidable GC because MeshRenderer has no non-alloc SetSharedMaterials
			renderer.sharedMaterials = matToSubMeshIndex.Keys.ToArray();

			mesh.Clear();
			mesh.SetVertices(vertices);

			mesh.subMeshCount = Mathf.Max(mesh.subMeshCount, matToSubMeshIndex.Count);

			for (int i = 0; i < matToSubMeshIndex.Count; i++)
			{
				mesh.SetTriangles(triangles[i], i, false);
			}

			mesh.SetUVs(0, uv);

			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
		}

		public void Clear()
		{
			vertices.Clear();
			for (int i = 0; i < triangles.Count; i++)
			{
				triangles[i].Clear();
			}

			uv.Clear();
			matToSubMeshIndex.Clear();
		}

		public void Dispose()
		{
			vertices.AddToStaticPool();
			vertices = null;

			for (int i = 0; i < triangles.Count; i++)
			{
				triangles[i].AddToStaticPool();
			}

			triangles.AddToStaticPool();
			triangles = null;
			uv.AddToStaticPool();
			uv = null;

			matToSubMeshIndex.Clear();
			NewPool<Dictionary<Material, int>>.SAdd(matToSubMeshIndex);
			matToSubMeshIndex = null;
		}
	}
}

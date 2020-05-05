using System;
using System.Linq;
using UnityEngine;

namespace Miren
{
	public class ChunkRenderer : MonoBehaviour
	{
		public MeshFilter MeshFilter;
		public MeshRenderer MeshRenderer;

		[NonSerialized]
		public Mesh Mesh;

		private void OnDestroy()
		{
			NewPool<Mesh>.SAdd(Mesh);
		}

		public void AddToPool()
		{
			gameObject.SetActive(false);
			World.ChunkRendererPool.Add(this);
		}
	}
}

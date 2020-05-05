using System;
using UnityEngine;

namespace Miren
{
	public class MeshReceiver : MainThreadReceiver<(ChunkRenderer, MeshData)>
	{
		private void LateUpdate()
		{
			while (Receiver.TryReceive(out var data))
			{
				(ChunkRenderer chunk, MeshData meshData) = data;
				meshData.ToMesh(chunk.Mesh, chunk.MeshRenderer);
				meshData.Dispose();
			}
		}
	}
}
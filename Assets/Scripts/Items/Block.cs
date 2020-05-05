using System;
using UnityEngine;

namespace Miren.Items
{
	[CreateAssetMenu]
	public class Block : Item
	{
		public int ID;

		public FaceTextures textures;

		public void InitMaterial(MaterialCache matCache, Material baseMat)
		{
			textures.Init(matCache, baseMat);
		}
	}

	[Serializable]
	public struct FaceTextures
	{
		[SerializeField]
		private Sprite up, north, east, south, west, down;

		[NonSerialized]
		public SpriteData UpData, NorthData, EastData, SouthData, WestData, DownData;

		[NonSerialized]
		public Material UpMat, NorthMat, EastMat, SouthMat, WestMat, DownMat;

		public void Init(MaterialCache matCache, Material baseMat)
		{
			UpData = SpriteData.Create(up);
			NorthData = SpriteData.Create(north);
			EastData = SpriteData.Create(east);
			SouthData = SpriteData.Create(south);
			WestData = SpriteData.Create(west);
			DownData = SpriteData.Create(down);

			InitFace(matCache, baseMat, up, out UpMat);
			InitFace(matCache, baseMat, north, out NorthMat);
			InitFace(matCache, baseMat, east, out EastMat);
			InitFace(matCache, baseMat, south, out SouthMat);
			InitFace(matCache, baseMat, west, out WestMat);
			InitFace(matCache, baseMat, down, out DownMat);
		}

		private static void InitFace(MaterialCache matCache, Material baseMat, Sprite face, out Material faceMat)
		{
			if (matCache.Add(face, baseMat, out faceMat))
			{
				faceMat.mainTexture = face.texture;
			}
		}
	}
}

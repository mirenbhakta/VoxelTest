using UnityEngine;

namespace Miren
{
	public struct SpriteData
	{
		public Vector2[] UV;

		public static SpriteData Create(Sprite sprite)
		{
			SpriteData data = new SpriteData();
			data.UV = new Vector2[4];
			Rect rect = sprite.textureRect;

			data.UV[0] = new Vector2(rect.xMin, rect.yMin);
			data.UV[1] = new Vector2(rect.xMin, rect.yMax);
			data.UV[2] = new Vector2(rect.xMax, rect.yMax);
			data.UV[3] = new Vector2(rect.xMax, rect.yMin);

			return data;
		}
	}
}

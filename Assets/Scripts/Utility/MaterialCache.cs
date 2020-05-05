using System;
using System.Collections.Generic;
using UnityEngine;

public struct MaterialCache : IDisposable
{
	private Dictionary<Texture, Material> map;

	public static MaterialCache Create()
	{
		return new MaterialCache()
		{
			map = new Dictionary<Texture, Material>(),
		};
	}

	public bool Contains(Texture tex)
	{
		return map.ContainsKey(tex);
	}

	public bool Contains(Sprite sprite)
	{
		return Contains(sprite.texture);
	}

	public Material Get(Texture tex)
	{
		return map[tex];
	}

	public Material Get(Sprite sprite)
	{
		return Get(sprite.texture);
	}

	public bool TryGet(Texture tex, out Material mat)
	{
		return map.TryGetValue(tex, out mat);
	}

	public bool TryGet(Sprite sprite, out Material mat)
	{
		return TryGet(sprite.texture, out mat);
	}

	public bool Add(Texture tex, Material baseMat, out Material createdMaterial)
	{
		if (map.TryGetValue(tex, out createdMaterial))
		{
			// material already exists, but give the material to the caller so they can hold a ref to it
			return false;
		}

		createdMaterial = new Material(baseMat);
		map.Add(tex, createdMaterial);
		// this is a new material, so return true to let the caller populate its fields
		return true;
	}

	public bool Add(Sprite sprite, Material baseMat, out Material createdMaterial)
	{
		return Add(sprite.texture, baseMat, out createdMaterial);
	}

	public void Dispose()
	{
		foreach (KeyValuePair<Texture, Material> keyValuePair in map)
		{
			UnityEngine.Object.Destroy(keyValuePair.Value);
		}
		
		map.Clear();
	}
}

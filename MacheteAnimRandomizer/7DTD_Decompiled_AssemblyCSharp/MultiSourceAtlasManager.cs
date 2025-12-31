using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001025 RID: 4133
public class MultiSourceAtlasManager : MonoBehaviour
{
	// Token: 0x06008301 RID: 33537 RVA: 0x0034EDB0 File Offset: 0x0034CFB0
	public UIAtlas GetAtlasForSprite(string _spriteName)
	{
		MultiSourceAtlasManager.BaseAtlas baseAtlas;
		if (this.atlasesForSprites.TryGetValue(_spriteName, out baseAtlas))
		{
			return baseAtlas.Atlas;
		}
		if (this.atlases.Count <= 0)
		{
			return null;
		}
		return this.atlases[0].Atlas;
	}

	// Token: 0x06008302 RID: 33538 RVA: 0x0034EDF8 File Offset: 0x0034CFF8
	public void AddAtlas(UIAtlas _atlas, bool _isLoadingInGame)
	{
		MultiSourceAtlasManager.BaseAtlas item = new MultiSourceAtlasManager.BaseAtlas
		{
			Parent = _atlas.gameObject,
			Atlas = _atlas,
			IsLoadedInGame = _isLoadingInGame
		};
		this.atlases.Add(item);
		_atlas.name = base.name;
		this.recalcSpriteSources();
	}

	// Token: 0x06008303 RID: 33539 RVA: 0x0034EE44 File Offset: 0x0034D044
	public void CleanupAfterGame()
	{
		for (int i = this.atlases.Count - 1; i >= 0; i--)
		{
			MultiSourceAtlasManager.BaseAtlas baseAtlas = this.atlases[i];
			if (baseAtlas.IsLoadedInGame)
			{
				this.atlases.RemoveAt(i);
				UnityEngine.Object.Destroy(baseAtlas.Atlas.spriteMaterial.mainTexture);
				UnityEngine.Object.Destroy(baseAtlas.Parent);
			}
		}
		this.recalcSpriteSources();
	}

	// Token: 0x06008304 RID: 33540 RVA: 0x0034EEB0 File Offset: 0x0034D0B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void recalcSpriteSources()
	{
		foreach (MultiSourceAtlasManager.BaseAtlas baseAtlas in this.atlases)
		{
			foreach (UISpriteData uispriteData in baseAtlas.Atlas.spriteList)
			{
				this.atlasesForSprites[uispriteData.name] = baseAtlas;
			}
		}
	}

	// Token: 0x06008305 RID: 33541 RVA: 0x0034EF50 File Offset: 0x0034D150
	public static MultiSourceAtlasManager Create(GameObject _parent, string _name)
	{
		return new GameObject(_name)
		{
			transform = 
			{
				parent = _parent.transform
			}
		}.AddComponent<MultiSourceAtlasManager>();
	}

	// Token: 0x0400651F RID: 25887
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<MultiSourceAtlasManager.BaseAtlas> atlases = new List<MultiSourceAtlasManager.BaseAtlas>();

	// Token: 0x04006520 RID: 25888
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Dictionary<string, MultiSourceAtlasManager.BaseAtlas> atlasesForSprites = new Dictionary<string, MultiSourceAtlasManager.BaseAtlas>();

	// Token: 0x02001026 RID: 4134
	[PublicizedFrom(EAccessModifier.Private)]
	public class BaseAtlas
	{
		// Token: 0x04006521 RID: 25889
		public GameObject Parent;

		// Token: 0x04006522 RID: 25890
		public UIAtlas Atlas;

		// Token: 0x04006523 RID: 25891
		public bool IsLoadedInGame;
	}
}

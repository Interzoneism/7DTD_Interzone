using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AEB RID: 2795
public class TextureLoadingManager : MonoBehaviour
{
	// Token: 0x060055EA RID: 21994 RVA: 0x00232092 File Offset: 0x00230292
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		TextureLoadingManager.Instance = this;
	}

	// Token: 0x060055EB RID: 21995 RVA: 0x0023209C File Offset: 0x0023029C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Time.time - this.lastTimeChecked < 1f)
		{
			return;
		}
		this.lastTimeChecked = Time.time;
		for (int i = this.runningRequests.Count - 1; i >= 0; i--)
		{
			TextureLoadingManager.AsyncLoadInfo asyncLoadInfo = this.runningRequests[i];
			TextureLoadingManager.TextureInfo textureInfo;
			if (asyncLoadInfo.resRequest.isDone && this.availableTextures.TryGetValue(asyncLoadInfo.fullPath, out textureInfo))
			{
				textureInfo.tex = (Texture)asyncLoadInfo.resRequest.asset;
				textureInfo.bPending = false;
				if (asyncLoadInfo.material)
				{
					asyncLoadInfo.material.SetTexture(asyncLoadInfo.propName, textureInfo.tex);
				}
				this.runningRequests.RemoveAt(i);
			}
		}
	}

	// Token: 0x060055EC RID: 21996 RVA: 0x00232162 File Offset: 0x00230362
	public void Cleanup()
	{
		this.availableTextures.Clear();
		this.runningRequests.Clear();
	}

	// Token: 0x060055ED RID: 21997 RVA: 0x0023217C File Offset: 0x0023037C
	public void LoadTexture(Material _m, string _propName, string _assetPath, string _texName, Texture _lowResTexture)
	{
		if (!Application.isPlaying)
		{
			Texture value = Resources.Load<Texture2D>(_assetPath + _texName);
			_m.SetTexture(_propName, value);
			return;
		}
		TextureLoadingManager.TextureInfo textureInfo = null;
		string text = _assetPath + _texName;
		if (this.availableTextures.TryGetValue(text, out textureInfo) && !textureInfo.bPending)
		{
			_m.SetTexture(_propName, textureInfo.tex);
			textureInfo.refCounts++;
			return;
		}
		ResourceRequest resRequest = Resources.LoadAsync<Texture2D>(text);
		TextureLoadingManager.AsyncLoadInfo item = default(TextureLoadingManager.AsyncLoadInfo);
		item.resRequest = resRequest;
		item.propName = _propName;
		item.material = _m;
		item.fullPath = text;
		item.lowResTexture = _lowResTexture;
		this.runningRequests.Add(item);
		if (textureInfo == null)
		{
			textureInfo = new TextureLoadingManager.TextureInfo();
			textureInfo.bPending = true;
			textureInfo.refCounts = 1;
			this.availableTextures.Add(text, textureInfo);
			return;
		}
		textureInfo.refCounts++;
	}

	// Token: 0x060055EE RID: 21998 RVA: 0x00232260 File Offset: 0x00230460
	public bool UnloadTexture(string _assetPath, string _texName)
	{
		string key = _assetPath + _texName;
		TextureLoadingManager.TextureInfo textureInfo;
		if (this.availableTextures.TryGetValue(key, out textureInfo))
		{
			textureInfo.refCounts--;
			if (textureInfo.refCounts == 0)
			{
				this.availableTextures.Remove(key);
				Resources.UnloadAsset(textureInfo.tex);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060055EF RID: 21999 RVA: 0x002322B6 File Offset: 0x002304B6
	public int GetHiResTextureCount()
	{
		return this.availableTextures.Count;
	}

	// Token: 0x0400429A RID: 17050
	public static TextureLoadingManager Instance;

	// Token: 0x0400429B RID: 17051
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, TextureLoadingManager.TextureInfo> availableTextures = new Dictionary<string, TextureLoadingManager.TextureInfo>();

	// Token: 0x0400429C RID: 17052
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<TextureLoadingManager.AsyncLoadInfo> runningRequests = new List<TextureLoadingManager.AsyncLoadInfo>();

	// Token: 0x0400429D RID: 17053
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeChecked;

	// Token: 0x02000AEC RID: 2796
	[PublicizedFrom(EAccessModifier.Private)]
	public struct AsyncLoadInfo
	{
		// Token: 0x0400429E RID: 17054
		public ResourceRequest resRequest;

		// Token: 0x0400429F RID: 17055
		public string propName;

		// Token: 0x040042A0 RID: 17056
		public Material material;

		// Token: 0x040042A1 RID: 17057
		public string fullPath;

		// Token: 0x040042A2 RID: 17058
		public Texture lowResTexture;
	}

	// Token: 0x02000AED RID: 2797
	[PublicizedFrom(EAccessModifier.Private)]
	public class TextureInfo
	{
		// Token: 0x040042A3 RID: 17059
		public bool bPending;

		// Token: 0x040042A4 RID: 17060
		public int refCounts;

		// Token: 0x040042A5 RID: 17061
		public Texture tex;
	}
}

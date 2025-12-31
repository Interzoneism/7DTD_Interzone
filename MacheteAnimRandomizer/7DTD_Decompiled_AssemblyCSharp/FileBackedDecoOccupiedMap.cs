using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AAF RID: 2735
public class FileBackedDecoOccupiedMap : IDisposable
{
	// Token: 0x0600545A RID: 21594 RVA: 0x0021F074 File Offset: 0x0021D274
	public FileBackedDecoOccupiedMap(int _worldWidth, int _worldHeight)
	{
		this.width = _worldWidth;
		this.height = _worldHeight;
		this.heightHalf = this.height / 2;
		this.occupiedMap = new FileBackedArray<EnumDecoOccupied>(this.width * this.height);
		this.cacheLength = this.width * 128;
	}

	// Token: 0x0600545B RID: 21595 RVA: 0x0021F0CD File Offset: 0x0021D2CD
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetDecoChunkRowCacheStart(int offset)
	{
		return offset / this.cacheLength * this.cacheLength;
	}

	// Token: 0x0600545C RID: 21596 RVA: 0x0021F0E0 File Offset: 0x0021D2E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Cache(int offset)
	{
		if (offset >= this.cacheEnd || offset < this.cacheStart)
		{
			this.cacheStart = this.GetDecoChunkRowCacheStart(offset);
			this.cacheEnd = this.cacheStart + this.cacheLength;
			IBackedArrayHandle backedArrayHandle = this.cacheHandle;
			if (backedArrayHandle != null)
			{
				backedArrayHandle.Dispose();
			}
			this.cacheHandle = this.occupiedMap.GetReadOnlyMemory(this.cacheStart, this.cacheLength, out this.cache);
		}
	}

	// Token: 0x0600545D RID: 21597 RVA: 0x0021F154 File Offset: 0x0021D354
	public unsafe EnumDecoOccupied Get(int _offs)
	{
		this.Cache(_offs);
		return (EnumDecoOccupied)(*this.cache.Span[_offs - this.cacheStart]);
	}

	// Token: 0x0600545E RID: 21598 RVA: 0x0021F184 File Offset: 0x0021D384
	public void CopyDecoChunkRow(int row, EnumDecoOccupied[] from)
	{
		int num = this.heightHalf / 128;
		int start = (row + num) * 128 * this.width;
		Span<EnumDecoOccupied> destination;
		using (this.occupiedMap.GetSpan(start, this.cacheLength, out destination))
		{
			from.AsSpan(start, this.cacheLength).CopyTo(destination);
		}
	}

	// Token: 0x0600545F RID: 21599 RVA: 0x0021F1F8 File Offset: 0x0021D3F8
	public void Dispose()
	{
		IBackedArrayHandle backedArrayHandle = this.cacheHandle;
		if (backedArrayHandle != null)
		{
			backedArrayHandle.Dispose();
		}
		this.cacheHandle = null;
		FileBackedArray<EnumDecoOccupied> fileBackedArray = this.occupiedMap;
		if (fileBackedArray != null)
		{
			fileBackedArray.Dispose();
		}
		this.occupiedMap = null;
	}

	// Token: 0x06005460 RID: 21600 RVA: 0x0021F22C File Offset: 0x0021D42C
	public void SaveAsTexture(string path, bool includeFlatAreas = false, List<FlatArea> flatAreas = null)
	{
		Color32[] array = new Color32[this.occupiedMap.Length];
		for (int i = 0; i < this.occupiedMap.Length; i++)
		{
			Color c = Color.black;
			switch (this.Get(i))
			{
			case EnumDecoOccupied.SmallSlope:
				c = Color.blue;
				break;
			case EnumDecoOccupied.Stop_BigDeco:
				c = Color.gray;
				break;
			case EnumDecoOccupied.Perimeter:
				c = Color.red;
				break;
			case EnumDecoOccupied.Stop_AnyDeco:
				c = Color.cyan;
				break;
			case EnumDecoOccupied.Deco:
				c = Color.green;
				break;
			case EnumDecoOccupied.POI:
				c = Color.magenta;
				break;
			case EnumDecoOccupied.BigSlope:
				c = Color.yellow;
				break;
			case EnumDecoOccupied.NoneAllowed:
				c = Color.white;
				break;
			}
			array[i] = c;
		}
		if (includeFlatAreas)
		{
			if (flatAreas == null || flatAreas.Count == 0)
			{
				FlatAreaManager flatAreaManager = GameManager.Instance.World.FlatAreaManager;
				flatAreas = ((flatAreaManager != null) ? flatAreaManager.GetAllFlatAreas() : null);
			}
			if (flatAreas != null)
			{
				foreach (FlatArea flatArea in flatAreas)
				{
					foreach (Vector2i vector2i in flatArea.GetPositions())
					{
						Color red;
						if (vector2i.x == flatArea.position.x && vector2i.y == flatArea.position.z)
						{
							red = Color.red;
						}
						else if (flatArea.size == 16)
						{
							red = new Color(0.75f, 0.75f, 0.75f, 1f);
						}
						else
						{
							red = new Color(0.5f, 0.5f, 0.5f, 1f);
						}
						int num = DecoManager.CheckPosition(this.width, this.height, vector2i.x, vector2i.y);
						array[num] = red;
					}
				}
			}
		}
		Texture2D texture2D = new Texture2D(this.width, this.height);
		texture2D.SetPixels32(array);
		texture2D.Apply();
		TextureUtils.SaveTexture(texture2D, path);
		Log.Out("Saved deco texture to {0}", new object[]
		{
			path
		});
		UnityEngine.Object.Destroy(texture2D);
	}

	// Token: 0x040040A9 RID: 16553
	[PublicizedFrom(EAccessModifier.Private)]
	public FileBackedArray<EnumDecoOccupied> occupiedMap;

	// Token: 0x040040AA RID: 16554
	[PublicizedFrom(EAccessModifier.Private)]
	public int width;

	// Token: 0x040040AB RID: 16555
	[PublicizedFrom(EAccessModifier.Private)]
	public int height;

	// Token: 0x040040AC RID: 16556
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightHalf;

	// Token: 0x040040AD RID: 16557
	[PublicizedFrom(EAccessModifier.Private)]
	public int cacheLength;

	// Token: 0x040040AE RID: 16558
	[PublicizedFrom(EAccessModifier.Private)]
	public IBackedArrayHandle cacheHandle;

	// Token: 0x040040AF RID: 16559
	[PublicizedFrom(EAccessModifier.Private)]
	public ReadOnlyMemory<EnumDecoOccupied> cache;

	// Token: 0x040040B0 RID: 16560
	[PublicizedFrom(EAccessModifier.Private)]
	public int cacheStart;

	// Token: 0x040040B1 RID: 16561
	[PublicizedFrom(EAccessModifier.Private)]
	public int cacheEnd;
}

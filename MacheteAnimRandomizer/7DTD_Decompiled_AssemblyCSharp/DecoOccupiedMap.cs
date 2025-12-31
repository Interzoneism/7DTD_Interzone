using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000AA6 RID: 2726
public class DecoOccupiedMap
{
	// Token: 0x06005431 RID: 21553 RVA: 0x0021E380 File Offset: 0x0021C580
	public DecoOccupiedMap(int _worldWidth, int _worldHeight)
	{
		this.width = _worldWidth;
		this.height = _worldHeight;
		this.widthHalf = this.width / 2;
		this.heightHalf = this.height / 2;
		this.occupiedMap = new EnumDecoOccupied[this.width * this.height];
	}

	// Token: 0x06005432 RID: 21554 RVA: 0x0021E3D5 File Offset: 0x0021C5D5
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EnumDecoOccupied Get(int _offs)
	{
		return this.occupiedMap[_offs];
	}

	// Token: 0x06005433 RID: 21555 RVA: 0x0021E3E0 File Offset: 0x0021C5E0
	public EnumDecoOccupied Get(int _x, int _z)
	{
		int num = DecoManager.CheckPosition(this.width, this.height, _x, _z);
		if (num >= 0)
		{
			return this.occupiedMap[num];
		}
		return EnumDecoOccupied.NoneAllowed;
	}

	// Token: 0x06005434 RID: 21556 RVA: 0x0021E40F File Offset: 0x0021C60F
	public void Set(int _offs, EnumDecoOccupied _v)
	{
		this.occupiedMap[_offs] = _v;
	}

	// Token: 0x06005435 RID: 21557 RVA: 0x0021E41C File Offset: 0x0021C61C
	public void Set(int _x, int _z, EnumDecoOccupied _v)
	{
		int num = DecoManager.CheckPosition(this.width, this.height, _x, _z);
		if (num >= 0)
		{
			this.occupiedMap[num] = _v;
		}
	}

	// Token: 0x06005436 RID: 21558 RVA: 0x0021E44C File Offset: 0x0021C64C
	public bool CheckArea(int _x, int _z, EnumDecoOccupied _v, int _rectSizeX, int _rectSizeZ)
	{
		int num = DecoManager.CheckPosition(this.width, this.height, _x, _z);
		if (num < 0)
		{
			return true;
		}
		for (int i = 0; i < _rectSizeZ; i++)
		{
			for (int j = 0; j < _rectSizeX; j++)
			{
				if (num >= this.occupiedMap.Length)
				{
					return true;
				}
				if (this.occupiedMap[num] >= _v)
				{
					return true;
				}
				num++;
			}
			num += this.width - _rectSizeX;
		}
		return false;
	}

	// Token: 0x06005437 RID: 21559 RVA: 0x0021E4B8 File Offset: 0x0021C6B8
	public void SetArea(int _x, int _z, EnumDecoOccupied _v, int _rectSizeX, int _rectSizeZ)
	{
		int num = _x + this.widthHalf + (_z + this.heightHalf) * this.width;
		for (int i = 0; i < _rectSizeZ; i++)
		{
			for (int j = 0; j < _rectSizeX; j++)
			{
				if (num < 0 || num >= this.occupiedMap.Length)
				{
					num++;
				}
				else
				{
					if (this.occupiedMap[num] < _v)
					{
						this.occupiedMap[num] = _v;
					}
					num++;
				}
			}
			num += this.width - _rectSizeX;
		}
	}

	// Token: 0x06005438 RID: 21560 RVA: 0x0021E531 File Offset: 0x0021C731
	public EnumDecoOccupied[] GetData()
	{
		return this.occupiedMap;
	}

	// Token: 0x06005439 RID: 21561 RVA: 0x0021E53C File Offset: 0x0021C73C
	public void SaveAsTexture(string path, bool includeFlatAreas = false, List<FlatArea> flatAreas = null)
	{
		Color32[] array = new Color32[this.occupiedMap.Length];
		for (int i = 0; i < this.occupiedMap.Length; i++)
		{
			Color c = Color.black;
			switch (this.occupiedMap[i])
			{
			case EnumDecoOccupied.SmallSlope:
				c = Color.blue;
				break;
			case EnumDecoOccupied.Stop_BigDeco:
				c = Color.gray;
				break;
			case EnumDecoOccupied.Perimeter:
				if (!includeFlatAreas)
				{
					c = Color.red;
				}
				break;
			case EnumDecoOccupied.Stop_AnyDeco:
				c = Color.cyan;
				break;
			case EnumDecoOccupied.Deco:
				if (!includeFlatAreas)
				{
					c = Color.green;
				}
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

	// Token: 0x04004089 RID: 16521
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumDecoOccupied[] occupiedMap;

	// Token: 0x0400408A RID: 16522
	[PublicizedFrom(EAccessModifier.Private)]
	public int width;

	// Token: 0x0400408B RID: 16523
	[PublicizedFrom(EAccessModifier.Private)]
	public int height;

	// Token: 0x0400408C RID: 16524
	[PublicizedFrom(EAccessModifier.Private)]
	public int widthHalf;

	// Token: 0x0400408D RID: 16525
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightHalf;
}

using System;
using UnityEngine;

// Token: 0x0200085A RID: 2138
public class PrefabDataInstance
{
	// Token: 0x17000662 RID: 1634
	// (get) Token: 0x06003D98 RID: 15768 RVA: 0x0018BBBE File Offset: 0x00189DBE
	public Vector3i boundingBoxSize
	{
		get
		{
			return this.prefab.size;
		}
	}

	// Token: 0x17000663 RID: 1635
	// (get) Token: 0x06003D99 RID: 15769 RVA: 0x0018BBCC File Offset: 0x00189DCC
	public Vector2i CenterXZ
	{
		get
		{
			Vector2i result;
			result.x = this.boundingBoxPosition.x + this.prefab.size.x / 2;
			result.y = this.boundingBoxPosition.z + this.prefab.size.z / 2;
			return result;
		}
	}

	// Token: 0x17000664 RID: 1636
	// (get) Token: 0x06003D9A RID: 15770 RVA: 0x0018BC24 File Offset: 0x00189E24
	public Vector2 CenterXZV2
	{
		get
		{
			Vector2 result;
			result.x = (float)this.boundingBoxPosition.x + (float)this.prefab.size.x * 0.5f;
			result.y = (float)this.boundingBoxPosition.z + (float)this.prefab.size.z * 0.5f;
			return result;
		}
	}

	// Token: 0x17000665 RID: 1637
	// (get) Token: 0x06003D9B RID: 15771 RVA: 0x0018BC88 File Offset: 0x00189E88
	public PathAbstractions.AbstractedLocation location
	{
		get
		{
			return this.prefab.location;
		}
	}

	// Token: 0x06003D9C RID: 15772 RVA: 0x0018BC95 File Offset: 0x00189E95
	public PrefabDataInstance(int _id, Vector3i _position, byte _rotation, PrefabData _prefabData)
	{
		this.id = _id;
		this.prefab = _prefabData;
		this.boundingBoxPosition = _position;
		this.rotation = _rotation;
		this.previewColor = PrefabDataInstance.previewColorDefault;
	}

	// Token: 0x040031C5 RID: 12741
	public int id;

	// Token: 0x040031C6 RID: 12742
	public PrefabData prefab;

	// Token: 0x040031C7 RID: 12743
	public Vector3i boundingBoxPosition;

	// Token: 0x040031C8 RID: 12744
	public byte rotation;

	// Token: 0x040031C9 RID: 12745
	public Color32 previewColor;

	// Token: 0x040031CA RID: 12746
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Color32 previewColorDefault = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
}

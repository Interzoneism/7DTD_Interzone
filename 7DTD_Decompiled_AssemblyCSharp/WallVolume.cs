using System;
using System.IO;
using UnityEngine;

// Token: 0x02000A71 RID: 2673
public class WallVolume
{
	// Token: 0x0600518E RID: 20878 RVA: 0x0020C3DD File Offset: 0x0020A5DD
	public static WallVolume Create(Prefab.PrefabWallVolume psv, Vector3i _boxMin, Vector3i _boxMax)
	{
		WallVolume wallVolume = new WallVolume();
		wallVolume.SetMinMax(_boxMin, _boxMax);
		wallVolume.AddToPrefabInstance();
		return wallVolume;
	}

	// Token: 0x0600518F RID: 20879 RVA: 0x0020C3F4 File Offset: 0x0020A5F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetMinMax(Vector3i _boxMin, Vector3i _boxMax)
	{
		this.BoxMin = _boxMin;
		this.BoxMax = _boxMax;
		this.Center = (this.BoxMin + this.BoxMax).ToVector3() * 0.5f;
	}

	// Token: 0x17000849 RID: 2121
	// (get) Token: 0x06005190 RID: 20880 RVA: 0x0020C438 File Offset: 0x0020A638
	public PrefabInstance PrefabInstance
	{
		get
		{
			return this.prefabInstance;
		}
	}

	// Token: 0x06005191 RID: 20881 RVA: 0x0020C440 File Offset: 0x0020A640
	public void AddToPrefabInstance()
	{
		this.prefabInstance = GameManager.Instance.World.ChunkCache.ChunkProvider.GetDynamicPrefabDecorator().GetPrefabAtPosition(this.Center, true);
		if (this.prefabInstance != null)
		{
			this.prefabInstance.AddWallVolume(this);
		}
	}

	// Token: 0x06005192 RID: 20882 RVA: 0x0020C48C File Offset: 0x0020A68C
	public static WallVolume Read(BinaryReader _br)
	{
		WallVolume wallVolume = new WallVolume();
		_br.ReadByte();
		wallVolume.SetMinMax(new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32()), new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32()));
		return wallVolume;
	}

	// Token: 0x06005193 RID: 20883 RVA: 0x0020C4DC File Offset: 0x0020A6DC
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(1);
		_bw.Write(this.BoxMin.x);
		_bw.Write(this.BoxMin.y);
		_bw.Write(this.BoxMin.z);
		_bw.Write(this.BoxMax.x);
		_bw.Write(this.BoxMax.y);
		_bw.Write(this.BoxMax.z);
	}

	// Token: 0x04003E9A RID: 16026
	public const int BinarySize = 25;

	// Token: 0x04003E9B RID: 16027
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte VERSION = 1;

	// Token: 0x04003E9C RID: 16028
	[PublicizedFrom(EAccessModifier.Private)]
	public PrefabInstance prefabInstance;

	// Token: 0x04003E9D RID: 16029
	public Vector3i BoxMin;

	// Token: 0x04003E9E RID: 16030
	public Vector3i BoxMax;

	// Token: 0x04003E9F RID: 16031
	public Vector3 Center;
}

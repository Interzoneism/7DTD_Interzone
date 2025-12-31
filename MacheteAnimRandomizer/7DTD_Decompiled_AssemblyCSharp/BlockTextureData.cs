using System;

// Token: 0x02000144 RID: 324
public class BlockTextureData
{
	// Token: 0x0600090E RID: 2318 RVA: 0x0003F0DD File Offset: 0x0003D2DD
	public static void InitStatic()
	{
		BlockTextureData.list = new BlockTextureData[256];
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x0003F0EE File Offset: 0x0003D2EE
	public void Init()
	{
		BlockTextureData.list[this.ID] = this;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x0003F0FD File Offset: 0x0003D2FD
	public static void Cleanup()
	{
		BlockTextureData.list = null;
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x0003F108 File Offset: 0x0003D308
	public static BlockTextureData GetDataByTextureID(int textureID)
	{
		for (int i = 0; i < BlockTextureData.list.Length; i++)
		{
			if (BlockTextureData.list[i] != null && (int)BlockTextureData.list[i].TextureID == textureID)
			{
				return BlockTextureData.list[i];
			}
		}
		return null;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0003F148 File Offset: 0x0003D348
	public bool GetLocked(EntityPlayerLocal player)
	{
		if (this.LockedByPerk != "")
		{
			ProgressionValue progressionValue = player.Progression.GetProgressionValue(this.LockedByPerk);
			if (progressionValue != null && progressionValue.CalculatedLevel(player) >= (int)this.RequiredLevel)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040008BE RID: 2238
	public static BlockTextureData[] list;

	// Token: 0x040008BF RID: 2239
	public int ID;

	// Token: 0x040008C0 RID: 2240
	public ushort TextureID;

	// Token: 0x040008C1 RID: 2241
	public string Name;

	// Token: 0x040008C2 RID: 2242
	public string LocalizedName;

	// Token: 0x040008C3 RID: 2243
	public string Group;

	// Token: 0x040008C4 RID: 2244
	public ushort PaintCost;

	// Token: 0x040008C5 RID: 2245
	public bool Hidden;

	// Token: 0x040008C6 RID: 2246
	public byte SortIndex = byte.MaxValue;

	// Token: 0x040008C7 RID: 2247
	public string LockedByPerk = "";

	// Token: 0x040008C8 RID: 2248
	public ushort RequiredLevel;
}

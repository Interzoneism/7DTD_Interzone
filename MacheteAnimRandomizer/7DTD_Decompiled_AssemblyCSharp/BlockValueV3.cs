using System;
using System.Runtime.CompilerServices;

// Token: 0x02000152 RID: 338
public struct BlockValueV3
{
	// Token: 0x06000984 RID: 2436 RVA: 0x00040BA4 File Offset: 0x0003EDA4
	public static uint ConvertOldRawData(uint _rawData)
	{
		BlockValueV3.convertBV3.rawData = _rawData;
		int type = BlockValueV3.convertBV3.type;
		BlockValueV3.convertBV.type = type;
		if (!BlockValueV3.convertBV3.ischild)
		{
			BlockValueV3.convertBV.rotation = BlockValueV3.convertBV3.rotation;
			BlockValueV3.convertBV.meta = BlockValueV3.convertBV3.meta;
			BlockValueV3.convertBV.meta2 = BlockValueV3.convertBV3.meta2;
		}
		else
		{
			BlockValueV3.convertBV.parent = BlockValueV3.convertBV3.parent;
			BlockValueV3.convertBV.ischild = true;
		}
		BlockValueV3.convertBV.hasdecal = BlockValueV3.convertBV3.hasdecal;
		return BlockValueV3.convertBV.rawData;
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00040C59 File Offset: 0x0003EE59
	public BlockValueV3(uint _rawData)
	{
		this.rawData = _rawData;
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000986 RID: 2438 RVA: 0x00040C62 File Offset: 0x0003EE62
	public Block Block
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Block.list[this.type];
		}
	}

	// Token: 0x06000987 RID: 2439 RVA: 0x00040C70 File Offset: 0x0003EE70
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetTypeMasked(uint _v)
	{
		return _v & 32767U;
	}

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x06000988 RID: 2440 RVA: 0x00040C79 File Offset: 0x0003EE79
	public bool isair
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.type == 0;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06000989 RID: 2441 RVA: 0x00040C84 File Offset: 0x0003EE84
	public bool isWater
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.type == 240 || this.type == 241 || this.type == 242;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x0600098A RID: 2442 RVA: 0x00040CAF File Offset: 0x0003EEAF
	// (set) Token: 0x0600098B RID: 2443 RVA: 0x00040CBD File Offset: 0x0003EEBD
	public int type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (int)(this.rawData & 32767U);
		}
		set
		{
			this.rawData = ((this.rawData & 4294934528U) | (uint)((long)value & 32767L));
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x0600098C RID: 2444 RVA: 0x00040CDC File Offset: 0x0003EEDC
	// (set) Token: 0x0600098D RID: 2445 RVA: 0x00040CEE File Offset: 0x0003EEEE
	public byte rotation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)((this.rawData & 1015808U) >> 15);
		}
		set
		{
			this.rawData = ((this.rawData & 4293951487U) | (uint)((uint)(value & 31) << 15));
		}
	}

	// Token: 0x1700009E RID: 158
	// (get) Token: 0x0600098E RID: 2446 RVA: 0x00040D0A File Offset: 0x0003EF0A
	// (set) Token: 0x0600098F RID: 2447 RVA: 0x00040D1C File Offset: 0x0003EF1C
	public byte meta
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)((this.rawData & 15728640U) >> 20);
		}
		set
		{
			this.rawData = ((this.rawData & 4279238655U) | (uint)((uint)(value & 15) << 20));
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x06000990 RID: 2448 RVA: 0x00040D38 File Offset: 0x0003EF38
	// (set) Token: 0x06000991 RID: 2449 RVA: 0x00040D4A File Offset: 0x0003EF4A
	public byte meta2
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)((this.rawData & 251658240U) >> 24);
		}
		set
		{
			this.rawData = ((this.rawData & 4043309055U) | (uint)((uint)(value & 15) << 24));
		}
	}

	// Token: 0x170000A0 RID: 160
	// (get) Token: 0x06000992 RID: 2450 RVA: 0x00040D66 File Offset: 0x0003EF66
	// (set) Token: 0x06000993 RID: 2451 RVA: 0x00040D78 File Offset: 0x0003EF78
	public byte meta3
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return (byte)((this.rawData & 805306368U) >> 28);
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.rawData = ((this.rawData & 3489660927U) | (uint)((uint)(value & 3) << 28));
		}
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000994 RID: 2452 RVA: 0x00040D93 File Offset: 0x0003EF93
	// (set) Token: 0x06000995 RID: 2453 RVA: 0x00040DA5 File Offset: 0x0003EFA5
	public byte meta2and1
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)((int)this.meta2 << 4 | (int)this.meta);
		}
		set
		{
			this.meta2 = (byte)(value >> 4 & 15);
			this.meta = (value & 15);
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000996 RID: 2454 RVA: 0x00040DBF File Offset: 0x0003EFBF
	// (set) Token: 0x06000997 RID: 2455 RVA: 0x00040DD1 File Offset: 0x0003EFD1
	public byte rotationAndMeta3
	{
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)((int)this.rotation << 2 | (int)this.meta3);
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.rotation = (byte)((long)(value >> 2) & 31L);
			this.meta3 = value;
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000998 RID: 2456 RVA: 0x00040DE9 File Offset: 0x0003EFE9
	// (set) Token: 0x06000999 RID: 2457 RVA: 0x00040DFA File Offset: 0x0003EFFA
	public bool hasdecal
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.rawData & 2147483648U) > 0U;
		}
		set
		{
			this.rawData = ((this.rawData & 2147483647U) | (value ? 2147483648U : 0U));
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x0600099A RID: 2458 RVA: 0x00040E1A File Offset: 0x0003F01A
	public BlockFaceFlag rotatedWaterFlowMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return BlockFaceFlags.RotateFlags(this.Block.WaterFlowMask, this.rotation);
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x0600099B RID: 2459 RVA: 0x00040D0A File Offset: 0x0003EF0A
	// (set) Token: 0x0600099C RID: 2460 RVA: 0x00040E32 File Offset: 0x0003F032
	public BlockFace decalface
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (BlockFace)((this.rawData & 15728640U) >> 20);
		}
		set
		{
			this.rawData = ((this.rawData & 4279238655U) | (uint)((uint)value << 20));
		}
	}

	// Token: 0x170000A6 RID: 166
	// (get) Token: 0x0600099D RID: 2461 RVA: 0x00040D38 File Offset: 0x0003EF38
	// (set) Token: 0x0600099E RID: 2462 RVA: 0x00040E4B File Offset: 0x0003F04B
	public byte decaltex
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)((this.rawData & 251658240U) >> 24);
		}
		set
		{
			this.rawData = ((this.rawData & 4043309055U) | (uint)((uint)value << 24));
		}
	}

	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x0600099F RID: 2463 RVA: 0x00040E64 File Offset: 0x0003F064
	// (set) Token: 0x060009A0 RID: 2464 RVA: 0x00040E75 File Offset: 0x0003F075
	public bool ischild
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.rawData & 1073741824U) > 0U;
		}
		set
		{
			this.rawData = ((this.rawData & 3221225471U) | (value ? 1073741824U : 0U));
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060009A1 RID: 2465 RVA: 0x00040E98 File Offset: 0x0003F098
	// (set) Token: 0x060009A2 RID: 2466 RVA: 0x00040EC8 File Offset: 0x0003F0C8
	public int parentx
	{
		get
		{
			int num = (int)((this.rawData & 251658240U) >> 24);
			return ((num & 8) != 0) ? (-(num & 7)) : (num & 7);
		}
		set
		{
			int num = (value < 0) ? (8 | (-value & 7)) : (value & 7);
			this.rawData = ((this.rawData & 4043309055U) | (uint)((uint)num << 24));
		}
	}

	// Token: 0x170000A9 RID: 169
	// (get) Token: 0x060009A3 RID: 2467 RVA: 0x00040EFC File Offset: 0x0003F0FC
	// (set) Token: 0x060009A4 RID: 2468 RVA: 0x00040F28 File Offset: 0x0003F128
	public int parenty
	{
		get
		{
			int rotationAndMeta = (int)this.rotationAndMeta3;
			return ((rotationAndMeta & 32) != 0) ? (-(rotationAndMeta & 31)) : (rotationAndMeta & 31);
		}
		set
		{
			int num = (value < 0) ? (32 | (-value & 31)) : (value & 31);
			this.rotationAndMeta3 = (byte)num;
		}
	}

	// Token: 0x170000AA RID: 170
	// (get) Token: 0x060009A5 RID: 2469 RVA: 0x00040F50 File Offset: 0x0003F150
	// (set) Token: 0x060009A6 RID: 2470 RVA: 0x00040F80 File Offset: 0x0003F180
	public int parentz
	{
		get
		{
			int num = (int)((this.rawData & 15728640U) >> 20);
			return ((num & 8) != 0) ? (-(num & 7)) : (num & 7);
		}
		set
		{
			int num = (value < 0) ? (8 | (-value & 7)) : (value & 7);
			this.rawData = ((this.rawData & 4279238655U) | (uint)((uint)num << 20));
		}
	}

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x060009A7 RID: 2471 RVA: 0x00040FB4 File Offset: 0x0003F1B4
	// (set) Token: 0x060009A8 RID: 2472 RVA: 0x00040FCD File Offset: 0x0003F1CD
	public Vector3i parent
	{
		get
		{
			return new Vector3i(this.parentx, this.parenty, this.parentz);
		}
		set
		{
			this.parentx = value.x;
			this.parenty = value.y;
			this.parentz = value.z;
		}
	}

	// Token: 0x040008F9 RID: 2297
	public const uint TypeMask = 32767U;

	// Token: 0x040008FA RID: 2298
	public const uint RotationMax = 31U;

	// Token: 0x040008FB RID: 2299
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint RotationMask = 1015808U;

	// Token: 0x040008FC RID: 2300
	[PublicizedFrom(EAccessModifier.Private)]
	public const int RotationShift = 15;

	// Token: 0x040008FD RID: 2301
	public const uint MetadataMax = 15U;

	// Token: 0x040008FE RID: 2302
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint MetadataMask = 15728640U;

	// Token: 0x040008FF RID: 2303
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MetadataShift = 20;

	// Token: 0x04000900 RID: 2304
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint Metadata2Mask = 251658240U;

	// Token: 0x04000901 RID: 2305
	[PublicizedFrom(EAccessModifier.Private)]
	public const int Metadata2Shift = 24;

	// Token: 0x04000902 RID: 2306
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint Metadata3Max = 3U;

	// Token: 0x04000903 RID: 2307
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint Metadata3Mask = 805306368U;

	// Token: 0x04000904 RID: 2308
	[PublicizedFrom(EAccessModifier.Private)]
	public const int Metadata3Shift = 28;

	// Token: 0x04000905 RID: 2309
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint ChildMask = 1073741824U;

	// Token: 0x04000906 RID: 2310
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ChildShift = 30;

	// Token: 0x04000907 RID: 2311
	[PublicizedFrom(EAccessModifier.Private)]
	public const uint HasDecalMask = 2147483648U;

	// Token: 0x04000908 RID: 2312
	[PublicizedFrom(EAccessModifier.Private)]
	public const int HasDecalShift = 31;

	// Token: 0x04000909 RID: 2313
	public uint rawData;

	// Token: 0x0400090A RID: 2314
	[PublicizedFrom(EAccessModifier.Private)]
	public static BlockValueV3 convertBV3;

	// Token: 0x0400090B RID: 2315
	[PublicizedFrom(EAccessModifier.Private)]
	public static BlockValue convertBV;
}

using System;
using System.Runtime.CompilerServices;

// Token: 0x02000151 RID: 337
[Serializable]
public struct BlockValue : IEquatable<BlockValue>
{
	// Token: 0x06000957 RID: 2391 RVA: 0x00040753 File Offset: 0x0003E953
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BlockValue(uint _rawData)
	{
		this.rawData = _rawData;
		this.damage = 0;
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x00040763 File Offset: 0x0003E963
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public BlockValue(uint _rawData, int _damage)
	{
		this.rawData = _rawData;
		this.damage = _damage;
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x00040773 File Offset: 0x0003E973
	public BlockValue set(int _type, byte _meta, byte _damage, byte _rotation)
	{
		this.type = _type;
		this.meta = _meta;
		this.damage = (int)_damage;
		this.rotation = _rotation;
		return this;
	}

	// Token: 0x17000086 RID: 134
	// (get) Token: 0x0600095A RID: 2394 RVA: 0x00040798 File Offset: 0x0003E998
	public Block Block
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Block.list[this.type];
		}
	}

	// Token: 0x0600095B RID: 2395 RVA: 0x000407A6 File Offset: 0x0003E9A6
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint GetTypeMasked(uint _v)
	{
		return _v & 65535U;
	}

	// Token: 0x17000087 RID: 135
	// (get) Token: 0x0600095C RID: 2396 RVA: 0x000407AF File Offset: 0x0003E9AF
	public bool isair
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.type == 0;
		}
	}

	// Token: 0x17000088 RID: 136
	// (get) Token: 0x0600095D RID: 2397 RVA: 0x000407BA File Offset: 0x0003E9BA
	public bool isWater
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.type == 240 || this.type == 241 || this.type == 242;
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x0600095E RID: 2398 RVA: 0x000407E5 File Offset: 0x0003E9E5
	// (set) Token: 0x0600095F RID: 2399 RVA: 0x000407F3 File Offset: 0x0003E9F3
	public int type
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (int)(this.rawData & 65535U);
		}
		set
		{
			this.rawData = ((this.rawData & 4294901760U) | (uint)(value & 65535));
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000960 RID: 2400 RVA: 0x0004080F File Offset: 0x0003EA0F
	// (set) Token: 0x06000961 RID: 2401 RVA: 0x0004081E File Offset: 0x0003EA1E
	public byte rotation
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)(this.rawData >> 16 & 31U);
		}
		set
		{
			this.rawData = ((this.rawData & 4292935679U) | (uint)((uint)(value & 31) << 16));
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x06000962 RID: 2402 RVA: 0x0004083A File Offset: 0x0003EA3A
	// (set) Token: 0x06000963 RID: 2403 RVA: 0x00040849 File Offset: 0x0003EA49
	public byte meta
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)(this.rawData >> 22 & 15U);
		}
		set
		{
			this.rawData = ((this.rawData & 4232052735U) | (uint)((uint)(value & 15) << 22));
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000964 RID: 2404 RVA: 0x00040865 File Offset: 0x0003EA65
	// (set) Token: 0x06000965 RID: 2405 RVA: 0x00040874 File Offset: 0x0003EA74
	public byte meta2
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)(this.rawData >> 26 & 15U);
		}
		set
		{
			this.rawData = ((this.rawData & 3288334335U) | (uint)((uint)(value & 15) << 26));
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000966 RID: 2406 RVA: 0x00040890 File Offset: 0x0003EA90
	// (set) Token: 0x06000967 RID: 2407 RVA: 0x000408A2 File Offset: 0x0003EAA2
	public byte meta2and1
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)(this.rawData >> 22 & 255U);
		}
		set
		{
			this.rawData = ((this.rawData & 3225419775U) | (uint)((uint)value << 22));
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000968 RID: 2408 RVA: 0x000408BB File Offset: 0x0003EABB
	public byte meta3
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return (byte)(this.rawData >> 21 & 1U);
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000969 RID: 2409 RVA: 0x000408C9 File Offset: 0x0003EAC9
	// (set) Token: 0x0600096A RID: 2410 RVA: 0x000408D8 File Offset: 0x0003EAD8
	public byte rotationAndMeta3
	{
		[PublicizedFrom(EAccessModifier.Private)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)(this.rawData >> 16 & 63U);
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.rawData = ((this.rawData & 4290838527U) | (uint)((uint)(value & 63) << 16));
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x0600096B RID: 2411 RVA: 0x000408F4 File Offset: 0x0003EAF4
	// (set) Token: 0x0600096C RID: 2412 RVA: 0x00040905 File Offset: 0x0003EB05
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

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x0600096D RID: 2413 RVA: 0x00040925 File Offset: 0x0003EB25
	public BlockFaceFlag rotatedWaterFlowMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return BlockFaceFlags.RotateFlags(this.Block.WaterFlowMask, this.rotation);
		}
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x0600096E RID: 2414 RVA: 0x0004083A File Offset: 0x0003EA3A
	// (set) Token: 0x0600096F RID: 2415 RVA: 0x00040849 File Offset: 0x0003EA49
	public BlockFace decalface
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (BlockFace)(this.rawData >> 22 & 15U);
		}
		set
		{
			this.rawData = ((this.rawData & 4232052735U) | (uint)((uint)(value & (BlockFace)15) << 22));
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000970 RID: 2416 RVA: 0x00040865 File Offset: 0x0003EA65
	// (set) Token: 0x06000971 RID: 2417 RVA: 0x00040874 File Offset: 0x0003EA74
	public byte decaltex
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (byte)(this.rawData >> 26 & 15U);
		}
		set
		{
			this.rawData = ((this.rawData & 3288334335U) | (uint)((uint)(value & 15) << 26));
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000972 RID: 2418 RVA: 0x0004093D File Offset: 0x0003EB3D
	// (set) Token: 0x06000973 RID: 2419 RVA: 0x0004094E File Offset: 0x0003EB4E
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

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000974 RID: 2420 RVA: 0x0004096E File Offset: 0x0003EB6E
	// (set) Token: 0x06000975 RID: 2421 RVA: 0x00040978 File Offset: 0x0003EB78
	public int parentx
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (int)(this.meta - 8);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			this.meta = (byte)(value + 8);
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000976 RID: 2422 RVA: 0x00040984 File Offset: 0x0003EB84
	// (set) Token: 0x06000977 RID: 2423 RVA: 0x0004098F File Offset: 0x0003EB8F
	public int parenty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (int)(this.rotationAndMeta3 - 32);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			this.rotationAndMeta3 = (byte)(value + 32);
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000978 RID: 2424 RVA: 0x0004099C File Offset: 0x0003EB9C
	// (set) Token: 0x06000979 RID: 2425 RVA: 0x000409A6 File Offset: 0x0003EBA6
	public int parentz
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (int)(this.meta2 - 8);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			this.meta2 = (byte)(value + 8);
		}
	}

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x0600097A RID: 2426 RVA: 0x000409B2 File Offset: 0x0003EBB2
	// (set) Token: 0x0600097B RID: 2427 RVA: 0x000409CB File Offset: 0x0003EBCB
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

	// Token: 0x0600097C RID: 2428 RVA: 0x000409F1 File Offset: 0x0003EBF1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetForceToOtherBlock(BlockValue _other)
	{
		return Utils.FastMin(this.Block.blockMaterial.StabilityGlue, _other.Block.blockMaterial.StabilityGlue);
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00040A19 File Offset: 0x0003EC19
	public int ToItemType()
	{
		return this.type;
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00040A21 File Offset: 0x0003EC21
	public ItemValue ToItemValue()
	{
		return new ItemValue
		{
			type = this.type
		};
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00040A19 File Offset: 0x0003EC19
	public override int GetHashCode()
	{
		return this.type;
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00040A34 File Offset: 0x0003EC34
	public override bool Equals(object _other)
	{
		return _other is BlockValue && ((BlockValue)_other).type == this.type;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x00040A61 File Offset: 0x0003EC61
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(BlockValue _other)
	{
		return _other.type == this.type;
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x00040A74 File Offset: 0x0003EC74
	public bool EqualsExceptRotation(BlockValue _other)
	{
		uint num = this.rawData & 4292935679U;
		uint num2 = _other.rawData & 4292935679U;
		return num == num2;
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x00040AA0 File Offset: 0x0003ECA0
	public override string ToString()
	{
		if (!this.ischild)
		{
			string format = "id={0} r={1} d={2} m={3} m2={4} m3={5} name={6}";
			object[] array = new object[7];
			array[0] = this.type;
			array[1] = this.rotation;
			array[2] = this.damage;
			array[3] = this.meta;
			array[4] = this.meta2;
			array[5] = this.meta3;
			int num = 6;
			Block block = this.Block;
			array[num] = (((block != null) ? block.GetBlockName() : null) ?? "-null-");
			return string.Format(format, array);
		}
		string format2 = "id={0} px={1} py={2} pz={3} name={4}";
		object[] array2 = new object[5];
		array2[0] = this.type;
		array2[1] = this.parentx;
		array2[2] = this.parenty;
		array2[3] = this.parentz;
		int num2 = 4;
		Block block2 = this.Block;
		array2[num2] = (((block2 != null) ? block2.GetBlockName() : null) ?? "-null-");
		return string.Format(format2, array2);
	}

	// Token: 0x040008E3 RID: 2275
	public const uint TypeMask = 65535U;

	// Token: 0x040008E4 RID: 2276
	public const uint RotationMax = 31U;

	// Token: 0x040008E5 RID: 2277
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint RotationMask = 2031616U;

	// Token: 0x040008E6 RID: 2278
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int RotationShift = 16;

	// Token: 0x040008E7 RID: 2279
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint Metadata3Max = 1U;

	// Token: 0x040008E8 RID: 2280
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint Metadata3Mask = 2097152U;

	// Token: 0x040008E9 RID: 2281
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int Metadata3Shift = 21;

	// Token: 0x040008EA RID: 2282
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint RotationMeta3Max = 63U;

	// Token: 0x040008EB RID: 2283
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint RotationMeta3Mask = 4128768U;

	// Token: 0x040008EC RID: 2284
	public const uint MetadataMax = 15U;

	// Token: 0x040008ED RID: 2285
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint Metadata1Mask = 62914560U;

	// Token: 0x040008EE RID: 2286
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int Metadata1Shift = 22;

	// Token: 0x040008EF RID: 2287
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint Metadata2Mask = 1006632960U;

	// Token: 0x040008F0 RID: 2288
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int Metadata2Shift = 26;

	// Token: 0x040008F1 RID: 2289
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint Metadata12Max = 255U;

	// Token: 0x040008F2 RID: 2290
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint ChildMask = 1073741824U;

	// Token: 0x040008F3 RID: 2291
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int ChildShift = 30;

	// Token: 0x040008F4 RID: 2292
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const uint HasDecalMask = 2147483648U;

	// Token: 0x040008F5 RID: 2293
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int HasDecalShift = 31;

	// Token: 0x040008F6 RID: 2294
	public static BlockValue Air;

	// Token: 0x040008F7 RID: 2295
	public uint rawData;

	// Token: 0x040008F8 RID: 2296
	public int damage;
}

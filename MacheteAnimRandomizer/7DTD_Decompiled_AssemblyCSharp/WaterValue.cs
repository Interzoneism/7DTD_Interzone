using System;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x02000B7F RID: 2943
public struct WaterValue
{
	// Token: 0x06005B36 RID: 23350 RVA: 0x0024889C File Offset: 0x00246A9C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasMass()
	{
		return this.mass > 195;
	}

	// Token: 0x06005B37 RID: 23351 RVA: 0x002488AB File Offset: 0x00246AAB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetMass()
	{
		return (int)this.mass;
	}

	// Token: 0x06005B38 RID: 23352 RVA: 0x002488B3 File Offset: 0x00246AB3
	public float GetMassPercent()
	{
		if (this.mass <= 195)
		{
			return 0f;
		}
		if (this.mass >= 15600)
		{
			return 1f;
		}
		return (float)(this.mass - 195) / 15405f;
	}

	// Token: 0x06005B39 RID: 23353 RVA: 0x002488EE File Offset: 0x00246AEE
	public void SetMass(int value)
	{
		this.mass = (ushort)Utils.FastClamp(value, 0, 65535);
	}

	// Token: 0x06005B3A RID: 23354 RVA: 0x00248903 File Offset: 0x00246B03
	public override string ToString()
	{
		return string.Format("Raw Mass: {0:d}", this.mass);
	}

	// Token: 0x1700094F RID: 2383
	// (get) Token: 0x06005B3B RID: 23355 RVA: 0x0024891A File Offset: 0x00246B1A
	public long RawData
	{
		get
		{
			return (long)((ulong)this.mass);
		}
	}

	// Token: 0x06005B3C RID: 23356 RVA: 0x00248924 File Offset: 0x00246B24
	public static WaterValue FromRawData(long rawData)
	{
		return new WaterValue
		{
			mass = (ushort)rawData
		};
	}

	// Token: 0x06005B3D RID: 23357 RVA: 0x00248943 File Offset: 0x00246B43
	public static WaterValue FromBlockType(int type)
	{
		if (type == 240 || type == 241 || type == 242)
		{
			return new WaterValue(19500);
		}
		return WaterValue.Empty;
	}

	// Token: 0x06005B3E RID: 23358 RVA: 0x00248970 File Offset: 0x00246B70
	public static WaterValue FromStream(BinaryReader _reader)
	{
		WaterValue result = default(WaterValue);
		result.Read(_reader);
		return result;
	}

	// Token: 0x06005B3F RID: 23359 RVA: 0x0024898E File Offset: 0x00246B8E
	public WaterValue(BlockValue _bv)
	{
		this.mass = (_bv.isWater ? 19500 : 0);
	}

	// Token: 0x06005B40 RID: 23360 RVA: 0x002488EE File Offset: 0x00246AEE
	public WaterValue(int mass)
	{
		this.mass = (ushort)Utils.FastClamp(mass, 0, 65535);
	}

	// Token: 0x06005B41 RID: 23361 RVA: 0x002489A8 File Offset: 0x00246BA8
	public void Write(BinaryWriter writer)
	{
		writer.Write(this.mass);
	}

	// Token: 0x06005B42 RID: 23362 RVA: 0x002489B6 File Offset: 0x00246BB6
	public void Read(BinaryReader reader)
	{
		this.mass = reader.ReadUInt16();
	}

	// Token: 0x06005B43 RID: 23363 RVA: 0x000282C0 File Offset: 0x000264C0
	public static int SerializedLength()
	{
		return 2;
	}

	// Token: 0x040045C1 RID: 17857
	public const float cTopPerCap = 0.6f;

	// Token: 0x040045C2 RID: 17858
	public const int MAX_MASS_VALUE = 65535;

	// Token: 0x040045C3 RID: 17859
	public static readonly WaterValue Empty = new WaterValue(0);

	// Token: 0x040045C4 RID: 17860
	public static readonly WaterValue Full = new WaterValue(19500);

	// Token: 0x040045C5 RID: 17861
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort mass;
}

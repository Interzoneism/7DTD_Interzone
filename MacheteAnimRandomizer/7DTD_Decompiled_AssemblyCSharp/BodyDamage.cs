using System;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x0200040C RID: 1036
public struct BodyDamage
{
	// Token: 0x17000368 RID: 872
	// (get) Token: 0x06001EF7 RID: 7927 RVA: 0x000C09C0 File Offset: 0x000BEBC0
	public bool HasLeftLeg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 96U) == 0U;
		}
	}

	// Token: 0x17000369 RID: 873
	// (get) Token: 0x06001EF8 RID: 7928 RVA: 0x000C09CE File Offset: 0x000BEBCE
	public bool HasRightLeg
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 384U) == 0U;
		}
	}

	// Token: 0x1700036A RID: 874
	// (get) Token: 0x06001EF9 RID: 7929 RVA: 0x000C09DF File Offset: 0x000BEBDF
	public bool HasLimbs
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 330U) != 330U;
		}
	}

	// Token: 0x1700036B RID: 875
	// (get) Token: 0x06001EFA RID: 7930 RVA: 0x000C09F7 File Offset: 0x000BEBF7
	public bool IsAnyLegMissing
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 480U) > 0U;
		}
	}

	// Token: 0x1700036C RID: 876
	// (get) Token: 0x06001EFB RID: 7931 RVA: 0x000C0A08 File Offset: 0x000BEC08
	public bool IsAnyArmOrLegMissing
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (this.Flags & 510U) > 0U;
		}
	}

	// Token: 0x1700036D RID: 877
	// (get) Token: 0x06001EFC RID: 7932 RVA: 0x000C0A19 File Offset: 0x000BEC19
	public bool IsCrippled
	{
		get
		{
			return (this.Flags & 12288U) > 0U;
		}
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x000C0A2C File Offset: 0x000BEC2C
	public static BodyDamage Read(BinaryReader _br, int _version)
	{
		if (_version > 21)
		{
			return BodyDamage.ReadData(_br, _br.ReadInt32());
		}
		if (_version > 20)
		{
			return BodyDamage.ReadData(_br, 0);
		}
		if (_version > 19)
		{
			_br.ReadInt32();
		}
		return default(BodyDamage);
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x000C0A70 File Offset: 0x000BEC70
	[PublicizedFrom(EAccessModifier.Private)]
	public static BodyDamage ReadData(BinaryReader br, int version)
	{
		BodyDamage result = default(BodyDamage);
		if (version >= 4)
		{
			result.damageType = (EnumDamageTypes)br.ReadInt32();
		}
		if (version >= 3)
		{
			result.Flags = br.ReadUInt32();
		}
		else
		{
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			br.ReadInt16();
			if (br.ReadBoolean())
			{
				result.Flags |= 2U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 8U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 1U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 128U;
			}
			if (br.ReadBoolean())
			{
				result.Flags |= 8192U;
			}
			if (version >= 1)
			{
				br.ReadInt16();
				br.ReadInt16();
				br.ReadInt16();
				br.ReadInt16();
				if (br.ReadBoolean())
				{
					result.Flags |= 4U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 16U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 64U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 256U;
				}
				if (version >= 2 && br.ReadBoolean())
				{
					result.Flags |= 32U;
				}
				if (br.ReadBoolean())
				{
					result.Flags |= 4096U;
				}
			}
		}
		result.ShouldBeCrawler = (!result.HasLeftLeg || !result.HasRightLeg);
		return result;
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x000C0C0A File Offset: 0x000BEE0A
	public void Write(BinaryWriter bw)
	{
		bw.Write(BodyDamage.cBinaryVersion);
		bw.Write((int)this.damageType);
		bw.Write(this.Flags);
	}

	// Token: 0x0400159E RID: 5534
	[PublicizedFrom(EAccessModifier.Private)]
	public static int cBinaryVersion = 4;

	// Token: 0x0400159F RID: 5535
	public int StunKnee;

	// Token: 0x040015A0 RID: 5536
	public int StunProne;

	// Token: 0x040015A1 RID: 5537
	public float StunDuration;

	// Token: 0x040015A2 RID: 5538
	public EnumEntityStunType CurrentStun;

	// Token: 0x040015A3 RID: 5539
	public bool ShouldBeCrawler;

	// Token: 0x040015A4 RID: 5540
	public const uint cNoHead = 1U;

	// Token: 0x040015A5 RID: 5541
	public const uint cNoArmLUpper = 2U;

	// Token: 0x040015A6 RID: 5542
	public const uint cNoArmLLower = 4U;

	// Token: 0x040015A7 RID: 5543
	public const uint cNoArmRUpper = 8U;

	// Token: 0x040015A8 RID: 5544
	public const uint cNoArmRLower = 16U;

	// Token: 0x040015A9 RID: 5545
	public const uint cNoArm = 30U;

	// Token: 0x040015AA RID: 5546
	public const uint cNoLegLUpper = 32U;

	// Token: 0x040015AB RID: 5547
	public const uint cNoLegLLower = 64U;

	// Token: 0x040015AC RID: 5548
	public const uint cNoLegRUpper = 128U;

	// Token: 0x040015AD RID: 5549
	public const uint cNoLegRLower = 256U;

	// Token: 0x040015AE RID: 5550
	public const uint cNoLeg = 480U;

	// Token: 0x040015AF RID: 5551
	public const uint cCrippledLegL = 4096U;

	// Token: 0x040015B0 RID: 5552
	public const uint cCrippledLegR = 8192U;

	// Token: 0x040015B1 RID: 5553
	public uint Flags;

	// Token: 0x040015B2 RID: 5554
	public EnumDamageTypes damageType;

	// Token: 0x040015B3 RID: 5555
	public EnumBodyPartHit bodyPartHit;
}

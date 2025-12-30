using System;
using System.IO;

// Token: 0x02000950 RID: 2384
public class PlayerProfile
{
	// Token: 0x17000786 RID: 1926
	// (get) Token: 0x060047CB RID: 18379 RVA: 0x001C2217 File Offset: 0x001C0417
	// (set) Token: 0x060047CC RID: 18380 RVA: 0x001C221F File Offset: 0x001C041F
	public bool IsMale
	{
		get
		{
			return this.isMale;
		}
		set
		{
			this.isMale = value;
		}
	}

	// Token: 0x17000787 RID: 1927
	// (get) Token: 0x060047CD RID: 18381 RVA: 0x001C2228 File Offset: 0x001C0428
	// (set) Token: 0x060047CE RID: 18382 RVA: 0x001C2230 File Offset: 0x001C0430
	public string RaceName
	{
		get
		{
			return this.raceName;
		}
		set
		{
			this.raceName = value;
		}
	}

	// Token: 0x17000788 RID: 1928
	// (get) Token: 0x060047CF RID: 18383 RVA: 0x001C2239 File Offset: 0x001C0439
	// (set) Token: 0x060047D0 RID: 18384 RVA: 0x001C2241 File Offset: 0x001C0441
	public string EyeColor
	{
		get
		{
			return this.eyeColor;
		}
		set
		{
			this.eyeColor = value;
		}
	}

	// Token: 0x17000789 RID: 1929
	// (get) Token: 0x060047D1 RID: 18385 RVA: 0x001C224A File Offset: 0x001C044A
	// (set) Token: 0x060047D2 RID: 18386 RVA: 0x001C2252 File Offset: 0x001C0452
	public string HairName
	{
		get
		{
			return this.hairName;
		}
		set
		{
			this.hairName = value;
		}
	}

	// Token: 0x1700078A RID: 1930
	// (get) Token: 0x060047D3 RID: 18387 RVA: 0x001C225B File Offset: 0x001C045B
	// (set) Token: 0x060047D4 RID: 18388 RVA: 0x001C2263 File Offset: 0x001C0463
	public string HairColor
	{
		get
		{
			return this.hairColor;
		}
		set
		{
			this.hairColor = value;
		}
	}

	// Token: 0x1700078B RID: 1931
	// (get) Token: 0x060047D5 RID: 18389 RVA: 0x001C226C File Offset: 0x001C046C
	// (set) Token: 0x060047D6 RID: 18390 RVA: 0x001C2274 File Offset: 0x001C0474
	public string MustacheName
	{
		get
		{
			return this.mustacheName;
		}
		set
		{
			this.mustacheName = value;
		}
	}

	// Token: 0x1700078C RID: 1932
	// (get) Token: 0x060047D7 RID: 18391 RVA: 0x001C227D File Offset: 0x001C047D
	// (set) Token: 0x060047D8 RID: 18392 RVA: 0x001C2285 File Offset: 0x001C0485
	public string ChopsName
	{
		get
		{
			return this.chopsName;
		}
		set
		{
			this.chopsName = value;
		}
	}

	// Token: 0x1700078D RID: 1933
	// (get) Token: 0x060047D9 RID: 18393 RVA: 0x001C228E File Offset: 0x001C048E
	// (set) Token: 0x060047DA RID: 18394 RVA: 0x001C2296 File Offset: 0x001C0496
	public string BeardName
	{
		get
		{
			return this.beardName;
		}
		set
		{
			this.beardName = value;
		}
	}

	// Token: 0x1700078E RID: 1934
	// (get) Token: 0x060047DB RID: 18395 RVA: 0x001C229F File Offset: 0x001C049F
	// (set) Token: 0x060047DC RID: 18396 RVA: 0x001C22A7 File Offset: 0x001C04A7
	public int VariantNumber
	{
		get
		{
			return this.variantNumber;
		}
		set
		{
			this.variantNumber = value;
		}
	}

	// Token: 0x1700078F RID: 1935
	// (get) Token: 0x060047DD RID: 18397 RVA: 0x001C22B0 File Offset: 0x001C04B0
	// (set) Token: 0x060047DE RID: 18398 RVA: 0x001C22FD File Offset: 0x001C04FD
	public string ProfileArchetype
	{
		get
		{
			if (this.archetype == null || this.archetype == string.Empty)
			{
				if (this.isMale)
				{
					this.archetype = "BaseMale";
				}
				else
				{
					this.archetype = "BaseFemale";
				}
			}
			return this.archetype;
		}
		set
		{
			this.archetype = value;
		}
	}

	// Token: 0x060047DF RID: 18399 RVA: 0x001C2308 File Offset: 0x001C0508
	public Archetype CreateTempArchetype()
	{
		if (this.archetype != "BaseMale" && this.archetype != "BaseFemale")
		{
			return Archetype.GetArchetype(this.archetype);
		}
		return new Archetype(this.archetype, this.isMale, true)
		{
			Race = this.raceName,
			Variant = this.variantNumber,
			Hair = this.hairName,
			HairColor = this.hairColor,
			MustacheName = this.mustacheName,
			ChopsName = this.chopsName,
			BeardName = this.beardName,
			EyeColorName = this.EyeColor
		};
	}

	// Token: 0x17000790 RID: 1936
	// (get) Token: 0x060047E0 RID: 18400 RVA: 0x001C23B7 File Offset: 0x001C05B7
	public string EntityClassName
	{
		get
		{
			if (!this.isMale)
			{
				return "playerFemale";
			}
			return "playerMale";
		}
	}

	// Token: 0x060047E1 RID: 18401 RVA: 0x001C23CC File Offset: 0x001C05CC
	public PlayerProfile()
	{
		this.raceName = "white";
		this.isMale = true;
		this.variantNumber = 1;
	}

	// Token: 0x060047E2 RID: 18402 RVA: 0x001C2460 File Offset: 0x001C0660
	public PlayerProfile Clone()
	{
		return new PlayerProfile
		{
			raceName = this.raceName,
			isMale = this.isMale,
			variantNumber = this.variantNumber,
			archetype = this.archetype,
			hairName = this.hairName,
			hairColor = this.hairColor,
			mustacheName = this.mustacheName,
			chopsName = this.chopsName,
			beardName = this.beardName,
			EyeColor = this.EyeColor
		};
	}

	// Token: 0x060047E3 RID: 18403 RVA: 0x001C24EC File Offset: 0x001C06EC
	public void Write(BinaryWriter writer)
	{
		writer.Write(5);
		writer.Write(this.archetype);
		writer.Write(this.isMale);
		writer.Write(this.raceName);
		writer.Write((byte)this.variantNumber);
		writer.Write(this.hairName ?? "");
		writer.Write(this.hairColor ?? "");
		writer.Write(this.mustacheName ?? "");
		writer.Write(this.chopsName ?? "");
		writer.Write(this.beardName ?? "");
		writer.Write(this.eyeColor ?? "Blue01");
	}

	// Token: 0x060047E4 RID: 18404 RVA: 0x001C25B0 File Offset: 0x001C07B0
	public static PlayerProfile Read(BinaryReader reader)
	{
		PlayerProfile playerProfile = new PlayerProfile();
		int num = reader.ReadInt32();
		playerProfile.archetype = reader.ReadString();
		playerProfile.IsMale = reader.ReadBoolean();
		playerProfile.RaceName = reader.ReadString();
		playerProfile.VariantNumber = (int)reader.ReadByte();
		if (num > 1)
		{
			playerProfile.HairName = reader.ReadString();
		}
		if (num > 2)
		{
			playerProfile.HairColor = reader.ReadString();
		}
		if (num > 3)
		{
			playerProfile.MustacheName = reader.ReadString();
			playerProfile.ChopsName = reader.ReadString();
			playerProfile.BeardName = reader.ReadString();
		}
		if (num > 4)
		{
			playerProfile.EyeColor = reader.ReadString();
		}
		return playerProfile;
	}

	// Token: 0x060047E5 RID: 18405 RVA: 0x001C2653 File Offset: 0x001C0853
	public static PlayerProfile LoadLocalProfile()
	{
		return PlayerProfile.LoadProfile(ProfileSDF.CurrentProfileName());
	}

	// Token: 0x060047E6 RID: 18406 RVA: 0x001C2660 File Offset: 0x001C0860
	public static PlayerProfile LoadProfile(string _profileName)
	{
		return new PlayerProfile
		{
			IsMale = ProfileSDF.GetIsMale(_profileName),
			RaceName = ProfileSDF.GetRaceName(_profileName),
			VariantNumber = ProfileSDF.GetVariantNumber(_profileName),
			ProfileArchetype = ProfileSDF.GetArchetype(_profileName),
			HairName = ProfileSDF.GetHairName(_profileName),
			HairColor = ProfileSDF.GetHairColorName(_profileName),
			MustacheName = ProfileSDF.GetMustacheName(_profileName),
			ChopsName = ProfileSDF.GetChopsName(_profileName),
			BeardName = ProfileSDF.GetBeardName(_profileName),
			EyeColor = ProfileSDF.GetEyeColorName(_profileName)
		};
	}

	// Token: 0x0400370B RID: 14091
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isMale = true;

	// Token: 0x0400370C RID: 14092
	[PublicizedFrom(EAccessModifier.Private)]
	public string raceName = "";

	// Token: 0x0400370D RID: 14093
	[PublicizedFrom(EAccessModifier.Private)]
	public int variantNumber = 1;

	// Token: 0x0400370E RID: 14094
	[PublicizedFrom(EAccessModifier.Private)]
	public string archetype = "";

	// Token: 0x0400370F RID: 14095
	[PublicizedFrom(EAccessModifier.Private)]
	public string eyeColor = "";

	// Token: 0x04003710 RID: 14096
	[PublicizedFrom(EAccessModifier.Private)]
	public string hairName = "";

	// Token: 0x04003711 RID: 14097
	[PublicizedFrom(EAccessModifier.Private)]
	public string hairColor = "";

	// Token: 0x04003712 RID: 14098
	[PublicizedFrom(EAccessModifier.Private)]
	public string mustacheName = "";

	// Token: 0x04003713 RID: 14099
	[PublicizedFrom(EAccessModifier.Private)]
	public string chopsName = "";

	// Token: 0x04003714 RID: 14100
	[PublicizedFrom(EAccessModifier.Private)]
	public string beardName = "";

	// Token: 0x04003715 RID: 14101
	[PublicizedFrom(EAccessModifier.Private)]
	public const int version = 5;
}

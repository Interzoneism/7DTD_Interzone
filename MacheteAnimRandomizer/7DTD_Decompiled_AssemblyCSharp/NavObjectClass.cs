using System;
using System.Collections.Generic;

// Token: 0x020006A8 RID: 1704
public class NavObjectClass
{
	// Token: 0x0600325B RID: 12891 RVA: 0x00155C60 File Offset: 0x00153E60
	public static void Reset()
	{
		NavObjectClass.NavObjectClassList.Clear();
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x00155C6C File Offset: 0x00153E6C
	public NavObjectMapSettings GetMapSettings(bool isActive)
	{
		if (isActive)
		{
			return this.MapSettings;
		}
		return this.InactiveMapSettings;
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x00155C7E File Offset: 0x00153E7E
	public NavObjectCompassSettings GetCompassSettings(bool isActive)
	{
		if (isActive)
		{
			return this.CompassSettings;
		}
		return this.InactiveCompassSettings;
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x00155C90 File Offset: 0x00153E90
	public NavObjectScreenSettings GetOnScreenSettings(bool isActive)
	{
		if (isActive)
		{
			return this.OnScreenSettings;
		}
		return this.InactiveOnScreenSettings;
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x00155CA2 File Offset: 0x00153EA2
	public NavObjectClass(string name)
	{
		this.NavObjectClassName = name;
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x00155CD4 File Offset: 0x00153ED4
	public static NavObjectClass GetNavObjectClass(string className)
	{
		for (int i = 0; i < NavObjectClass.NavObjectClassList.Count; i++)
		{
			if (NavObjectClass.NavObjectClassList[i].NavObjectClassName == className)
			{
				return NavObjectClass.NavObjectClassList[i];
			}
		}
		return null;
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x00155D1C File Offset: 0x00153F1C
	public void Init()
	{
		if (this.Properties.Values.ContainsKey("requirement_type"))
		{
			if (!Enum.TryParse<NavObjectClass.RequirementTypes>(this.Properties.Values["requirement_type"], out this.RequirementType))
			{
				this.RequirementType = NavObjectClass.RequirementTypes.None;
			}
			if (this.RequirementType != NavObjectClass.RequirementTypes.None)
			{
				this.Properties.ParseString("requirement_name", ref this.RequirementName);
			}
		}
		this.Properties.ParseString("tag", ref this.Tag);
		this.Properties.ParseBool("use_override_icon", ref this.UseOverrideIcon);
	}

	// Token: 0x04002933 RID: 10547
	public static List<NavObjectClass> NavObjectClassList = new List<NavObjectClass>();

	// Token: 0x04002934 RID: 10548
	public DynamicProperties Properties = new DynamicProperties();

	// Token: 0x04002935 RID: 10549
	public string NavObjectClassName = "";

	// Token: 0x04002936 RID: 10550
	public NavObjectClass.RequirementTypes RequirementType;

	// Token: 0x04002937 RID: 10551
	public string RequirementName = "";

	// Token: 0x04002938 RID: 10552
	public bool UseOverrideIcon;

	// Token: 0x04002939 RID: 10553
	public string Tag;

	// Token: 0x0400293A RID: 10554
	public NavObjectMapSettings MapSettings;

	// Token: 0x0400293B RID: 10555
	public NavObjectCompassSettings CompassSettings;

	// Token: 0x0400293C RID: 10556
	public NavObjectScreenSettings OnScreenSettings;

	// Token: 0x0400293D RID: 10557
	public NavObjectMapSettings InactiveMapSettings;

	// Token: 0x0400293E RID: 10558
	public NavObjectCompassSettings InactiveCompassSettings;

	// Token: 0x0400293F RID: 10559
	public NavObjectScreenSettings InactiveOnScreenSettings;

	// Token: 0x020006A9 RID: 1705
	public enum RequirementTypes
	{
		// Token: 0x04002941 RID: 10561
		None,
		// Token: 0x04002942 RID: 10562
		CVar,
		// Token: 0x04002943 RID: 10563
		QuestBounds,
		// Token: 0x04002944 RID: 10564
		Tracking,
		// Token: 0x04002945 RID: 10565
		NoTag,
		// Token: 0x04002946 RID: 10566
		InParty,
		// Token: 0x04002947 RID: 10567
		IsAlly,
		// Token: 0x04002948 RID: 10568
		IsPlayer,
		// Token: 0x04002949 RID: 10569
		IsVehicleOwner,
		// Token: 0x0400294A RID: 10570
		IsOwner,
		// Token: 0x0400294B RID: 10571
		NoActiveQuests,
		// Token: 0x0400294C RID: 10572
		MinimumTreasureRadius,
		// Token: 0x0400294D RID: 10573
		IsTwitchSpawnedSelf,
		// Token: 0x0400294E RID: 10574
		IsTwitchSpawnedOther
	}
}

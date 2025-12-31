using System;
using UnityEngine.Scripting;

// Token: 0x02000562 RID: 1378
[Preserve]
public class ItemClassArmor : ItemClass
{
	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x06002CAA RID: 11434 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsEquipment
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002CAB RID: 11435 RVA: 0x0012A665 File Offset: 0x00128865
	public override bool KeepOnDeath()
	{
		return this.keepOnDeath;
	}

	// Token: 0x06002CAC RID: 11436 RVA: 0x0012A670 File Offset: 0x00128870
	public override void Init()
	{
		base.Init();
		string text = "";
		this.Properties.ParseString("ArmorGroup", ref text);
		this.ArmorGroup = text.Split(',', StringSplitOptions.None);
		if (this.Properties.Values.ContainsKey("EquipSlot"))
		{
			this.EquipSlot = EnumUtils.Parse<EquipmentSlots>(this.Properties.Values["EquipSlot"], false);
		}
		this.Properties.ParseBool("IsCosmetic", ref this.IsCosmetic);
		this.Properties.ParseBool("KeepOnDeath", ref this.keepOnDeath);
		this.Properties.ParseBool("AllowUnEquip", ref this.AllowUnEquip);
		this.Properties.ParseBool("AutoEquip", ref this.AutoEquip);
		this.Properties.ParseString("ReplaceByTag", ref this.ReplaceByTag);
	}

	// Token: 0x04002345 RID: 9029
	public const string PropEquipSlot = "EquipSlot";

	// Token: 0x04002346 RID: 9030
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropArmorGroup = "ArmorGroup";

	// Token: 0x04002347 RID: 9031
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropIsCosmetic = "IsCosmetic";

	// Token: 0x04002348 RID: 9032
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropKeepOnDeath = "KeepOnDeath";

	// Token: 0x04002349 RID: 9033
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropAllowUnEquip = "AllowUnEquip";

	// Token: 0x0400234A RID: 9034
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropAutoEquip = "AutoEquip";

	// Token: 0x0400234B RID: 9035
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PropReplaceByTag = "ReplaceByTag";

	// Token: 0x0400234C RID: 9036
	public EquipmentSlots EquipSlot = EquipmentSlots.Count;

	// Token: 0x0400234D RID: 9037
	public string[] ArmorGroup;

	// Token: 0x0400234E RID: 9038
	public bool IsCosmetic = true;

	// Token: 0x0400234F RID: 9039
	public int CosmeticID = -1;

	// Token: 0x04002350 RID: 9040
	[PublicizedFrom(EAccessModifier.Private)]
	public bool keepOnDeath;

	// Token: 0x04002351 RID: 9041
	public bool AllowUnEquip = true;

	// Token: 0x04002352 RID: 9042
	public bool AutoEquip;

	// Token: 0x04002353 RID: 9043
	public string ReplaceByTag;
}

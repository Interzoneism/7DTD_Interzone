using System;
using System.Collections.Generic;

// Token: 0x020002AA RID: 682
public abstract class BaseDialogRequirement
{
	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06001339 RID: 4921 RVA: 0x000767A8 File Offset: 0x000749A8
	// (set) Token: 0x0600133A RID: 4922 RVA: 0x000767B0 File Offset: 0x000749B0
	public string ID { get; set; }

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x0600133B RID: 4923 RVA: 0x000767B9 File Offset: 0x000749B9
	// (set) Token: 0x0600133C RID: 4924 RVA: 0x000767C1 File Offset: 0x000749C1
	public string Value { get; set; }

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x0600133D RID: 4925 RVA: 0x000767CA File Offset: 0x000749CA
	// (set) Token: 0x0600133E RID: 4926 RVA: 0x000767D2 File Offset: 0x000749D2
	public string Tag { get; set; }

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x0600133F RID: 4927 RVA: 0x000767DB File Offset: 0x000749DB
	// (set) Token: 0x06001340 RID: 4928 RVA: 0x000767E3 File Offset: 0x000749E3
	public Dialog Owner { get; set; }

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06001341 RID: 4929 RVA: 0x000767EC File Offset: 0x000749EC
	// (set) Token: 0x06001342 RID: 4930 RVA: 0x000767F4 File Offset: 0x000749F4
	public BaseDialogRequirement.RequirementVisibilityTypes RequirementVisibilityType { get; set; }

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06001343 RID: 4931 RVA: 0x000767FD File Offset: 0x000749FD
	// (set) Token: 0x06001344 RID: 4932 RVA: 0x00076805 File Offset: 0x00074A05
	public string Description { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06001345 RID: 4933 RVA: 0x0007680E File Offset: 0x00074A0E
	// (set) Token: 0x06001346 RID: 4934 RVA: 0x00076816 File Offset: 0x00074A16
	public string StatusText { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x06001347 RID: 4935 RVA: 0x00019766 File Offset: 0x00017966
	public virtual List<string> GetRequirementIDTypes()
	{
		return null;
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x06001348 RID: 4936 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual BaseDialogRequirement.RequirementTypes RequirementType
	{
		get
		{
			return BaseDialogRequirement.RequirementTypes.Buff;
		}
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x0002B133 File Offset: 0x00029333
	public virtual string GetRequiredDescription(EntityPlayer player)
	{
		return "";
	}

	// Token: 0x0600134A RID: 4938 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupRequirement()
	{
	}

	// Token: 0x0600134B RID: 4939 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool CheckRequirement(EntityPlayer player, EntityNPC talkingTo)
	{
		return false;
	}

	// Token: 0x0600134C RID: 4940 RVA: 0x00019766 File Offset: 0x00017966
	public virtual BaseDialogRequirement Clone()
	{
		return null;
	}

	// Token: 0x0600134D RID: 4941 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public BaseDialogRequirement()
	{
	}

	// Token: 0x020002AB RID: 683
	public enum RequirementTypes
	{
		// Token: 0x04000CB9 RID: 3257
		Buff,
		// Token: 0x04000CBA RID: 3258
		QuestStatus,
		// Token: 0x04000CBB RID: 3259
		QuestsAvailable,
		// Token: 0x04000CBC RID: 3260
		QuestTier,
		// Token: 0x04000CBD RID: 3261
		QuestTierHighest,
		// Token: 0x04000CBE RID: 3262
		QuestEditorTag,
		// Token: 0x04000CBF RID: 3263
		Skill,
		// Token: 0x04000CC0 RID: 3264
		Admin,
		// Token: 0x04000CC1 RID: 3265
		DroneState,
		// Token: 0x04000CC2 RID: 3266
		DroneStateExclude,
		// Token: 0x04000CC3 RID: 3267
		CVar
	}

	// Token: 0x020002AC RID: 684
	public enum RequirementVisibilityTypes
	{
		// Token: 0x04000CC5 RID: 3269
		AlternateText,
		// Token: 0x04000CC6 RID: 3270
		Hide
	}
}

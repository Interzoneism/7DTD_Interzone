using System;
using UnityEngine.Scripting;

// Token: 0x020008B3 RID: 2227
[Preserve]
public class ObjectiveBlockActivate : BaseObjective
{
	// Token: 0x170006C0 RID: 1728
	// (get) Token: 0x06004109 RID: 16649 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Boolean;
		}
	}

	// Token: 0x0600410A RID: 16650 RVA: 0x001A625C File Offset: 0x001A445C
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveBlockActivate_keyword", false);
		this.localizedName = ((this.ID != "" && this.ID != null) ? Localization.Get(this.ID, false) : "Any Block");
	}

	// Token: 0x0600410B RID: 16651 RVA: 0x001A62AD File Offset: 0x001A44AD
	public override void SetupDisplay()
	{
		base.Description = string.Format(this.keyword, this.localizedName);
	}

	// Token: 0x0600410C RID: 16652 RVA: 0x00002914 File Offset: 0x00000B14
	public override void AddHooks()
	{
	}

	// Token: 0x0600410D RID: 16653 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RemoveHooks()
	{
	}

	// Token: 0x0600410E RID: 16654 RVA: 0x001A62C8 File Offset: 0x001A44C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockActivate(string blockName)
	{
		if (base.Complete)
		{
			return;
		}
		if ((this.ID == null || this.ID == "" || this.ID.EqualsCaseInsensitive(blockName)) && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue = 1;
			this.Refresh();
		}
	}

	// Token: 0x0600410F RID: 16655 RVA: 0x001A6320 File Offset: 0x001A4520
	public override void Refresh()
	{
		bool complete = base.CurrentValue == 1;
		base.Complete = complete;
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004110 RID: 16656 RVA: 0x001A6358 File Offset: 0x001A4558
	public override BaseObjective Clone()
	{
		ObjectiveBlockActivate objectiveBlockActivate = new ObjectiveBlockActivate();
		this.CopyValues(objectiveBlockActivate);
		return objectiveBlockActivate;
	}

	// Token: 0x04003413 RID: 13331
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName = "";
}

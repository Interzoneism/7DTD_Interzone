using System;
using UnityEngine.Scripting;

// Token: 0x020008B7 RID: 2231
[Preserve]
public class ObjectiveBuff : BaseObjective
{
	// Token: 0x0600412D RID: 16685 RVA: 0x001A6A0A File Offset: 0x001A4C0A
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveBuff_keyword", false);
		this.name = BuffManager.GetBuff(this.ID).Name;
	}

	// Token: 0x0600412E RID: 16686 RVA: 0x001A6A33 File Offset: 0x001A4C33
	public override void SetupDisplay()
	{
		byte currentValue = base.CurrentValue;
		base.Description = string.Format(this.keyword, this.name);
		this.StatusText = "";
	}

	// Token: 0x0600412F RID: 16687 RVA: 0x00002914 File Offset: 0x00000B14
	public override void AddHooks()
	{
	}

	// Token: 0x06004130 RID: 16688 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RemoveHooks()
	{
	}

	// Token: 0x06004131 RID: 16689 RVA: 0x001A6A60 File Offset: 0x001A4C60
	public override void Refresh()
	{
		if (base.Complete)
		{
			return;
		}
		bool complete = base.CurrentValue == 1;
		base.Complete = complete;
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x06004132 RID: 16690 RVA: 0x001A6AA0 File Offset: 0x001A4CA0
	public override BaseObjective Clone()
	{
		ObjectiveBuff objectiveBuff = new ObjectiveBuff();
		this.CopyValues(objectiveBuff);
		return objectiveBuff;
	}

	// Token: 0x06004133 RID: 16691 RVA: 0x001A6ABC File Offset: 0x001A4CBC
	public override string ParseBinding(string bindingName)
	{
		string id = this.ID;
		string value = this.Value;
		if (bindingName == "name")
		{
			return BuffManager.GetBuff(id).Name;
		}
		return "";
	}

	// Token: 0x0400341A RID: 13338
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}

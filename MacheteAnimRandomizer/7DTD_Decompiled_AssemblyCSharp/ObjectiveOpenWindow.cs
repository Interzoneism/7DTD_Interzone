using System;
using UnityEngine.Scripting;

// Token: 0x020008C8 RID: 2248
[Preserve]
public class ObjectiveOpenWindow : BaseObjective
{
	// Token: 0x060041F5 RID: 16885 RVA: 0x001AA4C0 File Offset: 0x001A86C0
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveOpenWindow_keyword", false);
	}

	// Token: 0x060041F6 RID: 16886 RVA: 0x001AA4D3 File Offset: 0x001A86D3
	public override void SetupDisplay()
	{
		byte currentValue = base.CurrentValue;
		base.Description = string.Format(this.keyword, this.ID);
		this.StatusText = "";
	}

	// Token: 0x060041F7 RID: 16887 RVA: 0x001AA4FE File Offset: 0x001A86FE
	public override void AddHooks()
	{
		QuestEventManager.Current.WindowChanged += this.Current_WindowOpened;
	}

	// Token: 0x060041F8 RID: 16888 RVA: 0x001AA516 File Offset: 0x001A8716
	public override void RemoveHooks()
	{
		QuestEventManager.Current.WindowChanged -= this.Current_WindowOpened;
	}

	// Token: 0x060041F9 RID: 16889 RVA: 0x001AA52E File Offset: 0x001A872E
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_WindowOpened(string windowName)
	{
		if (windowName.EqualsCaseInsensitive(this.ID) && base.OwnerQuest.CheckRequirements())
		{
			base.CurrentValue = 1;
			this.Refresh();
		}
	}

	// Token: 0x060041FA RID: 16890 RVA: 0x001AA558 File Offset: 0x001A8758
	public override void Refresh()
	{
		bool complete = base.CurrentValue == 1;
		base.Complete = complete;
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
		}
	}

	// Token: 0x060041FB RID: 16891 RVA: 0x00002914 File Offset: 0x00000B14
	public override void RemoveObjectives()
	{
	}

	// Token: 0x060041FC RID: 16892 RVA: 0x001AA590 File Offset: 0x001A8790
	public override BaseObjective Clone()
	{
		ObjectiveOpenWindow objectiveOpenWindow = new ObjectiveOpenWindow();
		this.CopyValues(objectiveOpenWindow);
		return objectiveOpenWindow;
	}
}

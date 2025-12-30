using System;
using UnityEngine.Scripting;

// Token: 0x0200091F RID: 2335
[Preserve]
public class RewardShowMessageWindow : BaseReward
{
	// Token: 0x060045A3 RID: 17827 RVA: 0x001BD4E9 File Offset: 0x001BB6E9
	public RewardShowMessageWindow()
	{
		base.HiddenReward = true;
	}

	// Token: 0x060045A4 RID: 17828 RVA: 0x001BD50E File Offset: 0x001BB70E
	public override void SetupReward()
	{
		base.HiddenReward = true;
	}

	// Token: 0x060045A5 RID: 17829 RVA: 0x001BD517 File Offset: 0x001BB717
	public override void GiveReward(EntityPlayer player)
	{
		XUiC_TipWindow.ShowTip(this.message, this.title, player as EntityPlayerLocal, null);
	}

	// Token: 0x060045A6 RID: 17830 RVA: 0x001BD534 File Offset: 0x001BB734
	public override BaseReward Clone()
	{
		RewardShowMessageWindow rewardShowMessageWindow = new RewardShowMessageWindow();
		base.CopyValues(rewardShowMessageWindow);
		rewardShowMessageWindow.title = this.title;
		rewardShowMessageWindow.message = this.message;
		return rewardShowMessageWindow;
	}

	// Token: 0x060045A7 RID: 17831 RVA: 0x001BD567 File Offset: 0x001BB767
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		properties.ParseString(RewardShowMessageWindow.PropMessage, ref this.message);
		properties.ParseString(RewardShowMessageWindow.PropTitle, ref this.title);
	}

	// Token: 0x0400366D RID: 13933
	[PublicizedFrom(EAccessModifier.Private)]
	public static string PropMessage = "message";

	// Token: 0x0400366E RID: 13934
	[PublicizedFrom(EAccessModifier.Private)]
	public static string PropTitle = "title";

	// Token: 0x0400366F RID: 13935
	[PublicizedFrom(EAccessModifier.Private)]
	public string message = "";

	// Token: 0x04003670 RID: 13936
	[PublicizedFrom(EAccessModifier.Private)]
	public string title = "";
}

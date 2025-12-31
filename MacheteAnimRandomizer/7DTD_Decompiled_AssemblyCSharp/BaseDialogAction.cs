using System;

// Token: 0x0200029A RID: 666
public abstract class BaseDialogAction
{
	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060012F3 RID: 4851 RVA: 0x0007584D File Offset: 0x00073A4D
	// (set) Token: 0x060012F4 RID: 4852 RVA: 0x00075855 File Offset: 0x00073A55
	public string ID { get; set; }

	// Token: 0x170001FE RID: 510
	// (get) Token: 0x060012F5 RID: 4853 RVA: 0x0007585E File Offset: 0x00073A5E
	// (set) Token: 0x060012F6 RID: 4854 RVA: 0x00075866 File Offset: 0x00073A66
	public string Value { get; set; }

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x060012F7 RID: 4855 RVA: 0x0007586F File Offset: 0x00073A6F
	// (set) Token: 0x060012F8 RID: 4856 RVA: 0x00075877 File Offset: 0x00073A77
	public Dialog OwnerDialog { get; set; }

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x060012F9 RID: 4857 RVA: 0x00075880 File Offset: 0x00073A80
	// (set) Token: 0x060012FA RID: 4858 RVA: 0x00075888 File Offset: 0x00073A88
	public DialogResponse Owner { get; set; }

	// Token: 0x17000201 RID: 513
	// (get) Token: 0x060012FB RID: 4859 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.AddBuff;
		}
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x00075891 File Offset: 0x00073A91
	public BaseDialogAction()
	{
		this.ID = "";
		this.Value = "";
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x000758AF File Offset: 0x00073AAF
	[PublicizedFrom(EAccessModifier.Protected)]
	public void CopyValues(BaseDialogAction action)
	{
		action.ID = this.ID;
		action.Value = this.Value;
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetupAction()
	{
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void PerformAction(EntityPlayer player)
	{
	}

	// Token: 0x06001300 RID: 4864 RVA: 0x00019766 File Offset: 0x00017966
	public virtual BaseDialogAction Clone()
	{
		return null;
	}

	// Token: 0x0200029B RID: 667
	public enum ActionTypes
	{
		// Token: 0x04000C84 RID: 3204
		AddBuff,
		// Token: 0x04000C85 RID: 3205
		AddItem,
		// Token: 0x04000C86 RID: 3206
		AddQuest,
		// Token: 0x04000C87 RID: 3207
		CompleteQuest,
		// Token: 0x04000C88 RID: 3208
		Trader,
		// Token: 0x04000C89 RID: 3209
		Voice
	}
}

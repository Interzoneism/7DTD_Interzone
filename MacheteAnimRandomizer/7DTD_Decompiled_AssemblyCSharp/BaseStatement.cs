using System;
using System.Collections.Generic;

// Token: 0x020002A5 RID: 677
public class BaseStatement : BaseDialogItem
{
	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06001325 RID: 4901 RVA: 0x000761DF File Offset: 0x000743DF
	// (set) Token: 0x06001326 RID: 4902 RVA: 0x000761E7 File Offset: 0x000743E7
	public string NextStatementID { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x06001327 RID: 4903 RVA: 0x000761F0 File Offset: 0x000743F0
	public override string ToString()
	{
		return this.Text;
	}

	// Token: 0x06001328 RID: 4904 RVA: 0x000761F8 File Offset: 0x000743F8
	[PublicizedFrom(EAccessModifier.Internal)]
	public void AddAction(BaseDialogAction action)
	{
		this.Actions.Add(action);
	}

	// Token: 0x04000CA2 RID: 3234
	public string Text;

	// Token: 0x04000CA4 RID: 3236
	public List<BaseDialogAction> Actions = new List<BaseDialogAction>();
}

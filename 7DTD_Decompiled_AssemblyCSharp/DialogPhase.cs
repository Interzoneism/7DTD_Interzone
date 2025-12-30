using System;
using System.Collections.Generic;

// Token: 0x020002A6 RID: 678
public class DialogPhase : BaseDialogItem
{
	// Token: 0x1700020C RID: 524
	// (get) Token: 0x0600132A RID: 4906 RVA: 0x00076219 File Offset: 0x00074419
	// (set) Token: 0x0600132B RID: 4907 RVA: 0x00076221 File Offset: 0x00074421
	public string StartStatementID { get; set; }

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x0600132C RID: 4908 RVA: 0x0007622A File Offset: 0x0007442A
	// (set) Token: 0x0600132D RID: 4909 RVA: 0x00076232 File Offset: 0x00074432
	public string StartResponseID { get; set; }

	// Token: 0x0600132E RID: 4910 RVA: 0x0007623B File Offset: 0x0007443B
	public DialogPhase(string newID)
	{
		this.ID = newID;
		this.HeaderName = string.Format("Phase : {0}", newID);
	}

	// Token: 0x0600132F RID: 4911 RVA: 0x00076266 File Offset: 0x00074466
	[PublicizedFrom(EAccessModifier.Internal)]
	public void AddRequirement(BaseDialogRequirement requirement)
	{
		this.RequirementList.Add(requirement);
	}

	// Token: 0x06001330 RID: 4912 RVA: 0x00076274 File Offset: 0x00074474
	public override string ToString()
	{
		return this.HeaderName;
	}

	// Token: 0x04000CA5 RID: 3237
	public List<BaseDialogRequirement> RequirementList = new List<BaseDialogRequirement>();
}

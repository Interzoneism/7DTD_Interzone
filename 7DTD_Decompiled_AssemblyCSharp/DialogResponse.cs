using System;
using System.Collections.Generic;

// Token: 0x020002A7 RID: 679
public class DialogResponse : BaseStatement
{
	// Token: 0x06001331 RID: 4913 RVA: 0x0007627C File Offset: 0x0007447C
	public DialogResponse(string newID)
	{
		this.ID = newID;
		this.HeaderName = string.Format("Response : {0}", newID);
	}

	// Token: 0x06001332 RID: 4914 RVA: 0x000762B2 File Offset: 0x000744B2
	[PublicizedFrom(EAccessModifier.Internal)]
	public static DialogResponse NextStatementEntry(string nextStatementID)
	{
		DialogResponse.nextStatementEntry.NextStatementID = nextStatementID;
		DialogResponse.nextStatementEntry.Text = "[" + Localization.Get("xuiNext", false) + "]";
		return DialogResponse.nextStatementEntry;
	}

	// Token: 0x06001333 RID: 4915 RVA: 0x000762E8 File Offset: 0x000744E8
	[PublicizedFrom(EAccessModifier.Internal)]
	public void AddRequirement(BaseDialogRequirement requirement)
	{
		this.RequirementList.Add(requirement);
	}

	// Token: 0x06001334 RID: 4916 RVA: 0x000762F6 File Offset: 0x000744F6
	public string GetRequiredDescription(EntityPlayer player)
	{
		if (this.RequirementList.Count == 0)
		{
			return "";
		}
		return this.RequirementList[0].GetRequiredDescription(player);
	}

	// Token: 0x04000CA8 RID: 3240
	public List<BaseDialogRequirement> RequirementList = new List<BaseDialogRequirement>();

	// Token: 0x04000CA9 RID: 3241
	public string ReturnStatementID = "";

	// Token: 0x04000CAA RID: 3242
	[PublicizedFrom(EAccessModifier.Private)]
	public static DialogResponse nextStatementEntry = new DialogResponse("__nextStatementEntry");
}

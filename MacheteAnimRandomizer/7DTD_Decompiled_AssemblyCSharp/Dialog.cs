using System;
using System.Collections.Generic;

// Token: 0x020002A3 RID: 675
public class Dialog
{
	// Token: 0x17000208 RID: 520
	// (get) Token: 0x06001315 RID: 4885 RVA: 0x00075E65 File Offset: 0x00074065
	// (set) Token: 0x06001316 RID: 4886 RVA: 0x00075E81 File Offset: 0x00074081
	public DialogStatement CurrentStatement
	{
		get
		{
			if (this.ChildDialog != null)
			{
				return this.ChildDialog.CurrentStatement;
			}
			return this.currentStatement;
		}
		set
		{
			if (this.ChildDialog != null)
			{
				this.ChildDialog.CurrentStatement = value;
				return;
			}
			this.currentStatement = value;
		}
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x00075EA0 File Offset: 0x000740A0
	public Dialog(string newID)
	{
		this.ID = newID;
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x00075F14 File Offset: 0x00074114
	[PublicizedFrom(EAccessModifier.Internal)]
	public DialogStatement GetStatement(string currentStatementID)
	{
		if (this.ChildDialog != null)
		{
			return this.ChildDialog.GetStatement(currentStatementID);
		}
		if (currentStatementID == "")
		{
			currentStatementID = this.StartStatementID;
		}
		for (int i = 0; i < this.Statements.Count; i++)
		{
			if (this.Statements[i].ID == currentStatementID)
			{
				return this.Statements[i];
			}
		}
		return null;
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x00075F88 File Offset: 0x00074188
	[PublicizedFrom(EAccessModifier.Internal)]
	public DialogResponse GetResponse(string currentResponseID)
	{
		if (this.ChildDialog != null)
		{
			return this.ChildDialog.GetResponse(currentResponseID);
		}
		for (int i = 0; i < this.Responses.Count; i++)
		{
			if (this.Responses[i].ID == currentResponseID)
			{
				return this.Responses[i];
			}
		}
		return null;
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x00075FE8 File Offset: 0x000741E8
	[PublicizedFrom(EAccessModifier.Internal)]
	public DialogStatement GetFirstStatment(EntityPlayer player)
	{
		string startStatementID = this.StartStatementID;
		for (int i = 0; i < this.Phases.Count; i++)
		{
			bool flag = true;
			for (int j = 0; j < this.Phases[i].RequirementList.Count; j++)
			{
				if (!this.Phases[i].RequirementList[j].CheckRequirement(player, this.CurrentOwner))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				startStatementID = this.Phases[i].StartStatementID;
				break;
			}
		}
		for (int k = 0; k < this.Statements.Count; k++)
		{
			if (this.Statements[k].ID == startStatementID)
			{
				return this.Statements[k];
			}
		}
		return null;
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x000760B8 File Offset: 0x000742B8
	public void RestartDialog(EntityPlayer player)
	{
		this.CurrentStatement = this.GetFirstStatment(player);
		this.ChildDialog = null;
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x000760D0 File Offset: 0x000742D0
	public void SelectResponse(DialogResponse response, EntityPlayer player)
	{
		if (this.ChildDialog != null)
		{
			this.ChildDialog.SelectResponse(response, player);
			return;
		}
		if (response.Actions.Count > 0)
		{
			for (int i = 0; i < response.Actions.Count; i++)
			{
				response.Actions[i].PerformAction(player);
			}
		}
		if (response is DialogResponseQuest)
		{
			DialogResponseQuest dialogResponseQuest = response as DialogResponseQuest;
			QuestClass questClass = dialogResponseQuest.Quest.QuestClass;
			this.CurrentStatement = new DialogStatement("");
			this.CurrentStatement.NextStatementID = dialogResponseQuest.NextStatementID;
			this.CurrentStatement.Text = dialogResponseQuest.Quest.GetParsedText(questClass.StatementText);
			return;
		}
		this.CurrentStatement = this.GetStatement(response.NextStatementID);
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x00076194 File Offset: 0x00074394
	public static void Cleanup()
	{
		Dialog.DialogList.Clear();
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x000761A0 File Offset: 0x000743A0
	public static void ReloadDialogs()
	{
		Dialog.Cleanup();
		WorldStaticData.Reset("dialogs");
	}

	// Token: 0x04000C93 RID: 3219
	public static Dictionary<string, Dialog> DialogList = new Dictionary<string, Dialog>();

	// Token: 0x04000C94 RID: 3220
	public string ID = "";

	// Token: 0x04000C95 RID: 3221
	public string StartStatementID = "";

	// Token: 0x04000C96 RID: 3222
	public string StartResponseID = "";

	// Token: 0x04000C97 RID: 3223
	public List<DialogPhase> Phases = new List<DialogPhase>();

	// Token: 0x04000C98 RID: 3224
	public List<DialogStatement> Statements = new List<DialogStatement>();

	// Token: 0x04000C99 RID: 3225
	public List<DialogResponse> Responses = new List<DialogResponse>();

	// Token: 0x04000C9A RID: 3226
	public EntityNPC CurrentOwner;

	// Token: 0x04000C9B RID: 3227
	public Dialog ChildDialog;

	// Token: 0x04000C9C RID: 3228
	public List<QuestEntry> QuestEntryList = new List<QuestEntry>();

	// Token: 0x04000C9D RID: 3229
	[PublicizedFrom(EAccessModifier.Private)]
	public DialogStatement currentStatement;

	// Token: 0x04000C9E RID: 3230
	public string currentReturnStatement = "";
}

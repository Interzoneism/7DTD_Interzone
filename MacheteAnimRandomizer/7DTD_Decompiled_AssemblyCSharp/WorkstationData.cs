using System;

// Token: 0x02000299 RID: 665
public class WorkstationData
{
	// Token: 0x060012F2 RID: 4850 RVA: 0x00075640 File Offset: 0x00073840
	public WorkstationData(string blockName, DynamicProperties properties)
	{
		if (properties.Values.ContainsKey("WorkstationName"))
		{
			this.WorkstationName = properties.Values["WorkstationName"];
		}
		else
		{
			this.WorkstationName = blockName;
		}
		if (properties.Values.ContainsKey("WorkstationIcon"))
		{
			this.WorkstationIcon = properties.Values["WorkstationIcon"];
		}
		else
		{
			this.WorkstationIcon = "ui_game_symbol_hammer";
		}
		if (properties.Values.ContainsKey("CraftActionName"))
		{
			this.CraftActionName = Localization.Get(properties.Values["CraftActionName"], false);
		}
		else
		{
			this.CraftActionName = Localization.Get("lblContextActionCraft", false);
		}
		if (properties.Values.ContainsKey("CraftIcon"))
		{
			this.CraftIcon = properties.Values["CraftIcon"];
		}
		else
		{
			this.CraftIcon = "ui_game_symbol_hammer";
		}
		if (properties.Values.ContainsKey("OpenSound"))
		{
			this.OpenSound = properties.Values["OpenSound"];
		}
		else
		{
			this.OpenSound = "open_workbench";
		}
		if (properties.Values.ContainsKey("CloseSound"))
		{
			this.CloseSound = properties.Values["CloseSound"];
		}
		else
		{
			this.CloseSound = "close_workbench";
		}
		if (properties.Values.ContainsKey("CraftSound"))
		{
			this.CraftSound = properties.Values["CraftSound"];
		}
		else
		{
			this.CraftSound = "craft_click_craft";
		}
		if (properties.Values.ContainsKey("CraftCompleteSound"))
		{
			this.CraftCompleteSound = properties.Values["CraftCompleteSound"];
		}
		else
		{
			this.CraftCompleteSound = "craft_complete_item";
		}
		if (properties.Values.ContainsKey("WorkstationWindow"))
		{
			this.WorkstationWindow = properties.Values["WorkstationWindow"];
			return;
		}
		this.WorkstationWindow = "";
	}

	// Token: 0x04000C76 RID: 3190
	public string WorkstationName;

	// Token: 0x04000C77 RID: 3191
	public string WorkstationIcon;

	// Token: 0x04000C78 RID: 3192
	public string CraftIcon;

	// Token: 0x04000C79 RID: 3193
	public string CraftActionName = "";

	// Token: 0x04000C7A RID: 3194
	public string WorkstationWindow = "";

	// Token: 0x04000C7B RID: 3195
	public string OpenSound;

	// Token: 0x04000C7C RID: 3196
	public string CloseSound;

	// Token: 0x04000C7D RID: 3197
	public string CraftSound;

	// Token: 0x04000C7E RID: 3198
	public string CraftCompleteSound;
}

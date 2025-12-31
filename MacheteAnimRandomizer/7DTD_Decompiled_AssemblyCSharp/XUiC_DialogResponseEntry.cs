using System;
using UnityEngine.Scripting;

// Token: 0x02000C8F RID: 3215
[Preserve]
public class XUiC_DialogResponseEntry : XUiController
{
	// Token: 0x17000A1F RID: 2591
	// (get) Token: 0x06006332 RID: 25394 RVA: 0x00284351 File Offset: 0x00282551
	// (set) Token: 0x06006333 RID: 25395 RVA: 0x0028435C File Offset: 0x0028255C
	public DialogResponse CurrentResponse
	{
		get
		{
			return this.currentResponse;
		}
		set
		{
			this.currentResponse = value;
			this.HasRequirement = true;
			base.ViewComponent.Enabled = (value != null);
			if (this.currentResponse != null && this.currentResponse.RequirementList.Count > 0)
			{
				int i = 0;
				while (i < this.currentResponse.RequirementList.Count)
				{
					if (!this.currentResponse.RequirementList[i].CheckRequirement(base.xui.playerUI.entityPlayer, base.xui.Dialog.Respondent))
					{
						this.HasRequirement = false;
						if (this.currentResponse.RequirementList[i].RequirementVisibilityType == BaseDialogRequirement.RequirementVisibilityTypes.Hide)
						{
							this.currentResponse = null;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006334 RID: 25396 RVA: 0x00284428 File Offset: 0x00282628
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.currentResponse != null;
		if (bindingName == "response")
		{
			value = "";
			if (flag)
			{
				value = (this.HasRequirement ? this.currentResponse.Text : this.currentResponse.GetRequiredDescription(base.xui.playerUI.entityPlayer));
			}
			return true;
		}
		if (bindingName == "textstatecolor")
		{
			value = "255,255,255,255";
			if (flag)
			{
				value = (this.HasRequirement ? XUiC_DialogResponseEntry.enabledColor : XUiC_DialogResponseEntry.disabledColor);
			}
			return true;
		}
		if (bindingName == "rowstatecolor")
		{
			value = "255,255,255,255";
			if (flag)
			{
				if (this.HasRequirement)
				{
					value = (this.Selected ? "255,255,255,255" : (this.IsHovered ? this.hoverColor : XUiC_DialogResponseEntry.enabledColor));
				}
				else
				{
					value = XUiC_DialogResponseEntry.disabledColor;
				}
			}
			return true;
		}
		if (bindingName == "rowstatesprite")
		{
			value = (this.Selected ? "ui_game_select_row" : "menu_empty");
			return true;
		}
		if (!(bindingName == "showresponse"))
		{
			return false;
		}
		value = flag.ToString();
		return true;
	}

	// Token: 0x06006335 RID: 25397 RVA: 0x00284555 File Offset: 0x00282755
	public override void Init()
	{
		base.Init();
		this.IsDirty = true;
	}

	// Token: 0x06006336 RID: 25398 RVA: 0x00284564 File Offset: 0x00282764
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		base.OnHovered(_isOver);
		if (this.currentResponse == null)
		{
			this.IsHovered = false;
			return;
		}
		if (this.IsHovered != _isOver)
		{
			this.IsHovered = _isOver;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06006337 RID: 25399 RVA: 0x00284594 File Offset: 0x00282794
	public override void Update(float _dt)
	{
		base.RefreshBindings(this.IsDirty);
		this.IsDirty = false;
		base.Update(_dt);
	}

	// Token: 0x06006338 RID: 25400 RVA: 0x0007FB49 File Offset: 0x0007DD49
	public void Refresh()
	{
		this.IsDirty = true;
	}

	// Token: 0x06006339 RID: 25401 RVA: 0x002845B0 File Offset: 0x002827B0
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "enabled_color")
		{
			XUiC_DialogResponseEntry.enabledColor = value;
			return true;
		}
		if (name == "disabled_color")
		{
			XUiC_DialogResponseEntry.disabledColor = value;
			return true;
		}
		if (name == "row_color")
		{
			this.rowColor = value;
			return true;
		}
		if (!(name == "hover_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		this.hoverColor = value;
		return true;
	}

	// Token: 0x04004ABC RID: 19132
	[PublicizedFrom(EAccessModifier.Private)]
	public string rowColor;

	// Token: 0x04004ABD RID: 19133
	[PublicizedFrom(EAccessModifier.Private)]
	public string hoverColor;

	// Token: 0x04004ABE RID: 19134
	public new bool Selected;

	// Token: 0x04004ABF RID: 19135
	public bool IsHovered;

	// Token: 0x04004AC0 RID: 19136
	public bool HasRequirement = true;

	// Token: 0x04004AC1 RID: 19137
	[PublicizedFrom(EAccessModifier.Private)]
	public static string enabledColor = "255,255,255,255";

	// Token: 0x04004AC2 RID: 19138
	[PublicizedFrom(EAccessModifier.Private)]
	public static string disabledColor = "200,200,200,255";

	// Token: 0x04004AC3 RID: 19139
	[PublicizedFrom(EAccessModifier.Private)]
	public DialogResponse currentResponse;
}

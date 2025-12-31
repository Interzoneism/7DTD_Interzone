using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000C90 RID: 3216
[Preserve]
public class XUiC_DialogResponseList : XUiController
{
	// Token: 0x17000A20 RID: 2592
	// (get) Token: 0x0600633C RID: 25404 RVA: 0x00284643 File Offset: 0x00282843
	// (set) Token: 0x0600633D RID: 25405 RVA: 0x0028464B File Offset: 0x0028284B
	public Dialog CurrentDialog
	{
		get
		{
			return this.currentDialog;
		}
		set
		{
			this.currentDialog = value;
			base.RefreshBindings(true);
			this.IsDirty = true;
		}
	}

	// Token: 0x0600633E RID: 25406 RVA: 0x00284664 File Offset: 0x00282864
	public override void Init()
	{
		base.Init();
		this.xuiQuestDescriptionLabel = Localization.Get("xuiDescriptionLabel", false);
		this.lblResponderName = (base.GetChildById("lblName").ViewComponent as XUiV_Label);
		XUiController childById = base.GetChildById("items");
		for (int i = 0; i < childById.Children.Count; i++)
		{
			XUiC_DialogResponseEntry xuiC_DialogResponseEntry = childById.Children[i] as XUiC_DialogResponseEntry;
			if (xuiC_DialogResponseEntry != null)
			{
				this.entryList.Add(xuiC_DialogResponseEntry);
				this.length++;
			}
		}
	}

	// Token: 0x0600633F RID: 25407 RVA: 0x002846F4 File Offset: 0x002828F4
	public override void OnOpen()
	{
		base.OnOpen();
		this.lblResponderName.Text = Localization.Get(base.xui.Dialog.Respondent.EntityName, false);
	}

	// Token: 0x06006340 RID: 25408 RVA: 0x00284724 File Offset: 0x00282924
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			List<BaseResponseEntry> list = new List<BaseResponseEntry>();
			this.uniqueResponseIDs.Clear();
			if (this.currentDialog != null)
			{
				list = this.currentDialog.CurrentStatement.GetResponses();
			}
			int num = 0;
			for (int i = 0; i < this.entryList.Count; i++)
			{
				XUiC_DialogResponseEntry xuiC_DialogResponseEntry = this.entryList[i];
				if (xuiC_DialogResponseEntry != null)
				{
					xuiC_DialogResponseEntry.OnPress -= this.OnPressResponse;
					if (num < list.Count)
					{
						xuiC_DialogResponseEntry.ViewComponent.SoundPlayOnClick = true;
						if (list[num].UniqueID == "" || !this.uniqueResponseIDs.Contains(list[num].UniqueID))
						{
							xuiC_DialogResponseEntry.CurrentResponse = list[num].Response;
							xuiC_DialogResponseEntry.OnPress += this.OnPressResponse;
						}
						else
						{
							xuiC_DialogResponseEntry.CurrentResponse = null;
						}
						if (xuiC_DialogResponseEntry.CurrentResponse == null)
						{
							i--;
						}
						else if (list[num].UniqueID != "")
						{
							this.uniqueResponseIDs.Add(list[num].UniqueID);
						}
						num++;
					}
					else
					{
						xuiC_DialogResponseEntry.ViewComponent.SoundPlayOnClick = false;
						xuiC_DialogResponseEntry.CurrentResponse = null;
					}
				}
			}
			if (list.Count > 0)
			{
				this.entryList[0].SelectCursorElement(true, false);
			}
			this.IsDirty = false;
		}
	}

	// Token: 0x06006341 RID: 25409 RVA: 0x002848A0 File Offset: 0x00282AA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressResponse(XUiController _sender, int _mouseButton)
	{
		if (((XUiC_DialogResponseEntry)_sender).HasRequirement)
		{
			DialogResponse currentResponse = ((XUiC_DialogResponseEntry)_sender).CurrentResponse;
			this.currentDialog.SelectResponse(currentResponse, base.xui.playerUI.entityPlayer);
			((XUiC_DialogWindowGroup)this.windowGroup.Controller).RefreshDialog();
		}
	}

	// Token: 0x06006342 RID: 25410 RVA: 0x002848F7 File Offset: 0x00282AF7
	public void Refresh()
	{
		this.IsDirty = true;
		base.RefreshBindings(true);
	}

	// Token: 0x04004AC4 RID: 19140
	[PublicizedFrom(EAccessModifier.Private)]
	public Dialog conversation;

	// Token: 0x04004AC5 RID: 19141
	[PublicizedFrom(EAccessModifier.Private)]
	public string xuiQuestDescriptionLabel;

	// Token: 0x04004AC6 RID: 19142
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_DialogResponseEntry> entryList = new List<XUiC_DialogResponseEntry>();

	// Token: 0x04004AC7 RID: 19143
	[PublicizedFrom(EAccessModifier.Private)]
	public int length;

	// Token: 0x04004AC8 RID: 19144
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> uniqueResponseIDs = new List<string>();

	// Token: 0x04004AC9 RID: 19145
	[PublicizedFrom(EAccessModifier.Private)]
	public Dialog currentDialog;

	// Token: 0x04004ACA RID: 19146
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblResponderName;
}

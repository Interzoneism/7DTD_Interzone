using System;
using UnityEngine.Scripting;

// Token: 0x02000E2A RID: 3626
[Preserve]
public class XUiC_SignWindow : XUiController
{
	// Token: 0x06007193 RID: 29075 RVA: 0x002E4284 File Offset: 0x002E2484
	public override void Init()
	{
		base.Init();
		XUiView xuiView = (XUiV_Button)base.GetChildById("clickable").ViewComponent;
		this.textInput1 = (base.GetChildById("input1") as XUiC_TextInput);
		this.textInput1.OnSubmitHandler += this.TextInput_OnSubmitHandler;
		this.textInput1.SupportBbCode = false;
		this.textInput2 = (base.GetChildById("input2") as XUiC_TextInput);
		if (this.textInput2 != null)
		{
			this.textInput2.OnSubmitHandler += this.TextInput_OnSubmitHandler;
			this.textInput2.SupportBbCode = false;
			this.textInput3 = (base.GetChildById("input3") as XUiC_TextInput);
			this.textInput3.OnSubmitHandler += this.TextInput_OnSubmitHandler;
			this.textInput3.SupportBbCode = false;
			this.textInput1.SelectOnTab = this.textInput2;
			this.textInput2.SelectOnTab = this.textInput3;
			this.textInput3.SelectOnTab = this.textInput1;
			this.separateLineMode = true;
		}
		xuiView.Controller.OnPress += this.closeButton_OnPress;
	}

	// Token: 0x06007194 RID: 29076 RVA: 0x002E43B4 File Offset: 0x002E25B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void TextInput_OnSubmitHandler(XUiController _sender, string _text)
	{
		if (this.separateLineMode)
		{
			XUiC_TextInput xuiC_TextInput = _sender as XUiC_TextInput;
			if (xuiC_TextInput != null)
			{
				xuiC_TextInput.SelectOnTab.SelectCursorElement(false, false);
				xuiC_TextInput.SelectOnTab.SetSelected(true, false);
				return;
			}
		}
		else
		{
			base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
		}
	}

	// Token: 0x06007195 RID: 29077 RVA: 0x00269150 File Offset: 0x00267350
	[PublicizedFrom(EAccessModifier.Private)]
	public void closeButton_OnPress(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(base.WindowGroup.ID);
	}

	// Token: 0x06007196 RID: 29078 RVA: 0x002E440F File Offset: 0x002E260F
	public void SetTileEntitySign(ITileEntitySignable _te)
	{
		this.SignTileEntity = _te;
	}

	// Token: 0x06007197 RID: 29079 RVA: 0x002E4418 File Offset: 0x002E2618
	public override void OnOpen()
	{
		base.OnOpen();
		string displayTextImmediately = GeneratedTextManager.GetDisplayTextImmediately(this.SignTileEntity.GetAuthoredText(), true, GeneratedTextManager.TextFilteringMode.Filter, GeneratedTextManager.BbCodeSupportMode.NotSupported);
		if (this.separateLineMode)
		{
			this.textInput1.Text = (this.textInput2.Text = (this.textInput3.Text = ""));
			string[] array = displayTextImmediately.Split('\n', StringSplitOptions.None);
			if (array.Length != 0)
			{
				this.textInput1.Text = array[0];
				if (array.Length > 1)
				{
					this.textInput2.Text = array[1];
					if (array.Length > 2)
					{
						this.textInput3.Text = array[2];
					}
				}
			}
		}
		else
		{
			this.textInput1.Text = displayTextImmediately;
		}
		base.xui.playerUI.entityPlayer.PlayOneShot("open_sign", false, false, false, null);
		base.xui.playerUI.CursorController.SetNavigationLockView(base.ViewComponent, null);
	}

	// Token: 0x06007198 RID: 29080 RVA: 0x002E4500 File Offset: 0x002E2700
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.entityPlayer.PlayOneShot("close_sign", false, false, false, null);
		base.xui.playerUI.CursorController.SetNavigationLockView(null, null);
		if (GameManager.Instance.World.GetTileEntity(this.SignTileEntity.GetClrIdx(), this.SignTileEntity.ToWorldPos()).GetSelfOrFeature<ITileEntitySignable>() != this.SignTileEntity)
		{
			this.FinishClosing();
			return;
		}
		string text = this.separateLineMode ? string.Format("{0}\n{1}\n{2}", this.textInput1.Text, this.textInput2.Text, this.textInput3.Text) : this.textInput1.Text;
		if (!this.SignTileEntity.CanRenderString(text))
		{
			GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "uiInvalidCharacters", false, false, 0f);
			this.FinishClosing();
			return;
		}
		PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(base.xui.playerUI.entityPlayer.entityId);
		this.SignTileEntity.SetText(text, true, (playerDataFromEntityID != null) ? playerDataFromEntityID.PrimaryId : null);
		GeneratedTextManager.GetDisplayText(this.SignTileEntity.GetAuthoredText(), delegate(string _)
		{
			this.FinishClosing();
		}, true, false, GeneratedTextManager.TextFilteringMode.FilterWithSafeString, GeneratedTextManager.BbCodeSupportMode.NotSupported);
	}

	// Token: 0x06007199 RID: 29081 RVA: 0x002E4656 File Offset: 0x002E2856
	[PublicizedFrom(EAccessModifier.Private)]
	public void FinishClosing()
	{
		this.SignTileEntity.SetUserAccessing(false);
		GameManager.Instance.TEUnlockServer(this.SignTileEntity.GetClrIdx(), this.SignTileEntity.ToWorldPos(), this.SignTileEntity.EntityId, true);
	}

	// Token: 0x04005669 RID: 22121
	[PublicizedFrom(EAccessModifier.Private)]
	public ITileEntitySignable SignTileEntity;

	// Token: 0x0400566A RID: 22122
	[PublicizedFrom(EAccessModifier.Private)]
	public bool separateLineMode;

	// Token: 0x0400566B RID: 22123
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput textInput1;

	// Token: 0x0400566C RID: 22124
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput textInput2;

	// Token: 0x0400566D RID: 22125
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput textInput3;
}

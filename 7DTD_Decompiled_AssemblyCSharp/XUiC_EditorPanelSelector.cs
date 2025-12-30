using System;
using UnityEngine.Scripting;

// Token: 0x02000CA9 RID: 3241
[Preserve]
public class XUiC_EditorPanelSelector : XUiController
{
	// Token: 0x06006418 RID: 25624 RVA: 0x00288440 File Offset: 0x00286640
	public override void Init()
	{
		base.Init();
		XUiC_EditorPanelSelector.ID = base.WindowGroup.ID;
		this.buttons = base.GetChildByType<XUiC_CategoryList>();
		if (this.buttons != null)
		{
			this.buttons.CategoryClickChanged += this.ButtonsOnCategoryClickChanged;
			this.buttons.SetCategoryToFirst();
		}
	}

	// Token: 0x06006419 RID: 25625 RVA: 0x00288499 File Offset: 0x00286699
	[PublicizedFrom(EAccessModifier.Private)]
	public void ButtonsOnCategoryClickChanged(XUiC_CategoryEntry _categoryEntry)
	{
		this.OpenSelectedWindow();
	}

	// Token: 0x0600641A RID: 25626 RVA: 0x002884A4 File Offset: 0x002866A4
	public void OpenSelectedWindow()
	{
		if (this.buttons == null)
		{
			return;
		}
		XUiC_CategoryEntry currentCategory = this.buttons.CurrentCategory;
		string text = (currentCategory != null) ? currentCategory.CategoryName : null;
		foreach (XUiC_CategoryEntry xuiC_CategoryEntry in this.buttons.CategoryButtons)
		{
			if (text == null || text != xuiC_CategoryEntry.CategoryName)
			{
				base.xui.playerUI.windowManager.Close(xuiC_CategoryEntry.CategoryName);
			}
		}
		if (text != null)
		{
			base.xui.playerUI.windowManager.OpenIfNotOpen(text, false, false, true);
		}
	}

	// Token: 0x0600641B RID: 25627 RVA: 0x00288560 File Offset: 0x00286760
	public void SetSelected(string _name)
	{
		XUiC_CategoryList xuiC_CategoryList = this.buttons;
		if (xuiC_CategoryList == null)
		{
			return;
		}
		xuiC_CategoryList.SetCategory(_name);
	}

	// Token: 0x0600641C RID: 25628 RVA: 0x00288573 File Offset: 0x00286773
	public override void OnOpen()
	{
		base.OnOpen();
		if (PrefabEditModeManager.Instance.IsActive() && PrefabEditModeManager.Instance.VoxelPrefab != null)
		{
			PrefabEditModeManager.Instance.VoxelPrefab.RenderingCostStats = WorldStats.CaptureWorldStats();
		}
		this.OpenSelectedWindow();
	}

	// Token: 0x0600641D RID: 25629 RVA: 0x002885B0 File Offset: 0x002867B0
	public override void OnClose()
	{
		base.OnClose();
		if (this.buttons != null)
		{
			foreach (XUiC_CategoryEntry xuiC_CategoryEntry in this.buttons.CategoryButtons)
			{
				base.xui.playerUI.windowManager.Close(xuiC_CategoryEntry.CategoryName);
			}
		}
	}

	// Token: 0x0600641E RID: 25630 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x0600641F RID: 25631 RVA: 0x0028862C File Offset: 0x0028682C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "panelname")
		{
			XUiC_CategoryList xuiC_CategoryList = this.buttons;
			string text;
			if (xuiC_CategoryList == null)
			{
				text = null;
			}
			else
			{
				XUiC_CategoryEntry currentCategory = xuiC_CategoryList.CurrentCategory;
				text = ((currentCategory != null) ? currentCategory.CategoryDisplayName : null);
			}
			_value = (text ?? "");
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x04004B45 RID: 19269
	public static string ID = "";

	// Token: 0x04004B46 RID: 19270
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CategoryList buttons;
}

using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E99 RID: 3737
[Preserve]
public class XUiC_UnlockByEntry : XUiController
{
	// Token: 0x17000C03 RID: 3075
	// (get) Token: 0x060075F5 RID: 30197 RVA: 0x00300C41 File Offset: 0x002FEE41
	// (set) Token: 0x060075F6 RID: 30198 RVA: 0x00300C49 File Offset: 0x002FEE49
	public Recipe Recipe { get; set; }

	// Token: 0x17000C04 RID: 3076
	// (get) Token: 0x060075F7 RID: 30199 RVA: 0x00300C52 File Offset: 0x002FEE52
	// (set) Token: 0x060075F8 RID: 30200 RVA: 0x00300C5A File Offset: 0x002FEE5A
	public RecipeUnlockData UnlockData
	{
		get
		{
			return this.unlockData;
		}
		set
		{
			this.unlockData = value;
			this.isDirty = true;
		}
	}

	// Token: 0x060075F9 RID: 30201 RVA: 0x00300C6A File Offset: 0x002FEE6A
	public override void Init()
	{
		base.Init();
		this.isDirty = false;
	}

	// Token: 0x060075FA RID: 30202 RVA: 0x00300C7C File Offset: 0x002FEE7C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.UnlockData != null;
		if (bindingName == "name")
		{
			if (flag)
			{
				value = this.UnlockData.GetName();
			}
			else
			{
				value = "";
			}
			return true;
		}
		if (bindingName == "itemicon")
		{
			if (flag)
			{
				value = this.UnlockData.GetIcon();
			}
			else
			{
				value = "";
			}
			return true;
		}
		if (bindingName == "itemiconatlas")
		{
			if (flag)
			{
				value = this.UnlockData.GetIconAtlas();
			}
			else
			{
				value = "UIAtlas";
			}
			return true;
		}
		if (bindingName == "itemicontint")
		{
			Color32 v = Color.white;
			if (flag)
			{
				v = this.UnlockData.GetItemTint();
			}
			value = this.itemicontintcolorFormatter.Format(v);
			return true;
		}
		if (!(bindingName == "level"))
		{
			return false;
		}
		if (flag)
		{
			value = this.UnlockData.GetLevel(base.xui.playerUI.entityPlayer, this.Recipe.GetOutputItemClass().Name);
		}
		else
		{
			value = "";
		}
		return true;
	}

	// Token: 0x060075FB RID: 30203 RVA: 0x00300D98 File Offset: 0x002FEF98
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		this.isDirty = true;
	}

	// Token: 0x060075FC RID: 30204 RVA: 0x00300DA1 File Offset: 0x002FEFA1
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			base.RefreshBindings(false);
			base.ViewComponent.IsVisible = true;
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x040059FD RID: 23037
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x040059FF RID: 23039
	[PublicizedFrom(EAccessModifier.Private)]
	public RecipeUnlockData unlockData;

	// Token: 0x04005A00 RID: 23040
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemicontintcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04005A01 RID: 23041
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt havecountFormatter = new CachedStringFormatterInt();

	// Token: 0x04005A02 RID: 23042
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt needcountFormatter = new CachedStringFormatterInt();
}

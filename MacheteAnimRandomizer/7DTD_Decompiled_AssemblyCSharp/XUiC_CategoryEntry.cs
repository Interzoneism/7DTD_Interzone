using System;
using UnityEngine.Scripting;

// Token: 0x02000C1F RID: 3103
[Preserve]
public class XUiC_CategoryEntry : XUiController
{
	// Token: 0x170009D2 RID: 2514
	// (get) Token: 0x06005F45 RID: 24389 RVA: 0x0026A5B8 File Offset: 0x002687B8
	// (set) Token: 0x06005F46 RID: 24390 RVA: 0x0026A5C0 File Offset: 0x002687C0
	public XUiC_CategoryList CategoryList { get; set; }

	// Token: 0x170009D3 RID: 2515
	// (get) Token: 0x06005F47 RID: 24391 RVA: 0x0026A5C9 File Offset: 0x002687C9
	// (set) Token: 0x06005F48 RID: 24392 RVA: 0x0026A5D1 File Offset: 0x002687D1
	public string CategoryName
	{
		get
		{
			return this.categoryName;
		}
		set
		{
			this.categoryName = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x170009D4 RID: 2516
	// (get) Token: 0x06005F49 RID: 24393 RVA: 0x0026A5E1 File Offset: 0x002687E1
	// (set) Token: 0x06005F4A RID: 24394 RVA: 0x0026A5E9 File Offset: 0x002687E9
	public string CategoryDisplayName
	{
		get
		{
			return this.categoryDisplayName;
		}
		set
		{
			this.categoryDisplayName = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x170009D5 RID: 2517
	// (get) Token: 0x06005F4B RID: 24395 RVA: 0x0026A5F9 File Offset: 0x002687F9
	// (set) Token: 0x06005F4C RID: 24396 RVA: 0x0026A601 File Offset: 0x00268801
	public string SpriteName
	{
		get
		{
			return this.spriteName;
		}
		set
		{
			this.spriteName = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x170009D6 RID: 2518
	// (get) Token: 0x06005F4D RID: 24397 RVA: 0x0026A611 File Offset: 0x00268811
	// (set) Token: 0x06005F4E RID: 24398 RVA: 0x0026A619 File Offset: 0x00268819
	public new bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			this.selected = value;
			this.button.Selected = this.selected;
		}
	}

	// Token: 0x06005F4F RID: 24399 RVA: 0x0026A633 File Offset: 0x00268833
	public override void Init()
	{
		base.Init();
		this.button = (XUiV_Button)base.ViewComponent;
		base.OnPress += this.XUiC_CategoryEntry_OnPress;
		this.IsDirty = true;
	}

	// Token: 0x06005F50 RID: 24400 RVA: 0x0026A668 File Offset: 0x00268868
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiC_CategoryEntry_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.spriteName != string.Empty)
		{
			if (this.CategoryList.CurrentCategory == this && this.CategoryList.AllowUnselect)
			{
				this.CategoryList.CurrentCategory = null;
			}
			else
			{
				this.CategoryList.CurrentCategory = this;
			}
			this.CategoryList.HandleCategoryChanged();
		}
	}

	// Token: 0x06005F51 RID: 24401 RVA: 0x0026A6C7 File Offset: 0x002688C7
	public void PlayButtonClickSound()
	{
		this.button.PlayClickSound();
	}

	// Token: 0x06005F52 RID: 24402 RVA: 0x0026A6D4 File Offset: 0x002688D4
	public override void Update(float _dt)
	{
		if (this.IsDirty)
		{
			base.ViewComponent.IsNavigatable = !string.IsNullOrEmpty(this.SpriteName);
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x06005F53 RID: 24403 RVA: 0x0026A70C File Offset: 0x0026890C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "categoryicon")
		{
			value = this.spriteName;
			return true;
		}
		if (!(bindingName == "categorydisplayname"))
		{
			return false;
		}
		value = this.categoryDisplayName;
		return true;
	}

	// Token: 0x06005F54 RID: 24404 RVA: 0x0026A740 File Offset: 0x00268940
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "categoryname")
		{
			if (!string.IsNullOrEmpty(_value))
			{
				this.CategoryName = _value;
			}
			return true;
		}
		if (_name == "spritename")
		{
			if (!string.IsNullOrEmpty(_value))
			{
				this.SpriteName = _value;
			}
			return true;
		}
		if (_name == "displayname")
		{
			if (!string.IsNullOrEmpty(_value))
			{
				this.CategoryDisplayName = _value;
			}
			return true;
		}
		if (!(_name == "displayname_key"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		if (!string.IsNullOrEmpty(_value))
		{
			this.CategoryDisplayName = Localization.Get(_value, false);
		}
		return true;
	}

	// Token: 0x040047DC RID: 18396
	[PublicizedFrom(EAccessModifier.Private)]
	public string categoryName = "";

	// Token: 0x040047DD RID: 18397
	[PublicizedFrom(EAccessModifier.Private)]
	public string categoryDisplayName = "";

	// Token: 0x040047DE RID: 18398
	[PublicizedFrom(EAccessModifier.Private)]
	public string spriteName = "";

	// Token: 0x040047DF RID: 18399
	[PublicizedFrom(EAccessModifier.Private)]
	public bool selected;

	// Token: 0x040047E0 RID: 18400
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button button;
}

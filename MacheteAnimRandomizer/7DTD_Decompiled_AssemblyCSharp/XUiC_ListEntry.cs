using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D01 RID: 3329
[Preserve]
public class XUiC_ListEntry<T> : XUiController where T : XUiListEntry<T>
{
	// Token: 0x06006786 RID: 26502 RVA: 0x0029F814 File Offset: 0x0029DA14
	[PublicizedFrom(EAccessModifier.Private)]
	static XUiC_ListEntry()
	{
		MethodInfo method = typeof(T).GetMethod("GetNullBindingValues", BindingFlags.Static | BindingFlags.Public, null, new Type[]
		{
			typeof(string).MakeByRefType(),
			typeof(string)
		}, null);
		if (method != null)
		{
			XUiC_ListEntry<T>.nullBindings = (Delegate.CreateDelegate(typeof(XUiC_ListEntry<T>.NullBindingDelegate), method) as XUiC_ListEntry<T>.NullBindingDelegate);
			return;
		}
		Log.Warning("[XUi] List entry type \"" + typeof(T).FullName + "\" does not have a static GetNullBindingValues method");
	}

	// Token: 0x17000A99 RID: 2713
	// (get) Token: 0x06006787 RID: 26503 RVA: 0x0029F8A6 File Offset: 0x0029DAA6
	public bool HasEntry
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.entryData != null;
		}
	}

	// Token: 0x17000A9A RID: 2714
	// (get) Token: 0x06006788 RID: 26504 RVA: 0x0029F8B6 File Offset: 0x0029DAB6
	// (set) Token: 0x06006789 RID: 26505 RVA: 0x0029F8C0 File Offset: 0x0029DAC0
	public new bool Selected
	{
		get
		{
			return this.selected;
		}
		set
		{
			if (value)
			{
				if (this.List.SelectedEntry != null)
				{
					this.List.SelectedEntry.SelectedChanged(false);
					this.List.SelectedEntry.selected = false;
				}
			}
			else if (this.List.SelectedEntry == this)
			{
				this.SelectedChanged(false);
				this.selected = false;
				this.List.ClearSelection();
			}
			this.selected = value;
			if (this.selected)
			{
				this.List.SelectedEntry = this;
			}
			this.SelectedChanged(this.selected);
		}
	}

	// Token: 0x17000A9B RID: 2715
	// (get) Token: 0x0600678A RID: 26506 RVA: 0x0029F94F File Offset: 0x0029DB4F
	// (set) Token: 0x0600678B RID: 26507 RVA: 0x0029F957 File Offset: 0x0029DB57
	public bool ForceHovered
	{
		get
		{
			return this.forceHovered;
		}
		set
		{
			if (value != this.forceHovered)
			{
				this.forceHovered = value;
				this.updateHoveredEffect();
			}
		}
	}

	// Token: 0x0600678C RID: 26508 RVA: 0x0029F970 File Offset: 0x0029DB70
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SelectedChanged(bool isSelected)
	{
		if (this.background != null)
		{
			this.background.Color = (isSelected ? this.bgColorSelected : this.bgColorUnselected);
			this.background.SpriteName = (isSelected ? this.bgSpriteNameSelected : this.bgSpriteNameUnselected);
		}
	}

	// Token: 0x0600678D RID: 26509 RVA: 0x0029F9C4 File Offset: 0x0029DBC4
	public override void Init()
	{
		base.Init();
		for (int i = 0; i < this.children.Count; i++)
		{
			XUiView viewComponent = this.children[i].ViewComponent;
			if (viewComponent.ID.EqualsCaseInsensitive("background"))
			{
				this.background = (viewComponent as XUiV_Sprite);
			}
		}
		base.OnPress += this.XUiC_ListEntry_OnPress;
		base.ViewComponent.Enabled = this.HasEntry;
		this.IsDirty = true;
	}

	// Token: 0x0600678E RID: 26510 RVA: 0x0029FA47 File Offset: 0x0029DC47
	public void XUiC_ListEntry_OnPress(XUiController _sender, int _mouseButton)
	{
		if (!base.ViewComponent.Enabled)
		{
			return;
		}
		if (!this.Selected)
		{
			this.Selected = true;
		}
		this.List.OnListEntryClicked(this);
	}

	// Token: 0x0600678F RID: 26511 RVA: 0x0029FA74 File Offset: 0x0029DC74
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateHoveredEffect()
	{
		if (this.background != null && this.HasEntry && !this.Selected)
		{
			if (this.forceHovered || this.isHovered)
			{
				this.background.Color = this.bgColorHovered;
				return;
			}
			this.background.Color = this.bgColorUnselected;
		}
	}

	// Token: 0x06006790 RID: 26512 RVA: 0x0029FAD6 File Offset: 0x0029DCD6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isHovered = _isOver;
		this.updateHoveredEffect();
		base.RefreshBindings(false);
		base.OnHovered(_isOver);
	}

	// Token: 0x06006791 RID: 26513 RVA: 0x0007FB71 File Offset: 0x0007DD71
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
	}

	// Token: 0x06006792 RID: 26514 RVA: 0x0029FAF4 File Offset: 0x0029DCF4
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (base.ParseAttribute(name, value, _parent))
		{
			return true;
		}
		if (!(name == "background_color_unselected"))
		{
			if (!(name == "background_color_hovered"))
			{
				if (!(name == "background_color_selected"))
				{
					if (!(name == "background_sprite_unselected"))
					{
						if (!(name == "background_sprite_selected"))
						{
							return false;
						}
						this.bgSpriteNameSelected = value;
					}
					else
					{
						this.bgSpriteNameUnselected = value;
					}
				}
				else
				{
					this.bgColorSelected = StringParsers.ParseColor32(value);
				}
			}
			else
			{
				this.bgColorHovered = StringParsers.ParseColor32(value);
			}
		}
		else
		{
			this.bgColorUnselected = StringParsers.ParseColor32(value);
		}
		return true;
	}

	// Token: 0x06006793 RID: 26515 RVA: 0x0029FBA0 File Offset: 0x0029DDA0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "hasentry")
		{
			_value = (this.entryData != null).ToString();
			return true;
		}
		if (_bindingName == "hovered")
		{
			_value = this.isHovered.ToString();
			return true;
		}
		if (this.entryData != null)
		{
			return this.entryData.GetBindingValue(ref _value, _bindingName);
		}
		return XUiC_ListEntry<T>.nullBindings != null && XUiC_ListEntry<T>.nullBindings(ref _value, _bindingName);
	}

	// Token: 0x06006794 RID: 26516 RVA: 0x0029FC28 File Offset: 0x0029DE28
	public virtual void SetEntry(T _data)
	{
		if (_data != this.entryData)
		{
			this.entryData = _data;
			base.ViewComponent.Enabled = this.HasEntry;
			if ((!this.Selected || !this.HasEntry) && this.background != null)
			{
				this.background.Color = this.bgColorUnselected;
			}
		}
		base.ViewComponent.IsNavigatable = (base.ViewComponent.IsSnappable = this.HasEntry);
		this.IsDirty = true;
	}

	// Token: 0x06006795 RID: 26517 RVA: 0x0029FCB4 File Offset: 0x0029DEB4
	public T GetEntry()
	{
		return this.entryData;
	}

	// Token: 0x04004E19 RID: 19993
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isHovered;

	// Token: 0x04004E1A RID: 19994
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite background;

	// Token: 0x04004E1B RID: 19995
	public XUiC_List<T> List;

	// Token: 0x04004E1C RID: 19996
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 bgColorUnselected = new Color32(64, 64, 64, byte.MaxValue);

	// Token: 0x04004E1D RID: 19997
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 bgColorHovered = new Color32(96, 96, 96, byte.MaxValue);

	// Token: 0x04004E1E RID: 19998
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 bgColorSelected = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004E1F RID: 19999
	[PublicizedFrom(EAccessModifier.Protected)]
	public string bgSpriteNameUnselected = "menu_empty";

	// Token: 0x04004E20 RID: 20000
	[PublicizedFrom(EAccessModifier.Protected)]
	public string bgSpriteNameSelected = "ui_game_select_row";

	// Token: 0x04004E21 RID: 20001
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_ListEntry<T>.NullBindingDelegate nullBindings;

	// Token: 0x04004E22 RID: 20002
	[PublicizedFrom(EAccessModifier.Private)]
	public T entryData;

	// Token: 0x04004E23 RID: 20003
	[PublicizedFrom(EAccessModifier.Private)]
	public bool selected;

	// Token: 0x04004E24 RID: 20004
	[PublicizedFrom(EAccessModifier.Private)]
	public bool forceHovered;

	// Token: 0x02000D02 RID: 3330
	// (Invoke) Token: 0x06006798 RID: 26520
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate bool NullBindingDelegate(ref string _value, string _bindingName);
}

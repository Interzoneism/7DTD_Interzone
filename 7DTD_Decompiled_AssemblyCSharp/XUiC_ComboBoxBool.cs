using System;
using UnityEngine.Scripting;

// Token: 0x02000C56 RID: 3158
[Preserve]
public class XUiC_ComboBoxBool : XUiC_ComboBox<bool>
{
	// Token: 0x170009FE RID: 2558
	// (get) Token: 0x06006129 RID: 24873 RVA: 0x00276A09 File Offset: 0x00274C09
	// (set) Token: 0x0600612A RID: 24874 RVA: 0x00276A11 File Offset: 0x00274C11
	public override bool Value
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			if (this.currentValue != value)
			{
				this.currentValue = value;
				this.IsDirty = true;
				this.UpdateLabel();
			}
		}
	}

	// Token: 0x170009FF RID: 2559
	// (get) Token: 0x0600612B RID: 24875 RVA: 0x000282C0 File Offset: 0x000264C0
	public override int IndexElementCount
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return 2;
		}
	}

	// Token: 0x17000A00 RID: 2560
	// (get) Token: 0x0600612C RID: 24876 RVA: 0x00276A30 File Offset: 0x00274C30
	public override int IndexMarkerIndex
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (!this.currentValue)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x0600612D RID: 24877 RVA: 0x00276A40 File Offset: 0x00274C40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateLabel()
	{
		base.ValueText = ((!string.IsNullOrEmpty(this.LocalizationPrefix)) ? Localization.Get(this.LocalizationPrefix + (this.currentValue ? "On" : "Off"), false) : (this.currentValue ? "Yes" : "No"));
	}

	// Token: 0x0600612E RID: 24878 RVA: 0x00276A9B File Offset: 0x00274C9B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDifferentValue(bool _oldVal, bool _currentValue)
	{
		return _oldVal != _currentValue;
	}

	// Token: 0x0600612F RID: 24879 RVA: 0x00276AA4 File Offset: 0x00274CA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void BackPressed()
	{
		this.currentValue = !this.currentValue;
	}

	// Token: 0x06006130 RID: 24880 RVA: 0x00276AA4 File Offset: 0x00274CA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ForwardPressed()
	{
		this.currentValue = !this.currentValue;
	}

	// Token: 0x06006131 RID: 24881 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMax()
	{
		return false;
	}

	// Token: 0x06006132 RID: 24882 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMin()
	{
		return false;
	}

	// Token: 0x06006133 RID: 24883 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isEmpty()
	{
		return false;
	}

	// Token: 0x06006134 RID: 24884 RVA: 0x00276AB5 File Offset: 0x00274CB5
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "localization_prefix")
		{
			this.LocalizationPrefix = _value;
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x06006135 RID: 24885 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setRelativeValue(double _value)
	{
	}

	// Token: 0x06006136 RID: 24886 RVA: 0x00276AD6 File Offset: 0x00274CD6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void incrementalChangeValue(double _value)
	{
		base.ForwardButton_OnPress(this, -1);
	}

	// Token: 0x0400491E RID: 18718
	public string LocalizationPrefix;
}

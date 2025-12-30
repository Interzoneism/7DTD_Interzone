using System;
using UnityEngine.Scripting;

// Token: 0x02000C51 RID: 3153
[Preserve]
public class XUiC_ComboBoxEnum<TEnum> : XUiC_ComboBox<TEnum> where TEnum : struct, IConvertible
{
	// Token: 0x170009F8 RID: 2552
	// (get) Token: 0x060060F6 RID: 24822 RVA: 0x00275FF4 File Offset: 0x002741F4
	// (set) Token: 0x060060F7 RID: 24823 RVA: 0x00275FFC File Offset: 0x002741FC
	public override TEnum Value
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			if (this.currentValue.Ordinal<TEnum>() != value.Ordinal<TEnum>())
			{
				this.currentValue = value;
				this.IsDirty = true;
				this.UpdateLabel();
			}
		}
	}

	// Token: 0x170009F9 RID: 2553
	// (get) Token: 0x060060F8 RID: 24824 RVA: 0x000ECF59 File Offset: 0x000EB159
	public override int IndexElementCount
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return -1;
		}
	}

	// Token: 0x170009FA RID: 2554
	// (get) Token: 0x060060F9 RID: 24825 RVA: 0x000ECF59 File Offset: 0x000EB159
	public override int IndexMarkerIndex
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return -1;
		}
	}

	// Token: 0x060060FA RID: 24826 RVA: 0x00276025 File Offset: 0x00274225
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateLabel()
	{
		base.ValueText = ((!string.IsNullOrEmpty(this.LocalizationPrefix)) ? Localization.Get(this.LocalizationPrefix + this.currentValue.ToStringCached<TEnum>(), false) : this.currentValue.ToStringCached<TEnum>());
	}

	// Token: 0x060060FB RID: 24827 RVA: 0x00276063 File Offset: 0x00274263
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDifferentValue(TEnum _oldVal, TEnum _currentValue)
	{
		return _oldVal.Ordinal<TEnum>() != _currentValue.Ordinal<TEnum>();
	}

	// Token: 0x060060FC RID: 24828 RVA: 0x00276078 File Offset: 0x00274278
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void BackPressed()
	{
		if (this.MinSet && this.MaxSet)
		{
			this.currentValue = this.currentValue.CycleEnum(this.Min, this.Max, true, this.Wrap);
			return;
		}
		this.currentValue = this.currentValue.CycleEnum(true, this.Wrap);
	}

	// Token: 0x060060FD RID: 24829 RVA: 0x002760D4 File Offset: 0x002742D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ForwardPressed()
	{
		if (this.MinSet && this.MaxSet)
		{
			this.currentValue = this.currentValue.CycleEnum(this.Min, this.Max, false, this.Wrap);
			return;
		}
		this.currentValue = this.currentValue.CycleEnum(false, this.Wrap);
	}

	// Token: 0x060060FE RID: 24830 RVA: 0x00276130 File Offset: 0x00274330
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMax()
	{
		if (!this.MinSet || !this.MaxSet)
		{
			return this.currentValue.Ordinal<TEnum>() == EnumUtils.MaxValue<TEnum>().Ordinal<TEnum>();
		}
		return this.currentValue.Ordinal<TEnum>() == this.Max.Ordinal<TEnum>();
	}

	// Token: 0x060060FF RID: 24831 RVA: 0x00276180 File Offset: 0x00274380
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMin()
	{
		if (!this.MinSet || !this.MaxSet)
		{
			return this.currentValue.Ordinal<TEnum>() == EnumUtils.MinValue<TEnum>().Ordinal<TEnum>();
		}
		return this.currentValue.Ordinal<TEnum>() == this.Min.Ordinal<TEnum>();
	}

	// Token: 0x06006100 RID: 24832 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isEmpty()
	{
		return false;
	}

	// Token: 0x06006101 RID: 24833 RVA: 0x002761D0 File Offset: 0x002743D0
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "value_min")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				this.Min = EnumUtils.Parse<TEnum>(_value, false);
				this.MinSet = true;
			}
			return true;
		}
		if (_name == "value_max")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				this.Max = EnumUtils.Parse<TEnum>(_value, false);
				this.MaxSet = true;
			}
			return true;
		}
		if (!(_name == "localization_prefix"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.LocalizationPrefix = _value;
		return true;
	}

	// Token: 0x06006102 RID: 24834 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setRelativeValue(double _value)
	{
	}

	// Token: 0x06006103 RID: 24835 RVA: 0x0027625E File Offset: 0x0027445E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void incrementalChangeValue(double _value)
	{
		if (_value > 0.0)
		{
			base.ForwardButton_OnPress(this, -1);
			return;
		}
		if (_value < 0.0)
		{
			base.BackButton_OnPress(this, -1);
		}
	}

	// Token: 0x06006104 RID: 24836 RVA: 0x00276289 File Offset: 0x00274489
	public void SetMinMax(TEnum _min, TEnum _max)
	{
		this.Min = _min;
		this.Max = _max;
		this.MinSet = true;
		this.MaxSet = true;
	}

	// Token: 0x04004916 RID: 18710
	public string LocalizationPrefix;

	// Token: 0x04004917 RID: 18711
	[PublicizedFrom(EAccessModifier.Private)]
	public bool MinSet;

	// Token: 0x04004918 RID: 18712
	[PublicizedFrom(EAccessModifier.Private)]
	public bool MaxSet;
}

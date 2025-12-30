using System;
using UnityEngine.Scripting;

// Token: 0x02000C52 RID: 3154
[Preserve]
public abstract class XUiC_ComboBoxOrdinal<TValue> : XUiC_ComboBox<TValue> where TValue : struct, IEquatable<TValue>, IComparable<TValue>, IFormattable, IConvertible
{
	// Token: 0x170009FB RID: 2555
	// (get) Token: 0x06006106 RID: 24838 RVA: 0x00275FF4 File Offset: 0x002741F4
	// (set) Token: 0x06006107 RID: 24839 RVA: 0x002762B0 File Offset: 0x002744B0
	public override TValue Value
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			if (!this.currentValue.Equals(value))
			{
				if (value.CompareTo(this.Max) > 0)
				{
					value = this.Max;
				}
				else if (value.CompareTo(this.Min) < 0)
				{
					value = this.Min;
				}
				this.currentValue = value;
				this.IsDirty = true;
				this.UpdateLabel();
			}
		}
	}

	// Token: 0x170009FC RID: 2556
	// (get) Token: 0x06006108 RID: 24840 RVA: 0x000ECF59 File Offset: 0x000EB159
	public override int IndexElementCount
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return -1;
		}
	}

	// Token: 0x170009FD RID: 2557
	// (get) Token: 0x06006109 RID: 24841 RVA: 0x000ECF59 File Offset: 0x000EB159
	public override int IndexMarkerIndex
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return -1;
		}
	}

	// Token: 0x0600610A RID: 24842 RVA: 0x00276324 File Offset: 0x00274524
	public override void OnOpen()
	{
		if (this.currentValue.CompareTo(this.Max) > 0)
		{
			this.Value = this.Max;
		}
		else if (this.currentValue.CompareTo(this.Min) < 0)
		{
			this.Value = this.Min;
		}
		base.OnOpen();
	}

	// Token: 0x0600610B RID: 24843 RVA: 0x00276388 File Offset: 0x00274588
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateLabel()
	{
		if (this.CustomValueFormatter != null)
		{
			base.ValueText = this.CustomValueFormatter(this.currentValue);
			return;
		}
		base.ValueText = this.currentValue.ToString(this.FormatString, Utils.StandardCulture);
	}

	// Token: 0x0600610C RID: 24844 RVA: 0x002763D7 File Offset: 0x002745D7
	public void UpdateLabel(string _text)
	{
		base.ValueText = _text;
	}

	// Token: 0x0600610D RID: 24845 RVA: 0x002763E0 File Offset: 0x002745E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDifferentValue(TValue _oldVal, TValue _currentValue)
	{
		return !_oldVal.Equals(_currentValue);
	}

	// Token: 0x0600610E RID: 24846 RVA: 0x002763F3 File Offset: 0x002745F3
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void BackPressed()
	{
		if (this.currentValue.CompareTo(this.Min) < 0)
		{
			this.currentValue = (this.Wrap ? this.Max : this.Min);
		}
	}

	// Token: 0x0600610F RID: 24847 RVA: 0x0027642B File Offset: 0x0027462B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ForwardPressed()
	{
		if (this.currentValue.CompareTo(this.Max) > 0)
		{
			this.currentValue = (this.Wrap ? this.Min : this.Max);
		}
	}

	// Token: 0x06006110 RID: 24848 RVA: 0x00276463 File Offset: 0x00274663
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMax()
	{
		return this.currentValue.CompareTo(this.Max) == 0;
	}

	// Token: 0x06006111 RID: 24849 RVA: 0x0027647F File Offset: 0x0027467F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMin()
	{
		return this.currentValue.CompareTo(this.Min) == 0;
	}

	// Token: 0x06006112 RID: 24850 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isEmpty()
	{
		return false;
	}

	// Token: 0x06006113 RID: 24851 RVA: 0x0027649B File Offset: 0x0027469B
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "format_string")
		{
			this.FormatString = (string.IsNullOrEmpty(_value) ? null : _value);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x06006114 RID: 24852 RVA: 0x002764C8 File Offset: 0x002746C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "isnumber")
		{
			_value = true.ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06006115 RID: 24853 RVA: 0x002762A7 File Offset: 0x002744A7
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_ComboBoxOrdinal()
	{
	}

	// Token: 0x04004919 RID: 18713
	public string FormatString;

	// Token: 0x0400491A RID: 18714
	public XUiC_ComboBoxOrdinal<TValue>.CustomValueFormatterDelegate CustomValueFormatter;

	// Token: 0x02000C53 RID: 3155
	// (Invoke) Token: 0x06006117 RID: 24855
	public delegate string CustomValueFormatterDelegate(TValue _value);
}

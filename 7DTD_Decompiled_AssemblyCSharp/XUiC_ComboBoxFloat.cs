using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000C55 RID: 3157
[Preserve]
public class XUiC_ComboBoxFloat : XUiC_ComboBoxOrdinal<double>
{
	// Token: 0x06006121 RID: 24865 RVA: 0x0027676B File Offset: 0x0027496B
	public XUiC_ComboBoxFloat()
	{
		this.Max = 1.0;
		this.Min = 0.0;
	}

	// Token: 0x06006122 RID: 24866 RVA: 0x002767A0 File Offset: 0x002749A0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void BackPressed()
	{
		this.currentValue -= this.IncrementSize;
		base.BackPressed();
	}

	// Token: 0x06006123 RID: 24867 RVA: 0x002767BB File Offset: 0x002749BB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ForwardPressed()
	{
		this.currentValue += this.IncrementSize;
		base.ForwardPressed();
	}

	// Token: 0x06006124 RID: 24868 RVA: 0x002767D8 File Offset: 0x002749D8
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "value_min")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				this.Min = StringParsers.ParseDouble(_value, 0, -1, NumberStyles.Any);
			}
			return true;
		}
		if (_name == "value_max")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				this.Max = StringParsers.ParseDouble(_value, 0, -1, NumberStyles.Any);
			}
			return true;
		}
		if (!(_name == "value_increment"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		if (!_value.EqualsCaseInsensitive("@def"))
		{
			this.IncrementSize = StringParsers.ParseDouble(_value, 0, -1, NumberStyles.Any);
		}
		return true;
	}

	// Token: 0x06006125 RID: 24869 RVA: 0x0027687D File Offset: 0x00274A7D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "fillvalue")
		{
			_value = ((this.currentValue - this.Min) / (this.Max - this.Min)).ToCultureInvariantString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06006126 RID: 24870 RVA: 0x002768B8 File Offset: 0x00274AB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setRelativeValue(double _value)
	{
		double value = this.Value;
		this.Value = (this.Max - this.Min) * _value + this.Min;
		base.TriggerValueChangedEvent(value);
	}

	// Token: 0x06006127 RID: 24871 RVA: 0x002768F0 File Offset: 0x00274AF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void incrementalChangeValue(double _value)
	{
		double num = (this.Max - this.Min) * _value * 0.5;
		if (num > this.IncrementSize)
		{
			num = this.IncrementSize;
		}
		else if (num < -this.IncrementSize)
		{
			num = -this.IncrementSize;
		}
		double value = this.Value;
		double num2 = this.Value + num;
		if (_value < 0.0 && num2 < this.Min && this.Wrap)
		{
			num2 = this.Max;
		}
		else if (_value > 0.0 && num2 > this.Max && this.Wrap)
		{
			num2 = this.Min;
		}
		this.Value = num2;
		base.TriggerValueChangedEvent(value);
	}

	// Token: 0x06006128 RID: 24872 RVA: 0x002769A4 File Offset: 0x00274BA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool handleSegmentedFillValueBinding(ref string _value, int _index)
	{
		double num = (this.Max - this.Min) / (double)base.SegmentedFillCount;
		double num2 = (double)_index * num;
		double num3 = (double)(_index + 1) * num;
		if (this.currentValue <= num2)
		{
			_value = "0";
		}
		else if (this.currentValue >= num3)
		{
			_value = "1";
		}
		else
		{
			_value = ((this.currentValue - num2) / num).ToCultureInvariantString();
		}
		return true;
	}

	// Token: 0x0400491D RID: 18717
	public double IncrementSize = 1.0;
}

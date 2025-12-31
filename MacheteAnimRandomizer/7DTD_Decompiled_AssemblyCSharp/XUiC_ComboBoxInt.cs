using System;
using System.Globalization;
using UnityEngine.Scripting;

// Token: 0x02000C54 RID: 3156
[Preserve]
public class XUiC_ComboBoxInt : XUiC_ComboBoxOrdinal<long>
{
	// Token: 0x0600611A RID: 24858 RVA: 0x002764F7 File Offset: 0x002746F7
	public XUiC_ComboBoxInt()
	{
		this.Max = long.MaxValue;
		this.Min = long.MinValue;
	}

	// Token: 0x0600611B RID: 24859 RVA: 0x00276531 File Offset: 0x00274731
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void BackPressed()
	{
		this.currentValue -= this.IncrementSize;
		base.BackPressed();
	}

	// Token: 0x0600611C RID: 24860 RVA: 0x0027654C File Offset: 0x0027474C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ForwardPressed()
	{
		this.currentValue += this.IncrementSize;
		base.ForwardPressed();
	}

	// Token: 0x0600611D RID: 24861 RVA: 0x00276568 File Offset: 0x00274768
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "value_min")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				this.Min = StringParsers.ParseSInt64(_value, 0, -1, NumberStyles.Integer);
			}
			return true;
		}
		if (_name == "value_max")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				this.Max = StringParsers.ParseSInt64(_value, 0, -1, NumberStyles.Integer);
			}
			return true;
		}
		if (!(_name == "value_increment"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		if (!_value.EqualsCaseInsensitive("@def"))
		{
			this.IncrementSize = StringParsers.ParseSInt64(_value, 0, -1, NumberStyles.Integer);
		}
		return true;
	}

	// Token: 0x0600611E RID: 24862 RVA: 0x00276604 File Offset: 0x00274804
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "fillvalue")
		{
			_value = this.fillvalueFormatter.Format(((float)this.currentValue - (float)this.Min) / (float)(this.Max - this.Min));
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x0600611F RID: 24863 RVA: 0x00276654 File Offset: 0x00274854
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setRelativeValue(double _value)
	{
		long value = this.Value;
		this.Value = (long)((double)(this.Max - this.Min) * _value) + this.Min;
		base.TriggerValueChangedEvent(value);
	}

	// Token: 0x06006120 RID: 24864 RVA: 0x00276690 File Offset: 0x00274890
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void incrementalChangeValue(double _value)
	{
		long num = (long)((double)(this.Max - this.Min) * _value * 0.5);
		if (_value > 0.0 && num == 0L)
		{
			num = 1L;
		}
		else if (_value < 0.0 && num == 0L)
		{
			num = -1L;
		}
		if (num > this.IncrementSize)
		{
			num = this.IncrementSize;
		}
		else if (num < -this.IncrementSize)
		{
			num = -this.IncrementSize;
		}
		long value = this.Value;
		long num2 = this.Value + num;
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

	// Token: 0x0400491B RID: 18715
	public long IncrementSize = 1L;

	// Token: 0x0400491C RID: 18716
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat fillvalueFormatter = new CachedStringFormatterFloat(null);
}

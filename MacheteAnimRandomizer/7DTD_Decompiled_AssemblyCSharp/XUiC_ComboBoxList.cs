using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000C57 RID: 3159
[Preserve]
public class XUiC_ComboBoxList<TElement> : XUiC_ComboBox<TElement>
{
	// Token: 0x17000A01 RID: 2561
	// (get) Token: 0x06006138 RID: 24888 RVA: 0x00275FF4 File Offset: 0x002741F4
	// (set) Token: 0x06006139 RID: 24889 RVA: 0x00276AE8 File Offset: 0x00274CE8
	public override TElement Value
	{
		get
		{
			return this.currentValue;
		}
		set
		{
			if (this.Elements.Contains(value))
			{
				int selectedIndex = this.Elements.IndexOf(value);
				this.SelectedIndex = selectedIndex;
				this.useCustomValue = false;
				return;
			}
			if (value != null)
			{
				this.CustomValue = value;
				this.useCustomValue = true;
				this.SelectedIndex = -1;
			}
		}
	}

	// Token: 0x17000A02 RID: 2562
	// (get) Token: 0x0600613A RID: 24890 RVA: 0x00276B3C File Offset: 0x00274D3C
	public override int IndexElementCount
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.MaxIndex - this.MinIndex + 1;
		}
	}

	// Token: 0x17000A03 RID: 2563
	// (get) Token: 0x0600613B RID: 24891 RVA: 0x00276B4D File Offset: 0x00274D4D
	public override int IndexMarkerIndex
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (!this.ReverseList)
			{
				return this.SelectedIndex - this.MinIndex;
			}
			return this.MaxIndex - this.SelectedIndex;
		}
	}

	// Token: 0x17000A04 RID: 2564
	// (get) Token: 0x0600613C RID: 24892 RVA: 0x00276B72 File Offset: 0x00274D72
	// (set) Token: 0x0600613D RID: 24893 RVA: 0x00276B7C File Offset: 0x00274D7C
	public int SelectedIndex
	{
		get
		{
			return this.currentIndex;
		}
		set
		{
			if (value < 0)
			{
				if (this.useCustomValue)
				{
					this.currentIndex = value;
					this.currentValue = this.CustomValue;
				}
				else
				{
					this.currentIndex = this.minIndex;
					this.currentValue = this.Elements[this.currentIndex];
				}
			}
			else
			{
				if (value >= this.Elements.Count)
				{
					value = this.Elements.Count - 1;
				}
				if (value < this.minIndex)
				{
					value = this.minIndex;
				}
				else if (value > this.maxIndex)
				{
					value = this.maxIndex;
				}
				this.currentIndex = value;
				this.currentValue = this.Elements[this.currentIndex];
			}
			this.IsDirty = true;
			this.UpdateLabel();
		}
	}

	// Token: 0x17000A05 RID: 2565
	// (get) Token: 0x0600613E RID: 24894 RVA: 0x00276C3E File Offset: 0x00274E3E
	// (set) Token: 0x0600613F RID: 24895 RVA: 0x00276C51 File Offset: 0x00274E51
	public int MinIndex
	{
		get
		{
			if (this.minIndex >= 0)
			{
				return this.minIndex;
			}
			return 0;
		}
		set
		{
			if (value != this.minIndex)
			{
				this.minIndex = value;
				if (this.currentIndex < this.minIndex)
				{
					this.SelectedIndex = this.minIndex;
				}
				base.UpdateIndexMarkerPositions();
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000A06 RID: 2566
	// (get) Token: 0x06006140 RID: 24896 RVA: 0x00276C8A File Offset: 0x00274E8A
	// (set) Token: 0x06006141 RID: 24897 RVA: 0x00276CB3 File Offset: 0x00274EB3
	public int MaxIndex
	{
		get
		{
			if (this.maxIndex < this.Elements.Count)
			{
				return this.maxIndex;
			}
			return this.Elements.Count - 1;
		}
		set
		{
			if (value != this.maxIndex)
			{
				this.maxIndex = value;
				if (this.currentIndex > this.maxIndex)
				{
					this.SelectedIndex = this.maxIndex;
				}
				base.UpdateIndexMarkerPositions();
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x06006142 RID: 24898 RVA: 0x00276CEC File Offset: 0x00274EEC
	public override void OnOpen()
	{
		if (this.Elements.Count > 0 && !this.useCustomValue)
		{
			if (this.currentIndex < 0)
			{
				this.SelectedIndex = 0;
			}
			if (this.currentIndex > this.Elements.Count)
			{
				this.SelectedIndex = this.Elements.Count - 1;
			}
			if (this.currentIndex < this.minIndex)
			{
				this.SelectedIndex = this.minIndex;
			}
			if (this.currentIndex > this.maxIndex)
			{
				this.SelectedIndex = this.maxIndex;
			}
		}
		base.OnOpen();
	}

	// Token: 0x06006143 RID: 24899 RVA: 0x00276D80 File Offset: 0x00274F80
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateLabel()
	{
		if (this.isEmpty())
		{
			base.ValueText = "";
			return;
		}
		base.ValueText = ((!string.IsNullOrEmpty(this.LocalizationPrefix)) ? Localization.Get(this.LocalizationPrefix + this.currentValue.ToString(), this.LocalizationKeyCaseInsensitive) : this.currentValue.ToString());
	}

	// Token: 0x06006144 RID: 24900 RVA: 0x00276DEE File Offset: 0x00274FEE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isDifferentValue(TElement _oldVal, TElement _currentValue)
	{
		return (_oldVal != null || _currentValue != null) && (_oldVal == null || _currentValue == null || !_oldVal.Equals(_currentValue));
	}

	// Token: 0x06006145 RID: 24901 RVA: 0x00276E2A File Offset: 0x0027502A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void BackPressed()
	{
		this.ChangeIndex(this.ReverseList ? 1 : -1);
	}

	// Token: 0x06006146 RID: 24902 RVA: 0x00276E3E File Offset: 0x0027503E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void ForwardPressed()
	{
		this.ChangeIndex(this.ReverseList ? -1 : 1);
	}

	// Token: 0x06006147 RID: 24903 RVA: 0x00276E54 File Offset: 0x00275054
	public void ChangeIndex(int _direction)
	{
		int num = this.currentIndex + _direction;
		if (num < this.minIndex)
		{
			num = (this.Wrap ? Utils.FastMin(this.Elements.Count - 1, this.maxIndex) : this.minIndex);
		}
		if (num > this.maxIndex)
		{
			num = (this.Wrap ? Utils.FastMax(0, this.minIndex) : this.maxIndex);
		}
		if (num < 0)
		{
			num = (this.Wrap ? (this.Elements.Count - 1) : 0);
		}
		if (num >= this.Elements.Count)
		{
			num = (this.Wrap ? 0 : (this.Elements.Count - 1));
		}
		this.SelectedIndex = num;
	}

	// Token: 0x06006148 RID: 24904 RVA: 0x00276F0D File Offset: 0x0027510D
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMax()
	{
		if (!this.ReverseList)
		{
			return this.isMaxIndex();
		}
		return this.isMinIndex();
	}

	// Token: 0x06006149 RID: 24905 RVA: 0x00276F24 File Offset: 0x00275124
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isMin()
	{
		if (!this.ReverseList)
		{
			return this.isMinIndex();
		}
		return this.isMaxIndex();
	}

	// Token: 0x0600614A RID: 24906 RVA: 0x00276F3B File Offset: 0x0027513B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isEmpty()
	{
		return this.Elements.Count == 0;
	}

	// Token: 0x0600614B RID: 24907 RVA: 0x00276F4B File Offset: 0x0027514B
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isMaxIndex()
	{
		return this.currentIndex == this.maxIndex || this.currentIndex == this.Elements.Count - 1;
	}

	// Token: 0x0600614C RID: 24908 RVA: 0x00276F72 File Offset: 0x00275172
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isMinIndex()
	{
		return this.currentIndex == this.minIndex || this.currentIndex == 0;
	}

	// Token: 0x0600614D RID: 24909 RVA: 0x00276F90 File Offset: 0x00275190
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "values")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				string[] array = _value.Split(',', StringSplitOptions.None);
				Type typeFromHandle = typeof(TElement);
				if (typeFromHandle == typeof(string))
				{
					foreach (string text in array)
					{
						this.Elements.Add((TElement)((object)text.Trim()));
					}
				}
				else if (typeof(IConvertible).IsAssignableFrom(typeFromHandle))
				{
					foreach (string text2 in array)
					{
						try
						{
							this.Elements.Add((TElement)((object)Convert.ChangeType(text2, typeFromHandle)));
						}
						catch (Exception e)
						{
							Log.Error(string.Format("[XUi] Value \"{0}\" not supported for the ComboBox type {1}", text2, typeFromHandle));
							Log.Exception(e);
						}
					}
				}
			}
			return true;
		}
		if (_name == "reverse_list")
		{
			if (!_value.EqualsCaseInsensitive("@def"))
			{
				this.ReverseList = StringParsers.ParseBool(_value, 0, -1, true);
			}
			return true;
		}
		if (_name == "localization_prefix")
		{
			this.LocalizationPrefix = _value;
			return true;
		}
		if (!(_name == "localization_key_caseinsensitive"))
		{
			return base.ParseAttribute(_name, _value, _parent);
		}
		this.LocalizationKeyCaseInsensitive = StringParsers.ParseBool(_value, 0, -1, true);
		return true;
	}

	// Token: 0x0600614E RID: 24910 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setRelativeValue(double _value)
	{
	}

	// Token: 0x0600614F RID: 24911 RVA: 0x0027625E File Offset: 0x0027445E
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

	// Token: 0x0400491F RID: 18719
	public string LocalizationPrefix;

	// Token: 0x04004920 RID: 18720
	public bool LocalizationKeyCaseInsensitive;

	// Token: 0x04004921 RID: 18721
	public readonly List<TElement> Elements = new List<TElement>();

	// Token: 0x04004922 RID: 18722
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentIndex = int.MinValue;

	// Token: 0x04004923 RID: 18723
	public TElement CustomValue;

	// Token: 0x04004924 RID: 18724
	[PublicizedFrom(EAccessModifier.Private)]
	public bool useCustomValue;

	// Token: 0x04004925 RID: 18725
	[PublicizedFrom(EAccessModifier.Private)]
	public int minIndex = int.MinValue;

	// Token: 0x04004926 RID: 18726
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxIndex = int.MaxValue;

	// Token: 0x04004927 RID: 18727
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ReverseList;
}

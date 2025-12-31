using System;
using System.Globalization;

// Token: 0x02000F24 RID: 3876
public readonly struct XUiSideSizes
{
	// Token: 0x17000CFF RID: 3327
	// (get) Token: 0x06007BC9 RID: 31689 RVA: 0x00321A31 File Offset: 0x0031FC31
	public int SumLeftRight
	{
		get
		{
			return this.Left + this.Right;
		}
	}

	// Token: 0x17000D00 RID: 3328
	// (get) Token: 0x06007BCA RID: 31690 RVA: 0x00321A40 File Offset: 0x0031FC40
	public int SumTopBottom
	{
		get
		{
			return this.Top + this.Bottom;
		}
	}

	// Token: 0x06007BCB RID: 31691 RVA: 0x00321A4F File Offset: 0x0031FC4F
	public XUiSideSizes(int _left, int _right, int _top, int _bottom)
	{
		this.Left = _left;
		this.Right = _right;
		this.Top = _top;
		this.Bottom = _bottom;
	}

	// Token: 0x06007BCC RID: 31692 RVA: 0x00321A6E File Offset: 0x0031FC6E
	public XUiSideSizes SetLeft(int _value)
	{
		return new XUiSideSizes(_value, this.Right, this.Top, this.Bottom);
	}

	// Token: 0x06007BCD RID: 31693 RVA: 0x00321A88 File Offset: 0x0031FC88
	public XUiSideSizes SetRight(int _value)
	{
		return new XUiSideSizes(this.Left, _value, this.Top, this.Bottom);
	}

	// Token: 0x06007BCE RID: 31694 RVA: 0x00321AA2 File Offset: 0x0031FCA2
	public XUiSideSizes SetTop(int _value)
	{
		return new XUiSideSizes(this.Left, this.Right, _value, this.Bottom);
	}

	// Token: 0x06007BCF RID: 31695 RVA: 0x00321ABC File Offset: 0x0031FCBC
	public XUiSideSizes SetBottom(int _value)
	{
		return new XUiSideSizes(this.Left, this.Right, this.Top, _value);
	}

	// Token: 0x06007BD0 RID: 31696 RVA: 0x00321AD8 File Offset: 0x0031FCD8
	public static bool TryParse(string _value, out XUiSideSizes _result, string _valueName)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_value, ',', 3, 0, -1);
		if (separatorPositions.TotalFound > 3)
		{
			Log.Warning(string.Format("[XUi] Invalid number of values for {0}: {1}, max of 4 expected (input string: '{2}')", _valueName, separatorPositions.TotalFound, _value));
			_result = default(XUiSideSizes);
			return false;
		}
		int num;
		if (!StringParsers.TryParseSInt32(_value, out num, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 1st value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		if (separatorPositions.TotalFound == 0)
		{
			_result = new XUiSideSizes(num, num, num, num);
			return true;
		}
		int num2;
		if (!StringParsers.TryParseSInt32(_value, out num2, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 2nd value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		if (separatorPositions.TotalFound == 1)
		{
			_result = new XUiSideSizes(num2, num2, num, num);
			return true;
		}
		int bottom;
		if (!StringParsers.TryParseSInt32(_value, out bottom, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 3rd value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		if (separatorPositions.TotalFound == 2)
		{
			_result = new XUiSideSizes(num2, num2, num, bottom);
			return true;
		}
		int left;
		if (!StringParsers.TryParseSInt32(_value, out left, separatorPositions.Sep3 + 1, separatorPositions.Sep4 - 1, NumberStyles.Integer))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] ",
				_valueName,
				" can not be parsed, not an integer as 4th value (input string: '",
				_value,
				"')"
			}));
			_result = default(XUiSideSizes);
			return false;
		}
		_result = new XUiSideSizes(left, num2, num, bottom);
		return true;
	}

	// Token: 0x04005DA5 RID: 23973
	public readonly int Left;

	// Token: 0x04005DA6 RID: 23974
	public readonly int Right;

	// Token: 0x04005DA7 RID: 23975
	public readonly int Top;

	// Token: 0x04005DA8 RID: 23976
	public readonly int Bottom;
}

using System;
using System.Text;
using UnityEngine;

// Token: 0x0200124E RID: 4686
public static class ValueDisplayFormatters
{
	// Token: 0x060092AE RID: 37550 RVA: 0x003A7A1C File Offset: 0x003A5C1C
	public static double RoundToSignificantDigits(this double _value, int _digits)
	{
		if (_value == 0.0)
		{
			return 0.0;
		}
		double num = Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(_value))) + 1.0);
		return num * Math.Round(_value / num, _digits);
	}

	// Token: 0x060092AF RID: 37551 RVA: 0x003A7A74 File Offset: 0x003A5C74
	public static string FormatNumberWithMetricPrefix(this double _value, bool _allowDecimals = true, int _significantDigits = 3)
	{
		int num = 5;
		double num2 = (double)Math.Sign(_value);
		double num3 = Math.Abs(_value);
		if (_allowDecimals)
		{
			num3 = num3.RoundToSignificantDigits(_significantDigits);
			while (num3 > 1000.0)
			{
				num3 /= 1000.0;
				num++;
			}
			while (num3 != 0.0 && num3 < 1.0)
			{
				num3 *= 1000.0;
				num--;
			}
			if (num >= 0 && num < ValueDisplayFormatters.metricPrefixes.Length)
			{
				return (num2 * num3).ToCultureInvariantString("G" + _significantDigits.ToString()) + ((num != 5) ? ValueDisplayFormatters.metricPrefixes[num].ToString() : "");
			}
		}
		else
		{
			while (num3 > 10000.0)
			{
				num3 /= 1000.0;
				num++;
			}
			while (num3 != 0.0 && num3 < 10.0)
			{
				num3 *= 1000.0;
				num--;
			}
			if (num >= 0 && num < ValueDisplayFormatters.metricPrefixes.Length)
			{
				return (num2 * num3).ToCultureInvariantString("0") + ((num != 5) ? ValueDisplayFormatters.metricPrefixes[num].ToString() : "");
			}
		}
		return _value.ToCultureInvariantString("g");
	}

	// Token: 0x17000F20 RID: 3872
	// (get) Token: 0x060092B0 RID: 37552 RVA: 0x003A7BC3 File Offset: 0x003A5DC3
	public static bool UseEnglishCardinalDirections
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return GamePrefs.GetBool(EnumGamePrefs.OptionsUiCompassUseEnglishCardinalDirections);
		}
	}

	// Token: 0x17000F21 RID: 3873
	// (get) Token: 0x060092B1 RID: 37553 RVA: 0x003A7BCF File Offset: 0x003A5DCF
	public static string CardinalDirectionsLanguage
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (!ValueDisplayFormatters.UseEnglishCardinalDirections)
			{
				return null;
			}
			return Localization.DefaultLanguage;
		}
	}

	// Token: 0x060092B2 RID: 37554 RVA: 0x003A7BDF File Offset: 0x003A5DDF
	public static string WorldPos(Vector3 _pos, string _delim = " ", bool _useLongGeoDirs = false)
	{
		return ValueDisplayFormatters.WorldPosLongitude(_pos, _useLongGeoDirs) + _delim + ValueDisplayFormatters.WorldPosLatitude(_pos, _useLongGeoDirs);
	}

	// Token: 0x060092B3 RID: 37555 RVA: 0x003A7BF8 File Offset: 0x003A5DF8
	public static string WorldPosLatitude(Vector3 _pos, bool _useLongGeoDirs = false)
	{
		string cardinalDirectionsLanguage = ValueDisplayFormatters.CardinalDirectionsLanguage;
		string arg = Localization.Get("geoDirection" + ((_pos.z >= 0f) ? "N" : "S") + (_useLongGeoDirs ? "Long" : ""), cardinalDirectionsLanguage, false);
		string arg2 = Utils.FastAbs(_pos.z).ToCultureInvariantString("0");
		return string.Format(Localization.Get("geoLocationSingleAxis", cardinalDirectionsLanguage, false), arg2, arg);
	}

	// Token: 0x060092B4 RID: 37556 RVA: 0x003A7C70 File Offset: 0x003A5E70
	public static string WorldPosLongitude(Vector3 _pos, bool _useLongGeoDirs = false)
	{
		string cardinalDirectionsLanguage = ValueDisplayFormatters.CardinalDirectionsLanguage;
		string arg = Localization.Get("geoDirection" + ((_pos.x >= 0f) ? "E" : "W") + (_useLongGeoDirs ? "Long" : ""), cardinalDirectionsLanguage, false);
		string arg2 = Utils.FastAbs(_pos.x).ToCultureInvariantString("0");
		return string.Format(Localization.Get("geoLocationSingleAxis", cardinalDirectionsLanguage, false), arg2, arg);
	}

	// Token: 0x060092B5 RID: 37557 RVA: 0x003A7CE8 File Offset: 0x003A5EE8
	public static string Direction(GameUtils.DirEightWay _direction, bool _useLongGeoDirs = false)
	{
		string cardinalDirectionsLanguage = ValueDisplayFormatters.CardinalDirectionsLanguage;
		return Localization.Get("geoDirection" + _direction.ToStringCached<GameUtils.DirEightWay>() + (_useLongGeoDirs ? "Long" : ""), cardinalDirectionsLanguage, false);
	}

	// Token: 0x060092B6 RID: 37558 RVA: 0x003A7D24 File Offset: 0x003A5F24
	public static string WorldTime(ulong _worldTime, string _format)
	{
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		return string.Format(_format, item, item2, item3);
	}

	// Token: 0x060092B7 RID: 37559 RVA: 0x003A7D64 File Offset: 0x003A5F64
	public static string Distance(float _distance, bool _useLongDistanceName)
	{
		string format = Localization.Get("geoDistanceTemplate", false);
		string arg;
		if (_distance > 1000f)
		{
			arg = Localization.Get(_useLongDistanceName ? "geoDistanceKmLong" : "geoDistanceKm", false);
			return string.Format(format, (_distance / 1000f).ToCultureInvariantString("0.0"), arg);
		}
		arg = Localization.Get(_useLongDistanceName ? "geoDistanceMLong" : "geoDistanceM", false);
		if (_distance > 100f)
		{
			return string.Format(format, _distance.ToCultureInvariantString("0"), arg);
		}
		return string.Format(format, _distance.ToCultureInvariantString("0.0"), arg);
	}

	// Token: 0x060092B8 RID: 37560 RVA: 0x003A7DF7 File Offset: 0x003A5FF7
	public static string Distance(float _distance)
	{
		return ValueDisplayFormatters.Distance(_distance, false);
	}

	// Token: 0x060092B9 RID: 37561 RVA: 0x003A7E00 File Offset: 0x003A6000
	public static string DateAge(DateTime _dateTime)
	{
		TimeSpan timeSpan = DateTime.Now - _dateTime;
		if (timeSpan.TotalHours < 24.0)
		{
			return string.Format(Localization.Get("timeAgeHours", false), Mathf.RoundToInt((float)timeSpan.TotalHours));
		}
		if (timeSpan.TotalDays < 7.0)
		{
			return string.Format(Localization.Get("timeAgeDays", false), Mathf.RoundToInt((float)timeSpan.TotalDays));
		}
		if (timeSpan.TotalDays < 31.0)
		{
			return string.Format(Localization.Get("timeAgeWeeks", false), Mathf.RoundToInt((float)(timeSpan.TotalDays / 7.0)));
		}
		if (timeSpan.TotalDays < 365.0)
		{
			return string.Format(Localization.Get("timeAgeMonths", false), Mathf.RoundToInt((float)(timeSpan.TotalDays / 31.0)));
		}
		return string.Format(Localization.Get("timeAgeYears", false), Mathf.RoundToInt((float)(timeSpan.TotalDays / 365.0)));
	}

	// Token: 0x060092BA RID: 37562 RVA: 0x003A7F30 File Offset: 0x003A6130
	public static string Temperature(float _fahrenheit, int _decimals)
	{
		string str = "°F";
		if (GamePrefs.GetBool(EnumGamePrefs.OptionsTempCelsius))
		{
			_fahrenheit = Utils.ToCelsius(_fahrenheit);
			str = "°C";
		}
		return _fahrenheit.ToString(string.Format("F{0}", _decimals)) + str;
	}

	// Token: 0x060092BB RID: 37563 RVA: 0x003A7F7C File Offset: 0x003A617C
	public static string TemperatureRelative(float _fahrenheit, int _decimals)
	{
		string str = "°F";
		if (GamePrefs.GetBool(EnumGamePrefs.OptionsTempCelsius))
		{
			_fahrenheit = Utils.ToRelativeCelsius(_fahrenheit);
			str = "°C";
		}
		return ((_fahrenheit >= 0f) ? "+" : "") + _fahrenheit.ToString(string.Format("F{0}", _decimals)) + str;
	}

	// Token: 0x060092BC RID: 37564 RVA: 0x003A7FDC File Offset: 0x003A61DC
	public static string RomanNumber(int _value)
	{
		if (_value <= 0)
		{
			Log.Warning(string.Format("{0}: Trying to convert {1} - this method is meant for positive numbers only.", "RomanNumber", _value));
			return _value.ToString();
		}
		if (_value >= 4000)
		{
			Log.Warning(string.Format("{0}: Trying to convert {1} - this method is meant for numbers below 4000.", "RomanNumber", _value));
			return _value.ToString();
		}
		if (_value <= 10)
		{
			return ValueDisplayFormatters.romanNumbers[_value];
		}
		StringBuilder stringBuilder = new StringBuilder();
		while (_value > 0)
		{
			if (_value >= 1000)
			{
				stringBuilder.Append("M");
				_value -= 1000;
			}
			else if (_value >= 900)
			{
				stringBuilder.Append("CM");
				_value -= 900;
			}
			else if (_value >= 500)
			{
				stringBuilder.Append("D");
				_value -= 500;
			}
			else if (_value >= 400)
			{
				stringBuilder.Append("CD");
				_value -= 400;
			}
			else if (_value >= 100)
			{
				stringBuilder.Append("C");
				_value -= 100;
			}
			else if (_value >= 90)
			{
				stringBuilder.Append("XC");
				_value -= 90;
			}
			else if (_value >= 50)
			{
				stringBuilder.Append("L");
				_value -= 50;
			}
			else if (_value >= 40)
			{
				stringBuilder.Append("XL");
				_value -= 40;
			}
			else if (_value >= 10)
			{
				stringBuilder.Append("X");
				_value -= 10;
			}
			else
			{
				stringBuilder.Append(ValueDisplayFormatters.romanNumbers[_value]);
				_value = 0;
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0400704A RID: 28746
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] metricPrefixes = new char[]
	{
		'f',
		'p',
		'n',
		'µ',
		'm',
		' ',
		'k',
		'M',
		'G',
		'T',
		'P'
	};

	// Token: 0x0400704B RID: 28747
	[PublicizedFrom(EAccessModifier.Private)]
	public const int BaseMetricPrefixIndex = 5;

	// Token: 0x0400704C RID: 28748
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] romanNumbers = new string[]
	{
		"",
		"I",
		"II",
		"III",
		"IV",
		"V",
		"VI",
		"VII",
		"VIII",
		"IX",
		"X"
	};
}

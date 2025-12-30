using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x0200121D RID: 4637
public static class StringParsers
{
	// Token: 0x060090B2 RID: 37042 RVA: 0x0039B134 File Offset: 0x00399334
	public static sbyte ParseSInt8(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out num, out num2, out ex, true, _startIndex, _endIndex))
		{
			throw ex;
		}
		if (num < -128L || num > 127L)
		{
			throw new OverflowException("Value too large or too small (input='" + _input + "')");
		}
		return (sbyte)num;
	}

	// Token: 0x060090B3 RID: 37043 RVA: 0x0039B17C File Offset: 0x0039937C
	public static byte ParseUInt8(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out num, out num2, out ex, false, _startIndex, _endIndex))
		{
			throw ex;
		}
		if (num2 > 255UL)
		{
			throw new OverflowException("Value too large or too small (input='" + _input + "')");
		}
		return (byte)num2;
	}

	// Token: 0x060090B4 RID: 37044 RVA: 0x0039B1C0 File Offset: 0x003993C0
	public static bool TryParseSInt8(string _input, out sbyte _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, true, _startIndex, _endIndex))
		{
			_result = 0;
			return false;
		}
		if (num < -128L || num > 127L)
		{
			_result = 0;
			return false;
		}
		_result = (sbyte)num;
		return true;
	}

	// Token: 0x060090B5 RID: 37045 RVA: 0x0039B1FC File Offset: 0x003993FC
	public static bool TryParseUInt8(string _input, out byte _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, false, _startIndex, _endIndex))
		{
			_result = 0;
			return false;
		}
		if (num2 > 255UL)
		{
			_result = 0;
			return false;
		}
		_result = (byte)num2;
		return true;
	}

	// Token: 0x060090B6 RID: 37046 RVA: 0x0039B238 File Offset: 0x00399438
	public static short ParseSInt16(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out num, out num2, out ex, true, _startIndex, _endIndex))
		{
			throw ex;
		}
		if (num < -32768L || num > 32767L)
		{
			throw new OverflowException("Value too large or too small (input='" + _input + "')");
		}
		return (short)num;
	}

	// Token: 0x060090B7 RID: 37047 RVA: 0x0039B284 File Offset: 0x00399484
	public static ushort ParseUInt16(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out num, out num2, out ex, false, _startIndex, _endIndex))
		{
			throw ex;
		}
		if (num2 > 65535UL)
		{
			throw new OverflowException("Value too large or too small (input='" + _input + "')");
		}
		return (ushort)num2;
	}

	// Token: 0x060090B8 RID: 37048 RVA: 0x0039B2C8 File Offset: 0x003994C8
	public static bool TryParseSInt16(string _input, out short _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, true, _startIndex, _endIndex))
		{
			_result = 0;
			return false;
		}
		if (num < -32768L || num > 32767L)
		{
			_result = 0;
			return false;
		}
		_result = (short)num;
		return true;
	}

	// Token: 0x060090B9 RID: 37049 RVA: 0x0039B30C File Offset: 0x0039950C
	public static bool TryParseUInt16(string _input, out ushort _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, false, _startIndex, _endIndex))
		{
			_result = 0;
			return false;
		}
		if (num2 > 65535UL)
		{
			_result = 0;
			return false;
		}
		_result = (ushort)num2;
		return true;
	}

	// Token: 0x060090BA RID: 37050 RVA: 0x0039B348 File Offset: 0x00399548
	public static int ParseSInt32(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out num, out num2, out ex, true, _startIndex, _endIndex))
		{
			throw ex;
		}
		if (num < -2147483648L || num > 2147483647L)
		{
			throw new OverflowException("Value too large or too small (input='" + _input + "')");
		}
		return (int)num;
	}

	// Token: 0x060090BB RID: 37051 RVA: 0x0039B394 File Offset: 0x00399594
	public static uint ParseUInt32(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out num, out num2, out ex, false, _startIndex, _endIndex))
		{
			throw ex;
		}
		if (num2 > (ulong)-1)
		{
			throw new OverflowException("Value too large or too small (input='" + _input + "')");
		}
		return (uint)num2;
	}

	// Token: 0x060090BC RID: 37052 RVA: 0x0039B3D4 File Offset: 0x003995D4
	public static bool TryParseSInt32(string _input, out int _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, true, _startIndex, _endIndex))
		{
			_result = 0;
			return false;
		}
		if (num < -2147483648L || num > 2147483647L)
		{
			_result = 0;
			return false;
		}
		_result = (int)num;
		return true;
	}

	// Token: 0x060090BD RID: 37053 RVA: 0x0039B418 File Offset: 0x00399618
	public static bool TryParseUInt32(string _input, out uint _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, false, _startIndex, _endIndex))
		{
			_result = 0U;
			return false;
		}
		if (num2 > (ulong)-1)
		{
			_result = 0U;
			return false;
		}
		_result = (uint)num2;
		return true;
	}

	// Token: 0x060090BE RID: 37054 RVA: 0x0039B450 File Offset: 0x00399650
	public static long ParseSInt64(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long result;
		ulong num;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out result, out num, out ex, true, _startIndex, _endIndex))
		{
			throw ex;
		}
		return result;
	}

	// Token: 0x060090BF RID: 37055 RVA: 0x0039B474 File Offset: 0x00399674
	public static ulong ParseUInt64(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong result;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, false, out num, out result, out ex, false, _startIndex, _endIndex))
		{
			throw ex;
		}
		return result;
	}

	// Token: 0x060090C0 RID: 37056 RVA: 0x0039B498 File Offset: 0x00399698
	public static bool TryParseSInt64(string _input, out long _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, true, _startIndex, _endIndex))
		{
			_result = 0L;
			return false;
		}
		_result = num;
		return true;
	}

	// Token: 0x060090C1 RID: 37057 RVA: 0x0039B4C4 File Offset: 0x003996C4
	public static bool TryParseUInt64(string _input, out ulong _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Integer)
	{
		long num;
		ulong num2;
		Exception ex;
		if (!StringParsers.internalParseInt64(_input, _style, true, out num, out num2, out ex, false, _startIndex, _endIndex))
		{
			_result = 0UL;
			return false;
		}
		_result = num2;
		return true;
	}

	// Token: 0x060090C2 RID: 37058 RVA: 0x0039B4F0 File Offset: 0x003996F0
	public static float ParseFloat(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Any)
	{
		double num = StringParsers.ParseDouble(_input, _startIndex, _endIndex, _style);
		if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
		{
			throw new OverflowException();
		}
		return (float)num;
	}

	// Token: 0x060090C3 RID: 37059 RVA: 0x0039B530 File Offset: 0x00399730
	public static bool TryParseFloat(string _input, out float _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Any)
	{
		double num;
		Exception ex;
		if (!StringParsers.internalParseDouble(_input, _style, true, out num, out ex, _startIndex, _endIndex))
		{
			_result = 0f;
			return false;
		}
		if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
		{
			_result = 0f;
			return false;
		}
		_result = (float)num;
		return true;
	}

	// Token: 0x060090C4 RID: 37060 RVA: 0x0039B584 File Offset: 0x00399784
	public static double ParseDouble(string _input, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Any)
	{
		double result;
		Exception ex;
		if (!StringParsers.internalParseDouble(_input, _style, false, out result, out ex, _startIndex, _endIndex))
		{
			throw ex;
		}
		return result;
	}

	// Token: 0x060090C5 RID: 37061 RVA: 0x0039B5A4 File Offset: 0x003997A4
	public static bool TryParseDouble(string _input, out double _result, int _startIndex = 0, int _endIndex = -1, NumberStyles _style = NumberStyles.Any)
	{
		Exception ex;
		if (!StringParsers.internalParseDouble(_input, _style, true, out _result, out ex, _startIndex, _endIndex))
		{
			_result = 0.0;
			return false;
		}
		return true;
	}

	// Token: 0x060090C6 RID: 37062 RVA: 0x0039B5CF File Offset: 0x003997CF
	public static DateTime ParseDateTime(string _s)
	{
		return DateTime.Parse(_s, Utils.StandardCulture);
	}

	// Token: 0x060090C7 RID: 37063 RVA: 0x0039B5DC File Offset: 0x003997DC
	public static bool TryParseDateTime(string _s, out DateTime _result)
	{
		return DateTime.TryParse(_s, Utils.StandardCulture, DateTimeStyles.None, out _result);
	}

	// Token: 0x060090C8 RID: 37064 RVA: 0x0039B5EC File Offset: 0x003997EC
	public static bool ParseBool(string _input, int _startIndex = 0, int _endIndex = -1, bool _ignoreCase = true)
	{
		bool result;
		Exception ex;
		if (!StringParsers.internalParseBool(_input, false, out result, out ex, _ignoreCase, _startIndex, _endIndex))
		{
			throw ex;
		}
		return result;
	}

	// Token: 0x060090C9 RID: 37065 RVA: 0x0039B60C File Offset: 0x0039980C
	public static bool TryParseBool(string _input, out bool _result, int _startIndex = 0, int _endIndex = -1, bool _ignoreCase = true)
	{
		Exception ex;
		if (!StringParsers.internalParseBool(_input, true, out _result, out ex, _ignoreCase, _startIndex, _endIndex))
		{
			_result = false;
			return false;
		}
		return true;
	}

	// Token: 0x060090CA RID: 37066 RVA: 0x0039B630 File Offset: 0x00399830
	public static bool TryParseRange(string _input, out FloatRange _result, float? _defaultMax = null)
	{
		_result = default(FloatRange);
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound == 0)
		{
			float num;
			if (StringParsers.TryParseFloat(_input, out num, 0, -1, NumberStyles.Any))
			{
				_result = new FloatRange(num, _defaultMax ?? num);
				return true;
			}
			return false;
		}
		else
		{
			float min;
			float max;
			if (StringParsers.TryParseFloat(_input, out min, 0, separatorPositions.Sep1 - 1, NumberStyles.Any) && StringParsers.TryParseFloat(_input, out max, separatorPositions.Sep1 + 1, -1, NumberStyles.Any))
			{
				_result = new FloatRange(min, max);
				return true;
			}
			return false;
		}
	}

	// Token: 0x060090CB RID: 37067 RVA: 0x0039B6D0 File Offset: 0x003998D0
	public static bool TryParseRange(string _input, out IntRange _result, int? _defaultMax = null)
	{
		_result = default(IntRange);
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound == 0)
		{
			int num;
			if (StringParsers.TryParseSInt32(_input, out num, 0, -1, NumberStyles.Integer))
			{
				_result = new IntRange(num, _defaultMax ?? num);
				return true;
			}
			return false;
		}
		else
		{
			int min;
			int max;
			if (StringParsers.TryParseSInt32(_input, out min, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer) && StringParsers.TryParseSInt32(_input, out max, separatorPositions.Sep1 + 1, -1, NumberStyles.Integer))
			{
				_result = new IntRange(min, max);
				return true;
			}
			return false;
		}
	}

	// Token: 0x060090CC RID: 37068 RVA: 0x0039B764 File Offset: 0x00399964
	public static Vector2 ParseVector2(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound != 1)
		{
			return Vector2.zero;
		}
		return new Vector2(StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, -1, NumberStyles.Any));
	}

	// Token: 0x060090CD RID: 37069 RVA: 0x0039B7BC File Offset: 0x003999BC
	public static Vector2 ParseVector2(string _input, float _defaultValue)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound == 0)
		{
			return new Vector2(StringParsers.ParseFloat(_input, 0, -1, NumberStyles.Any), _defaultValue);
		}
		return new Vector2(StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, -1, NumberStyles.Any));
	}

	// Token: 0x060090CE RID: 37070 RVA: 0x0039B820 File Offset: 0x00399A20
	public static Vector3 ParseVector3(string _input, int _startIndex = 0, int _endIndex = -1)
	{
		if (_startIndex == 0 && _endIndex < 0 && _input.Length > 0 && _input[0] == '(' && _input[_input.Length - 1] == ')')
		{
			_startIndex = 1;
			_endIndex = _input.Length - 2;
		}
		if (_endIndex < 0)
		{
			_endIndex = _input.Length - 1;
		}
		if (_endIndex < _startIndex || _endIndex >= _input.Length)
		{
			throw new ArgumentException("_endIndex out of range (input='" + _input + "')", "_endIndex");
		}
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 2, _startIndex, _endIndex);
		if (separatorPositions.TotalFound != 2)
		{
			return Vector3.zero;
		}
		return new Vector3(StringParsers.ParseFloat(_input, _startIndex, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep2 + 1, _endIndex, NumberStyles.Any));
	}

	// Token: 0x060090CF RID: 37071 RVA: 0x0039B900 File Offset: 0x00399B00
	public static Vector3 ParseVector3(string _input, float _defaultValue)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound == 0)
		{
			return new Vector3(StringParsers.ParseFloat(_input, 0, -1, NumberStyles.Any), _defaultValue, _defaultValue);
		}
		if (separatorPositions.TotalFound == 1)
		{
			return new Vector3(StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, -1, NumberStyles.Any), _defaultValue);
		}
		return new Vector3(StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep2 + 1, -1, NumberStyles.Any));
	}

	// Token: 0x060090D0 RID: 37072 RVA: 0x0039B9B8 File Offset: 0x00399BB8
	public static Vector4 ParseVector4(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 3, 0, -1);
		if (separatorPositions.TotalFound != 3)
		{
			return Vector4.zero;
		}
		return new Vector4(StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep3 + 1, -1, NumberStyles.Any));
	}

	// Token: 0x060090D1 RID: 37073 RVA: 0x0039BA44 File Offset: 0x00399C44
	public static BlockFaceFlag ParseWaterFlowMask(string _input)
	{
		if (_input.EqualsCaseInsensitive("permitted"))
		{
			return BlockFaceFlag.None;
		}
		if (_input.Contains(','))
		{
			string[] array = _input.Split(',', StringSplitOptions.None);
			BlockFaceFlag blockFaceFlag = BlockFaceFlag.All;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				char c;
				if (char.TryParse(array2[i], out c))
				{
					blockFaceFlag &= ~BlockFaceFlags.FromBlockFace(BlockFaces.CharToFace(c));
				}
			}
			return blockFaceFlag;
		}
		return BlockFaceFlag.All;
	}

	// Token: 0x060090D2 RID: 37074 RVA: 0x0039BAA4 File Offset: 0x00399CA4
	public static Plane ParsePlane(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 3, 0, -1);
		if (separatorPositions.TotalFound != 3)
		{
			return default(Plane);
		}
		Vector3 inNormal = new Vector3(StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Any));
		float d = StringParsers.ParseFloat(_input, separatorPositions.Sep3 + 1, -1, NumberStyles.Any);
		return new Plane(inNormal, d);
	}

	// Token: 0x060090D3 RID: 37075 RVA: 0x0039BB3C File Offset: 0x00399D3C
	public static Quaternion ParseQuaternion(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 3, 0, -1);
		if (separatorPositions.TotalFound != 3)
		{
			return Quaternion.identity;
		}
		return new Quaternion(StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Any), StringParsers.ParseFloat(_input, separatorPositions.Sep3 + 1, -1, NumberStyles.Any));
	}

	// Token: 0x060090D4 RID: 37076 RVA: 0x0039BBC8 File Offset: 0x00399DC8
	public static Color ParseColor(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 3, 0, -1);
		if (separatorPositions.TotalFound < 2 || separatorPositions.TotalFound > 3)
		{
			return Color.white;
		}
		float r = StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any);
		float g = StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Any);
		float b = StringParsers.ParseFloat(_input, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Any);
		if (separatorPositions.TotalFound == 2)
		{
			return new Color(r, g, b);
		}
		float a = StringParsers.ParseFloat(_input, separatorPositions.Sep3 + 1, -1, NumberStyles.Any);
		return new Color(r, g, b, a);
	}

	// Token: 0x060090D5 RID: 37077 RVA: 0x0039BC78 File Offset: 0x00399E78
	public static Color ParseHexColor(string _input)
	{
		if (_input == null)
		{
			return Color.clear;
		}
		if (_input.IndexOf(',') >= 0)
		{
			return StringParsers.ParseColor32(_input);
		}
		if (_input.Length < 6)
		{
			return Color.clear;
		}
		int num = 0;
		if (_input[0] == '#')
		{
			num = 1;
		}
		byte r = StringParsers.ParseUInt8(_input, num, num + 1, NumberStyles.HexNumber);
		byte g = StringParsers.ParseUInt8(_input, num + 2, num + 3, NumberStyles.HexNumber);
		byte b = StringParsers.ParseUInt8(_input, num + 4, num + 5, NumberStyles.HexNumber);
		return new Color32(r, g, b, byte.MaxValue);
	}

	// Token: 0x060090D6 RID: 37078 RVA: 0x0039BD04 File Offset: 0x00399F04
	public static Color ParseColor32(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 3, 0, -1);
		if (separatorPositions.TotalFound < 2)
		{
			return Color.white;
		}
		float num = (float)StringParsers.ParseSInt32(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer);
		float num2 = (float)StringParsers.ParseSInt32(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Integer);
		float num3 = (float)StringParsers.ParseSInt32(_input, separatorPositions.Sep2 + 1, separatorPositions.Sep3 - 1, NumberStyles.Integer);
		float num4 = 255f;
		if (separatorPositions.TotalFound > 2)
		{
			num4 = (float)StringParsers.ParseSInt32(_input, separatorPositions.Sep3 + 1, -1, NumberStyles.Integer);
		}
		return new Color(num / 255f, num2 / 255f, num3 / 255f, num4 / 255f);
	}

	// Token: 0x060090D7 RID: 37079 RVA: 0x0039BDB0 File Offset: 0x00399FB0
	public static Bounds ParseBounds(string _input)
	{
		int num = _input.IndexOf('(');
		if (num < 0)
		{
			throw new FormatException("Bounds input string is not in the correct format. Expected \"([Center X], [Center Y], [Center Z]), ([Size X], [Size Y], [Size Z])\" - e.g. \"(-0.5,0.5,-0.5),(2,2,6)\" (input='" + _input + "')");
		}
		int num2 = _input.IndexOf(')', num + 1);
		if (num2 < 0)
		{
			throw new FormatException("Bounds input string is not in the correct format. Expected \"([Center X], [Center Y], [Center Z]), ([Size X], [Size Y], [Size Z])\" - e.g. \"(-0.5,0.5,-0.5),(2,2,6)\" (input='" + _input + "')");
		}
		int num3 = _input.IndexOf('(', num2 + 1);
		if (num3 < 0)
		{
			throw new FormatException("Bounds input string is not in the correct format. Expected \"([Center X], [Center Y], [Center Z]), ([Size X], [Size Y], [Size Z])\" - e.g. \"(-0.5,0.5,-0.5),(2,2,6)\" (input='" + _input + "')");
		}
		int num4 = _input.IndexOf(')', num3 + 1);
		if (num4 < 0)
		{
			throw new FormatException("Bounds input string is not in the correct format. Expected \"([Center X], [Center Y], [Center Z]), ([Size X], [Size Y], [Size Z])\" - e.g. \"(-0.5,0.5,-0.5),(2,2,6)\" (input='" + _input + "')");
		}
		Vector3 center = StringParsers.ParseVector3(_input, num + 1, num2 - 1);
		Vector3 size = StringParsers.ParseVector3(_input, num3 + 1, num4 - 1);
		return new Bounds(center, size);
	}

	// Token: 0x060090D8 RID: 37080 RVA: 0x0039BE74 File Offset: 0x0039A074
	public static Vector2d ParseVector2d(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound != 1)
		{
			return Vector2d.Zero;
		}
		return new Vector2d(StringParsers.ParseDouble(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseDouble(_input, separatorPositions.Sep1 + 1, -1, NumberStyles.Any));
	}

	// Token: 0x060090D9 RID: 37081 RVA: 0x0039BECC File Offset: 0x0039A0CC
	public static Vector3d ParseVector3d(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 2, 0, -1);
		if (separatorPositions.TotalFound != 2)
		{
			return Vector3d.Zero;
		}
		return new Vector3d(StringParsers.ParseDouble(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any), StringParsers.ParseDouble(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Any), StringParsers.ParseDouble(_input, separatorPositions.Sep2 + 1, -1, NumberStyles.Any));
	}

	// Token: 0x060090DA RID: 37082 RVA: 0x0039BF3C File Offset: 0x0039A13C
	public static Vector2i ParseVector2i(string _input, char _customSep = ',')
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, _customSep, 1, 0, -1);
		if (separatorPositions.TotalFound != 1)
		{
			return Vector2i.zero;
		}
		return new Vector2i(StringParsers.ParseSInt32(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer), StringParsers.ParseSInt32(_input, separatorPositions.Sep1 + 1, -1, NumberStyles.Integer));
	}

	// Token: 0x060090DB RID: 37083 RVA: 0x0039BF88 File Offset: 0x0039A188
	public static Vector3i ParseVector3i(string _input, int _startIndex = 0, int _endIndex = -1, bool _errorOnFailure = false)
	{
		if (_endIndex < 0)
		{
			_endIndex = _input.Length - 1;
		}
		if (_endIndex < _startIndex || _endIndex >= _input.Length)
		{
			throw new ArgumentException("_endIndex out of range (input='" + _input + "')", "_endIndex");
		}
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 2, _startIndex, _endIndex);
		if (separatorPositions.TotalFound == 2)
		{
			return new Vector3i(StringParsers.ParseSInt32(_input, _startIndex, separatorPositions.Sep1 - 1, NumberStyles.Integer), StringParsers.ParseSInt32(_input, separatorPositions.Sep1 + 1, separatorPositions.Sep2 - 1, NumberStyles.Integer), StringParsers.ParseSInt32(_input, separatorPositions.Sep2 + 1, _endIndex, NumberStyles.Integer));
		}
		if (_errorOnFailure)
		{
			throw new FormatException("_input in invalid format (input='" + _input + "')");
		}
		return Vector3i.zero;
	}

	// Token: 0x060090DC RID: 37084 RVA: 0x0039C03C File Offset: 0x0039A23C
	public static List<T> ParseList<T>(string _input, char _separator, Func<string, int, int, T> _parserFunc)
	{
		List<T> list = new List<T>();
		int num = -1;
		for (int i = _input.IndexOf(_separator, 0); i >= 0; i = _input.IndexOf(_separator, num + 1))
		{
			list.Add(_parserFunc(_input, num + 1, i - 1));
			num = i;
		}
		if (num + 1 < _input.Length)
		{
			list.Add(_parserFunc(_input, num + 1, -1));
		}
		return list;
	}

	// Token: 0x060090DD RID: 37085 RVA: 0x0039C0A0 File Offset: 0x0039A2A0
	public static void ParseMinMaxCount(string _input, out int _minCount, out int _maxCount)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound > 1)
		{
			throw new Exception("Parsing error count (input='" + _input + "')");
		}
		if (separatorPositions.TotalFound == 0)
		{
			if (!StringParsers.TryParseSInt32(_input, out _minCount, 0, -1, NumberStyles.Integer))
			{
				throw new Exception("Parsing error count (input='" + _input + "')");
			}
			_maxCount = _minCount;
		}
		if (!StringParsers.TryParseSInt32(_input, out _minCount, 0, separatorPositions.Sep1 - 1, NumberStyles.Integer) || !StringParsers.TryParseSInt32(_input, out _maxCount, separatorPositions.Sep1 + 1, -1, NumberStyles.Integer))
		{
			throw new Exception("Parsing error count (input='" + _input + "')");
		}
	}

	// Token: 0x060090DE RID: 37086 RVA: 0x0039C144 File Offset: 0x0039A344
	public static void ParseMinMaxCount(string _input, out float _minCount, out float _maxCount)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound > 1)
		{
			throw new Exception("Parsing error count (input='" + _input + "')");
		}
		if (separatorPositions.TotalFound == 0)
		{
			if (!StringParsers.TryParseFloat(_input, out _minCount, 0, -1, NumberStyles.Any))
			{
				throw new Exception("Parsing error count (input='" + _input + "')");
			}
			_maxCount = _minCount;
		}
		if (!StringParsers.TryParseFloat(_input, out _minCount, 0, separatorPositions.Sep1 - 1, NumberStyles.Any) || !StringParsers.TryParseFloat(_input, out _maxCount, separatorPositions.Sep1 + 1, -1, NumberStyles.Any))
		{
			throw new Exception("Parsing error count (input='" + _input + "')");
		}
	}

	// Token: 0x060090DF RID: 37087 RVA: 0x0039C1F4 File Offset: 0x0039A3F4
	public static Vector2 ParseMinMaxCount(string _input)
	{
		StringParsers.SeparatorPositions separatorPositions = StringParsers.GetSeparatorPositions(_input, ',', 1, 0, -1);
		if (separatorPositions.TotalFound != 1)
		{
			return Vector2.zero;
		}
		float num = StringParsers.ParseFloat(_input, 0, separatorPositions.Sep1 - 1, NumberStyles.Any);
		float num2 = StringParsers.ParseFloat(_input, separatorPositions.Sep1 + 1, -1, NumberStyles.Any);
		if (num != num2)
		{
			return new Vector2(Mathf.Min(num, num2), Mathf.Max(num, num2));
		}
		return new Vector2(num, num2);
	}

	// Token: 0x060090E0 RID: 37088 RVA: 0x0039C265 File Offset: 0x0039A465
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool findOther(ref int _pos, string _input, char _other)
	{
		if (_input[_pos] == _other)
		{
			_pos++;
			return true;
		}
		return false;
	}

	// Token: 0x060090E1 RID: 37089 RVA: 0x0039C27C File Offset: 0x0039A47C
	public static StringParsers.SeparatorPositions GetSeparatorPositions(string _input, char _separator, int _expected, int _startIndex = 0, int _endIndex = -1)
	{
		StringParsers.SeparatorPositions separatorPositions = new StringParsers.SeparatorPositions(0);
		if (_expected <= 0)
		{
			throw new ArgumentException("_expected has to be greater than 0");
		}
		if (_input == null)
		{
			throw new ArgumentNullException("_input");
		}
		if (_input.Length == 0)
		{
			return separatorPositions;
		}
		if (_endIndex < 0)
		{
			_endIndex = _input.Length - 1;
		}
		if (_endIndex < _startIndex || _endIndex >= _input.Length)
		{
			throw new ArgumentException("_endIndex out of range (input='" + _input + "')", "_endIndex");
		}
		separatorPositions.Sep1 = _input.IndexOf(_separator, _startIndex, _endIndex - _startIndex + 1);
		if (separatorPositions.Sep1 < 0)
		{
			return separatorPositions;
		}
		separatorPositions.TotalFound++;
		separatorPositions.Sep2 = _input.IndexOf(_separator, separatorPositions.Sep1 + 1, _endIndex - separatorPositions.Sep1);
		if (separatorPositions.Sep2 < 0)
		{
			return separatorPositions;
		}
		separatorPositions.TotalFound++;
		if (_expected == 1)
		{
			return separatorPositions;
		}
		separatorPositions.Sep3 = _input.IndexOf(_separator, separatorPositions.Sep2 + 1, _endIndex - separatorPositions.Sep2);
		if (separatorPositions.Sep3 < 0)
		{
			return separatorPositions;
		}
		separatorPositions.TotalFound++;
		if (_expected == 2)
		{
			return separatorPositions;
		}
		separatorPositions.Sep4 = _input.IndexOf(_separator, separatorPositions.Sep3 + 1, _endIndex - separatorPositions.Sep3);
		if (separatorPositions.Sep4 < 0)
		{
			return separatorPositions;
		}
		separatorPositions.TotalFound++;
		if (_expected == 3)
		{
			return separatorPositions;
		}
		if (_input.IndexOf(_separator, separatorPositions.Sep4 + 1, _endIndex - separatorPositions.Sep4) < 0)
		{
			return separatorPositions;
		}
		separatorPositions.TotalFound++;
		return separatorPositions;
	}

	// Token: 0x060090E2 RID: 37090 RVA: 0x0039C3FC File Offset: 0x0039A5FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool internalParseDouble(string _input, NumberStyles _numberStyle, bool _tryParse, out double _result, out Exception _exception, int _startIndex, int _endIndex)
	{
		_result = 0.0;
		_exception = null;
		if (_input == null)
		{
			if (!_tryParse)
			{
				_exception = new ArgumentNullException("_input");
			}
			return false;
		}
		if (_input.Length == 0)
		{
			if (!_tryParse)
			{
				_exception = new FormatException("Empty input string");
			}
			return false;
		}
		if (_startIndex < 0 || _startIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exception = new ArgumentException(string.Format("_startIndex ({0}) out of range (input='{1}')", _startIndex, _input), "_startIndex");
			}
			return false;
		}
		if (_endIndex < 0)
		{
			_endIndex = _input.Length - 1;
		}
		if (_endIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exception = new ArgumentException(string.Format("_endIndex ({0}) out of range (input='{1}')", _endIndex, _input), "_endIndex");
			}
			return false;
		}
		if ((_numberStyle & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
		{
			throw new ArgumentException("Double doesn't support parsing with 'AllowHexSpecifier' (input='" + _input + "')");
		}
		if (_numberStyle > NumberStyles.Any)
		{
			if (!_tryParse)
			{
				_exception = new ArgumentException();
			}
			return false;
		}
		bool flag = (_numberStyle & NumberStyles.AllowLeadingWhite) > NumberStyles.None;
		bool flag2 = (_numberStyle & NumberStyles.AllowTrailingWhite) > NumberStyles.None;
		bool flag3 = (_numberStyle & NumberStyles.AllowLeadingSign) > NumberStyles.None;
		bool flag4 = (_numberStyle & NumberStyles.AllowExponent) > NumberStyles.None;
		bool flag5 = (_numberStyle & NumberStyles.AllowDecimalPoint) > NumberStyles.None;
		bool flag6 = (_numberStyle & NumberStyles.AllowThousands) > NumberStyles.None;
		bool flag7 = (_numberStyle & NumberStyles.AllowCurrencySymbol) > NumberStyles.None;
		int i = _startIndex;
		if (flag)
		{
			while (i <= _endIndex && char.IsWhiteSpace(_input[i]))
			{
				i++;
			}
			if (i > _endIndex)
			{
				if (!_tryParse)
				{
					_exception = new FormatException("Input string was not in the correct format (input='" + _input + "')");
				}
				return false;
			}
		}
		if (flag2)
		{
			while (_endIndex >= 0 && char.IsWhiteSpace(_input[_endIndex]))
			{
				_endIndex--;
			}
		}
		if (i > _endIndex || _endIndex < 0)
		{
			if (!_tryParse)
			{
				_exception = new FormatException("Input string was not in the correct format (input='" + _input + "')");
			}
			return false;
		}
		if (_endIndex - _startIndex + 1 == StringParsers.NAN_SYMBOL.Length && string.Compare(StringParsers.NAN_SYMBOL, 0, _input, _startIndex, StringParsers.NAN_SYMBOL.Length, StringComparison.Ordinal) == 0)
		{
			_result = double.NaN;
			return true;
		}
		if (_endIndex - _startIndex + 1 == StringParsers.POSITIVE_INFINITY_SYMBOL.Length && string.Compare(StringParsers.POSITIVE_INFINITY_SYMBOL, 0, _input, _startIndex, StringParsers.POSITIVE_INFINITY_SYMBOL.Length, StringComparison.Ordinal) == 0)
		{
			_result = double.PositiveInfinity;
			return true;
		}
		if (_endIndex - _startIndex + 1 == StringParsers.NEGATIVE_INFINITY_SYMBOL.Length && string.Compare(StringParsers.NEGATIVE_INFINITY_SYMBOL, 0, _input, _startIndex, StringParsers.NEGATIVE_INFINITY_SYMBOL.Length, StringComparison.Ordinal) == 0)
		{
			_result = double.NegativeInfinity;
			return true;
		}
		double num = 0.0;
		bool flag8 = false;
		bool flag9 = false;
		int num2 = 0;
		int j = 0;
		StringParsers.EFloatParseState efloatParseState = StringParsers.EFloatParseState.SignOrIntegralDigit;
		while (i <= _endIndex)
		{
			char c = _input[i];
			int num3 = (int)(c ^ '0');
			bool flag10 = num3 <= 9;
			if (c == '\0')
			{
				break;
			}
			switch (efloatParseState)
			{
			case StringParsers.EFloatParseState.SignOrIntegralDigit:
				if (flag3 && c == '+')
				{
					efloatParseState = StringParsers.EFloatParseState.IntegralDigit;
				}
				else if (flag3 && c == '-')
				{
					efloatParseState = StringParsers.EFloatParseState.IntegralDigit;
					flag8 = true;
				}
				else
				{
					efloatParseState = StringParsers.EFloatParseState.IntegralDigit;
					i--;
				}
				break;
			case StringParsers.EFloatParseState.IntegralDigit:
				if (flag10)
				{
					num = num * 10.0 + (double)num3;
				}
				else if (c == 'e' || c == 'E')
				{
					efloatParseState = StringParsers.EFloatParseState.ExponentialTest;
					i--;
				}
				else if (flag5 && c == '.')
				{
					efloatParseState = StringParsers.EFloatParseState.DecimalDigit;
				}
				else if ((!flag6 || c != ',') && (!flag7 || c != '¤'))
				{
					if (!char.IsWhiteSpace(c))
					{
						if (!_tryParse)
						{
							_exception = new FormatException(string.Format("Unknown char: {0} (input: '{1}')", c, _input));
						}
						return false;
					}
					efloatParseState = StringParsers.EFloatParseState.TrailingWs;
					i--;
				}
				break;
			case StringParsers.EFloatParseState.DecimalDigit:
				if (flag10)
				{
					num = num * 10.0 + (double)num3;
					num2++;
				}
				else if (c == 'e' || c == 'E')
				{
					efloatParseState = StringParsers.EFloatParseState.ExponentialTest;
					i--;
				}
				else
				{
					if (!char.IsWhiteSpace(c))
					{
						if (!_tryParse)
						{
							_exception = new FormatException(string.Format("Unknown char: {0} (input: '{1}')", c, _input));
						}
						return false;
					}
					efloatParseState = StringParsers.EFloatParseState.TrailingWs;
					i--;
				}
				break;
			case StringParsers.EFloatParseState.ExponentialTest:
				if (!flag4)
				{
					if (!_tryParse)
					{
						_exception = new FormatException(string.Format("Unknown char: {0} (input: '{1}')", c, _input));
					}
					return false;
				}
				efloatParseState = StringParsers.EFloatParseState.ExponentialSignOrDigit;
				break;
			case StringParsers.EFloatParseState.ExponentialSignOrDigit:
				if (flag10)
				{
					efloatParseState = StringParsers.EFloatParseState.ExponentialDigit;
					i--;
				}
				else if (c == '+')
				{
					efloatParseState = StringParsers.EFloatParseState.ExponentialDigit;
				}
				else if (c == '-')
				{
					flag9 = true;
					efloatParseState = StringParsers.EFloatParseState.ExponentialDigit;
				}
				else
				{
					if (!char.IsWhiteSpace(c))
					{
						if (!_tryParse)
						{
							_exception = new FormatException(string.Format("Unknown char: {0} (input: '{1}')", c, _input));
						}
						return false;
					}
					efloatParseState = StringParsers.EFloatParseState.TrailingWs;
					i--;
				}
				break;
			case StringParsers.EFloatParseState.ExponentialDigit:
				if (flag10)
				{
					j = j * 10 + num3;
				}
				else
				{
					if (!char.IsWhiteSpace(c))
					{
						if (!_tryParse)
						{
							_exception = new FormatException(string.Format("Unknown char: {0} (input: '{1}')", c, _input));
						}
						return false;
					}
					efloatParseState = StringParsers.EFloatParseState.TrailingWs;
					i--;
				}
				break;
			case StringParsers.EFloatParseState.TrailingWs:
				if (!flag2 || !char.IsWhiteSpace(c))
				{
					if (!_tryParse)
					{
						_exception = new FormatException(string.Format("Unknown char: {0} (input: '{1}')", c, _input));
					}
					return false;
				}
				break;
			}
			if (efloatParseState > StringParsers.EFloatParseState.TrailingWs)
			{
				break;
			}
			i++;
		}
		if (num != 0.0)
		{
			if (flag8)
			{
				num *= -1.0;
			}
			if (flag9)
			{
				j *= -1;
			}
			j -= num2;
			if (j < 0)
			{
				flag9 = true;
				j *= -1;
			}
			double num4 = 1.0;
			double num5 = 10.0;
			while (j > 0)
			{
				if (j % 2 == 1)
				{
					num4 *= num5;
				}
				j >>= 1;
				num5 *= num5;
			}
			if (flag9)
			{
				num /= num4;
			}
			else
			{
				num *= num4;
			}
		}
		if (double.IsPositiveInfinity(num) || double.IsNegativeInfinity(num))
		{
			if (!_tryParse)
			{
				_exception = new OverflowException();
			}
			return false;
		}
		_result = num;
		return true;
	}

	// Token: 0x060090E3 RID: 37091 RVA: 0x0039C9D0 File Offset: 0x0039ABD0
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool internalParseBool(string _input, bool _tryParse, out bool _result, out Exception _exception, bool _ignoreCase, int _startIndex, int _endIndex)
	{
		_result = false;
		_exception = null;
		if (_input == null)
		{
			if (!_tryParse)
			{
				_exception = new ArgumentNullException("_input");
			}
			return false;
		}
		if (_input.Length == 0)
		{
			if (!_tryParse)
			{
				_exception = new FormatException("Value is not equivalent to either TrueString or FalseString (input='" + _input + "')");
			}
			return false;
		}
		if (_startIndex < 0 || _startIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exception = new ArgumentException("_startIndex out of range (input='" + _input + "')", "_startIndex");
			}
			return false;
		}
		if (_endIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exception = new ArgumentException("_endIndex out of range (input='" + _input + "')", "_endIndex");
			}
			return false;
		}
		if (_endIndex < 0)
		{
			_endIndex = _input.Length - 1;
		}
		while (_startIndex <= _endIndex && char.IsWhiteSpace(_input[_startIndex]))
		{
			_startIndex++;
		}
		if (_startIndex > _endIndex)
		{
			if (!_tryParse)
			{
				_exception = new FormatException("Input string was not in the correct format (input='" + _input + "')");
			}
			return false;
		}
		while (_endIndex >= 0 && char.IsWhiteSpace(_input[_endIndex]))
		{
			_endIndex--;
		}
		if (_startIndex > _endIndex || _endIndex < 0)
		{
			if (!_tryParse)
			{
				_exception = new FormatException("Input string was not in the correct format (input='" + _input + "')");
			}
			return false;
		}
		StringComparison comparisonType = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
		if (string.Compare(_input, _startIndex, bool.TrueString, 0, _endIndex - _startIndex + 1, comparisonType) == 0)
		{
			_result = true;
			return true;
		}
		if (string.Compare(_input, _startIndex, bool.FalseString, 0, _endIndex - _startIndex + 1, comparisonType) == 0)
		{
			_result = false;
			return true;
		}
		if (!_tryParse)
		{
			_exception = new FormatException("Value is not equivalent to either TrueString or FalseString (input='" + _input + "')");
		}
		return false;
	}

	// Token: 0x060090E4 RID: 37092 RVA: 0x0039CB64 File Offset: 0x0039AD64
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool internalParseInt64(string _input, NumberStyles _numberStyle, bool _tryParse, out long _resultSigned, out ulong _resultUnsigned, out Exception _exc, bool _signedResult, int _startIndex, int _endIndex)
	{
		if (_numberStyle != NumberStyles.Integer)
		{
			return StringParsers.internalParseInt64Advanced(_input, _numberStyle, _tryParse, out _resultSigned, out _resultUnsigned, out _exc, _signedResult, _startIndex, _endIndex);
		}
		_resultSigned = 0L;
		_resultUnsigned = 0UL;
		_exc = null;
		if (_input == null)
		{
			if (!_tryParse)
			{
				_exc = new ArgumentNullException("_input");
			}
			return false;
		}
		if (_input.Length == 0)
		{
			if (!_tryParse)
			{
				_exc = new FormatException("Input string was not in the correct format: s.Length==0.");
			}
			return false;
		}
		if (_startIndex < 0 || _startIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exc = new ArgumentException("_startIndex out of range (input='" + _input + "')", "_startIndex");
			}
			return false;
		}
		if (_endIndex < 0)
		{
			_endIndex = _input.Length - 1;
		}
		if (_endIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exc = new ArgumentException("_endIndex out of range (input='" + _input + "')", "_endIndex");
			}
			return false;
		}
		int num = _startIndex;
		while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
		{
			num++;
		}
		if (num > _endIndex)
		{
			if (!_tryParse)
			{
				_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
			}
			return false;
		}
		bool flag = false;
		if (_input[num] == '-')
		{
			flag = true;
			num++;
		}
		else if (_input[num] == '+')
		{
			num++;
		}
		if (num > _endIndex)
		{
			if (!_tryParse)
			{
				_exc = new FormatException("Input string only has a sign (input='" + _input + "')");
			}
			return false;
		}
		ulong num2 = 0UL;
		int num3 = 0;
		do
		{
			char c = _input[num];
			if ((c ^ '0') > '\t')
			{
				break;
			}
			num3++;
			try
			{
				num2 = checked(num2 * 10UL + (ulong)(unchecked((long)(c ^ '0'))));
			}
			catch (OverflowException)
			{
				if (!_tryParse)
				{
					_exc = new OverflowException("Value too large or too small (input='" + _input + "')");
				}
				return false;
			}
			num++;
		}
		while (num <= _endIndex);
		if (num3 == 0)
		{
			if (!_tryParse)
			{
				_exc = new FormatException("Input string was not in the correct format: nDigits == 0 (input='" + _input + "')");
			}
			return false;
		}
		if (num <= _endIndex)
		{
			while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
			{
				num++;
			}
			if (num <= _endIndex && _input[num] != '\0')
			{
				if (!_tryParse)
				{
					_exc = new FormatException(string.Format("Input string was not in the correct format: Did not parse entire string. pos = {0} endIndex = {1} (input='{2}')", num, _endIndex, _input));
				}
				return false;
			}
		}
		if (_signedResult)
		{
			if (flag)
			{
				if (num2 > 9223372036854775808UL)
				{
					if (!_tryParse)
					{
						_exc = new OverflowException("Value too large or too small (input='" + _input + "')");
					}
					return false;
				}
				_resultSigned = (long)(ulong.MaxValue * (num2 - 1UL) - 1UL);
				return true;
			}
			else
			{
				if (num2 > 9223372036854775807UL)
				{
					if (!_tryParse)
					{
						_exc = new OverflowException("Value too large or too small (input='" + _input + "')");
					}
					return false;
				}
				_resultSigned = (long)num2;
				return true;
			}
		}
		else
		{
			if (flag && num2 > 0UL)
			{
				if (!_tryParse)
				{
					_exc = new OverflowException("Negative number (input='" + _input + "')");
				}
				return false;
			}
			_resultUnsigned = num2;
			return true;
		}
		bool result;
		return result;
	}

	// Token: 0x060090E5 RID: 37093 RVA: 0x0039CE28 File Offset: 0x0039B028
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool internalParseInt64Advanced(string _input, NumberStyles _numberStyle, bool _tryParse, out long _resultSigned, out ulong _resultUnsigned, out Exception _exc, bool _signedResult, int _startIndex, int _endIndex)
	{
		_resultSigned = 0L;
		_resultUnsigned = 0UL;
		_exc = null;
		if (_input == null)
		{
			if (!_tryParse)
			{
				_exc = new ArgumentNullException("_input");
			}
			return false;
		}
		if (_input.Length == 0)
		{
			if (!_tryParse)
			{
				_exc = new FormatException("Input string was not in the correct format: s.Length==0.");
			}
			return false;
		}
		if (_startIndex < 0 || _startIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exc = new ArgumentException("_startIndex out of range (input='" + _input + "')", "_startIndex");
			}
			return false;
		}
		if (_endIndex < 0)
		{
			_endIndex = _input.Length - 1;
		}
		if (_endIndex >= _input.Length)
		{
			if (!_tryParse)
			{
				_exc = new ArgumentException("_endIndex out of range (input='" + _input + "')", "_endIndex");
			}
			return false;
		}
		if ((_numberStyle & NumberStyles.AllowHexSpecifier) != NumberStyles.None)
		{
			NumberStyles numberStyles = _numberStyle ^ NumberStyles.AllowHexSpecifier;
			if ((numberStyles & NumberStyles.AllowLeadingWhite) != NumberStyles.None)
			{
				numberStyles ^= NumberStyles.AllowLeadingWhite;
			}
			if ((numberStyles & NumberStyles.AllowTrailingWhite) != NumberStyles.None)
			{
				numberStyles ^= NumberStyles.AllowTrailingWhite;
			}
			if (numberStyles != NumberStyles.None)
			{
				if (!_tryParse)
				{
					_exc = new ArgumentException("With AllowHexSpecifier only AllowLeadingWhite and AllowTrailingWhite are permitted (input='" + _input + "')");
				}
				return false;
			}
		}
		else if (_numberStyle > NumberStyles.Any)
		{
			if (!_tryParse)
			{
				_exc = new ArgumentException("Not a valid number style (input='" + _input + "')");
			}
			return false;
		}
		bool flag = (_numberStyle & NumberStyles.AllowCurrencySymbol) > NumberStyles.None;
		bool flag2 = (_numberStyle & NumberStyles.AllowHexSpecifier) > NumberStyles.None;
		bool flag3 = (_numberStyle & NumberStyles.AllowThousands) > NumberStyles.None;
		bool flag4 = (_numberStyle & NumberStyles.AllowDecimalPoint) > NumberStyles.None;
		bool flag5 = (_numberStyle & NumberStyles.AllowParentheses) > NumberStyles.None;
		bool flag6 = (_numberStyle & NumberStyles.AllowTrailingSign) > NumberStyles.None;
		bool flag7 = (_numberStyle & NumberStyles.AllowLeadingSign) > NumberStyles.None;
		bool flag8 = (_numberStyle & NumberStyles.AllowTrailingWhite) > NumberStyles.None;
		bool flag9 = (_numberStyle & NumberStyles.AllowLeadingWhite) > NumberStyles.None;
		int num = _startIndex;
		if (flag9)
		{
			while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
			{
				num++;
			}
			if (num > _endIndex)
			{
				if (!_tryParse)
				{
					_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
				}
				return false;
			}
		}
		bool flag10 = false;
		bool flag11 = false;
		bool flag12 = false;
		bool flag13 = false;
		if (flag5 && _input[num] == '(')
		{
			flag10 = true;
			flag12 = true;
			flag11 = true;
			num++;
			if (flag9)
			{
				while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
				{
					num++;
				}
				if (num > _endIndex)
				{
					if (!_tryParse)
					{
						_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
					}
					return false;
				}
			}
			if (_input[num] == '-')
			{
				if (!_tryParse)
				{
					_exc = new FormatException("Input string was not in the correct format: Has Negative Sign (input='" + _input + "')");
				}
				return false;
			}
			if (_input[num] == '+')
			{
				if (!_tryParse)
				{
					_exc = new FormatException("Input string was not in the correct format: Has Positive Sign (input='" + _input + "')");
				}
				return false;
			}
		}
		if (flag7 && !flag12)
		{
			if (_input[num] == '-')
			{
				flag11 = true;
				flag12 = true;
				num++;
			}
			else if (_input[num] == '+')
			{
				flag12 = true;
				num++;
			}
			if (flag12 && flag9)
			{
				while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
				{
					num++;
				}
				if (num > _endIndex)
				{
					if (!_tryParse)
					{
						_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
					}
					return false;
				}
			}
		}
		if (flag)
		{
			if (_input[num] == '¤')
			{
				flag13 = true;
				num++;
			}
			if (flag13)
			{
				if (flag9)
				{
					while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
					{
						num++;
					}
					if (num > _endIndex)
					{
						if (!_tryParse)
						{
							_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
						}
						return false;
					}
				}
				if (!flag12 && flag7)
				{
					if (_input[num] == '-')
					{
						flag11 = true;
						flag12 = true;
						num++;
					}
					else if (_input[num] == '+')
					{
						flag11 = false;
						flag12 = true;
						num++;
					}
					if (flag12 && flag9)
					{
						while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
						{
							num++;
						}
						if (num > _endIndex)
						{
							if (!_tryParse)
							{
								_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
							}
							return false;
						}
					}
				}
			}
		}
		ulong num2 = 0UL;
		int num3 = 0;
		bool flag14 = false;
		for (;;)
		{
			char c = _input[num];
			if ((c ^ '0') > '\t' && (!flag2 || ((c < 'A' || c > 'F') && (c < 'a' || c > 'f'))))
			{
				if (!flag3 || !StringParsers.findOther(ref num, _input, ','))
				{
					if (flag14 || !flag4 || !StringParsers.findOther(ref num, _input, '.'))
					{
						goto IL_52E;
					}
					flag14 = true;
				}
			}
			else if (flag2)
			{
				num3++;
				int num4;
				if ((c ^ '0') <= '\t')
				{
					num4 = (int)(c - '0');
				}
				else if (c < 'a')
				{
					num4 = (int)(c - 'A' + '\n');
				}
				else
				{
					num4 = (int)(c - 'a' + '\n');
				}
				try
				{
					num2 = checked(num2 * 16UL + (ulong)num4);
				}
				catch (OverflowException ex)
				{
					if (!_tryParse)
					{
						_exc = ex;
					}
					return false;
				}
				num++;
			}
			else if (flag14)
			{
				num3++;
				if (c != '0')
				{
					break;
				}
				num++;
			}
			else
			{
				num3++;
				try
				{
					num2 = checked(num2 * 10UL + (ulong)(unchecked((long)(checked(c - '0')))));
				}
				catch (OverflowException)
				{
					if (!_tryParse)
					{
						_exc = new OverflowException("Value too large or too small (input='" + _input + "')");
					}
					return false;
				}
				num++;
			}
			if (num > _endIndex)
			{
				goto IL_52E;
			}
		}
		if (!_tryParse)
		{
			_exc = new OverflowException("Value too large or too small (input='" + _input + "')");
		}
		return false;
		IL_52E:
		if (num3 == 0)
		{
			if (!_tryParse)
			{
				_exc = new FormatException("Input string was not in the correct format: nDigits == 0 (input='" + _input + "')");
			}
			return false;
		}
		if (flag6 && !flag12)
		{
			if (_input[num] == '-')
			{
				flag11 = true;
				flag12 = true;
				num++;
			}
			else if (_input[num] == '+')
			{
				flag11 = false;
				flag12 = true;
				num++;
			}
			if (flag12)
			{
				if (flag8)
				{
					while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
					{
						num++;
					}
					if (num > _endIndex)
					{
						if (!_tryParse)
						{
							_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
						}
						return false;
					}
				}
				if (flag && _input[num] == '¤')
				{
					flag13 = true;
					num++;
				}
			}
		}
		if (flag && !flag13)
		{
			if (_input[num] == '¤')
			{
				flag13 = true;
				num++;
			}
			if (flag13 && num < _input.Length)
			{
				if (flag8)
				{
					while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
					{
						num++;
					}
					if (num > _endIndex)
					{
						if (!_tryParse)
						{
							_exc = new FormatException("Input string was not in the correct format (input='" + _input + "')");
						}
						return false;
					}
				}
				if (!flag12 && flag6)
				{
					if (_input[num] == '-')
					{
						flag11 = true;
						flag12 = true;
						num++;
					}
					else if (_input[num] == '+')
					{
						flag11 = false;
						flag12 = true;
						num++;
					}
				}
			}
		}
		if (flag8 && num <= _endIndex)
		{
			while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
			{
				num++;
			}
		}
		if (flag10)
		{
			if (num > _endIndex || _input[num++] != ')')
			{
				if (!_tryParse)
				{
					_exc = new FormatException("Input string was not in the correct format: No room for close parens (input='" + _input + "')");
				}
				return false;
			}
			if (flag8 && num <= _endIndex)
			{
				while (num <= _endIndex && char.IsWhiteSpace(_input[num]))
				{
					num++;
				}
			}
		}
		if (num <= _endIndex && _input[num] != '\0')
		{
			if (!_tryParse)
			{
				_exc = new FormatException(string.Format("Input string was not in the correct format: Did not parse entire string. pos = {0} endIndex = {1} (input='{2}')", num, _endIndex, _input));
			}
			return false;
		}
		if (_signedResult)
		{
			if (flag11)
			{
				ulong num5 = 9223372036854775808UL;
				if (num2 > num5)
				{
					if (!_tryParse)
					{
						_exc = new OverflowException("Value too large or too small (input='" + _input + "')");
					}
					return false;
				}
				_resultSigned = (long)(ulong.MaxValue * (num2 - 1UL) - 1UL);
				return true;
			}
			else
			{
				ulong num6 = 9223372036854775807UL;
				if (num2 > num6)
				{
					if (!_tryParse)
					{
						_exc = new OverflowException("Value too large or too small (input='" + _input + "')");
					}
					return false;
				}
				_resultSigned = (long)num2;
				return true;
			}
		}
		else
		{
			if (flag11)
			{
				if (!_tryParse)
				{
					_exc = new OverflowException("Negative number (input='" + _input + "')");
				}
				return false;
			}
			_resultUnsigned = num2;
			return true;
		}
		bool result;
		return result;
	}

	// Token: 0x04006F55 RID: 28501
	[PublicizedFrom(EAccessModifier.Private)]
	public const char NEGATIVE_SIGN = '-';

	// Token: 0x04006F56 RID: 28502
	[PublicizedFrom(EAccessModifier.Private)]
	public const char POSITIVE_SIGN = '+';

	// Token: 0x04006F57 RID: 28503
	[PublicizedFrom(EAccessModifier.Private)]
	public const char CURRENCY_SYMBOL = '¤';

	// Token: 0x04006F58 RID: 28504
	[PublicizedFrom(EAccessModifier.Private)]
	public const char DECIMAL_SEP = '.';

	// Token: 0x04006F59 RID: 28505
	[PublicizedFrom(EAccessModifier.Private)]
	public const char THOUSANDS_SEP = ',';

	// Token: 0x04006F5A RID: 28506
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string NAN_SYMBOL = "NaN";

	// Token: 0x04006F5B RID: 28507
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string POSITIVE_INFINITY_SYMBOL = "Infinity";

	// Token: 0x04006F5C RID: 28508
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string NEGATIVE_INFINITY_SYMBOL = "-Infinity";

	// Token: 0x0200121E RID: 4638
	public struct SeparatorPositions
	{
		// Token: 0x060090E7 RID: 37095 RVA: 0x0039D67C File Offset: 0x0039B87C
		public SeparatorPositions(int _tmp)
		{
			this.TotalFound = 0;
			this.Sep1 = -1;
			this.Sep2 = -1;
			this.Sep3 = -1;
			this.Sep4 = -1;
		}

		// Token: 0x04006F5D RID: 28509
		public int TotalFound;

		// Token: 0x04006F5E RID: 28510
		public int Sep1;

		// Token: 0x04006F5F RID: 28511
		public int Sep2;

		// Token: 0x04006F60 RID: 28512
		public int Sep3;

		// Token: 0x04006F61 RID: 28513
		public int Sep4;
	}

	// Token: 0x0200121F RID: 4639
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EFloatParseState
	{
		// Token: 0x04006F63 RID: 28515
		SignOrIntegralDigit,
		// Token: 0x04006F64 RID: 28516
		IntegralDigit,
		// Token: 0x04006F65 RID: 28517
		DecimalDigit,
		// Token: 0x04006F66 RID: 28518
		ExponentialTest,
		// Token: 0x04006F67 RID: 28519
		ExponentialSignOrDigit,
		// Token: 0x04006F68 RID: 28520
		ExponentialDigit,
		// Token: 0x04006F69 RID: 28521
		TrailingWs
	}
}

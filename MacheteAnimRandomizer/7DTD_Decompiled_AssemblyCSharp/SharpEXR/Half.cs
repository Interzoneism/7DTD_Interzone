using System;
using System.Diagnostics;
using System.Globalization;

namespace SharpEXR
{
	// Token: 0x02001408 RID: 5128
	[Serializable]
	public struct Half : IComparable, IFormattable, IConvertible, IComparable<Half>, IEquatable<Half>
	{
		// Token: 0x06009FA2 RID: 40866 RVA: 0x003F527C File Offset: 0x003F347C
		public Half(float value)
		{
			this = HalfHelper.SingleToHalf(value);
		}

		// Token: 0x06009FA3 RID: 40867 RVA: 0x003F528A File Offset: 0x003F348A
		public Half(int value)
		{
			this = new Half((float)value);
		}

		// Token: 0x06009FA4 RID: 40868 RVA: 0x003F528A File Offset: 0x003F348A
		public Half(long value)
		{
			this = new Half((float)value);
		}

		// Token: 0x06009FA5 RID: 40869 RVA: 0x003F528A File Offset: 0x003F348A
		public Half(double value)
		{
			this = new Half((float)value);
		}

		// Token: 0x06009FA6 RID: 40870 RVA: 0x003F5294 File Offset: 0x003F3494
		public Half(decimal value)
		{
			this = new Half((float)value);
		}

		// Token: 0x06009FA7 RID: 40871 RVA: 0x003F52A3 File Offset: 0x003F34A3
		public Half(uint value)
		{
			this = new Half(value);
		}

		// Token: 0x06009FA8 RID: 40872 RVA: 0x003F52A3 File Offset: 0x003F34A3
		public Half(ulong value)
		{
			this = new Half(value);
		}

		// Token: 0x06009FA9 RID: 40873 RVA: 0x003F52AE File Offset: 0x003F34AE
		public static Half Negate(Half half)
		{
			return -half;
		}

		// Token: 0x06009FAA RID: 40874 RVA: 0x003F52B6 File Offset: 0x003F34B6
		public static Half Add(Half half1, Half half2)
		{
			return half1 + half2;
		}

		// Token: 0x06009FAB RID: 40875 RVA: 0x003F52BF File Offset: 0x003F34BF
		public static Half Subtract(Half half1, Half half2)
		{
			return half1 - half2;
		}

		// Token: 0x06009FAC RID: 40876 RVA: 0x003F52C8 File Offset: 0x003F34C8
		public static Half Multiply(Half half1, Half half2)
		{
			return half1 * half2;
		}

		// Token: 0x06009FAD RID: 40877 RVA: 0x003F52D1 File Offset: 0x003F34D1
		public static Half Divide(Half half1, Half half2)
		{
			return half1 / half2;
		}

		// Token: 0x06009FAE RID: 40878 RVA: 0x00112051 File Offset: 0x00110251
		public static Half operator +(Half half)
		{
			return half;
		}

		// Token: 0x06009FAF RID: 40879 RVA: 0x003F52DA File Offset: 0x003F34DA
		public static Half operator -(Half half)
		{
			return HalfHelper.Negate(half);
		}

		// Token: 0x06009FB0 RID: 40880 RVA: 0x003F52E2 File Offset: 0x003F34E2
		public static Half operator ++(Half half)
		{
			return (Half)(half + 1f);
		}

		// Token: 0x06009FB1 RID: 40881 RVA: 0x003F52F5 File Offset: 0x003F34F5
		public static Half operator --(Half half)
		{
			return (Half)(half - 1f);
		}

		// Token: 0x06009FB2 RID: 40882 RVA: 0x003F5308 File Offset: 0x003F3508
		public static Half operator +(Half half1, Half half2)
		{
			return (Half)(half1 + half2);
		}

		// Token: 0x06009FB3 RID: 40883 RVA: 0x003F531E File Offset: 0x003F351E
		public static Half operator -(Half half1, Half half2)
		{
			return (Half)(half1 - half2);
		}

		// Token: 0x06009FB4 RID: 40884 RVA: 0x003F5334 File Offset: 0x003F3534
		public static Half operator *(Half half1, Half half2)
		{
			return (Half)(half1 * half2);
		}

		// Token: 0x06009FB5 RID: 40885 RVA: 0x003F534A File Offset: 0x003F354A
		public static Half operator /(Half half1, Half half2)
		{
			return (Half)(half1 / half2);
		}

		// Token: 0x06009FB6 RID: 40886 RVA: 0x003F5360 File Offset: 0x003F3560
		public static bool operator ==(Half half1, Half half2)
		{
			return !Half.IsNaN(half1) && half1.value == half2.value;
		}

		// Token: 0x06009FB7 RID: 40887 RVA: 0x003F537A File Offset: 0x003F357A
		public static bool operator !=(Half half1, Half half2)
		{
			return half1.value != half2.value;
		}

		// Token: 0x06009FB8 RID: 40888 RVA: 0x003F538D File Offset: 0x003F358D
		public static bool operator <(Half half1, Half half2)
		{
			return half1 < half2;
		}

		// Token: 0x06009FB9 RID: 40889 RVA: 0x003F539F File Offset: 0x003F359F
		public static bool operator >(Half half1, Half half2)
		{
			return half1 > half2;
		}

		// Token: 0x06009FBA RID: 40890 RVA: 0x003F53B1 File Offset: 0x003F35B1
		public static bool operator <=(Half half1, Half half2)
		{
			return half1 == half2 || half1 < half2;
		}

		// Token: 0x06009FBB RID: 40891 RVA: 0x003F53C5 File Offset: 0x003F35C5
		public static bool operator >=(Half half1, Half half2)
		{
			return half1 == half2 || half1 > half2;
		}

		// Token: 0x06009FBC RID: 40892 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static implicit operator Half(byte value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FBD RID: 40893 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static implicit operator Half(short value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FBE RID: 40894 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static implicit operator Half(char value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FBF RID: 40895 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static implicit operator Half(int value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FC0 RID: 40896 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static implicit operator Half(long value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FC1 RID: 40897 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static explicit operator Half(float value)
		{
			return new Half(value);
		}

		// Token: 0x06009FC2 RID: 40898 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static explicit operator Half(double value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FC3 RID: 40899 RVA: 0x003F53E2 File Offset: 0x003F35E2
		public static explicit operator Half(decimal value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FC4 RID: 40900 RVA: 0x003F53F0 File Offset: 0x003F35F0
		public static explicit operator byte(Half value)
		{
			return (byte)value;
		}

		// Token: 0x06009FC5 RID: 40901 RVA: 0x003F53FA File Offset: 0x003F35FA
		public static explicit operator char(Half value)
		{
			return (char)value;
		}

		// Token: 0x06009FC6 RID: 40902 RVA: 0x003F5404 File Offset: 0x003F3604
		public static explicit operator short(Half value)
		{
			return (short)value;
		}

		// Token: 0x06009FC7 RID: 40903 RVA: 0x003F540E File Offset: 0x003F360E
		public static explicit operator int(Half value)
		{
			return (int)value;
		}

		// Token: 0x06009FC8 RID: 40904 RVA: 0x003F5418 File Offset: 0x003F3618
		public static explicit operator long(Half value)
		{
			return (long)value;
		}

		// Token: 0x06009FC9 RID: 40905 RVA: 0x003F5422 File Offset: 0x003F3622
		public static implicit operator float(Half value)
		{
			return HalfHelper.HalfToSingle(value);
		}

		// Token: 0x06009FCA RID: 40906 RVA: 0x003F542B File Offset: 0x003F362B
		public static implicit operator double(Half value)
		{
			return (double)value;
		}

		// Token: 0x06009FCB RID: 40907 RVA: 0x003F5435 File Offset: 0x003F3635
		public static explicit operator decimal(Half value)
		{
			return (decimal)value;
		}

		// Token: 0x06009FCC RID: 40908 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static implicit operator Half(sbyte value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FCD RID: 40909 RVA: 0x003F53D9 File Offset: 0x003F35D9
		public static implicit operator Half(ushort value)
		{
			return new Half((float)value);
		}

		// Token: 0x06009FCE RID: 40910 RVA: 0x003F5443 File Offset: 0x003F3643
		public static implicit operator Half(uint value)
		{
			return new Half(value);
		}

		// Token: 0x06009FCF RID: 40911 RVA: 0x003F5443 File Offset: 0x003F3643
		public static implicit operator Half(ulong value)
		{
			return new Half(value);
		}

		// Token: 0x06009FD0 RID: 40912 RVA: 0x003F544D File Offset: 0x003F364D
		public static explicit operator sbyte(Half value)
		{
			return (sbyte)value;
		}

		// Token: 0x06009FD1 RID: 40913 RVA: 0x003F53FA File Offset: 0x003F35FA
		public static explicit operator ushort(Half value)
		{
			return (ushort)value;
		}

		// Token: 0x06009FD2 RID: 40914 RVA: 0x003F5457 File Offset: 0x003F3657
		public static explicit operator uint(Half value)
		{
			return (uint)value;
		}

		// Token: 0x06009FD3 RID: 40915 RVA: 0x003F5461 File Offset: 0x003F3661
		public static explicit operator ulong(Half value)
		{
			return (ulong)value;
		}

		// Token: 0x06009FD4 RID: 40916 RVA: 0x003F546C File Offset: 0x003F366C
		public int CompareTo(Half other)
		{
			int result = 0;
			if (this < other)
			{
				result = -1;
			}
			else if (this > other)
			{
				result = 1;
			}
			else if (this != other)
			{
				if (!Half.IsNaN(this))
				{
					result = 1;
				}
				else if (!Half.IsNaN(other))
				{
					result = -1;
				}
			}
			return result;
		}

		// Token: 0x06009FD5 RID: 40917 RVA: 0x003F54CC File Offset: 0x003F36CC
		public int CompareTo(object obj)
		{
			int result;
			if (obj == null)
			{
				result = 1;
			}
			else
			{
				if (!(obj is Half))
				{
					throw new ArgumentException("Object must be of type Half.");
				}
				result = this.CompareTo((Half)obj);
			}
			return result;
		}

		// Token: 0x06009FD6 RID: 40918 RVA: 0x003F5505 File Offset: 0x003F3705
		public bool Equals(Half other)
		{
			return other == this || (Half.IsNaN(other) && Half.IsNaN(this));
		}

		// Token: 0x06009FD7 RID: 40919 RVA: 0x003F552C File Offset: 0x003F372C
		public override bool Equals(object obj)
		{
			bool result = false;
			if (obj is Half)
			{
				Half half = (Half)obj;
				if (half == this || (Half.IsNaN(half) && Half.IsNaN(this)))
				{
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06009FD8 RID: 40920 RVA: 0x003F5570 File Offset: 0x003F3770
		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		// Token: 0x06009FD9 RID: 40921 RVA: 0x00047102 File Offset: 0x00045302
		public TypeCode GetTypeCode()
		{
			return (TypeCode)255;
		}

		// Token: 0x06009FDA RID: 40922 RVA: 0x003F557D File Offset: 0x003F377D
		public static byte[] GetBytes(Half value)
		{
			return BitConverter.GetBytes(value.value);
		}

		// Token: 0x06009FDB RID: 40923 RVA: 0x003F558A File Offset: 0x003F378A
		public static ushort GetBits(Half value)
		{
			return value.value;
		}

		// Token: 0x06009FDC RID: 40924 RVA: 0x003F5592 File Offset: 0x003F3792
		public static Half ToHalf(byte[] value, int startIndex)
		{
			return Half.ToHalf((ushort)BitConverter.ToInt16(value, startIndex));
		}

		// Token: 0x06009FDD RID: 40925 RVA: 0x003F55A4 File Offset: 0x003F37A4
		public static Half ToHalf(ushort bits)
		{
			return new Half
			{
				value = bits
			};
		}

		// Token: 0x06009FDE RID: 40926 RVA: 0x003F55C2 File Offset: 0x003F37C2
		public static int Sign(Half value)
		{
			if (value < 0)
			{
				return -1;
			}
			if (value > 0)
			{
				return 1;
			}
			if (value != 0)
			{
				throw new ArithmeticException("Function does not accept floating point Not-a-Number values.");
			}
			return 0;
		}

		// Token: 0x06009FDF RID: 40927 RVA: 0x003F55FE File Offset: 0x003F37FE
		public static Half Abs(Half value)
		{
			return HalfHelper.Abs(value);
		}

		// Token: 0x06009FE0 RID: 40928 RVA: 0x003F5606 File Offset: 0x003F3806
		public static Half Max(Half value1, Half value2)
		{
			if (!(value1 < value2))
			{
				return value1;
			}
			return value2;
		}

		// Token: 0x06009FE1 RID: 40929 RVA: 0x003F5614 File Offset: 0x003F3814
		public static Half Min(Half value1, Half value2)
		{
			if (!(value1 < value2))
			{
				return value2;
			}
			return value1;
		}

		// Token: 0x06009FE2 RID: 40930 RVA: 0x003F5622 File Offset: 0x003F3822
		public static bool IsNaN(Half half)
		{
			return HalfHelper.IsNaN(half);
		}

		// Token: 0x06009FE3 RID: 40931 RVA: 0x003F562A File Offset: 0x003F382A
		public static bool IsInfinity(Half half)
		{
			return HalfHelper.IsInfinity(half);
		}

		// Token: 0x06009FE4 RID: 40932 RVA: 0x003F5632 File Offset: 0x003F3832
		public static bool IsNegativeInfinity(Half half)
		{
			return HalfHelper.IsNegativeInfinity(half);
		}

		// Token: 0x06009FE5 RID: 40933 RVA: 0x003F563A File Offset: 0x003F383A
		public static bool IsPositiveInfinity(Half half)
		{
			return HalfHelper.IsPositiveInfinity(half);
		}

		// Token: 0x06009FE6 RID: 40934 RVA: 0x003F5642 File Offset: 0x003F3842
		public static Half Parse(string value)
		{
			return (Half)float.Parse(value, CultureInfo.InvariantCulture);
		}

		// Token: 0x06009FE7 RID: 40935 RVA: 0x003F5654 File Offset: 0x003F3854
		public static Half Parse(string value, IFormatProvider provider)
		{
			return (Half)float.Parse(value, provider);
		}

		// Token: 0x06009FE8 RID: 40936 RVA: 0x003F5662 File Offset: 0x003F3862
		public static Half Parse(string value, NumberStyles style)
		{
			return (Half)float.Parse(value, style, CultureInfo.InvariantCulture);
		}

		// Token: 0x06009FE9 RID: 40937 RVA: 0x003F5675 File Offset: 0x003F3875
		public static Half Parse(string value, NumberStyles style, IFormatProvider provider)
		{
			return (Half)float.Parse(value, style, provider);
		}

		// Token: 0x06009FEA RID: 40938 RVA: 0x003F5684 File Offset: 0x003F3884
		public static bool TryParse(string value, out Half result)
		{
			float num;
			if (float.TryParse(value, out num))
			{
				result = (Half)num;
				return true;
			}
			result = default(Half);
			return false;
		}

		// Token: 0x06009FEB RID: 40939 RVA: 0x003F56B4 File Offset: 0x003F38B4
		public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out Half result)
		{
			bool result2 = false;
			float num;
			if (float.TryParse(value, style, provider, out num))
			{
				result = (Half)num;
				result2 = true;
			}
			else
			{
				result = default(Half);
			}
			return result2;
		}

		// Token: 0x06009FEC RID: 40940 RVA: 0x003F56E8 File Offset: 0x003F38E8
		public override string ToString()
		{
			return this.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x06009FED RID: 40941 RVA: 0x003F5710 File Offset: 0x003F3910
		public string ToString(IFormatProvider formatProvider)
		{
			return this.ToString(formatProvider);
		}

		// Token: 0x06009FEE RID: 40942 RVA: 0x003F5734 File Offset: 0x003F3934
		public string ToString(string format)
		{
			return this.ToString(format, CultureInfo.InvariantCulture);
		}

		// Token: 0x06009FEF RID: 40943 RVA: 0x003F575C File Offset: 0x003F395C
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.ToString(format, formatProvider);
		}

		// Token: 0x06009FF0 RID: 40944 RVA: 0x003F577F File Offset: 0x003F397F
		[PublicizedFrom(EAccessModifier.Private)]
		public float ToSingle(IFormatProvider provider)
		{
			return this;
		}

		// Token: 0x06009FF1 RID: 40945 RVA: 0x003F578D File Offset: 0x003F398D
		[PublicizedFrom(EAccessModifier.Private)]
		public TypeCode GetTypeCode()
		{
			return this.GetTypeCode();
		}

		// Token: 0x06009FF2 RID: 40946 RVA: 0x003F5795 File Offset: 0x003F3995
		[PublicizedFrom(EAccessModifier.Private)]
		public bool ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		// Token: 0x06009FF3 RID: 40947 RVA: 0x003F57A8 File Offset: 0x003F39A8
		[PublicizedFrom(EAccessModifier.Private)]
		public byte ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		// Token: 0x06009FF4 RID: 40948 RVA: 0x003F57BB File Offset: 0x003F39BB
		[PublicizedFrom(EAccessModifier.Private)]
		public char ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Invalid cast from '{0}' to '{1}'.", "Half", "Char"));
		}

		// Token: 0x06009FF5 RID: 40949 RVA: 0x003F57DB File Offset: 0x003F39DB
		[PublicizedFrom(EAccessModifier.Private)]
		public DateTime ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, "Invalid cast from '{0}' to '{1}'.", "Half", "DateTime"));
		}

		// Token: 0x06009FF6 RID: 40950 RVA: 0x003F57FB File Offset: 0x003F39FB
		[PublicizedFrom(EAccessModifier.Private)]
		public decimal ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		// Token: 0x06009FF7 RID: 40951 RVA: 0x003F580E File Offset: 0x003F3A0E
		[PublicizedFrom(EAccessModifier.Private)]
		public double ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		// Token: 0x06009FF8 RID: 40952 RVA: 0x003F5821 File Offset: 0x003F3A21
		[PublicizedFrom(EAccessModifier.Private)]
		public short ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		// Token: 0x06009FF9 RID: 40953 RVA: 0x003F5834 File Offset: 0x003F3A34
		[PublicizedFrom(EAccessModifier.Private)]
		public int ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		// Token: 0x06009FFA RID: 40954 RVA: 0x003F5847 File Offset: 0x003F3A47
		[PublicizedFrom(EAccessModifier.Private)]
		public long ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		// Token: 0x06009FFB RID: 40955 RVA: 0x003F585A File Offset: 0x003F3A5A
		[PublicizedFrom(EAccessModifier.Private)]
		public sbyte ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		// Token: 0x06009FFC RID: 40956 RVA: 0x003F586D File Offset: 0x003F3A6D
		[PublicizedFrom(EAccessModifier.Private)]
		public string ToString(IFormatProvider provider)
		{
			return Convert.ToString(this, CultureInfo.InvariantCulture);
		}

		// Token: 0x06009FFD RID: 40957 RVA: 0x003F5885 File Offset: 0x003F3A85
		[PublicizedFrom(EAccessModifier.Private)]
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			return ((IConvertible)this).ToType(conversionType, provider);
		}

		// Token: 0x06009FFE RID: 40958 RVA: 0x003F589F File Offset: 0x003F3A9F
		[PublicizedFrom(EAccessModifier.Private)]
		public ushort ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		// Token: 0x06009FFF RID: 40959 RVA: 0x003F58B2 File Offset: 0x003F3AB2
		[PublicizedFrom(EAccessModifier.Private)]
		public uint ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		// Token: 0x0600A000 RID: 40960 RVA: 0x003F58C5 File Offset: 0x003F3AC5
		[PublicizedFrom(EAccessModifier.Private)]
		public ulong ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		// Token: 0x04007ABC RID: 31420
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[PublicizedFrom(EAccessModifier.Internal)]
		[NonSerialized]
		public ushort value;

		// Token: 0x04007ABD RID: 31421
		public static readonly Half Epsilon = Half.ToHalf(1);

		// Token: 0x04007ABE RID: 31422
		public static readonly Half MaxValue = Half.ToHalf(31743);

		// Token: 0x04007ABF RID: 31423
		public static readonly Half MinValue = Half.ToHalf(64511);

		// Token: 0x04007AC0 RID: 31424
		public static readonly Half NaN = Half.ToHalf(65024);

		// Token: 0x04007AC1 RID: 31425
		public static readonly Half NegativeInfinity = Half.ToHalf(64512);

		// Token: 0x04007AC2 RID: 31426
		public static readonly Half PositiveInfinity = Half.ToHalf(31744);
	}
}

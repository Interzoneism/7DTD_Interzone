using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02000998 RID: 2456
public struct TextureFullArray : IEquatable<TextureFullArray>
{
	// Token: 0x170007C6 RID: 1990
	public unsafe long this[int index]
	{
		get
		{
			if (index < 0 || index >= 1)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is outside of the valid range of min: 0, max: {1}.", index, 1));
			}
			return *(ref this.values.FixedElementField + (IntPtr)index * 8);
		}
		set
		{
			if (index < 0 || index >= 1)
			{
				throw new IndexOutOfRangeException(string.Format("Index {0} is outside of the valid range of min: 0, max: {1}.", index, 1));
			}
			*(ref this.values.FixedElementField + (IntPtr)index * 8) = value;
		}
	}

	// Token: 0x06004A09 RID: 18953 RVA: 0x001D3E83 File Offset: 0x001D2083
	public TextureFullArray(long _fillValue)
	{
		this.Fill(_fillValue);
	}

	// Token: 0x170007C7 RID: 1991
	// (get) Token: 0x06004A0A RID: 18954 RVA: 0x001D3E8C File Offset: 0x001D208C
	public static TextureFullArray Default
	{
		get
		{
			return TextureFullArray._default;
		}
	}

	// Token: 0x06004A0B RID: 18955 RVA: 0x001D3E94 File Offset: 0x001D2094
	public unsafe void Fill(long _fillValue)
	{
		for (int i = 0; i < 1; i++)
		{
			*(ref this.values.FixedElementField + (IntPtr)i * 8) = _fillValue;
		}
	}

	// Token: 0x170007C8 RID: 1992
	// (get) Token: 0x06004A0C RID: 18956 RVA: 0x001D3EBF File Offset: 0x001D20BF
	public bool IsDefault
	{
		get
		{
			return this.Equals(TextureFullArray._default);
		}
	}

	// Token: 0x06004A0D RID: 18957 RVA: 0x001D3ECC File Offset: 0x001D20CC
	public unsafe void Read(BinaryReader _br, int count = 1)
	{
		int i;
		for (i = 0; i < count; i++)
		{
			long num = _br.ReadInt64();
			if (i < 1)
			{
				*(ref this.values.FixedElementField + (IntPtr)i * 8) = num;
			}
		}
		while (i < 1)
		{
			*(ref this.values.FixedElementField + (IntPtr)i * 8) = this.values.FixedElementField;
			i++;
		}
	}

	// Token: 0x06004A0E RID: 18958 RVA: 0x001D3F2C File Offset: 0x001D212C
	public unsafe void Write(BinaryWriter _bw)
	{
		for (int i = 0; i < 1; i++)
		{
			_bw.Write(*(ref this.values.FixedElementField + (IntPtr)i * 8));
		}
	}

	// Token: 0x06004A0F RID: 18959 RVA: 0x001D3F5C File Offset: 0x001D215C
	public unsafe bool Equals(TextureFullArray other)
	{
		for (int i = 0; i < 1; i++)
		{
			if (*(ref this.values.FixedElementField + (IntPtr)i * 8) != *(ref other.values.FixedElementField + (IntPtr)i * 8))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06004A10 RID: 18960 RVA: 0x001D3FA0 File Offset: 0x001D21A0
	public override bool Equals(object obj)
	{
		if (obj is TextureFullArray)
		{
			TextureFullArray other = (TextureFullArray)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06004A11 RID: 18961 RVA: 0x001D3FC8 File Offset: 0x001D21C8
	public override int GetHashCode()
	{
		int num = 17;
		for (int i = 0; i < 1; i++)
		{
			num = num * 31 + (ref this.values.FixedElementField + (IntPtr)i * 8).GetHashCode();
		}
		return num;
	}

	// Token: 0x06004A12 RID: 18962 RVA: 0x001D4000 File Offset: 0x001D2200
	public static bool operator ==(TextureFullArray left, TextureFullArray right)
	{
		return left.Equals(right);
	}

	// Token: 0x06004A13 RID: 18963 RVA: 0x001D400A File Offset: 0x001D220A
	public static bool operator !=(TextureFullArray left, TextureFullArray right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400391A RID: 14618
	[FixedBuffer(typeof(long), 1)]
	[PublicizedFrom(EAccessModifier.Private)]
	public TextureFullArray.<values>e__FixedBuffer values;

	// Token: 0x0400391B RID: 14619
	public static readonly TextureFullArray _default = new TextureFullArray(0L);

	// Token: 0x02000999 RID: 2457
	[CompilerGenerated]
	[UnsafeValueType]
	[StructLayout(LayoutKind.Sequential, Size = 8)]
	public struct <values>e__FixedBuffer
	{
		// Token: 0x0400391C RID: 14620
		public long FixedElementField;
	}
}

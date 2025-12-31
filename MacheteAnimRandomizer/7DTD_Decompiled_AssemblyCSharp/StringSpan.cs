using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02001228 RID: 4648
public readonly ref struct StringSpan
{
	// Token: 0x06009113 RID: 37139 RVA: 0x0039E683 File Offset: 0x0039C883
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan(ReadOnlySpan<char> span)
	{
		this.m_span = span;
	}

	// Token: 0x17000EFE RID: 3838
	// (get) Token: 0x06009114 RID: 37140 RVA: 0x0039E68C File Offset: 0x0039C88C
	public int Length
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_span.Length;
		}
	}

	// Token: 0x06009115 RID: 37141 RVA: 0x0039E699 File Offset: 0x0039C899
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<char> AsSpan()
	{
		return this.m_span;
	}

	// Token: 0x06009116 RID: 37142 RVA: 0x0039E6A1 File Offset: 0x0039C8A1
	public override string ToString()
	{
		return new string(this.m_span);
	}

	// Token: 0x17000EFF RID: 3839
	public char this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.m_span[index];
		}
	}

	// Token: 0x06009118 RID: 37144 RVA: 0x0039E6BC File Offset: 0x0039C8BC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan Slice(int start)
	{
		return this.m_span.Slice(start);
	}

	// Token: 0x06009119 RID: 37145 RVA: 0x0039E6CF File Offset: 0x0039C8CF
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan Slice(int start, int length)
	{
		return this.m_span.Slice(start, length);
	}

	// Token: 0x0600911A RID: 37146 RVA: 0x0039E6E3 File Offset: 0x0039C8E3
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(StringSpan other, StringComparison comparisonType = StringComparison.Ordinal)
	{
		return this.m_span == other.m_span || this.m_span.Equals(other.AsSpan(), comparisonType);
	}

	// Token: 0x0600911B RID: 37147 RVA: 0x0039E70D File Offset: 0x0039C90D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(StringSpan other, StringComparison comparisonType = StringComparison.Ordinal)
	{
		if (!(this.m_span == other.m_span))
		{
			return this.m_span.CompareTo(other.AsSpan(), comparisonType);
		}
		return 0;
	}

	// Token: 0x0600911C RID: 37148 RVA: 0x0039E737 File Offset: 0x0039C937
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(char value)
	{
		return this.IndexOf(value) >= 0;
	}

	// Token: 0x0600911D RID: 37149 RVA: 0x0039E746 File Offset: 0x0039C946
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(char value, StringComparison comparisonType)
	{
		return this.IndexOf(value, comparisonType) >= 0;
	}

	// Token: 0x0600911E RID: 37150 RVA: 0x0039E756 File Offset: 0x0039C956
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(string value)
	{
		return this.IndexOf(value, StringComparison.Ordinal) >= 0;
	}

	// Token: 0x0600911F RID: 37151 RVA: 0x0039E76B File Offset: 0x0039C96B
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Contains(string value, StringComparison comparisonType)
	{
		return this.IndexOf(value, comparisonType) >= 0;
	}

	// Token: 0x06009120 RID: 37152 RVA: 0x0039E780 File Offset: 0x0039C980
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOf(char value)
	{
		return this.m_span.IndexOf(value);
	}

	// Token: 0x06009121 RID: 37153 RVA: 0x0039E78E File Offset: 0x0039C98E
	public int IndexOf(char value, StringComparison comparisonType)
	{
		return this.m_span.IndexOf(MemoryMarshal.CreateReadOnlySpan<char>(ref value, 1), comparisonType);
	}

	// Token: 0x06009122 RID: 37154 RVA: 0x0039E7A4 File Offset: 0x0039C9A4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOf(StringSpan value)
	{
		return this.m_span.IndexOf(value.AsSpan());
	}

	// Token: 0x06009123 RID: 37155 RVA: 0x0039E7B8 File Offset: 0x0039C9B8
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int IndexOf(StringSpan value, StringComparison comparisonType)
	{
		return this.m_span.IndexOf(value.AsSpan(), comparisonType);
	}

	// Token: 0x06009124 RID: 37156 RVA: 0x0039E7CD File Offset: 0x0039C9CD
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int LastIndexOf(char value)
	{
		return this.m_span.LastIndexOf(value);
	}

	// Token: 0x06009125 RID: 37157 RVA: 0x0039E7DB File Offset: 0x0039C9DB
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int LastIndexOf(StringSpan value)
	{
		return this.m_span.LastIndexOf(value.AsSpan());
	}

	// Token: 0x06009126 RID: 37158 RVA: 0x0039E7EF File Offset: 0x0039C9EF
	public int IndexOfAny(StringSpan value)
	{
		return this.m_span.IndexOfAny(value.AsSpan());
	}

	// Token: 0x06009127 RID: 37159 RVA: 0x0039E803 File Offset: 0x0039CA03
	public int LastIndexOfAny(StringSpan value)
	{
		return this.m_span.LastIndexOfAny(value.AsSpan());
	}

	// Token: 0x06009128 RID: 37160 RVA: 0x0039E817 File Offset: 0x0039CA17
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan.WhitespaceSplitEnumerator GetSplitEnumerator(StringSplitOptions options = StringSplitOptions.None)
	{
		return new StringSpan.WhitespaceSplitEnumerator(this.m_span, options);
	}

	// Token: 0x06009129 RID: 37161 RVA: 0x0039E825 File Offset: 0x0039CA25
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan.CharSplitEnumerator GetSplitEnumerator(char separator, StringSplitOptions options = StringSplitOptions.None)
	{
		return new StringSpan.CharSplitEnumerator(this.m_span, separator, options);
	}

	// Token: 0x0600912A RID: 37162 RVA: 0x0039E834 File Offset: 0x0039CA34
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan.StringSplitEnumerator GetSplitEnumerator(ReadOnlySpan<char> separator, StringSplitOptions options = StringSplitOptions.None)
	{
		return new StringSpan.StringSplitEnumerator(this.m_span, separator, options);
	}

	// Token: 0x0600912B RID: 37163 RVA: 0x0039E843 File Offset: 0x0039CA43
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan.SeparatorSplitAnyEnumerator GetSplitAnyEnumerator(string[] separator, StringSplitOptions options = StringSplitOptions.None)
	{
		return new StringSpan.SeparatorSplitAnyEnumerator(this.m_span, options, separator);
	}

	// Token: 0x0600912C RID: 37164 RVA: 0x0039E854 File Offset: 0x0039CA54
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan Substring(int startIndex)
	{
		StringSpan stringSpan = this;
		return stringSpan.Slice(startIndex, stringSpan.Length - startIndex);
	}

	// Token: 0x0600912D RID: 37165 RVA: 0x0039E87C File Offset: 0x0039CA7C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StringSpan Substring(int startIndex, int length)
	{
		StringSpan stringSpan = this;
		return stringSpan.Slice(startIndex, startIndex + length - startIndex);
	}

	// Token: 0x0600912E RID: 37166 RVA: 0x0039E8A0 File Offset: 0x0039CAA0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe StringSpan Trim()
	{
		int num = 0;
		while (num < this.Length && char.IsWhiteSpace((char)(*this[num])))
		{
			num++;
		}
		int num2 = this.Length - 1;
		while (num2 >= num && char.IsWhiteSpace((char)(*this[num2])))
		{
			num2--;
		}
		if (num > num2)
		{
			return default(StringSpan);
		}
		return this.Slice(num, num2 - num + 1);
	}

	// Token: 0x0600912F RID: 37167 RVA: 0x0039E909 File Offset: 0x0039CB09
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator StringSpan(string str)
	{
		return new StringSpan(str);
	}

	// Token: 0x06009130 RID: 37168 RVA: 0x0039E916 File Offset: 0x0039CB16
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator StringSpan(ReadOnlySpan<char> span)
	{
		return new StringSpan(span);
	}

	// Token: 0x06009131 RID: 37169 RVA: 0x0039E6A1 File Offset: 0x0039C8A1
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator string(StringSpan span)
	{
		return new string(span.m_span);
	}

	// Token: 0x06009132 RID: 37170 RVA: 0x0039E699 File Offset: 0x0039C899
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator ReadOnlySpan<char>(StringSpan span)
	{
		return span.m_span;
	}

	// Token: 0x06009133 RID: 37171 RVA: 0x0039E91E File Offset: 0x0039CB1E
	public override bool Equals(object obj)
	{
		throw new NotSupportedException("StringSpan.Equals(object) is not supported. Use another method or the operator == instead.");
	}

	// Token: 0x06009134 RID: 37172 RVA: 0x0039E92A File Offset: 0x0039CB2A
	public override int GetHashCode()
	{
		return SpanUtils.GetHashCode<char>(this.m_span);
	}

	// Token: 0x06009135 RID: 37173 RVA: 0x0039E937 File Offset: 0x0039CB37
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(StringSpan left, StringSpan right)
	{
		return left.Equals(right, StringComparison.Ordinal);
	}

	// Token: 0x06009136 RID: 37174 RVA: 0x0039E942 File Offset: 0x0039CB42
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(StringSpan left, StringSpan right)
	{
		return !(left == right);
	}

	// Token: 0x04006F78 RID: 28536
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly ReadOnlySpan<char> m_span;

	// Token: 0x02001229 RID: 4649
	public ref struct CharSplitEnumerator
	{
		// Token: 0x06009137 RID: 37175 RVA: 0x0039E94E File Offset: 0x0039CB4E
		public CharSplitEnumerator(ReadOnlySpan<char> span, char separator, StringSplitOptions options = StringSplitOptions.None)
		{
			this.m_remainder = span;
			this.m_separator = separator;
			this.m_removeEmptyEntries = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
			this.m_current = default(StringSpan);
			this.m_done = false;
		}

		// Token: 0x06009138 RID: 37176 RVA: 0x0039E988 File Offset: 0x0039CB88
		public StringSpan.CharSplitEnumerator GetEnumerator()
		{
			return this;
		}

		// Token: 0x17000F00 RID: 3840
		// (get) Token: 0x06009139 RID: 37177 RVA: 0x0039E990 File Offset: 0x0039CB90
		public StringSpan Current
		{
			get
			{
				return this.m_current;
			}
		}

		// Token: 0x0600913A RID: 37178 RVA: 0x0039E998 File Offset: 0x0039CB98
		public bool MoveNext()
		{
			while (this.MoveToNextInternal())
			{
				if (!this.m_removeEmptyEntries || this.m_current.Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600913B RID: 37179 RVA: 0x0039E9BC File Offset: 0x0039CBBC
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe bool MoveToNextInternal()
		{
			if (this.m_done)
			{
				return false;
			}
			for (int i = 0; i < this.m_remainder.Length; i++)
			{
				if (*this.m_remainder[i] == (ushort)this.m_separator)
				{
					ReadOnlySpan<char> remainder = this.m_remainder;
					this.m_current = remainder.Slice(0, i);
					remainder = this.m_remainder;
					int num = i + 1;
					this.m_remainder = remainder.Slice(num, remainder.Length - num);
					return true;
				}
			}
			this.m_current = this.m_remainder;
			this.m_remainder = default(ReadOnlySpan<char>);
			this.m_done = true;
			return true;
		}

		// Token: 0x04006F79 RID: 28537
		[PublicizedFrom(EAccessModifier.Private)]
		public ReadOnlySpan<char> m_remainder;

		// Token: 0x04006F7A RID: 28538
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly char m_separator;

		// Token: 0x04006F7B RID: 28539
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_removeEmptyEntries;

		// Token: 0x04006F7C RID: 28540
		[PublicizedFrom(EAccessModifier.Private)]
		public StringSpan m_current;

		// Token: 0x04006F7D RID: 28541
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_done;
	}

	// Token: 0x0200122A RID: 4650
	public ref struct StringSplitEnumerator
	{
		// Token: 0x0600913C RID: 37180 RVA: 0x0039EA62 File Offset: 0x0039CC62
		public StringSplitEnumerator(ReadOnlySpan<char> span, ReadOnlySpan<char> separator, StringSplitOptions options = StringSplitOptions.None)
		{
			this.m_remainder = span;
			this.m_separator = separator;
			this.m_removeEmptyEntries = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
			this.m_current = default(StringSpan);
			this.m_done = false;
		}

		// Token: 0x0600913D RID: 37181 RVA: 0x0039EA9C File Offset: 0x0039CC9C
		public StringSpan.StringSplitEnumerator GetEnumerator()
		{
			return this;
		}

		// Token: 0x17000F01 RID: 3841
		// (get) Token: 0x0600913E RID: 37182 RVA: 0x0039EAA4 File Offset: 0x0039CCA4
		public StringSpan Current
		{
			get
			{
				return this.m_current;
			}
		}

		// Token: 0x0600913F RID: 37183 RVA: 0x0039EAAC File Offset: 0x0039CCAC
		public bool MoveNext()
		{
			while (this.MoveToNextInternal())
			{
				if (!this.m_removeEmptyEntries || this.m_current.Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06009140 RID: 37184 RVA: 0x0039EAD0 File Offset: 0x0039CCD0
		[PublicizedFrom(EAccessModifier.Private)]
		public bool MoveToNextInternal()
		{
			if (this.m_done)
			{
				return false;
			}
			if (this.m_separator.Length <= 0)
			{
				this.m_current = this.m_remainder;
				this.m_remainder = default(ReadOnlySpan<char>);
				this.m_done = true;
				return true;
			}
			int num = this.m_remainder.Length + 1 - this.m_separator.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.m_remainder.Slice(i, this.m_separator.Length).CompareTo(this.m_separator, StringComparison.Ordinal) == 0)
				{
					ReadOnlySpan<char> remainder = this.m_remainder;
					this.m_current = remainder.Slice(0, i);
					remainder = this.m_remainder;
					int num2 = i + this.m_separator.Length;
					this.m_remainder = remainder.Slice(num2, remainder.Length - num2);
					return true;
				}
			}
			this.m_current = this.m_remainder;
			this.m_remainder = default(ReadOnlySpan<char>);
			this.m_done = true;
			return true;
		}

		// Token: 0x04006F7E RID: 28542
		[PublicizedFrom(EAccessModifier.Private)]
		public ReadOnlySpan<char> m_remainder;

		// Token: 0x04006F7F RID: 28543
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ReadOnlySpan<char> m_separator;

		// Token: 0x04006F80 RID: 28544
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_removeEmptyEntries;

		// Token: 0x04006F81 RID: 28545
		[PublicizedFrom(EAccessModifier.Private)]
		public StringSpan m_current;

		// Token: 0x04006F82 RID: 28546
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_done;
	}

	// Token: 0x0200122B RID: 4651
	public ref struct WhitespaceSplitEnumerator
	{
		// Token: 0x06009141 RID: 37185 RVA: 0x0039EBD4 File Offset: 0x0039CDD4
		public WhitespaceSplitEnumerator(ReadOnlySpan<char> text, StringSplitOptions options = StringSplitOptions.None)
		{
			this.m_remainder = text;
			this.m_removeEmptyEntries = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
			this.m_current = default(StringSpan);
			this.m_done = false;
		}

		// Token: 0x06009142 RID: 37186 RVA: 0x0039EC0C File Offset: 0x0039CE0C
		public StringSpan.WhitespaceSplitEnumerator GetEnumerator()
		{
			return this;
		}

		// Token: 0x17000F02 RID: 3842
		// (get) Token: 0x06009143 RID: 37187 RVA: 0x0039EC14 File Offset: 0x0039CE14
		public StringSpan Current
		{
			get
			{
				return this.m_current;
			}
		}

		// Token: 0x06009144 RID: 37188 RVA: 0x0039EC1C File Offset: 0x0039CE1C
		public bool MoveNext()
		{
			while (this.MoveToNextInternal())
			{
				if (!this.m_removeEmptyEntries || this.m_current.Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06009145 RID: 37189 RVA: 0x0039EC40 File Offset: 0x0039CE40
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe bool MoveToNextInternal()
		{
			if (this.m_done)
			{
				return false;
			}
			for (int i = 0; i < this.m_remainder.Length; i++)
			{
				if (char.IsWhiteSpace((char)(*this.m_remainder[i])))
				{
					StringSpan remainder = this.m_remainder;
					this.m_current = remainder.Slice(0, i);
					remainder = this.m_remainder;
					int num = i + 1;
					this.m_remainder = remainder.Slice(num, remainder.Length - num);
					return true;
				}
			}
			this.m_current = this.m_remainder;
			this.m_remainder = default(StringSpan);
			this.m_done = true;
			return true;
		}

		// Token: 0x04006F83 RID: 28547
		[PublicizedFrom(EAccessModifier.Private)]
		public StringSpan m_remainder;

		// Token: 0x04006F84 RID: 28548
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_removeEmptyEntries;

		// Token: 0x04006F85 RID: 28549
		[PublicizedFrom(EAccessModifier.Private)]
		public StringSpan m_current;

		// Token: 0x04006F86 RID: 28550
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_done;
	}

	// Token: 0x0200122C RID: 4652
	public ref struct SeparatorSplitAnyEnumerator
	{
		// Token: 0x06009146 RID: 37190 RVA: 0x0039ECDC File Offset: 0x0039CEDC
		public SeparatorSplitAnyEnumerator(ReadOnlySpan<char> text, StringSplitOptions options = StringSplitOptions.None, params string[] separators)
		{
			this.m_remainder = text;
			this.m_removeEmptyEntries = options.HasFlag(StringSplitOptions.RemoveEmptyEntries);
			bool flag = false;
			if (separators != null)
			{
				for (int i = 0; i < separators.Length; i++)
				{
					if (!string.IsNullOrEmpty(separators[i]))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				throw new ArgumentException("StringSplitEnumerator requires at least one non-empty separator");
			}
			this.m_separators = separators;
			this.m_current = default(StringSpan);
			this.m_done = false;
		}

		// Token: 0x06009147 RID: 37191 RVA: 0x0039ED57 File Offset: 0x0039CF57
		public StringSpan.SeparatorSplitAnyEnumerator GetEnumerator()
		{
			return this;
		}

		// Token: 0x17000F03 RID: 3843
		// (get) Token: 0x06009148 RID: 37192 RVA: 0x0039ED5F File Offset: 0x0039CF5F
		public StringSpan Current
		{
			get
			{
				return this.m_current;
			}
		}

		// Token: 0x06009149 RID: 37193 RVA: 0x0039ED67 File Offset: 0x0039CF67
		public bool MoveNext()
		{
			while (this.MoveToNextInternal())
			{
				if (!this.m_removeEmptyEntries || this.m_current.Length != 0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600914A RID: 37194 RVA: 0x0039ED8C File Offset: 0x0039CF8C
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe bool MoveToNextInternal()
		{
			if (this.m_done)
			{
				return false;
			}
			for (int i = 0; i < this.m_remainder.Length; i++)
			{
				foreach (string text in this.m_separators)
				{
					if (!string.IsNullOrEmpty(text) && *this.m_remainder[i] == (ushort)text[0] && i + text.Length <= this.m_remainder.Length && this.m_remainder.Slice(i, text.Length).CompareTo(text.AsSpan(), StringComparison.Ordinal) == 0)
					{
						StringSpan remainder = this.m_remainder;
						this.m_current = remainder.Slice(0, i);
						remainder = this.m_remainder;
						int num = i + text.Length;
						this.m_remainder = remainder.Slice(num, remainder.Length - num);
						return true;
					}
				}
			}
			this.m_current = this.m_remainder;
			this.m_remainder = default(StringSpan);
			this.m_done = true;
			return true;
		}

		// Token: 0x04006F87 RID: 28551
		[PublicizedFrom(EAccessModifier.Private)]
		public StringSpan m_remainder;

		// Token: 0x04006F88 RID: 28552
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_removeEmptyEntries;

		// Token: 0x04006F89 RID: 28553
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string[] m_separators;

		// Token: 0x04006F8A RID: 28554
		[PublicizedFrom(EAccessModifier.Private)]
		public StringSpan m_current;

		// Token: 0x04006F8B RID: 28555
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_done;
	}
}

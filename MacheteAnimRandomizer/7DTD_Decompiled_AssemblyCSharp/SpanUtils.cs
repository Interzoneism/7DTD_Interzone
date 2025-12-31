using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Token: 0x02001216 RID: 4630
public static class SpanUtils
{
	// Token: 0x06009070 RID: 36976 RVA: 0x0039985D File Offset: 0x00397A5D
	public static int GetHashCode<[IsUnmanaged] T>(ReadOnlySpan<T> span) where T : struct, ValueType
	{
		return SpanUtils.GetHashCodeInternal(MemoryMarshal.Cast<T, byte>(span));
	}

	// Token: 0x06009071 RID: 36977 RVA: 0x0039986C File Offset: 0x00397A6C
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static int GetHashCodeInternal(ReadOnlySpan<byte> span)
	{
		int num = span.Length / 4 * 4;
		ReadOnlySpan<int> readOnlySpan = MemoryMarshal.Cast<byte, int>(span.Slice(0, num));
		int num2 = 1009;
		ReadOnlySpan<int> readOnlySpan2 = readOnlySpan;
		int i;
		for (i = 0; i < readOnlySpan2.Length; i++)
		{
			int num3 = *readOnlySpan2[i];
			num2 = num2 * 9176 + num3;
		}
		if (num == span.Length)
		{
			return num2;
		}
		int num4 = span.Length - num;
		switch (num4)
		{
		case 1:
			i = (int)(*span[num]);
			break;
		case 2:
			i = ((int)(*span[num]) | (int)(*span[num + 1]) << 8);
			break;
		case 3:
			i = ((int)(*span[num]) | (int)(*span[num + 1]) << 8 | (int)(*span[num + 2]) << 16);
			break;
		default:
			throw new InvalidOperationException(string.Format("Remainder should be 1, 2 or 3, but was: {0}", num4));
		}
		int num5 = i;
		return num2 * 9176 + num5;
	}

	// Token: 0x06009072 RID: 36978 RVA: 0x00399968 File Offset: 0x00397B68
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int ConcatAccLength(int length, ref int totalLength)
	{
		totalLength += length;
		return length;
	}

	// Token: 0x06009073 RID: 36979 RVA: 0x00399974 File Offset: 0x00397B74
	[PublicizedFrom(EAccessModifier.Private)]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ConcatCopyThenSlice(ref Span<char> dest, [TupleElementNames(new string[]
	{
		"ptr",
		"len"
	})] ValueTuple<IntPtr, int> src)
	{
		ReadOnlySpan<char> readOnlySpan = new ReadOnlySpan<char>(src.Item1.ToPointer(), src.Item2);
		readOnlySpan.CopyTo(dest);
		Span<char> span = dest;
		int item = src.Item2;
		dest = span.Slice(item, span.Length - item);
	}

	// Token: 0x06009074 RID: 36980 RVA: 0x003999CC File Offset: 0x00397BCC
	public unsafe static string Concat(StringSpan s0, StringSpan s1)
	{
		fixed (char* pinnableReference = s0.AsSpan().GetPinnableReference())
		{
			void* value = (void*)pinnableReference;
			fixed (char* pinnableReference2 = s1.AsSpan().GetPinnableReference())
			{
				void* value2 = (void*)pinnableReference2;
				int length = 0;
				ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> state = new ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>(new ValueTuple<IntPtr, int>((IntPtr)value, SpanUtils.ConcatAccLength(s0.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value2, SpanUtils.ConcatAccLength(s1.Length, ref length)));
				return string.Create<ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>>(length, state, delegate(Span<char> span, [TupleElementNames(new string[]
				{
					"d0",
					"d1",
					null,
					null,
					null,
					null
				})] ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> data)
				{
					SpanUtils.ConcatCopyThenSlice(ref span, data.Item1);
					SpanUtils.ConcatCopyThenSlice(ref span, data.Item2);
				});
			}
		}
	}

	// Token: 0x06009075 RID: 36981 RVA: 0x00399A68 File Offset: 0x00397C68
	public unsafe static string Concat(StringSpan s0, StringSpan s1, StringSpan s2)
	{
		fixed (char* pinnableReference = s0.AsSpan().GetPinnableReference())
		{
			void* value = (void*)pinnableReference;
			fixed (char* pinnableReference2 = s1.AsSpan().GetPinnableReference())
			{
				void* value2 = (void*)pinnableReference2;
				fixed (char* pinnableReference3 = s2.AsSpan().GetPinnableReference())
				{
					void* value3 = (void*)pinnableReference3;
					int length = 0;
					ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> state = new ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>(new ValueTuple<IntPtr, int>((IntPtr)value, SpanUtils.ConcatAccLength(s0.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value2, SpanUtils.ConcatAccLength(s1.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value3, SpanUtils.ConcatAccLength(s2.Length, ref length)));
					return string.Create<ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>>(length, state, delegate(Span<char> span, [TupleElementNames(new string[]
					{
						"d0",
						"d1",
						"d2",
						null,
						null,
						null,
						null,
						null,
						null
					})] ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> data)
					{
						SpanUtils.ConcatCopyThenSlice(ref span, data.Item1);
						SpanUtils.ConcatCopyThenSlice(ref span, data.Item2);
						SpanUtils.ConcatCopyThenSlice(ref span, data.Item3);
					});
				}
			}
		}
	}

	// Token: 0x06009076 RID: 36982 RVA: 0x00399B34 File Offset: 0x00397D34
	public unsafe static string Concat(StringSpan s0, StringSpan s1, StringSpan s2, StringSpan s3)
	{
		fixed (char* pinnableReference = s0.AsSpan().GetPinnableReference())
		{
			void* value = (void*)pinnableReference;
			fixed (char* pinnableReference2 = s1.AsSpan().GetPinnableReference())
			{
				void* value2 = (void*)pinnableReference2;
				fixed (char* pinnableReference3 = s2.AsSpan().GetPinnableReference())
				{
					void* value3 = (void*)pinnableReference3;
					fixed (char* pinnableReference4 = s3.AsSpan().GetPinnableReference())
					{
						void* value4 = (void*)pinnableReference4;
						int length = 0;
						ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> state = new ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>(new ValueTuple<IntPtr, int>((IntPtr)value, SpanUtils.ConcatAccLength(s0.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value2, SpanUtils.ConcatAccLength(s1.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value3, SpanUtils.ConcatAccLength(s2.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value4, SpanUtils.ConcatAccLength(s3.Length, ref length)));
						return string.Create<ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>>(length, state, delegate(Span<char> span, [TupleElementNames(new string[]
						{
							"d0",
							"d1",
							"d2",
							"d3",
							null,
							null,
							null,
							null,
							null,
							null,
							null,
							null
						})] ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> data)
						{
							SpanUtils.ConcatCopyThenSlice(ref span, data.Item1);
							SpanUtils.ConcatCopyThenSlice(ref span, data.Item2);
							SpanUtils.ConcatCopyThenSlice(ref span, data.Item3);
							SpanUtils.ConcatCopyThenSlice(ref span, data.Item4);
						});
					}
				}
			}
		}
	}

	// Token: 0x06009077 RID: 36983 RVA: 0x00399C30 File Offset: 0x00397E30
	public unsafe static string Concat(StringSpan s0, StringSpan s1, StringSpan s2, StringSpan s3, StringSpan s4)
	{
		fixed (char* pinnableReference = s0.AsSpan().GetPinnableReference())
		{
			void* value = (void*)pinnableReference;
			fixed (char* pinnableReference2 = s1.AsSpan().GetPinnableReference())
			{
				void* value2 = (void*)pinnableReference2;
				fixed (char* pinnableReference3 = s2.AsSpan().GetPinnableReference())
				{
					void* value3 = (void*)pinnableReference3;
					fixed (char* pinnableReference4 = s3.AsSpan().GetPinnableReference())
					{
						void* value4 = (void*)pinnableReference4;
						fixed (char* pinnableReference5 = s4.AsSpan().GetPinnableReference())
						{
							void* value5 = (void*)pinnableReference5;
							int length = 0;
							ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> state = new ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>(new ValueTuple<IntPtr, int>((IntPtr)value, SpanUtils.ConcatAccLength(s0.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value2, SpanUtils.ConcatAccLength(s1.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value3, SpanUtils.ConcatAccLength(s2.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value4, SpanUtils.ConcatAccLength(s3.Length, ref length)), new ValueTuple<IntPtr, int>((IntPtr)value5, SpanUtils.ConcatAccLength(s4.Length, ref length)));
							return string.Create<ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>>>(length, state, delegate(Span<char> span, [TupleElementNames(new string[]
							{
								"d0",
								"d1",
								"d2",
								"d3",
								"d4",
								null,
								null,
								null,
								null,
								null,
								null,
								null,
								null,
								null,
								null
							})] ValueTuple<ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>, ValueTuple<IntPtr, int>> data)
							{
								SpanUtils.ConcatCopyThenSlice(ref span, data.Item1);
								SpanUtils.ConcatCopyThenSlice(ref span, data.Item2);
								SpanUtils.ConcatCopyThenSlice(ref span, data.Item3);
								SpanUtils.ConcatCopyThenSlice(ref span, data.Item4);
								SpanUtils.ConcatCopyThenSlice(ref span, data.Item5);
							});
						}
					}
				}
			}
		}
	}
}

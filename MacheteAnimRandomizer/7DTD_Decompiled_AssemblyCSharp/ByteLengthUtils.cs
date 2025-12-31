using System;
using System.Text;

// Token: 0x02001141 RID: 4417
public static class ByteLengthUtils
{
	// Token: 0x06008AB1 RID: 35505 RVA: 0x00381928 File Offset: 0x0037FB28
	[PublicizedFrom(EAccessModifier.Private)]
	public static int GetBinaryWriter7BitEncodedIntLength(int value)
	{
		int num = 0;
		for (uint num2 = (uint)value; num2 >= 128U; num2 >>= 7)
		{
			num++;
		}
		return num + 1;
	}

	// Token: 0x06008AB2 RID: 35506 RVA: 0x00381950 File Offset: 0x0037FB50
	public static int GetBinaryWriterLength(this string text, Encoding encoding)
	{
		int byteCount = encoding.GetByteCount(text);
		return ByteLengthUtils.GetBinaryWriter7BitEncodedIntLength(byteCount) + byteCount;
	}
}

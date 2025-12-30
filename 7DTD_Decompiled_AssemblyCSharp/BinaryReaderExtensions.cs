using System;
using System.IO;

// Token: 0x02001135 RID: 4405
public static class BinaryReaderExtensions
{
	// Token: 0x06008A6D RID: 35437 RVA: 0x003801C4 File Offset: 0x0037E3C4
	public static bool TryReadAllBytes(this BinaryReader reader, Span<byte> dest)
	{
		int num;
		return reader.TryReadAllBytes(dest, out num);
	}

	// Token: 0x06008A6E RID: 35438 RVA: 0x003801DC File Offset: 0x0037E3DC
	public static bool TryReadAllBytes(this BinaryReader reader, Span<byte> dest, out int totalBytesRead)
	{
		int num2;
		for (totalBytesRead = 0; totalBytesRead < dest.Length; totalBytesRead += num2)
		{
			Span<byte> span = dest;
			int num = totalBytesRead;
			num2 = reader.Read(span.Slice(num, span.Length - num));
			if (num2 <= 0)
			{
				return false;
			}
		}
		return true;
	}
}

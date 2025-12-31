using System;
using System.IO;

// Token: 0x02000A16 RID: 2582
public abstract class RegionFile
{
	// Token: 0x1700081C RID: 2076
	// (get) Token: 0x06004F19 RID: 20249 RVA: 0x001F5896 File Offset: 0x001F3A96
	// (set) Token: 0x06004F1A RID: 20250 RVA: 0x001F589E File Offset: 0x001F3A9E
	public long Length { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x06004F1B RID: 20251 RVA: 0x001F58A7 File Offset: 0x001F3AA7
	public static string ConstructFullFilePath(string dir, int rX, int rZ, string ext)
	{
		return string.Format("{0}/r.{1}.{2}.{3}", new object[]
		{
			dir,
			rX,
			rZ,
			ext
		});
	}

	// Token: 0x06004F1C RID: 20252 RVA: 0x001F58D3 File Offset: 0x001F3AD3
	[PublicizedFrom(EAccessModifier.Protected)]
	public RegionFile(string fullFilePath, int rX, int rZ)
	{
		this.regionX = rX;
		this.regionZ = rZ;
		this.fullFilePath = fullFilePath;
	}

	// Token: 0x06004F1D RID: 20253 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Close()
	{
	}

	// Token: 0x06004F1E RID: 20254 RVA: 0x001F5900 File Offset: 0x001F3B00
	public void GetPositionAndPath(out int regionX, out int regionZ, out string fullFilePath)
	{
		regionX = this.regionX;
		regionZ = this.regionZ;
		fullFilePath = this.fullFilePath;
	}

	// Token: 0x06004F1F RID: 20255
	public abstract void SaveHeaderData();

	// Token: 0x06004F20 RID: 20256
	public abstract void GetTimestampInfo(int cX, int cZ, out uint timeStamp);

	// Token: 0x06004F21 RID: 20257
	public abstract void SetTimestampInfo(int cX, int cZ, uint timeStamp);

	// Token: 0x06004F22 RID: 20258
	public abstract bool HasChunk(int _cX, int _cZ);

	// Token: 0x06004F23 RID: 20259
	public abstract int GetChunkByteCount(int _cX, int _cZ);

	// Token: 0x06004F24 RID: 20260
	public abstract void ReadData(int cX, int cZ, Stream _targetStream);

	// Token: 0x06004F25 RID: 20261
	public abstract void WriteData(int _cX, int _cZ, int _dataLength, byte _compression, byte[] _data, bool _saveHeaderToFile);

	// Token: 0x06004F26 RID: 20262
	public abstract void RemoveChunk(int cX, int cZ);

	// Token: 0x06004F27 RID: 20263
	public abstract void OptimizeLayout();

	// Token: 0x06004F28 RID: 20264
	public abstract int ChunkCount();

	// Token: 0x06004F29 RID: 20265 RVA: 0x001F591C File Offset: 0x001F3B1C
	[PublicizedFrom(EAccessModifier.Protected)]
	public static short ToShort(short byte1, short byte2)
	{
		int value = ((int)byte2 << 8) + (int)byte1;
		short result;
		try
		{
			result = Convert.ToInt16(value);
		}
		catch
		{
			result = 0;
		}
		return result;
	}

	// Token: 0x06004F2A RID: 20266 RVA: 0x001F5950 File Offset: 0x001F3B50
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void FromShort(short number, out byte byte1, out byte byte2)
	{
		byte2 = (byte)(number >> 8);
		byte1 = (byte)(number & 255);
	}

	// Token: 0x04003C9D RID: 15517
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly byte[] tempReadBuffer = new byte[4096];

	// Token: 0x04003C9E RID: 15518
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly string fullFilePath;

	// Token: 0x04003C9F RID: 15519
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly int regionX;

	// Token: 0x04003CA0 RID: 15520
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly int regionZ;
}

using System;
using System.IO;

// Token: 0x0200037C RID: 892
public class WaveReader
{
	// Token: 0x06001A6C RID: 6764 RVA: 0x000A4260 File Offset: 0x000A2460
	public WaveReader(string _fileName)
	{
		this.filename = _fileName;
		this.buffer = MemoryPools.poolByte.Alloc(8192);
		using (BinaryReader binaryReader = new BinaryReader(SdFile.Open(this.filename, FileMode.Open, FileAccess.Read, FileShare.Read)))
		{
			string text = new string(binaryReader.ReadChars(88));
			this.position = (this.dataStartPos = text.IndexOf("data") + 8);
		}
	}

	// Token: 0x17000308 RID: 776
	// (set) Token: 0x06001A6D RID: 6765 RVA: 0x000A42EC File Offset: 0x000A24EC
	public int Position
	{
		set
		{
			this.position = this.dataStartPos + value;
		}
	}

	// Token: 0x06001A6E RID: 6766 RVA: 0x000A42FC File Offset: 0x000A24FC
	public void Read(float[] data, int count)
	{
		using (BinaryReader binaryReader = new BinaryReader(SdFile.Open(this.filename, FileMode.Open, FileAccess.Read, FileShare.Read)))
		{
			binaryReader.BaseStream.Position = (long)this.position;
			binaryReader.Read(this.buffer, 0, 8192);
			for (int i = 0; i < data.Length; i++)
			{
				short num = BitConverter.ToInt16(this.buffer, 2 * i);
				data[i] = (float)num / 32767f;
			}
		}
	}

	// Token: 0x06001A6F RID: 6767 RVA: 0x000A4388 File Offset: 0x000A2588
	public void Cleanup()
	{
		MemoryPools.poolByte.Free(this.buffer);
		this.buffer = null;
	}

	// Token: 0x04001128 RID: 4392
	[PublicizedFrom(EAccessModifier.Private)]
	public string filename;

	// Token: 0x04001129 RID: 4393
	[PublicizedFrom(EAccessModifier.Private)]
	public const int bufferSize = 8192;

	// Token: 0x0400112A RID: 4394
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] buffer;

	// Token: 0x0400112B RID: 4395
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int dataStartPos;

	// Token: 0x0400112C RID: 4396
	[PublicizedFrom(EAccessModifier.Private)]
	public int position;
}

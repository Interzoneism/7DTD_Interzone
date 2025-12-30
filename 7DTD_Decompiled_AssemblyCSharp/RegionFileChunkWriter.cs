using System;
using System.IO;
using Noemax.GZip;

// Token: 0x02000A24 RID: 2596
public class RegionFileChunkWriter
{
	// Token: 0x06004F75 RID: 20341 RVA: 0x001F69D3 File Offset: 0x001F4BD3
	public RegionFileChunkWriter(RegionFileAccessAbstract regionFileAccess)
	{
		this.regionFileAccess = regionFileAccess;
	}

	// Token: 0x06004F76 RID: 20342 RVA: 0x001F69F4 File Offset: 0x001F4BF4
	public void WriteStreamCompressed(string dir, int chunkX, int chunkZ, string ext, MemoryStream memoryStream)
	{
		Stream outputStream = this.regionFileAccess.GetOutputStream(dir, chunkX, chunkZ, ext);
		long v = StreamUtils.ReadInt64(memoryStream);
		StreamUtils.Write(outputStream, v);
		if (this.zipSaveStream == null || this.innerSaveStream != outputStream)
		{
			if (this.zipSaveStream != null)
			{
				Log.Warning("RFM.Save: Creating new DeflateStream, underlying Stream changed");
			}
			this.zipSaveStream = new DeflateOutputStream(outputStream, 3, true);
			this.innerSaveStream = outputStream;
		}
		StreamUtils.StreamCopy(memoryStream, this.zipSaveStream, this.saveBuffer, true);
		this.zipSaveStream.Restart();
		outputStream.Close();
	}

	// Token: 0x04003CBA RID: 15546
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileAccessAbstract regionFileAccess;

	// Token: 0x04003CBB RID: 15547
	[PublicizedFrom(EAccessModifier.Private)]
	public Stream innerSaveStream;

	// Token: 0x04003CBC RID: 15548
	[PublicizedFrom(EAccessModifier.Private)]
	public DeflateOutputStream zipSaveStream;

	// Token: 0x04003CBD RID: 15549
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] saveBuffer = new byte[4096];
}

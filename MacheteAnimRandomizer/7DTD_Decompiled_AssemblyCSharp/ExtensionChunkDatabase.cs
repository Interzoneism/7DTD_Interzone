using System;
using System.IO;

// Token: 0x02000AC6 RID: 2758
public class ExtensionChunkDatabase : DatabaseWithFixedDS<long, byte[]>
{
	// Token: 0x060054E8 RID: 21736 RVA: 0x0022B3CC File Offset: 0x002295CC
	public ExtensionChunkDatabase(int _magicBytes, int AraSizeX, int AraSizeY, int ChunkSize) : base(_magicBytes, 4, AraSizeX * AraSizeY / ChunkSize / ChunkSize, ChunkSize * ChunkSize, -1L, -1)
	{
		this.SizeOfDataSet = ChunkSize * ChunkSize;
	}

	// Token: 0x060054E9 RID: 21737 RVA: 0x0022B3F2 File Offset: 0x002295F2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override long readKey(BinaryReader _br)
	{
		return _br.ReadInt64();
	}

	// Token: 0x060054EA RID: 21738 RVA: 0x0022B3FA File Offset: 0x002295FA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void writeKey(BinaryWriter _bw, long _key)
	{
		_bw.Write(_key);
	}

	// Token: 0x060054EB RID: 21739 RVA: 0x0022B404 File Offset: 0x00229604
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void copyFromRead(byte[] _dataRead, byte[] _data)
	{
		for (int i = 0; i < _data.Length; i++)
		{
			_data[i] = _dataRead[i];
		}
	}

	// Token: 0x060054EC RID: 21740 RVA: 0x0022B428 File Offset: 0x00229628
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void copyToWrite(byte[] _data, byte[] _dataWrite)
	{
		for (int i = 0; i < _data.Length; i++)
		{
			_dataWrite[i] = _data[i];
		}
	}

	// Token: 0x060054ED RID: 21741 RVA: 0x0022B449 File Offset: 0x00229649
	[PublicizedFrom(EAccessModifier.Protected)]
	public override byte[] allocateDataStorage()
	{
		return new byte[this.SizeOfDataSet];
	}

	// Token: 0x040041C6 RID: 16838
	[PublicizedFrom(EAccessModifier.Private)]
	public int SizeOfDataSet;
}

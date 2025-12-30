using System;
using System.IO;

// Token: 0x020009B5 RID: 2485
public class ChunkCustomData
{
	// Token: 0x06004BFB RID: 19451 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public ChunkCustomData()
	{
	}

	// Token: 0x06004BFC RID: 19452 RVA: 0x001E0459 File Offset: 0x001DE659
	public ChunkCustomData(string _key, ulong _expiresInWorldTime, bool _isSavedToNetwork)
	{
		this.key = _key;
		this.expiresInWorldTime = _expiresInWorldTime;
		this.isSavedToNetwork = _isSavedToNetwork;
	}

	// Token: 0x06004BFD RID: 19453 RVA: 0x001E0478 File Offset: 0x001DE678
	public void Read(BinaryReader _br)
	{
		this.key = _br.ReadString();
		this.expiresInWorldTime = _br.ReadUInt64();
		this.isSavedToNetwork = _br.ReadBoolean();
		int num = (int)_br.ReadUInt16();
		if (num > 0)
		{
			this.data = _br.ReadBytes(num);
			return;
		}
		this.data = null;
	}

	// Token: 0x06004BFE RID: 19454 RVA: 0x001E04CC File Offset: 0x001DE6CC
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.key);
		_bw.Write(this.expiresInWorldTime);
		_bw.Write(this.isSavedToNetwork);
		if (this.TriggerWriteDataDelegate != null)
		{
			this.TriggerWriteDataDelegate();
		}
		_bw.Write((ushort)((this.data != null) ? this.data.Length : 0));
		if (this.data != null && this.data.Length != 0)
		{
			_bw.Write(this.data);
		}
	}

	// Token: 0x06004BFF RID: 19455 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void OnRemove(Chunk chunk)
	{
	}

	// Token: 0x040039F2 RID: 14834
	public string key;

	// Token: 0x040039F3 RID: 14835
	public ulong expiresInWorldTime;

	// Token: 0x040039F4 RID: 14836
	public bool isSavedToNetwork;

	// Token: 0x040039F5 RID: 14837
	public byte[] data;

	// Token: 0x040039F6 RID: 14838
	public ChunkCustomData.TriggerWriteData TriggerWriteDataDelegate;

	// Token: 0x020009B6 RID: 2486
	// (Invoke) Token: 0x06004C01 RID: 19457
	public delegate void TriggerWriteData();
}

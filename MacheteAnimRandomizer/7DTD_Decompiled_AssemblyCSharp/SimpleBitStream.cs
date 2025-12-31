using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x02001210 RID: 4624
[PublicizedFrom(EAccessModifier.Internal)]
public class SimpleBitStream
{
	// Token: 0x06009040 RID: 36928 RVA: 0x00398956 File Offset: 0x00396B56
	public SimpleBitStream(int _initialCapacity = 1000)
	{
		this.data = new List<byte>(_initialCapacity);
		this.Reset();
	}

	// Token: 0x06009041 RID: 36929 RVA: 0x00398970 File Offset: 0x00396B70
	public void Reset()
	{
		this.data.Clear();
		this.curBitIdx = 0;
		this.curByteIdx = 0;
		this.curByteData = 0;
	}

	// Token: 0x06009042 RID: 36930 RVA: 0x00398994 File Offset: 0x00396B94
	public void Add(bool _b)
	{
		if (_b)
		{
			this.curByteData = (byte)((int)this.curByteData | 1 << this.curBitIdx);
		}
		this.curBitIdx++;
		if (this.curBitIdx > 7)
		{
			this.data.Add(this.curByteData);
			this.curBitIdx = 0;
			this.curByteIdx++;
			this.curByteData = 0;
		}
	}

	// Token: 0x06009043 RID: 36931 RVA: 0x00398A04 File Offset: 0x00396C04
	public bool GetNext()
	{
		if (this.curBitIdx > 7)
		{
			List<byte> list = this.data;
			int index = this.curByteIdx + 1;
			this.curByteIdx = index;
			this.curByteData = list[index];
			this.curBitIdx = 0;
		}
		bool result = (this.curByteData & 1) > 0;
		this.curByteData = (byte)(this.curByteData >> 1);
		this.curBitIdx++;
		return result;
	}

	// Token: 0x06009044 RID: 36932 RVA: 0x00398A6C File Offset: 0x00396C6C
	public int GetNextOffset()
	{
		bool flag = false;
		for (;;)
		{
			if (this.curBitIdx > 7)
			{
				this.curByteIdx++;
				if (this.curByteIdx >= this.data.Count)
				{
					break;
				}
				this.curByteData = this.data[this.curByteIdx];
				this.curBitIdx = 0;
			}
			if (this.curByteData == 0)
			{
				this.curBitIdx = 8;
			}
			else
			{
				flag = ((this.curByteData & 1) == 1);
				this.curByteData = (byte)(this.curByteData >> 1);
				this.curBitIdx++;
			}
			if (flag)
			{
				goto Block_4;
			}
		}
		return -1;
		Block_4:
		return this.curByteIdx * 8 + this.curBitIdx - 1;
	}

	// Token: 0x06009045 RID: 36933 RVA: 0x00398B18 File Offset: 0x00396D18
	public void Write(BinaryWriter _bw)
	{
		if (this.curBitIdx > 0)
		{
			this.data.Add(this.curByteData);
		}
		_bw.Write(this.data.Count);
		for (int i = 0; i < this.data.Count; i++)
		{
			_bw.Write(this.data[i]);
		}
	}

	// Token: 0x06009046 RID: 36934 RVA: 0x00398B78 File Offset: 0x00396D78
	public void Read(BinaryReader _br)
	{
		int num = _br.ReadInt32();
		byte[] collection = _br.ReadBytes(num);
		this.data.Clear();
		this.data.AddRange(collection);
		if (num > 0)
		{
			this.curByteData = this.data[0];
		}
	}

	// Token: 0x04006F2E RID: 28462
	[PublicizedFrom(EAccessModifier.Private)]
	public List<byte> data;

	// Token: 0x04006F2F RID: 28463
	[PublicizedFrom(EAccessModifier.Private)]
	public int curBitIdx;

	// Token: 0x04006F30 RID: 28464
	[PublicizedFrom(EAccessModifier.Private)]
	public int curByteIdx;

	// Token: 0x04006F31 RID: 28465
	[PublicizedFrom(EAccessModifier.Private)]
	public byte curByteData;
}

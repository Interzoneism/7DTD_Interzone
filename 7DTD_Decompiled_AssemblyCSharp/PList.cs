using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x020011EB RID: 4587
public class PList<T> : List<T>
{
	// Token: 0x06008F20 RID: 36640 RVA: 0x003939F8 File Offset: 0x00391BF8
	public PList() : this(1U)
	{
	}

	// Token: 0x06008F21 RID: 36641 RVA: 0x00393A01 File Offset: 0x00391C01
	public PList(uint _saveVersion)
	{
		this.saveVersion = _saveVersion;
	}

	// Token: 0x06008F22 RID: 36642 RVA: 0x00393A1C File Offset: 0x00391C1C
	public void Write(BinaryWriter _bw)
	{
		_bw.Write((ushort)base.Count);
		_bw.Write((byte)this.saveVersion);
		foreach (T arg in this)
		{
			this.writeElement(_bw, arg);
		}
	}

	// Token: 0x06008F23 RID: 36643 RVA: 0x00393A8C File Offset: 0x00391C8C
	public void Read(BinaryReader _br)
	{
		base.Clear();
		int num = (int)_br.ReadUInt16();
		uint arg = (uint)_br.ReadByte();
		for (int i = 0; i < num; i++)
		{
			T item = this.readElement(_br, arg);
			base.Add(item);
		}
	}

	// Token: 0x06008F24 RID: 36644 RVA: 0x00393ACE File Offset: 0x00391CCE
	public void MarkToRemove(T _v)
	{
		this.toRemove.Add(_v);
	}

	// Token: 0x06008F25 RID: 36645 RVA: 0x00393ADC File Offset: 0x00391CDC
	public void RemoveAllMarked()
	{
		foreach (T item in this.toRemove)
		{
			base.Remove(item);
		}
		this.toRemove.Clear();
	}

	// Token: 0x04006E9F RID: 28319
	public Action<BinaryWriter, T> writeElement;

	// Token: 0x04006EA0 RID: 28320
	public Func<BinaryReader, uint, T> readElement;

	// Token: 0x04006EA1 RID: 28321
	[PublicizedFrom(EAccessModifier.Protected)]
	public uint saveVersion;

	// Token: 0x04006EA2 RID: 28322
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<T> toRemove = new List<T>();
}

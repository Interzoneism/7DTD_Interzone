using System;
using System.IO;

// Token: 0x020011E0 RID: 4576
public class PackedBoolArray
{
	// Token: 0x17000ECE RID: 3790
	// (get) Token: 0x06008EDA RID: 36570 RVA: 0x0039251C File Offset: 0x0039071C
	// (set) Token: 0x06008EDB RID: 36571 RVA: 0x00392524 File Offset: 0x00390724
	public int Length
	{
		get
		{
			return this.length;
		}
		set
		{
			if (value == this.length)
			{
				return;
			}
			if (value == 0)
			{
				this.data = null;
				this.length = value;
				return;
			}
			if (this.data == null)
			{
				this.data = new byte[PackedBoolArray.calcArraySize(value)];
				this.length = value;
				return;
			}
			int num = PackedBoolArray.calcArraySize(this.length);
			int num2 = PackedBoolArray.calcArraySize(value);
			if (num2 > num)
			{
				byte[] destinationArray = new byte[num2];
				Array.Copy(this.data, destinationArray, num);
				this.data = destinationArray;
				this.length = value;
				return;
			}
			if (num2 < num)
			{
				byte[] destinationArray2 = new byte[num2];
				Array.Copy(this.data, destinationArray2, num2);
				this.data = destinationArray2;
			}
			this.length = num2 * 8;
			for (int i = value; i < num2 * 8; i++)
			{
				this[i] = false;
			}
			this.length = value;
		}
	}

	// Token: 0x17000ECF RID: 3791
	// (get) Token: 0x06008EDC RID: 36572 RVA: 0x003925F2 File Offset: 0x003907F2
	public int ByteSize
	{
		get
		{
			return PackedBoolArray.calcArraySize(this.Length);
		}
	}

	// Token: 0x17000ED0 RID: 3792
	public bool this[int _i]
	{
		get
		{
			this.validateIndex(_i);
			return ((int)this.data[_i / 8] & 1 << _i % 8) != 0;
		}
		set
		{
			this.validateIndex(_i);
			if (value)
			{
				byte[] array = this.data;
				int num = _i / 8;
				array[num] |= (byte)(1 << _i % 8);
				return;
			}
			byte[] array2 = this.data;
			int num2 = _i / 8;
			array2[num2] &= (byte)(~(byte)(1 << _i % 8));
		}
	}

	// Token: 0x06008EDF RID: 36575 RVA: 0x00392671 File Offset: 0x00390871
	public PackedBoolArray(int _length = 0)
	{
		this.Length = _length;
	}

	// Token: 0x06008EE0 RID: 36576 RVA: 0x00392680 File Offset: 0x00390880
	[PublicizedFrom(EAccessModifier.Private)]
	public void validateIndex(int _i)
	{
		if (_i < 0)
		{
			throw new IndexOutOfRangeException(string.Format("Index ({0}) needs to be non-negative", _i));
		}
		if (_i >= this.length)
		{
			throw new IndexOutOfRangeException(string.Format("Index ({0}) needs to be lower than length ({1})", _i, this.Length));
		}
	}

	// Token: 0x06008EE1 RID: 36577 RVA: 0x003926D4 File Offset: 0x003908D4
	public PackedBoolArray Clone()
	{
		PackedBoolArray packedBoolArray = new PackedBoolArray(this.Length);
		for (int i = 0; i < PackedBoolArray.calcArraySize(this.Length); i++)
		{
			packedBoolArray.data[i] = this.data[i];
		}
		return packedBoolArray;
	}

	// Token: 0x06008EE2 RID: 36578 RVA: 0x00392714 File Offset: 0x00390914
	public void Write(Stream _targetStream)
	{
		StreamUtils.Write7BitEncodedInt(_targetStream, this.Length);
		_targetStream.Write(this.data, 0, this.ByteSize);
	}

	// Token: 0x06008EE3 RID: 36579 RVA: 0x00392735 File Offset: 0x00390935
	public void Write(BinaryWriter _targetWriter)
	{
		StreamUtils.Write7BitEncodedInt(_targetWriter.BaseStream, this.Length);
		_targetWriter.Write(this.data);
	}

	// Token: 0x06008EE4 RID: 36580 RVA: 0x00392754 File Offset: 0x00390954
	public void Write(PooledBinaryWriter _targetWriter)
	{
		_targetWriter.Write7BitEncodedInt(this.Length);
		_targetWriter.Write(this.data);
	}

	// Token: 0x06008EE5 RID: 36581 RVA: 0x0039276E File Offset: 0x0039096E
	public void Read(Stream _sourceStream)
	{
		this.Length = StreamUtils.Read7BitEncodedInt(_sourceStream);
		_sourceStream.Read(this.data, 0, this.ByteSize);
	}

	// Token: 0x06008EE6 RID: 36582 RVA: 0x00392790 File Offset: 0x00390990
	public void Read(BinaryReader _sourceReader)
	{
		this.Length = StreamUtils.Read7BitEncodedInt(_sourceReader.BaseStream);
		_sourceReader.Read(this.data, 0, this.ByteSize);
	}

	// Token: 0x06008EE7 RID: 36583 RVA: 0x003927B7 File Offset: 0x003909B7
	public void Read(PooledBinaryReader _sourceReader)
	{
		this.Length = _sourceReader.Read7BitEncodedInt();
		_sourceReader.Read(this.data, 0, this.ByteSize);
	}

	// Token: 0x06008EE8 RID: 36584 RVA: 0x003927D9 File Offset: 0x003909D9
	[PublicizedFrom(EAccessModifier.Private)]
	public static int calcArraySize(int _length)
	{
		return (_length + 7) / 8;
	}

	// Token: 0x06008EE9 RID: 36585 RVA: 0x003927E0 File Offset: 0x003909E0
	public void Clear()
	{
		for (int i = 0; i < this.ByteSize; i++)
		{
			this.data[i] = 0;
		}
	}

	// Token: 0x04006E77 RID: 28279
	[PublicizedFrom(EAccessModifier.Private)]
	public int length;

	// Token: 0x04006E78 RID: 28280
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;
}

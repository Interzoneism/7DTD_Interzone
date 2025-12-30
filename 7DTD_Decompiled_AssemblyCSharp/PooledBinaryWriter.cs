using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020011F0 RID: 4592
public class PooledBinaryWriter : BinaryWriter, IBinaryReaderOrWriter, IMemoryPoolableObject, IDisposable
{
	// Token: 0x17000ED9 RID: 3801
	// (get) Token: 0x06008F6D RID: 36717 RVA: 0x00395353 File Offset: 0x00393553
	// (set) Token: 0x06008F6E RID: 36718 RVA: 0x0039535B File Offset: 0x0039355B
	public Encoding Encoding
	{
		get
		{
			return this.encoding;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.encoding = value;
			this.encoder = null;
			this.maxBytesPerChar = this.encoding.GetMaxByteCount(1);
		}
	}

	// Token: 0x06008F6F RID: 36719 RVA: 0x0039538C File Offset: 0x0039358C
	public PooledBinaryWriter()
	{
		this.Encoding = new UTF8Encoding(false, false);
		Interlocked.Increment(ref PooledBinaryWriter.INSTANCES_CREATED);
		Interlocked.Increment(ref PooledBinaryWriter.INSTANCES_LIVE);
		if (PooledBinaryWriter.INSTANCES_LIVE > PooledBinaryWriter.INSTANCES_MAX)
		{
			Interlocked.Exchange(ref PooledBinaryWriter.INSTANCES_MAX, PooledBinaryWriter.INSTANCES_LIVE);
		}
	}

	// Token: 0x06008F70 RID: 36720 RVA: 0x00395400 File Offset: 0x00393600
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~PooledBinaryWriter()
	{
		Interlocked.Decrement(ref PooledBinaryWriter.INSTANCES_LIVE);
	}

	// Token: 0x06008F71 RID: 36721 RVA: 0x00395434 File Offset: 0x00393634
	public void SetBaseStream(Stream _stream)
	{
		if (_stream != null && !_stream.CanWrite)
		{
			throw new ArgumentException("Stream does not support writing or already closed.");
		}
		this.OutStream = _stream;
		this.encoder = null;
	}

	// Token: 0x17000EDA RID: 3802
	// (get) Token: 0x06008F72 RID: 36722 RVA: 0x0039545A File Offset: 0x0039365A
	public override Stream BaseStream
	{
		get
		{
			return this.OutStream;
		}
	}

	// Token: 0x06008F73 RID: 36723 RVA: 0x00395462 File Offset: 0x00393662
	public override void Flush()
	{
		this.OutStream.Flush();
	}

	// Token: 0x06008F74 RID: 36724 RVA: 0x0039546F File Offset: 0x0039366F
	public override long Seek(int _offset, SeekOrigin _origin)
	{
		return this.OutStream.Seek((long)_offset, _origin);
	}

	// Token: 0x06008F75 RID: 36725 RVA: 0x0039547F File Offset: 0x0039367F
	public override void Write(bool _value)
	{
		this.buffer[0] = ((!_value) ? 0 : 1);
		this.OutStream.Write(this.buffer, 0, 1);
	}

	// Token: 0x06008F76 RID: 36726 RVA: 0x003954A4 File Offset: 0x003936A4
	public override void Write(byte _value)
	{
		this.OutStream.WriteByte(_value);
	}

	// Token: 0x06008F77 RID: 36727 RVA: 0x003954B2 File Offset: 0x003936B2
	public override void Write(byte[] _buffer)
	{
		if (_buffer == null)
		{
			throw new ArgumentNullException("_buffer");
		}
		this.OutStream.Write(_buffer, 0, _buffer.Length);
	}

	// Token: 0x06008F78 RID: 36728 RVA: 0x003954D2 File Offset: 0x003936D2
	public override void Write(byte[] _buffer, int _index, int _count)
	{
		if (_buffer == null)
		{
			throw new ArgumentNullException("_buffer");
		}
		this.OutStream.Write(_buffer, _index, _count);
	}

	// Token: 0x06008F79 RID: 36729 RVA: 0x003954F0 File Offset: 0x003936F0
	public override void Write(ReadOnlySpan<byte> _buffer)
	{
		this.OutStream.Write(_buffer);
	}

	// Token: 0x06008F7A RID: 36730 RVA: 0x00395500 File Offset: 0x00393700
	public override void Write(char _ch)
	{
		this.charBuffer[0] = _ch;
		int bytes = this.encoding.GetBytes(this.charBuffer, 0, 1, this.buffer, 0);
		this.OutStream.Write(this.buffer, 0, bytes);
	}

	// Token: 0x06008F7B RID: 36731 RVA: 0x00395544 File Offset: 0x00393744
	public override void Write(char[] _chars)
	{
		this.Write(_chars, 0, _chars.Length);
	}

	// Token: 0x06008F7C RID: 36732 RVA: 0x00395551 File Offset: 0x00393751
	public override void Write(char[] _chars, int _index, int _count)
	{
		if (_chars == null)
		{
			throw new ArgumentNullException("_chars");
		}
		this.Write(_chars.AsSpan(_index, _count));
	}

	// Token: 0x06008F7D RID: 36733 RVA: 0x00395574 File Offset: 0x00393774
	public override void Write(ReadOnlySpan<char> _chars)
	{
		int num;
		for (int i = 0; i < _chars.Length; i += num)
		{
			num = Math.Min(128 / this.maxBytesPerChar, _chars.Length - i);
			int bytes = this.encoding.GetBytes(_chars.Slice(i, num), this.buffer);
			this.OutStream.Write(this.buffer, 0, bytes);
		}
	}

	// Token: 0x06008F7E RID: 36734 RVA: 0x003955E4 File Offset: 0x003937E4
	public unsafe override void Write(decimal _value)
	{
		byte* ptr = (byte*)(&_value);
		if (BitConverter.IsLittleEndian)
		{
			for (int i = 0; i < 16; i++)
			{
				if (i < 4)
				{
					this.buffer[i + 12] = ptr[i];
				}
				else if (i < 8)
				{
					this.buffer[i + 4] = ptr[i];
				}
				else if (i < 12)
				{
					this.buffer[i - 8] = ptr[i];
				}
				else
				{
					this.buffer[i - 8] = ptr[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < 16; j++)
			{
				if (j < 4)
				{
					this.buffer[15 - j] = ptr[j];
				}
				else if (j < 8)
				{
					this.buffer[15 - j] = ptr[j];
				}
				else if (j < 12)
				{
					this.buffer[11 - j] = ptr[j];
				}
				else
				{
					this.buffer[19 - j] = ptr[j];
				}
			}
		}
		this.OutStream.Write(this.buffer, 0, 16);
	}

	// Token: 0x06008F7F RID: 36735 RVA: 0x003956C7 File Offset: 0x003938C7
	public override void Write(double _value)
	{
		BitConverterLE.GetBytes(_value, this.buffer);
		this.OutStream.Write(this.buffer, 0, 8);
	}

	// Token: 0x06008F80 RID: 36736 RVA: 0x003956E8 File Offset: 0x003938E8
	public override void Write(short _value)
	{
		this.buffer[0] = (byte)_value;
		this.buffer[1] = (byte)(_value >> 8);
		this.OutStream.Write(this.buffer, 0, 2);
	}

	// Token: 0x06008F81 RID: 36737 RVA: 0x00395714 File Offset: 0x00393914
	public override void Write(int _value)
	{
		this.buffer[0] = (byte)_value;
		this.buffer[1] = (byte)(_value >> 8);
		this.buffer[2] = (byte)(_value >> 16);
		this.buffer[3] = (byte)(_value >> 24);
		this.OutStream.Write(this.buffer, 0, 4);
	}

	// Token: 0x06008F82 RID: 36738 RVA: 0x00395764 File Offset: 0x00393964
	public override void Write(long _value)
	{
		int i = 0;
		int num = 0;
		while (i < 8)
		{
			this.buffer[i] = (byte)(_value >> num);
			i++;
			num += 8;
		}
		this.OutStream.Write(this.buffer, 0, 8);
	}

	// Token: 0x06008F83 RID: 36739 RVA: 0x003957A5 File Offset: 0x003939A5
	public override void Write(sbyte _value)
	{
		this.buffer[0] = (byte)_value;
		this.OutStream.Write(this.buffer, 0, 1);
	}

	// Token: 0x06008F84 RID: 36740 RVA: 0x003957C4 File Offset: 0x003939C4
	public override void Write(float _value)
	{
		BitConverterLE.GetBytes(_value, this.buffer);
		this.OutStream.Write(this.buffer, 0, 4);
	}

	// Token: 0x06008F85 RID: 36741 RVA: 0x003957E8 File Offset: 0x003939E8
	public unsafe override void Write(string _value)
	{
		if (_value == null)
		{
			throw new ArgumentNullException("_value");
		}
		if (this.encoder == null)
		{
			this.encoder = this.encoding.GetEncoder();
		}
		int byteCount;
		fixed (string text = _value)
		{
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			byteCount = this.encoder.GetByteCount(ptr, _value.Length, true);
		}
		this.Write7BitEncodedInt(byteCount);
		int num = 128 / this.maxBytesPerChar;
		int num2 = 0;
		int num3;
		for (int i = _value.Length; i > 0; i -= num3)
		{
			num3 = ((i <= num) ? i : num);
			int bytes2;
			fixed (string text = _value)
			{
				char* ptr2 = text;
				if (ptr2 != null)
				{
					ptr2 += RuntimeHelpers.OffsetToStringData / 2;
				}
				byte[] array;
				byte* bytes;
				if ((array = this.buffer) == null || array.Length == 0)
				{
					bytes = null;
				}
				else
				{
					bytes = &array[0];
				}
				bytes2 = this.encoder.GetBytes((char*)((void*)((UIntPtr)((void*)ptr2) + num2 * 2)), num3, bytes, 128, num3 == i);
				array = null;
			}
			this.OutStream.Write(this.buffer, 0, bytes2);
			num2 += num3;
		}
	}

	// Token: 0x06008F86 RID: 36742 RVA: 0x003956E8 File Offset: 0x003938E8
	public override void Write(ushort _value)
	{
		this.buffer[0] = (byte)_value;
		this.buffer[1] = (byte)(_value >> 8);
		this.OutStream.Write(this.buffer, 0, 2);
	}

	// Token: 0x06008F87 RID: 36743 RVA: 0x0039590C File Offset: 0x00393B0C
	public override void Write(uint _value)
	{
		this.buffer[0] = (byte)_value;
		this.buffer[1] = (byte)(_value >> 8);
		this.buffer[2] = (byte)(_value >> 16);
		this.buffer[3] = (byte)(_value >> 24);
		this.OutStream.Write(this.buffer, 0, 4);
	}

	// Token: 0x06008F88 RID: 36744 RVA: 0x0039595C File Offset: 0x00393B5C
	public override void Write(ulong _value)
	{
		int i = 0;
		int num = 0;
		while (i < 8)
		{
			this.buffer[i] = (byte)(_value >> num);
			i++;
			num += 8;
		}
		this.OutStream.Write(this.buffer, 0, 8);
	}

	// Token: 0x06008F89 RID: 36745 RVA: 0x003959A0 File Offset: 0x00393BA0
	public void Write7BitEncodedSignedInt(int _value)
	{
		long num = (long)_value;
		bool flag = num < 0L;
		num = Math.Abs(num);
		long num2 = num >> 6 & 33554431L;
		byte b = (byte)(num & 63L);
		if (num2 != 0L)
		{
			b |= 128;
		}
		if (flag)
		{
			b |= 64;
		}
		this.Write(b);
		for (num = num2; num != 0L; num = num2)
		{
			num2 = (num >> 7 & 16777215L);
			b = (byte)(num & 127L);
			if (num2 != 0L)
			{
				b |= 128;
			}
			this.Write(b);
		}
	}

	// Token: 0x06008F8A RID: 36746 RVA: 0x00395A18 File Offset: 0x00393C18
	public new void Write7BitEncodedInt(int _value)
	{
		do
		{
			int num = _value >> 7 & 16777215;
			byte b = (byte)(_value & 127);
			if (num != 0)
			{
				b |= 128;
			}
			this.Write(b);
			_value = num;
		}
		while (_value != 0);
	}

	// Token: 0x06008F8B RID: 36747 RVA: 0x00395A4C File Offset: 0x00393C4C
	[MustUseReturnValue]
	public PooledBinaryWriter.StreamWriteSizeMarker ReserveSizeMarker(PooledBinaryWriter.EMarkerSize _markerSize)
	{
		long position = this.OutStream.Position;
		Array.Clear(this.buffer, 0, (int)_markerSize);
		this.OutStream.Write(this.buffer, 0, (int)_markerSize);
		return new PooledBinaryWriter.StreamWriteSizeMarker(position, _markerSize);
	}

	// Token: 0x06008F8C RID: 36748 RVA: 0x00395A80 File Offset: 0x00393C80
	public void FinalizeSizeMarker(ref PooledBinaryWriter.StreamWriteSizeMarker _sizeMarker)
	{
		long position = this.OutStream.Position;
		long num = position - _sizeMarker.Position;
		if (num < 0L)
		{
			throw new Exception(string.Format("FinalizeMarker position ({0}) before Reserved position ({1})", position, _sizeMarker.Position));
		}
		uint num2;
		switch (_sizeMarker.MarkerSize)
		{
		case PooledBinaryWriter.EMarkerSize.UInt8:
			num2 = 255U;
			goto IL_7D;
		case PooledBinaryWriter.EMarkerSize.UInt16:
			num2 = 65535U;
			goto IL_7D;
		case PooledBinaryWriter.EMarkerSize.UInt32:
			num2 = uint.MaxValue;
			goto IL_7D;
		}
		throw new ArgumentOutOfRangeException("MarkerSize");
		IL_7D:
		long num3 = (long)((ulong)num2);
		if (num > num3)
		{
			throw new Exception(string.Format("Marked size ({0}) exceeding marker type ({1} maximum ({2})", num, _sizeMarker.MarkerSize.ToStringCached<PooledBinaryWriter.EMarkerSize>(), num3));
		}
		this.OutStream.Position = _sizeMarker.Position;
		switch (_sizeMarker.MarkerSize)
		{
		case PooledBinaryWriter.EMarkerSize.UInt8:
			this.Write((byte)num);
			break;
		case PooledBinaryWriter.EMarkerSize.UInt16:
			this.Write((ushort)num);
			break;
		case PooledBinaryWriter.EMarkerSize.UInt32:
			this.Write((uint)num);
			break;
		}
		this.OutStream.Position = position;
	}

	// Token: 0x06008F8D RID: 36749 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Close()
	{
	}

	// Token: 0x06008F8E RID: 36750 RVA: 0x00395B94 File Offset: 0x00393D94
	public void Reset()
	{
		this.SetBaseStream(null);
	}

	// Token: 0x06008F8F RID: 36751 RVA: 0x00395B9D File Offset: 0x00393D9D
	public void Cleanup()
	{
		this.Reset();
	}

	// Token: 0x06008F90 RID: 36752 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Dispose(bool _disposing)
	{
	}

	// Token: 0x06008F91 RID: 36753 RVA: 0x00395BA5 File Offset: 0x00393DA5
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose()
	{
		MemoryPools.poolBinaryWriter.FreeSync(this);
	}

	// Token: 0x06008F92 RID: 36754 RVA: 0x00395BB2 File Offset: 0x00393DB2
	public bool ReadWrite(bool _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F93 RID: 36755 RVA: 0x00395BBC File Offset: 0x00393DBC
	public byte ReadWrite(byte _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F94 RID: 36756 RVA: 0x00395BC6 File Offset: 0x00393DC6
	public sbyte ReadWrite(sbyte _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F95 RID: 36757 RVA: 0x00395BD0 File Offset: 0x00393DD0
	public char ReadWrite(char _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F96 RID: 36758 RVA: 0x00395BDA File Offset: 0x00393DDA
	public short ReadWrite(short _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F97 RID: 36759 RVA: 0x00395BE4 File Offset: 0x00393DE4
	public ushort ReadWrite(ushort _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F98 RID: 36760 RVA: 0x00395BEE File Offset: 0x00393DEE
	public int ReadWrite(int _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F99 RID: 36761 RVA: 0x00395BF8 File Offset: 0x00393DF8
	public uint ReadWrite(uint _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F9A RID: 36762 RVA: 0x00395C02 File Offset: 0x00393E02
	public long ReadWrite(long _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F9B RID: 36763 RVA: 0x00395C0C File Offset: 0x00393E0C
	public ulong ReadWrite(ulong _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F9C RID: 36764 RVA: 0x00395C16 File Offset: 0x00393E16
	public float ReadWrite(float _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F9D RID: 36765 RVA: 0x00395C20 File Offset: 0x00393E20
	public double ReadWrite(double _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F9E RID: 36766 RVA: 0x00395C2A File Offset: 0x00393E2A
	public decimal ReadWrite(decimal _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008F9F RID: 36767 RVA: 0x00395C34 File Offset: 0x00393E34
	public string ReadWrite(string _value)
	{
		this.Write(_value);
		return _value;
	}

	// Token: 0x06008FA0 RID: 36768 RVA: 0x00395C3E File Offset: 0x00393E3E
	public void ReadWrite(byte[] _buffer, int _index, int _count)
	{
		this.Write(_buffer, _index, _count);
	}

	// Token: 0x06008FA1 RID: 36769 RVA: 0x00395C49 File Offset: 0x00393E49
	public Vector3 ReadWrite(Vector3 _value)
	{
		this.Write(_value.x);
		this.Write(_value.y);
		this.Write(_value.z);
		return _value;
	}

	// Token: 0x04006EBB RID: 28347
	public static int INSTANCES_LIVE;

	// Token: 0x04006EBC RID: 28348
	public static int INSTANCES_MAX;

	// Token: 0x04006EBD RID: 28349
	public static int INSTANCES_CREATED;

	// Token: 0x04006EBE RID: 28350
	[PublicizedFrom(EAccessModifier.Private)]
	public const int BYTE_BUFFER_SIZE = 128;

	// Token: 0x04006EBF RID: 28351
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CHAR_BUFFER_SIZE = 128;

	// Token: 0x04006EC0 RID: 28352
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly byte[] buffer = new byte[128];

	// Token: 0x04006EC1 RID: 28353
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly char[] charBuffer = new char[128];

	// Token: 0x04006EC2 RID: 28354
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxBytesPerChar;

	// Token: 0x04006EC3 RID: 28355
	[PublicizedFrom(EAccessModifier.Private)]
	public Encoder encoder;

	// Token: 0x04006EC4 RID: 28356
	[PublicizedFrom(EAccessModifier.Private)]
	public Encoding encoding;

	// Token: 0x020011F1 RID: 4593
	public enum EMarkerSize
	{
		// Token: 0x04006EC6 RID: 28358
		UInt8 = 1,
		// Token: 0x04006EC7 RID: 28359
		UInt16,
		// Token: 0x04006EC8 RID: 28360
		UInt32 = 4
	}

	// Token: 0x020011F2 RID: 4594
	public readonly struct StreamWriteSizeMarker
	{
		// Token: 0x06008FA2 RID: 36770 RVA: 0x00395C70 File Offset: 0x00393E70
		public StreamWriteSizeMarker(long _position, PooledBinaryWriter.EMarkerSize _markerSize)
		{
			this.Position = _position;
			this.MarkerSize = _markerSize;
		}

		// Token: 0x04006EC9 RID: 28361
		public readonly long Position;

		// Token: 0x04006ECA RID: 28362
		public readonly PooledBinaryWriter.EMarkerSize MarkerSize;
	}
}

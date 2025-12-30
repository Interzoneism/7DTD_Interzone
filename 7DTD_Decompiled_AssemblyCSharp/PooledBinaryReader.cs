using System;
using System.IO;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020011EE RID: 4590
public class PooledBinaryReader : BinaryReader, IBinaryReaderOrWriter, IMemoryPoolableObject, IDisposable
{
	// Token: 0x17000ED7 RID: 3799
	// (get) Token: 0x06008F33 RID: 36659 RVA: 0x003948FE File Offset: 0x00392AFE
	public override Stream BaseStream
	{
		get
		{
			return this.baseStream;
		}
	}

	// Token: 0x17000ED8 RID: 3800
	// (get) Token: 0x06008F34 RID: 36660 RVA: 0x00394906 File Offset: 0x00392B06
	// (set) Token: 0x06008F35 RID: 36661 RVA: 0x0039490E File Offset: 0x00392B0E
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
			this.decoder = null;
		}
	}

	// Token: 0x06008F36 RID: 36662 RVA: 0x0039492C File Offset: 0x00392B2C
	public PooledBinaryReader() : base(Stream.Null)
	{
		this.Encoding = new UTF8Encoding(false, false);
		Interlocked.Increment(ref PooledBinaryReader.INSTANCES_CREATED);
		Interlocked.Increment(ref PooledBinaryReader.INSTANCES_LIVE);
		if (PooledBinaryReader.INSTANCES_LIVE > PooledBinaryReader.INSTANCES_MAX)
		{
			Interlocked.Exchange(ref PooledBinaryReader.INSTANCES_MAX, PooledBinaryReader.INSTANCES_LIVE);
		}
	}

	// Token: 0x06008F37 RID: 36663 RVA: 0x003949B4 File Offset: 0x00392BB4
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~PooledBinaryReader()
	{
		Interlocked.Decrement(ref PooledBinaryReader.INSTANCES_LIVE);
	}

	// Token: 0x06008F38 RID: 36664 RVA: 0x003949E8 File Offset: 0x00392BE8
	public void SetBaseStream(Stream _stream)
	{
		if (_stream != null && !_stream.CanRead)
		{
			throw new ArgumentException("The stream doesn't support reading.");
		}
		this.baseStream = _stream;
		this.decoder = null;
	}

	// Token: 0x06008F39 RID: 36665 RVA: 0x00394A10 File Offset: 0x00392C10
	public override int PeekChar()
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		if (!this.baseStream.CanSeek)
		{
			return -1;
		}
		int num;
		bool flag = this.ReadCharBytes(this.charBuffer, 0, 1, out num) != 0;
		this.baseStream.Position -= (long)num;
		if (!flag)
		{
			return -1;
		}
		return (int)this.charBuffer[0];
	}

	// Token: 0x06008F3A RID: 36666 RVA: 0x00394A6F File Offset: 0x00392C6F
	public override int Read()
	{
		if (this.Read(this.charBuffer, 0, 1) == 0)
		{
			return -1;
		}
		return (int)this.charBuffer[0];
	}

	// Token: 0x06008F3B RID: 36667 RVA: 0x00394A8B File Offset: 0x00392C8B
	public override bool ReadBoolean()
	{
		return this.ReadByte() > 0;
	}

	// Token: 0x06008F3C RID: 36668 RVA: 0x00394A96 File Offset: 0x00392C96
	public override byte ReadByte()
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		int num = this.baseStream.ReadByte();
		if (num == -1)
		{
			throw new EndOfStreamException();
		}
		return (byte)num;
	}

	// Token: 0x06008F3D RID: 36669 RVA: 0x00394AC1 File Offset: 0x00392CC1
	public override sbyte ReadSByte()
	{
		return (sbyte)this.ReadByte();
	}

	// Token: 0x06008F3E RID: 36670 RVA: 0x00394ACA File Offset: 0x00392CCA
	public override char ReadChar()
	{
		int num = this.Read();
		if (num == -1)
		{
			throw new EndOfStreamException();
		}
		return (char)num;
	}

	// Token: 0x06008F3F RID: 36671 RVA: 0x00394ADD File Offset: 0x00392CDD
	public override short ReadInt16()
	{
		this.FillBuffer(2);
		return (short)((int)this.buffer[0] | (int)this.buffer[1] << 8);
	}

	// Token: 0x06008F40 RID: 36672 RVA: 0x00394AFA File Offset: 0x00392CFA
	public override ushort ReadUInt16()
	{
		this.FillBuffer(2);
		return (ushort)((int)this.buffer[0] | (int)this.buffer[1] << 8);
	}

	// Token: 0x06008F41 RID: 36673 RVA: 0x00394B17 File Offset: 0x00392D17
	public override int ReadInt32()
	{
		this.FillBuffer(4);
		return (int)this.buffer[0] | (int)this.buffer[1] << 8 | (int)this.buffer[2] << 16 | (int)this.buffer[3] << 24;
	}

	// Token: 0x06008F42 RID: 36674 RVA: 0x00394B17 File Offset: 0x00392D17
	public override uint ReadUInt32()
	{
		this.FillBuffer(4);
		return (uint)((int)this.buffer[0] | (int)this.buffer[1] << 8 | (int)this.buffer[2] << 16 | (int)this.buffer[3] << 24);
	}

	// Token: 0x06008F43 RID: 36675 RVA: 0x00394B4C File Offset: 0x00392D4C
	public override long ReadInt64()
	{
		this.FillBuffer(8);
		uint num = (uint)((int)this.buffer[0] | (int)this.buffer[1] << 8 | (int)this.buffer[2] << 16 | (int)this.buffer[3] << 24);
		return (long)((ulong)((int)this.buffer[4] | (int)this.buffer[5] << 8 | (int)this.buffer[6] << 16 | (int)this.buffer[7] << 24) << 32 | (ulong)num);
	}

	// Token: 0x06008F44 RID: 36676 RVA: 0x00394BC0 File Offset: 0x00392DC0
	public override ulong ReadUInt64()
	{
		this.FillBuffer(8);
		uint num = (uint)((int)this.buffer[0] | (int)this.buffer[1] << 8 | (int)this.buffer[2] << 16 | (int)this.buffer[3] << 24);
		return (ulong)((int)this.buffer[4] | (int)this.buffer[5] << 8 | (int)this.buffer[6] << 16 | (int)this.buffer[7] << 24) << 32 | (ulong)num;
	}

	// Token: 0x06008F45 RID: 36677 RVA: 0x00394C32 File Offset: 0x00392E32
	public override float ReadSingle()
	{
		this.FillBuffer(4);
		return BitConverterLE.ToSingle(this.buffer, 0);
	}

	// Token: 0x06008F46 RID: 36678 RVA: 0x00394C47 File Offset: 0x00392E47
	public override double ReadDouble()
	{
		this.FillBuffer(8);
		return BitConverterLE.ToDouble(this.buffer, 0);
	}

	// Token: 0x06008F47 RID: 36679 RVA: 0x00394C5C File Offset: 0x00392E5C
	public unsafe override decimal ReadDecimal()
	{
		this.FillBuffer(16);
		decimal result;
		byte* ptr = (byte*)(&result);
		if (BitConverter.IsLittleEndian)
		{
			for (int i = 0; i < 16; i++)
			{
				if (i < 4)
				{
					ptr[i + 8] = this.buffer[i];
				}
				else if (i < 8)
				{
					ptr[i + 8] = this.buffer[i];
				}
				else if (i < 12)
				{
					ptr[i - 4] = this.buffer[i];
				}
				else if (i < 16)
				{
					ptr[i - 12] = this.buffer[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < 16; j++)
			{
				if (j < 4)
				{
					ptr[11 - j] = this.buffer[j];
				}
				else if (j < 8)
				{
					ptr[19 - j] = this.buffer[j];
				}
				else if (j < 12)
				{
					ptr[15 - j] = this.buffer[j];
				}
				else if (j < 16)
				{
					ptr[15 - j] = this.buffer[j];
				}
			}
		}
		return result;
	}

	// Token: 0x06008F48 RID: 36680 RVA: 0x00394D40 File Offset: 0x00392F40
	public override string ReadString()
	{
		int num = this.Read7BitEncodedInt();
		if (num < 0)
		{
			throw new IOException("Invalid binary file (string len < 0)");
		}
		if (num == 0)
		{
			return string.Empty;
		}
		this.stringBuilder.Length = 0;
		this.stringBuilder.EnsureCapacity(num);
		do
		{
			int num2 = (num <= 128) ? num : 128;
			this.FillBuffer(num2);
			if (this.decoder == null)
			{
				this.decoder = this.encoding.GetDecoder();
			}
			int chars = this.decoder.GetChars(this.buffer, 0, num2, this.charBuffer, 0);
			this.stringBuilder.Append(this.charBuffer, 0, chars);
			num -= num2;
		}
		while (num > 0);
		return this.stringBuilder.ToString();
	}

	// Token: 0x06008F49 RID: 36681 RVA: 0x00394DF8 File Offset: 0x00392FF8
	public override int Read(char[] _buffer, int _index, int _count)
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		if (_buffer == null)
		{
			throw new ArgumentNullException("_buffer", "_buffer is null");
		}
		if (_index < 0)
		{
			throw new ArgumentOutOfRangeException("_index", "_index is less than 0");
		}
		if (_count < 0)
		{
			throw new ArgumentOutOfRangeException("_count", "_count is less than 0");
		}
		if (_buffer.Length - _index < _count)
		{
			throw new ArgumentException("buffer is too small");
		}
		int num;
		return this.ReadCharBytes(_buffer, _index, _count, out num);
	}

	// Token: 0x06008F4A RID: 36682 RVA: 0x00394E74 File Offset: 0x00393074
	[Obsolete("char[] ReadChars (int) allocates memory. Try using int Read (char[], int, int) instead.")]
	public override char[] ReadChars(int _count)
	{
		if (_count < 0)
		{
			throw new ArgumentOutOfRangeException("_count", "_count is less than 0");
		}
		if (_count == 0)
		{
			return new char[0];
		}
		char[] array = new char[_count];
		int num = this.Read(array, 0, _count);
		if (num == 0)
		{
			throw new EndOfStreamException();
		}
		if (num != array.Length)
		{
			char[] array2 = new char[num];
			Array.Copy(array, 0, array2, 0, num);
			return array2;
		}
		return array;
	}

	// Token: 0x06008F4B RID: 36683 RVA: 0x00394ED4 File Offset: 0x003930D4
	public override int Read(Span<char> _buffer)
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		int num;
		return this.ReadCharBytes(_buffer, out num);
	}

	// Token: 0x06008F4C RID: 36684 RVA: 0x00394EFD File Offset: 0x003930FD
	public override int Read(Span<byte> _buffer)
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		return this.baseStream.Read(_buffer);
	}

	// Token: 0x06008F4D RID: 36685 RVA: 0x00394F20 File Offset: 0x00393120
	public override int Read(byte[] _buffer, int _index, int _count)
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		if (_buffer == null)
		{
			throw new ArgumentNullException("_buffer", "_buffer is null");
		}
		if (_index < 0)
		{
			throw new ArgumentOutOfRangeException("_index", "_index is less than 0");
		}
		if (_count < 0)
		{
			throw new ArgumentOutOfRangeException("_count", "_count is less than 0");
		}
		if (_buffer.Length - _index < _count)
		{
			throw new ArgumentException("buffer is too small");
		}
		return this.baseStream.Read(_buffer, _index, _count);
	}

	// Token: 0x06008F4E RID: 36686 RVA: 0x00394F9C File Offset: 0x0039319C
	[Obsolete("byte[] ReadBytes (int) allocates memory. Try using int Read (byte[], int, int) instead.")]
	public override byte[] ReadBytes(int _count)
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		if (_count < 0)
		{
			throw new ArgumentOutOfRangeException("_count", "_count is less than 0");
		}
		byte[] array = new byte[_count];
		int i;
		int num;
		for (i = 0; i < _count; i += num)
		{
			num = this.baseStream.Read(array, i, _count - i);
			if (num == 0)
			{
				break;
			}
		}
		if (i != _count)
		{
			byte[] array2 = new byte[i];
			Buffer.BlockCopy(array, 0, array2, 0, i);
			return array2;
		}
		return array;
	}

	// Token: 0x06008F4F RID: 36687 RVA: 0x00395010 File Offset: 0x00393210
	public int Read7BitEncodedSignedInt()
	{
		int num = 0;
		int i = 0;
		byte b = this.ReadByte();
		num |= (int)(b & 63);
		i += 6;
		bool flag = (b & 64) > 0;
		if ((b & 128) != 0)
		{
			while (i < 32)
			{
				b = this.ReadByte();
				num |= (int)(b & 127) << i;
				i += 7;
				if ((b & 128) == 0)
				{
					if (!flag)
					{
						return num;
					}
					return -num;
				}
			}
			throw new FormatException("Illegal encoding for 7 bit encoded int");
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	// Token: 0x06008F50 RID: 36688 RVA: 0x00395084 File Offset: 0x00393284
	public new int Read7BitEncodedInt()
	{
		int num = 0;
		int i = 0;
		while (i < 35)
		{
			byte b = this.ReadByte();
			num |= (int)(b & 127) << i;
			i += 7;
			if ((b & 128) == 0)
			{
				return num;
			}
		}
		throw new FormatException("Illegal encoding for 7 bit encoded int");
	}

	// Token: 0x06008F51 RID: 36689 RVA: 0x003950C8 File Offset: 0x003932C8
	[MustUseReturnValue]
	public PooledBinaryReader.StreamReadSizeMarker ReadSizeMarker(PooledBinaryWriter.EMarkerSize _markerSize)
	{
		long position = this.baseStream.Position;
		uint num;
		switch (_markerSize)
		{
		case PooledBinaryWriter.EMarkerSize.UInt8:
			num = (uint)this.ReadByte();
			goto IL_4C;
		case PooledBinaryWriter.EMarkerSize.UInt16:
			num = (uint)this.ReadUInt16();
			goto IL_4C;
		case PooledBinaryWriter.EMarkerSize.UInt32:
			num = this.ReadUInt32();
			goto IL_4C;
		}
		throw new ArgumentOutOfRangeException("_markerSize");
		IL_4C:
		uint expectedSize = num;
		return new PooledBinaryReader.StreamReadSizeMarker(position, expectedSize);
	}

	// Token: 0x06008F52 RID: 36690 RVA: 0x0039512C File Offset: 0x0039332C
	public bool ValidateSizeMarker(ref PooledBinaryReader.StreamReadSizeMarker _sizeMarker, out uint _bytesReceived, bool _fixPosition = true)
	{
		long num = this.baseStream.Position - _sizeMarker.Position;
		_bytesReceived = (uint)num;
		if (num == (long)((ulong)_sizeMarker.ExpectedSize))
		{
			return true;
		}
		if (_fixPosition)
		{
			this.baseStream.Position = _sizeMarker.Position + (long)((ulong)_sizeMarker.ExpectedSize);
		}
		return false;
	}

	// Token: 0x06008F53 RID: 36691 RVA: 0x00395179 File Offset: 0x00393379
	[PublicizedFrom(EAccessModifier.Private)]
	public int ReadCharBytes(char[] _targetBuffer, int _targetIndex, int _count, out int _bytesRead)
	{
		return this.ReadCharBytes(_targetBuffer.AsSpan(_targetIndex, _count), out _bytesRead);
	}

	// Token: 0x06008F54 RID: 36692 RVA: 0x0039518C File Offset: 0x0039338C
	[PublicizedFrom(EAccessModifier.Private)]
	public int ReadCharBytes(Span<char> _targetBuffer, out int _bytesRead)
	{
		int i = 0;
		_bytesRead = 0;
		while (i < _targetBuffer.Length)
		{
			int length = 0;
			int chars;
			do
			{
				int num = this.baseStream.ReadByte();
				if (num == -1)
				{
					return i;
				}
				this.buffer[length++] = (byte)num;
				_bytesRead++;
				chars = this.encoding.GetChars(this.buffer.AsSpan(0, length), _targetBuffer.Slice(i));
			}
			while (chars <= 0);
			i++;
		}
		return i;
	}

	// Token: 0x06008F55 RID: 36693 RVA: 0x00395204 File Offset: 0x00393404
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void FillBuffer(int _numBytes)
	{
		if (this.baseStream == null)
		{
			throw new IOException("Stream is invalid");
		}
		int num;
		for (int i = 0; i < _numBytes; i += num)
		{
			num = this.baseStream.Read(this.buffer, i, _numBytes - i);
			if (num == 0)
			{
				throw new EndOfStreamException();
			}
		}
	}

	// Token: 0x06008F56 RID: 36694 RVA: 0x0039524F File Offset: 0x0039344F
	public void Flush()
	{
		if (this.baseStream != null)
		{
			this.baseStream.Flush();
		}
	}

	// Token: 0x06008F57 RID: 36695 RVA: 0x00002914 File Offset: 0x00000B14
	public override void Close()
	{
	}

	// Token: 0x06008F58 RID: 36696 RVA: 0x00395264 File Offset: 0x00393464
	public void Reset()
	{
		this.stringBuilder.Length = 0;
		this.baseStream = null;
	}

	// Token: 0x06008F59 RID: 36697 RVA: 0x00395279 File Offset: 0x00393479
	public void Cleanup()
	{
		this.Reset();
	}

	// Token: 0x06008F5A RID: 36698 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Dispose(bool _disposing)
	{
	}

	// Token: 0x06008F5B RID: 36699 RVA: 0x00395281 File Offset: 0x00393481
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose()
	{
		MemoryPools.poolBinaryReader.FreeSync(this);
	}

	// Token: 0x06008F5C RID: 36700 RVA: 0x0039528E File Offset: 0x0039348E
	public bool ReadWrite(bool _value)
	{
		return this.ReadBoolean();
	}

	// Token: 0x06008F5D RID: 36701 RVA: 0x00395296 File Offset: 0x00393496
	public byte ReadWrite(byte _value)
	{
		return this.ReadByte();
	}

	// Token: 0x06008F5E RID: 36702 RVA: 0x0039529E File Offset: 0x0039349E
	public sbyte ReadWrite(sbyte _value)
	{
		return this.ReadSByte();
	}

	// Token: 0x06008F5F RID: 36703 RVA: 0x003952A6 File Offset: 0x003934A6
	public char ReadWrite(char _value)
	{
		return this.ReadChar();
	}

	// Token: 0x06008F60 RID: 36704 RVA: 0x003952AE File Offset: 0x003934AE
	public short ReadWrite(short _value)
	{
		return this.ReadInt16();
	}

	// Token: 0x06008F61 RID: 36705 RVA: 0x003952B6 File Offset: 0x003934B6
	public ushort ReadWrite(ushort _value)
	{
		return this.ReadUInt16();
	}

	// Token: 0x06008F62 RID: 36706 RVA: 0x003952BE File Offset: 0x003934BE
	public int ReadWrite(int _value)
	{
		return this.ReadInt32();
	}

	// Token: 0x06008F63 RID: 36707 RVA: 0x003952C6 File Offset: 0x003934C6
	public uint ReadWrite(uint _value)
	{
		return this.ReadUInt32();
	}

	// Token: 0x06008F64 RID: 36708 RVA: 0x003952CE File Offset: 0x003934CE
	public long ReadWrite(long _value)
	{
		return this.ReadInt64();
	}

	// Token: 0x06008F65 RID: 36709 RVA: 0x003952D6 File Offset: 0x003934D6
	public ulong ReadWrite(ulong _value)
	{
		return this.ReadUInt64();
	}

	// Token: 0x06008F66 RID: 36710 RVA: 0x003952DE File Offset: 0x003934DE
	public float ReadWrite(float _value)
	{
		return this.ReadSingle();
	}

	// Token: 0x06008F67 RID: 36711 RVA: 0x003952E6 File Offset: 0x003934E6
	public double ReadWrite(double _value)
	{
		return this.ReadDouble();
	}

	// Token: 0x06008F68 RID: 36712 RVA: 0x003952EE File Offset: 0x003934EE
	public decimal ReadWrite(decimal _value)
	{
		return this.ReadDecimal();
	}

	// Token: 0x06008F69 RID: 36713 RVA: 0x003952F6 File Offset: 0x003934F6
	public string ReadWrite(string _value)
	{
		return this.ReadString();
	}

	// Token: 0x06008F6A RID: 36714 RVA: 0x003952FE File Offset: 0x003934FE
	public void ReadWrite(byte[] _buffer, int _index, int _count)
	{
		this.Read(_buffer, _index, _count);
	}

	// Token: 0x06008F6B RID: 36715 RVA: 0x0039530C File Offset: 0x0039350C
	public Vector3 ReadWrite(Vector3 _value)
	{
		Vector3 result = _value;
		result.x = this.ReadSingle();
		result.y = this.ReadSingle();
		result.z = this.ReadSingle();
		return result;
	}

	// Token: 0x04006EAE RID: 28334
	public static int INSTANCES_LIVE;

	// Token: 0x04006EAF RID: 28335
	public static int INSTANCES_MAX;

	// Token: 0x04006EB0 RID: 28336
	public static int INSTANCES_CREATED;

	// Token: 0x04006EB1 RID: 28337
	[PublicizedFrom(EAccessModifier.Private)]
	public const int BYTE_BUFFER_SIZE = 128;

	// Token: 0x04006EB2 RID: 28338
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CHAR_BUFFER_SIZE = 128;

	// Token: 0x04006EB3 RID: 28339
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly byte[] buffer = new byte[128];

	// Token: 0x04006EB4 RID: 28340
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly char[] charBuffer = new char[128];

	// Token: 0x04006EB5 RID: 28341
	[PublicizedFrom(EAccessModifier.Private)]
	public Decoder decoder;

	// Token: 0x04006EB6 RID: 28342
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly StringBuilder stringBuilder = new StringBuilder(128);

	// Token: 0x04006EB7 RID: 28343
	[PublicizedFrom(EAccessModifier.Private)]
	public Stream baseStream;

	// Token: 0x04006EB8 RID: 28344
	[PublicizedFrom(EAccessModifier.Private)]
	public Encoding encoding;

	// Token: 0x020011EF RID: 4591
	public readonly struct StreamReadSizeMarker
	{
		// Token: 0x06008F6C RID: 36716 RVA: 0x00395343 File Offset: 0x00393543
		public StreamReadSizeMarker(long _position, uint _expectedSize)
		{
			this.Position = _position;
			this.ExpectedSize = _expectedSize;
		}

		// Token: 0x04006EB9 RID: 28345
		public readonly long Position;

		// Token: 0x04006EBA RID: 28346
		public readonly uint ExpectedSize;
	}
}

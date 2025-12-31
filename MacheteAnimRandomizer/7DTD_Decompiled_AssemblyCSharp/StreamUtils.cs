using System;
using System.IO;
using UnityEngine;

// Token: 0x0200121C RID: 4636
public static class StreamUtils
{
	// Token: 0x0600908B RID: 37003 RVA: 0x0039A8C8 File Offset: 0x00398AC8
	public static long ReadInt64(Stream clientStream)
	{
		return (long)(clientStream.ReadByte() & 255) | (long)(clientStream.ReadByte() & 255) << 8 | (long)(clientStream.ReadByte() & 255) << 16 | (long)(clientStream.ReadByte() & 255) << 24 | (long)(clientStream.ReadByte() & 255) << 32 | (long)(clientStream.ReadByte() & 255) << 40 | (long)(clientStream.ReadByte() & 255) << 48 | (long)(clientStream.ReadByte() & 255) << 56;
	}

	// Token: 0x0600908C RID: 37004 RVA: 0x0039A958 File Offset: 0x00398B58
	public static void Write(Stream clientStream, long v)
	{
		clientStream.WriteByte((byte)(v & 255L));
		clientStream.WriteByte((byte)(v >> 8 & 255L));
		clientStream.WriteByte((byte)(v >> 16 & 255L));
		clientStream.WriteByte((byte)(v >> 24 & 255L));
		clientStream.WriteByte((byte)(v >> 32 & 255L));
		clientStream.WriteByte((byte)(v >> 40 & 255L));
		clientStream.WriteByte((byte)(v >> 48 & 255L));
		clientStream.WriteByte((byte)(v >> 56 & 255L));
	}

	// Token: 0x0600908D RID: 37005 RVA: 0x0039A9F1 File Offset: 0x00398BF1
	public static int ReadInt32(Stream clientStream)
	{
		return 0 | (clientStream.ReadByte() & 255) | (clientStream.ReadByte() & 255) << 8 | (clientStream.ReadByte() & 255) << 16 | (clientStream.ReadByte() & 255) << 24;
	}

	// Token: 0x0600908E RID: 37006 RVA: 0x0039AA30 File Offset: 0x00398C30
	public static int ReadInt32(byte[] buffer, ref int offset)
	{
		int num = 0;
		int num2 = offset;
		offset = num2 + 1;
		int num3 = num | (int)buffer[num2];
		num2 = offset;
		offset = num2 + 1;
		int num4 = num3 | (int)buffer[num2] << 8;
		num2 = offset;
		offset = num2 + 1;
		int num5 = num4 | (int)buffer[num2] << 16;
		num2 = offset;
		offset = num2 + 1;
		return num5 | (int)buffer[num2] << 24;
	}

	// Token: 0x0600908F RID: 37007 RVA: 0x0039AA78 File Offset: 0x00398C78
	public static void Write(Stream clientStream, int v)
	{
		clientStream.WriteByte((byte)(v & 255));
		clientStream.WriteByte((byte)(v >> 8 & 255));
		clientStream.WriteByte((byte)(v >> 16 & 255));
		clientStream.WriteByte((byte)(v >> 24 & 255));
	}

	// Token: 0x06009090 RID: 37008 RVA: 0x0039AAC8 File Offset: 0x00398CC8
	public static void Write(byte[] buffer, int v, ref int offset)
	{
		int num = offset;
		offset = num + 1;
		buffer[num] = (byte)(v & 255);
		num = offset;
		offset = num + 1;
		buffer[num] = (byte)(v >> 8 & 255);
		num = offset;
		offset = num + 1;
		buffer[num] = (byte)(v >> 16 & 255);
		num = offset;
		offset = num + 1;
		buffer[num] = (byte)(v >> 24 & 255);
	}

	// Token: 0x06009091 RID: 37009 RVA: 0x0039AB29 File Offset: 0x00398D29
	public static ushort ReadUInt16(Stream clientStream)
	{
		return 0 | (ushort)(clientStream.ReadByte() & 255) | (ushort)((clientStream.ReadByte() & 255) << 8);
	}

	// Token: 0x06009092 RID: 37010 RVA: 0x0039AB4C File Offset: 0x00398D4C
	public static ushort ReadUInt16(byte[] buffer, ref int offset)
	{
		byte b = 0;
		int num = offset;
		offset = num + 1;
		ushort num2 = (ushort)(b | buffer[num]);
		num = offset;
		offset = num + 1;
		return num2 | (ushort)(buffer[num] << 8);
	}

	// Token: 0x06009093 RID: 37011 RVA: 0x0039AB77 File Offset: 0x00398D77
	public static void Write(Stream clientStream, ushort v)
	{
		clientStream.WriteByte((byte)(v & 255));
		clientStream.WriteByte((byte)(v >> 8 & 255));
	}

	// Token: 0x06009094 RID: 37012 RVA: 0x0039AB97 File Offset: 0x00398D97
	public static short ReadInt16(Stream clientStream)
	{
		return 0 | (short)(clientStream.ReadByte() & 255) | (short)((clientStream.ReadByte() & 255) << 8);
	}

	// Token: 0x06009095 RID: 37013 RVA: 0x0039ABBC File Offset: 0x00398DBC
	public static short ReadInt16(byte[] buffer, ref int offset)
	{
		byte b = 0;
		int num = offset;
		offset = num + 1;
		short num2 = (short)(b | buffer[num]);
		num = offset;
		offset = num + 1;
		return num2 | (short)(buffer[num] << 8);
	}

	// Token: 0x06009096 RID: 37014 RVA: 0x0039AB77 File Offset: 0x00398D77
	public static void Write(Stream clientStream, short v)
	{
		clientStream.WriteByte((byte)(v & 255));
		clientStream.WriteByte((byte)(v >> 8 & 255));
	}

	// Token: 0x06009097 RID: 37015 RVA: 0x0039ABE7 File Offset: 0x00398DE7
	public static byte ReadByte(Stream clientStream)
	{
		return (byte)clientStream.ReadByte();
	}

	// Token: 0x06009098 RID: 37016 RVA: 0x0039ABF0 File Offset: 0x00398DF0
	public static byte ReadByte(byte[] buffer, ref int offset)
	{
		int num = offset;
		offset = num + 1;
		return buffer[num];
	}

	// Token: 0x06009099 RID: 37017 RVA: 0x0039AC08 File Offset: 0x00398E08
	public static void Write(Stream clientStream, byte _b)
	{
		clientStream.WriteByte(_b);
	}

	// Token: 0x0600909A RID: 37018 RVA: 0x0039AC11 File Offset: 0x00398E11
	public static void Write(BinaryWriter _bw, Vector3 _v)
	{
		_bw.Write(_v.x);
		_bw.Write(_v.y);
		_bw.Write(_v.z);
	}

	// Token: 0x0600909B RID: 37019 RVA: 0x0039AC3A File Offset: 0x00398E3A
	public static Vector3 ReadVector3(BinaryReader _br)
	{
		return new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
	}

	// Token: 0x0600909C RID: 37020 RVA: 0x0039AC53 File Offset: 0x00398E53
	public static void Write(BinaryWriter _bw, Vector3i _v)
	{
		_bw.Write(_v.x);
		_bw.Write(_v.y);
		_bw.Write(_v.z);
	}

	// Token: 0x0600909D RID: 37021 RVA: 0x0039AC79 File Offset: 0x00398E79
	public static Vector3i ReadVector3i(BinaryReader _br)
	{
		return new Vector3i(_br.ReadInt32(), _br.ReadInt32(), _br.ReadInt32());
	}

	// Token: 0x0600909E RID: 37022 RVA: 0x0039AC92 File Offset: 0x00398E92
	public static void Write(BinaryWriter _bw, Vector2 _v)
	{
		_bw.Write(_v.x);
		_bw.Write(_v.y);
	}

	// Token: 0x0600909F RID: 37023 RVA: 0x0039ACAE File Offset: 0x00398EAE
	public static Vector2 ReadVector2(BinaryReader _br)
	{
		return new Vector2(_br.ReadSingle(), _br.ReadSingle());
	}

	// Token: 0x060090A0 RID: 37024 RVA: 0x0039ACC1 File Offset: 0x00398EC1
	public static void Write(BinaryWriter _bw, Vector2i _v)
	{
		_bw.Write(_v.x);
		_bw.Write(_v.y);
	}

	// Token: 0x060090A1 RID: 37025 RVA: 0x0039ACDB File Offset: 0x00398EDB
	public static Vector2i ReadVector2i(BinaryReader _br)
	{
		return new Vector2i(_br.ReadInt32(), _br.ReadInt32());
	}

	// Token: 0x060090A2 RID: 37026 RVA: 0x0039ACEE File Offset: 0x00398EEE
	public static void Write(BinaryWriter _bw, Quaternion _q)
	{
		_bw.Write(_q.x);
		_bw.Write(_q.y);
		_bw.Write(_q.z);
		_bw.Write(_q.w);
	}

	// Token: 0x060090A3 RID: 37027 RVA: 0x0039AD24 File Offset: 0x00398F24
	public static Quaternion ReadQuaterion(BinaryReader _br)
	{
		return new Quaternion(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
	}

	// Token: 0x060090A4 RID: 37028 RVA: 0x0039AD43 File Offset: 0x00398F43
	public static Color ReadColor(BinaryReader _br)
	{
		return new Color(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
	}

	// Token: 0x060090A5 RID: 37029 RVA: 0x0039AD62 File Offset: 0x00398F62
	public static void Write(BinaryWriter _bw, Color _c)
	{
		_bw.Write(_c.r);
		_bw.Write(_c.g);
		_bw.Write(_c.b);
		_bw.Write(_c.a);
	}

	// Token: 0x060090A6 RID: 37030 RVA: 0x0039AD98 File Offset: 0x00398F98
	public static Color ReadColor32(BinaryReader _br)
	{
		uint num = _br.ReadUInt32();
		return new Color((num >> 24 & 255U) / 255f, (num >> 16 & 255U) / 255f, (num >> 8 & 255U) / 255f, (num & 255U) / 255f);
	}

	// Token: 0x060090A7 RID: 37031 RVA: 0x0039ADF8 File Offset: 0x00398FF8
	public static void WriteColor32(BinaryWriter _bw, Color _c)
	{
		uint value = (uint)(_c.r * 255f) << 24 | (uint)(_c.g * 255f) << 16 | (uint)(_c.b * 255f) << 8 | (uint)(_c.a * 255f);
		_bw.Write(value);
	}

	// Token: 0x060090A8 RID: 37032 RVA: 0x0039AE4C File Offset: 0x0039904C
	public static void Write(BinaryWriter _bw, string _s)
	{
		_bw.Write(_s != null);
		if (_s != null)
		{
			_bw.Write(_s);
		}
	}

	// Token: 0x060090A9 RID: 37033 RVA: 0x0039AE62 File Offset: 0x00399062
	public static string ReadString(BinaryReader _br)
	{
		if (!_br.ReadBoolean())
		{
			return null;
		}
		return _br.ReadString();
	}

	// Token: 0x060090AA RID: 37034 RVA: 0x0039AE74 File Offset: 0x00399074
	public static void Write7BitEncodedSignedInt(Stream _stream, int _value)
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
		StreamUtils.Write(_stream, b);
		for (num = num2; num != 0L; num = num2)
		{
			num2 = (num >> 7 & 16777215L);
			b = (byte)(num & 127L);
			if (num2 != 0L)
			{
				b |= 128;
			}
			StreamUtils.Write(_stream, b);
		}
	}

	// Token: 0x060090AB RID: 37035 RVA: 0x0039AEEC File Offset: 0x003990EC
	public static void Write7BitEncodedInt(Stream _stream, int _value)
	{
		do
		{
			int num = _value >> 7 & 16777215;
			byte b = (byte)(_value & 127);
			if (num != 0)
			{
				b |= 128;
			}
			StreamUtils.Write(_stream, b);
			_value = num;
		}
		while (_value != 0);
	}

	// Token: 0x060090AC RID: 37036 RVA: 0x0039AF20 File Offset: 0x00399120
	public static int Read7BitEncodedSignedInt(Stream _stream)
	{
		int num = 0;
		int i = 0;
		byte b = (byte)(_stream.ReadByte() & 255);
		num |= (int)(b & 63);
		i += 6;
		bool flag = (b & 64) > 0;
		if ((b & 128) != 0)
		{
			while (i < 32)
			{
				b = (byte)(_stream.ReadByte() & 255);
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

	// Token: 0x060090AD RID: 37037 RVA: 0x0039AFA4 File Offset: 0x003991A4
	public static int Read7BitEncodedInt(Stream _stream)
	{
		int num = 0;
		int i = 0;
		while (i < 35)
		{
			byte b = (byte)(_stream.ReadByte() & 255);
			num |= (int)(b & 127) << i;
			i += 7;
			if ((b & 128) == 0)
			{
				return num;
			}
		}
		throw new FormatException("Illegal encoding for 7 bit encoded int");
	}

	// Token: 0x060090AE RID: 37038 RVA: 0x0039AFF0 File Offset: 0x003991F0
	public static void StreamCopy(Stream _source, Stream _destination, byte[] _tempBuf = null, bool _bFlush = true)
	{
		byte[] array = _tempBuf ?? MemoryPools.poolByte.Alloc(4096);
		bool flag = true;
		while (flag)
		{
			int num = _source.Read(array, 0, array.Length);
			if (num > 0)
			{
				_destination.Write(array, 0, num);
			}
			else
			{
				if (_bFlush)
				{
					_destination.Flush();
				}
				flag = false;
			}
		}
		if (_tempBuf == null)
		{
			MemoryPools.poolByte.Free(array);
		}
	}

	// Token: 0x060090AF RID: 37039 RVA: 0x0039B050 File Offset: 0x00399250
	public static void StreamCopy(Stream _source, Stream _destination, int _length, byte[] _tempBuf = null, bool _bFlush = true)
	{
		byte[] array = _tempBuf ?? MemoryPools.poolByte.Alloc(4096);
		bool flag = true;
		while (flag)
		{
			int num = _source.Read(array, 0, Math.Min(_length, array.Length));
			if (num > 0)
			{
				_destination.Write(array, 0, num);
				_length -= num;
			}
			else
			{
				if (_bFlush)
				{
					_destination.Flush();
				}
				flag = false;
			}
		}
		if (_tempBuf == null)
		{
			MemoryPools.poolByte.Free(array);
		}
	}

	// Token: 0x060090B0 RID: 37040 RVA: 0x0039B0BC File Offset: 0x003992BC
	public static void WriteStreamToFile(Stream _source, string _fileName)
	{
		using (Stream stream = SdFile.Create(_fileName))
		{
			StreamUtils.StreamCopy(_source, stream, null, true);
		}
	}

	// Token: 0x060090B1 RID: 37041 RVA: 0x0039B0F8 File Offset: 0x003992F8
	public static void WriteStreamToFile(Stream _source, string _fileName, int _length)
	{
		using (Stream stream = SdFile.Create(_fileName))
		{
			StreamUtils.StreamCopy(_source, stream, _length, null, true);
		}
	}
}

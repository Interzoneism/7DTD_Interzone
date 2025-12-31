using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

// Token: 0x02000A35 RID: 2613
public class RegionFileRaw : RegionFile
{
	// Token: 0x06004FDA RID: 20442 RVA: 0x001FAA6C File Offset: 0x001F8C6C
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileRaw(string fullFilePath, int rX, int rZ, int paddingBytes, int version) : base(fullFilePath, rX, rZ)
	{
		this.paddingBytes = paddingBytes;
		this.version = version;
	}

	// Token: 0x06004FDB RID: 20443 RVA: 0x001FAABC File Offset: 0x001F8CBC
	public static RegionFileRaw Load(string fullFilePath, int rX, int rZ)
	{
		RegionFileRaw result;
		using (Stream stream = SdFile.Open(fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			for (int i = 0; i < 3; i++)
			{
				if ((int)RegionFileRaw.FileHeaderMagicBytes[i] != stream.ReadByte())
				{
					throw new Exception("Incorrect header: " + fullFilePath);
				}
			}
			int num = StreamUtils.ReadInt32(stream);
			int num2 = StreamUtils.ReadInt32(stream);
			RegionFileRaw regionFileRaw = new RegionFileRaw(fullFilePath, rX, rZ, num2, num);
			RegionFileRaw.ReadBytes<int>(stream, regionFileRaw.locationHeader, 0, regionFileRaw.locationHeader.Length);
			RegionFileRaw.ReadBytes<uint>(stream, regionFileRaw.timestampHeader, 0, regionFileRaw.timestampHeader.Length);
			regionFileRaw.InitUsedSectors();
			regionFileRaw.Length = stream.Length;
			result = regionFileRaw;
		}
		return result;
	}

	// Token: 0x06004FDC RID: 20444 RVA: 0x001FAB7C File Offset: 0x001F8D7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitUsedSectors()
	{
		lock (this)
		{
			this.usedSectors.Clear();
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num;
					int num2;
					this.GetLocationInfo(j, i, out num, out num2);
					if (num > 0 && num2 > 0)
					{
						this.usedSectors.Add(num, num2);
					}
				}
			}
		}
	}

	// Token: 0x06004FDD RID: 20445 RVA: 0x001FABFC File Offset: 0x001F8DFC
	public static RegionFileRaw New(string fullFilePath, int rX, int rZ, int paddingBytes)
	{
		RegionFileRaw result;
		using (Stream stream = SdFile.Open(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
		{
			RegionFileRaw regionFileRaw = new RegionFileRaw(fullFilePath, rX, rZ, paddingBytes, 1);
			stream.Write(RegionFileRaw.FileHeaderMagicBytes, 0, 3);
			StreamUtils.Write(stream, 1);
			StreamUtils.Write(stream, paddingBytes);
			if (stream.Position != 11L)
			{
				throw new Exception(string.Format("Unexpected header length written. Expected: {0}, Actual: {1}", 11, stream.Position));
			}
			RegionFileRaw.WriteBytes<int>(stream, regionFileRaw.locationHeader, 0, regionFileRaw.locationHeader.Length);
			RegionFileRaw.WriteBytes<uint>(stream, regionFileRaw.timestampHeader, 0, regionFileRaw.timestampHeader.Length);
			regionFileRaw.Length = stream.Length;
			result = regionFileRaw;
		}
		return result;
	}

	// Token: 0x06004FDE RID: 20446 RVA: 0x001FACBC File Offset: 0x001F8EBC
	public override void ReadData(int cX, int cZ, Stream targetStream)
	{
		lock (this)
		{
			int num;
			int num2;
			this.GetLocationInfo(cX, cZ, out num, out num2);
			if (num < 779)
			{
				throw new Exception(string.Format("Reading outside of allowed bounds. R={0}/{1}, C={2}/{3}, Off={4}, Len={5}", new object[]
				{
					this.regionX,
					this.regionZ,
					cX,
					cZ,
					num,
					num2
				}));
			}
			using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				stream.Seek((long)num, SeekOrigin.Begin);
				int num3 = StreamUtils.ReadInt32(stream);
				if (num3 > 0)
				{
					targetStream.SetLength(0L);
				}
				StreamUtils.StreamCopy(stream, targetStream, num3, this.tempReadBuffer, true);
			}
		}
	}

	// Token: 0x06004FDF RID: 20447 RVA: 0x001FADB8 File Offset: 0x001F8FB8
	public override void WriteData(int cX, int cZ, int dataLength, byte compression, byte[] data, bool saveHeaderToFile)
	{
		lock (this)
		{
			uint timeStamp = GameUtils.WorldTimeToTotalMinutes(GameManager.Instance.World.worldTime);
			int num = dataLength + 4;
			int num2;
			int num3;
			this.GetLocationInfo(cX, cZ, out num2, out num3);
			if (num2 == 0 || num > num3)
			{
				num += this.paddingBytes;
				if (num2 > 0)
				{
					this.usedSectors.Remove(num2);
				}
				num2 = this.FindBestFreeSpace(num);
				this.SetLocationInfo(cX, cZ, num2, num);
			}
			else
			{
				num = num3;
			}
			if (num2 < 779)
			{
				throw new Exception(string.Format("Cannot write to protected offset: R={0}/{1}, C={2}/{3}, Off={4}, Len={5}, DataLen={6}", new object[]
				{
					this.regionX,
					this.regionZ,
					cX,
					cZ,
					num2,
					num,
					dataLength
				}));
			}
			this.SetTimestampInfo(cX, cZ, timeStamp);
			int num4 = num2 + num;
			using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				stream.Seek((long)num2, SeekOrigin.Begin);
				StreamUtils.Write(stream, dataLength);
				stream.Write(data, 0, dataLength);
				int num5 = num - 4 - dataLength;
				if (num5 > 0)
				{
					stream.Seek((long)(num5 - 1), SeekOrigin.Current);
					stream.WriteByte(0);
				}
				if ((long)num4 != stream.Position)
				{
					throw new Exception(string.Format("Wrong write end: R={0}/{1}, C={2}/{3}, Off={4}, Len={5}, DataLen={6}, FOEndExp={7}, FOEndFound={8}", new object[]
					{
						this.regionX,
						this.regionZ,
						cX,
						cZ,
						num2,
						num,
						dataLength,
						num4,
						stream.Position
					}));
				}
				base.Length = stream.Length;
			}
			if (saveHeaderToFile)
			{
				this.SaveHeaderData();
			}
		}
	}

	// Token: 0x06004FE0 RID: 20448 RVA: 0x001FAFE8 File Offset: 0x001F91E8
	public override void SaveHeaderData()
	{
		lock (this)
		{
			using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				stream.Seek(11L, SeekOrigin.Begin);
				RegionFileRaw.WriteBytes<int>(stream, this.locationHeader, 0, this.locationHeader.Length);
				RegionFileRaw.WriteBytes<uint>(stream, this.timestampHeader, 0, this.timestampHeader.Length);
			}
		}
	}

	// Token: 0x06004FE1 RID: 20449 RVA: 0x001FB078 File Offset: 0x001F9278
	public override bool HasChunk(int cX, int cZ)
	{
		int num;
		int num2;
		this.GetLocationInfo(cX, cZ, out num, out num2);
		return num > 0 && num2 > 0;
	}

	// Token: 0x06004FE2 RID: 20450 RVA: 0x001FB09C File Offset: 0x001F929C
	public override void RemoveChunk(int cX, int cZ)
	{
		lock (this)
		{
			int num;
			int num2;
			this.GetLocationInfo(cX, cZ, out num, out num2);
			if (num > 0 && num2 > 0)
			{
				this.usedSectors.Remove(num);
			}
			this.SetLocationInfo(cX, cZ, 0, 0);
		}
	}

	// Token: 0x06004FE3 RID: 20451 RVA: 0x001FB0FC File Offset: 0x001F92FC
	public override int ChunkCount()
	{
		int result;
		lock (this)
		{
			int num = 0;
			using (SortedDictionary<int, int>.ValueCollection.Enumerator enumerator = this.usedSectors.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current > 0)
					{
						num++;
					}
				}
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06004FE4 RID: 20452 RVA: 0x001FB180 File Offset: 0x001F9380
	public override int GetChunkByteCount(int cX, int cZ)
	{
		int num;
		int result;
		this.GetLocationInfo(cX, cZ, out num, out result);
		return result;
	}

	// Token: 0x06004FE5 RID: 20453 RVA: 0x001FB19C File Offset: 0x001F939C
	public override void GetTimestampInfo(int cX, int cZ, out uint timeStamp)
	{
		lock (this)
		{
			long offsetFromXz = this.GetOffsetFromXz(cX, cZ);
			timeStamp = this.timestampHeader[(int)(checked((IntPtr)offsetFromXz))];
		}
	}

	// Token: 0x06004FE6 RID: 20454 RVA: 0x001FB1E8 File Offset: 0x001F93E8
	public override void SetTimestampInfo(int cX, int cZ, uint timeStamp)
	{
		long offsetFromXz = this.GetOffsetFromXz(cX, cZ);
		this.timestampHeader[(int)(checked((IntPtr)offsetFromXz))] = timeStamp;
	}

	// Token: 0x06004FE7 RID: 20455 RVA: 0x001FB208 File Offset: 0x001F9408
	public override void OptimizeLayout()
	{
		lock (this)
		{
			this.usedSectors.Clear();
			using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
				{
					pooledExpandableMemoryStream.Seek(779L, SeekOrigin.Begin);
					for (int i = 0; i < 8; i++)
					{
						for (int j = 0; j < 8; j++)
						{
							int num;
							int num2;
							this.GetLocationInfo(j, i, out num, out num2);
							if (num > 0 && num2 > 0)
							{
								stream.Seek((long)num, SeekOrigin.Begin);
								int offset = (int)pooledExpandableMemoryStream.Position;
								StreamUtils.StreamCopy(stream, pooledExpandableMemoryStream, num2, null, true);
								this.SetLocationInfo(j, i, offset, num2);
							}
						}
					}
					stream.Seek(11L, SeekOrigin.Begin);
					RegionFileRaw.WriteBytes<int>(stream, this.locationHeader, 0, this.locationHeader.Length);
					RegionFileRaw.WriteBytes<uint>(stream, this.timestampHeader, 0, this.timestampHeader.Length);
					pooledExpandableMemoryStream.Seek(779L, SeekOrigin.Begin);
					StreamUtils.StreamCopy(pooledExpandableMemoryStream, stream, null, true);
					stream.SetLength(pooledExpandableMemoryStream.Length);
					base.Length = stream.Length;
				}
			}
		}
	}

	// Token: 0x06004FE8 RID: 20456 RVA: 0x001FB38C File Offset: 0x001F958C
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetLocationInfo(int cX, int cZ, out int offset, out int length)
	{
		lock (this)
		{
			long num = this.GetOffsetFromXz(cX, cZ) * 2L;
			checked
			{
				offset = this.locationHeader[(int)((IntPtr)num)];
				length = this.locationHeader[(int)((IntPtr)(unchecked(num + 1L)))];
			}
		}
	}

	// Token: 0x06004FE9 RID: 20457 RVA: 0x001FB3E8 File Offset: 0x001F95E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetLocationInfo(int cX, int cZ, int offset, int length)
	{
		lock (this)
		{
			long num = this.GetOffsetFromXz(cX, cZ) * 2L;
			checked
			{
				this.locationHeader[(int)((IntPtr)num)] = offset;
				this.locationHeader[(int)((IntPtr)(unchecked(num + 1L)))] = length;
				if (offset > 0)
				{
					this.usedSectors[offset] = length;
				}
			}
		}
	}

	// Token: 0x06004FEA RID: 20458 RVA: 0x001FB454 File Offset: 0x001F9654
	[PublicizedFrom(EAccessModifier.Private)]
	public long GetOffsetFromXz(int _cX, int _cZ)
	{
		int num = _cX % 8;
		int num2 = _cZ % 8;
		if (_cX < 0)
		{
			num += 7;
		}
		if (_cZ < 0)
		{
			num2 += 7;
		}
		return (long)(num + num2 * 8);
	}

	// Token: 0x06004FEB RID: 20459 RVA: 0x001FB480 File Offset: 0x001F9680
	[PublicizedFrom(EAccessModifier.Private)]
	public int FindBestFreeSpace(int requiredLength)
	{
		int result;
		lock (this)
		{
			int num = 779;
			int num2 = -1;
			int num3 = int.MaxValue;
			foreach (KeyValuePair<int, int> keyValuePair in this.usedSectors)
			{
				int num4 = keyValuePair.Key - num;
				int num5 = num4 - requiredLength;
				if (num5 == 0)
				{
					return num;
				}
				if (num4 > requiredLength && num5 < num3 - requiredLength)
				{
					num2 = num;
					num3 = num4;
				}
				num = keyValuePair.Key + keyValuePair.Value;
			}
			if (num2 > 0)
			{
				result = num2;
			}
			else
			{
				result = num;
			}
		}
		return result;
	}

	// Token: 0x06004FEC RID: 20460 RVA: 0x001FB54C File Offset: 0x001F974C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void WriteBytes<[IsUnmanaged] T>(Stream stream, T[] buf, int offset, int length) where T : struct, ValueType
	{
		ReadOnlySpan<byte> buffer = MemoryMarshal.Cast<T, byte>(buf.AsSpan(offset, length));
		stream.Write(buffer);
	}

	// Token: 0x06004FED RID: 20461 RVA: 0x001FB574 File Offset: 0x001F9774
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ReadBytes<[IsUnmanaged] T>(Stream stream, T[] buf, int offset, int length) where T : struct, ValueType
	{
		Span<byte> span = MemoryMarshal.Cast<T, byte>(buf.AsSpan(offset, length));
		while (span.Length > 0)
		{
			int num = stream.Read(span);
			if (num <= 0)
			{
				throw new EndOfStreamException();
			}
			Span<byte> span2 = span;
			int num2 = num;
			span = span2.Slice(num2, span2.Length - num2);
		}
	}

	// Token: 0x04003D1D RID: 15645
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CurrentVersion = 1;

	// Token: 0x04003D1E RID: 15646
	[PublicizedFrom(EAccessModifier.Protected)]
	public static readonly byte[] FileHeaderMagicBytes = Encoding.ASCII.GetBytes("7rr");

	// Token: 0x04003D1F RID: 15647
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int FileHeaderMagicBytesLength = 3;

	// Token: 0x04003D20 RID: 15648
	public const int ChunksPerRegionPerDimension = 8;

	// Token: 0x04003D21 RID: 15649
	public const int ChunksPerRegion = 64;

	// Token: 0x04003D22 RID: 15650
	[PublicizedFrom(EAccessModifier.Private)]
	public const int fileHeaderLength = 11;

	// Token: 0x04003D23 RID: 15651
	[PublicizedFrom(EAccessModifier.Private)]
	public const int locationHeaderLength = 128;

	// Token: 0x04003D24 RID: 15652
	[PublicizedFrom(EAccessModifier.Private)]
	public const int timestampHeaderLength = 64;

	// Token: 0x04003D25 RID: 15653
	[PublicizedFrom(EAccessModifier.Private)]
	public const int sectorsStartOffset = 779;

	// Token: 0x04003D26 RID: 15654
	[PublicizedFrom(EAccessModifier.Private)]
	public const int reservedBytesPerEntry = 4;

	// Token: 0x04003D27 RID: 15655
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int[] locationHeader = new int[128];

	// Token: 0x04003D28 RID: 15656
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly uint[] timestampHeader = new uint[64];

	// Token: 0x04003D29 RID: 15657
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int paddingBytes;

	// Token: 0x04003D2A RID: 15658
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly int version;

	// Token: 0x04003D2B RID: 15659
	[PublicizedFrom(EAccessModifier.Private)]
	public SortedDictionary<int, int> usedSectors = new SortedDictionary<int, int>();
}

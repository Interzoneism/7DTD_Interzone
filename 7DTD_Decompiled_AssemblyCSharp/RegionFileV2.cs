using System;
using System.Collections.Generic;
using System.IO;

// Token: 0x02000A38 RID: 2616
public class RegionFileV2 : RegionFileSectorBased
{
	// Token: 0x06005000 RID: 20480 RVA: 0x001FBCE8 File Offset: 0x001F9EE8
	public RegionFileV2(string _fullFilePath, int _rX, int _rZ, Stream _fileStream, int _version) : base(_fullFilePath, _rX, _rZ, _version)
	{
		try
		{
			if (_fileStream == null)
			{
				_fileStream = SdFile.Open(_fullFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
				byte[] array = new byte[12288];
				array[0] = RegionFileSectorBased.FileHeaderMagicBytes[0];
				array[1] = RegionFileSectorBased.FileHeaderMagicBytes[1];
				array[2] = RegionFileSectorBased.FileHeaderMagicBytes[2];
				array[3] = (byte)_version;
				_fileStream.Write(array, 0, array.Length);
				_fileStream.Seek(0L, SeekOrigin.Begin);
			}
			_fileStream.Seek(4096L, SeekOrigin.Begin);
			_fileStream.Read(this.regionLocationHeader, 0, 4096);
			_fileStream.Read(this.regionTimestampHeader, 0, 4096);
			this.initSectorInfo();
			base.Length = _fileStream.Length;
		}
		finally
		{
			if (_fileStream != null)
			{
				_fileStream.Dispose();
			}
		}
	}

	// Token: 0x06005001 RID: 20481 RVA: 0x001FBDC8 File Offset: 0x001F9FC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void initSectorInfo()
	{
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				short key;
				byte b;
				this.GetLocationInfo(j, i, out key, out b);
				if (b > 0)
				{
					this.usedSectors[(int)key] = (int)b;
				}
			}
		}
	}

	// Token: 0x06005002 RID: 20482 RVA: 0x001FBE0C File Offset: 0x001FA00C
	[PublicizedFrom(EAccessModifier.Private)]
	public int findFreeSectorOfSize(int _sectorLength)
	{
		int result;
		lock (this)
		{
			int num = 3;
			foreach (KeyValuePair<int, int> keyValuePair in this.usedSectors)
			{
				if (keyValuePair.Key - num >= _sectorLength)
				{
					return num;
				}
				num = keyValuePair.Key + keyValuePair.Value;
			}
			result = num;
		}
		return result;
	}

	// Token: 0x06005003 RID: 20483 RVA: 0x001FBEA8 File Offset: 0x001FA0A8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SetLocationInfo(int _cX, int _cZ, short _sectorOffset, byte _sectorLength)
	{
		lock (this)
		{
			base.SetLocationInfo(_cX, _cZ, _sectorOffset, _sectorLength);
			this.usedSectors[(int)_sectorOffset] = (int)_sectorLength;
		}
	}

	// Token: 0x06005004 RID: 20484 RVA: 0x001FBEF8 File Offset: 0x001FA0F8
	public override void SaveHeaderData()
	{
		lock (this)
		{
			using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
			{
				stream.Seek(4096L, SeekOrigin.Begin);
				stream.Write(this.regionLocationHeader, 0, 4096);
				stream.Write(this.regionTimestampHeader, 0, 4096);
				base.Length = stream.Length;
			}
		}
	}

	// Token: 0x06005005 RID: 20485 RVA: 0x001FBF90 File Offset: 0x001FA190
	public override void ReadData(int _cX, int _cZ, Stream _targetStream)
	{
		lock (this)
		{
			short num;
			byte b;
			this.GetLocationInfo(_cX, _cZ, out num, out b);
			using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				long num2 = (long)(num * 4096);
				int num3 = (int)b * 4096;
				stream.Seek(num2, SeekOrigin.Begin);
				int num4 = StreamUtils.ReadInt32(stream);
				stream.Seek(12L, SeekOrigin.Current);
				long num5 = (long)num3 + num2;
				long position = _targetStream.Position;
				if (num4 > 0)
				{
					_targetStream.SetLength(0L);
				}
				if (num < 3)
				{
					Log.Error(string.Format("ChunkRead: R={0}/{1}, C={2}/{3}, Off={4}, Len={5}, DataLen={6}, TotalLen={7}, FOStart={8}, FSize={9}, FOEndExp={10}, FOEndRead={11}, TSPosBefore={12}", new object[]
					{
						this.regionX,
						this.regionZ,
						_cX,
						_cZ,
						num,
						b,
						num4,
						num4 + 16,
						num2,
						num3,
						num5,
						stream.Position,
						position
					}));
				}
				else
				{
					try
					{
						StreamUtils.StreamCopy(stream, _targetStream, num4, this.tempReadBuffer, true);
					}
					catch (NotSupportedException)
					{
						Log.Error(string.Format("ChunkRead: R={0}/{1}, C={2}/{3}, Off={4}, Len={5}, DataLen={6}, TotalLen={7}, FOStart={8}, FSize={9}, FOEndExp={10}, FOEndRead={11}, TSPosBefore={12}", new object[]
						{
							this.regionX,
							this.regionZ,
							_cX,
							_cZ,
							num,
							b,
							num4,
							num4 + 16,
							num2,
							num3,
							num5,
							stream.Position,
							position
						}));
						throw;
					}
				}
				base.Length = stream.Length;
			}
		}
	}

	// Token: 0x06005006 RID: 20486 RVA: 0x001FC1FC File Offset: 0x001FA3FC
	public override void WriteData(int _cX, int _cZ, int _dataLength, byte _compression, byte[] _data, bool _saveHeaderToFile)
	{
		lock (this)
		{
			uint timeStamp = GameUtils.WorldTimeToTotalMinutes(GameManager.Instance.World.worldTime);
			int num = _dataLength + 16;
			byte b = (byte)Math.Ceiling((double)num / 4096.0);
			using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
			{
				short num2;
				byte b2;
				this.GetLocationInfo(_cX, _cZ, out num2, out b2);
				if (num2 == 0 || b > b2)
				{
					if (num2 > 0)
					{
						this.usedSectors.Remove((int)num2);
					}
					num2 = (short)this.findFreeSectorOfSize((int)b);
					this.SetLocationInfo(_cX, _cZ, num2, b);
				}
				if (num2 < 3)
				{
					Log.Error(string.Format("Sector offset < 3: R={0}/{1}, C={2}/{3}, Off={4}, Len={5}, DataLen={6}", new object[]
					{
						this.regionX,
						this.regionZ,
						_cX,
						_cZ,
						num2,
						b,
						_dataLength
					}));
				}
				this.SetTimestampInfo(_cX, _cZ, timeStamp);
				long num3 = (long)(num2 * 4096);
				int num4 = (int)b * 4096;
				long num5 = (long)num4 + num3;
				stream.Seek(num3, SeekOrigin.Begin);
				StreamUtils.Write(stream, _dataLength);
				stream.Seek(12L, SeekOrigin.Current);
				stream.Write(_data, 0, _dataLength);
				stream.Write(RegionFileV2.emptyBytes, 0, num4 - num);
				long position = stream.Position;
				if (position != num5)
				{
					Log.Error(string.Format("Wrong write end: R={0}/{1}, C={2}/{3}, Off={4}, Len={5}, DataLen={6}, TotalLen={7}, FOStart={8}, FSize={9}, FOEndExp={10}, FOEndFound={11}", new object[]
					{
						this.regionX,
						this.regionZ,
						_cX,
						_cZ,
						num2,
						b,
						_dataLength,
						num,
						num3,
						num4,
						num5,
						position
					}));
				}
				base.Length = stream.Length;
			}
			if (_saveHeaderToFile)
			{
				this.SaveHeaderData();
			}
		}
	}

	// Token: 0x06005007 RID: 20487 RVA: 0x001FC468 File Offset: 0x001FA668
	public override void RemoveChunk(int _cX, int _cZ)
	{
		lock (this)
		{
			short num;
			byte b;
			this.GetLocationInfo(_cX, _cZ, out num, out b);
			if (num > 0 && b > 0)
			{
				this.usedSectors.Remove((int)num);
			}
			base.RemoveChunk(_cX, _cZ);
		}
	}

	// Token: 0x06005008 RID: 20488 RVA: 0x001FC4C8 File Offset: 0x001FA6C8
	public override void OptimizeLayout()
	{
		using (Stream stream = SdFile.Open(this.fullFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
		{
			int num = 3;
			foreach (int num2 in this.usedSectors.Values)
			{
				num += num2;
			}
			int num3 = num * 4096;
			this.usedSectors.Clear();
			RegionFileV2.optimizerMemoryStream.Position = 0L;
			stream.Seek(0L, SeekOrigin.Begin);
			StreamUtils.StreamCopy(stream, RegionFileV2.optimizerMemoryStream, 12288, null, true);
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 32; j++)
				{
					short num4;
					byte b;
					this.GetLocationInfo(j, i, out num4, out b);
					if (num4 > 0 && b > 0)
					{
						stream.Seek((long)(num4 * 4096), SeekOrigin.Begin);
						short sectorOffset = (short)(RegionFileV2.optimizerMemoryStream.Position / 4096L);
						StreamUtils.StreamCopy(stream, RegionFileV2.optimizerMemoryStream, (int)b * 4096, null, true);
						this.SetLocationInfo(j, i, sectorOffset, b);
					}
				}
			}
			RegionFileV2.optimizerMemoryStream.Seek(4096L, SeekOrigin.Begin);
			RegionFileV2.optimizerMemoryStream.Write(this.regionLocationHeader, 0, 4096);
			stream.SetLength((long)num3);
			stream.Position = 0L;
			RegionFileV2.optimizerMemoryStream.Position = 0L;
			StreamUtils.StreamCopy(RegionFileV2.optimizerMemoryStream, stream, num3, null, true);
			base.Length = stream.Length;
		}
	}

	// Token: 0x06005009 RID: 20489 RVA: 0x001FC67C File Offset: 0x001FA87C
	public override int ChunkCount()
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
		return num;
	}

	// Token: 0x04003D36 RID: 15670
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly SortedDictionary<int, int> usedSectors = new SortedDictionary<int, int>();

	// Token: 0x04003D37 RID: 15671
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ReservedSectors = 3;

	// Token: 0x04003D38 RID: 15672
	[PublicizedFrom(EAccessModifier.Private)]
	public const int ReservedBytesPerEntry = 16;

	// Token: 0x04003D39 RID: 15673
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly byte[] emptyBytes = new byte[4096];

	// Token: 0x04003D3A RID: 15674
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryStream optimizerMemoryStream = new MemoryStream(16789504);
}

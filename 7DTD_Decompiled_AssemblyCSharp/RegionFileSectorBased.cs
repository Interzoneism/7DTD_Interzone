using System;
using System.IO;
using System.Text;

// Token: 0x02000A36 RID: 2614
public abstract class RegionFileSectorBased : RegionFile
{
	// Token: 0x06004FEF RID: 20463 RVA: 0x001FB5D7 File Offset: 0x001F97D7
	[PublicizedFrom(EAccessModifier.Protected)]
	public RegionFileSectorBased(string _fullFilePath, int _rX, int _rZ, int _version) : base(_fullFilePath, _rX, _rZ)
	{
		this.Version = _version;
	}

	// Token: 0x06004FF0 RID: 20464 RVA: 0x001FB60C File Offset: 0x001F980C
	public static RegionFile Get(string dir, int rX, int rZ, string ext)
	{
		string text = RegionFile.ConstructFullFilePath(dir, rX, rZ, ext);
		if (!SdFile.Exists(text))
		{
			SdFile.Create(text).Close();
			return new RegionFileV2(text, rX, rZ, null, 1);
		}
		Stream stream = SdFile.Open(text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		byte[] array = new byte[3];
		stream.Read(array, 0, 3);
		for (int i = 0; i < 3; i++)
		{
			if (array[i] != RegionFileSectorBased.FileHeaderMagicBytes[i])
			{
				throw new Exception("Incorrect region file header! " + text);
			}
		}
		int num = (int)((byte)stream.ReadByte());
		if (num < 1)
		{
			return new RegionFileV1(text, rX, rZ, stream, num);
		}
		return new RegionFileV2(text, rX, rZ, stream, num);
	}

	// Token: 0x06004FF1 RID: 20465 RVA: 0x001FB6AC File Offset: 0x001F98AC
	public override void GetTimestampInfo(int _cX, int _cZ, out uint _timeStamp)
	{
		lock (this)
		{
			long offsetFromXz = this.GetOffsetFromXz(_cX, _cZ);
			_timeStamp = BitConverter.ToUInt32(this.regionTimestampHeader, (int)offsetFromXz);
		}
	}

	// Token: 0x06004FF2 RID: 20466 RVA: 0x001FB6FC File Offset: 0x001F98FC
	public override void SetTimestampInfo(int _cX, int _cZ, uint _timeStamp)
	{
		lock (this)
		{
			long offsetFromXz = this.GetOffsetFromXz(_cX, _cZ);
			Utils.GetBytes(_timeStamp, this.regionTimestampHeader, (int)offsetFromXz);
		}
	}

	// Token: 0x06004FF3 RID: 20467 RVA: 0x001FB748 File Offset: 0x001F9948
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void GetLocationInfo(int _cX, int _cZ, out short _sectorOffset, out byte _sectorLength)
	{
		checked
		{
			lock (this)
			{
				long offsetFromXz = this.GetOffsetFromXz(_cX, _cZ);
				_sectorOffset = RegionFile.ToShort((short)this.regionLocationHeader[(int)((IntPtr)offsetFromXz)], (short)this.regionLocationHeader[(int)((IntPtr)(unchecked(offsetFromXz + 1L)))]);
				_sectorLength = this.regionLocationHeader[(int)((IntPtr)(unchecked(offsetFromXz + 3L)))];
			}
		}
	}

	// Token: 0x06004FF4 RID: 20468 RVA: 0x001FB7B4 File Offset: 0x001F99B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetLocationInfo(int _cX, int _cZ, short _sectorOffset, byte _sectorLength)
	{
		checked
		{
			lock (this)
			{
				long offsetFromXz = this.GetOffsetFromXz(_cX, _cZ);
				RegionFile.FromShort(_sectorOffset, out this.regionLocationHeader[(int)((IntPtr)offsetFromXz)], out this.regionLocationHeader[(int)((IntPtr)(unchecked(offsetFromXz + 1L)))]);
				this.regionLocationHeader[(int)((IntPtr)(unchecked(offsetFromXz + 3L)))] = _sectorLength;
			}
		}
	}

	// Token: 0x06004FF5 RID: 20469 RVA: 0x001FB824 File Offset: 0x001F9A24
	public override bool HasChunk(int _cX, int _cZ)
	{
		short num;
		byte b;
		this.GetLocationInfo(_cX, _cZ, out num, out b);
		return num > 0 && b > 0;
	}

	// Token: 0x06004FF6 RID: 20470 RVA: 0x001FB848 File Offset: 0x001F9A48
	public override int GetChunkByteCount(int _cX, int _cZ)
	{
		short num;
		byte b;
		this.GetLocationInfo(_cX, _cZ, out num, out b);
		if (num <= 0 || b <= 0)
		{
			return 0;
		}
		return (int)b * 4096;
	}

	// Token: 0x06004FF7 RID: 20471 RVA: 0x001FB872 File Offset: 0x001F9A72
	public override void RemoveChunk(int _cX, int _cZ)
	{
		this.SetLocationInfo(_cX, _cZ, 0, 0);
	}

	// Token: 0x06004FF8 RID: 20472 RVA: 0x001FB880 File Offset: 0x001F9A80
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual long GetOffsetFromXz(int _cX, int _cZ)
	{
		int num = _cX % 32;
		int num2 = _cZ % 32;
		if (_cX < 0)
		{
			num += 31;
		}
		if (_cZ < 0)
		{
			num2 += 31;
		}
		return (long)(4 * (num + num2 * 32));
	}

	// Token: 0x04003D2C RID: 15660
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int CURRENT_VERSION = 1;

	// Token: 0x04003D2D RID: 15661
	[PublicizedFrom(EAccessModifier.Protected)]
	public static readonly byte[] FileHeaderMagicBytes = Encoding.ASCII.GetBytes("7rg");

	// Token: 0x04003D2E RID: 15662
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int FileHeaderMagicBytesLength = 3;

	// Token: 0x04003D2F RID: 15663
	public const int ChunksPerRegionPerDimension = 32;

	// Token: 0x04003D30 RID: 15664
	public const int ChunksPerRegion = 1024;

	// Token: 0x04003D31 RID: 15665
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int SECTOR_SIZE = 4096;

	// Token: 0x04003D32 RID: 15666
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly byte[] regionLocationHeader = new byte[4096];

	// Token: 0x04003D33 RID: 15667
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly byte[] regionTimestampHeader = new byte[4096];

	// Token: 0x04003D34 RID: 15668
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly int Version;
}

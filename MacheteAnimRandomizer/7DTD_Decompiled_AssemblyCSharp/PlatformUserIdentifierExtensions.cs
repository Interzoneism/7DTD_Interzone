using System;
using System.IO;
using System.Text;

// Token: 0x02000060 RID: 96
public static class PlatformUserIdentifierExtensions
{
	// Token: 0x060001D0 RID: 464 RVA: 0x0000FEC8 File Offset: 0x0000E0C8
	public static void ToStream(this PlatformUserIdentifierAbs _instance, Stream _targetStream, bool _inclCustomData = false)
	{
		if (_targetStream == null)
		{
			return;
		}
		if (_instance == null)
		{
			_targetStream.WriteByte(0);
			return;
		}
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(true))
		{
			pooledBinaryWriter.SetBaseStream(_targetStream);
			_instance.ToStream(pooledBinaryWriter, _inclCustomData);
		}
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000FF1C File Offset: 0x0000E11C
	public static void ToStream(this PlatformUserIdentifierAbs _instance, BinaryWriter _writer, bool _inclCustomData = false)
	{
		if (_writer == null)
		{
			return;
		}
		if (_instance == null)
		{
			_writer.Write(0);
			return;
		}
		_writer.Write(1);
		_writer.Write(1);
		_writer.Write(_instance.PlatformIdentifierString);
		_writer.Write(_instance.ReadablePlatformUserIdentifier);
		if (_inclCustomData)
		{
			_instance.WriteCustomData(_writer);
		}
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000FF68 File Offset: 0x0000E168
	public static int GetToStreamLength(this PlatformUserIdentifierAbs _instance, Encoding encoding, bool _inclCustomData = false)
	{
		if (_instance == null)
		{
			return 1;
		}
		return 0 + 1 + 1 + _instance.PlatformIdentifierString.GetBinaryWriterLength(encoding) + _instance.ReadablePlatformUserIdentifier.GetBinaryWriterLength(encoding) + _instance.GetCustomDataLengthEstimate();
	}
}

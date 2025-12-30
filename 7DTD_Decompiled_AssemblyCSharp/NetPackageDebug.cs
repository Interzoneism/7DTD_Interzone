using System;
using UnityEngine.Scripting;

// Token: 0x02000715 RID: 1813
[Preserve]
public class NetPackageDebug : NetPackage
{
	// Token: 0x06003550 RID: 13648 RVA: 0x001636D7 File Offset: 0x001618D7
	public NetPackageDebug Setup(NetPackageDebug.Type _type, int _entityId = -1, byte[] _data = null)
	{
		this.type = _type;
		this.entityId = _entityId;
		this.data = _data;
		return this;
	}

	// Token: 0x06003551 RID: 13649 RVA: 0x001636F0 File Offset: 0x001618F0
	public override void read(PooledBinaryReader _reader)
	{
		this.type = (NetPackageDebug.Type)_reader.ReadInt16();
		this.entityId = _reader.ReadInt32();
		int num = _reader.ReadInt32();
		if (num > 0)
		{
			this.data = _reader.ReadBytes(num);
		}
	}

	// Token: 0x06003552 RID: 13650 RVA: 0x00163730 File Offset: 0x00161930
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((short)this.type);
		_writer.Write(this.entityId);
		if (this.data == null || this.data.Length == 0)
		{
			_writer.Write(0);
			return;
		}
		_writer.Write(this.data.Length);
		_writer.Write(this.data);
	}

	// Token: 0x06003553 RID: 13651 RVA: 0x00163790 File Offset: 0x00161990
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		switch (this.type)
		{
		case NetPackageDebug.Type.AILatency:
			AIDirector.DebugReceiveLatency(this.entityId, this.data);
			return;
		case NetPackageDebug.Type.AILatencyClientOff:
			AIDirector.DebugLatencyOff();
			return;
		case NetPackageDebug.Type.AINameInfo:
			AIDirector.DebugReceiveNameInfo(this.entityId, this.data);
			return;
		case NetPackageDebug.Type.AINameInfoClientOff:
			EntityAlive.SetupAllDebugNameHUDs(false);
			return;
		case NetPackageDebug.Type.AINameInfoServerToggle:
			AIDirector.DebugToggleSendNameInfo(base.Sender.entityId);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003554 RID: 13652 RVA: 0x00163800 File Offset: 0x00161A00
	public override int GetLength()
	{
		return 10 + ((this.data == null) ? 0 : this.data.Length);
	}

	// Token: 0x17000567 RID: 1383
	// (get) Token: 0x06003555 RID: 13653 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.Both;
		}
	}

	// Token: 0x04002B7B RID: 11131
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageDebug.Type type;

	// Token: 0x04002B7C RID: 11132
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002B7D RID: 11133
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;

	// Token: 0x02000716 RID: 1814
	public enum Type
	{
		// Token: 0x04002B7F RID: 11135
		AILatency,
		// Token: 0x04002B80 RID: 11136
		AILatencyClientOff,
		// Token: 0x04002B81 RID: 11137
		AINameInfo,
		// Token: 0x04002B82 RID: 11138
		AINameInfoClientOff,
		// Token: 0x04002B83 RID: 11139
		AINameInfoServerToggle
	}
}

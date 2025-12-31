using System;
using UnityEngine.Scripting;

// Token: 0x02000824 RID: 2084
[Preserve]
public class NetPackagePersistentPlayerState : NetPackage
{
	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x06003BEB RID: 15339 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003BEC RID: 15340 RVA: 0x00181523 File Offset: 0x0017F723
	public NetPackagePersistentPlayerState Setup(PersistentPlayerData ppData, EnumPersistentPlayerDataReason reason)
	{
		this.m_ppData = ppData;
		this.m_reason = reason;
		return this;
	}

	// Token: 0x06003BED RID: 15341 RVA: 0x00181534 File Offset: 0x0017F734
	public override void read(PooledBinaryReader _reader)
	{
		this.m_reason = (EnumPersistentPlayerDataReason)_reader.ReadByte();
		this.m_ppData = PersistentPlayerData.Read(_reader);
	}

	// Token: 0x06003BEE RID: 15342 RVA: 0x0018154E File Offset: 0x0017F74E
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((byte)this.m_reason);
		this.m_ppData.Write(_writer);
	}

	// Token: 0x06003BEF RID: 15343 RVA: 0x00181570 File Offset: 0x0017F770
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		_callbacks.PersistentPlayerLogin(this.m_ppData);
	}

	// Token: 0x06003BF0 RID: 15344 RVA: 0x0005B5EB File Offset: 0x000597EB
	public override int GetLength()
	{
		return 1000;
	}

	// Token: 0x0400308E RID: 12430
	[PublicizedFrom(EAccessModifier.Private)]
	public PersistentPlayerData m_ppData;

	// Token: 0x0400308F RID: 12431
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumPersistentPlayerDataReason m_reason;
}

using System;
using UnityEngine.Scripting;

// Token: 0x02000825 RID: 2085
[Preserve]
public class NetPackagePlayerAcl : NetPackage
{
	// Token: 0x06003BF2 RID: 15346 RVA: 0x0018157E File Offset: 0x0017F77E
	public NetPackagePlayerAcl Setup(PlatformUserIdentifierAbs playerId, PlatformUserIdentifierAbs otherPlayerID, EnumPersistentPlayerDataReason reason)
	{
		this.m_reason = reason;
		this.m_playerID = playerId;
		this.m_otherPlayerID = otherPlayerID;
		return this;
	}

	// Token: 0x06003BF3 RID: 15347 RVA: 0x00181596 File Offset: 0x0017F796
	public override void read(PooledBinaryReader _reader)
	{
		this.m_reason = (EnumPersistentPlayerDataReason)_reader.ReadByte();
		this.m_playerID = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
		this.m_otherPlayerID = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
	}

	// Token: 0x06003BF4 RID: 15348 RVA: 0x001815C0 File Offset: 0x0017F7C0
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((byte)this.m_reason);
		this.m_playerID.ToStream(_writer, false);
		this.m_otherPlayerID.ToStream(_writer, false);
	}

	// Token: 0x06003BF5 RID: 15349 RVA: 0x001815F0 File Offset: 0x0017F7F0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!base.ValidUserIdForSender(this.m_playerID))
		{
			return;
		}
		_callbacks.PersistentPlayerEvent(this.m_playerID, this.m_otherPlayerID, this.m_reason);
	}

	// Token: 0x06003BF6 RID: 15350 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04003090 RID: 12432
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumPersistentPlayerDataReason m_reason;

	// Token: 0x04003091 RID: 12433
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs m_playerID;

	// Token: 0x04003092 RID: 12434
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs m_otherPlayerID;
}

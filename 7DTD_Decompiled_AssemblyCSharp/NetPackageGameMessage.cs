using System;
using UnityEngine.Scripting;

// Token: 0x0200074E RID: 1870
[Preserve]
public class NetPackageGameMessage : NetPackage
{
	// Token: 0x06003694 RID: 13972 RVA: 0x00167E24 File Offset: 0x00166024
	public NetPackageGameMessage Setup(EnumGameMessages _type, int _mainEntityId, int _secondaryEntityId)
	{
		this.msgType = _type;
		this.mainEntityId = _mainEntityId;
		this.secondaryEntityId = _secondaryEntityId;
		return this;
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x00167E3C File Offset: 0x0016603C
	public override void read(PooledBinaryReader _br)
	{
		this.msgType = (EnumGameMessages)_br.ReadByte();
		this.mainEntityId = _br.ReadInt32();
		this.secondaryEntityId = _br.ReadInt32();
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x00167E62 File Offset: 0x00166062
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.msgType);
		_bw.Write(this.mainEntityId);
		_bw.Write(this.secondaryEntityId);
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x00167E90 File Offset: 0x00166090
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			GameManager.Instance.GameMessageServer(base.Sender, this.msgType, this.mainEntityId, this.secondaryEntityId);
			return;
		}
		GameManager.Instance.DisplayGameMessage(this.msgType, this.mainEntityId, this.secondaryEntityId, true);
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002C5D RID: 11357
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumGameMessages msgType;

	// Token: 0x04002C5E RID: 11358
	[PublicizedFrom(EAccessModifier.Private)]
	public int mainEntityId;

	// Token: 0x04002C5F RID: 11359
	[PublicizedFrom(EAccessModifier.Private)]
	public int secondaryEntityId;
}

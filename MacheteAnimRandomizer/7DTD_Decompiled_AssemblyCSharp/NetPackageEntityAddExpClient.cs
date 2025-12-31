using System;
using UnityEngine.Scripting;

// Token: 0x0200072B RID: 1835
[Preserve]
public class NetPackageEntityAddExpClient : NetPackage
{
	// Token: 0x17000570 RID: 1392
	// (get) Token: 0x060035BE RID: 13758 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060035BF RID: 13759 RVA: 0x00164EA8 File Offset: 0x001630A8
	public NetPackageEntityAddExpClient Setup(int _entityId, int _xp, Progression.XPTypes _xpType)
	{
		this.entityId = _entityId;
		this.xp = _xp;
		this.xpType = (int)_xpType;
		return this;
	}

	// Token: 0x060035C0 RID: 13760 RVA: 0x00164EC0 File Offset: 0x001630C0
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.xp = _reader.ReadInt32();
		this.xpType = (int)_reader.ReadInt16();
	}

	// Token: 0x060035C1 RID: 13761 RVA: 0x00164EE6 File Offset: 0x001630E6
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.xp);
		_writer.Write((short)this.xpType);
	}

	// Token: 0x060035C2 RID: 13762 RVA: 0x00164F14 File Offset: 0x00163114
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)_world.GetEntity(this.entityId);
		if (entityAlive != null)
		{
			string cvarXPName = "_xpOther";
			if (this.xpType == 0)
			{
				cvarXPName = "_xpFromKill";
			}
			entityAlive.Progression.AddLevelExp(this.xp, cvarXPName, (Progression.XPTypes)this.xpType, true, true);
		}
	}

	// Token: 0x060035C3 RID: 13763 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002BC9 RID: 11209
	[PublicizedFrom(EAccessModifier.Protected)]
	public int entityId;

	// Token: 0x04002BCA RID: 11210
	[PublicizedFrom(EAccessModifier.Protected)]
	public int xp;

	// Token: 0x04002BCB RID: 11211
	[PublicizedFrom(EAccessModifier.Protected)]
	public int xpType;
}

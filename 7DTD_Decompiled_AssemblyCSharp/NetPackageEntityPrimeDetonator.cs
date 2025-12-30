using System;
using UnityEngine.Scripting;

// Token: 0x0200073B RID: 1851
[Preserve]
public class NetPackageEntityPrimeDetonator : NetPackage
{
	// Token: 0x1700057B RID: 1403
	// (get) Token: 0x0600361F RID: 13855 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003620 RID: 13856 RVA: 0x00166044 File Offset: 0x00164244
	public NetPackageEntityPrimeDetonator Setup(EntityZombieCop entity)
	{
		this.id = entity.entityId;
		return this;
	}

	// Token: 0x06003621 RID: 13857 RVA: 0x00166053 File Offset: 0x00164253
	public override void read(PooledBinaryReader _br)
	{
		this.id = _br.ReadInt32();
	}

	// Token: 0x06003622 RID: 13858 RVA: 0x00166061 File Offset: 0x00164261
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.id);
	}

	// Token: 0x06003623 RID: 13859 RVA: 0x00166078 File Offset: 0x00164278
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityZombieCop entityZombieCop = _world.GetEntity(this.id) as EntityZombieCop;
		if (entityZombieCop == null)
		{
			Log.Out("Discarding " + base.GetType().Name);
			return;
		}
		entityZombieCop.PrimeDetonator();
	}

	// Token: 0x06003624 RID: 13860 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002C01 RID: 11265
	[PublicizedFrom(EAccessModifier.Private)]
	public int id;
}

using System;
using UnityEngine.Scripting;

// Token: 0x02000740 RID: 1856
[Preserve]
public class NetPackageEntitySetPartActive : NetPackage
{
	// Token: 0x06003642 RID: 13890 RVA: 0x001666F4 File Offset: 0x001648F4
	public NetPackageEntitySetPartActive Setup(Entity entity, string partName, bool active)
	{
		this.id = entity.entityId;
		this.active = active;
		this.partName = partName;
		return this;
	}

	// Token: 0x06003643 RID: 13891 RVA: 0x00166711 File Offset: 0x00164911
	public override void read(PooledBinaryReader _br)
	{
		this.id = _br.ReadInt32();
		this.active = _br.ReadBoolean();
		this.partName = _br.ReadString();
	}

	// Token: 0x06003644 RID: 13892 RVA: 0x00166737 File Offset: 0x00164937
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.id);
		_bw.Write(this.active);
		_bw.Write(this.partName ?? "");
	}

	// Token: 0x06003645 RID: 13893 RVA: 0x00166770 File Offset: 0x00164970
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Entity entity = _world.GetEntity(this.id);
		if (entity == null)
		{
			Log.Out("Discarding " + base.GetType().Name);
			return;
		}
		if (string.IsNullOrEmpty(this.partName))
		{
			Log.Out("Discarding " + base.GetType().Name + " unexpected no part name");
			return;
		}
		entity.SetTransformActive(this.partName, this.active);
	}

	// Token: 0x06003646 RID: 13894 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002C12 RID: 11282
	[PublicizedFrom(EAccessModifier.Private)]
	public int id;

	// Token: 0x04002C13 RID: 11283
	[PublicizedFrom(EAccessModifier.Private)]
	public bool active;

	// Token: 0x04002C14 RID: 11284
	[PublicizedFrom(EAccessModifier.Private)]
	public string partName;
}

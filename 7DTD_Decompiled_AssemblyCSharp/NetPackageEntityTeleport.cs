using System;
using UnityEngine.Scripting;

// Token: 0x02000745 RID: 1861
[Preserve]
public class NetPackageEntityTeleport : NetPackageEntityPosAndRot
{
	// Token: 0x06003664 RID: 13924 RVA: 0x00166D1F File Offset: 0x00164F1F
	public new NetPackageEntityTeleport Setup(Entity _entity)
	{
		base.Setup(_entity);
		return this;
	}

	// Token: 0x06003665 RID: 13925 RVA: 0x00166D2C File Offset: 0x00164F2C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!base.ValidEntityIdForSender(this.entityId, true))
		{
			return;
		}
		Entity entity = _world.GetEntity(this.entityId);
		if (entity != null)
		{
			entity.serverPos = NetEntityDistributionEntry.EncodePos(this.pos);
			entity.SetPosAndRotFromNetwork(this.pos, this.rot, 0);
			entity.SetPosition(this.pos, true);
			entity.SetRotation(this.rot);
			entity.SetLastTickPos(this.pos);
			entity.onGround = this.onGround;
			return;
		}
		Log.Out("Discarding " + base.GetType().Name + " for entity Id=" + this.entityId.ToString());
	}

	// Token: 0x06003666 RID: 13926 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}
}

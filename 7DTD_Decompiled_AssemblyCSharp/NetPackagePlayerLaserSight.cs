using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000774 RID: 1908
[Preserve]
public class NetPackagePlayerLaserSight : NetPackage
{
	// Token: 0x0600377F RID: 14207 RVA: 0x0016B13B File Offset: 0x0016933B
	public NetPackagePlayerLaserSight Setup(int _entityId, bool _laserSightActive, Vector3 _laserSightPosition)
	{
		this.entityId = _entityId;
		this.laserSightActive = _laserSightActive;
		this.laserSightPosition = _laserSightPosition;
		return this;
	}

	// Token: 0x06003780 RID: 14208 RVA: 0x0016B153 File Offset: 0x00169353
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.laserSightActive = _reader.ReadBoolean();
		if (this.laserSightActive)
		{
			this.laserSightPosition = StreamUtils.ReadVector3(_reader);
		}
	}

	// Token: 0x06003781 RID: 14209 RVA: 0x0016B181 File Offset: 0x00169381
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.laserSightActive);
		if (this.laserSightActive)
		{
			StreamUtils.Write(_writer, this.laserSightPosition);
		}
	}

	// Token: 0x06003782 RID: 14210 RVA: 0x0016B1B8 File Offset: 0x001693B8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerLaserSight>().Setup(this.entityId, this.laserSightActive, this.laserSightPosition), false, -1, base.Sender.entityId, -1, null, 192, false);
		}
		EntityAlive entityAlive = (EntityAlive)_world.GetEntity(this.entityId);
		if (entityAlive != null && entityAlive.inventory.holdingItem != null)
		{
			ItemActionRanged.ItemActionDataRanged itemActionDataRanged = entityAlive.inventory.holdingItemData.actionData[0] as ItemActionRanged.ItemActionDataRanged;
			if (itemActionDataRanged != null && itemActionDataRanged.Laser != null)
			{
				itemActionDataRanged.Laser.gameObject.SetActive(this.laserSightActive);
				itemActionDataRanged.Laser.position = this.laserSightPosition - Origin.position;
			}
		}
	}

	// Token: 0x06003783 RID: 14211 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002CF5 RID: 11509
	public int entityId;

	// Token: 0x04002CF6 RID: 11510
	[PublicizedFrom(EAccessModifier.Private)]
	public bool laserSightActive;

	// Token: 0x04002CF7 RID: 11511
	public Vector3 laserSightPosition;
}

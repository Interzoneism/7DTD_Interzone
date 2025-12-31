using System;
using UnityEngine;

// Token: 0x020005A1 RID: 1441
public class MapObjectVehicle : MapObject
{
	// Token: 0x06002E78 RID: 11896 RVA: 0x001318FE File Offset: 0x0012FAFE
	public MapObjectVehicle(Entity _entity) : base(EnumMapObjectType.Entity, Vector3.zero, (long)_entity.entityId, _entity, false)
	{
	}

	// Token: 0x06002E79 RID: 11897 RVA: 0x00131915 File Offset: 0x0012FB15
	public MapObjectVehicle(MapObjectVehicle _other) : base(EnumMapObjectType.Entity, _other.position, (long)_other.entity.entityId, _other.entity, false)
	{
	}

	// Token: 0x06002E7A RID: 11898 RVA: 0x001321E0 File Offset: 0x001303E0
	public override bool IsOnCompass()
	{
		return !(this.entity as EntityVehicle).HasDriver;
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x001321F5 File Offset: 0x001303F5
	public override string GetCompassIcon()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetMapIcon();
		}
		return null;
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x000470CA File Offset: 0x000452CA
	public override Vector3 GetRotation()
	{
		return Vector3.zero;
	}
}

using System;
using UnityEngine;

// Token: 0x020005A2 RID: 1442
public class MapObjectVendingMachine : MapObject
{
	// Token: 0x06002E7D RID: 11901 RVA: 0x0013221A File Offset: 0x0013041A
	public MapObjectVendingMachine(Vector3 _position, Entity _entity) : base(EnumMapObjectType.VendingMachine, _position, (long)_entity.entityId, _entity, false)
	{
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x0013222E File Offset: 0x0013042E
	public override string GetMapIcon()
	{
		return "ui_game_symbol_vending";
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x0013222E File Offset: 0x0013042E
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_vending";
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconEnabled()
	{
		return true;
	}

	// Token: 0x06002E83 RID: 11907 RVA: 0x000B195B File Offset: 0x000AFB5B
	public override Color GetMapIconColor()
	{
		return Color.white;
	}
}

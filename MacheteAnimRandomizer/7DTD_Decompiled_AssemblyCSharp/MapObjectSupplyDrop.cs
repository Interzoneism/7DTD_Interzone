using System;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class MapObjectSupplyDrop : MapObject
{
	// Token: 0x06002E67 RID: 11879 RVA: 0x00132173 File Offset: 0x00130373
	public MapObjectSupplyDrop(Vector3 _position, long entityID) : base(EnumMapObjectType.SupplyDrop, _position, entityID, null, false)
	{
	}

	// Token: 0x06002E68 RID: 11880 RVA: 0x00132181 File Offset: 0x00130381
	public override string GetMapIcon()
	{
		return "ui_game_symbol_airdrop";
	}

	// Token: 0x06002E69 RID: 11881 RVA: 0x00132181 File Offset: 0x00130381
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_airdrop";
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E6B RID: 11883 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E6C RID: 11884 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconEnabled()
	{
		return true;
	}

	// Token: 0x06002E6D RID: 11885 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsShowName()
	{
		return false;
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x00132188 File Offset: 0x00130388
	public override float GetMaxCompassDistance()
	{
		return 4096f;
	}

	// Token: 0x06002E6F RID: 11887 RVA: 0x00132088 File Offset: 0x00130288
	public override Color GetMapIconColor()
	{
		return new Color32(byte.MaxValue, 180, 0, byte.MaxValue);
	}
}

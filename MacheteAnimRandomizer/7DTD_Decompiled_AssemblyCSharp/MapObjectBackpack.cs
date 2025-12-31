using System;
using UnityEngine;

// Token: 0x02000594 RID: 1428
public class MapObjectBackpack : MapObject
{
	// Token: 0x06002DFD RID: 11773 RVA: 0x00131A1F File Offset: 0x0012FC1F
	public MapObjectBackpack(EntityPlayerLocal _epl, Vector3 _position, int _key) : base(EnumMapObjectType.Backpack, _position, (long)_key, null, false)
	{
		this.owningLocalPlayer = _epl;
	}

	// Token: 0x06002DFE RID: 11774 RVA: 0x00131A34 File Offset: 0x0012FC34
	public override string GetMapIcon()
	{
		return "ui_game_symbol_backpack";
	}

	// Token: 0x06002DFF RID: 11775 RVA: 0x00131A34 File Offset: 0x0012FC34
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_backpack";
	}

	// Token: 0x06002E00 RID: 11776 RVA: 0x00131A3B File Offset: 0x0012FC3B
	public override Color GetMapIconColor()
	{
		return Color.cyan;
	}

	// Token: 0x06002E01 RID: 11777 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E02 RID: 11778 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E03 RID: 11779 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconEnabled()
	{
		return true;
	}

	// Token: 0x06002E04 RID: 11780 RVA: 0x00131A42 File Offset: 0x0012FC42
	public override void SetPosition(Vector3 _pos)
	{
		this.position = _pos;
	}

	// Token: 0x06002E05 RID: 11781 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCenterOnLeftBottomCorner()
	{
		return true;
	}

	// Token: 0x06002E06 RID: 11782 RVA: 0x00131A4C File Offset: 0x0012FC4C
	public override Vector3 GetPosition()
	{
		if (this.myBackpack == null && Time.time - this.lastTimeBackpackChecked > 5f)
		{
			this.lastTimeBackpackChecked = Time.time;
			World world = GameManager.Instance.World;
			for (int i = world.Entities.list.Count - 1; i >= 0; i--)
			{
				if (world.Entities.list[i] is EntityBackpack && ((EntityBackpack)world.Entities.list[i]).RefPlayerId == this.owningLocalPlayer.entityId)
				{
					this.myBackpack = world.Entities.list[i];
					break;
				}
			}
		}
		if (this.myBackpack != null && this.myBackpack.IsMarkedForUnload())
		{
			this.myBackpack = null;
		}
		if (!(this.myBackpack != null))
		{
			return base.GetPosition();
		}
		return this.myBackpack.position;
	}

	// Token: 0x04002490 RID: 9360
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal owningLocalPlayer;

	// Token: 0x04002491 RID: 9361
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity myBackpack;

	// Token: 0x04002492 RID: 9362
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastTimeBackpackChecked;
}

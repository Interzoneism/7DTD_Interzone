using System;
using UnityEngine;

// Token: 0x02000592 RID: 1426
public class MapObject
{
	// Token: 0x06002DD3 RID: 11731 RVA: 0x00131470 File Offset: 0x0012F670
	public MapObject(EnumMapObjectType _type, Vector3 _position, long _key, Entity _entity, bool _bSelectable)
	{
		this.type = _type;
		this.position = _position;
		this.key = _key;
		this.bSelectable = _bSelectable;
		this.entity = _entity;
	}

	// Token: 0x06002DD4 RID: 11732 RVA: 0x001314A0 File Offset: 0x0012F6A0
	public MapObject(MapObject _other)
	{
		this.type = _other.type;
		this.position = _other.position;
		this.key = _other.key;
		this.bSelectable = _other.bSelectable;
		this.entity = _other.entity;
	}

	// Token: 0x06002DD5 RID: 11733 RVA: 0x001314EF File Offset: 0x0012F6EF
	public virtual Vector3 GetPosition()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetPosition();
		}
		return this.position;
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x00131519 File Offset: 0x0012F719
	public virtual void SetPosition(Vector3 _pos)
	{
		if (this.type == EnumMapObjectType.Entity)
		{
			throw new Exception("Setting of position not allowed!");
		}
		this.position = _pos;
	}

	// Token: 0x06002DD7 RID: 11735 RVA: 0x00131538 File Offset: 0x0012F738
	public virtual Vector3 GetRotation()
	{
		if (this.type != EnumMapObjectType.Entity || !(this.entity != null))
		{
			return Vector3.zero;
		}
		if (this.entity.AttachedToEntity != null)
		{
			return this.entity.AttachedToEntity.rotation;
		}
		return this.entity.rotation;
	}

	// Token: 0x06002DD8 RID: 11736 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsTracked()
	{
		return true;
	}

	// Token: 0x06002DD9 RID: 11737 RVA: 0x00131590 File Offset: 0x0012F790
	public virtual bool IsMapIconEnabled()
	{
		return this.type != EnumMapObjectType.Entity || !(this.entity != null) || this.entity.IsDrawMapIcon();
	}

	// Token: 0x06002DDA RID: 11738 RVA: 0x001315B5 File Offset: 0x0012F7B5
	public virtual float GetMaxCompassDistance()
	{
		return 1024f;
	}

	// Token: 0x06002DDB RID: 11739 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public virtual float GetMinCompassDistance()
	{
		return 0f;
	}

	// Token: 0x06002DDC RID: 11740 RVA: 0x001315BC File Offset: 0x0012F7BC
	public virtual float GetMaxCompassIconScale()
	{
		return 1.25f;
	}

	// Token: 0x06002DDD RID: 11741 RVA: 0x001315C3 File Offset: 0x0012F7C3
	public virtual float GetMinCompassIconScale()
	{
		return 0.5f;
	}

	// Token: 0x06002DDE RID: 11742 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsCompassIconClamped()
	{
		return false;
	}

	// Token: 0x06002DDF RID: 11743 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool NearbyCompassBlink()
	{
		return false;
	}

	// Token: 0x06002DE0 RID: 11744 RVA: 0x001315CA File Offset: 0x0012F7CA
	public virtual Vector3 GetMapIconScale()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetMapIconScale();
		}
		return Vector3.one;
	}

	// Token: 0x06002DE1 RID: 11745 RVA: 0x001315F3 File Offset: 0x0012F7F3
	public virtual string GetMapIcon()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetMapIcon();
		}
		return "";
	}

	// Token: 0x06002DE2 RID: 11746 RVA: 0x0013161C File Offset: 0x0012F81C
	public virtual string GetCompassIcon()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetCompassIcon();
		}
		return null;
	}

	// Token: 0x06002DE3 RID: 11747 RVA: 0x00131641 File Offset: 0x0012F841
	public virtual string GetCompassUpIcon()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetCompassUpIcon();
		}
		return "";
	}

	// Token: 0x06002DE4 RID: 11748 RVA: 0x0013166A File Offset: 0x0012F86A
	public virtual string GetCompassDownIcon()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetCompassDownIcon();
		}
		return "";
	}

	// Token: 0x06002DE5 RID: 11749 RVA: 0x00131693 File Offset: 0x0012F893
	public virtual bool UseUpDownCompassIcons()
	{
		return this.type == EnumMapObjectType.Entity && this.entity != null && this.entity.GetCompassDownIcon() != null;
	}

	// Token: 0x06002DE6 RID: 11750 RVA: 0x001316BB File Offset: 0x0012F8BB
	public virtual bool IsMapIconBlinking()
	{
		return this.type == EnumMapObjectType.Entity && this.entity != null && this.entity.IsMapIconBlinking();
	}

	// Token: 0x06002DE7 RID: 11751 RVA: 0x001316E0 File Offset: 0x0012F8E0
	public virtual Color GetMapIconColor()
	{
		if (this.type != EnumMapObjectType.Entity || !(this.entity != null))
		{
			return Color.white;
		}
		EntityPlayerLocal primaryPlayer = this.entity.world.GetPrimaryPlayer();
		if (primaryPlayer != null && primaryPlayer.Party != null && primaryPlayer.Party.MemberList.Contains(this.entity as EntityPlayer))
		{
			int num = primaryPlayer.Party.MemberList.IndexOf(this.entity as EntityPlayer);
			return Constants.TrackedFriendColors[num % Constants.TrackedFriendColors.Length];
		}
		return this.entity.GetMapIconColor();
	}

	// Token: 0x06002DE8 RID: 11752 RVA: 0x00131786 File Offset: 0x0012F986
	public virtual bool CanMapIconBeSelected()
	{
		return this.type == EnumMapObjectType.Entity && this.entity != null && this.entity.CanMapIconBeSelected();
	}

	// Token: 0x06002DE9 RID: 11753 RVA: 0x001317AC File Offset: 0x0012F9AC
	public virtual bool IsOnCompass()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			EntityPlayerLocal primaryPlayer = this.entity.world.GetPrimaryPlayer();
			if (primaryPlayer != null && primaryPlayer.Party != null && this.entity != primaryPlayer)
			{
				return primaryPlayer.Party.MemberList.Contains(this.entity as EntityPlayer);
			}
		}
		return false;
	}

	// Token: 0x06002DEA RID: 11754 RVA: 0x0013181C File Offset: 0x0012FA1C
	public virtual int GetLayerForMapIcon()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			return this.entity.GetLayerForMapIcon();
		}
		return 2;
	}

	// Token: 0x06002DEB RID: 11755 RVA: 0x00131844 File Offset: 0x0012FA44
	public virtual string GetName()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity is EntityAlive)
		{
			bool flag = !SingletonMonoBehaviour<ConnectionManager>.Instance.IsSinglePlayer;
			EntityPlayerLocal primaryPlayer = this.entity.world.GetPrimaryPlayer();
			if (!(this.entity is EntityPlayerLocal) && !(this.entity is EntityVehicle))
			{
				return ((EntityAlive)this.entity).EntityName;
			}
			if (primaryPlayer == this.entity && flag)
			{
				return Localization.Get("xuiMapSelfLabel", false);
			}
		}
		return null;
	}

	// Token: 0x06002DEC RID: 11756 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool IsShowName()
	{
		return true;
	}

	// Token: 0x06002DED RID: 11757 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool IsCenterOnLeftBottomCorner()
	{
		return false;
	}

	// Token: 0x06002DEE RID: 11758 RVA: 0x001318D0 File Offset: 0x0012FAD0
	public virtual float GetCompassIconScale(float _distance)
	{
		float t = 1f - _distance / this.GetMaxCompassDistance();
		return Mathf.Lerp(this.GetMinCompassIconScale(), this.GetMaxCompassIconScale(), t);
	}

	// Token: 0x06002DEF RID: 11759 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void RefreshData()
	{
	}

	// Token: 0x0400248A RID: 9354
	public EnumMapObjectType type;

	// Token: 0x0400248B RID: 9355
	public long key;

	// Token: 0x0400248C RID: 9356
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3 position;

	// Token: 0x0400248D RID: 9357
	public bool bSelectable;

	// Token: 0x0400248E RID: 9358
	[PublicizedFrom(EAccessModifier.Protected)]
	public Entity entity;
}

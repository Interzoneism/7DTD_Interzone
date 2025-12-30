using System;
using System.IO;

// Token: 0x02001098 RID: 4248
public class Waypoint
{
	// Token: 0x17000DF5 RID: 3573
	// (get) Token: 0x060085FC RID: 34300 RVA: 0x00366BCD File Offset: 0x00364DCD
	// (set) Token: 0x060085FD RID: 34301 RVA: 0x00366BD8 File Offset: 0x00364DD8
	public int InviterEntityId
	{
		get
		{
			return this.inviterEntityId;
		}
		set
		{
			this.inviterEntityId = value;
			PlatformUserIdentifierAbs primaryId = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(value).PrimaryId;
			this.name.Update(this.name.Text, primaryId);
		}
	}

	// Token: 0x17000DF6 RID: 3574
	// (get) Token: 0x060085FE RID: 34302 RVA: 0x00366C19 File Offset: 0x00364E19
	// (set) Token: 0x060085FF RID: 34303 RVA: 0x00366C21 File Offset: 0x00364E21
	public bool HiddenOnMap
	{
		get
		{
			return this.hiddenOnMap;
		}
		set
		{
			this.hiddenOnMap = value;
			this.navObject.hiddenOnMap = this.hiddenOnMap;
		}
	}

	// Token: 0x06008600 RID: 34304 RVA: 0x00366C3B File Offset: 0x00364E3B
	public Waypoint()
	{
		this.name = new AuthoredText();
	}

	// Token: 0x06008601 RID: 34305 RVA: 0x00366C5C File Offset: 0x00364E5C
	public Waypoint Clone()
	{
		return new Waypoint
		{
			pos = this.pos,
			icon = this.icon,
			name = AuthoredText.Clone(this.name),
			bTracked = this.bTracked,
			ownerId = this.ownerId,
			lastKnownPositionEntityId = this.lastKnownPositionEntityId,
			lastKnownPositionEntityType = this.lastKnownPositionEntityType,
			navObject = this.navObject,
			hiddenOnCompass = this.hiddenOnCompass,
			bIsAutoWaypoint = this.bIsAutoWaypoint,
			bUsingLocalizationId = this.bUsingLocalizationId,
			IsSaved = this.IsSaved,
			inviterEntityId = this.inviterEntityId,
			hiddenOnMap = this.hiddenOnMap
		};
	}

	// Token: 0x06008602 RID: 34306 RVA: 0x00366D1C File Offset: 0x00364F1C
	public void Read(BinaryReader _br, int version = 7)
	{
		this.pos = StreamUtils.ReadVector3i(_br);
		this.icon = _br.ReadString();
		this.name = AuthoredText.FromStream(_br);
		this.bTracked = _br.ReadBoolean();
		if (version > 2)
		{
			this.hiddenOnCompass = _br.ReadBoolean();
		}
		if (version > 1)
		{
			this.ownerId = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			this.lastKnownPositionEntityId = _br.ReadInt32();
		}
		if (version > 3)
		{
			this.bIsAutoWaypoint = _br.ReadBoolean();
			this.bUsingLocalizationId = _br.ReadBoolean();
		}
		if (version > 4)
		{
			this.inviterEntityId = _br.ReadInt32();
		}
		if (version > 5)
		{
			this.hiddenOnMap = _br.ReadBoolean();
		}
		if (version > 6)
		{
			this.lastKnownPositionEntityType = (eLastKnownPositionEntityType)_br.ReadInt32();
			return;
		}
		if (this.lastKnownPositionEntityId > -1)
		{
			this.lastKnownPositionEntityType = eLastKnownPositionEntityType.Vehicle;
		}
	}

	// Token: 0x06008603 RID: 34307 RVA: 0x00366DE4 File Offset: 0x00364FE4
	public void Write(BinaryWriter _bw)
	{
		StreamUtils.Write(_bw, this.pos);
		if (this.icon == null)
		{
			_bw.Write("");
		}
		else
		{
			_bw.Write(this.icon);
		}
		AuthoredText.ToStream(this.name, _bw);
		_bw.Write(this.bTracked);
		_bw.Write(this.hiddenOnCompass);
		this.ownerId.ToStream(_bw, false);
		_bw.Write(this.lastKnownPositionEntityId);
		_bw.Write(this.bIsAutoWaypoint);
		_bw.Write(this.bUsingLocalizationId);
		_bw.Write(this.inviterEntityId);
		_bw.Write(this.hiddenOnMap);
		_bw.Write((int)this.lastKnownPositionEntityType);
	}

	// Token: 0x06008604 RID: 34308 RVA: 0x00366E98 File Offset: 0x00365098
	public override bool Equals(object obj)
	{
		Waypoint waypoint = obj as Waypoint;
		return waypoint != null && (waypoint.pos.Equals(this.pos) && waypoint.icon == this.icon && waypoint.name.Text == this.name.Text && object.Equals(waypoint.ownerId, this.ownerId)) && waypoint.lastKnownPositionEntityId == this.lastKnownPositionEntityId;
	}

	// Token: 0x06008605 RID: 34309 RVA: 0x00366F18 File Offset: 0x00365118
	public override int GetHashCode()
	{
		int num = (((17 * 31 + this.pos.GetHashCode()) * 31 + this.icon.GetHashCode()) * 31 + this.name.Text.GetHashCode()) * 31;
		PlatformUserIdentifierAbs platformUserIdentifierAbs = this.ownerId;
		return (num + ((platformUserIdentifierAbs != null) ? platformUserIdentifierAbs.GetHashCode() : 0)) * 31 + this.lastKnownPositionEntityId.GetHashCode();
	}

	// Token: 0x06008606 RID: 34310 RVA: 0x00366F84 File Offset: 0x00365184
	public bool CanBeViewedBy(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.lastKnownPositionEntityId == -1 || (_userIdentifier != null && _userIdentifier.Equals(this.ownerId));
	}

	// Token: 0x06008607 RID: 34311 RVA: 0x00366FA4 File Offset: 0x003651A4
	public override string ToString()
	{
		string[] array = new string[6];
		array[0] = "Waypoint name:";
		int num = 1;
		AuthoredText authoredText = this.name;
		array[num] = ((authoredText != null) ? authoredText.ToString() : null);
		array[2] = " icon:";
		array[3] = this.icon;
		array[4] = " Entity ID:";
		array[5] = this.lastKnownPositionEntityId.ToString();
		return string.Concat(array);
	}

	// Token: 0x0400681D RID: 26653
	public Vector3i pos;

	// Token: 0x0400681E RID: 26654
	public string icon;

	// Token: 0x0400681F RID: 26655
	public AuthoredText name;

	// Token: 0x04006820 RID: 26656
	public bool bTracked;

	// Token: 0x04006821 RID: 26657
	public PlatformUserIdentifierAbs ownerId;

	// Token: 0x04006822 RID: 26658
	public int lastKnownPositionEntityId = -1;

	// Token: 0x04006823 RID: 26659
	public eLastKnownPositionEntityType lastKnownPositionEntityType;

	// Token: 0x04006824 RID: 26660
	public long MapObjectKey;

	// Token: 0x04006825 RID: 26661
	public bool hiddenOnCompass;

	// Token: 0x04006826 RID: 26662
	public NavObject navObject;

	// Token: 0x04006827 RID: 26663
	public bool bIsAutoWaypoint;

	// Token: 0x04006828 RID: 26664
	public bool bUsingLocalizationId;

	// Token: 0x04006829 RID: 26665
	public bool IsSaved = true;

	// Token: 0x0400682A RID: 26666
	[PublicizedFrom(EAccessModifier.Private)]
	public int inviterEntityId;

	// Token: 0x0400682B RID: 26667
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hiddenOnMap;
}

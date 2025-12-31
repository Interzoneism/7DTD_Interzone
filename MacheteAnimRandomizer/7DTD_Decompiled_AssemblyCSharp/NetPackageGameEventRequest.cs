using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200074B RID: 1867
[Preserve]
public class NetPackageGameEventRequest : NetPackage
{
	// Token: 0x06003683 RID: 13955 RVA: 0x00167508 File Offset: 0x00165708
	public NetPackageGameEventRequest Setup(string _event, int _entityId, bool _isTwitchEvent, Vector3 _targetPos, string _extraData = "", string _tag = "", bool _crateShare = false, bool _allowRefunds = true, string _sequenceLink = "")
	{
		this.eventName = _event;
		this.entityID = _entityId;
		this.extraData = _extraData;
		this.tag = _tag;
		this.isTwitchEvent = _isTwitchEvent;
		this.crateShare = _crateShare;
		this.targetPos = _targetPos;
		this.allowRefunds = _allowRefunds;
		this.sequenceLink = _sequenceLink;
		return this;
	}

	// Token: 0x06003684 RID: 13956 RVA: 0x0016755C File Offset: 0x0016575C
	public override void read(PooledBinaryReader _br)
	{
		this.eventName = _br.ReadString();
		this.entityID = _br.ReadInt32();
		this.extraData = _br.ReadString();
		this.tag = _br.ReadString();
		this.isTwitchEvent = _br.ReadBoolean();
		this.crateShare = _br.ReadBoolean();
		this.allowRefunds = _br.ReadBoolean();
		this.sequenceLink = _br.ReadString();
		this.targetPos = StreamUtils.ReadVector3(_br);
	}

	// Token: 0x06003685 RID: 13957 RVA: 0x001675D8 File Offset: 0x001657D8
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.eventName);
		_bw.Write(this.entityID);
		_bw.Write(this.extraData);
		_bw.Write(this.tag);
		_bw.Write(this.isTwitchEvent);
		_bw.Write(this.crateShare);
		_bw.Write(this.allowRefunds);
		_bw.Write(this.sequenceLink);
		StreamUtils.Write(_bw, this.targetPos);
	}

	// Token: 0x06003686 RID: 13958 RVA: 0x00167658 File Offset: 0x00165858
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(base.Sender.entityId) as EntityPlayer;
		Entity entity = GameManager.Instance.World.GetEntity(this.entityID);
		EntityPlayer entityPlayer2 = entity as EntityPlayer;
		if (entityPlayer2 == null || entityPlayer == entityPlayer2 || (entityPlayer.Party != null && entityPlayer.Party.ContainsMember(entityPlayer2)))
		{
			if (GameEventManager.Current.HandleAction(this.eventName, entityPlayer, entity, this.isTwitchEvent, this.targetPos, this.extraData, this.tag, this.crateShare, this.allowRefunds, this.sequenceLink, null))
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.eventName, entity ? entity.entityId : -1, this.extraData, this.tag, NetPackageGameEventResponse.ResponseTypes.Approved, -1, -1, false), false, base.Sender.entityId, -1, -1, null, 192, false);
				if (this.isTwitchEvent && entityPlayer.Party != null)
				{
					for (int i = 0; i < entityPlayer.Party.MemberList.Count; i++)
					{
						EntityPlayer entityPlayer3 = entityPlayer.Party.MemberList[i];
						if (entityPlayer3 != entityPlayer && entityPlayer3.TwitchEnabled)
						{
							if (entityPlayer3 is EntityPlayerLocal)
							{
								GameEventManager.Current.HandleTwitchPartyGameEventApproved(this.eventName, entity ? entity.entityId : -1, this.extraData, this.tag);
							}
							else
							{
								SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.eventName, entity ? entity.entityId : -1, this.extraData, this.tag, NetPackageGameEventResponse.ResponseTypes.TwitchPartyActionApproved, -1, -1, false), false, entityPlayer3.entityId, -1, -1, null, 192, false);
							}
						}
					}
					return;
				}
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(this.eventName, entity ? entity.entityId : -1, this.extraData, this.tag, NetPackageGameEventResponse.ResponseTypes.Denied, -1, -1, false), false, base.Sender.entityId, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x17000585 RID: 1413
	// (get) Token: 0x06003687 RID: 13959 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003688 RID: 13960 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int GetLength()
	{
		return 30;
	}

	// Token: 0x04002C3B RID: 11323
	[PublicizedFrom(EAccessModifier.Private)]
	public string eventName;

	// Token: 0x04002C3C RID: 11324
	[PublicizedFrom(EAccessModifier.Private)]
	public string extraData;

	// Token: 0x04002C3D RID: 11325
	[PublicizedFrom(EAccessModifier.Private)]
	public string tag;

	// Token: 0x04002C3E RID: 11326
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityID = -1;

	// Token: 0x04002C3F RID: 11327
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTwitchEvent;

	// Token: 0x04002C40 RID: 11328
	[PublicizedFrom(EAccessModifier.Private)]
	public bool crateShare;

	// Token: 0x04002C41 RID: 11329
	[PublicizedFrom(EAccessModifier.Private)]
	public bool allowRefunds = true;

	// Token: 0x04002C42 RID: 11330
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 targetPos = Vector3.zero;

	// Token: 0x04002C43 RID: 11331
	[PublicizedFrom(EAccessModifier.Private)]
	public string sequenceLink = "";
}

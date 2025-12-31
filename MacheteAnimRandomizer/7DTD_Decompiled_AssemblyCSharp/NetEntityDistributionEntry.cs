using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class NetEntityDistributionEntry
{
	// Token: 0x0600343E RID: 13374 RVA: 0x0015F2CC File Offset: 0x0015D4CC
	public NetEntityDistributionEntry(Entity _e, int _d, int _ticks, bool _isMotionSent)
	{
		this.updateCounter = 0;
		this.sendFullUpdateAfterTicks = 0;
		this.trackedPlayers = new HashSet<EntityPlayer>();
		this.trackedEntity = _e;
		this.trackingDistanceThreshold = Math.Min(46340, _d);
		this.updatTickCounter = _ticks;
		this.shouldSendMotionUpdates = _isMotionSent;
		this.encodedPos = NetEntityDistributionEntry.EncodePos(_e.position);
		this.encodedRot = NetEntityDistributionEntry.EncodeRot(_e.rotation);
		this.encodedOnGround = _e.onGround;
	}

	// Token: 0x0600343F RID: 13375 RVA: 0x0015F354 File Offset: 0x0015D554
	public override bool Equals(object _other)
	{
		return _other is NetEntityDistributionEntry && ((NetEntityDistributionEntry)_other).trackedEntity.entityId == this.trackedEntity.entityId;
	}

	// Token: 0x06003440 RID: 13376 RVA: 0x0015F37D File Offset: 0x0015D57D
	public override int GetHashCode()
	{
		return this.trackedEntity.entityId;
	}

	// Token: 0x06003441 RID: 13377 RVA: 0x0015F38C File Offset: 0x0015D58C
	public void SendToPlayers(NetPackage _packet, int _excludePlayer, bool _inRangeOnly = false, int _range = 192)
	{
		foreach (EntityPlayer entityPlayer in this.trackedPlayers)
		{
			if (entityPlayer.entityId != _excludePlayer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(_packet, false, entityPlayer.entityId, -1, _inRangeOnly ? this.trackedEntity.entityId : -1, null, _range, false);
			}
		}
	}

	// Token: 0x06003442 RID: 13378 RVA: 0x0015F414 File Offset: 0x0015D614
	public void sendPacketToTrackedPlayersAndTrackedEntity(NetPackage _packet, int _excludePlayer, bool _inRangeOnly = false)
	{
		this.SendToPlayers(_packet, _excludePlayer, _inRangeOnly, 192);
		if (this.trackedEntity is EntityPlayer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(_packet, false, this.trackedEntity.entityId, -1, _inRangeOnly ? this.trackedEntity.entityId : -1, null, 192, false);
		}
	}

	// Token: 0x06003443 RID: 13379 RVA: 0x0015F474 File Offset: 0x0015D674
	public void SendDestroyEntityToPlayers()
	{
		this.SendToPlayers(NetPackageManager.GetPackage<NetPackageEntityRemove>().Setup(this.trackedEntity.entityId, EnumRemoveEntityReason.Killed), -1, false, 192);
	}

	// Token: 0x06003444 RID: 13380 RVA: 0x0015F499 File Offset: 0x0015D699
	public void SendUnloadEntityToPlayers()
	{
		this.SendToPlayers(NetPackageManager.GetPackage<NetPackageEntityRemove>().Setup(this.trackedEntity.entityId, EnumRemoveEntityReason.Unloaded), -1, false, 192);
	}

	// Token: 0x06003445 RID: 13381 RVA: 0x0015F4BE File Offset: 0x0015D6BE
	public void Remove(EntityPlayer _e)
	{
		if (this.trackedPlayers.Contains(_e))
		{
			this.trackedPlayers.Remove(_e);
		}
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x0015F4DC File Offset: 0x0015D6DC
	public void updatePlayerEntity(EntityPlayer _ep)
	{
		if (_ep == this.trackedEntity)
		{
			return;
		}
		float num = _ep.position.x - (float)(this.encodedPos.x / 32);
		float num2 = _ep.position.z - (float)(this.encodedPos.z / 32);
		if (num * num + num2 * num2 <= (float)(this.trackingDistanceThreshold * this.trackingDistanceThreshold))
		{
			if (!this.trackedPlayers.Contains(_ep))
			{
				this.trackedPlayers.Add(_ep);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(this.getSpawnPacket(), false, _ep.entityId, -1, -1, null, 192, false);
				EntityAlive entityAlive = this.trackedEntity as EntityAlive;
				if (entityAlive)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityAliveFlags>().Setup(entityAlive), false, _ep.entityId, -1, -1, null, 192, false);
					if (entityAlive is EntityPlayer)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(entityAlive), false, _ep.entityId, -1, -1, null, 192, false);
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackagePlayerTwitchStats>().Setup(entityAlive), false, _ep.entityId, -1, -1, null, 192, false);
					}
				}
				EModelBase emodel = this.trackedEntity.emodel;
				if (emodel != null)
				{
					AvatarController avatarController = emodel.avatarController;
					if (avatarController != null)
					{
						avatarController.SyncAnimParameters(_ep.entityId);
					}
				}
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntitySpeeds>().Setup(this.trackedEntity), false, _ep.entityId, -1, -1, null, 192, false);
				if (this.shouldSendMotionUpdates)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityVelocity>().Setup(this.trackedEntity.entityId, this.trackedEntity.motion, false), false, _ep.entityId, -1, -1, null, 192, false);
					return;
				}
			}
		}
		else if (this.trackedPlayers.Contains(_ep))
		{
			this.trackedPlayers.Remove(_ep);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityRemove>().Setup(this.trackedEntity.entityId, EnumRemoveEntityReason.Unloaded), false, _ep.entityId, -1, -1, null, 192, false);
		}
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x0015F734 File Offset: 0x0015D934
	public void updatePlayerEntities(List<EntityPlayer> _list)
	{
		for (int i = 0; i < _list.Count; i++)
		{
			EntityPlayer ep = _list[i];
			this.updatePlayerEntity(ep);
		}
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x0015F761 File Offset: 0x0015D961
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackage getSpawnPacket()
	{
		return NetPackageManager.GetPackage<NetPackageEntitySpawn>().Setup(new EntityCreationData(this.trackedEntity, true));
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x0015F77C File Offset: 0x0015D97C
	public void updatePlayerList(List<EntityPlayer> _playerList)
	{
		if (!this.firstUpdateDone || this.trackedEntity.GetDistanceSq(this.lastTrackedEntityPos) > 16f)
		{
			this.lastTrackedEntityPos = this.trackedEntity.position;
			this.firstUpdateDone = true;
			this.updatePlayerEntities(_playerList);
		}
		if (this.trackedEntity.usePhysicsMaster)
		{
			if (this.trackedEntity.isPhysicsMaster)
			{
				int num = this.updateCounter;
				this.updateCounter = num + 1;
				if (num % this.updatTickCounter == 0)
				{
					NetPackageEntityPhysics netPackageEntityPhysics = this.trackedEntity.PhysicsMasterSetupBroadcast();
					if (netPackageEntityPhysics != null)
					{
						this.SendToPlayers(netPackageEntityPhysics, -1, false, 192);
					}
				}
			}
			return;
		}
		this.sendFullUpdateAfterTicks++;
		bool flag = this.priorityLevel == 0 || this.trackedEntity.IsAirBorne();
		int updateSteps = (this.priorityLevel == 0) ? 1 : 3;
		this.updateCounter++;
		if (!flag)
		{
			switch (this.priorityLevel)
			{
			case 1:
				flag = (this.updateCounter % this.updatTickCounter == 0);
				updateSteps = 3;
				break;
			case 2:
				flag = (this.updateCounter % 6 == 0);
				updateSteps = 6;
				break;
			case 3:
				flag = (this.updateCounter % 10 == 0);
				updateSteps = 10;
				break;
			}
		}
		if (flag)
		{
			Vector3i one = NetEntityDistributionEntry.EncodePos(this.trackedEntity.position);
			Vector3i vector3i = one - this.encodedPos;
			bool flag2 = Utils.FastAbs((float)vector3i.x) >= 2f || Utils.FastAbs((float)vector3i.y) >= 2f || Utils.FastAbs((float)vector3i.z) >= 2f || this.encodedOnGround != this.trackedEntity.onGround;
			Vector3i vector3i2 = NetEntityDistributionEntry.EncodeRot(this.trackedEntity.rotation);
			Vector3i vector3i3 = vector3i2 - this.encodedRot;
			bool flag3 = Utils.FastAbs((float)vector3i3.x) >= 2f || Utils.FastAbs((float)vector3i3.y) >= 2f || Utils.FastAbs((float)vector3i3.z) >= 2f;
			NetPackage netPackage = null;
			bool inRangeOnly = false;
			if (this.trackedEntity.IsMovementReplicated)
			{
				if (this.updatTickCounter == 1)
				{
					this.sendFullUpdateAfterTicks = int.MaxValue;
					inRangeOnly = true;
				}
				if (vector3i.x < -256 || vector3i.x >= 256 || vector3i.y < -256 || vector3i.y >= 256 || vector3i.z < -256 || vector3i.z >= 256)
				{
					this.sendFullUpdateAfterTicks = 0;
					netPackage = NetPackageManager.GetPackage<NetPackageEntityTeleport>().Setup(this.trackedEntity);
				}
				else if (vector3i.x < -128 || vector3i.x >= 128 || vector3i.y < -128 || vector3i.y >= 128 || vector3i.z < -128 || vector3i.z >= 128 || this.sendFullUpdateAfterTicks > 100)
				{
					this.sendFullUpdateAfterTicks = 0;
					netPackage = NetPackageManager.GetPackage<NetPackageEntityPosAndRot>().Setup(this.trackedEntity);
				}
				else if (flag2 && flag3)
				{
					netPackage = NetPackageManager.GetPackage<NetPackageEntityRelPosAndRot>().Setup(this.trackedEntity.entityId, vector3i, vector3i2, this.trackedEntity.qrotation, this.trackedEntity.onGround, this.trackedEntity.IsQRotationUsed(), updateSteps);
					inRangeOnly = true;
				}
				else if (flag2)
				{
					netPackage = NetPackageManager.GetPackage<NetPackageEntityRelPosAndRot>().Setup(this.trackedEntity.entityId, vector3i, vector3i2, this.trackedEntity.qrotation, this.trackedEntity.onGround, this.trackedEntity.IsQRotationUsed(), updateSteps);
					inRangeOnly = true;
				}
				else if (flag3)
				{
					netPackage = NetPackageManager.GetPackage<NetPackageEntityRotation>().Setup(this.trackedEntity.entityId, vector3i2, this.trackedEntity.qrotation, this.trackedEntity.IsQRotationUsed());
					inRangeOnly = true;
				}
			}
			if (this.shouldSendMotionUpdates)
			{
				float sqrMagnitude = (this.trackedEntity.motion - this.lastTrackedEntityMotion).sqrMagnitude;
				if (sqrMagnitude > 0.040000003f || (sqrMagnitude > 0f && this.trackedEntity.motion.Equals(Vector3.zero)))
				{
					this.lastTrackedEntityMotion = this.trackedEntity.motion;
					this.SendToPlayers(NetPackageManager.GetPackage<NetPackageEntityVelocity>().Setup(this.trackedEntity.entityId, this.lastTrackedEntityMotion, false), -1, false, 192);
				}
			}
			if (netPackage != null)
			{
				this.SendToPlayers(netPackage, -1, inRangeOnly, this.trackingDistanceThreshold);
			}
			EntityAlive entityAlive = this.trackedEntity as EntityAlive;
			if (entityAlive != null && entityAlive.bEntityAliveFlagsChanged)
			{
				this.SendToPlayers(NetPackageManager.GetPackage<NetPackageEntityAliveFlags>().Setup(entityAlive), this.trackedEntity.entityId, false, 192);
				entityAlive.bEntityAliveFlagsChanged = false;
			}
			EntityPlayer entityPlayer = this.trackedEntity as EntityPlayer;
			if (entityPlayer != null && entityPlayer.bPlayerStatsChanged)
			{
				this.SendToPlayers(NetPackageManager.GetPackage<NetPackagePlayerStats>().Setup(entityAlive), this.trackedEntity.entityId, false, 192);
				entityAlive.bPlayerStatsChanged = false;
			}
			if (entityPlayer != null && entityPlayer.bPlayerTwitchChanged)
			{
				this.SendToPlayers(NetPackageManager.GetPackage<NetPackagePlayerTwitchStats>().Setup(entityAlive), this.trackedEntity.entityId, false, 192);
				entityAlive.bPlayerTwitchChanged = false;
			}
			if (flag2)
			{
				this.encodedPos = one;
				this.encodedOnGround = this.trackedEntity.onGround;
			}
			if (flag3)
			{
				this.encodedRot = vector3i2;
			}
		}
		this.trackedEntity.SetAirBorne(false);
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x0015FD16 File Offset: 0x0015DF16
	public void SendFullUpdateNextTick()
	{
		this.sendFullUpdateAfterTicks = 100;
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x0015FD20 File Offset: 0x0015DF20
	public static Vector3i EncodePos(Vector3 _pos)
	{
		return new Vector3i(_pos.x * 32f + 0.5f, _pos.y * 32f + 0.5f, _pos.z * 32f + 0.5f);
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x0015FD5D File Offset: 0x0015DF5D
	public static Vector3i EncodeRot(Vector3 _rot)
	{
		return new Vector3i(_rot * 256f / 360f);
	}

	// Token: 0x04002AC4 RID: 10948
	[PublicizedFrom(EAccessModifier.Private)]
	public const int MaxTrackingDistance = 46340;

	// Token: 0x04002AC5 RID: 10949
	public Entity trackedEntity;

	// Token: 0x04002AC6 RID: 10950
	public int updatTickCounter;

	// Token: 0x04002AC7 RID: 10951
	public Vector3i encodedPos;

	// Token: 0x04002AC8 RID: 10952
	public Vector3i encodedRot;

	// Token: 0x04002AC9 RID: 10953
	public bool encodedOnGround;

	// Token: 0x04002ACA RID: 10954
	public Vector3 lastTrackedEntityMotion;

	// Token: 0x04002ACB RID: 10955
	public int updateCounter;

	// Token: 0x04002ACC RID: 10956
	public HashSet<EntityPlayer> trackedPlayers;

	// Token: 0x04002ACD RID: 10957
	[PublicizedFrom(EAccessModifier.Private)]
	public int trackingDistanceThreshold;

	// Token: 0x04002ACE RID: 10958
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 lastTrackedEntityPos;

	// Token: 0x04002ACF RID: 10959
	[PublicizedFrom(EAccessModifier.Private)]
	public bool firstUpdateDone;

	// Token: 0x04002AD0 RID: 10960
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shouldSendMotionUpdates;

	// Token: 0x04002AD1 RID: 10961
	[PublicizedFrom(EAccessModifier.Private)]
	public int sendFullUpdateAfterTicks;

	// Token: 0x04002AD2 RID: 10962
	public const int cFullUpdateAfterTicks = 100;

	// Token: 0x04002AD3 RID: 10963
	public int priorityLevel = 1;
}

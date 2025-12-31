using System;
using System.Globalization;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200050E RID: 1294
[Preserve]
public class ItemActionConnectPower : ItemAction
{
	// Token: 0x06002A2C RID: 10796 RVA: 0x00112EF3 File Offset: 0x001110F3
	public override ItemActionData CreateModifierData(ItemInventoryData _invData, int _indexInEntityOfAction)
	{
		return new ItemActionConnectPower.ConnectPowerData(_invData, _indexInEntityOfAction);
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x00112EFC File Offset: 0x001110FC
	public override void ReadFrom(DynamicProperties _props)
	{
		base.ReadFrom(_props);
		if (_props.Values.ContainsKey("WireOffset"))
		{
			this.wireOffset = StringParsers.ParseVector3(_props.Values["WireOffset"], 0, -1);
		}
		if (_props.Values.ContainsKey("MaxWireLength"))
		{
			this.maxWireLength = StringParsers.ParseSInt32(_props.Values["MaxWireLength"], 0, -1, NumberStyles.Integer);
		}
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x00112F70 File Offset: 0x00111170
	public override void StopHolding(ItemActionData _data)
	{
		base.StopHolding(_data);
		ItemActionConnectPower.ConnectPowerData connectPowerData = (ItemActionConnectPower.ConnectPowerData)_data;
		connectPowerData.HasStartPoint = false;
		if (connectPowerData.wireNode != null)
		{
			WireManager.Instance.RemoveActiveWire(connectPowerData.wireNode);
			UnityEngine.Object.Destroy(connectPowerData.wireNode.gameObject);
			connectPowerData.wireNode = null;
		}
		if (connectPowerData.invData.world.GetTileEntity(0, connectPowerData.startPoint) is TileEntityPowered)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, connectPowerData.invData.holdingEntity.entityId), false, -1, -1, -1, null, 192, false);
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, connectPowerData.invData.holdingEntity.entityId), false);
			}
		}
		if (!(_data.invData.holdingEntity is EntityPlayerLocal))
		{
			return;
		}
		((ItemActionConnectPower.ConnectPowerData)_data).playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.PowerInfo, null, true);
		WireManager.Instance.ToggleAllWirePulse(false);
	}

	// Token: 0x06002A2F RID: 10799 RVA: 0x00113094 File Offset: 0x00111294
	public override void StartHolding(ItemActionData _data)
	{
		base.StartHolding(_data);
		if (!(_data.invData.holdingEntity is EntityPlayerLocal))
		{
			return;
		}
		((ItemActionConnectPower.ConnectPowerData)_data).playerUI = LocalPlayerUI.GetUIForPlayer(_data.invData.holdingEntity as EntityPlayerLocal);
		WireManager.Instance.ToggleAllWirePulse(true);
	}

	// Token: 0x06002A30 RID: 10800 RVA: 0x001130E6 File Offset: 0x001112E6
	public override void ExecuteAction(ItemActionData _actionData, bool _bReleased)
	{
		if (!_bReleased)
		{
			return;
		}
		if (Time.time - _actionData.lastUseTime < this.Delay)
		{
			return;
		}
		_actionData.lastUseTime = Time.time;
		((ItemActionConnectPower.ConnectPowerData)_actionData).StartLink = true;
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x00113118 File Offset: 0x00111318
	public override bool IsActionRunning(ItemActionData _actionData)
	{
		ItemActionConnectPower.ConnectPowerData connectPowerData = (ItemActionConnectPower.ConnectPowerData)_actionData;
		return connectPowerData.StartLink && Time.time - connectPowerData.lastUseTime < 2f * AnimationDelayData.AnimationDelay[connectPowerData.invData.item.HoldType.Value].RayCast;
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x00113170 File Offset: 0x00111370
	public override void OnHoldingUpdate(ItemActionData _actionData)
	{
		ItemActionConnectPower.ConnectPowerData connectPowerData = (ItemActionConnectPower.ConnectPowerData)_actionData;
		Vector3i blockPos = _actionData.invData.hitInfo.hit.blockPos;
		bool flag = true;
		if (connectPowerData.invData.holdingEntity is EntityPlayerLocal && connectPowerData.playerUI == null)
		{
			connectPowerData.playerUI = LocalPlayerUI.GetUIForPlayer(connectPowerData.invData.holdingEntity as EntityPlayerLocal);
		}
		if (connectPowerData.playerUI != null && !connectPowerData.invData.world.CanPlaceBlockAt(blockPos, connectPowerData.invData.world.gameManager.GetPersistentLocalPlayer(), false))
		{
			connectPowerData.isFriendly = false;
			connectPowerData.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.PowerInfo, null, true);
			return;
		}
		connectPowerData.isFriendly = true;
		if (_actionData.invData.hitInfo.bHitValid)
		{
			int num = (int)(Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance);
			if (_actionData.invData.hitInfo.hit.distanceSq <= (float)num)
			{
				BlockValue block = _actionData.invData.world.GetBlock(blockPos);
				BlockPowered blockPowered = block.Block as BlockPowered;
				if (blockPowered != null)
				{
					if (connectPowerData.playerUI != null)
					{
						Color value = Color.grey;
						int num2 = blockPowered.RequiredPower;
						if (blockPowered.isMultiBlock && block.ischild)
						{
							connectPowerData.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.PowerInfo, null, true);
							return;
						}
						Vector3i vector3i = blockPos;
						ChunkCluster chunkCluster = _actionData.invData.world.ChunkClusters[_actionData.invData.hitInfo.hit.clrIdx];
						if (chunkCluster != null)
						{
							Chunk chunk = (Chunk)chunkCluster.GetChunkSync(World.toChunkXZ(vector3i.x), vector3i.y, World.toChunkXZ(vector3i.z));
							if (chunk != null)
							{
								TileEntityPowered tileEntityPowered = chunk.GetTileEntity(World.toBlock(vector3i)) as TileEntityPowered;
								if (tileEntityPowered != null)
								{
									value = (tileEntityPowered.IsPowered ? Color.yellow : Color.grey);
									num2 = tileEntityPowered.PowerUsed;
								}
								else
								{
									value = Color.grey;
								}
							}
						}
						connectPowerData.playerUI.nguiWindowManager.SetLabel(EnumNGUIWindow.PowerInfo, string.Format("{0}W", num2), new Color?(value), true);
					}
					flag = false;
				}
			}
		}
		if (flag && connectPowerData.playerUI != null)
		{
			connectPowerData.playerUI.nguiWindowManager.SetLabelText(EnumNGUIWindow.PowerInfo, null, true);
		}
		if (connectPowerData.HasStartPoint)
		{
			if (connectPowerData.wireNode == null)
			{
				return;
			}
			float num3 = Vector3.Distance(connectPowerData.startPoint.ToVector3(), _actionData.invData.holdingEntity.position);
			if (num3 < (float)(this.maxWireLength - 5))
			{
				connectPowerData.inRange = true;
				connectPowerData.wireNode.wireColor = new Color(0f, 0f, 0f, 0f);
			}
			if (num3 > (float)(this.maxWireLength - 5))
			{
				connectPowerData.inRange = false;
				connectPowerData.wireNode.wireColor = Color.red;
			}
			if (num3 > (float)this.maxWireLength)
			{
				connectPowerData.HasStartPoint = false;
				if (connectPowerData.wireNode != null)
				{
					WireManager.Instance.RemoveActiveWire(connectPowerData.wireNode);
					UnityEngine.Object.Destroy(connectPowerData.wireNode.gameObject);
					connectPowerData.wireNode = null;
				}
				Chunk chunk2 = connectPowerData.invData.world.GetChunkFromWorldPos(connectPowerData.startPoint) as Chunk;
				if (chunk2 == null)
				{
					return;
				}
				if (connectPowerData.invData.world.GetTileEntity(chunk2.ClrIdx, connectPowerData.startPoint) is TileEntityPowered)
				{
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, _actionData.invData.holdingEntity.entityId), false, -1, -1, -1, null, 192, false);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, _actionData.invData.holdingEntity.entityId), false);
					}
				}
				_actionData.invData.holdingEntity.RightArmAnimationUse = true;
				connectPowerData.invData.holdingEntity.PlayOneShot("ui_denied", false, false, false, null);
			}
		}
		if (!connectPowerData.StartLink || Time.time - connectPowerData.lastUseTime < AnimationDelayData.AnimationDelay[connectPowerData.invData.item.HoldType.Value].RayCast)
		{
			return;
		}
		connectPowerData.StartLink = false;
		ItemActionConnectPower.ConnectPowerData connectPowerData2 = (ItemActionConnectPower.ConnectPowerData)_actionData;
		ItemInventoryData invData = _actionData.invData;
		Vector3i lastBlockPos = invData.hitInfo.lastBlockPos;
		if (!invData.hitInfo.bHitValid || invData.hitInfo.tag.StartsWith("E_"))
		{
			connectPowerData2.HasStartPoint = false;
			return;
		}
		if (connectPowerData.invData.itemValue.MaxUseTimes > 0 && connectPowerData.invData.itemValue.UseTimes >= (float)connectPowerData.invData.itemValue.MaxUseTimes)
		{
			EntityPlayerLocal player = _actionData.invData.holdingEntity as EntityPlayerLocal;
			if (this.item.Properties.Values.ContainsKey(ItemClass.PropSoundJammed))
			{
				Manager.PlayInsidePlayerHead(this.item.Properties.Values[ItemClass.PropSoundJammed], -1, 0f, false, false);
			}
			GameManager.ShowTooltip(player, "ttItemNeedsRepair", false, false, 0f);
			return;
		}
		if (connectPowerData.invData.itemValue.MaxUseTimes > 0)
		{
			_actionData.invData.itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, _actionData.invData.itemValue, 1f, invData.holdingEntity, null, (_actionData.invData.itemValue.ItemClass != null) ? _actionData.invData.itemValue.ItemClass.ItemTags : FastTags<TagGroup.Global>.none, true, true, true, true, true, 1, true, false);
			base.HandleItemBreak(_actionData);
		}
		if (connectPowerData2.HasStartPoint)
		{
			if (connectPowerData2.startPoint == invData.hitInfo.hit.blockPos || !connectPowerData2.inRange)
			{
				return;
			}
			if (Vector3.Distance(connectPowerData.startPoint.ToVector3(), invData.hitInfo.hit.blockPos.ToVector3()) > (float)this.maxWireLength)
			{
				return;
			}
			TileEntityPowered poweredBlock = this.GetPoweredBlock(invData);
			if (poweredBlock != null)
			{
				TileEntityPowered poweredBlock2 = this.GetPoweredBlock(connectPowerData2.startPoint);
				if (poweredBlock2 != null)
				{
					if (!poweredBlock.CanHaveParent(poweredBlock2))
					{
						GameManager.ShowTooltip(_actionData.invData.holdingEntity as EntityPlayerLocal, Localization.Get("ttCantHaveParent", false), false, false, 0f);
						invData.holdingEntity.PlayOneShot("ui_denied", false, false, false, null);
						return;
					}
					if (poweredBlock2.ChildCount > 8)
					{
						GameManager.ShowTooltip(_actionData.invData.holdingEntity as EntityPlayerLocal, Localization.Get("ttWireLimit", false), false, false, 0f);
						invData.holdingEntity.PlayOneShot("ui_denied", false, false, false, null);
						return;
					}
					poweredBlock.SetParentWithWireTool(poweredBlock2, invData.holdingEntity.entityId);
					_actionData.invData.holdingEntity.RightArmAnimationUse = true;
					connectPowerData2.HasStartPoint = false;
					if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, _actionData.invData.holdingEntity.entityId), false, -1, -1, -1, null, 192, false);
					}
					else
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, _actionData.invData.holdingEntity.entityId), false);
					}
					EntityAlive holdingEntity = _actionData.invData.holdingEntity;
					string name = "wire_tool_" + (poweredBlock2.IsPowered ? "sparks" : "dust");
					Transform handTransform = this.GetHandTransform(holdingEntity);
					GameManager.Instance.SpawnParticleEffectServer(new ParticleEffect(name, handTransform.position + Origin.position, handTransform.rotation, holdingEntity.GetLightBrightness(), Color.white), invData.holdingEntity.entityId, false, false);
					if (connectPowerData.wireNode != null)
					{
						WireManager.Instance.RemoveActiveWire(connectPowerData.wireNode);
						UnityEngine.Object.Destroy(connectPowerData.wireNode.gameObject);
						connectPowerData.wireNode = null;
					}
					this.DecreaseDurability(connectPowerData);
					return;
				}
			}
		}
		else
		{
			TileEntityPowered poweredBlock3 = this.GetPoweredBlock(invData);
			if (poweredBlock3 != null)
			{
				_actionData.invData.holdingEntity.RightArmAnimationUse = true;
				connectPowerData2.startPoint = invData.hitInfo.hit.blockPos;
				connectPowerData2.HasStartPoint = true;
				EntityAlive holdingEntity2 = _actionData.invData.holdingEntity;
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.AddWire, connectPowerData2.startPoint, holdingEntity2.entityId), false, -1, -1, -1, null, 192, false);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.AddWire, connectPowerData2.startPoint, holdingEntity2.entityId), false);
				}
				Manager.BroadcastPlay(poweredBlock3.ToWorldPos().ToVector3(), poweredBlock3.IsPowered ? "wire_live_connect" : "wire_dead_connect", 0f);
				Transform handTransform2 = this.GetHandTransform(holdingEntity2);
				if (handTransform2 != null)
				{
					Transform transform = handTransform2.FindInChilds("wire_mesh", false);
					if (transform == null)
					{
						return;
					}
					if (connectPowerData2.wireNode != null)
					{
						WireManager.Instance.RemoveActiveWire(connectPowerData2.wireNode);
						UnityEngine.Object.Destroy(connectPowerData2.wireNode.gameObject);
						connectPowerData2.wireNode = null;
					}
					WireNode component = ((GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/WireNode"))).GetComponent<WireNode>();
					component.LocalPosition = invData.hitInfo.hit.blockPos.ToVector3() - Origin.position;
					component.localOffset = poweredBlock3.GetWireOffset();
					WireNode wireNode = component;
					wireNode.localOffset.x = wireNode.localOffset.x + 0.5f;
					WireNode wireNode2 = component;
					wireNode2.localOffset.y = wireNode2.localOffset.y + 0.5f;
					WireNode wireNode3 = component;
					wireNode3.localOffset.z = wireNode3.localOffset.z + 0.5f;
					component.Source = transform.gameObject;
					component.sourceOffset = this.wireOffset;
					component.TogglePulse(false);
					component.SetPulseSpeed(360f);
					connectPowerData2.wireNode = component;
					WireManager.Instance.AddActiveWire(component);
					string name2 = "wire_tool_" + (poweredBlock3.IsPowered ? "sparks" : "dust");
					GameManager.Instance.SpawnParticleEffectServer(new ParticleEffect(name2, handTransform2.position + Origin.position, handTransform2.rotation, holdingEntity2.GetLightBrightness(), Color.white), invData.holdingEntity.entityId, false, false);
				}
			}
		}
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x00113C4C File Offset: 0x00111E4C
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform GetHandTransform(EntityAlive holdingEntity)
	{
		Transform transform = holdingEntity.RootTransform.Find("Graphics").FindInChilds(holdingEntity.GetRightHandTransformName(), true);
		Transform result;
		if (transform != null && transform.childCount > 0)
		{
			result = transform;
		}
		else
		{
			Transform transform2 = holdingEntity.RootTransform.Find("Camera").FindInChilds(holdingEntity.GetRightHandTransformName(), true);
			if (transform2 != null && transform2.childCount > 0)
			{
				result = transform2;
			}
			else
			{
				result = holdingEntity.emodel.GetRightHandTransform();
			}
		}
		return result;
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x00113CD0 File Offset: 0x00111ED0
	[PublicizedFrom(EAccessModifier.Internal)]
	public void CheckForWireRemoveNeeded(EntityAlive _player, Vector3i _blockPos)
	{
		ItemActionConnectPower.ConnectPowerData connectPowerData = (ItemActionConnectPower.ConnectPowerData)_player.inventory.holdingItemData.actionData[1];
		if (connectPowerData.HasStartPoint && connectPowerData.startPoint == _blockPos)
		{
			this.DisconnectWire(connectPowerData);
		}
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x00113D18 File Offset: 0x00111F18
	public override ItemClass.EnumCrosshairType GetCrosshairType(ItemActionData _actionData)
	{
		if (_actionData.invData.hitInfo.bHitValid && (_actionData as ItemActionConnectPower.ConnectPowerData).isFriendly)
		{
			int num = (int)(Constants.cDigAndBuildDistance * Constants.cDigAndBuildDistance);
			if (_actionData.invData.hitInfo.hit.distanceSq <= (float)num)
			{
				Vector3i blockPos = _actionData.invData.hitInfo.hit.blockPos;
				Block block = _actionData.invData.world.GetBlock(blockPos).Block;
				if (block is BlockPowered)
				{
					return ItemClass.EnumCrosshairType.PowerItem;
				}
				if (block is BlockPowerSource)
				{
					return ItemClass.EnumCrosshairType.PowerSource;
				}
			}
		}
		return ItemClass.EnumCrosshairType.Plus;
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x00113DB0 File Offset: 0x00111FB0
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPowered GetPoweredBlock(ItemInventoryData data)
	{
		Block block = data.world.GetBlock(data.hitInfo.hit.blockPos).Block;
		if (!(block is BlockPowered) && !(block is BlockPowerSource))
		{
			return null;
		}
		Vector3i blockPos = data.hitInfo.hit.blockPos;
		ChunkCluster chunkCluster = data.world.ChunkClusters[data.hitInfo.hit.clrIdx];
		if (chunkCluster == null)
		{
			return null;
		}
		Chunk chunk = (Chunk)chunkCluster.GetChunkSync(World.toChunkXZ(blockPos.x), blockPos.y, World.toChunkXZ(blockPos.z));
		if (chunk == null)
		{
			return null;
		}
		TileEntity tileEntity = chunk.GetTileEntity(World.toBlock(blockPos));
		if (tileEntity == null)
		{
			if (block is BlockPowered)
			{
				tileEntity = (block as BlockPowered).CreateTileEntity(chunk);
			}
			else if (block is BlockPowerSource)
			{
				tileEntity = (block as BlockPowerSource).CreateTileEntity(chunk);
			}
			tileEntity.localChunkPos = World.toBlock(blockPos);
			BlockEntityData blockEntity = chunk.GetBlockEntity(blockPos);
			if (blockEntity != null)
			{
				((TileEntityPowered)tileEntity).BlockTransform = blockEntity.transform;
			}
			((TileEntityPowered)tileEntity).InitializePowerData();
			chunk.AddTileEntity(tileEntity);
		}
		return tileEntity as TileEntityPowered;
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x00113EEC File Offset: 0x001120EC
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityPowered GetPoweredBlock(Vector3i tileEntityPos)
	{
		World world = GameManager.Instance.World;
		Block block = world.GetBlock(tileEntityPos).Block;
		if (!(block is BlockPowered) && !(block is BlockPowerSource))
		{
			return null;
		}
		Chunk chunk = world.GetChunkFromWorldPos(tileEntityPos.x, tileEntityPos.y, tileEntityPos.z) as Chunk;
		if (chunk == null)
		{
			return null;
		}
		TileEntity tileEntity = chunk.GetTileEntity(World.toBlock(tileEntityPos));
		if (tileEntity == null)
		{
			if (block is BlockPowered)
			{
				tileEntity = (block as BlockPowered).CreateTileEntity(chunk);
			}
			else if (block is BlockPowerSource)
			{
				tileEntity = (block as BlockPowerSource).CreateTileEntity(chunk);
			}
			tileEntity.localChunkPos = World.toBlock(tileEntityPos);
			BlockEntityData blockEntity = chunk.GetBlockEntity(tileEntityPos);
			if (blockEntity != null)
			{
				((TileEntityPowered)tileEntity).BlockTransform = blockEntity.transform;
			}
			((TileEntityPowered)tileEntity).InitializePowerData();
			chunk.AddTileEntity(tileEntity);
		}
		return tileEntity as TileEntityPowered;
	}

	// Token: 0x06002A38 RID: 10808 RVA: 0x00113FD4 File Offset: 0x001121D4
	public bool DisconnectWire(ItemActionConnectPower.ConnectPowerData _actionData)
	{
		if (!_actionData.HasStartPoint)
		{
			return false;
		}
		_actionData.HasStartPoint = false;
		if (_actionData.wireNode != null)
		{
			WireManager.Instance.RemoveActiveWire(_actionData.wireNode);
			UnityEngine.Object.Destroy(_actionData.wireNode.gameObject);
			_actionData.wireNode = null;
		}
		Chunk chunk = _actionData.invData.world.GetChunkFromWorldPos(_actionData.startPoint) as Chunk;
		if (chunk == null)
		{
			return false;
		}
		TileEntityPowered tileEntityPowered = _actionData.invData.world.GetTileEntity(chunk.ClrIdx, _actionData.startPoint) as TileEntityPowered;
		if (tileEntityPowered != null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, _actionData.invData.holdingEntity.entityId), false, -1, -1, -1, null, 192, false);
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireToolActions>().Setup(NetPackageWireToolActions.WireActions.RemoveWire, Vector3i.zero, _actionData.invData.holdingEntity.entityId), false);
			}
			Manager.BroadcastPlay(tileEntityPowered.ToWorldPos().ToVector3(), tileEntityPowered.IsPowered ? "wire_live_break" : "wire_dead_break", 0f);
			EntityAlive holdingEntity = _actionData.invData.holdingEntity;
			string name = "wire_tool_" + (tileEntityPowered.IsPowered ? "sparks" : "dust");
			Transform handTransform = this.GetHandTransform(holdingEntity);
			GameManager.Instance.SpawnParticleEffectServer(new ParticleEffect(name, handTransform.position + Origin.position, handTransform.rotation, holdingEntity.GetLightBrightness(), Color.white), holdingEntity.entityId, false, false);
		}
		_actionData.invData.holdingEntity.RightArmAnimationAttack = true;
		_actionData.invData.holdingEntity.PlayOneShot("ui_denied", false, false, false, null);
		return true;
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x001141B0 File Offset: 0x001123B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void DecreaseDurability(ItemActionConnectPower.ConnectPowerData _actionData)
	{
		if (_actionData.invData.itemValue.MaxUseTimes > 0)
		{
			if (_actionData.invData.itemValue.UseTimes + 1f < (float)_actionData.invData.itemValue.MaxUseTimes)
			{
				_actionData.invData.itemValue.UseTimes += EffectManager.GetValue(PassiveEffects.DegradationPerUse, _actionData.invData.itemValue, 1f, _actionData.invData.holdingEntity, null, _actionData.invData.itemValue.ItemClass.ItemTags, true, true, true, true, true, 1, true, false);
				base.HandleItemBreak(_actionData);
				return;
			}
			_actionData.invData.holdingEntity.inventory.DecHoldingItem(1);
		}
	}

	// Token: 0x040020DC RID: 8412
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 wireOffset = Vector3.zero;

	// Token: 0x040020DD RID: 8413
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxWireLength = 15;

	// Token: 0x0200050F RID: 1295
	public class ConnectPowerData : ItemActionAttackData
	{
		// Token: 0x06002A3B RID: 10811 RVA: 0x00112618 File Offset: 0x00110818
		public ConnectPowerData(ItemInventoryData _invData, int _indexInEntityOfAction) : base(_invData, _indexInEntityOfAction)
		{
		}

		// Token: 0x040020DE RID: 8414
		public bool StartLink;

		// Token: 0x040020DF RID: 8415
		public bool HasStartPoint;

		// Token: 0x040020E0 RID: 8416
		public LocalPlayerUI playerUI;

		// Token: 0x040020E1 RID: 8417
		public Vector3i startPoint;

		// Token: 0x040020E2 RID: 8418
		public bool inRange;

		// Token: 0x040020E3 RID: 8419
		public bool isFriendly;

		// Token: 0x040020E4 RID: 8420
		public WireNode wireNode;
	}
}

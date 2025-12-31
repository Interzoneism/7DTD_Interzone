using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Platform;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class BlockToolSelection : ISelectionBoxCallback, IBlockTool
{
	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000BD1 RID: 3025 RVA: 0x00050110 File Offset: 0x0004E310
	public SelectionBox SelectionBox
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			SelectionBox selectionBox = SelectionBoxManager.Instance.GetCategory("Selection").GetBox("SingleInstance");
			if (selectionBox != null)
			{
				return selectionBox;
			}
			selectionBox = SelectionBoxManager.Instance.GetCategory("Selection").AddBox("SingleInstance", Vector3i.zero, Vector3i.one, false, false);
			selectionBox.SetVisible(false);
			selectionBox.SetSizeVisibility(true);
			return selectionBox;
		}
	}

	// Token: 0x06000BD2 RID: 3026 RVA: 0x0005017C File Offset: 0x0004E37C
	public BlockToolSelection()
	{
		BlockToolSelection.Instance = this;
		SelectionBoxManager.Instance.GetCategory("Selection").SetCallback(this);
		PlayerActionsLocal primaryPlayer = PlatformManager.NativePlatform.Input.PrimaryPlayer;
		NGuiAction nguiAction = new NGuiAction(Localization.Get("selectionToolsEditBlocksVolume", false), null, true);
		nguiAction.SetClickActionDelegate(delegate
		{
			GameManager.bVolumeBlocksEditing = !GameManager.bVolumeBlocksEditing;
		});
		nguiAction.SetIsCheckedDelegate(() => GameManager.bVolumeBlocksEditing);
		nguiAction.SetIsVisibleDelegate(() => GameManager.Instance.IsEditMode());
		NGuiAction nguiAction2 = new NGuiAction(Localization.Get("selectionToolsCopySleeperVolume", false), null);
		nguiAction2.SetClickActionDelegate(delegate
		{
			if (XUiC_WoPropsSleeperVolume.selectedPrefabInstance != null && XUiC_WoPropsSleeperVolume.selectedVolumeIndex >= 0)
			{
				int selectedVolumeIndex = XUiC_WoPropsSleeperVolume.selectedVolumeIndex;
				PrefabInstance selectedPrefabInstance = XUiC_WoPropsSleeperVolume.selectedPrefabInstance;
				XUiC_WoPropsSleeperVolume.selectedPrefabInstance.prefab.CloneSleeperVolume(selectedPrefabInstance.name, selectedPrefabInstance.boundingBoxPosition, selectedVolumeIndex);
			}
		});
		nguiAction2.SetIsVisibleDelegate(() => GameManager.Instance.IsEditMode());
		nguiAction2.SetTooltip("selectionToolsCopySleeperVolumeTip");
		NGuiAction nguiAction3 = new NGuiAction(Localization.Get("selectionToolsCopyAirBlocks", false), null, true);
		nguiAction3.SetClickActionDelegate(delegate
		{
			this.copyPasteAirBlocks = !this.copyPasteAirBlocks;
		});
		nguiAction3.SetIsCheckedDelegate(() => this.copyPasteAirBlocks);
		nguiAction3.SetIsVisibleDelegate(new NGuiAction.IsVisibleDelegate(GameManager.Instance.IsEditMode));
		NGuiAction nguiAction4 = new NGuiAction(Localization.Get("selectionToolsClearSelection", false), primaryPlayer.SelectionClear);
		nguiAction4.SetClickActionDelegate(delegate
		{
			if (this.SelectionLockMode == 2)
			{
				this.SelectionLockMode = 0;
				this.SelectionActive = false;
				return;
			}
			this.BeginUndo(0);
			BlockTools.CubeRPC(GameManager.Instance, this.SelectionClrIdx, this.SelectionStart, this.SelectionEnd, BlockValue.Air, MarchingCubes.DensityAir, 0, TextureFullArray.Default);
			BlockTools.CubeWaterRPC(GameManager.Instance, this.SelectionStart, this.SelectionEnd, WaterValue.Empty);
			this.EndUndo(0, false);
		});
		nguiAction4.SetIsEnabledDelegate(() => GameManager.Instance.IsEditMode() && this.SelectionActive);
		nguiAction4.SetIsVisibleDelegate(() => GameManager.Instance.IsEditMode());
		nguiAction4.SetTooltip("selectionToolsClearSelectionTip");
		NGuiAction nguiAction5 = new NGuiAction(Localization.Get("selectionToolsFillSelection", false), primaryPlayer.SelectionFill);
		nguiAction5.SetClickActionDelegate(delegate
		{
			this.BeginUndo(0);
			EntityPlayerLocal primaryPlayer2 = GameManager.Instance.World.GetPrimaryPlayer();
			ItemValue holdingItemItemValue = primaryPlayer2.inventory.holdingItemItemValue;
			BlockValue blockValue = holdingItemItemValue.ToBlockValue();
			if (blockValue.isair)
			{
				return;
			}
			Block block = blockValue.Block;
			BlockPlacement.Result result = new BlockPlacement.Result(0, Vector3.zero, Vector3i.zero, blockValue, -1f);
			block.OnBlockPlaceBefore(GameManager.Instance.World, ref result, primaryPlayer2, GameManager.Instance.World.GetGameRandom());
			blockValue = result.blockValue;
			blockValue.rotation = ((primaryPlayer2.inventory.holdingItemData is ItemClassBlock.ItemBlockInventoryData) ? ((ItemClassBlock.ItemBlockInventoryData)primaryPlayer2.inventory.holdingItemData).rotation : blockValue.rotation);
			BlockTools.CubeRPC(GameManager.Instance, this.SelectionClrIdx, this.m_selectionStartPoint, this.m_SelectionEndPoint, blockValue, blockValue.Block.shape.IsTerrain() ? MarchingCubes.DensityTerrain : MarchingCubes.DensityAir, 0, holdingItemItemValue.TextureFullArray);
			this.EndUndo(0, false);
		});
		nguiAction5.SetIsEnabledDelegate(() => GameManager.Instance.IsEditMode() && this.SelectionActive);
		nguiAction5.SetIsVisibleDelegate(() => GameManager.Instance.IsEditMode());
		nguiAction5.SetTooltip("selectionToolsFillSelectionTip");
		NGuiAction nguiAction6 = new NGuiAction(Localization.Get("selectionToolsRandomFillSelection", false), null);
		nguiAction6.SetClickActionDelegate(delegate
		{
			this.BeginUndo(0);
			BlockTools.CubeRandomRPC(GameManager.Instance, this.SelectionClrIdx, this.m_selectionStartPoint, this.m_SelectionEndPoint, GameManager.Instance.World.GetPrimaryPlayer().inventory.holdingItemItemValue.ToBlockValue(), 0.1f, new EBlockRotationClasses?(EBlockRotationClasses.Basic90));
			this.EndUndo(0, false);
		});
		nguiAction6.SetIsEnabledDelegate(() => this.SelectionActive);
		nguiAction6.SetIsVisibleDelegate(() => GameManager.Instance.IsEditMode());
		nguiAction6.SetTooltip("selectionToolsRandomFillSelectionTip");
		NGuiAction nguiAction7 = new NGuiAction(Localization.Get("selectionToolsUndo", false), null);
		nguiAction7.SetClickActionDelegate(delegate
		{
			this.blockUndoRedo(false);
		});
		nguiAction7.SetIsEnabledDelegate(() => this.undoQueue.Count > 0);
		nguiAction7.SetIsVisibleDelegate(() => GameManager.Instance.IsEditMode());
		nguiAction7.SetTooltip("selectionToolsUndoTip");
		NGuiAction nguiAction8 = new NGuiAction(Localization.Get("selectionToolsRedo", false), null);
		nguiAction8.SetClickActionDelegate(delegate
		{
			this.blockUndoRedo(true);
		});
		nguiAction8.SetIsEnabledDelegate(() => this.redoQueue.Count > 0);
		nguiAction8.SetIsVisibleDelegate(() => GameManager.Instance.IsEditMode());
		nguiAction8.SetTooltip("selectionToolsRedoTip");
		this.actions = new Dictionary<string, NGuiAction>
		{
			{
				"volumeBlocksEditing",
				nguiAction
			},
			{
				"copySleeperVolume",
				nguiAction2
			},
			{
				"copyAirBlocks",
				nguiAction3
			},
			{
				"sep1",
				NGuiAction.Separator
			},
			{
				"clearSelection",
				nguiAction4
			},
			{
				"fillSelection",
				nguiAction5
			},
			{
				"randomFillSelection",
				nguiAction6
			},
			{
				"sep2",
				NGuiAction.Separator
			},
			{
				"undo",
				nguiAction7
			},
			{
				"redo",
				nguiAction8
			}
		};
		foreach (KeyValuePair<string, NGuiAction> keyValuePair in this.actions)
		{
			string text;
			NGuiAction nguiAction9;
			keyValuePair.Deconstruct(out text, out nguiAction9);
			NGuiAction action = nguiAction9;
			LocalPlayerUI.primaryUI.windowManager.AddGlobalAction(action);
		}
		Origin.OriginChanged = (Action<Vector3>)Delegate.Combine(Origin.OriginChanged, new Action<Vector3>(this.OnOriginChanged));
	}

	// Token: 0x06000BD3 RID: 3027 RVA: 0x00050674 File Offset: 0x0004E874
	public void CheckSpecialKeys(Event ev, PlayerActionsLocal playerActions)
	{
		if (this.hitInfo == null)
		{
			return;
		}
		Vector3i vector3i = (GameManager.Instance.IsEditMode() && playerActions.Run.IsPressed) ? this.hitInfo.hit.blockPos : this.hitInfo.lastBlockPos;
		bool flag = InputUtils.IsMac ? ((ev.modifiers & EventModifiers.Command) > EventModifiers.None) : ((ev.modifiers & EventModifiers.Control) > EventModifiers.None);
		bool flag2 = (ev.modifiers & EventModifiers.Shift) > EventModifiers.None;
		KeyCode keyCode = ev.keyCode;
		if (keyCode != KeyCode.C)
		{
			if (keyCode != KeyCode.V)
			{
				if (keyCode != KeyCode.Z)
				{
					return;
				}
				if (flag)
				{
					this.blockUndoRedo(false);
				}
			}
			else if (flag)
			{
				if (!flag2 && this.SelectionLockMode != 2)
				{
					if (this.SelectionActive && this.clipboard.size.Equals(Vector3i.one) && !this.SelectionSize.Equals(this.clipboard.size))
					{
						this.BeginUndo(0);
						BlockValue block = this.clipboard.GetBlock(0, 0, 0);
						WaterValue water = this.clipboard.GetWater(0, 0, 0);
						TextureFullArray texture = this.clipboard.GetTexture(0, 0, 0);
						BlockTools.CubeRPC(GameManager.Instance, this.SelectionClrIdx, this.m_selectionStartPoint, this.m_SelectionEndPoint, block, block.Block.shape.IsTerrain() ? MarchingCubes.DensityTerrain : MarchingCubes.DensityAir, 0, texture);
						BlockTools.CubeWaterRPC(GameManager.Instance, this.m_selectionStartPoint, this.m_SelectionEndPoint, water);
						this.EndUndo(0, false);
						return;
					}
					if (this.SelectionActive && !this.SelectionSize.Equals(this.clipboard.size))
					{
						this.SelectionEnd = this.SelectionStart + this.clipboard.size - Vector3i.one;
						return;
					}
					if (!this.SelectionActive)
					{
						this.SelectionStart = vector3i;
						this.SelectionEnd = this.SelectionStart + this.clipboard.size - Vector3i.one;
						this.SelectionActive = true;
						return;
					}
					if (this.SelectionActive && this.SelectionSize.Equals(this.clipboard.size))
					{
						this.blockPaste(0, this.SelectionMin, this.clipboard);
						return;
					}
				}
				else
				{
					if (this.SelectionLockMode != 2)
					{
						if (this.SelectionSize != this.clipboard.size)
						{
							this.SelectionEnd = this.SelectionStart + this.clipboard.size - Vector3i.one;
						}
						this.SelectionActive = true;
						this.SelectionLockMode = 2;
						this.createBlockPreviewFrom(this.clipboard);
						return;
					}
					this.SelectionLockMode = 0;
					this.blockPaste(0, this.SelectionMin, this.clipboard);
					return;
				}
			}
		}
		else if (flag)
		{
			if (!this.SelectionActive)
			{
				this.SelectionStart = vector3i;
				this.SelectionEnd = vector3i;
			}
			this.blockCopy(this.clipboard);
			return;
		}
	}

	// Token: 0x06000BD4 RID: 3028 RVA: 0x00050978 File Offset: 0x0004EB78
	[PublicizedFrom(EAccessModifier.Private)]
	public void rotatePreviewAroundY()
	{
		if (this.previewGORot2 == null)
		{
			return;
		}
		this.previewGORot2.transform.localRotation = Quaternion.AngleAxis(90f, Vector3.up) * this.previewGORot2.transform.localRotation;
		this.clipboard.RotateY(false, 1);
		Vector3 b = this.previewGORot2.transform.localRotation * this.offsetToMin;
		Vector3 vector = this.selectionRotCenter + b;
		Vector3 b2 = this.previewGORot2.transform.localRotation * this.offsetToMax;
		Vector3 vector2 = this.selectionRotCenter + b2;
		Vector3i vector3i = new Vector3i(Utils.Fastfloor(Utils.FastMin(vector.x, vector2.x)), Utils.Fastfloor(Utils.FastMin(vector.y, vector2.y)), Utils.Fastfloor(Utils.FastMin(vector.z, vector2.z)));
		this.SelectionStart = vector3i;
		this.SelectionEnd = vector3i + this.clipboard.size - Vector3i.one;
	}

	// Token: 0x06000BD5 RID: 3029 RVA: 0x00050A9A File Offset: 0x0004EC9A
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeBlockPreview()
	{
		this.previewGORot3.transform.DestroyChildren();
	}

	// Token: 0x06000BD6 RID: 3030 RVA: 0x00050AAC File Offset: 0x0004ECAC
	[PublicizedFrom(EAccessModifier.Private)]
	public void createBlockPreviewFrom(Prefab _prefab)
	{
		if (this.previewGOParent == null)
		{
			this.previewGOParent = new GameObject("Preview");
			this.previewGOParent.transform.parent = null;
			this.previewGOParent.transform.localPosition = Vector3.zero;
			this.previewGORot1 = new GameObject("Rot1");
			this.previewGORot1.transform.parent = this.previewGOParent.transform;
			this.previewGORot2 = new GameObject("Rot2");
			this.previewGORot2.transform.parent = this.previewGORot1.transform;
			this.previewGORot3 = new GameObject("Rot3");
			this.previewGORot3.transform.parent = this.previewGORot2.transform;
		}
		else
		{
			this.removeBlockPreview();
		}
		ThreadManager.RunCoroutineSync(_prefab.ToTransform(true, true, true, false, this.previewGORot3.transform, "PrefabImposter", Vector3.zero, DynamicPrefabDecorator.PrefabPreviewLimit));
		Transform transform = this.previewGORot3.transform.Find("PrefabImposter");
		transform.localRotation = Quaternion.identity;
		transform.localPosition = Vector3.zero;
		Vector3 vector = new Vector3((float)(_prefab.size.x / 2), 0f, (float)(_prefab.size.z / 2));
		this.previewGORot1.transform.position = this.SelectionMin.ToVector3() - Origin.position;
		this.previewGORot1.transform.rotation = Quaternion.identity;
		this.previewGORot2.transform.localPosition = vector;
		this.previewGORot2.transform.localRotation = Quaternion.identity;
		this.previewGORot3.transform.localPosition = -vector;
		this.previewGORot3.transform.localRotation = Quaternion.identity;
		vector = -vector;
		vector.y = (float)(-(float)_prefab.size.y / 2);
		this.offsetToMax = vector + (_prefab.size - Vector3i.one).ToVector3() + Vector3.one * 0.5f;
		this.offsetToMin = vector + Vector3.one * 0.5f;
		this.selectionRotCenter = this.SelectionMin.ToVector3() - vector;
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x00050D1C File Offset: 0x0004EF1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnOriginChanged(Vector3 _newOrigin)
	{
		if (this.previewGORot1 == null)
		{
			return;
		}
		this.previewGORot1.transform.position = this.SelectionMin.ToVector3() - Origin.position;
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x00050D60 File Offset: 0x0004EF60
	public void RotateFocusedBlock(WorldRayHitInfo _hitInfo, PlayerActionsLocal _playerActions)
	{
		if (!_hitInfo.bHitValid)
		{
			return;
		}
		Vector3i vector3i = (GameManager.Instance.World.IsEditor() && _playerActions.Run.IsPressed) ? _hitInfo.hit.blockPos : _hitInfo.lastBlockPos;
		BlockValue block = GameManager.Instance.World.ChunkClusters[_hitInfo.hit.clrIdx].GetBlock(vector3i);
		if (block.Block.shape.IsRotatable)
		{
			block.rotation = block.Block.shape.Rotate(false, (int)block.rotation);
			this.setBlock(_hitInfo.hit.clrIdx, vector3i, block);
		}
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00050E18 File Offset: 0x0004F018
	public void CheckKeys(ItemInventoryData _data, WorldRayHitInfo _hitInfo, PlayerActionsLocal playerActions)
	{
		if (LocalPlayerUI.primaryUI.windowManager.IsInputActive())
		{
			return;
		}
		this.hitInfo = _hitInfo;
		Vector3i vector3i = (_data.world.IsEditor() && playerActions.Run.IsPressed) ? _hitInfo.hit.blockPos : _hitInfo.lastBlockPos;
		ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData = _data as ItemClassBlock.ItemBlockInventoryData;
		if (itemBlockInventoryData != null)
		{
			BlockValue bv = itemBlockInventoryData.itemValue.ToBlockValue();
			bv.rotation = itemBlockInventoryData.rotation;
			itemBlockInventoryData.rotation = bv.Block.BlockPlacementHelper.OnPlaceBlock(itemBlockInventoryData.mode, itemBlockInventoryData.localRot, GameManager.Instance.World, bv, this.hitInfo.hit, itemBlockInventoryData.holdingEntity.position).blockValue.rotation;
		}
		if (!GameManager.Instance.IsEditMode() && !GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled))
		{
			return;
		}
		if (playerActions.SelectionSet.IsPressed)
		{
			if (GameManager.Instance.World.ChunkClusters[_hitInfo.hit.clrIdx] == null)
			{
				return;
			}
			if (InputUtils.ControlKeyPressed)
			{
				return;
			}
			this.SelectionLockMode = 0;
			this.SelectionClrIdx = _hitInfo.hit.clrIdx;
			Vector3i vector3i2 = vector3i;
			if (!this.SelectionActive)
			{
				Vector3i selectionSize = this.SelectionSize;
				this.SelectionStart = vector3i2;
				if (this.SelectionLockMode == 1)
				{
					this.SelectionEnd = this.SelectionStart + selectionSize - Vector3i.one;
				}
				else
				{
					this.SelectionEnd = this.SelectionStart;
				}
				this.SelectionActive = true;
			}
			else
			{
				this.SelectionEnd = vector3i2;
			}
		}
		if (!GameManager.Instance.IsEditMode())
		{
			return;
		}
		if (playerActions.DensityM1.WasPressed || playerActions.DensityP1.WasPressed || playerActions.DensityM10.WasPressed || playerActions.DensityP10.WasPressed)
		{
			int num = (playerActions.DensityM1.WasPressed || playerActions.DensityP1.WasPressed) ? 1 : 10;
			if (playerActions.DensityM1.WasPressed || playerActions.DensityM10.WasPressed)
			{
				num = -num;
			}
			if (InputUtils.ControlKeyPressed)
			{
				num *= 50;
			}
			BlockValue block = GameManager.Instance.World.GetBlock(_hitInfo.hit.clrIdx, vector3i);
			Block block2 = block.Block;
			if (block2.BlockTag == BlockTags.Door)
			{
				if (num > 0)
				{
					num = ((block.damage + num >= block2.MaxDamagePlusDowngrades) ? (block2.MaxDamagePlusDowngrades - block.damage - 1) : num);
				}
				block2.DamageBlock(GameManager.Instance.World, _hitInfo.hit.clrIdx, vector3i, block, num, -1, null, false, false);
			}
			else
			{
				int num2;
				if (!this.SelectionActive)
				{
					num2 = (int)GameManager.Instance.World.GetDensity(_hitInfo.hit.clrIdx, vector3i);
				}
				else
				{
					num2 = (int)GameManager.Instance.World.GetDensity(0, this.m_selectionStartPoint);
				}
				num2 += num;
				num2 = Utils.FastClamp(num2, (int)MarchingCubes.DensityTerrain, (int)MarchingCubes.DensityAir);
				if (!this.SelectionActive)
				{
					GameManager.Instance.World.SetBlocksRPC(new List<BlockChangeInfo>
					{
						new BlockChangeInfo(_hitInfo.hit.clrIdx, vector3i, (sbyte)num2, false)
					});
				}
				else
				{
					BlockTools.CubeDensityRPC(GameManager.Instance, this.m_selectionStartPoint, this.m_SelectionEndPoint, (sbyte)num2);
				}
			}
		}
		if ((playerActions.FocusCopyBlock.WasPressed || (playerActions.Secondary.WasPressed && InputUtils.ControlKeyPressed)) && GameManager.Instance.IsEditMode() && _hitInfo.bHitValid && !_hitInfo.hit.blockValue.isair)
		{
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			BlockValue blockValue = _hitInfo.hit.blockValue;
			if (blockValue.ischild)
			{
				Vector3i parentPos = blockValue.Block.multiBlockPos.GetParentPos(_hitInfo.hit.blockPos, blockValue);
				blockValue = GameManager.Instance.World.GetBlock(parentPos);
			}
			ItemStack itemStack = new ItemStack(blockValue.ToItemValue(), 99);
			if (blockValue.Block.GetAutoShapeType() != EAutoShapeType.Helper)
			{
				itemStack.itemValue.TextureFullArray = GameManager.Instance.World.ChunkCache.GetTextureFullArray(_hitInfo.hit.blockPos);
			}
			if (primaryPlayer.inventory.GetItemCount(itemStack.itemValue, true, -1, -1, true) == 0 && primaryPlayer.inventory.CanTakeItem(itemStack))
			{
				int idx;
				if (primaryPlayer.inventory.AddItem(itemStack, out idx))
				{
					ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData2 = primaryPlayer.inventory.GetItemDataInSlot(idx) as ItemClassBlock.ItemBlockInventoryData;
					if (itemBlockInventoryData2 != null)
					{
						itemBlockInventoryData2.damage = blockValue.damage;
						return;
					}
				}
			}
			else
			{
				ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData3 = _data as ItemClassBlock.ItemBlockInventoryData;
				if (itemBlockInventoryData3 != null && this.hasSameShape(blockValue.type, primaryPlayer.inventory.holdingItemItemValue.type))
				{
					itemBlockInventoryData3.rotation = blockValue.rotation;
					itemBlockInventoryData3.damage = blockValue.damage;
				}
			}
		}
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x0005131C File Offset: 0x0004F51C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasSameShape(int _blockId1, int _blockId2)
	{
		Block block = Block.list[_blockId1];
		Block block2 = Block.list[_blockId2];
		return !(block.shape.GetType() != block2.shape.GetType()) && (!(block.shape is BlockShapeNew) || block.Properties.Values["Model"] == block2.Properties.Values["Model"]);
	}

	// Token: 0x06000BDB RID: 3035 RVA: 0x00051398 File Offset: 0x0004F598
	public bool ConsumeScrollWheel(ItemInventoryData _data, float _scrollWheelInput, PlayerActionsLocal _playerInput)
	{
		if ((_playerInput.Reload.IsPressed || _playerInput.PermanentActions.Reload.IsPressed) && _data is ItemClassBlock.ItemBlockInventoryData && Mathf.Abs(_scrollWheelInput) >= 0.001f)
		{
			ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData = (ItemClassBlock.ItemBlockInventoryData)_data;
			itemBlockInventoryData.rotation = itemBlockInventoryData.itemValue.ToBlockValue().Block.BlockPlacementHelper.LimitRotation(itemBlockInventoryData.mode, ref itemBlockInventoryData.localRot, _data.hitInfo.hit, _scrollWheelInput > 0f, itemBlockInventoryData.itemValue.ToBlockValue(), itemBlockInventoryData.rotation);
			return true;
		}
		return false;
	}

	// Token: 0x06000BDC RID: 3036 RVA: 0x00051438 File Offset: 0x0004F638
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i createBlockMoveVector(Vector3 _relPlayerAxis)
	{
		Vector3i zero = Vector3i.zero;
		if (Math.Abs(_relPlayerAxis.x) > Math.Abs(_relPlayerAxis.z))
		{
			zero = new Vector3i(Mathf.Sign(_relPlayerAxis.x), 0f, 0f);
		}
		else
		{
			zero = new Vector3i(0f, 0f, Mathf.Sign(_relPlayerAxis.z));
		}
		return zero;
	}

	// Token: 0x06000BDD RID: 3037 RVA: 0x000514A0 File Offset: 0x0004F6A0
	public bool ExecuteUseAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions)
	{
		if (!(_data is ItemClassBlock.ItemBlockInventoryData))
		{
			return false;
		}
		bool flag = GameManager.Instance.IsEditMode() || GameStats.GetInt(EnumGameStats.GameModeId) == 2;
		if (flag && playerActions.Drop.IsPressed)
		{
			return false;
		}
		if (_bReleased)
		{
			return false;
		}
		if (Time.time - this.lastBuildTime < Constants.cBuildIntervall)
		{
			return true;
		}
		this.lastBuildTime = Time.time;
		ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData = (ItemClassBlock.ItemBlockInventoryData)_data;
		EntityAlive holdingEntity = itemBlockInventoryData.holdingEntity;
		FastTags<TagGroup.Global> tags = FastTags<TagGroup.Global>.none;
		ItemClassBlock itemClassBlock = itemBlockInventoryData.item as ItemClassBlock;
		if (itemClassBlock != null)
		{
			tags = itemClassBlock.GetBlock().Tags;
		}
		if (EffectManager.GetValue(PassiveEffects.DisableItem, holdingEntity.inventory.holdingItemItemValue, 0f, holdingEntity, null, tags, true, true, true, true, true, 1, true, false) > 0f)
		{
			this.lastBuildTime = Time.time + 1f;
			Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
			return false;
		}
		HitInfoDetails hitInfoDetails = itemBlockInventoryData.hitInfo.hit.Clone();
		if (!itemBlockInventoryData.hitInfo.bHitValid)
		{
			return false;
		}
		hitInfoDetails.blockPos = ((flag && playerActions.Run.IsPressed) ? itemBlockInventoryData.hitInfo.hit.blockPos : itemBlockInventoryData.hitInfo.lastBlockPos);
		BlockValue blockValue = itemBlockInventoryData.itemValue.ToBlockValue();
		Block block = blockValue.Block;
		blockValue.damage = itemBlockInventoryData.damage;
		blockValue.rotation = itemBlockInventoryData.rotation;
		World world = GameManager.Instance.World;
		if (!GameManager.Instance.IsEditMode())
		{
			int placementDistanceSq = block.GetPlacementDistanceSq();
			if (hitInfoDetails.distanceSq > (float)placementDistanceSq)
			{
				return true;
			}
			Vector3i freePlacementPosition = block.GetFreePlacementPosition(world, itemBlockInventoryData.hitInfo.hit.clrIdx, hitInfoDetails.blockPos, blockValue, holdingEntity);
			if (!holdingEntity.IsGodMode.Value && GameUtils.IsColliderWithinBlock(freePlacementPosition, blockValue))
			{
				return true;
			}
			if (hitInfoDetails.blockPos == Vector3i.zero)
			{
				return true;
			}
		}
		_data.holdingEntity.RightArmAnimationUse = true;
		BlockPlacement.Result result = block.BlockPlacementHelper.OnPlaceBlock(itemBlockInventoryData.mode, itemBlockInventoryData.localRot, GameManager.Instance.World, blockValue, hitInfoDetails, itemBlockInventoryData.holdingEntity.position);
		block.OnBlockPlaceBefore(itemBlockInventoryData.world, ref result, itemBlockInventoryData.holdingEntity, itemBlockInventoryData.world.GetGameRandom());
		blockValue = result.blockValue;
		block = blockValue.Block;
		if (blockValue.damage == 0)
		{
			blockValue.damage = block.StartDamage;
			result.blockValue.damage = block.StartDamage;
		}
		if (!playerActions.Run.IsPressed)
		{
			result.blockPos = block.GetFreePlacementPosition(itemBlockInventoryData.holdingEntity.world, 0, result.blockPos, blockValue, itemBlockInventoryData.holdingEntity);
		}
		if (!block.CanPlaceBlockAt(itemBlockInventoryData.world, result.clrIdx, result.blockPos, blockValue, false))
		{
			itemBlockInventoryData.holdingEntity.PlayOneShot("keystone_build_warning", false, false, false, null);
			return true;
		}
		eSetBlockResponse eSetBlockResponse;
		if (!BlockLimitTracker.instance.CanAddBlock(blockValue, result.blockPos, out eSetBlockResponse))
		{
			if (eSetBlockResponse != eSetBlockResponse.PowerBlockLimitExceeded)
			{
				if (eSetBlockResponse == eSetBlockResponse.StorageBlockLimitExceeded)
				{
					GameManager.ShowTooltip(GameManager.Instance.World.GetPrimaryPlayer(), "uicannotaddstorageblock", false, false, 0f);
				}
			}
			else
			{
				GameManager.ShowTooltip(GameManager.Instance.World.GetPrimaryPlayer(), "uicannotaddpowerblock", false, false, 0f);
			}
			return true;
		}
		if (!GameManager.Instance.IsEditMode())
		{
			if (block.IndexName == "lpblock")
			{
				if (!itemBlockInventoryData.world.CanPlaceLandProtectionBlockAt(itemBlockInventoryData.hitInfo.lastBlockPos, itemBlockInventoryData.world.gameManager.GetPersistentLocalPlayer()))
				{
					itemBlockInventoryData.holdingEntity.PlayOneShot("keystone_build_warning", false, false, false, null);
					return true;
				}
				itemBlockInventoryData.holdingEntity.PlayOneShot("keystone_placed", false, false, false, null);
			}
			else if (!itemBlockInventoryData.world.CanPlaceBlockAt(itemBlockInventoryData.hitInfo.lastBlockPos, itemBlockInventoryData.world.gameManager.GetPersistentLocalPlayer(), false))
			{
				itemBlockInventoryData.holdingEntity.PlayOneShot("keystone_build_warning", false, false, false, null);
				return true;
			}
		}
		BiomeDefinition biome = itemBlockInventoryData.world.GetBiome(result.blockPos.x, result.blockPos.z);
		if (biome != null && biome.Replacements.ContainsKey(result.blockValue.type))
		{
			result.blockValue.type = biome.Replacements[result.blockValue.type];
		}
		this.addToUndo(_data.hitInfo.hit.clrIdx, result.blockPos, GameManager.Instance.World.GetBlock(result.blockPos));
		if (Block.list[itemBlockInventoryData.itemValue.type].SelectAlternates)
		{
			if (itemBlockInventoryData.itemValue.TextureFullArray.IsDefault)
			{
				block.PlaceBlock(itemBlockInventoryData.world, result, itemBlockInventoryData.holdingEntity);
			}
			else
			{
				BlockChangeInfo blockChangeInfo = new BlockChangeInfo(0, result.blockPos, blockValue, itemBlockInventoryData.holdingEntity.entityId);
				blockChangeInfo.textureFull = itemBlockInventoryData.itemValue.TextureFullArray;
				blockChangeInfo.bChangeTexture = true;
				GameManager.Instance.World.SetBlocksRPC(new List<BlockChangeInfo>
				{
					blockChangeInfo
				});
			}
		}
		else if (itemBlockInventoryData.itemValue.TextureFullArray.IsDefault)
		{
			block.PlaceBlock(itemBlockInventoryData.world, result, itemBlockInventoryData.holdingEntity);
		}
		else
		{
			BlockChangeInfo blockChangeInfo2 = new BlockChangeInfo(0, result.blockPos, blockValue, itemBlockInventoryData.holdingEntity.entityId);
			blockChangeInfo2.textureFull = itemBlockInventoryData.itemValue.TextureFullArray;
			blockChangeInfo2.bChangeTexture = true;
			GameManager.Instance.World.SetBlocksRPC(new List<BlockChangeInfo>
			{
				blockChangeInfo2
			});
		}
		QuestEventManager.Current.BlockPlaced(block.GetBlockName(), result.blockPos);
		itemBlockInventoryData.holdingEntity.RightArmAnimationUse = true;
		itemBlockInventoryData.lastBuildTime = Time.time;
		itemBlockInventoryData.holdingEntity.MinEventContext.ItemActionData = itemBlockInventoryData.actionData[0];
		itemBlockInventoryData.holdingEntity.MinEventContext.BlockValue = result.blockValue;
		itemBlockInventoryData.holdingEntity.MinEventContext.Position = result.pos;
		itemBlockInventoryData.holdingEntity.FireEvent(MinEventTypes.onSelfPlaceBlock, true);
		GameManager.Instance.StartCoroutine(this.decInventoryLater(itemBlockInventoryData, itemBlockInventoryData.holdingEntity.inventory.holdingItemIdx));
		if (!block.shape.IsOmitTerrainSnappingUp && !block.IsTerrainDecoration)
		{
			itemBlockInventoryData.world.ChunkCache.SnapTerrainToPositionAroundRPC(itemBlockInventoryData.world, itemBlockInventoryData.hitInfo.lastBlockPos - Vector3i.up);
		}
		return true;
	}

	// Token: 0x06000BDE RID: 3038 RVA: 0x00051B51 File Offset: 0x0004FD51
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator decInventoryLater(ItemInventoryData data, int index)
	{
		data.holdingEntity.inventory.WaitForSecondaryRelease = (data.holdingEntity.inventory.holdingItemStack.count == 1);
		yield return new WaitForSeconds(0.1f);
		if (!GameManager.Instance.IsEditMode())
		{
			ItemStack itemStack = data.holdingEntity.inventory.GetItem(index).Clone();
			if (itemStack.count > 0)
			{
				itemStack.count--;
			}
			data.holdingEntity.inventory.SetItem(index, itemStack);
		}
		BlockValue blockValue = data.itemValue.ToBlockValue();
		string clipName = "placeblock";
		Block block = blockValue.Block;
		if (block.CustomPlaceSound != null)
		{
			clipName = block.CustomPlaceSound;
		}
		data.holdingEntity.PlayOneShot(clipName, false, false, false, null);
		yield break;
	}

	// Token: 0x06000BDF RID: 3039 RVA: 0x00051B68 File Offset: 0x0004FD68
	public bool ExecuteAttackAction(ItemInventoryData _data, bool _bReleased, PlayerActionsLocal playerActions)
	{
		if (!_bReleased)
		{
			return false;
		}
		bool flag = false;
		if (GameManager.Instance.IsEditMode() && playerActions.Drop.IsPressed)
		{
			return false;
		}
		if (!playerActions.SelectionSet.IsPressed && this.SelectionActive)
		{
			if (!playerActions.Drop.IsPressed)
			{
				flag = (flag || this.SelectionActive);
				if (this.SelectionLockMode == 1)
				{
					Vector3i selectionSize = this.SelectionSize;
					this.SelectionStart = _data.hitInfo.hit.blockPos;
					this.SelectionEnd = this.SelectionStart + selectionSize - Vector3i.one;
				}
				else if (this.SelectionLockMode == 0)
				{
					this.SelectionActive = false;
				}
			}
		}
		else if (GameManager.Instance.IsEditMode() && playerActions.Run.IsPressed && _data.hitInfo.bHitValid)
		{
			Vector3i blockPos = playerActions.Run.IsPressed ? _data.hitInfo.hit.blockPos : _data.hitInfo.lastBlockPos;
			this.setBlock(_data.hitInfo.hit.clrIdx, blockPos, BlockValue.Air);
			flag = true;
		}
		else if (_data is ItemClassBlock.ItemBlockInventoryData)
		{
			ItemClassBlock.ItemBlockInventoryData itemBlockInventoryData = (ItemClassBlock.ItemBlockInventoryData)_data;
			itemBlockInventoryData.itemValue.ToBlockValue().Block.RotateHoldingBlock(itemBlockInventoryData, true, true);
			flag = true;
		}
		return flag;
	}

	// Token: 0x06000BE0 RID: 3040 RVA: 0x00051CCC File Offset: 0x0004FECC
	[PublicizedFrom(EAccessModifier.Private)]
	public void rotateSelectionAroundY()
	{
		Vector3i other = new Vector3i(Mathf.Abs(this.m_selectionStartPoint.x - this.m_SelectionEndPoint.x), Mathf.Abs(this.m_selectionStartPoint.y - this.m_SelectionEndPoint.y), Mathf.Abs(this.m_selectionStartPoint.z - this.m_SelectionEndPoint.z));
		Vector3i vector3i = new Vector3i(Mathf.Min(this.m_selectionStartPoint.x, this.m_SelectionEndPoint.x), Mathf.Min(this.m_selectionStartPoint.y, this.m_SelectionEndPoint.y), Mathf.Min(this.m_selectionStartPoint.z, this.m_SelectionEndPoint.z));
		Prefab prefab = BlockTools.CopyIntoStorage(GameManager.Instance, vector3i, vector3i + other);
		this.BeginUndo(0);
		new Prefab(prefab.size)
		{
			bCopyAirBlocks = true
		}.CopyIntoRPC(GameManager.Instance, vector3i, false);
		prefab.RotateY(false, 1);
		prefab.CopyIntoRPC(GameManager.Instance, vector3i, this.copyPasteAirBlocks);
		this.SelectionStart = vector3i;
		this.SelectionEnd = vector3i + prefab.size - Vector3i.one;
		this.EndUndo(0, false);
	}

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x06000BE1 RID: 3041 RVA: 0x00051E09 File Offset: 0x00050009
	// (set) Token: 0x06000BE2 RID: 3042 RVA: 0x00051E1F File Offset: 0x0005001F
	public bool SelectionActive
	{
		get
		{
			return SelectionBoxManager.Instance.IsActive("Selection", "SingleInstance");
		}
		set
		{
			if (this.SelectionActive != value)
			{
				SelectionBox selectionBox = this.SelectionBox;
				SelectionBoxManager.Instance.SetActive("Selection", "SingleInstance", value);
			}
		}
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x06000BE3 RID: 3043 RVA: 0x00051E46 File Offset: 0x00050046
	// (set) Token: 0x06000BE4 RID: 3044 RVA: 0x00051E50 File Offset: 0x00050050
	public int SelectionLockMode
	{
		get
		{
			return this.m_iSelectionLockMode;
		}
		set
		{
			if (this.m_iSelectionLockMode != value)
			{
				this.m_iSelectionLockMode = value;
				this.SelectionBox.SetVisible(this.SelectionActive);
				Color c = BlockToolSelection.colInactive;
				if (this.m_iSelectionLockMode == 1)
				{
					c = new Color(0.5f, 0f, 1f, 0.5f);
					this.removeBlockPreview();
				}
				else if (this.m_iSelectionLockMode == 2)
				{
					c = BlockToolSelection.colActive;
				}
				else
				{
					this.removeBlockPreview();
				}
				this.SelectionBox.SetAllFacesColor(c, true);
			}
		}
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x00051ED4 File Offset: 0x000500D4
	public Vector3i SelectionMin
	{
		get
		{
			return new Vector3i(Utils.FastMin(this.SelectionStart.x, this.SelectionEnd.x), Utils.FastMin(this.SelectionStart.y, this.SelectionEnd.y), Utils.FastMin(this.SelectionStart.z, this.SelectionEnd.z));
		}
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00051F37 File Offset: 0x00050137
	// (set) Token: 0x06000BE7 RID: 3047 RVA: 0x00051F3F File Offset: 0x0005013F
	public Vector3i SelectionStart
	{
		get
		{
			return this.m_selectionStartPoint;
		}
		set
		{
			if (!this.m_selectionStartPoint.Equals(value))
			{
				this.m_selectionStartPoint = value;
				this.updateSelection();
			}
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x06000BE8 RID: 3048 RVA: 0x00051F5C File Offset: 0x0005015C
	// (set) Token: 0x06000BE9 RID: 3049 RVA: 0x00051F64 File Offset: 0x00050164
	public Vector3i SelectionEnd
	{
		get
		{
			return this.m_SelectionEndPoint;
		}
		set
		{
			if (!this.m_SelectionEndPoint.Equals(value))
			{
				this.m_SelectionEndPoint = value;
				this.updateSelection();
			}
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x06000BEA RID: 3050 RVA: 0x00051F84 File Offset: 0x00050184
	public Vector3i SelectionSize
	{
		get
		{
			return new Vector3i(Mathf.Abs(this.m_selectionStartPoint.x - this.m_SelectionEndPoint.x) + 1, Mathf.Abs(this.m_selectionStartPoint.y - this.m_SelectionEndPoint.y) + 1, Mathf.Abs(this.m_selectionStartPoint.z - this.m_SelectionEndPoint.z) + 1);
		}
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x00002914 File Offset: 0x00000B14
	public void SelectionSizeSet(Vector3i _size)
	{
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00051FF0 File Offset: 0x000501F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateSelection()
	{
		Vector3 v = this.SelectionSize.ToVector3();
		Vector3 v2 = new Vector3((float)Mathf.Min(this.m_selectionStartPoint.x, this.m_SelectionEndPoint.x), (float)Mathf.Min(this.m_selectionStartPoint.y, this.m_SelectionEndPoint.y), (float)Mathf.Min(this.m_selectionStartPoint.z, this.m_SelectionEndPoint.z));
		this.SelectionBox.SetPositionAndSize(new Vector3i(v2), new Vector3i(v));
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x00052084 File Offset: 0x00050284
	[PublicizedFrom(EAccessModifier.Private)]
	public bool setBlock(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		BlockValue block = GameManager.Instance.World.GetBlock(_clrIdx, _blockPos);
		if (block.rawData == _blockValue.rawData)
		{
			return false;
		}
		TextureFullArray textureFullArray = GameManager.Instance.World.GetTextureFullArray(_blockPos.x, _blockPos.y, _blockPos.z);
		this.undoQueue.Add(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_clrIdx, _blockPos, block, MarchingCubes.DensityAir, textureFullArray)
		});
		if (this.undoQueue.Count > 100)
		{
			this.undoQueue.RemoveAt(0);
		}
		if (_blockValue.Block.shape.IsTerrain())
		{
			GameManager.Instance.World.SetBlockRPC(_clrIdx, _blockPos, _blockValue, MarchingCubes.DensityTerrain);
		}
		else
		{
			GameManager.Instance.World.SetBlockRPC(_clrIdx, _blockPos, _blockValue);
		}
		return true;
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x00052154 File Offset: 0x00050354
	[PublicizedFrom(EAccessModifier.Private)]
	public void addToUndo(int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue)
	{
		TextureFullArray textureFullArray = GameManager.Instance.World.GetTextureFullArray(_blockPos.x, _blockPos.y, _blockPos.z);
		this.undoQueue.Add(new List<BlockChangeInfo>
		{
			new BlockChangeInfo(_clrIdx, _blockPos, _oldBlockValue, MarchingCubes.DensityAir, textureFullArray)
		});
		if (this.undoQueue.Count > 100)
		{
			this.undoQueue.RemoveAt(0);
		}
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x000521C4 File Offset: 0x000503C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void blockUndoRedo(bool _redo)
	{
		List<List<BlockChangeInfo>> list = _redo ? this.redoQueue : this.undoQueue;
		if (list.Count == 0)
		{
			return;
		}
		List<BlockChangeInfo> list2 = list[list.Count - 1];
		this.BeginUndo(list2[0].clrIdx);
		GameManager.Instance.SetBlocksRPC(list2, null);
		list.RemoveAt(list.Count - 1);
		this.EndUndo(list2[0].clrIdx, !_redo);
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x00052240 File Offset: 0x00050440
	public void BeginUndo(int _clrIdx)
	{
		this.undoChanges = new List<BlockChangeInfo>();
		this.undoClrIdx = _clrIdx;
		ChunkCluster chunkCluster = GameManager.Instance.World.ChunkClusters[_clrIdx];
		if (chunkCluster != null)
		{
			chunkCluster.OnBlockChangedDelegates += this.undoBlockChangeDelegate;
		}
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0005228A File Offset: 0x0005048A
	[PublicizedFrom(EAccessModifier.Private)]
	public void undoBlockChangeDelegate(Vector3i pos, BlockValue bvOld, sbyte oldDens, TextureFullArray oldTex, BlockValue bvNew)
	{
		if (this.undoChanges != null && !bvOld.ischild)
		{
			this.undoChanges.Add(new BlockChangeInfo(this.undoClrIdx, pos, bvOld, oldDens, oldTex));
		}
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x000522B8 File Offset: 0x000504B8
	public void EndUndo(int _clrIdx, bool _bRedo = false)
	{
		ChunkCluster chunkCluster = GameManager.Instance.World.ChunkClusters[_clrIdx];
		if (chunkCluster != null)
		{
			chunkCluster.OnBlockChangedDelegates -= this.undoBlockChangeDelegate;
		}
		if (this.undoChanges.Count <= 0)
		{
			this.undoChanges = null;
			return;
		}
		this.undoChanges.Reverse();
		List<List<BlockChangeInfo>> list = _bRedo ? this.redoQueue : this.undoQueue;
		list.Add(this.undoChanges);
		if (list.Count > 100)
		{
			list.RemoveAt(0);
		}
		this.undoChanges = null;
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x00052347 File Offset: 0x00050547
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i blockCopy(Prefab _storage)
	{
		return _storage.CopyFromWorldWithEntities(GameManager.Instance.World, this.SelectionStart, this.SelectionEnd, null);
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x00052368 File Offset: 0x00050568
	[PublicizedFrom(EAccessModifier.Private)]
	public void blockPaste(int _clrIdx, Vector3i _destPos, Prefab _storage)
	{
		this.BeginUndo(0);
		_storage.CopyIntoRPC(GameManager.Instance, _destPos, this.copyPasteAirBlocks);
		this.SelectionActive = true;
		this.SelectionStart = _destPos;
		this.SelectionEnd = _destPos + _storage.size - Vector3i.one;
		this.EndUndo(0, false);
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x000523C0 File Offset: 0x000505C0
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i sizeFromPositions(Vector3i _posStart, Vector3i _posEnd)
	{
		Vector3i vector3i = new Vector3i(Math.Min(_posStart.x, _posEnd.x), Math.Min(_posStart.y, _posEnd.y), Math.Min(_posStart.z, _posEnd.z));
		Vector3i vector3i2 = new Vector3i(Math.Max(_posStart.x, _posEnd.x), Math.Max(_posStart.y, _posEnd.y), Math.Max(_posStart.z, _posEnd.z));
		return new Vector3i(Math.Abs(vector3i2.x - vector3i.x) + 1, Math.Abs(vector3i2.y - vector3i.y) + 1, Math.Abs(vector3i2.z - vector3i.z) + 1);
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00052484 File Offset: 0x00050684
	[PublicizedFrom(EAccessModifier.Private)]
	public void union(Vector3i _pos1Start, Vector3i _pos1End, Vector3i _pos2Start, Vector3i _pos2End, out Vector3i _unionStart, out Vector3i _unionEnd)
	{
		_unionStart = new Vector3i(Utils.FastMin(_pos1Start.x, _pos1End.x, _pos2Start.x, _pos2End.x), Utils.FastMin(_pos1Start.y, _pos1End.y, _pos2Start.y, _pos2End.y), Utils.FastMin(_pos1Start.z, _pos1End.z, _pos2Start.z, _pos2End.z));
		_unionEnd = new Vector3i(Utils.FastMax(_pos1Start.x, _pos1End.x, _pos2Start.x, _pos2End.x), Utils.FastMax(_pos1Start.y, _pos1End.y, _pos2Start.y, _pos2End.y), Utils.FastMax(_pos1Start.z, _pos1End.z, _pos2Start.z, _pos2End.z));
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x0005255D File Offset: 0x0005075D
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPrefabActive()
	{
		return GameManager.Instance.GetDynamicPrefabDecorator() != null && GameManager.Instance.GetDynamicPrefabDecorator().ActivePrefab != null;
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x0005257F File Offset: 0x0005077F
	public bool OnSelectionBoxActivated(string _category, string _name, bool _bActivated)
	{
		this.SelectionBox.SetVisible(_bActivated);
		if (!_bActivated)
		{
			this.SelectionLockMode = 0;
		}
		return true;
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x00052598 File Offset: 0x00050798
	public void OnSelectionBoxMoved(string _category, string _name, Vector3 _moveVector)
	{
		Vector3i other = new Vector3i(_moveVector);
		Vector3i zero = Vector3i.zero;
		int selectionLockMode = this.SelectionLockMode;
		this.SelectionStart += other;
		this.SelectionEnd += other;
		this.selectionRotCenter += other.ToVector3();
		if (this.SelectionLockMode == 2)
		{
			this.previewGORot1.transform.position += _moveVector;
		}
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00052620 File Offset: 0x00050820
	public void OnSelectionBoxSized(string _category, string _name, int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest)
	{
		if (this.SelectionLockMode == 2)
		{
			this.SelectionLockMode = 0;
			return;
		}
		if (_dEast != 0 && (_dEast >= 0 || this.SelectionSize.x > 1))
		{
			if (this.SelectionEnd.x > this.SelectionStart.x)
			{
				this.SelectionEnd = new Vector3i(this.SelectionEnd.x + _dEast, this.SelectionEnd.y, this.SelectionEnd.z);
			}
			else
			{
				this.SelectionStart = new Vector3i(this.SelectionStart.x + _dEast, this.SelectionStart.y, this.SelectionStart.z);
			}
		}
		if (_dWest != 0 && (_dWest >= 0 || this.SelectionSize.x > 1))
		{
			if (this.SelectionEnd.x <= this.SelectionStart.x)
			{
				this.SelectionEnd = new Vector3i(this.SelectionEnd.x - _dWest, this.SelectionEnd.y, this.SelectionEnd.z);
			}
			else
			{
				this.SelectionStart = new Vector3i(this.SelectionStart.x - _dWest, this.SelectionStart.y, this.SelectionStart.z);
			}
		}
		if (_dTop != 0 && (_dTop >= 0 || this.SelectionSize.y > 1))
		{
			if (this.SelectionEnd.y > this.SelectionStart.y)
			{
				this.SelectionEnd = new Vector3i(this.SelectionEnd.x, this.SelectionEnd.y + _dTop, this.SelectionEnd.z);
			}
			else
			{
				this.SelectionStart = new Vector3i(this.SelectionStart.x, this.SelectionStart.y + _dTop, this.SelectionStart.z);
			}
		}
		if (_dBottom != 0 && (_dBottom >= 0 || this.SelectionSize.y > 1))
		{
			if (this.SelectionEnd.y <= this.SelectionStart.y)
			{
				this.SelectionEnd = new Vector3i(this.SelectionEnd.x, this.SelectionEnd.y - _dBottom, this.SelectionEnd.z);
			}
			else
			{
				this.SelectionStart = new Vector3i(this.SelectionStart.x, this.SelectionStart.y - _dBottom, this.SelectionStart.z);
			}
		}
		if (_dNorth != 0 && (_dNorth >= 0 || this.SelectionSize.z > 1))
		{
			if (this.SelectionEnd.z > this.SelectionStart.z)
			{
				this.SelectionEnd = new Vector3i(this.SelectionEnd.x, this.SelectionEnd.y, this.SelectionEnd.z + _dNorth);
			}
			else
			{
				this.SelectionStart = new Vector3i(this.SelectionStart.x, this.SelectionStart.y, this.SelectionStart.z + _dNorth);
			}
		}
		if (_dSouth != 0 && (_dSouth >= 0 || this.SelectionSize.z > 1))
		{
			if (this.SelectionEnd.z <= this.SelectionStart.z)
			{
				this.SelectionEnd = new Vector3i(this.SelectionEnd.x, this.SelectionEnd.y, this.SelectionEnd.z - _dSouth);
				return;
			}
			this.SelectionStart = new Vector3i(this.SelectionStart.x, this.SelectionStart.y, this.SelectionStart.z - _dSouth);
		}
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x000529A8 File Offset: 0x00050BA8
	public void OnSelectionBoxMirrored(Vector3i _selAxis)
	{
		EnumMirrorAlong axis = EnumMirrorAlong.XAxis;
		if (_selAxis.y != 0)
		{
			axis = EnumMirrorAlong.YAxis;
		}
		else if (_selAxis.z != 0)
		{
			axis = EnumMirrorAlong.ZAxis;
		}
		if (this.previewGORot3 != null && this.previewGORot3.transform.childCount > 0)
		{
			this.clipboard.Mirror(axis);
			this.removeBlockPreview();
			this.createBlockPreviewFrom(this.clipboard);
			return;
		}
		Prefab prefab = new Prefab();
		prefab.CopyFromWorldWithEntities(GameManager.Instance.World, this.SelectionStart, this.SelectionEnd, null);
		prefab.Mirror(axis);
		prefab.CopyIntoRPC(GameManager.Instance, this.SelectionMin, this.copyPasteAirBlocks);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool OnSelectionBoxDelete(string _category, string _name)
	{
		return false;
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00052A4D File Offset: 0x00050C4D
	public bool OnSelectionBoxIsAvailable(string _category, EnumSelectionBoxAvailabilities _criteria)
	{
		return _criteria == EnumSelectionBoxAvailabilities.CanResize || _criteria == EnumSelectionBoxAvailabilities.CanMirror;
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnSelectionBoxShowProperties(bool _bVisible, GUIWindowManager _windowManager)
	{
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00052A59 File Offset: 0x00050C59
	public void OnSelectionBoxRotated(string _category, string _name)
	{
		if (this.SelectionLockMode == 2)
		{
			this.rotatePreviewAroundY();
			return;
		}
		this.rotateSelectionAroundY();
	}

	// Token: 0x06000C00 RID: 3072 RVA: 0x00052A74 File Offset: 0x00050C74
	public string GetDebugOutput()
	{
		if (this.SelectionActive)
		{
			return string.Format("Selection pos/size: {0}/{1}", this.SelectionStart.ToString(), this.SelectionSize.ToString());
		}
		return "-";
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00052AC1 File Offset: 0x00050CC1
	public Dictionary<string, NGuiAction> GetActions()
	{
		return this.actions;
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00052ACC File Offset: 0x00050CCC
	public void LoadPrefabIntoClipboard(Prefab _prefab)
	{
		this.clipboard = _prefab;
		this.SelectionLockMode = 2;
		if (this.SelectionSize != this.clipboard.size)
		{
			this.SelectionEnd = this.SelectionStart + this.clipboard.size - Vector3i.one;
		}
		this.SelectionActive = true;
		this.createBlockPreviewFrom(this.clipboard);
	}

	// Token: 0x04000A13 RID: 2579
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cBlockUndoRedoCount = 100;

	// Token: 0x04000A14 RID: 2580
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color colActive = new Color(1f, 0f, 0f, 0.5f);

	// Token: 0x04000A15 RID: 2581
	[PublicizedFrom(EAccessModifier.Private)]
	public static Color colInactive = new Color(0f, 0f, 1f, 0.5f);

	// Token: 0x04000A16 RID: 2582
	public static BlockToolSelection Instance;

	// Token: 0x04000A17 RID: 2583
	public Prefab clipboard = new Prefab();

	// Token: 0x04000A18 RID: 2584
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i m_selectionStartPoint;

	// Token: 0x04000A19 RID: 2585
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i m_SelectionEndPoint;

	// Token: 0x04000A1A RID: 2586
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iSelectionLockMode;

	// Token: 0x04000A1B RID: 2587
	[PublicizedFrom(EAccessModifier.Private)]
	public List<List<BlockChangeInfo>> undoQueue = new List<List<BlockChangeInfo>>();

	// Token: 0x04000A1C RID: 2588
	[PublicizedFrom(EAccessModifier.Private)]
	public List<List<BlockChangeInfo>> redoQueue = new List<List<BlockChangeInfo>>();

	// Token: 0x04000A1D RID: 2589
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastBuildTime;

	// Token: 0x04000A1E RID: 2590
	[PublicizedFrom(EAccessModifier.Private)]
	public const string SelectionBoxName = "SingleInstance";

	// Token: 0x04000A1F RID: 2591
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, NGuiAction> actions;

	// Token: 0x04000A20 RID: 2592
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject previewGOParent;

	// Token: 0x04000A21 RID: 2593
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject previewGORot1;

	// Token: 0x04000A22 RID: 2594
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject previewGORot2;

	// Token: 0x04000A23 RID: 2595
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject previewGORot3;

	// Token: 0x04000A24 RID: 2596
	[PublicizedFrom(EAccessModifier.Private)]
	public bool copyPasteAirBlocks = true;

	// Token: 0x04000A25 RID: 2597
	public PlayerActionsLocal playerInput;

	// Token: 0x04000A26 RID: 2598
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 selectionRotCenter;

	// Token: 0x04000A27 RID: 2599
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 offsetToMin;

	// Token: 0x04000A28 RID: 2600
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 offsetToMax;

	// Token: 0x04000A29 RID: 2601
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldRayHitInfo hitInfo;

	// Token: 0x04000A2A RID: 2602
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bWaitForRelease;

	// Token: 0x04000A2B RID: 2603
	public int SelectionClrIdx;

	// Token: 0x04000A2C RID: 2604
	[PublicizedFrom(EAccessModifier.Private)]
	public List<BlockChangeInfo> undoChanges = new List<BlockChangeInfo>();

	// Token: 0x04000A2D RID: 2605
	[PublicizedFrom(EAccessModifier.Private)]
	public int undoClrIdx;
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200010A RID: 266
[Preserve]
public class BlockLight : Block
{
	// Token: 0x17000077 RID: 119
	// (get) Token: 0x06000714 RID: 1812 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000715 RID: 1813 RVA: 0x00031E34 File Offset: 0x00030034
	public BlockLight()
	{
		this.HasTileEntity = true;
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x00031EA8 File Offset: 0x000300A8
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("RuntimeSwitch"))
		{
			this.isRuntimeSwitch = StringParsers.ParseBool(base.Properties.Values["RuntimeSwitch"], 0, -1, true);
		}
		if (base.Properties.Values.ContainsKey("Model"))
		{
			DataLoader.PreloadBundle(base.Properties.Values["Model"]);
		}
		base.Properties.ParseBool("IgnoreLightsOff", ref this.ignoreLightsOff);
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x00030168 File Offset: 0x0002E368
	public override byte GetLightValue(BlockValue _blockValue)
	{
		if ((_blockValue.meta & 2) == 0)
		{
			return 0;
		}
		return base.GetLightValue(_blockValue);
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x00031F3C File Offset: 0x0003013C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor())
		{
			return null;
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		if ((_blockValue.meta & 2) != 0)
		{
			return string.Format(Localization.Get("useSwitchLightOff", false), arg);
		}
		return string.Format(Localization.Get("useSwitchLightOn", false), arg);
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x00031FB8 File Offset: 0x000301B8
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!(_commandName == "light"))
		{
			if (!(_commandName == "edit"))
			{
				if (_commandName == "trigger")
				{
					XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, false, true);
				}
			}
			else
			{
				TileEntityLight te = (TileEntityLight)_world.GetTileEntity(_cIdx, _blockPos);
				if (_world.IsEditor())
				{
					XUiC_LightEditor.Open(_player.PlayerUI, te, _blockPos, _world as World, _cIdx, this);
					return true;
				}
			}
		}
		else if (_world.IsEditor() && this.updateLightState(_world, _cIdx, _blockPos, _blockValue, true, false))
		{
			return true;
		}
		return false;
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00032050 File Offset: 0x00030250
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateLightState(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool _bSwitchLight = false, bool _enableState = true)
	{
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		if (chunkCluster == null)
		{
			return false;
		}
		IChunk chunkSync = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
		if (chunkSync == null)
		{
			return false;
		}
		BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return false;
		}
		bool flag = (_blockValue.meta & 2) > 0;
		TileEntityLight tileEntityLight = (TileEntityLight)_world.GetTileEntity(_cIdx, _blockPos);
		if (_bSwitchLight)
		{
			flag = !flag;
			_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (flag ? 2 : 0));
			_world.SetBlockRPC(_cIdx, _blockPos, _blockValue);
		}
		Transform transform = blockEntity.transform.FindInChildren("MainLight");
		if (transform)
		{
			LightLOD component = transform.GetComponent<LightLOD>();
			if (component)
			{
				component.SwitchOnOff(flag, false);
				component.SetBlockEntityData(blockEntity);
				Light light = component.GetLight();
				if (tileEntityLight != null)
				{
					light.type = tileEntityLight.LightType;
					component.MaxIntensity = tileEntityLight.LightIntensity;
					light.color = tileEntityLight.LightColor;
					light.shadows = tileEntityLight.LightShadows;
					component.LightAngle = tileEntityLight.LightAngle;
					component.LightStateType = tileEntityLight.LightState;
					component.StateRate = tileEntityLight.Rate;
					component.FluxDelay = tileEntityLight.Delay;
					component.SetRange(tileEntityLight.LightRange);
					component.SetEmissiveColor(component.bSwitchedOn);
				}
				else
				{
					GameObject gameObject = DataLoader.LoadAsset<GameObject>(base.Properties.Values["Model"], false);
					if (gameObject != null)
					{
						Transform transform2 = gameObject.transform.Find("MainLight");
						if (transform2 != null)
						{
							LightLOD component2 = transform2.GetComponent<LightLOD>();
							Light light2 = component2.GetLight();
							if (light != null && light2 != null)
							{
								light.type = light2.type;
								component.MaxIntensity = light2.intensity;
								light.color = light2.color;
								light.shadows = light2.shadows;
								component.LightAngle = light2.spotAngle;
								component.LightStateType = component2.LightStateType;
								component.StateRate = component2.StateRate;
								component.FluxDelay = component2.FluxDelay;
								component.SetRange(light2.range);
								component.SetEmissiveColor(component.bSwitchedOn);
							}
						}
					}
				}
			}
		}
		transform = blockEntity.transform.Find("Point light");
		if (transform)
		{
			LightLOD component3 = transform.GetComponent<LightLOD>();
			if (component3)
			{
				component3.SwitchOnOff(flag, false);
				component3.SetBlockEntityData(blockEntity);
			}
		}
		return true;
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x00032320 File Offset: 0x00030520
	public bool IsLightOn(BlockValue _blockValue)
	{
		return (_blockValue.meta & 2) > 0;
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x0003232E File Offset: 0x0003052E
	public bool OriginalLightState(BlockValue _blockValue)
	{
		return !this.ignoreLightsOff && (_blockValue.meta & 1) > 0;
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x00032346 File Offset: 0x00030546
	public BlockValue SetLightState(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, bool isOn)
	{
		_blockValue.meta = (byte)(((int)_blockValue.meta & -3) | (isOn ? 2 : 0));
		return _blockValue;
	}

	// Token: 0x0600071E RID: 1822 RVA: 0x00032365 File Offset: 0x00030565
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		this.updateLightState(_world, _clrIdx, _blockPos, _newBlockValue, false, true);
	}

	// Token: 0x0600071F RID: 1823 RVA: 0x00032385 File Offset: 0x00030585
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		this.updateLightState(_world, _cIdx, _blockPos, _blockValue, false, true);
	}

	// Token: 0x06000720 RID: 1824 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000721 RID: 1825 RVA: 0x000323A4 File Offset: 0x000305A4
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		bool flag = false;
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster != null)
		{
			IChunk chunkSync = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z));
			if (chunkSync != null)
			{
				BlockEntityData blockEntity = chunkSync.GetBlockEntity(_blockPos);
				if (blockEntity != null && blockEntity.bHasTransform)
				{
					Transform transform = blockEntity.transform.Find("MainLight");
					if (transform != null)
					{
						LightLOD component = transform.GetComponent<LightLOD>();
						if (component != null && component.GetLight() != null)
						{
							flag = true;
						}
					}
				}
			}
		}
		this.cmds[0].enabled = (_world.IsEditor() || this.isRuntimeSwitch);
		this.cmds[1].enabled = (_world.IsEditor() && flag);
		this.cmds[2].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x06000722 RID: 1826 RVA: 0x000324AD File Offset: 0x000306AD
	public TileEntityLight CreateTileEntity(Chunk chunk)
	{
		return new TileEntityLight(chunk);
	}

	// Token: 0x06000723 RID: 1827 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsTileEntitySavedInPrefab()
	{
		return true;
	}

	// Token: 0x06000724 RID: 1828 RVA: 0x000324B8 File Offset: 0x000306B8
	public override void OnBlockAdded(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs _addedByPlayer)
	{
		base.OnBlockAdded(_world, _chunk, _blockPos, _blockValue, _addedByPlayer);
		if (_blockValue.ischild)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			bool flag = this.IsLightOn(_blockValue);
			if (this.OriginalLightState(_blockValue) != flag)
			{
				_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (flag ? 1 : 0));
				_world.SetBlockRPC(_chunk.ClrIdx, _blockPos, _blockValue);
			}
		}
	}

	// Token: 0x06000725 RID: 1829 RVA: 0x00032528 File Offset: 0x00030728
	public override void OnBlockReset(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		if (_blockValue.ischild)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			bool flag = this.IsLightOn(_blockValue);
			if (this.OriginalLightState(_blockValue) != flag)
			{
				_blockValue.meta = (byte)(((int)_blockValue.meta & -2) | (flag ? 1 : 0));
				_world.SetBlockRPC(_chunk.ClrIdx, _blockPos, _blockValue);
			}
		}
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x00032588 File Offset: 0x00030788
	public override void OnTriggered(EntityPlayer _player, WorldBase _world, int cIdx, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy)
	{
		base.OnTriggered(_player, _world, cIdx, _blockPos, _blockValue, _blockChanges, _triggeredBy);
		_blockValue = this.SetLightState(_world, cIdx, _blockPos, _blockValue, !this.IsLightOn(_blockValue));
		_blockChanges.Add(new BlockChangeInfo(cIdx, _blockPos, _blockValue));
	}

	// Token: 0x040007E5 RID: 2021
	public const int cMetaOriginalState = 1;

	// Token: 0x040007E6 RID: 2022
	public const int cMetaOn = 2;

	// Token: 0x040007E7 RID: 2023
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isRuntimeSwitch;

	// Token: 0x040007E8 RID: 2024
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ignoreLightsOff;

	// Token: 0x040007E9 RID: 2025
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("light", "electric_switch", true, false, null),
		new BlockActivationCommand("edit", "tool", true, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}

using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200012C RID: 300
[Preserve]
public class BlockQuestActivate : Block
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x0600085F RID: 2143 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowBlockTriggers
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0003A548 File Offset: 0x00038748
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		if (!_world.IsEditor())
		{
			if (_blockValue.meta2 != 1)
			{
				return "";
			}
			if (!QuestEventManager.Current.ActiveQuestBlocks.Contains(_blockPos))
			{
				return "";
			}
		}
		PlayerActionsLocal playerInput = ((EntityPlayerLocal)_entityFocusing).playerInput;
		string arg = playerInput.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null) + playerInput.PermanentActions.Activate.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		string localizedBlockName = _blockValue.Block.GetLocalizedBlockName();
		return string.Format(Localization.Get("questBlockActivate", false), arg, localizedBlockName);
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x0003A5DC File Offset: 0x000387DC
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (!_world.IsEditor() && _blockValue.meta2 != 1)
		{
			return true;
		}
		if (!(_commandName == "activate"))
		{
			if (!(_commandName == "trigger"))
			{
				return false;
			}
			XUiC_TriggerProperties.Show(_player.PlayerUI.xui, _cIdx, _blockPos, true, false);
			return true;
		}
		else
		{
			if (!QuestEventManager.Current.ActiveQuestBlocks.Contains(_blockPos))
			{
				return false;
			}
			if (this.activateTime > 0f)
			{
				LocalPlayerUI playerUI = _player.PlayerUI;
				playerUI.windowManager.Open("timer", true, false, true);
				XUiC_Timer childByType = playerUI.xui.GetChildByType<XUiC_Timer>();
				TimerEventData timerEventData = new TimerEventData();
				timerEventData.Data = new object[]
				{
					_cIdx,
					_blockValue,
					_blockPos,
					_player
				};
				timerEventData.CloseOnHit = true;
				timerEventData.Event += this.EventData_Event;
				if ((_blockValue.meta & 1) == 0)
				{
					timerEventData.alternateTime = _player.rand.RandomRange(this.activateTime * 0.25f, this.activateTime * 0.5f);
					timerEventData.AlternateEvent += this.EventData_AlternateEvent;
				}
				timerEventData.CloseEvent += this.EventData_CloseEvent;
				childByType.SetTimer(this.activateTime, timerEventData, -1f, "");
				Manager.BroadcastPlay(_blockPos.ToVector3(), "generator_start", 0f);
				_blockValue.meta2 = 2;
				(_world as World).SetBlockRPC(_cIdx, _blockPos, _blockValue);
				this.setControllerState(_world, _cIdx, null, _blockPos, QuestGeneratorController.GeneratorStates.RebootState, false);
			}
			else
			{
				QuestEventManager.Current.BlockActivated(_blockValue.Block.GetBlockName(), _blockPos);
				base.HandleTrigger(_player, (World)_world, _cIdx, _blockPos, _blockValue);
				Manager.BroadcastPlay(_blockPos.ToVector3(), "generator_start", 0f);
				_blockValue.meta2 = 4;
				(_world as World).SetBlockRPC(_cIdx, _blockPos, _blockValue);
			}
			return true;
		}
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x0003A7D8 File Offset: 0x000389D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void EventData_AlternateEvent(TimerEventData timerData)
	{
		object[] array = (object[])timerData.Data;
		int clrIdx = (int)array[0];
		BlockValue blockValue = (BlockValue)array[1];
		Vector3i vector3i = (Vector3i)array[2];
		EntityPlayer entityPlayer = (EntityPlayer)array[3];
		WorldBase world = GameManager.Instance.World;
		GameEventManager.Current.HandleAction("quest_restorepower_generator", entityPlayer, entityPlayer, false, vector3i, "", "", false, true, "", null);
		blockValue.meta |= 1;
		blockValue.meta2 = 1;
		world.SetBlockRPC(clrIdx, vector3i, blockValue);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0003A86C File Offset: 0x00038A6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void EventData_CloseEvent(TimerEventData timerData)
	{
		object[] array = (object[])timerData.Data;
		int clrIdx = (int)array[0];
		BlockValue blockValue = (BlockValue)array[1];
		Vector3i blockPos = (Vector3i)array[2];
		World world = GameManager.Instance.World;
		blockValue.meta2 = 1;
		world.SetBlockRPC(clrIdx, blockPos, blockValue);
		this.setControllerState(world, clrIdx, null, blockPos, QuestGeneratorController.GeneratorStates.Off, false);
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x0003A8C8 File Offset: 0x00038AC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void EventData_Event(TimerEventData timerData)
	{
		object[] array = (object[])timerData.Data;
		int num = (int)array[0];
		BlockValue blockValue = (BlockValue)array[1];
		Vector3i blockPos = (Vector3i)array[2];
		EntityPlayerLocal player = array[3] as EntityPlayerLocal;
		World world = GameManager.Instance.World;
		QuestEventManager.Current.BlockActivated(blockValue.Block.GetBlockName(), blockPos);
		base.HandleTrigger(player, world, num, blockPos, blockValue);
		blockValue.meta2 = 4;
		world.SetBlockRPC(num, blockPos, blockValue);
		this.setControllerState(world, num, null, blockPos, QuestGeneratorController.GeneratorStates.On, false);
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x0003A951 File Offset: 0x00038B51
	public override void OnBlockRemoved(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockRemoved(_world, _chunk, _blockPos, _blockValue);
		QuestEventManager.Current.BlockDestroyed(_blockValue.Block, _blockPos, null);
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x0003A974 File Offset: 0x00038B74
	public override void OnBlockValueChanged(WorldBase _world, Chunk _chunk, int _clrIdx, Vector3i _blockPos, BlockValue _oldBlockValue, BlockValue _newBlockValue)
	{
		base.OnBlockValueChanged(_world, _chunk, _clrIdx, _blockPos, _oldBlockValue, _newBlockValue);
		QuestGeneratorController.GeneratorStates meta = (QuestGeneratorController.GeneratorStates)_newBlockValue.meta2;
		if (meta == QuestGeneratorController.GeneratorStates.On)
		{
			QuestEventManager.Current.BlockActivated(_newBlockValue.Block.GetBlockName(), _blockPos);
		}
		this.setControllerState(_world, _clrIdx, _chunk, _blockPos, meta, false);
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x0003A9C4 File Offset: 0x00038BC4
	public override void Init()
	{
		base.Init();
		base.Properties.ParseFloat(BlockQuestActivate.PropActivateTime, ref this.activateTime);
		if (base.Properties.Values.ContainsKey(BlockQuestActivate.PropQuestTags))
		{
			this.ValidQuestTags = FastTags<TagGroup.Global>.Parse(base.Properties.Values[BlockQuestActivate.PropQuestTags]);
		}
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x0003AA24 File Offset: 0x00038C24
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		this.cmds[0].enabled = !_world.IsEditor();
		this.cmds[1].enabled = (_world.IsEditor() && !GameUtils.IsWorldEditor());
		return this.cmds;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x0003AA78 File Offset: 0x00038C78
	public override void OnTriggerAddedFromPrefab(BlockTrigger _trigger, Vector3i _blockPos, BlockValue _blockValue, FastTags<TagGroup.Global> _questTags)
	{
		if (GameManager.Instance.World.IsEditor())
		{
			return;
		}
		World world = GameManager.Instance.World;
		base.OnTriggerAddedFromPrefab(_trigger, _blockPos, _blockValue, _questTags);
		if (!this.ValidQuestTags.IsEmpty)
		{
			if (_questTags.IsEmpty)
			{
				_blockValue.meta2 = 0;
				world.SetBlock(_trigger.Chunk.ClrIdx, _trigger.ToWorldPos(), _blockValue, true, false);
				this.setControllerState(world, _trigger.Chunk.ClrIdx, _trigger.Chunk, _trigger.LocalChunkPos, QuestGeneratorController.GeneratorStates.OnNoQuest, false);
				return;
			}
			if (!_questTags.Test_AnySet(this.ValidQuestTags))
			{
				_blockValue.meta2 = 0;
				world.SetBlock(_trigger.Chunk.ClrIdx, _trigger.ToWorldPos(), _blockValue, true, false);
				this.setControllerState(world, _trigger.Chunk.ClrIdx, _trigger.Chunk, _trigger.LocalChunkPos, QuestGeneratorController.GeneratorStates.OnNoQuest, false);
			}
		}
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x0003AB5C File Offset: 0x00038D5C
	public override void OnBlockLoaded(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue)
	{
		base.OnBlockLoaded(_world, _clrIdx, _blockPos, _blockValue);
		ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
		if (chunkCluster == null)
		{
			return;
		}
		Chunk chunk = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z)) as Chunk;
		if (chunk == null)
		{
			return;
		}
		BlockTrigger blockTrigger = chunk.GetBlockTrigger(World.toBlock(_blockPos));
		if (blockTrigger != null)
		{
			GameManager.Instance.StartCoroutine(this.resetTriggerLater(blockTrigger, _blockValue, FastTags<TagGroup.Global>.none));
		}
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x0003ABE0 File Offset: 0x00038DE0
	public override void OnBlockEntityTransformAfterActivated(WorldBase _world, Vector3i _blockPos, int _cIdx, BlockValue _blockValue, BlockEntityData _ebcd)
	{
		base.OnBlockEntityTransformAfterActivated(_world, _blockPos, _cIdx, _blockValue, _ebcd);
		ChunkCluster chunkCluster = _world.ChunkClusters[_cIdx];
		if (chunkCluster == null)
		{
			return;
		}
		Chunk chunk = chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z)) as Chunk;
		if (chunk == null)
		{
			return;
		}
		QuestGeneratorController.GeneratorStates meta = (QuestGeneratorController.GeneratorStates)_blockValue.meta2;
		this.setControllerState(_world, _cIdx, chunk, _blockPos, meta, true);
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x0003AC50 File Offset: 0x00038E50
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator resetTriggerLater(BlockTrigger _trigger, BlockValue _blockValue, FastTags<TagGroup.Global> _questTag)
	{
		EntityPlayerLocal player = GameManager.Instance.World.GetPrimaryPlayer();
		if (player != null)
		{
			while (!player.QuestJournal.CheckRallyMarkerActivation())
			{
				yield return new WaitForSeconds(0.1f);
			}
		}
		if (!_questTag.Test_AnySet(QuestEventManager.restorePowerTag))
		{
			if (_trigger.NeedsTriggered == BlockTrigger.TriggeredStates.NotTriggered || _trigger.NeedsTriggered == BlockTrigger.TriggeredStates.NeedsTriggered)
			{
				_trigger.NeedsTriggered = BlockTrigger.TriggeredStates.NeedsTriggered;
				if (_trigger.TriggerDataOwner != null && !_trigger.TriggerDataOwner.NeedsTriggerUpdate)
				{
					_trigger.TriggerDataOwner.NeedsTriggerUpdate = true;
				}
			}
		}
		else
		{
			_trigger.NeedsTriggered = BlockTrigger.TriggeredStates.HasTriggered;
		}
		yield break;
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x0003AC66 File Offset: 0x00038E66
	public override void OnTriggerRefresh(BlockTrigger _trigger, BlockValue _bv, FastTags<TagGroup.Global> _questTag)
	{
		base.OnTriggerRefresh(_trigger, _bv, _questTag);
		GameManager.Instance.StartCoroutine(this.resetTriggerLater(_trigger, _bv, _questTag));
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x0003AC88 File Offset: 0x00038E88
	public void SetupForQuest(WorldBase _world, Chunk _chunk, Vector3i _blockPos, BlockValue _blockValue, List<BlockChangeInfo> blockChanges)
	{
		if (_world.IsEditor())
		{
			return;
		}
		if (_blockValue.ischild)
		{
			return;
		}
		_blockValue.meta2 = 1;
		blockChanges.Add(new BlockChangeInfo(_chunk.ClrIdx, _blockPos, _blockValue));
		this.setControllerState(_world, _chunk.ClrIdx, _chunk, _blockPos, QuestGeneratorController.GeneratorStates.Off, false);
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x0003ACD8 File Offset: 0x00038ED8
	[PublicizedFrom(EAccessModifier.Private)]
	public void setControllerState(WorldBase _world, int _clrIdx, Chunk _chunk, Vector3i _blockPos, QuestGeneratorController.GeneratorStates generatorState, bool isInit = false)
	{
		if (_chunk == null)
		{
			ChunkCluster chunkCluster = _world.ChunkClusters[_clrIdx];
			if (chunkCluster == null)
			{
				return;
			}
			_chunk = (chunkCluster.GetChunkSync(World.toChunkXZ(_blockPos.x), World.toChunkY(_blockPos.y), World.toChunkXZ(_blockPos.z)) as Chunk);
			if (_chunk == null)
			{
				return;
			}
		}
		BlockEntityData blockEntity = _chunk.GetBlockEntity(_blockPos);
		if (blockEntity == null || !blockEntity.bHasTransform)
		{
			return;
		}
		QuestGeneratorController component = blockEntity.transform.GetComponent<QuestGeneratorController>();
		if (component != null)
		{
			component.SetGeneratorState(generatorState, isInit);
		}
	}

	// Token: 0x0400086F RID: 2159
	public const int cMetaSpawned = 1;

	// Token: 0x04000870 RID: 2160
	[PublicizedFrom(EAccessModifier.Private)]
	public float activateTime;

	// Token: 0x04000871 RID: 2161
	public FastTags<TagGroup.Global> ValidQuestTags = FastTags<TagGroup.Global>.none;

	// Token: 0x04000872 RID: 2162
	public static string PropActivateTime = "ActivateTime";

	// Token: 0x04000873 RID: 2163
	public static string PropQuestTags = "ValidQuestTags";

	// Token: 0x04000874 RID: 2164
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("activate", "electric_switch", false, false, null),
		new BlockActivationCommand("trigger", "wrench", true, false, null)
	};
}

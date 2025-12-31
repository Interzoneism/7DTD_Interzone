using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200019A RID: 410
[Preserve]
public class PrefabTriggerData
{
	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x06000C7B RID: 3195 RVA: 0x00055052 File Offset: 0x00053252
	// (set) Token: 0x06000C7C RID: 3196 RVA: 0x00055064 File Offset: 0x00053264
	public bool NeedsTriggerUpdate
	{
		get
		{
			return this.needsTriggerTimer != -1f;
		}
		set
		{
			if (this.Owner == null)
			{
				this.Owner = this.world.triggerManager;
			}
			if (value)
			{
				this.Owner.AddToUpdateList(this);
				this.needsTriggerTimer = 3f;
				return;
			}
			this.Owner.RemoveFromUpdateList(this);
			this.needsTriggerTimer = -1f;
		}
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x000550BC File Offset: 0x000532BC
	public PrefabTriggerData(PrefabInstance instance)
	{
		this.PrefabInstance = instance;
		this.world = GameManager.Instance.World;
		this.SetupData();
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x00055124 File Offset: 0x00053324
	public void ResetData()
	{
		if (this.TriggeredLayers != null)
		{
			this.TriggeredLayers.Clear();
		}
		if (this.TriggeredByLayers != null)
		{
			this.TriggeredByLayers.Clear();
		}
		this.TriggeredByDictionary.Clear();
		this.TriggeredByVolumes.Clear();
		this.Triggers.Clear();
		this.SetupData();
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x00055180 File Offset: 0x00053380
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetupData()
	{
		bool flag = GameManager.Instance.World.IsEditor();
		HashSetLong occupiedChunks = this.PrefabInstance.GetOccupiedChunks();
		Vector3i boundingBoxSize = this.PrefabInstance.boundingBoxSize;
		Vector3i boundingBoxPosition = this.PrefabInstance.boundingBoxPosition;
		foreach (long key in occupiedChunks)
		{
			Chunk chunkSync = this.world.ChunkCache.GetChunkSync(key);
			if (chunkSync != null)
			{
				foreach (BlockTrigger blockTrigger in chunkSync.GetBlockTriggers().list)
				{
					Vector3i vector3i = blockTrigger.ToWorldPos();
					if (boundingBoxPosition.x <= vector3i.x && boundingBoxPosition.y <= vector3i.y && boundingBoxPosition.z <= vector3i.z && boundingBoxPosition.x + boundingBoxSize.x > vector3i.x && boundingBoxPosition.y + boundingBoxSize.y > vector3i.y && boundingBoxPosition.z + boundingBoxSize.z > vector3i.z)
					{
						using (List<byte>.Enumerator enumerator3 = blockTrigger.TriggeredByIndices.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								int num = (int)enumerator3.Current;
								List<BlockTrigger> list;
								if (!this.TriggeredByDictionary.TryGetValue(num, out list))
								{
									list = new List<BlockTrigger>();
									this.TriggeredByDictionary[num] = list;
								}
								list.Add(blockTrigger);
								if (flag)
								{
									if (this.TriggeredByLayers == null)
									{
										this.TriggeredByLayers = new List<byte>();
									}
									if (!this.TriggeredByLayers.Contains((byte)num))
									{
										this.TriggeredByLayers.Add((byte)num);
									}
								}
							}
						}
						using (List<byte>.Enumerator enumerator3 = blockTrigger.TriggersIndices.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								int num2 = (int)enumerator3.Current;
								if (this.TriggeredLayers == null)
								{
									this.TriggeredLayers = new List<byte>();
								}
								if (!this.TriggeredLayers.Contains((byte)num2))
								{
									this.TriggeredLayers.Add((byte)num2);
								}
							}
						}
						this.Triggers.Add(blockTrigger);
						blockTrigger.TriggerDataOwner = this;
					}
				}
			}
		}
		List<SleeperVolume> sleeperVolumes = this.PrefabInstance.sleeperVolumes;
		for (int i = 0; i < sleeperVolumes.Count; i++)
		{
			SleeperVolume sleeperVolume = sleeperVolumes[i];
			for (int j = 0; j < sleeperVolume.TriggeredByIndices.Count; j++)
			{
				this.AddTriggeredBy(sleeperVolume);
				if (flag)
				{
					if (this.TriggeredByLayers == null)
					{
						this.TriggeredByLayers = new List<byte>();
					}
					byte item = sleeperVolume.TriggeredByIndices[j];
					if (!this.TriggeredByLayers.Contains(item))
					{
						this.TriggeredByLayers.Add(item);
					}
				}
			}
		}
		this.RefreshTriggers();
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x000554DC File Offset: 0x000536DC
	public void Update(float deltaTime)
	{
		if (this.needsTriggerTimer != -1f)
		{
			this.needsTriggerTimer -= deltaTime;
			if (this.needsTriggerTimer <= 0f)
			{
				this.HandleNeedTriggers();
				this.NeedsTriggerUpdate = false;
			}
		}
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x00055514 File Offset: 0x00053714
	public void HandleNeedTriggers()
	{
		for (int i = 0; i < this.Triggers.Count; i++)
		{
			if (this.Triggers[i].NeedsTriggered == BlockTrigger.TriggeredStates.NeedsTriggered)
			{
				this.Trigger(null, this.Triggers[i]);
				this.Triggers[i].NeedsTriggered = BlockTrigger.TriggeredStates.HasTriggered;
			}
		}
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x00055570 File Offset: 0x00053770
	public void RefreshTriggers()
	{
		if (!GameManager.Instance.IsEditMode())
		{
			for (int i = 0; i < this.Triggers.Count; i++)
			{
				this.Triggers[i].Refresh(FastTags<TagGroup.Global>.none);
			}
		}
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x000555B8 File Offset: 0x000537B8
	public void RefreshTriggersForQuest(FastTags<TagGroup.Global> questTags)
	{
		if (!GameManager.Instance.IsEditMode())
		{
			for (int i = 0; i < this.Triggers.Count; i++)
			{
				this.Triggers[i].Refresh(questTags);
			}
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x000555FC File Offset: 0x000537FC
	public void ResetTriggers()
	{
		if (!GameManager.Instance.IsEditMode())
		{
			for (int i = 0; i < this.Triggers.Count; i++)
			{
				this.Triggers[i].NeedsTriggered = BlockTrigger.TriggeredStates.NotTriggered;
			}
		}
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0005563D File Offset: 0x0005383D
	public void AddPlayerInArea(int entityID)
	{
		if (!this.PlayersInArea.Contains(entityID))
		{
			this.PlayersInArea.Add(entityID);
		}
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x00055659 File Offset: 0x00053859
	public void RemovePlayerInArea(int entityID)
	{
		if (this.PlayersInArea.Contains(entityID))
		{
			this.PlayersInArea.Remove(entityID);
		}
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00055678 File Offset: 0x00053878
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AddTriggeredBy(SleeperVolume triggeredVolume)
	{
		for (int i = 0; i < triggeredVolume.TriggeredByIndices.Count; i++)
		{
			byte key = triggeredVolume.TriggeredByIndices[i];
			if (!this.TriggeredByVolumes.ContainsKey((int)key))
			{
				this.TriggeredByVolumes.Add((int)key, new List<SleeperVolume>());
			}
			this.TriggeredByVolumes[(int)key].Add(triggeredVolume);
		}
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x000556DC File Offset: 0x000538DC
	public void Trigger(EntityPlayer player, byte index)
	{
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		World world = GameManager.Instance.World;
		List<BlockTrigger> list2;
		if (this.TriggeredByDictionary.TryGetValue((int)index, out list2))
		{
			for (int i = 0; i < list2.Count; i++)
			{
				list2[i].OnTriggered(player, world, (int)index, list, null);
			}
		}
		List<SleeperVolume> list3;
		if (this.TriggeredByVolumes.TryGetValue((int)index, out list3))
		{
			foreach (SleeperVolume sleeperVolume in list3)
			{
				sleeperVolume.OnTriggered(player, world, (int)index);
			}
		}
		if (list.Count > 0)
		{
			this.UpdateBlocks(list);
		}
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x00055794 File Offset: 0x00053994
	public void Trigger(EntityPlayer player, BlockTrigger trigger)
	{
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		World world = GameManager.Instance.World;
		using (List<byte>.Enumerator enumerator = trigger.TriggersIndices.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int num = (int)enumerator.Current;
				List<BlockTrigger> list2;
				if (this.TriggeredByDictionary.TryGetValue(num, out list2))
				{
					foreach (BlockTrigger blockTrigger in list2)
					{
						blockTrigger.OnTriggered(player, world, num, list, trigger);
					}
				}
				if (player != null && this.TriggeredByVolumes.ContainsKey(num))
				{
					foreach (SleeperVolume sleeperVolume in this.TriggeredByVolumes[num])
					{
						sleeperVolume.OnTriggered(player, world, num);
					}
				}
			}
		}
		if (list.Count > 0)
		{
			this.UpdateBlocks(list);
		}
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x000558B8 File Offset: 0x00053AB8
	public void Trigger(EntityPlayer player, TriggerVolume trigger)
	{
		List<BlockChangeInfo> list = new List<BlockChangeInfo>();
		World world = GameManager.Instance.World;
		for (int i = 0; i < trigger.TriggersIndices.Count; i++)
		{
			int num = (int)trigger.TriggersIndices[i];
			if (this.TriggeredByDictionary.ContainsKey(num))
			{
				for (int j = 0; j < this.TriggeredByDictionary[num].Count; j++)
				{
					this.TriggeredByDictionary[num][j].OnTriggered(player, world, num, list, null);
				}
			}
			if (this.TriggeredByVolumes.ContainsKey(num))
			{
				for (int k = 0; k < this.TriggeredByVolumes[num].Count; k++)
				{
					this.TriggeredByVolumes[num][k].OnTriggered(player, world, num);
				}
			}
		}
		if (list.Count > 0)
		{
			this.UpdateBlocks(list);
		}
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x000559A3 File Offset: 0x00053BA3
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateBlocks(List<BlockChangeInfo> blockChanges)
	{
		if (GameManager.Instance.World != null && blockChanges != null)
		{
			GameManager.Instance.World.SetBlocksRPC(blockChanges);
		}
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x000559C4 File Offset: 0x00053BC4
	public void SetupTriggerTestNavObjects()
	{
		this.RemoveTriggerTestNavObjects();
		for (int i = 0; i < this.Triggers.Count; i++)
		{
			NavObject navObject = NavObjectManager.Instance.RegisterNavObject("editor_block_trigger", this.Triggers[i].ToWorldPos().ToVector3Center(), "", false, -1, null);
			navObject.name = this.Triggers[i].TriggerDisplay();
			navObject.OverrideColor = ((this.Triggers[i].TriggeredByIndices.Count > 0) ? Color.blue : Color.red);
		}
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00055A64 File Offset: 0x00053C64
	public void RemoveTriggerTestNavObjects()
	{
		for (int i = 0; i < this.Triggers.Count; i++)
		{
			NavObjectManager.Instance.UnRegisterNavObjectByPosition(this.Triggers[i].ToWorldPos().ToVector3Center(), "editor_block_trigger");
		}
	}

	// Token: 0x04000A65 RID: 2661
	public Dictionary<int, List<BlockTrigger>> TriggeredByDictionary = new Dictionary<int, List<BlockTrigger>>();

	// Token: 0x04000A66 RID: 2662
	public Dictionary<int, List<SleeperVolume>> TriggeredByVolumes = new Dictionary<int, List<SleeperVolume>>();

	// Token: 0x04000A67 RID: 2663
	public PrefabInstance PrefabInstance;

	// Token: 0x04000A68 RID: 2664
	[PublicizedFrom(EAccessModifier.Protected)]
	public World world;

	// Token: 0x04000A69 RID: 2665
	public List<int> PlayersInArea = new List<int>();

	// Token: 0x04000A6A RID: 2666
	public List<byte> TriggeredLayers;

	// Token: 0x04000A6B RID: 2667
	public List<byte> TriggeredByLayers;

	// Token: 0x04000A6C RID: 2668
	public List<BlockTrigger> Triggers = new List<BlockTrigger>();

	// Token: 0x04000A6D RID: 2669
	public TriggerManager Owner;

	// Token: 0x04000A6E RID: 2670
	[PublicizedFrom(EAccessModifier.Private)]
	public float needsTriggerTimer = -1f;
}

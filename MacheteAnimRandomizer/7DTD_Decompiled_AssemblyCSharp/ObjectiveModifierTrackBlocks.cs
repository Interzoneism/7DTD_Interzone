using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008A9 RID: 2217
[Preserve]
public class ObjectiveModifierTrackBlocks : BaseObjectiveModifier
{
	// Token: 0x060040A0 RID: 16544 RVA: 0x001A5257 File Offset: 0x001A3457
	public override void AddHooks()
	{
		base.OwnerObjective.OwnerQuest.TrackingHelper.AddTrackingEntry(this);
		QuestEventManager.Current.BlockChange += this.Current_BlockChange;
	}

	// Token: 0x060040A1 RID: 16545 RVA: 0x001A5288 File Offset: 0x001A3488
	public override void RemoveHooks()
	{
		base.OwnerObjective.OwnerQuest.TrackingHelper.RemoveTrackingEntry(this);
		QuestEventManager.Current.BlockChange -= this.Current_BlockChange;
		NavObjectManager instance = NavObjectManager.Instance;
		for (int i = this.TrackedBlocks.Count - 1; i >= 0; i--)
		{
			instance.UnRegisterNavObject(this.TrackedBlocks[i].NavObject);
			this.TrackedBlocks.RemoveAt(i);
		}
	}

	// Token: 0x060040A2 RID: 16546 RVA: 0x001A5304 File Offset: 0x001A3504
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockChange(Block blockOld, Block blockNew, Vector3i blockPos)
	{
		if (blockOld.IndexName == this.blockIndexName)
		{
			for (int i = 0; i < this.TrackedBlocks.Count; i++)
			{
				if (this.TrackedBlocks[i].WorldPos == blockPos)
				{
					NavObjectManager.Instance.UnRegisterNavObject(this.TrackedBlocks[i].NavObject);
					this.TrackedBlocks.RemoveAt(i);
					return;
				}
			}
		}
	}

	// Token: 0x060040A3 RID: 16547 RVA: 0x001A537C File Offset: 0x001A357C
	public void StartUpdate()
	{
		if (this.localPlayer == null)
		{
			this.localPlayer = base.OwnerObjective.OwnerQuest.OwnerJournal.OwnerPlayer;
		}
		for (int i = 0; i < this.TrackedBlocks.Count; i++)
		{
			this.TrackedBlocks[i].KeepAlive = false;
		}
	}

	// Token: 0x060040A4 RID: 16548 RVA: 0x001A53DC File Offset: 0x001A35DC
	public void HandleTrack(Chunk c)
	{
		List<Vector3i> list;
		if (c.IndexedBlocks.TryGetValue(this.blockIndexName, out list))
		{
			foreach (Vector3i vector3i in list)
			{
				if (!c.GetBlockNoDamage(vector3i.x, vector3i.y, vector3i.z).ischild)
				{
					Vector3i vector3i2 = c.ToWorldPos(vector3i);
					if (Vector3.Distance(vector3i2, this.localPlayer.position) < this.trackDistance)
					{
						this.HandleAddTrackedBlock(vector3i2);
					}
				}
			}
		}
	}

	// Token: 0x060040A5 RID: 16549 RVA: 0x001A5488 File Offset: 0x001A3688
	public void EndUpdate()
	{
		NavObjectManager instance = NavObjectManager.Instance;
		for (int i = this.TrackedBlocks.Count - 1; i >= 0; i--)
		{
			if (!this.TrackedBlocks[i].KeepAlive)
			{
				instance.UnRegisterNavObject(this.TrackedBlocks[i].NavObject);
				this.TrackedBlocks.RemoveAt(i);
			}
		}
	}

	// Token: 0x060040A6 RID: 16550 RVA: 0x001A54EC File Offset: 0x001A36EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleAddTrackedBlock(Vector3i pos)
	{
		for (int i = 0; i < this.TrackedBlocks.Count; i++)
		{
			if (pos == this.TrackedBlocks[i].WorldPos)
			{
				this.TrackedBlocks[i].KeepAlive = true;
			}
		}
		this.TrackedBlocks.Add(new ObjectiveModifierTrackBlocks.TrackedBlock(pos, this.navObjectName));
	}

	// Token: 0x060040A7 RID: 16551 RVA: 0x001A5554 File Offset: 0x001A3754
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectiveModifierTrackBlocks.PropBlockIndexName))
		{
			this.blockIndexName = properties.Values[ObjectiveModifierTrackBlocks.PropBlockIndexName];
		}
		if (properties.Values.ContainsKey(ObjectiveModifierTrackBlocks.PropNavObjectName))
		{
			this.navObjectName = properties.Values[ObjectiveModifierTrackBlocks.PropNavObjectName];
		}
		if (properties.Values.ContainsKey(ObjectiveModifierTrackBlocks.PropTrackDistance))
		{
			this.trackDistance = StringParsers.ParseFloat(properties.Values[ObjectiveModifierTrackBlocks.PropTrackDistance], 0, -1, NumberStyles.Any);
		}
	}

	// Token: 0x060040A8 RID: 16552 RVA: 0x001A55EC File Offset: 0x001A37EC
	public override BaseObjectiveModifier Clone()
	{
		return new ObjectiveModifierTrackBlocks
		{
			blockIndexName = this.blockIndexName,
			navObjectName = this.navObjectName,
			trackDistance = this.trackDistance
		};
	}

	// Token: 0x040033BB RID: 13243
	[PublicizedFrom(EAccessModifier.Protected)]
	public string blockIndexName = "questTracked";

	// Token: 0x040033BC RID: 13244
	[PublicizedFrom(EAccessModifier.Protected)]
	public string navObjectName = "quest_resource";

	// Token: 0x040033BD RID: 13245
	public float trackDistance = 20f;

	// Token: 0x040033BE RID: 13246
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal localPlayer;

	// Token: 0x040033BF RID: 13247
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<ObjectiveModifierTrackBlocks.TrackedBlock> TrackedBlocks = new List<ObjectiveModifierTrackBlocks.TrackedBlock>();

	// Token: 0x040033C0 RID: 13248
	public static string PropBlockIndexName = "block_index_name";

	// Token: 0x040033C1 RID: 13249
	public static string PropNavObjectName = "nav_object";

	// Token: 0x040033C2 RID: 13250
	public static string PropTrackDistance = "track_distance";

	// Token: 0x020008AA RID: 2218
	public class TrackedBlock
	{
		// Token: 0x060040AB RID: 16555 RVA: 0x001A566B File Offset: 0x001A386B
		public TrackedBlock(Vector3i worldPos, string NavObjectName)
		{
			this.WorldPos = worldPos;
			this.NavObject = NavObjectManager.Instance.RegisterNavObject(NavObjectName, this.WorldPos.ToVector3Center(), "", false, -1, null);
			this.KeepAlive = true;
		}

		// Token: 0x040033C3 RID: 13251
		public Vector3i WorldPos;

		// Token: 0x040033C4 RID: 13252
		public NavObject NavObject;

		// Token: 0x040033C5 RID: 13253
		public bool KeepAlive;
	}
}

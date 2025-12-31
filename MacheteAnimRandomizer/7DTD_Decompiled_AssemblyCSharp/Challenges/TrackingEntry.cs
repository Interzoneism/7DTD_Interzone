using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x0200160B RID: 5643
	[Preserve]
	public class TrackingEntry
	{
		// Token: 0x0600AD9B RID: 44443 RVA: 0x0043F6C8 File Offset: 0x0043D8C8
		public void AddHooks()
		{
			if (this.TrackingHelper != null)
			{
				this.TrackingHelper.AddTrackingEntry(this);
			}
			QuestEventManager.Current.BlockChange -= this.Current_BlockChange;
			QuestEventManager.Current.BlockChange += this.Current_BlockChange;
		}

		// Token: 0x0600AD9C RID: 44444 RVA: 0x0043F718 File Offset: 0x0043D918
		public void RemoveHooks()
		{
			if (this.TrackingHelper != null)
			{
				this.TrackingHelper.RemoveTrackingEntry(this);
			}
			QuestEventManager.Current.BlockChange -= this.Current_BlockChange;
			NavObjectManager instance = NavObjectManager.Instance;
			for (int i = this.TrackedBlocks.Count - 1; i >= 0; i--)
			{
				instance.UnRegisterNavObject(this.TrackedBlocks[i].NavObject);
				this.TrackedBlocks.RemoveAt(i);
			}
		}

		// Token: 0x0600AD9D RID: 44445 RVA: 0x0043F790 File Offset: 0x0043D990
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

		// Token: 0x0600AD9E RID: 44446 RVA: 0x0043F808 File Offset: 0x0043DA08
		public void StartUpdate()
		{
			if (this.localPlayer == null)
			{
				this.localPlayer = this.Owner.Owner.Owner.Player;
			}
			for (int i = 0; i < this.TrackedBlocks.Count; i++)
			{
				this.TrackedBlocks[i].KeepAlive = false;
			}
		}

		// Token: 0x0600AD9F RID: 44447 RVA: 0x0043F868 File Offset: 0x0043DA68
		public void HandleTrack(Chunk c)
		{
			List<Vector3i> list;
			if (c.IndexedBlocks.TryGetValue(this.blockIndexName, out list))
			{
				foreach (Vector3i pos in list)
				{
					Vector3i vector3i = c.ToWorldPos(pos);
					if (!c.GetBlock(pos).ischild && Vector3.Distance(vector3i, this.localPlayer.position) < this.trackDistance)
					{
						this.HandleAddTrackedBlock(vector3i);
					}
				}
			}
		}

		// Token: 0x0600ADA0 RID: 44448 RVA: 0x0043F904 File Offset: 0x0043DB04
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

		// Token: 0x0600ADA1 RID: 44449 RVA: 0x0043F968 File Offset: 0x0043DB68
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
			this.TrackedBlocks.Add(new TrackingEntry.TrackedBlock(pos, this.navObjectName));
		}

		// Token: 0x040086C8 RID: 34504
		public float trackDistance = 20f;

		// Token: 0x040086C9 RID: 34505
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal localPlayer;

		// Token: 0x040086CA RID: 34506
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<TrackingEntry.TrackedBlock> TrackedBlocks = new List<TrackingEntry.TrackedBlock>();

		// Token: 0x040086CB RID: 34507
		public ItemClass TrackedItem;

		// Token: 0x040086CC RID: 34508
		public BaseChallengeObjective Owner;

		// Token: 0x040086CD RID: 34509
		public ChallengeTrackingHandler TrackingHelper;

		// Token: 0x040086CE RID: 34510
		public string blockIndexName = "quest_wood";

		// Token: 0x040086CF RID: 34511
		public string navObjectName = "quest_resource";

		// Token: 0x0200160C RID: 5644
		public class TrackedBlock
		{
			// Token: 0x0600ADA3 RID: 44451 RVA: 0x0043FA01 File Offset: 0x0043DC01
			public TrackedBlock(Vector3i worldPos, string NavObjectName)
			{
				this.WorldPos = worldPos;
				this.NavObject = NavObjectManager.Instance.RegisterNavObject(NavObjectName, this.WorldPos.ToVector3Center(), "", false, -1, null);
				this.KeepAlive = true;
			}

			// Token: 0x040086D0 RID: 34512
			public Vector3i WorldPos;

			// Token: 0x040086D1 RID: 34513
			public NavObject NavObject;

			// Token: 0x040086D2 RID: 34514
			public bool KeepAlive;
		}
	}
}

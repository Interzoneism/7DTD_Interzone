using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
	// Token: 0x020015B0 RID: 5552
	public class TrackingHandler
	{
		// Token: 0x0600AA95 RID: 43669 RVA: 0x004339CC File Offset: 0x00431BCC
		public bool Update(float deltaTime)
		{
			if (this.LocalPlayer == null)
			{
				return true;
			}
			Quest quest = this.LocalPlayer.QuestJournal.FindActiveQuest(this.QuestCode);
			if (quest == null || quest.OwnerJournal == null || quest.OwnerJournal.OwnerPlayer == null)
			{
				return false;
			}
			if (Vector3.Distance(this.LastCheckedPosition, this.LocalPlayer.position) > this.RefreshDistance || this.NeedsRefresh)
			{
				this.LastCheckedPosition = this.LocalPlayer.position;
				this.HandleTracking();
				this.NeedsRefresh = false;
			}
			return true;
		}

		// Token: 0x0600AA96 RID: 43670 RVA: 0x00433A65 File Offset: 0x00431C65
		public void AddTrackingEntry(ObjectiveModifierTrackBlocks track)
		{
			if (!this.trackingEntries.Contains(track))
			{
				this.trackingEntries.Add(track);
				this.NeedsRefresh = true;
			}
			QuestEventManager.Current.AddTrackerToBeUpdated(this);
		}

		// Token: 0x0600AA97 RID: 43671 RVA: 0x00433A93 File Offset: 0x00431C93
		public void RemoveTrackingEntry(ObjectiveModifierTrackBlocks track)
		{
			if (this.trackingEntries.Contains(track))
			{
				this.trackingEntries.Remove(track);
				this.NeedsRefresh = true;
			}
			if (this.trackingEntries.Count == 0)
			{
				QuestEventManager.Current.RemoveTrackerToBeUpdated(this);
			}
		}

		// Token: 0x0600AA98 RID: 43672 RVA: 0x00433AD0 File Offset: 0x00431CD0
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleTracking()
		{
			NavObjectManager instance = NavObjectManager.Instance;
			List<Chunk> chunkArrayCopySync = GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync();
			for (int i = 0; i < this.trackingEntries.Count; i++)
			{
				this.trackingEntries[i].StartUpdate();
			}
			foreach (Chunk c in chunkArrayCopySync)
			{
				for (int j = 0; j < this.trackingEntries.Count; j++)
				{
					this.trackingEntries[j].HandleTrack(c);
				}
			}
			for (int k = 0; k < this.trackingEntries.Count; k++)
			{
				this.trackingEntries[k].EndUpdate();
			}
		}

		// Token: 0x04008538 RID: 34104
		public int QuestCode;

		// Token: 0x04008539 RID: 34105
		public EntityPlayerLocal LocalPlayer;

		// Token: 0x0400853A RID: 34106
		public List<ObjectiveModifierTrackBlocks> trackingEntries = new List<ObjectiveModifierTrackBlocks>();

		// Token: 0x0400853B RID: 34107
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3 LastCheckedPosition = new Vector3(0f, 9999f, 0f);

		// Token: 0x0400853C RID: 34108
		public float RefreshDistance = 5f;

		// Token: 0x0400853D RID: 34109
		public bool NeedsRefresh;
	}
}

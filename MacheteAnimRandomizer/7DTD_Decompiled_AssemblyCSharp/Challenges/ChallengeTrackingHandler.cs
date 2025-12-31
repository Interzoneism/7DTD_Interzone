using System;
using System.Collections.Generic;
using UnityEngine;

namespace Challenges
{
	// Token: 0x0200160D RID: 5645
	public class ChallengeTrackingHandler
	{
		// Token: 0x0600ADA4 RID: 44452 RVA: 0x0043FA3C File Offset: 0x0043DC3C
		public bool Update(float deltaTime)
		{
			if (this.LocalPlayer == null)
			{
				return true;
			}
			if (this.Owner == null || !this.Owner.IsActive)
			{
				return false;
			}
			if (this.LocalPlayer.IsInTrader != this.lastInTrader)
			{
				this.lastInTrader = this.LocalPlayer.IsInTrader;
				this.NeedsRefresh = true;
			}
			if (Vector3.Distance(this.LastCheckedPosition, this.LocalPlayer.position) > this.RefreshDistance || this.NeedsRefresh)
			{
				this.LastCheckedPosition = this.LocalPlayer.position;
				this.HandleTracking();
				this.NeedsRefresh = false;
			}
			return true;
		}

		// Token: 0x0600ADA5 RID: 44453 RVA: 0x0043FAE0 File Offset: 0x0043DCE0
		public void AddTrackingEntry(TrackingEntry track)
		{
			if (!this.trackingEntries.Contains(track))
			{
				this.trackingEntries.Add(track);
			}
			QuestEventManager.Current.AddTrackerToBeUpdated(this);
			this.NeedsRefresh = true;
		}

		// Token: 0x0600ADA6 RID: 44454 RVA: 0x0043FB0E File Offset: 0x0043DD0E
		public void RemoveTrackingEntry(TrackingEntry track)
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

		// Token: 0x0600ADA7 RID: 44455 RVA: 0x0043FB4C File Offset: 0x0043DD4C
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleTracking()
		{
			NavObjectManager instance = NavObjectManager.Instance;
			List<Chunk> chunkArrayCopySync = GameManager.Instance.World.ChunkCache.GetChunkArrayCopySync();
			for (int i = 0; i < this.trackingEntries.Count; i++)
			{
				this.trackingEntries[i].StartUpdate();
			}
			if (!this.LocalPlayer.IsInTrader)
			{
				foreach (Chunk c in chunkArrayCopySync)
				{
					for (int j = 0; j < this.trackingEntries.Count; j++)
					{
						this.trackingEntries[j].HandleTrack(c);
					}
				}
			}
			for (int k = 0; k < this.trackingEntries.Count; k++)
			{
				this.trackingEntries[k].EndUpdate();
			}
		}

		// Token: 0x040086D3 RID: 34515
		public Challenge Owner;

		// Token: 0x040086D4 RID: 34516
		public EntityPlayerLocal LocalPlayer;

		// Token: 0x040086D5 RID: 34517
		public List<TrackingEntry> trackingEntries = new List<TrackingEntry>();

		// Token: 0x040086D6 RID: 34518
		[PublicizedFrom(EAccessModifier.Protected)]
		public Vector3 LastCheckedPosition = new Vector3(0f, 9999f, 0f);

		// Token: 0x040086D7 RID: 34519
		public float RefreshDistance = 5f;

		// Token: 0x040086D8 RID: 34520
		public bool NeedsRefresh;

		// Token: 0x040086D9 RID: 34521
		[PublicizedFrom(EAccessModifier.Private)]
		public bool lastInTrader;
	}
}

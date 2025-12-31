using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200157A RID: 5498
	public class TwitchSpawnedBlocksEntry
	{
		// Token: 0x0600A956 RID: 43350 RVA: 0x0042DCE4 File Offset: 0x0042BEE4
		public bool CheckPos(Vector3i pos)
		{
			for (int i = 0; i < this.blocks.Count; i++)
			{
				if (this.blocks[i] == pos)
				{
					return true;
				}
			}
			if (this.recentlyRemoved != null)
			{
				for (int j = 0; j < this.recentlyRemoved.Count; j++)
				{
					if (this.recentlyRemoved[j] == pos)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600A957 RID: 43351 RVA: 0x0042DD54 File Offset: 0x0042BF54
		[PublicizedFrom(EAccessModifier.Internal)]
		public bool RemoveBlock(Vector3i blockRemoved)
		{
			for (int i = this.blocks.Count - 1; i >= 0; i--)
			{
				if (this.blocks[i] == blockRemoved)
				{
					if (this.recentlyRemoved == null)
					{
						this.recentlyRemoved = new List<Vector3i>();
					}
					this.recentlyRemoved.Add(this.blocks[i]);
					this.blocks.RemoveAt(i);
				}
			}
			return this.blocks.Count == 0;
		}

		// Token: 0x0600A958 RID: 43352 RVA: 0x0042DDD4 File Offset: 0x0042BFD4
		[PublicizedFrom(EAccessModifier.Internal)]
		public bool RemoveBlocks(List<Vector3i> blocksRemoved)
		{
			for (int i = this.blocks.Count - 1; i >= 0; i--)
			{
				for (int j = 0; j < blocksRemoved.Count; j++)
				{
					if (this.blocks[i] == blocksRemoved[j])
					{
						this.blocks.RemoveAt(i);
						break;
					}
				}
			}
			return this.blocks.Count == 0;
		}

		// Token: 0x040083B6 RID: 33718
		public List<Vector3i> blocks;

		// Token: 0x040083B7 RID: 33719
		public List<Vector3i> recentlyRemoved;

		// Token: 0x040083B8 RID: 33720
		public TwitchActionEntry Action;

		// Token: 0x040083B9 RID: 33721
		public TwitchEventActionEntry Event;

		// Token: 0x040083BA RID: 33722
		public TwitchVoteEntry Vote;

		// Token: 0x040083BB RID: 33723
		public int BlockGroupID = -1;

		// Token: 0x040083BC RID: 33724
		public float TimeRemaining = -1f;

		// Token: 0x040083BD RID: 33725
		public TwitchRespawnEntry RespawnEntry;
	}
}

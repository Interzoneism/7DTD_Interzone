using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200157F RID: 5503
	public class TwitchRespawnEntry
	{
		// Token: 0x0600A95F RID: 43359 RVA: 0x0042DF23 File Offset: 0x0042C123
		public TwitchRespawnEntry(string username, int respawnsLeft, EntityPlayer target, TwitchAction action)
		{
			this.UserName = username;
			this.Target = target;
			this.Action = action;
			this.RespawnsLeft = respawnsLeft;
		}

		// Token: 0x0600A960 RID: 43360 RVA: 0x0042DF5E File Offset: 0x0042C15E
		public bool CheckRespawn(string username, EntityPlayer target, TwitchAction action)
		{
			return this.UserName == username && this.Target == target && this.Action == action;
		}

		// Token: 0x0600A961 RID: 43361 RVA: 0x0042DF88 File Offset: 0x0042C188
		public bool RemoveSpawnedEntry(int entityID, bool checkForRemove)
		{
			bool result = false;
			for (int i = this.SpawnedEntities.Count - 1; i >= 0; i--)
			{
				if (this.SpawnedEntities[i] == entityID)
				{
					result = true;
					this.SpawnedEntities.RemoveAt(i);
				}
			}
			if (checkForRemove)
			{
				this.CheckReadyForRemove();
			}
			return result;
		}

		// Token: 0x0600A962 RID: 43362 RVA: 0x0042DFD8 File Offset: 0x0042C1D8
		public bool RemoveSpawnedBlock(Vector3i pos, bool checkForRemove)
		{
			bool result = false;
			for (int i = this.SpawnedBlocks.Count - 1; i >= 0; i--)
			{
				if (this.SpawnedBlocks[i] == pos)
				{
					result = true;
					this.SpawnedBlocks.RemoveAt(i);
				}
			}
			if (checkForRemove)
			{
				this.CheckReadyForRemove();
			}
			return result;
		}

		// Token: 0x0600A963 RID: 43363 RVA: 0x0042E02C File Offset: 0x0042C22C
		public bool RemoveAllSpawnedBlock(bool checkForRemove)
		{
			bool result = false;
			if (this.SpawnedBlocks.Count > 0)
			{
				result = true;
				this.SpawnedBlocks.Clear();
			}
			if (checkForRemove)
			{
				this.CheckReadyForRemove();
			}
			return result;
		}

		// Token: 0x0600A964 RID: 43364 RVA: 0x0042E060 File Offset: 0x0042C260
		public void CheckReadyForRemove()
		{
			TwitchAction.RespawnCountTypes respawnCountType = this.Action.RespawnCountType;
			if (respawnCountType == TwitchAction.RespawnCountTypes.SpawnsOnly)
			{
				this.ReadyForRemove = (this.SpawnedEntities.Count == 0);
				return;
			}
			if (respawnCountType == TwitchAction.RespawnCountTypes.BlocksOnly)
			{
				this.ReadyForRemove = (this.SpawnedBlocks.Count == 0);
				return;
			}
			this.ReadyForRemove = (this.SpawnedEntities.Count == 0 && this.SpawnedBlocks.Count == 0);
		}

		// Token: 0x0600A965 RID: 43365 RVA: 0x0042E0D0 File Offset: 0x0042C2D0
		public TwitchActionEntry RespawnAction()
		{
			TwitchActionEntry twitchActionEntry = this.Action.SetupActionEntry();
			twitchActionEntry.UserName = this.UserName;
			twitchActionEntry.Target = this.Target;
			twitchActionEntry.Action = this.Action;
			twitchActionEntry.IsRespawn = true;
			twitchActionEntry.IsBitAction = (this.Action.PointType == TwitchAction.PointTypes.Bits);
			this.RespawnsLeft--;
			this.NeedsRespawn = false;
			if (this.RespawnsLeft <= 0)
			{
				this.ReadyForRemove = true;
			}
			return twitchActionEntry;
		}

		// Token: 0x0600A966 RID: 43366 RVA: 0x0042E14C File Offset: 0x0042C34C
		public bool CanRespawn(TwitchManager tm)
		{
			return this.NeedsRespawn && tm.CheckCanRespawnEvent(this.Target);
		}

		// Token: 0x040083D0 RID: 33744
		public string UserName;

		// Token: 0x040083D1 RID: 33745
		public EntityPlayer Target;

		// Token: 0x040083D2 RID: 33746
		public TwitchAction Action;

		// Token: 0x040083D3 RID: 33747
		public int RespawnsLeft;

		// Token: 0x040083D4 RID: 33748
		public bool NeedsRespawn;

		// Token: 0x040083D5 RID: 33749
		public bool ReadyForRemove;

		// Token: 0x040083D6 RID: 33750
		public List<int> SpawnedEntities = new List<int>();

		// Token: 0x040083D7 RID: 33751
		public List<Vector3i> SpawnedBlocks = new List<Vector3i>();
	}
}

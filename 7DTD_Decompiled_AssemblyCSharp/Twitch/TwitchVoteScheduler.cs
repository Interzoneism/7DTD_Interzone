using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001588 RID: 5512
	public class TwitchVoteScheduler
	{
		// Token: 0x170012D2 RID: 4818
		// (get) Token: 0x0600A9A3 RID: 43427 RVA: 0x004304E4 File Offset: 0x0042E6E4
		public static TwitchVoteScheduler Current
		{
			get
			{
				if (TwitchVoteScheduler.instance == null)
				{
					TwitchVoteScheduler.instance = new TwitchVoteScheduler();
				}
				return TwitchVoteScheduler.instance;
			}
		}

		// Token: 0x170012D3 RID: 4819
		// (get) Token: 0x0600A9A4 RID: 43428 RVA: 0x004304FC File Offset: 0x0042E6FC
		public static bool HasInstance
		{
			get
			{
				return TwitchVoteScheduler.instance != null;
			}
		}

		// Token: 0x0600A9A5 RID: 43429 RVA: 0x00430506 File Offset: 0x0042E706
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchVoteScheduler()
		{
		}

		// Token: 0x0600A9A6 RID: 43430 RVA: 0x00430519 File Offset: 0x0042E719
		public void Cleanup()
		{
			this.ClearParticipants();
			TwitchVoteScheduler.instance = null;
		}

		// Token: 0x0600A9A7 RID: 43431 RVA: 0x00430527 File Offset: 0x0042E727
		public void AddParticipant(int entityID)
		{
			if (!this.votingParticipants.Contains(entityID))
			{
				this.votingParticipants.Add(entityID);
			}
		}

		// Token: 0x0600A9A8 RID: 43432 RVA: 0x00430543 File Offset: 0x0042E743
		public void ClearParticipants()
		{
			this.votingParticipants.Clear();
		}

		// Token: 0x0600A9A9 RID: 43433 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init()
		{
		}

		// Token: 0x0600A9AA RID: 43434 RVA: 0x00430550 File Offset: 0x0042E750
		public void Update(float deltaTime)
		{
			if (GameManager.Instance.World == null || GameManager.Instance.World.Players == null || GameManager.Instance.World.Players.Count == 0)
			{
				return;
			}
			if (this.nextVoteTime > 0f)
			{
				this.nextVoteTime -= deltaTime;
			}
			if (this.votingParticipants.Count == 0)
			{
				return;
			}
			if (this.nextVoteTime <= 0f)
			{
				if (GameManager.Instance.World.GetPrimaryPlayerId() == this.votingParticipants[0])
				{
					TwitchManager.Current.VotingManager.RequestApprovedToStart();
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTwitchVoteScheduling>().Setup(), false, this.votingParticipants[0], -1, -1, null, 192, false);
				}
				this.votingParticipants.RemoveAt(0);
				this.nextVoteTime = 3f;
			}
		}

		// Token: 0x04008450 RID: 33872
		[PublicizedFrom(EAccessModifier.Private)]
		public static TwitchVoteScheduler instance;

		// Token: 0x04008451 RID: 33873
		[PublicizedFrom(EAccessModifier.Private)]
		public List<int> votingParticipants = new List<int>();

		// Token: 0x04008452 RID: 33874
		[PublicizedFrom(EAccessModifier.Private)]
		public float nextVoteTime;
	}
}

using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001585 RID: 5509
	public class TwitchVoteEntry
	{
		// Token: 0x0600A99E RID: 43422 RVA: 0x004303B1 File Offset: 0x0042E5B1
		public TwitchVoteEntry(string voteCommand, TwitchVote voteClass)
		{
			this.VoteCommand = voteCommand;
			this.VoteClass = voteClass;
		}

		// Token: 0x0400843D RID: 33853
		public TwitchVote VoteClass;

		// Token: 0x0400843E RID: 33854
		public string VoteCommand;

		// Token: 0x0400843F RID: 33855
		public TwitchVotingManager Owner;

		// Token: 0x04008440 RID: 33856
		public int VoteCount;

		// Token: 0x04008441 RID: 33857
		public int Index = -1;

		// Token: 0x04008442 RID: 33858
		public bool UIDirty = true;

		// Token: 0x04008443 RID: 33859
		public bool Complete;

		// Token: 0x04008444 RID: 33860
		public List<string> VoterNames = new List<string>();

		// Token: 0x04008445 RID: 33861
		public List<int> ActiveSpawns = new List<int>();
	}
}

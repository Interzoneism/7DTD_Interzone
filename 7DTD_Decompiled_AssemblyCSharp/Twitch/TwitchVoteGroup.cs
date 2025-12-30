using System;
using System.Collections.Generic;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001586 RID: 5510
	public class TwitchVoteGroup
	{
		// Token: 0x0600A99F RID: 43423 RVA: 0x004303EB File Offset: 0x0042E5EB
		public TwitchVoteGroup(string name)
		{
			this.Name = name;
		}

		// Token: 0x0600A9A0 RID: 43424 RVA: 0x00430410 File Offset: 0x0042E610
		public TwitchVoteType GetNextVoteType()
		{
			this.index++;
			if (this.index >= this.VoteTypes.Count)
			{
				this.index = 0;
			}
			return this.VoteTypes[this.index];
		}

		// Token: 0x0600A9A1 RID: 43425 RVA: 0x0043044C File Offset: 0x0042E64C
		public void ShuffleVoteTypes()
		{
			for (int i = 0; i <= this.VoteTypes.Count * this.VoteTypes.Count; i++)
			{
				int num = UnityEngine.Random.Range(0, this.VoteTypes.Count);
				int num2 = UnityEngine.Random.Range(0, this.VoteTypes.Count);
				if (num != num2)
				{
					TwitchVoteType value = this.VoteTypes[num];
					this.VoteTypes[num] = this.VoteTypes[num2];
					this.VoteTypes[num2] = value;
				}
			}
		}

		// Token: 0x04008446 RID: 33862
		public string Name = "";

		// Token: 0x04008447 RID: 33863
		public List<TwitchVoteType> VoteTypes = new List<TwitchVoteType>();

		// Token: 0x04008448 RID: 33864
		[PublicizedFrom(EAccessModifier.Private)]
		public int index;

		// Token: 0x04008449 RID: 33865
		public bool SkippedThisVote;
	}
}

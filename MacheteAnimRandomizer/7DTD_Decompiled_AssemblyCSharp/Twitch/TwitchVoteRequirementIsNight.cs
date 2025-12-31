using System;

namespace Twitch
{
	// Token: 0x02001596 RID: 5526
	public class TwitchVoteRequirementIsNight : BaseTwitchVoteRequirement
	{
		// Token: 0x0600AA04 RID: 43524 RVA: 0x0043261B File Offset: 0x0043081B
		public override bool CanPerform(EntityPlayer player)
		{
			if (!this.Invert)
			{
				return !GameManager.Instance.World.IsDaytime();
			}
			return GameManager.Instance.World.IsDaytime();
		}
	}
}

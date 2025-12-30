using System;

namespace Twitch
{
	// Token: 0x02001593 RID: 5523
	public class BaseTwitchVoteRequirement
	{
		// Token: 0x0600A9F1 RID: 43505 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600A9F2 RID: 43506 RVA: 0x00432488 File Offset: 0x00430688
		public void Init()
		{
			this.OnInit();
		}

		// Token: 0x0600A9F3 RID: 43507 RVA: 0x000197A5 File Offset: 0x000179A5
		public virtual bool CanPerform(EntityPlayer player)
		{
			return true;
		}

		// Token: 0x0600A9F4 RID: 43508 RVA: 0x00432490 File Offset: 0x00430690
		public virtual void ParseProperties(DynamicProperties properties)
		{
			if (properties.Values.ContainsKey(BaseTwitchVoteRequirement.PropInvert))
			{
				this.Invert = StringParsers.ParseBool(properties.Values[BaseTwitchVoteRequirement.PropInvert], 0, -1, true);
			}
		}

		// Token: 0x040084DA RID: 34010
		public TwitchVote Owner;

		// Token: 0x040084DB RID: 34011
		public bool Invert;

		// Token: 0x040084DC RID: 34012
		public static string PropInvert = "invert";
	}
}

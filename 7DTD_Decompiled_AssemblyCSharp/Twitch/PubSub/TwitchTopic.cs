using System;

namespace Twitch.PubSub
{
	// Token: 0x020015AF RID: 5551
	public class TwitchTopic
	{
		// Token: 0x0600AA8D RID: 43661 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchTopic()
		{
		}

		// Token: 0x17001306 RID: 4870
		// (get) Token: 0x0600AA8E RID: 43662 RVA: 0x00433942 File Offset: 0x00431B42
		// (set) Token: 0x0600AA8F RID: 43663 RVA: 0x0043394A File Offset: 0x00431B4A
		public string TopicString { get; set; }

		// Token: 0x0600AA90 RID: 43664 RVA: 0x00433953 File Offset: 0x00431B53
		public static TwitchTopic ChannelPoints(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("channel-points-channel-v1.{0}", channelId)
			};
		}

		// Token: 0x0600AA91 RID: 43665 RVA: 0x0043396B File Offset: 0x00431B6B
		public static TwitchTopic Bits(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("channel-bits-events-v2.{0}", channelId)
			};
		}

		// Token: 0x0600AA92 RID: 43666 RVA: 0x00433983 File Offset: 0x00431B83
		public static TwitchTopic Subscription(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("channel-subscribe-events-v1.{0}", channelId)
			};
		}

		// Token: 0x0600AA93 RID: 43667 RVA: 0x0043399B File Offset: 0x00431B9B
		public static TwitchTopic HypeTrain(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("hype-train-events-v1.{0}", channelId)
			};
		}

		// Token: 0x0600AA94 RID: 43668 RVA: 0x004339B3 File Offset: 0x00431BB3
		public static TwitchTopic CreatorGoal(string channelId)
		{
			return new TwitchTopic
			{
				TopicString = string.Format("creator-goals-events-v1.{0}", channelId)
			};
		}
	}
}

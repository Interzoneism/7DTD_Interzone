using System;
using UnityEngine;

namespace Twitch.PubSub
{
	// Token: 0x020015A2 RID: 5538
	public class PubSubHypeTrainMessage : BasePubSubMessage
	{
		// Token: 0x0600AA44 RID: 43588 RVA: 0x00432834 File Offset: 0x00430A34
		public static PubSubHypeTrainMessage Deserialize(string message)
		{
			Debug.LogWarning("HypeTrainMessage:\n" + message);
			return new PubSubHypeTrainMessage();
		}

		// Token: 0x040084FD RID: 34045
		public PubSubHypeTrainMessage.HypeTrainData data;

		// Token: 0x020015A3 RID: 5539
		public class HypeTrainData : EventArgs
		{
			// Token: 0x170012F7 RID: 4855
			// (get) Token: 0x0600AA46 RID: 43590 RVA: 0x0043284B File Offset: 0x00430A4B
			// (set) Token: 0x0600AA47 RID: 43591 RVA: 0x00432853 File Offset: 0x00430A53
			public string user_name { get; set; }
		}
	}
}

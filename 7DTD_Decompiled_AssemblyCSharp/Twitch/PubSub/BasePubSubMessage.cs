using System;

namespace Twitch.PubSub
{
	// Token: 0x02001597 RID: 5527
	public class BasePubSubMessage
	{
		// Token: 0x170012DF RID: 4831
		// (get) Token: 0x0600AA06 RID: 43526 RVA: 0x00432647 File Offset: 0x00430847
		// (set) Token: 0x0600AA07 RID: 43527 RVA: 0x0043264F File Offset: 0x0043084F
		public string type { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170012E0 RID: 4832
		// (get) Token: 0x0600AA08 RID: 43528 RVA: 0x00432658 File Offset: 0x00430858
		// (set) Token: 0x0600AA09 RID: 43529 RVA: 0x00432660 File Offset: 0x00430860
		public string nonce { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

		// Token: 0x0600AA0A RID: 43530 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void ReceiveData(string data)
		{
		}
	}
}

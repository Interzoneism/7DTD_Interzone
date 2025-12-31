using System;

namespace Twitch
{
	// Token: 0x020014F8 RID: 5368
	[Serializable]
	public class ExtensionBitAction : ExtensionAction
	{
		// Token: 0x04008048 RID: 32840
		public string txn_id;

		// Token: 0x04008049 RID: 32841
		public long time_created;

		// Token: 0x0400804A RID: 32842
		public int cost;
	}
}

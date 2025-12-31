using System;
using Newtonsoft.Json;

namespace Twitch.PubSub
{
	// Token: 0x0200159F RID: 5535
	public class PubSubGoalMessage : BasePubSubMessage
	{
		// Token: 0x170012EF RID: 4847
		// (get) Token: 0x0600AA31 RID: 43569 RVA: 0x004327AD File Offset: 0x004309AD
		// (set) Token: 0x0600AA32 RID: 43570 RVA: 0x004327B5 File Offset: 0x004309B5
		public new string type { get; set; }

		// Token: 0x170012F0 RID: 4848
		// (get) Token: 0x0600AA33 RID: 43571 RVA: 0x004327BE File Offset: 0x004309BE
		public string TheType
		{
			get
			{
				return this.type;
			}
		}

		// Token: 0x170012F1 RID: 4849
		// (get) Token: 0x0600AA34 RID: 43572 RVA: 0x004327C6 File Offset: 0x004309C6
		// (set) Token: 0x0600AA35 RID: 43573 RVA: 0x004327CE File Offset: 0x004309CE
		public PubSubGoalMessage.GoalData data { get; set; }

		// Token: 0x0600AA36 RID: 43574 RVA: 0x004327D7 File Offset: 0x004309D7
		public static PubSubGoalMessage Deserialize(string message)
		{
			return JsonConvert.DeserializeObject<PubSubGoalMessage>(message);
		}

		// Token: 0x020015A0 RID: 5536
		public class GoalData
		{
			// Token: 0x170012F2 RID: 4850
			// (get) Token: 0x0600AA38 RID: 43576 RVA: 0x004327DF File Offset: 0x004309DF
			// (set) Token: 0x0600AA39 RID: 43577 RVA: 0x004327E7 File Offset: 0x004309E7
			public PubSubGoalMessage.Goal goal { get; set; }
		}

		// Token: 0x020015A1 RID: 5537
		public class Goal
		{
			// Token: 0x170012F3 RID: 4851
			// (get) Token: 0x0600AA3B RID: 43579 RVA: 0x004327F0 File Offset: 0x004309F0
			// (set) Token: 0x0600AA3C RID: 43580 RVA: 0x004327F8 File Offset: 0x004309F8
			public string contributionType { get; set; }

			// Token: 0x170012F4 RID: 4852
			// (get) Token: 0x0600AA3D RID: 43581 RVA: 0x00432801 File Offset: 0x00430A01
			// (set) Token: 0x0600AA3E RID: 43582 RVA: 0x00432809 File Offset: 0x00430A09
			public string state { get; set; }

			// Token: 0x170012F5 RID: 4853
			// (get) Token: 0x0600AA3F RID: 43583 RVA: 0x00432812 File Offset: 0x00430A12
			// (set) Token: 0x0600AA40 RID: 43584 RVA: 0x0043281A File Offset: 0x00430A1A
			public int currentContributions { get; set; }

			// Token: 0x170012F6 RID: 4854
			// (get) Token: 0x0600AA41 RID: 43585 RVA: 0x00432823 File Offset: 0x00430A23
			// (set) Token: 0x0600AA42 RID: 43586 RVA: 0x0043282B File Offset: 0x00430A2B
			public int targetContributions { get; set; }
		}
	}
}

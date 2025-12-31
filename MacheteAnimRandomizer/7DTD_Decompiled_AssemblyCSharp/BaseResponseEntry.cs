using System;
using UnityEngine.Scripting;

// Token: 0x020002B7 RID: 695
[Preserve]
public class BaseResponseEntry
{
	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06001370 RID: 4976 RVA: 0x00076E8B File Offset: 0x0007508B
	// (set) Token: 0x06001371 RID: 4977 RVA: 0x00076E93 File Offset: 0x00075093
	public string ID { get; set; }

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06001372 RID: 4978 RVA: 0x00076E9C File Offset: 0x0007509C
	// (set) Token: 0x06001373 RID: 4979 RVA: 0x00076EA4 File Offset: 0x000750A4
	public BaseResponseEntry.ResponseTypes ResponseType { get; set; }

	// Token: 0x04000CD0 RID: 3280
	public string UniqueID = "";

	// Token: 0x04000CD3 RID: 3283
	public DialogResponse Response;

	// Token: 0x020002B8 RID: 696
	public enum ResponseTypes
	{
		// Token: 0x04000CD5 RID: 3285
		Response,
		// Token: 0x04000CD6 RID: 3286
		QuestAdd
	}
}

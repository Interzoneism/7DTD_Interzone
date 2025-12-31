using System;
using UnityEngine.Scripting;

// Token: 0x020002B9 RID: 697
[Preserve]
public class DialogResponseEntry : BaseResponseEntry
{
	// Token: 0x06001375 RID: 4981 RVA: 0x00076EC0 File Offset: 0x000750C0
	public DialogResponseEntry(string _ID)
	{
		base.ID = _ID;
		base.ResponseType = BaseResponseEntry.ResponseTypes.Response;
	}
}

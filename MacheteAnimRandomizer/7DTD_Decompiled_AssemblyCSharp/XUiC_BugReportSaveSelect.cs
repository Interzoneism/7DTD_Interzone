using System;
using UnityEngine.Scripting;

// Token: 0x02000C14 RID: 3092
[Preserve]
public class XUiC_BugReportSaveSelect : XUiController
{
	// Token: 0x06005EF1 RID: 24305 RVA: 0x00268737 File Offset: 0x00266937
	public override void Init()
	{
		base.Init();
		this.list = base.GetChildByType<XUiC_BugReportSavesList>();
	}

	// Token: 0x06005EF2 RID: 24306 RVA: 0x0026874B File Offset: 0x0026694B
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList();
	}

	// Token: 0x06005EF3 RID: 24307 RVA: 0x00268759 File Offset: 0x00266959
	[PublicizedFrom(EAccessModifier.Private)]
	public void RebuildList()
	{
		this.list.RebuildList(SaveInfoProvider.Instance.SaveEntryInfos, false);
	}

	// Token: 0x040047A3 RID: 18339
	public XUiC_BugReportSavesList list;
}

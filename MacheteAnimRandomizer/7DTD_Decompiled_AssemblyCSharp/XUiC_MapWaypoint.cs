using System;
using UnityEngine.Scripting;

// Token: 0x02000D21 RID: 3361
[Preserve]
public class XUiC_MapWaypoint : XUiController
{
	// Token: 0x060068A1 RID: 26785 RVA: 0x002A80BC File Offset: 0x002A62BC
	public override void Init()
	{
		base.Init();
		this.waypointList = (XUiC_MapWaypointList)base.Parent.GetChildById("waypointList");
	}

	// Token: 0x060068A2 RID: 26786 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleWaypointSetPressed(XUiController _sender, EventArgs _e)
	{
	}

	// Token: 0x04004EEC RID: 20204
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_MapWaypointList waypointList;
}

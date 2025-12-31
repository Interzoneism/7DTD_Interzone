using System;
using UnityEngine.Scripting;

// Token: 0x02000CA2 RID: 3234
[Preserve]
public class XUiC_ItemDronePartStackGrid : XUiC_ItemPartStackGrid
{
	// Token: 0x17000A2B RID: 2603
	// (get) Token: 0x060063D1 RID: 25553 RVA: 0x002874F5 File Offset: 0x002856F5
	// (set) Token: 0x060063D2 RID: 25554 RVA: 0x002874FD File Offset: 0x002856FD
	public EntityDrone CurrentVehicle { get; set; }

	// Token: 0x060063D3 RID: 25555 RVA: 0x00287508 File Offset: 0x00285708
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_ItemDronePartStack>(null);
		this.itemControllers = childrenByType;
	}
}

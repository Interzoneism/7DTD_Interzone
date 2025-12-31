using System;
using UnityEngine.Scripting;

// Token: 0x02000CC7 RID: 3271
[Preserve]
public class XUiC_InGameHUD : XUiController
{
	// Token: 0x06006531 RID: 25905 RVA: 0x00290864 File Offset: 0x0028EA64
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_HUDStatBar>(null);
		this.statBarList = childrenByType;
		this.IsDirty = true;
	}

	// Token: 0x06006532 RID: 25906 RVA: 0x0029088D File Offset: 0x0028EA8D
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
			this.IsDirty = true;
		}
	}

	// Token: 0x04004C71 RID: 19569
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController[] statBarList;
}

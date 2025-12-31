using System;
using UnityEngine;

// Token: 0x0200057D RID: 1405
public class LocalPlayerFinalCamera : MonoBehaviour
{
	// Token: 0x06002D64 RID: 11620 RVA: 0x0012E763 File Offset: 0x0012C963
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreCull()
	{
		this.entityPlayerLocal.finalCamera.fieldOfView = this.entityPlayerLocal.playerCamera.fieldOfView;
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x0012E785 File Offset: 0x0012C985
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreRender()
	{
		this.entityPlayerLocal.renderManager.DynamicResolutionRender();
	}

	// Token: 0x040023F6 RID: 9206
	public EntityPlayerLocal entityPlayerLocal;
}

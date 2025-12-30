using System;
using UnityEngine;

// Token: 0x0200104E RID: 4174
public class PlayerAutoPilotControllor
{
	// Token: 0x060083F0 RID: 33776 RVA: 0x003552AF File Offset: 0x003534AF
	public PlayerAutoPilotControllor(GameManager _gm)
	{
	}

	// Token: 0x060083F1 RID: 33777 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool IsEnabled()
	{
		return false;
	}

	// Token: 0x060083F2 RID: 33778 RVA: 0x00002914 File Offset: 0x00000B14
	public void Update()
	{
	}

	// Token: 0x060083F3 RID: 33779 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetForwardMovement()
	{
		return 0f;
	}

	// Token: 0x040065EB RID: 26091
	[PublicizedFrom(EAccessModifier.Private)]
	public int frameCnt;

	// Token: 0x040065EC RID: 26092
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 lastPosition = Vector3.zero;
}

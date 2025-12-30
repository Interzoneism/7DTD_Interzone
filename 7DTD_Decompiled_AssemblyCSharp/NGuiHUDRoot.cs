using System;
using UnityEngine;

// Token: 0x02001035 RID: 4149
public class NGuiHUDRoot : MonoBehaviour
{
	// Token: 0x06008350 RID: 33616 RVA: 0x0034F23E File Offset: 0x0034D43E
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		NGuiHUDRoot.go = base.gameObject;
	}

	// Token: 0x0400653E RID: 25918
	public static GameObject go;
}

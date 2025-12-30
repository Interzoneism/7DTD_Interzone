using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class OpenURLOnClick : MonoBehaviour
{
	// Token: 0x06000186 RID: 390 RVA: 0x0000EC7C File Offset: 0x0000CE7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnClick()
	{
		UILabel component = base.GetComponent<UILabel>();
		if (component != null)
		{
			string urlAtPosition = component.GetUrlAtPosition(UICamera.lastWorldPosition);
			if (!string.IsNullOrEmpty(urlAtPosition))
			{
				Application.OpenURL(urlAtPosition);
			}
		}
	}
}

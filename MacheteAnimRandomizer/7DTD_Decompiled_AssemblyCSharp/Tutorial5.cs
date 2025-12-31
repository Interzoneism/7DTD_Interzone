using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class Tutorial5 : MonoBehaviour
{
	// Token: 0x0600019A RID: 410 RVA: 0x0000F294 File Offset: 0x0000D494
	public void SetDurationToCurrentProgress()
	{
		UITweener[] componentsInChildren = base.GetComponentsInChildren<UITweener>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].duration = Mathf.Lerp(2f, 0.5f, UIProgressBar.current.value);
		}
	}
}

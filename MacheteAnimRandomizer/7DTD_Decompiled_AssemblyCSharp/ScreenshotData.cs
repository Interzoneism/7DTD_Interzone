using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class ScreenshotData : MonoBehaviour
{
	// Token: 0x060001FD RID: 509 RVA: 0x00002914 File Offset: 0x00000B14
	public void Start()
	{
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00002914 File Offset: 0x00000B14
	public void Update()
	{
	}

	// Token: 0x060001FF RID: 511 RVA: 0x000112DC File Offset: 0x0000F4DC
	public void OnGUI()
	{
		if (GameManager.Instance == null || GameManager.Instance.World == null || GameManager.Instance.World.GetPrimaryPlayer() == null)
		{
			return;
		}
		LocalPlayerUI uiforPrimaryPlayer = LocalPlayerUI.GetUIForPrimaryPlayer();
		if (uiforPrimaryPlayer == null || !uiforPrimaryPlayer.windowManager.IsHUDEnabled() || GameManager.Instance.ShowBackground())
		{
			return;
		}
		GUI.Label(new Rect(10f, 10f, 200f, 20f), GameManager.Instance.backgroundColor.ToCultureInvariantString());
	}
}

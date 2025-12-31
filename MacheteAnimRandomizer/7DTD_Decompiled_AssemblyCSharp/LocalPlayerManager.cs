using System;
using UnityEngine;

// Token: 0x0200057E RID: 1406
public class LocalPlayerManager
{
	// Token: 0x1400003B RID: 59
	// (add) Token: 0x06002D67 RID: 11623 RVA: 0x0012E798 File Offset: 0x0012C998
	// (remove) Token: 0x06002D68 RID: 11624 RVA: 0x0012E7CC File Offset: 0x0012C9CC
	public static event Action OnLocalPlayersChanged;

	// Token: 0x06002D69 RID: 11625 RVA: 0x0012E7FF File Offset: 0x0012C9FF
	public static void LocalPlayersChanged()
	{
		Action onLocalPlayersChanged = LocalPlayerManager.OnLocalPlayersChanged;
		if (onLocalPlayersChanged == null)
		{
			return;
		}
		onLocalPlayersChanged();
	}

	// Token: 0x06002D6A RID: 11626 RVA: 0x0012E810 File Offset: 0x0012CA10
	public static void Init()
	{
		GameManager.Instance.OnLocalPlayerChanged += LocalPlayerManager.HandleLocalPlayerChanged;
	}

	// Token: 0x06002D6B RID: 11627 RVA: 0x0012E828 File Offset: 0x0012CA28
	public static void Destroy()
	{
		GameManager.Instance.OnLocalPlayerChanged -= LocalPlayerManager.HandleLocalPlayerChanged;
	}

	// Token: 0x06002D6C RID: 11628 RVA: 0x0012E840 File Offset: 0x0012CA40
	[PublicizedFrom(EAccessModifier.Private)]
	public static void HandleLocalPlayerChanged(EntityPlayerLocal localPlayer)
	{
		if (localPlayer != null)
		{
			return;
		}
		foreach (LocalPlayerUI localPlayerUI in LocalPlayerUI.PlayerUIs)
		{
			if (!localPlayerUI.isPrimaryUI)
			{
				UnityEngine.Object.Destroy(localPlayerUI.gameObject);
			}
		}
	}
}

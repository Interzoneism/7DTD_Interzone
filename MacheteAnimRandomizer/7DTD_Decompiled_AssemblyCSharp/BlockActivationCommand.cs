using System;
using UnityEngine;

// Token: 0x020000E3 RID: 227
public struct BlockActivationCommand
{
	// Token: 0x06000551 RID: 1361 RVA: 0x00025DE6 File Offset: 0x00023FE6
	public BlockActivationCommand(string _text, string _icon, bool _enabled, bool _highlighted = false, string _eventName = null)
	{
		this.text = _text;
		this.icon = _icon;
		this.enabled = _enabled;
		this.highlighted = _highlighted;
		this.eventName = _eventName;
		this.iconColor = Color.white;
	}

	// Token: 0x0400060F RID: 1551
	public string text;

	// Token: 0x04000610 RID: 1552
	public string icon;

	// Token: 0x04000611 RID: 1553
	public Color iconColor;

	// Token: 0x04000612 RID: 1554
	public bool enabled;

	// Token: 0x04000613 RID: 1555
	public bool highlighted;

	// Token: 0x04000614 RID: 1556
	public string eventName;

	// Token: 0x04000615 RID: 1557
	public static readonly BlockActivationCommand[] Empty = Array.Empty<BlockActivationCommand>();
}

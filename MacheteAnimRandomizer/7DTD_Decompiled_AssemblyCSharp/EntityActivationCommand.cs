using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003A2 RID: 930
[Preserve]
public struct EntityActivationCommand
{
	// Token: 0x06001BB4 RID: 7092 RVA: 0x000ADB79 File Offset: 0x000ABD79
	public EntityActivationCommand(string _text, string _icon, bool _enabled, string _eventName = null)
	{
		this.text = _text;
		this.icon = _icon;
		this.enabled = _enabled;
		this.eventName = _eventName;
		this.iconColor = Color.white;
	}

	// Token: 0x04001286 RID: 4742
	public string text;

	// Token: 0x04001287 RID: 4743
	public string icon;

	// Token: 0x04001288 RID: 4744
	public Color iconColor;

	// Token: 0x04001289 RID: 4745
	public bool enabled;

	// Token: 0x0400128A RID: 4746
	public string eventName;
}

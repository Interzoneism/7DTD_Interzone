using System;

// Token: 0x02000D81 RID: 3457
public class ToolTipEvent
{
	// Token: 0x17000ADC RID: 2780
	// (get) Token: 0x06006C04 RID: 27652 RVA: 0x002C2FFC File Offset: 0x002C11FC
	// (set) Token: 0x06006C05 RID: 27653 RVA: 0x002C3004 File Offset: 0x002C1204
	public object Parameter { get; set; }

	// Token: 0x140000B7 RID: 183
	// (add) Token: 0x06006C06 RID: 27654 RVA: 0x002C3010 File Offset: 0x002C1210
	// (remove) Token: 0x06006C07 RID: 27655 RVA: 0x002C3048 File Offset: 0x002C1248
	public event ToolTipEventHandler EventHandler;

	// Token: 0x06006C08 RID: 27656 RVA: 0x002C307D File Offset: 0x002C127D
	public void HandleEvent()
	{
		ToolTipEventHandler eventHandler = this.EventHandler;
		if (eventHandler == null)
		{
			return;
		}
		eventHandler(this.Parameter);
	}
}

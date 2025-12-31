using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
[RequireComponent(typeof(UIWidget))]
public class SetColorPickerColor : MonoBehaviour
{
	// Token: 0x06000190 RID: 400 RVA: 0x0000F0A3 File Offset: 0x0000D2A3
	public void SetToCurrent()
	{
		if (this.mWidget == null)
		{
			this.mWidget = base.GetComponent<UIWidget>();
		}
		if (UIColorPicker.current != null)
		{
			this.mWidget.color = UIColorPicker.current.value;
		}
	}

	// Token: 0x0400023F RID: 575
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIWidget mWidget;
}

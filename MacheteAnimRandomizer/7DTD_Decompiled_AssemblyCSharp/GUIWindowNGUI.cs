using System;
using UnityEngine;

// Token: 0x02000FC4 RID: 4036
public class GUIWindowNGUI : GUIWindow
{
	// Token: 0x060080B1 RID: 32945 RVA: 0x00344100 File Offset: 0x00342300
	public GUIWindowNGUI(EnumNGUIWindow _nguiEnum) : base(_nguiEnum.ToStringCached<EnumNGUIWindow>(), default(Rect))
	{
		this.nguiEnum = _nguiEnum;
	}

	// Token: 0x060080B2 RID: 32946 RVA: 0x0034412C File Offset: 0x0034232C
	public GUIWindowNGUI(EnumNGUIWindow _nguiEnum, bool _bDrawBackground) : base(_nguiEnum.ToStringCached<EnumNGUIWindow>(), default(Rect), _bDrawBackground)
	{
		this.nguiEnum = _nguiEnum;
	}

	// Token: 0x060080B3 RID: 32947 RVA: 0x00344156 File Offset: 0x00342356
	public override void OnOpen()
	{
		this.nguiWindowManager.Show(this.nguiEnum, true);
	}

	// Token: 0x060080B4 RID: 32948 RVA: 0x0034416A File Offset: 0x0034236A
	public override void OnClose()
	{
		base.OnClose();
		this.nguiWindowManager.Show(this.nguiEnum, false);
	}

	// Token: 0x04006369 RID: 25449
	[PublicizedFrom(EAccessModifier.Protected)]
	public EnumNGUIWindow nguiEnum;
}

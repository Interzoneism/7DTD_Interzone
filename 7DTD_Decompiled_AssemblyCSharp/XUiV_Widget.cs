using System;
using UnityEngine;

// Token: 0x02000F0F RID: 3855
public class XUiV_Widget : XUiView
{
	// Token: 0x06007ACB RID: 31435 RVA: 0x0031811A File Offset: 0x0031631A
	public XUiV_Widget(string _id) : base(_id)
	{
	}

	// Token: 0x06007ACC RID: 31436 RVA: 0x00319FB5 File Offset: 0x003181B5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UIWidget>();
	}

	// Token: 0x06007ACD RID: 31437 RVA: 0x0031C2F8 File Offset: 0x0031A4F8
	public override void InitView()
	{
		base.InitView();
		this.widget = this.uiTransform.gameObject.GetComponent<UIWidget>();
		this.widget.depth = this.depth;
		this.widget.pivot = this.pivot;
		base.parseAnchors(this.widget, true);
		this.RefreshBoxCollider();
	}

	// Token: 0x06007ACE RID: 31438 RVA: 0x0031C356 File Offset: 0x0031A556
	public override void UpdateData()
	{
		if (this.isDirty)
		{
			this.widget.pivot = this.pivot;
			base.parseAnchors(this.widget, true);
			this.RefreshBoxCollider();
		}
		base.UpdateData();
	}

	// Token: 0x04005CE3 RID: 23779
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIWidget widget;
}

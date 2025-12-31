using System;
using UnityEngine;

// Token: 0x02000F09 RID: 3849
public class XUiV_Rect : XUiView
{
	// Token: 0x06007A3E RID: 31294 RVA: 0x0031811A File Offset: 0x0031631A
	public XUiV_Rect(string _id) : base(_id)
	{
	}

	// Token: 0x17000C98 RID: 3224
	// (get) Token: 0x06007A3F RID: 31295 RVA: 0x00319FA4 File Offset: 0x003181A4
	// (set) Token: 0x06007A40 RID: 31296 RVA: 0x00319FAC File Offset: 0x003181AC
	public bool DisableFallthrough
	{
		get
		{
			return this.disableFallthrough;
		}
		set
		{
			this.disableFallthrough = value;
		}
	}

	// Token: 0x06007A41 RID: 31297 RVA: 0x00319FB5 File Offset: 0x003181B5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UIWidget>();
	}

	// Token: 0x06007A42 RID: 31298 RVA: 0x00319FC0 File Offset: 0x003181C0
	public override void InitView()
	{
		base.InitView();
		this.widget = this.uiTransform.gameObject.GetComponent<UIWidget>();
		if (this.createUiWidget)
		{
			this.widget.enabled = true;
			UIWidget uiwidget = this.widget;
			uiwidget.onChange = (UIWidget.OnDimensionsChanged)Delegate.Combine(uiwidget.onChange, new UIWidget.OnDimensionsChanged(delegate()
			{
				this.isDirty = true;
			}));
		}
		else
		{
			UnityEngine.Object.Destroy(this.widget);
			this.widget = null;
		}
		this.UpdateData();
	}

	// Token: 0x06007A43 RID: 31299 RVA: 0x0031A040 File Offset: 0x00318240
	public override void UpdateData()
	{
		if (!this.initialized)
		{
			this.initialized = true;
			if (this.widget != null)
			{
				this.widget.pivot = this.pivot;
				this.widget.depth = this.depth;
				this.uiTransform.localScale = Vector3.one;
				this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
			}
		}
		if (this.widget != null)
		{
			this.widget.pivot = this.pivot;
			this.widget.depth = this.depth;
			this.widget.keepAspectRatio = this.keepAspectRatio;
			this.widget.aspectRatio = this.aspectRatio;
			this.widget.autoResizeBoxCollider = true;
			base.parseAnchors(this.widget, true);
		}
		base.UpdateData();
	}

	// Token: 0x06007A44 RID: 31300 RVA: 0x0031A13C File Offset: 0x0031833C
	public override void RefreshBoxCollider()
	{
		base.RefreshBoxCollider();
		if (this.disableFallthrough)
		{
			BoxCollider collider = this.collider;
			if (collider != null)
			{
				int num = 100;
				Vector3 center = collider.center;
				center.z = (float)num;
				collider.center = center;
			}
		}
	}

	// Token: 0x06007A45 RID: 31301 RVA: 0x0031A184 File Offset: 0x00318384
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(attribute, value, _parent);
		if (!flag)
		{
			if (!(attribute == "disablefallthrough"))
			{
				if (!(attribute == "createuiwidget"))
				{
					return false;
				}
				this.createUiWidget = StringParsers.ParseBool(value, 0, -1, true);
			}
			else
			{
				this.DisableFallthrough = StringParsers.ParseBool(value, 0, -1, true);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x04005C9D RID: 23709
	[PublicizedFrom(EAccessModifier.Private)]
	public bool createUiWidget;

	// Token: 0x04005C9E RID: 23710
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIWidget widget;

	// Token: 0x04005C9F RID: 23711
	[PublicizedFrom(EAccessModifier.Private)]
	public bool disableFallthrough;
}

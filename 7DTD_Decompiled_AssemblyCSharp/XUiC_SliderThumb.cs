using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E49 RID: 3657
[Preserve]
public class XUiC_SliderThumb : XUiController
{
	// Token: 0x17000BA6 RID: 2982
	// (get) Token: 0x060072DF RID: 29407 RVA: 0x002EDBB0 File Offset: 0x002EBDB0
	// (set) Token: 0x060072E0 RID: 29408 RVA: 0x002EDBD8 File Offset: 0x002EBDD8
	public float ThumbPosition
	{
		get
		{
			return (base.ViewComponent.UiTransform.localPosition.x - this.left) / this.width;
		}
		set
		{
			base.ViewComponent.Position = new Vector2i((int)(value * this.width + this.left), base.ViewComponent.Position.y);
			base.ViewComponent.UiTransform.localPosition = new Vector3((float)((int)(value * this.width + this.left)), (float)base.ViewComponent.Position.y, 0f);
		}
	}

	// Token: 0x17000BA7 RID: 2983
	// (get) Token: 0x060072E1 RID: 29409 RVA: 0x002EDC51 File Offset: 0x002EBE51
	public bool IsDragging
	{
		get
		{
			return this.isDragging;
		}
	}

	// Token: 0x060072E2 RID: 29410 RVA: 0x002EDC59 File Offset: 0x002EBE59
	public override void Init()
	{
		base.Init();
		base.ViewComponent.EventOnHover = true;
		this.sliderController = base.GetParentByType<XUiC_Slider>();
		this.sliderBarController = this.sliderController.GetChildByType<XUiC_SliderBar>();
	}

	// Token: 0x060072E3 RID: 29411 RVA: 0x002EDC8A File Offset: 0x002EBE8A
	public void SetDimensions(float _left, float _width)
	{
		this.left = _left;
		this.width = _width;
	}

	// Token: 0x060072E4 RID: 29412 RVA: 0x002EDC9A File Offset: 0x002EBE9A
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		base.OnHovered(_isOver);
		this.isOver = _isOver;
	}

	// Token: 0x060072E5 RID: 29413 RVA: 0x002EDCAC File Offset: 0x002EBEAC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDragged(EDragType _dragType, Vector2 _mousePositionDelta)
	{
		base.OnDragged(_dragType, _mousePositionDelta);
		if (!this.isDragging && !this.isOver)
		{
			return;
		}
		Vector2i mouseXUIPosition = base.xui.GetMouseXUIPosition();
		if (_dragType == EDragType.DragStart)
		{
			this.lastMousePos = mouseXUIPosition;
			this.isDragging = true;
		}
		else if (_dragType == EDragType.DragEnd)
		{
			this.isDragging = false;
		}
		if (mouseXUIPosition.x - this.lastMousePos.x != 0)
		{
			float num = base.ViewComponent.UiTransform.localPosition.x + (float)(mouseXUIPosition.x - this.lastMousePos.x);
			num = Mathf.Clamp(num, this.left, this.left + this.width);
			this.lastMousePos = mouseXUIPosition;
			base.ViewComponent.UiTransform.localPosition = new Vector3(num, base.ViewComponent.UiTransform.localPosition.y, base.ViewComponent.UiTransform.localPosition.z);
			base.ViewComponent.Position = new Vector2i((int)num, base.ViewComponent.Position.y);
			this.sliderController.ValueChanged(this.ThumbPosition);
			this.sliderController.IsDirty = true;
		}
	}

	// Token: 0x060072E6 RID: 29414 RVA: 0x002EDDDD File Offset: 0x002EBFDD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnScrolled(float _delta)
	{
		base.OnScrolled(_delta);
		if (this.sliderBarController != null)
		{
			this.sliderBarController.Scrolled(_delta);
		}
	}

	// Token: 0x04005784 RID: 22404
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i lastMousePos = new Vector2i(-100000, -100000);

	// Token: 0x04005785 RID: 22405
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x04005786 RID: 22406
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDragging;

	// Token: 0x04005787 RID: 22407
	[PublicizedFrom(EAccessModifier.Private)]
	public float left;

	// Token: 0x04005788 RID: 22408
	[PublicizedFrom(EAccessModifier.Private)]
	public float width;

	// Token: 0x04005789 RID: 22409
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Slider sliderController;

	// Token: 0x0400578A RID: 22410
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SliderBar sliderBarController;
}

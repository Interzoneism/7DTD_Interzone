using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E48 RID: 3656
[Preserve]
public class XUiC_SliderBar : XUiController
{
	// Token: 0x060072D9 RID: 29401 RVA: 0x002EDA1D File Offset: 0x002EBC1D
	public override void Init()
	{
		base.Init();
		this.sliderController = base.GetParentByType<XUiC_Slider>();
	}

	// Token: 0x060072DA RID: 29402 RVA: 0x002EDA34 File Offset: 0x002EBC34
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnPressed(int _mouseButton)
	{
		Vector2i mouseXUIPosition = base.xui.GetMouseXUIPosition();
		XUiController xuiController = this;
		Vector2i vector2i = xuiController.ViewComponent.Position;
		while (xuiController.Parent != null && xuiController.Parent.ViewComponent != null)
		{
			xuiController = xuiController.Parent;
			vector2i += xuiController.ViewComponent.Position;
		}
		vector2i += new Vector2i((int)xuiController.ViewComponent.UiTransform.parent.localPosition.x, (int)xuiController.ViewComponent.UiTransform.parent.localPosition.y);
		int num = (vector2i + base.ViewComponent.Size).x - vector2i.x;
		float newVal = (float)(mouseXUIPosition.x - vector2i.x) / (float)num;
		this.sliderController.ValueChanged(newVal);
		this.sliderController.updateThumb();
		this.sliderController.IsDirty = true;
	}

	// Token: 0x060072DB RID: 29403 RVA: 0x002EDB24 File Offset: 0x002EBD24
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnScrolled(float _delta)
	{
		base.OnScrolled(_delta);
		float num = this.sliderController.Value;
		num += Mathf.Clamp(_delta, -this.sliderController.Step, this.sliderController.Step);
		this.sliderController.ValueChanged(num);
		this.sliderController.updateThumb();
		this.sliderController.IsDirty = true;
	}

	// Token: 0x060072DC RID: 29404 RVA: 0x002EDB87 File Offset: 0x002EBD87
	public bool PageUpAction()
	{
		this.OnScrolled(this.sliderController.Step);
		return true;
	}

	// Token: 0x060072DD RID: 29405 RVA: 0x002EDB9B File Offset: 0x002EBD9B
	public bool PageDownAction()
	{
		this.OnScrolled(-this.sliderController.Step);
		return true;
	}

	// Token: 0x04005783 RID: 22403
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_Slider sliderController;
}

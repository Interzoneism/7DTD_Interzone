using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E47 RID: 3655
[Preserve]
public class XUiC_Slider : XUiController
{
	// Token: 0x140000C3 RID: 195
	// (add) Token: 0x060072CA RID: 29386 RVA: 0x002ED6FC File Offset: 0x002EB8FC
	// (remove) Token: 0x060072CB RID: 29387 RVA: 0x002ED734 File Offset: 0x002EB934
	public event XUiEvent_SliderValueChanged OnValueChanged;

	// Token: 0x17000BA3 RID: 2979
	// (get) Token: 0x060072CC RID: 29388 RVA: 0x002ED769 File Offset: 0x002EB969
	// (set) Token: 0x060072CD RID: 29389 RVA: 0x002ED771 File Offset: 0x002EB971
	public Func<float, string> ValueFormatter
	{
		get
		{
			return this.valueFormatter;
		}
		set
		{
			this.valueFormatter = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BA4 RID: 2980
	// (get) Token: 0x060072CE RID: 29390 RVA: 0x002ED781 File Offset: 0x002EB981
	// (set) Token: 0x060072CF RID: 29391 RVA: 0x002ED789 File Offset: 0x002EB989
	public string Label
	{
		get
		{
			return this.name;
		}
		set
		{
			this.name = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000BA5 RID: 2981
	// (get) Token: 0x060072D0 RID: 29392 RVA: 0x002ED799 File Offset: 0x002EB999
	// (set) Token: 0x060072D1 RID: 29393 RVA: 0x002ED7A1 File Offset: 0x002EB9A1
	public float Value
	{
		get
		{
			return this.val;
		}
		set
		{
			if (!this.thumbController.IsDragging && value != this.val)
			{
				this.val = Mathf.Clamp01(value);
				this.updateThumb();
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x060072D2 RID: 29394 RVA: 0x002ED7D4 File Offset: 0x002EB9D4
	public override void Init()
	{
		base.Init();
		this.thumbController = (base.GetChildById("thumb") as XUiC_SliderThumb);
		if (this.thumbController == null)
		{
			Log.Error("Thumb slider not found!");
			return;
		}
		this.thumbController.ViewComponent.IsNavigatable = (this.thumbController.ViewComponent.IsSnappable = false);
		this.barController = (base.GetChildById("bar") as XUiC_SliderBar);
		if (this.barController == null)
		{
			Log.Error("Thumb bar not found!");
			return;
		}
		this.left = (float)this.barController.ViewComponent.Position.x;
		this.width = (float)this.barController.ViewComponent.Size.x;
		this.thumbController.SetDimensions(this.left, this.width);
	}

	// Token: 0x060072D3 RID: 29395 RVA: 0x002ED8AC File Offset: 0x002EBAAC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.thumbController == null || this.thumbController.ViewComponent == null || float.IsNaN(this.left))
		{
			return;
		}
		if (this.IsDirty)
		{
			base.RefreshBindings(false);
			this.IsDirty = false;
		}
		if (base.xui.playerUI.CursorController.navigationTarget == this.barController.ViewComponent)
		{
			XUi.HandlePaging(base.xui, new Func<bool>(this.barController.PageUpAction), new Func<bool>(this.barController.PageDownAction), false);
		}
	}

	// Token: 0x060072D4 RID: 29396 RVA: 0x002ED949 File Offset: 0x002EBB49
	public void Reset()
	{
		this.initialized = false;
	}

	// Token: 0x060072D5 RID: 29397 RVA: 0x002ED954 File Offset: 0x002EBB54
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "name")
		{
			value = this.name;
			return true;
		}
		if (!(bindingName == "value"))
		{
			return false;
		}
		if (this.valueFormatter != null)
		{
			value = this.valueFormatter(this.val);
		}
		else
		{
			value = this.internalValueFormatter.Format(this.val);
		}
		return true;
	}

	// Token: 0x060072D6 RID: 29398 RVA: 0x002ED9BA File Offset: 0x002EBBBA
	public void ValueChanged(float _newVal)
	{
		this.val = Mathf.Clamp01(_newVal);
		if (this.OnValueChanged != null)
		{
			this.OnValueChanged(this);
		}
	}

	// Token: 0x060072D7 RID: 29399 RVA: 0x002ED9DC File Offset: 0x002EBBDC
	[PublicizedFrom(EAccessModifier.Internal)]
	public void updateThumb()
	{
		this.thumbController.ThumbPosition = this.val;
	}

	// Token: 0x04005778 RID: 22392
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SliderThumb thumbController;

	// Token: 0x04005779 RID: 22393
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SliderBar barController;

	// Token: 0x0400577A RID: 22394
	[PublicizedFrom(EAccessModifier.Protected)]
	public string name;

	// Token: 0x0400577B RID: 22395
	[PublicizedFrom(EAccessModifier.Protected)]
	public float val;

	// Token: 0x0400577C RID: 22396
	[PublicizedFrom(EAccessModifier.Protected)]
	public Func<float, string> valueFormatter;

	// Token: 0x0400577D RID: 22397
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool initialized;

	// Token: 0x0400577E RID: 22398
	[PublicizedFrom(EAccessModifier.Protected)]
	public float left = float.NaN;

	// Token: 0x0400577F RID: 22399
	[PublicizedFrom(EAccessModifier.Protected)]
	public float width;

	// Token: 0x04005780 RID: 22400
	public string Tag;

	// Token: 0x04005781 RID: 22401
	public float Step = 0.1f;

	// Token: 0x04005782 RID: 22402
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat internalValueFormatter = new CachedStringFormatterFloat("0.00");
}

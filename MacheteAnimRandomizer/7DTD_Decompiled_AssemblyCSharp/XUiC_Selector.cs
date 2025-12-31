using System;
using UnityEngine.Scripting;

// Token: 0x02000C4B RID: 3147
[Preserve]
public class XUiC_Selector : XUiController
{
	// Token: 0x14000097 RID: 151
	// (add) Token: 0x060060A2 RID: 24738 RVA: 0x0027414C File Offset: 0x0027234C
	// (remove) Token: 0x060060A3 RID: 24739 RVA: 0x00274184 File Offset: 0x00272384
	public event XUiEvent_SelectedIndexChanged OnSelectedIndexChanged;

	// Token: 0x060060A4 RID: 24740 RVA: 0x002741B9 File Offset: 0x002723B9
	public override void OnOpen()
	{
		base.OnOpen();
		this.currentValue.Text = this.selectedIndex.ToString();
	}

	// Token: 0x170009EF RID: 2543
	// (get) Token: 0x060060A5 RID: 24741 RVA: 0x002741D7 File Offset: 0x002723D7
	// (set) Token: 0x060060A6 RID: 24742 RVA: 0x002741DF File Offset: 0x002723DF
	public int SelectedIndex
	{
		get
		{
			return this.selectedIndex;
		}
		set
		{
			this.selectedIndex = value;
		}
	}

	// Token: 0x060060A7 RID: 24743 RVA: 0x002741E8 File Offset: 0x002723E8
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "min")
		{
			this.Min = int.Parse(value);
			return true;
		}
		if (!(name == "max"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		this.Max = int.Parse(value);
		return true;
	}

	// Token: 0x060060A8 RID: 24744 RVA: 0x00274238 File Offset: 0x00272438
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("forward");
		XUiController childById2 = base.GetChildById("back");
		XUiController childById3 = base.GetChildById("currentValue");
		if (childById != null)
		{
			childById.OnPress += this.ForwardButton_OnPress;
		}
		if (childById2 != null)
		{
			childById2.OnPress += this.BackButton_OnPress;
		}
		if (childById3 != null && childById3.ViewComponent is XUiV_Label)
		{
			this.currentValue = (childById3.ViewComponent as XUiV_Label);
		}
	}

	// Token: 0x060060A9 RID: 24745 RVA: 0x002742BA File Offset: 0x002724BA
	[PublicizedFrom(EAccessModifier.Private)]
	public void BackButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.selectedIndex--;
		this.BackPressed();
		this.IsDirty = true;
	}

	// Token: 0x060060AA RID: 24746 RVA: 0x002742D7 File Offset: 0x002724D7
	[PublicizedFrom(EAccessModifier.Private)]
	public void ForwardButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.selectedIndex++;
		this.ForwardPressed();
		this.IsDirty = true;
	}

	// Token: 0x060060AB RID: 24747 RVA: 0x002742F4 File Offset: 0x002724F4
	public virtual void BackPressed()
	{
		if (this.selectedIndex < this.Min)
		{
			this.selectedIndex = this.Max;
		}
		this.currentValue.Text = this.selectedIndex.ToString();
		if (this.OnSelectedIndexChanged != null)
		{
			this.OnSelectedIndexChanged(this.selectedIndex);
		}
	}

	// Token: 0x060060AC RID: 24748 RVA: 0x0027434C File Offset: 0x0027254C
	public virtual void ForwardPressed()
	{
		if (this.selectedIndex > this.Max)
		{
			this.selectedIndex = this.Min;
		}
		this.currentValue.Text = this.selectedIndex.ToString();
		if (this.OnSelectedIndexChanged != null)
		{
			this.OnSelectedIndexChanged(this.selectedIndex);
		}
	}

	// Token: 0x040048EC RID: 18668
	public int Min;

	// Token: 0x040048ED RID: 18669
	public int Max = int.MaxValue;

	// Token: 0x040048EE RID: 18670
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Label currentValue;

	// Token: 0x040048EF RID: 18671
	[PublicizedFrom(EAccessModifier.Protected)]
	public int selectedIndex;
}

using System;
using UnityEngine.Scripting;

// Token: 0x02000C62 RID: 3170
[Preserve]
public class XUiC_Counter : XUiController
{
	// Token: 0x1400009B RID: 155
	// (add) Token: 0x0600619B RID: 24987 RVA: 0x00279A04 File Offset: 0x00277C04
	// (remove) Token: 0x0600619C RID: 24988 RVA: 0x00279A3C File Offset: 0x00277C3C
	public event XUiEvent_OnCountChanged OnCountChanged;

	// Token: 0x17000A0A RID: 2570
	// (get) Token: 0x0600619D RID: 24989 RVA: 0x00279A71 File Offset: 0x00277C71
	// (set) Token: 0x0600619E RID: 24990 RVA: 0x00279A79 File Offset: 0x00277C79
	public int Count
	{
		get
		{
			return this.count;
		}
		set
		{
			this.count = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x0600619F RID: 24991 RVA: 0x00279A8C File Offset: 0x00277C8C
	public override void Init()
	{
		base.Init();
		this.countUp = base.GetChildById("countUp");
		this.countDown = base.GetChildById("countDown");
		this.countMax = base.GetChildById("countMax");
		this.counter = base.GetChildById("text");
		this.countUp.OnPress += this.HandleCountUpOnPress;
		this.countDown.OnPress += this.HandleCountDownOnPress;
		this.countMax.OnPress += this.HandleMaxCountOnPress;
		this.countUp.OnHold += delegate(XUiController _sender, EHoldType _event, float _duration, float _timedEvent)
		{
			if (_event == EHoldType.HoldTimed)
			{
				this.HandleCountUpOnPress(_sender, -1);
			}
		};
		this.countDown.OnHold += delegate(XUiController _sender, EHoldType _event, float _duration, float _timedEvent)
		{
			if (_event == EHoldType.HoldTimed)
			{
				this.HandleCountDownOnPress(_sender, -1);
			}
		};
		this.textInputChangedDelegate = new XUiEvent_InputOnChangedEventHandler(this.TextInput_OnChangeHandler);
		this.textInput = base.GetChildByType<XUiC_TextInput>();
		this.textInput.OnChangeHandler += this.textInputChangedDelegate;
		this.textInput.OnInputSelectedHandler += this.TextInput_HandleInputDeselected;
	}

	// Token: 0x060061A0 RID: 24992 RVA: 0x00279BA0 File Offset: 0x00277DA0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleCountChangedEvent()
	{
		if (this.OnCountChanged != null)
		{
			OnCountChangedEventArgs onCountChangedEventArgs = new OnCountChangedEventArgs();
			onCountChangedEventArgs.Count = this.Count;
			this.OnCountChanged(this, onCountChangedEventArgs);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060061A1 RID: 24993 RVA: 0x00279BDC File Offset: 0x00277DDC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void TextInput_OnChangeHandler(XUiController _sender, string _text, bool _changeFromCode)
	{
		int num = 0;
		if (int.TryParse(_text, out num))
		{
			this.Count = num;
			if (this.Count > this.MaxCount)
			{
				this.Count = this.MaxCount;
				((XUiC_TextInput)_sender).Text = this.Count.ToString();
			}
			else if (this.Count <= 0)
			{
				this.Count = this.Step;
				((XUiC_TextInput)_sender).Text = this.Count.ToString();
			}
			this.HandleCountChangedEvent();
			return;
		}
		this.Count = this.Step;
		this.HandleCountChangedEvent();
	}

	// Token: 0x060061A2 RID: 24994 RVA: 0x00279C78 File Offset: 0x00277E78
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleMaxCountOnPress(XUiController _sender, int _mouseButton)
	{
		this.Count = this.MaxCount;
		this.HandleCountChangedEvent();
	}

	// Token: 0x060061A3 RID: 24995 RVA: 0x00279C8C File Offset: 0x00277E8C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.textInput.OnChangeHandler -= this.textInputChangedDelegate;
			((XUiV_Label)this.counter.ViewComponent).Text = (this.textInput.Text = ((this.textInput.Text == "") ? "" : ((this.Count > 0) ? this.Count.ToString() : "-")));
			this.textInput.OnChangeHandler += this.textInputChangedDelegate;
			this.IsDirty = false;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x060061A4 RID: 24996 RVA: 0x00279D3A File Offset: 0x00277F3A
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleCountDownOnPress(XUiController _sender, int _mouseButton)
	{
		if (this.Count > 1)
		{
			this.Count -= this.Step;
			this.HandleStepClamping();
		}
		this.HandleCountChangedEvent();
		this.ForceTextRefresh();
	}

	// Token: 0x060061A5 RID: 24997 RVA: 0x00279D6A File Offset: 0x00277F6A
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleCountUpOnPress(XUiController _sender, int _mouseButton)
	{
		if (this.Count < this.MaxCount)
		{
			this.Count += this.Step;
			this.HandleStepClamping();
			this.HandleCountChangedEvent();
			this.ForceTextRefresh();
		}
	}

	// Token: 0x060061A6 RID: 24998 RVA: 0x00279D9F File Offset: 0x00277F9F
	public void SetToMaxCount()
	{
		this.Count = this.MaxCount;
		this.HandleStepClamping();
		this.HandleCountChangedEvent();
		this.ForceTextRefresh();
	}

	// Token: 0x060061A7 RID: 24999 RVA: 0x00279DBF File Offset: 0x00277FBF
	public void SetCount(int count)
	{
		if (this.Count != count)
		{
			this.Count = count;
			this.HandleCountChangedEvent();
		}
	}

	// Token: 0x060061A8 RID: 25000 RVA: 0x00279DD8 File Offset: 0x00277FD8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleStepClamping()
	{
		if (this.Step > 1)
		{
			int num = this.Count % this.Step;
			if (num != 0)
			{
				this.Count -= num;
			}
		}
	}

	// Token: 0x060061A9 RID: 25001 RVA: 0x00279E10 File Offset: 0x00278010
	public void ForceTextRefresh()
	{
		((XUiV_Label)this.counter.ViewComponent).Text = (this.textInput.Text = this.count.ToString());
	}

	// Token: 0x060061AA RID: 25002 RVA: 0x00279E4B File Offset: 0x0027804B
	[PublicizedFrom(EAccessModifier.Protected)]
	public void TextInput_HandleInputDeselected(XUiController _sender, bool _selected)
	{
		if (!_selected)
		{
			this.ForceTextRefresh();
		}
	}

	// Token: 0x04004961 RID: 18785
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController countUp;

	// Token: 0x04004962 RID: 18786
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController countDown;

	// Token: 0x04004963 RID: 18787
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController countMax;

	// Token: 0x04004964 RID: 18788
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController counter;

	// Token: 0x04004965 RID: 18789
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiC_TextInput textInput;

	// Token: 0x04004966 RID: 18790
	[PublicizedFrom(EAccessModifier.Private)]
	public int count = 1;

	// Token: 0x04004967 RID: 18791
	public int MaxCount = 1;

	// Token: 0x04004968 RID: 18792
	public int Step = 1;

	// Token: 0x04004969 RID: 18793
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiEvent_InputOnChangedEventHandler textInputChangedDelegate;
}

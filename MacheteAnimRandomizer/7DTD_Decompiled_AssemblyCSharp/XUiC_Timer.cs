using System;
using GUI_2;
using UnityEngine.Scripting;

// Token: 0x02000E6D RID: 3693
[Preserve]
public class XUiC_Timer : XUiController
{
	// Token: 0x060073FF RID: 29695 RVA: 0x002F2E48 File Offset: 0x002F1048
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.CursorController.SetCursorHidden(true);
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.CancelButton, "igcoCancel", XUiC_GamepadCalloutWindow.CalloutType.Menu);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu, 0f);
	}

	// Token: 0x06007400 RID: 29696 RVA: 0x002F2EB0 File Offset: 0x002F10B0
	public override void OnClose()
	{
		base.OnClose();
		this.UpdateTimer(0f, 1f);
		this.fullTime = 0f;
		base.xui.playerUI.CursorController.SetCursorHidden(false);
		base.xui.calloutWindow.DisableCallouts(XUiC_GamepadCalloutWindow.CalloutType.Menu);
		if (this.timeLeft > 0f)
		{
			this.eventData.timeLeft = this.timeLeft;
			this.eventData.HandleCloseEvent(this.timeLeft);
		}
		base.xui.playerUI.entityPlayer.SetControllable(true);
		CursorControllerAbs.SetCursor(CursorControllerAbs.ECursorType.Default);
	}

	// Token: 0x06007401 RID: 29697 RVA: 0x002F2F50 File Offset: 0x002F1150
	public void UpdateTimer(float _timeLeft, float _fillAmount)
	{
		this.currentTimeLeft = _timeLeft;
		this.currentFillAmount = _fillAmount;
		base.RefreshBindings(false);
	}

	// Token: 0x06007402 RID: 29698 RVA: 0x002F2F68 File Offset: 0x002F1168
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.fullTime > 0f)
		{
			if (this.eventData.CloseOnHit && base.xui.playerUI.entityPlayer != null && base.xui.playerUI.entityPlayer.hasBeenAttackedTime > 0)
			{
				base.xui.playerUI.windowManager.Close("timer");
				return;
			}
			this.timeLeft -= _dt;
			float fillAmount = this.timeLeft / this.fullTime;
			this.UpdateTimer(this.timeLeft, fillAmount);
			if (this.eventData.alternateTime != -1f && this.fullTime - this.timeLeft > this.eventData.alternateTime)
			{
				this.eventData.timeLeft = this.timeLeft;
				this.timeLeft = 0f;
				this.fullTime = 0f;
				base.xui.playerUI.windowManager.Close("timer");
				GameManager.Instance.SetPauseWindowEffects(false);
				base.xui.dragAndDrop.InMenu = false;
				this.eventData.HandleAlternateEvent();
				return;
			}
			if (this.timeLeft <= 0f)
			{
				this.timeLeft = 0f;
				this.fullTime = 0f;
				base.xui.playerUI.windowManager.Close("timer");
				GameManager.Instance.SetPauseWindowEffects(false);
				base.xui.dragAndDrop.InMenu = false;
				this.eventData.HandleEvent();
			}
		}
	}

	// Token: 0x06007403 RID: 29699 RVA: 0x002F310C File Offset: 0x002F130C
	public void SetTimer(float _fullTime, TimerEventData _eventData, float startTime = -1f, string _labelText = "")
	{
		this.currentOpenEventText = _labelText;
		this.fullTime = _fullTime;
		if (startTime == -1f)
		{
			this.timeLeft = this.fullTime;
		}
		else
		{
			this.timeLeft = startTime;
		}
		this.eventData = _eventData;
		this.UpdateTimer(this.timeLeft, this.timeLeft / this.fullTime);
	}

	// Token: 0x06007404 RID: 29700 RVA: 0x002F3168 File Offset: 0x002F1368
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "timeleft")
		{
			_value = this.currentTimeLeft.ToCultureInvariantString("0.0");
			return true;
		}
		if (_bindingName == "percent")
		{
			_value = this.currentFillAmount.ToCultureInvariantString();
			return true;
		}
		if (!(_bindingName == "caption"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = (this.currentOpenEventText ?? "");
		return true;
	}

	// Token: 0x0400583C RID: 22588
	[PublicizedFrom(EAccessModifier.Private)]
	public float fullTime;

	// Token: 0x0400583D RID: 22589
	[PublicizedFrom(EAccessModifier.Private)]
	public float timeLeft;

	// Token: 0x0400583E RID: 22590
	public string currentOpenEventText;

	// Token: 0x0400583F RID: 22591
	[PublicizedFrom(EAccessModifier.Private)]
	public float currentFillAmount;

	// Token: 0x04005840 RID: 22592
	[PublicizedFrom(EAccessModifier.Private)]
	public float currentTimeLeft;

	// Token: 0x04005841 RID: 22593
	[PublicizedFrom(EAccessModifier.Private)]
	public TimerEventData eventData;
}

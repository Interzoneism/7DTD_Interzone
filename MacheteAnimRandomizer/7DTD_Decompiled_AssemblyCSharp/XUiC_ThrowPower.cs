using System;
using UnityEngine.Scripting;

// Token: 0x02000E6C RID: 3692
[Preserve]
public class XUiC_ThrowPower : XUiController
{
	// Token: 0x17000BC8 RID: 3016
	// (get) Token: 0x060073F8 RID: 29688 RVA: 0x002F2D7D File Offset: 0x002F0F7D
	// (set) Token: 0x060073F9 RID: 29689 RVA: 0x002F2D85 File Offset: 0x002F0F85
	public float CurrentPower
	{
		get
		{
			return this.currentPower;
		}
		set
		{
			if (value != this.currentPower)
			{
				this.currentPower = value;
				base.RefreshBindings(false);
			}
		}
	}

	// Token: 0x060073FA RID: 29690 RVA: 0x002F2D9E File Offset: 0x002F0F9E
	public override void OnOpen()
	{
		base.OnOpen();
		((XUiV_Window)this.viewComponent).ForceVisible(-1f);
	}

	// Token: 0x060073FB RID: 29691 RVA: 0x002F2DBB File Offset: 0x002F0FBB
	public override void OnClose()
	{
		base.OnClose();
		this.CurrentPower = 0f;
	}

	// Token: 0x060073FC RID: 29692 RVA: 0x002F2DCE File Offset: 0x002F0FCE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "fill")
		{
			_value = this.currentPower.ToCultureInvariantString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060073FD RID: 29693 RVA: 0x002F2DF4 File Offset: 0x002F0FF4
	public static void Status(LocalPlayerUI _playerUi, float _currentPower = -1f)
	{
		XUiC_ThrowPower windowByType = _playerUi.xui.GetWindowByType<XUiC_ThrowPower>();
		if (windowByType != null)
		{
			windowByType.CurrentPower = _currentPower;
			if (_currentPower >= 0f)
			{
				_playerUi.windowManager.Open(windowByType.windowGroup, false, true, true);
				return;
			}
			_playerUi.windowManager.Close(windowByType.windowGroup, false);
		}
	}

	// Token: 0x0400583B RID: 22587
	[PublicizedFrom(EAccessModifier.Private)]
	public float currentPower;
}

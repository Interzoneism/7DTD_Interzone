using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CCA RID: 3274
[Preserve]
public class XUiC_InGameTimeControls : XUiController
{
	// Token: 0x17000A54 RID: 2644
	// (get) Token: 0x0600654F RID: 25935 RVA: 0x0028FBBD File Offset: 0x0028DDBD
	public static bool HasWorld
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return GameManager.Instance.World != null;
		}
	}

	// Token: 0x17000A55 RID: 2645
	// (get) Token: 0x06006550 RID: 25936 RVA: 0x002911CF File Offset: 0x0028F3CF
	public static int CurrentSpeed
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return GameStats.GetInt(EnumGameStats.TimeOfDayIncPerSec);
		}
	}

	// Token: 0x06006551 RID: 25937 RVA: 0x002911D8 File Offset: 0x0028F3D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "is_paused")
		{
			_value = (XUiC_InGameTimeControls.CurrentSpeed == 0).ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x06006552 RID: 25938 RVA: 0x00291210 File Offset: 0x0028F410
	public override void Init()
	{
		base.Init();
		GameStats.OnChangedDelegates += this.onGameStatsChanged;
		XUiController childById = base.GetChildById("cbxDay");
		this.cbxDay = ((childById != null) ? childById.GetChildByType<XUiC_ComboBoxInt>() : null);
		if (this.cbxDay != null)
		{
			this.cbxDay.OnValueChanged += delegate(XUiController _, long _, long _newValue)
			{
				this.setDay((int)_newValue);
			};
		}
		XUiController childById2 = base.GetChildById("cbxTime");
		this.cbxTime = ((childById2 != null) ? childById2.GetChildByType<XUiC_ComboBoxFloat>() : null);
		if (this.cbxTime != null)
		{
			this.cbxTime.OnValueChanged += delegate(XUiController _, double _, double _newValue)
			{
				this.setTime((float)_newValue);
			};
			this.cbxTime.CustomValueFormatter = delegate(double _value)
			{
				int num = (int)_value;
				int num2 = (int)((_value - (double)num) * 60.0);
				return string.Format("{0:00}:{1:00}", num, num2);
			};
		}
		XUiController childById3 = base.GetChildById("cbxSpeed");
		this.cbxSpeed = ((childById3 != null) ? childById3.GetChildByType<XUiC_ComboBoxInt>() : null);
		if (this.cbxSpeed != null)
		{
			this.cbxSpeed.OnValueChanged += delegate(XUiController _, long _, long _newValue)
			{
				this.setSpeed((int)_newValue);
			};
		}
		XUiController childById4 = base.GetChildById("btnTimeSkipBack");
		XUiV_Button xuiV_Button = ((childById4 != null) ? childById4.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button != null)
		{
			xuiV_Button.Controller.OnPress += this.btnTimeSkipBackPressed;
		}
		XUiController childById5 = base.GetChildById("btnTimeSlower");
		XUiV_Button xuiV_Button2 = ((childById5 != null) ? childById5.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button2 != null)
		{
			xuiV_Button2.Controller.OnPress += this.btnTimeSlowerPressed;
		}
		XUiController childById6 = base.GetChildById("btnTimePlayPause");
		XUiV_Button xuiV_Button3 = ((childById6 != null) ? childById6.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button3 != null)
		{
			xuiV_Button3.Controller.OnPress += this.btnTimePlayPausePressed;
		}
		XUiController childById7 = base.GetChildById("btnTimeFaster");
		XUiV_Button xuiV_Button4 = ((childById7 != null) ? childById7.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button4 != null)
		{
			xuiV_Button4.Controller.OnPress += this.btnTimeFasterPressed;
		}
		XUiController childById8 = base.GetChildById("btnTimeSkipForward");
		XUiV_Button xuiV_Button5 = ((childById8 != null) ? childById8.ViewComponent : null) as XUiV_Button;
		if (xuiV_Button5 != null)
		{
			xuiV_Button5.Controller.OnPress += this.btnTimeSkipForwardPressed;
		}
	}

	// Token: 0x06006553 RID: 25939 RVA: 0x00291428 File Offset: 0x0028F628
	public override void Cleanup()
	{
		GameStats.OnChangedDelegates -= this.onGameStatsChanged;
		base.Cleanup();
	}

	// Token: 0x06006554 RID: 25940 RVA: 0x00291444 File Offset: 0x0028F644
	[PublicizedFrom(EAccessModifier.Private)]
	public void onGameStatsChanged(EnumGameStats _stat, object _value)
	{
		if (_stat != EnumGameStats.TimeOfDayIncPerSec)
		{
			return;
		}
		int num = (int)_value;
		if (num > 0)
		{
			this.lastNonZeroSpeed = num;
		}
	}

	// Token: 0x06006555 RID: 25941 RVA: 0x0029146C File Offset: 0x0028F66C
	[PublicizedFrom(EAccessModifier.Private)]
	public void setDay(int _day)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		ulong num = GameManager.Instance.World.worldTime % 24000UL;
		GameManager.Instance.World.SetTimeJump(num + (ulong)((long)(_day - 1) * 24000L), true);
	}

	// Token: 0x06006556 RID: 25942 RVA: 0x002914BC File Offset: 0x0028F6BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void setTime(float _dayTime)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		ulong num = GameManager.Instance.World.worldTime / 24000UL;
		int num2 = Mathf.Clamp((int)(_dayTime * 1000f), 0, 23999);
		GameManager.Instance.World.SetTimeJump(num * 24000UL + (ulong)((long)num2), true);
	}

	// Token: 0x06006557 RID: 25943 RVA: 0x0029151C File Offset: 0x0028F71C
	[PublicizedFrom(EAccessModifier.Private)]
	public void setSpeed(int _value)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		GameStats.Set(EnumGameStats.TimeOfDayIncPerSec, _value);
		base.RefreshBindings(false);
	}

	// Token: 0x06006558 RID: 25944 RVA: 0x0029153C File Offset: 0x0028F73C
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTimeSkipBackPressed(XUiController _sender, int _mouseButton)
	{
		float num = GameManager.Instance.World.worldTime % 24000UL / 1000f - 0.050000004f;
		for (int i = this.skipHours.Length - 1; i >= 0; i--)
		{
			float num2 = this.skipHours[i];
			if (num2 < num)
			{
				this.setTime(num2);
				return;
			}
		}
	}

	// Token: 0x06006559 RID: 25945 RVA: 0x00291598 File Offset: 0x0028F798
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTimeSlowerPressed(XUiController _sender, int _mouseButton)
	{
		this.setSpeed((int)MathUtils.ToPreviousPowerOfTwo((uint)XUiC_InGameTimeControls.CurrentSpeed, false));
	}

	// Token: 0x0600655A RID: 25946 RVA: 0x002915AB File Offset: 0x0028F7AB
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTimePlayPausePressed(XUiController _sender, int _mouseButton)
	{
		this.setSpeed((XUiC_InGameTimeControls.CurrentSpeed == 0) ? this.lastNonZeroSpeed : 0);
	}

	// Token: 0x0600655B RID: 25947 RVA: 0x002915C3 File Offset: 0x0028F7C3
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTimeFasterPressed(XUiController _sender, int _mouseButton)
	{
		this.setSpeed((int)MathUtils.ToNextPowerOfTwo((uint)XUiC_InGameTimeControls.CurrentSpeed, false));
	}

	// Token: 0x0600655C RID: 25948 RVA: 0x002915D8 File Offset: 0x0028F7D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnTimeSkipForwardPressed(XUiController _sender, int _mouseButton)
	{
		float num = GameManager.Instance.World.worldTime % 24000UL / 1000f + 0.050000004f;
		for (int i = 0; i < this.skipHours.Length; i++)
		{
			float num2 = this.skipHours[i];
			if (num2 > num)
			{
				this.setTime(num2);
				return;
			}
		}
	}

	// Token: 0x0600655D RID: 25949 RVA: 0x00291634 File Offset: 0x0028F834
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.cbxDay != null)
		{
			this.cbxDay.Value = (long)(XUiC_InGameTimeControls.HasWorld ? (GameManager.Instance.World.worldTime / 24000UL + 1UL) : 0UL);
		}
		if (this.cbxTime != null)
		{
			this.cbxTime.Value = (double)(XUiC_InGameTimeControls.HasWorld ? (GameManager.Instance.World.worldTime % 24000UL / 1000f) : 0f);
		}
		if (this.cbxSpeed != null)
		{
			this.cbxSpeed.Value = (long)XUiC_InGameTimeControls.CurrentSpeed;
		}
		base.RefreshBindings(true);
	}

	// Token: 0x04004C83 RID: 19587
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly float[] skipHours = new float[]
	{
		0f,
		4f,
		12f,
		22f
	};

	// Token: 0x04004C84 RID: 19588
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt cbxDay;

	// Token: 0x04004C85 RID: 19589
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat cbxTime;

	// Token: 0x04004C86 RID: 19590
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxInt cbxSpeed;

	// Token: 0x04004C87 RID: 19591
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastNonZeroSpeed = 1;
}

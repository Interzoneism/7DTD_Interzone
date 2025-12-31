using System;
using UnityEngine.Scripting;

// Token: 0x02000CB4 RID: 3252
[Preserve]
public class XUiC_FocusedBlockHealth : XUiController
{
	// Token: 0x17000A43 RID: 2627
	// (get) Token: 0x06006494 RID: 25748 RVA: 0x0028BFEE File Offset: 0x0028A1EE
	// (set) Token: 0x06006495 RID: 25749 RVA: 0x0028BFF6 File Offset: 0x0028A1F6
	public string Text
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.text;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (value != this.text)
			{
				this.text = (value ?? "");
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000A44 RID: 2628
	// (get) Token: 0x06006496 RID: 25750 RVA: 0x0028C01D File Offset: 0x0028A21D
	// (set) Token: 0x06006497 RID: 25751 RVA: 0x0028C025 File Offset: 0x0028A225
	public float Fill
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.fill;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (value != this.fill)
			{
				this.fill = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x06006498 RID: 25752 RVA: 0x0028C03E File Offset: 0x0028A23E
	public override void Init()
	{
		base.Init();
		XUiC_FocusedBlockHealth.ID = base.WindowGroup.ID;
	}

	// Token: 0x06006499 RID: 25753 RVA: 0x0028C056 File Offset: 0x0028A256
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x0600649A RID: 25754 RVA: 0x0028C075 File Offset: 0x0028A275
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "text")
		{
			_value = this.text;
			return true;
		}
		if (!(_bindingName == "fill"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.fill.ToCultureInvariantString();
		return true;
	}

	// Token: 0x0600649B RID: 25755 RVA: 0x0028C0B4 File Offset: 0x0028A2B4
	public static void SetData(LocalPlayerUI _playerUi, string _text, float _fill)
	{
		if (_playerUi != null && _playerUi.xui != null)
		{
			XUiController xuiController = _playerUi.xui.FindWindowGroupByName(XUiC_FocusedBlockHealth.ID);
			XUiC_FocusedBlockHealth xuiC_FocusedBlockHealth = (xuiController != null) ? xuiController.GetChildByType<XUiC_FocusedBlockHealth>() : null;
			if (xuiC_FocusedBlockHealth == null)
			{
				return;
			}
			xuiC_FocusedBlockHealth.Text = _text;
			xuiC_FocusedBlockHealth.Fill = _fill;
			if (_text == null)
			{
				_playerUi.windowManager.Close(XUiC_FocusedBlockHealth.ID);
				return;
			}
			_playerUi.windowManager.Open(XUiC_FocusedBlockHealth.ID, false, false, false);
		}
	}

	// Token: 0x0600649C RID: 25756 RVA: 0x0028C12E File Offset: 0x0028A32E
	public static bool IsWindowOpen(LocalPlayerUI _playerUi)
	{
		return _playerUi.windowManager.IsWindowOpen(XUiC_FocusedBlockHealth.ID);
	}

	// Token: 0x04004BDD RID: 19421
	public static string ID = "";

	// Token: 0x04004BDE RID: 19422
	[PublicizedFrom(EAccessModifier.Private)]
	public string text = "";

	// Token: 0x04004BDF RID: 19423
	[PublicizedFrom(EAccessModifier.Private)]
	public float fill;
}

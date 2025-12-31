using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E98 RID: 3736
[Preserve]
public class XUiC_UiLimitsWindow : XUiController
{
	// Token: 0x060075F0 RID: 30192 RVA: 0x0028C056 File Offset: 0x0028A256
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			this.IsDirty = false;
		}
	}

	// Token: 0x060075F1 RID: 30193 RVA: 0x00300B04 File Offset: 0x002FED04
	public override void OnOpen()
	{
		base.OnOpen();
		int manualHeight = UnityEngine.Object.FindObjectOfType<UIRoot>().manualHeight;
		float scale = base.xui.GetScale();
		this.availableXuiHeight = (float)manualHeight / scale;
		base.RefreshBindings(true);
	}

	// Token: 0x060075F2 RID: 30194 RVA: 0x00300B40 File Offset: 0x002FED40
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName.StartsWith("width_", StringComparison.Ordinal))
		{
			return this.handleArBinding(ref _value, _bindingName);
		}
		if (_bindingName == "height")
		{
			_value = Mathf.FloorToInt(this.availableXuiHeight).ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x060075F3 RID: 30195 RVA: 0x00300B90 File Offset: 0x002FED90
	[PublicizedFrom(EAccessModifier.Private)]
	public bool handleArBinding(ref string _value, string _bindingName)
	{
		int num = _bindingName.IndexOf('_', "width_".Length);
		if (num < 0)
		{
			return false;
		}
		ReadOnlySpan<char> s = _bindingName.AsSpan("width_".Length, num - "width_".Length);
		ReadOnlySpan<char> s2 = _bindingName.AsSpan(num + 1);
		int num2;
		if (!int.TryParse(s, out num2))
		{
			return false;
		}
		int num3;
		if (!int.TryParse(s2, out num3))
		{
			return false;
		}
		double uiSizeLimit = GameOptionsManager.GetUiSizeLimit((double)num2 / (double)num3);
		int num4 = Mathf.RoundToInt((float)((double)(this.availableXuiHeight / (float)num3 * (float)num2) / uiSizeLimit));
		if (num4 % 2 > 0)
		{
			num4--;
		}
		_value = num4.ToString();
		return true;
	}

	// Token: 0x040059FB RID: 23035
	[PublicizedFrom(EAccessModifier.Private)]
	public float availableXuiHeight = 1f;

	// Token: 0x040059FC RID: 23036
	[PublicizedFrom(EAccessModifier.Private)]
	public const string bindingWidthPrefix = "width_";
}

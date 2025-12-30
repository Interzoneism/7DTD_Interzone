using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C8A RID: 3210
[Preserve]
public class XUiC_DewCollectorStack : XUiC_RequiredItemStack
{
	// Token: 0x17000A19 RID: 2585
	// (get) Token: 0x06006303 RID: 25347 RVA: 0x00283630 File Offset: 0x00281830
	// (set) Token: 0x06006304 RID: 25348 RVA: 0x00283638 File Offset: 0x00281838
	public bool IsModded
	{
		get
		{
			return this.isModded;
		}
		set
		{
			this.isModded = value;
			this.currentFillColor = (this.isModded ? this.moddedFillColor : this.standardFillColor);
		}
	}

	// Token: 0x17000A1A RID: 2586
	// (get) Token: 0x06006305 RID: 25349 RVA: 0x0028365D File Offset: 0x0028185D
	// (set) Token: 0x06006306 RID: 25350 RVA: 0x00283665 File Offset: 0x00281865
	public float FillAmount
	{
		get
		{
			return this.fillAmount;
		}
		set
		{
			if (this.fillAmount != value)
			{
				this.fillAmount = value;
			}
		}
	}

	// Token: 0x06006307 RID: 25351 RVA: 0x00283678 File Offset: 0x00281878
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("fillIcon");
		if (childById != null)
		{
			this.fillIcon = (childById.ViewComponent as XUiV_Sprite);
		}
	}

	// Token: 0x06006308 RID: 25352 RVA: 0x002836AC File Offset: 0x002818AC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsBlocked)
		{
			this.fillIcon.Color = this.blockedColor;
			return;
		}
		if (this.IsCurrentStack)
		{
			float num = Mathf.PingPong(Time.time, 0.5f);
			this.fillIcon.Color = Color.Lerp(Color.grey, this.currentFillColor, num * 4f);
		}
		else if (this.fillIcon.Color != this.disabledColor)
		{
			this.fillIcon.Color = this.disabledColor;
		}
		base.ViewComponent.IsNavigatable = !base.ItemStack.IsEmpty();
	}

	// Token: 0x06006309 RID: 25353 RVA: 0x00283758 File Offset: 0x00281958
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "hasfill")
		{
			_value = (this.IsCurrentStack ? (this.FillAmount != -1f).ToString() : "false");
			return true;
		}
		if (_bindingName == "waterfill")
		{
			_value = (this.IsCurrentStack ? (this.FillAmount / this.MaxFill).ToString() : "0");
			return true;
		}
		if (_bindingName == "showitem")
		{
			_value = (!this.itemStack.IsEmpty()).ToString();
			return true;
		}
		if (_bindingName == "fillcolor")
		{
			if (this.IsBlocked)
			{
				_value = "255,0,0,255";
			}
			else if (this.isModded)
			{
				_value = this.moddedFillColorString;
			}
			else
			{
				_value = this.standardFillColorString;
			}
			return true;
		}
		if (!(_bindingName == "showempty"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = this.itemStack.IsEmpty().ToString();
		return true;
	}

	// Token: 0x0600630A RID: 25354 RVA: 0x0028386C File Offset: 0x00281A6C
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "standard_fill_color"))
			{
				if (!(name == "modded_fill_color"))
				{
					return false;
				}
				this.moddedFillColorString = value;
				this.moddedFillColor = StringParsers.ParseColor32(value);
			}
			else
			{
				this.standardFillColorString = value;
				this.standardFillColor = StringParsers.ParseColor32(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x0600630B RID: 25355 RVA: 0x002838D1 File Offset: 0x00281AD1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		if (this.itemStack.IsEmpty())
		{
			base.OnHovered(false);
			return;
		}
		base.OnHovered(_isOver);
	}

	// Token: 0x04004A98 RID: 19096
	public bool IsCurrentStack;

	// Token: 0x04004A99 RID: 19097
	public bool IsBlocked;

	// Token: 0x04004A9A RID: 19098
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isModded;

	// Token: 0x04004A9B RID: 19099
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite fillIcon;

	// Token: 0x04004A9C RID: 19100
	[PublicizedFrom(EAccessModifier.Private)]
	public string standardFillColorString = "202,190,33,255";

	// Token: 0x04004A9D RID: 19101
	[PublicizedFrom(EAccessModifier.Private)]
	public Color standardFillColor = new Color32(202, 190, 33, byte.MaxValue);

	// Token: 0x04004A9E RID: 19102
	[PublicizedFrom(EAccessModifier.Private)]
	public string moddedFillColorString = "0,173,216,255";

	// Token: 0x04004A9F RID: 19103
	[PublicizedFrom(EAccessModifier.Private)]
	public Color moddedFillColor = new Color32(0, 173, 216, byte.MaxValue);

	// Token: 0x04004AA0 RID: 19104
	[PublicizedFrom(EAccessModifier.Private)]
	public Color disabledColor = new Color32(64, 64, 64, byte.MaxValue);

	// Token: 0x04004AA1 RID: 19105
	[PublicizedFrom(EAccessModifier.Private)]
	public Color blockedColor = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

	// Token: 0x04004AA2 RID: 19106
	[PublicizedFrom(EAccessModifier.Private)]
	public Color currentFillColor;

	// Token: 0x04004AA3 RID: 19107
	[PublicizedFrom(EAccessModifier.Private)]
	public float fillAmount = -1f;

	// Token: 0x04004AA4 RID: 19108
	public float MaxFill = 15f;
}

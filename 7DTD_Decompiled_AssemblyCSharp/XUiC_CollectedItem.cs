using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C3F RID: 3135
[Preserve]
public class XUiC_CollectedItem : XUiController
{
	// Token: 0x170009E9 RID: 2537
	// (get) Token: 0x06006056 RID: 24662 RVA: 0x002720E5 File Offset: 0x002702E5
	// (set) Token: 0x06006057 RID: 24663 RVA: 0x002720ED File Offset: 0x002702ED
	public ItemStack ItemStack
	{
		get
		{
			return this.itemStack;
		}
		set
		{
			this.itemStack = value;
			this.itemClass = this.itemStack.itemValue.ItemClass;
			base.RefreshBindings(true);
		}
	}

	// Token: 0x06006058 RID: 24664 RVA: 0x00272114 File Offset: 0x00270314
	public override void Init()
	{
		base.Init();
		TweenColor tweenColor = base.ViewComponent.UiTransform.gameObject.AddComponent<TweenColor>();
		tweenColor.enabled = false;
		tweenColor.from = Color.white;
		tweenColor.to = new Color(1f, 1f, 1f, 0f);
		tweenColor.duration = 0.8f;
		base.ViewComponent.IsVisible = false;
	}

	// Token: 0x06006059 RID: 24665 RVA: 0x00272183 File Offset: 0x00270383
	public override void Update(float _dt)
	{
		base.Update(_dt);
	}

	// Token: 0x0600605A RID: 24666 RVA: 0x0027218C File Offset: 0x0027038C
	public void ShowItem()
	{
		TweenColor component = base.ViewComponent.UiTransform.gameObject.GetComponent<TweenColor>();
		component.from = Color.white;
		component.to = Color.white;
		component.duration = 0.1f;
		component.enabled = true;
	}

	// Token: 0x0600605B RID: 24667 RVA: 0x002721CC File Offset: 0x002703CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "itemicon")
		{
			value = ((this.itemClass != null) ? this.itemClass.GetIconName() : "");
			return true;
		}
		if (bindingName == "itemiconcolor")
		{
			Color32 v = Color.white;
			if (this.itemStack != null && this.itemStack.itemValue.type != 0)
			{
				v = this.itemClass.GetIconTint(this.itemStack.itemValue);
			}
			value = this.itemiconcolorFormatter.Format(v);
			return true;
		}
		if (bindingName == "itemcount")
		{
			value = "";
			if (this.ItemStack != null && this.itemStack.itemValue.type != 0)
			{
				value = ((this.ItemStack.count > 0) ? this.itemcountFormatter.Format(this.ItemStack.count) : "0");
			}
			return true;
		}
		if (bindingName == "itembackground")
		{
			value = "menu_empty";
			if (this.itemClass != null && this.itemStack.itemValue.type != 0)
			{
				value = "ui_game_popup";
			}
			return true;
		}
		if (!(bindingName == "itembackgroundcolor"))
		{
			return false;
		}
		value = "255, 255, 255, 0";
		if (this.itemClass != null && this.itemStack.itemValue.type != 0)
		{
			value = "255, 255, 255, 255";
		}
		return true;
	}

	// Token: 0x040048BF RID: 18623
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack itemStack = ItemStack.Empty.Clone();

	// Token: 0x040048C0 RID: 18624
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemClass itemClass;

	// Token: 0x040048C1 RID: 18625
	public GameObject Item;

	// Token: 0x040048C2 RID: 18626
	public float TimeAdded;

	// Token: 0x040048C3 RID: 18627
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor itemiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x040048C4 RID: 18628
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> itemcountFormatter = new CachedStringFormatter<int>((int _i) => "+" + _i.ToString());
}

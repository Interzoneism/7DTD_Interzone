using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D1F RID: 3359
[Preserve]
public class XUiC_MapSubPopupEntry : XUiController
{
	// Token: 0x06006894 RID: 26772 RVA: 0x002A7D74 File Offset: 0x002A5F74
	public override void Init()
	{
		base.Init();
		base.OnPress += this.onPressed;
		base.OnVisiblity += this.XUiC_MapSubPopupEntry_OnVisiblity;
	}

	// Token: 0x06006895 RID: 26773 RVA: 0x002A7DA0 File Offset: 0x002A5FA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiC_MapSubPopupEntry_OnVisiblity(XUiController _sender, bool _visible)
	{
		this.select(false);
	}

	// Token: 0x06006896 RID: 26774 RVA: 0x002A7DA9 File Offset: 0x002A5FA9
	public void SetIndex(int _idx)
	{
		this.index = _idx;
	}

	// Token: 0x06006897 RID: 26775 RVA: 0x002A7DB4 File Offset: 0x002A5FB4
	public void SetSpriteName(string _s)
	{
		this.spriteName = _s;
		for (int i = 0; i < base.Parent.Children.Count; i++)
		{
			XUiView viewComponent = base.Parent.Children[i].ViewComponent;
			if (viewComponent.ID.EqualsCaseInsensitive("icon"))
			{
				((XUiV_Sprite)viewComponent).SpriteName = _s;
			}
		}
	}

	// Token: 0x06006898 RID: 26776 RVA: 0x002A7E18 File Offset: 0x002A6018
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.select(_isOver);
	}

	// Token: 0x06006899 RID: 26777 RVA: 0x002A7E24 File Offset: 0x002A6024
	[PublicizedFrom(EAccessModifier.Private)]
	public void onPressed(XUiController _sender, int _mouseButton)
	{
		this.select(true);
		((XUiC_MapArea)base.xui.GetWindow("mapArea").Controller).OnWaypointEntryChosen(this.spriteName);
		XUiC_MapEnterWaypoint childByType = base.xui.GetWindow("mapAreaEnterWaypointName").Controller.GetChildByType<XUiC_MapEnterWaypoint>();
		int num = this.index / 10;
		int num2 = this.index % 10;
		Vector2i position = base.xui.GetWindow("mapAreaChooseWaypoint").Position + new Vector2i(52 * (num + 1), num2 * -43);
		childByType.Show(position);
	}

	// Token: 0x0600689A RID: 26778 RVA: 0x002A7EBC File Offset: 0x002A60BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void select(bool _bSelect)
	{
		XUiV_Sprite xuiV_Sprite = (XUiV_Sprite)this.viewComponent;
		if (xuiV_Sprite != null)
		{
			xuiV_Sprite.Color = (_bSelect ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : new Color32(64, 64, 64, byte.MaxValue));
			xuiV_Sprite.SpriteName = (_bSelect ? "ui_game_select_row" : "menu_empty");
		}
	}

	// Token: 0x0600689B RID: 26779 RVA: 0x002A7DA0 File Offset: 0x002A5FA0
	public void Reset()
	{
		this.select(false);
	}

	// Token: 0x04004EE9 RID: 20201
	[PublicizedFrom(EAccessModifier.Private)]
	public int index;

	// Token: 0x04004EEA RID: 20202
	[PublicizedFrom(EAccessModifier.Private)]
	public string spriteName;
}

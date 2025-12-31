using System;
using UnityEngine.Scripting;

// Token: 0x02000D1C RID: 3356
[Preserve]
public class XUiC_MapPopupList : XUiController
{
	// Token: 0x06006888 RID: 26760 RVA: 0x002A77EC File Offset: 0x002A59EC
	public override void Init()
	{
		base.Init();
		this.children[0].OnPress += this.onPressEntry1;
		this.children[1].OnPress += this.onPressEntry2;
	}

	// Token: 0x06006889 RID: 26761 RVA: 0x002A7839 File Offset: 0x002A5A39
	[PublicizedFrom(EAccessModifier.Private)]
	public void onPressEntry1(XUiController _sender, int _mouseButton)
	{
		XUiC_MapArea xuiC_MapArea = base.xui.GetWindow("mapArea").Controller as XUiC_MapArea;
		xuiC_MapArea.OnSetWaypoint();
		xuiC_MapArea.GetChildById("mapView").SelectCursorElement(true, false);
	}

	// Token: 0x0600688A RID: 26762 RVA: 0x002A7870 File Offset: 0x002A5A70
	[PublicizedFrom(EAccessModifier.Private)]
	public void onPressEntry2(XUiController _sender, int _mouseButton)
	{
		if (_sender is XUiC_MapPopupEntry)
		{
			XUiV_Window window = base.xui.GetWindow("mapAreaSetWaypoint");
			XUiV_Window window2 = base.xui.GetWindow("mapAreaChooseWaypoint");
			int num = 46;
			window2.Position = window.Position + new Vector2i(199, -num);
			if (window2.Position.y < 0)
			{
				window2.Position = new Vector2i(window2.Position.x, window2.Position.y + window2.Size.y);
			}
			window2.IsVisible = true;
			window2.Controller.GetChildByType<XUiC_MapSubPopupList>().ResetList();
		}
	}
}

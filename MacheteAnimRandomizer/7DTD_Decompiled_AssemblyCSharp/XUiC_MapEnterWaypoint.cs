using System;
using UnityEngine.Scripting;

// Token: 0x02000D15 RID: 3349
[Preserve]
public class XUiC_MapEnterWaypoint : XUiController
{
	// Token: 0x06006866 RID: 26726 RVA: 0x002A6CA4 File Offset: 0x002A4EA4
	public override void Init()
	{
		base.Init();
		this.txtInput = (XUiC_TextInput)this.windowGroup.Controller.GetChildById("waypointInput");
		if (this.txtInput != null)
		{
			this.txtInput.Text = string.Empty;
		}
		if (this.txtInput != null)
		{
			this.txtInput.OnSubmitHandler += this.waypointOnSubmitHandler;
		}
	}

	// Token: 0x06006867 RID: 26727 RVA: 0x002A6D0E File Offset: 0x002A4F0E
	[PublicizedFrom(EAccessModifier.Private)]
	public void waypointOnInputAbortedHandler(XUiController _sender)
	{
		((XUiC_MapArea)base.xui.GetWindow("mapArea").Controller).closeAllPopups();
	}

	// Token: 0x06006868 RID: 26728 RVA: 0x002A6D2F File Offset: 0x002A4F2F
	[PublicizedFrom(EAccessModifier.Private)]
	public void waypointOnSubmitHandler(XUiController _sender, string _text)
	{
		((XUiC_MapArea)base.xui.GetWindow("mapArea").Controller).OnWaypointCreated(_text);
	}

	// Token: 0x06006869 RID: 26729 RVA: 0x002A6D51 File Offset: 0x002A4F51
	public void Show(Vector2i _position)
	{
		XUiV_Window window = base.xui.GetWindow("mapAreaEnterWaypointName");
		window.Position = _position;
		window.IsVisible = true;
		this.txtInput.Text = string.Empty;
		this.txtInput.SelectOrVirtualKeyboard(true);
	}

	// Token: 0x04004ECC RID: 20172
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;
}

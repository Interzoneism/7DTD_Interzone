using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000CC4 RID: 3268
[Preserve]
public class XUiC_InfoWindow : XUiController
{
	// Token: 0x0600650D RID: 25869 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Deselect()
	{
	}

	// Token: 0x0600650E RID: 25870 RVA: 0x0028FAFA File Offset: 0x0028DCFA
	public override void Init()
	{
		base.Init();
	}

	// Token: 0x0600650F RID: 25871 RVA: 0x00272183 File Offset: 0x00270383
	public override void Update(float _dt)
	{
		base.Update(_dt);
	}

	// Token: 0x06006510 RID: 25872 RVA: 0x0028FB04 File Offset: 0x0028DD04
	public override void OnVisibilityChanged(bool _isVisible)
	{
		bool flag = this.windowGroup == null || this.windowGroup.isShowing;
		if (_isVisible)
		{
			List<XUiC_InfoWindow> childrenByType = base.xui.GetChildrenByType<XUiC_InfoWindow>();
			for (int i = 0; i < childrenByType.Count; i++)
			{
				if (childrenByType[i] != this && (flag || !childrenByType[i].windowGroup.isShowing))
				{
					childrenByType[i].ViewComponent.IsVisible = false;
				}
			}
		}
		else
		{
			this.Deselect();
		}
		base.OnVisibilityChanged(_isVisible);
	}
}

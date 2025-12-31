using System;

// Token: 0x02000ED7 RID: 3799
public class ParentControllerState
{
	// Token: 0x060077F6 RID: 30710 RVA: 0x0030D92C File Offset: 0x0030BB2C
	public ParentControllerState(XUiController parentController)
	{
		this.m_parentController = parentController;
		if (this.m_parentController == null)
		{
			return;
		}
		this.m_isVisible = this.m_parentController.ViewComponent.IsVisible;
		this.m_isEscClosable = this.m_parentController.WindowGroup.isEscClosable;
	}

	// Token: 0x060077F7 RID: 30711 RVA: 0x0030D97B File Offset: 0x0030BB7B
	public void Hide()
	{
		if (this.m_parentController == null)
		{
			return;
		}
		this.m_parentController.ViewComponent.IsVisible = false;
		this.m_parentController.WindowGroup.isEscClosable = false;
	}

	// Token: 0x060077F8 RID: 30712 RVA: 0x0030D9A8 File Offset: 0x0030BBA8
	public void Restore()
	{
		if (this.m_parentController == null)
		{
			return;
		}
		this.m_parentController.ViewComponent.IsVisible = this.m_isVisible;
		this.m_parentController.WindowGroup.isEscClosable = this.m_isEscClosable;
	}

	// Token: 0x04005B7C RID: 23420
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly XUiController m_parentController;

	// Token: 0x04005B7D RID: 23421
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool m_isVisible;

	// Token: 0x04005B7E RID: 23422
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool m_isEscClosable;
}

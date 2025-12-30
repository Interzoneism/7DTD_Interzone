using System;

// Token: 0x02000F1D RID: 3869
public class XUiEventManager
{
	// Token: 0x06007B9F RID: 31647 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiEventManager()
	{
	}

	// Token: 0x17000CFA RID: 3322
	// (get) Token: 0x06007BA0 RID: 31648 RVA: 0x0031F941 File Offset: 0x0031DB41
	public static XUiEventManager Instance
	{
		get
		{
			if (XUiEventManager.instance == null)
			{
				XUiEventManager.instance = new XUiEventManager();
			}
			return XUiEventManager.instance;
		}
	}

	// Token: 0x140000E3 RID: 227
	// (add) Token: 0x06007BA1 RID: 31649 RVA: 0x0031F95C File Offset: 0x0031DB5C
	// (remove) Token: 0x06007BA2 RID: 31650 RVA: 0x0031F994 File Offset: 0x0031DB94
	public event XUiEventManager.XUiEvent_SkillExperienceAdded OnSkillExperienceAdded;

	// Token: 0x06007BA3 RID: 31651 RVA: 0x0031F9C9 File Offset: 0x0031DBC9
	public void SkillExperienceAdded(ProgressionValue skill, int newXP)
	{
		if (this.OnSkillExperienceAdded != null)
		{
			this.OnSkillExperienceAdded(skill, newXP);
		}
	}

	// Token: 0x04005D8D RID: 23949
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiEventManager instance;

	// Token: 0x02000F1E RID: 3870
	// (Invoke) Token: 0x06007BA5 RID: 31653
	public delegate void XUiEvent_SkillExperienceAdded(ProgressionValue skill, int newXP);
}

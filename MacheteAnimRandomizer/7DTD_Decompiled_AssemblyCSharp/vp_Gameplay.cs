using System;

// Token: 0x0200136F RID: 4975
public class vp_Gameplay
{
	// Token: 0x17001044 RID: 4164
	// (get) Token: 0x06009BEA RID: 39914 RVA: 0x003DF723 File Offset: 0x003DD923
	// (set) Token: 0x06009BEB RID: 39915 RVA: 0x003DF733 File Offset: 0x003DD933
	public static bool isMaster
	{
		get
		{
			return !vp_Gameplay.isMultiplayer || vp_Gameplay.m_IsMaster;
		}
		set
		{
			if (!vp_Gameplay.isMultiplayer)
			{
				return;
			}
			vp_Gameplay.m_IsMaster = value;
		}
	}

	// Token: 0x04007886 RID: 30854
	public static bool isMultiplayer = false;

	// Token: 0x04007887 RID: 30855
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool m_IsMaster = true;
}

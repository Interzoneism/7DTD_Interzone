using System;

// Token: 0x02001357 RID: 4951
public class vp_FPWeaponHandler : vp_WeaponHandler
{
	// Token: 0x06009AAB RID: 39595 RVA: 0x003D89DB File Offset: 0x003D6BDB
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_AutoReload()
	{
		return this.ReloadAutomatically && this.m_Player.Reload.TryStart(true);
	}
}

using System;

// Token: 0x0200133C RID: 4924
public class vp_AmmoPickup : vp_Pickup
{
	// Token: 0x06009903 RID: 39171 RVA: 0x003CDC98 File Offset: 0x003CBE98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Dead.Active)
		{
			return false;
		}
		int i = 0;
		while (i < this.GiveAmount)
		{
			if (!base.TryGive(player))
			{
				if (this.TryReloadIfEmpty(player))
				{
					base.TryGive(player);
					return true;
				}
				return false;
			}
			else
			{
				i++;
			}
		}
		this.TryReloadIfEmpty(player);
		return true;
	}

	// Token: 0x06009904 RID: 39172 RVA: 0x003CDCEC File Offset: 0x003CBEEC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool TryReloadIfEmpty(vp_FPPlayerEventHandler player)
	{
		return player.CurrentWeaponAmmoCount.Get() <= 0 && !(player.CurrentWeaponClipType.Get() != this.InventoryName) && player.Reload.TryStart(true);
	}

	// Token: 0x040075A3 RID: 30115
	public int GiveAmount = 1;
}

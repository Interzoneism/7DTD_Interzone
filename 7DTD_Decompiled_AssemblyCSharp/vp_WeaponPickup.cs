using System;

// Token: 0x02001342 RID: 4930
public class vp_WeaponPickup : vp_Pickup
{
	// Token: 0x06009919 RID: 39193 RVA: 0x003CE690 File Offset: 0x003CC890
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool TryGive(vp_FPPlayerEventHandler player)
	{
		if (player.Dead.Active)
		{
			return false;
		}
		if (!base.TryGive(player))
		{
			return false;
		}
		player.SetWeaponByName.Try(this.InventoryName);
		if (this.AmmoIncluded > 0)
		{
			player.AddAmmo.Try(new object[]
			{
				this.InventoryName,
				this.AmmoIncluded
			});
		}
		return true;
	}

	// Token: 0x040075C7 RID: 30151
	public int AmmoIncluded;
}

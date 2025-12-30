using System;
using UnityEngine;

// Token: 0x0200136E RID: 4974
public class vp_WeaponThrower : MonoBehaviour
{
	// Token: 0x1700103A RID: 4154
	// (get) Token: 0x06009BD2 RID: 39890 RVA: 0x003DF16C File Offset: 0x003DD36C
	public Transform Transform
	{
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}

	// Token: 0x1700103B RID: 4155
	// (get) Token: 0x06009BD3 RID: 39891 RVA: 0x003DF18E File Offset: 0x003DD38E
	public Transform Root
	{
		get
		{
			if (this.m_Root == null)
			{
				this.m_Root = this.Transform.root;
			}
			return this.m_Root;
		}
	}

	// Token: 0x1700103C RID: 4156
	// (get) Token: 0x06009BD4 RID: 39892 RVA: 0x003DF1B5 File Offset: 0x003DD3B5
	public vp_Weapon Weapon
	{
		get
		{
			if (this.m_Weapon == null)
			{
				this.m_Weapon = (vp_Weapon)this.Transform.GetComponent(typeof(vp_Weapon));
			}
			return this.m_Weapon;
		}
	}

	// Token: 0x1700103D RID: 4157
	// (get) Token: 0x06009BD5 RID: 39893 RVA: 0x003DF1EB File Offset: 0x003DD3EB
	public vp_WeaponShooter Shooter
	{
		get
		{
			if (this.m_Shooter == null)
			{
				this.m_Shooter = (vp_WeaponShooter)this.Transform.GetComponent(typeof(vp_WeaponShooter));
			}
			return this.m_Shooter;
		}
	}

	// Token: 0x1700103E RID: 4158
	// (get) Token: 0x06009BD6 RID: 39894 RVA: 0x003DF224 File Offset: 0x003DD424
	public vp_UnitBankType UnitBankType
	{
		get
		{
			if (this.ItemIdentifier == null)
			{
				return null;
			}
			vp_ItemType itemType = this.m_ItemIdentifier.GetItemType();
			if (itemType == null)
			{
				return null;
			}
			vp_UnitBankType vp_UnitBankType = itemType as vp_UnitBankType;
			if (vp_UnitBankType == null)
			{
				return null;
			}
			return vp_UnitBankType;
		}
	}

	// Token: 0x1700103F RID: 4159
	// (get) Token: 0x06009BD7 RID: 39895 RVA: 0x003DF26C File Offset: 0x003DD46C
	public vp_UnitBankInstance UnitBank
	{
		get
		{
			if (this.m_UnitBank == null && this.UnitBankType != null && this.Inventory != null)
			{
				foreach (vp_UnitBankInstance vp_UnitBankInstance in this.Inventory.UnitBankInstances)
				{
					if (vp_UnitBankInstance.UnitType == this.UnitBankType.Unit)
					{
						this.m_UnitBank = vp_UnitBankInstance;
					}
				}
			}
			return this.m_UnitBank;
		}
	}

	// Token: 0x17001040 RID: 4160
	// (get) Token: 0x06009BD8 RID: 39896 RVA: 0x003DF308 File Offset: 0x003DD508
	public vp_ItemIdentifier ItemIdentifier
	{
		get
		{
			if (this.m_ItemIdentifier == null)
			{
				this.m_ItemIdentifier = (vp_ItemIdentifier)this.Transform.GetComponent(typeof(vp_ItemIdentifier));
			}
			return this.m_ItemIdentifier;
		}
	}

	// Token: 0x17001041 RID: 4161
	// (get) Token: 0x06009BD9 RID: 39897 RVA: 0x003DF33E File Offset: 0x003DD53E
	public vp_PlayerEventHandler Player
	{
		get
		{
			if (this.m_Player == null)
			{
				this.m_Player = (vp_PlayerEventHandler)this.Root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
			}
			return this.m_Player;
		}
	}

	// Token: 0x17001042 RID: 4162
	// (get) Token: 0x06009BDA RID: 39898 RVA: 0x003DF374 File Offset: 0x003DD574
	public vp_PlayerInventory Inventory
	{
		get
		{
			if (this.m_Inventory == null)
			{
				this.m_Inventory = (vp_PlayerInventory)this.Root.GetComponentInChildren(typeof(vp_PlayerInventory));
			}
			return this.m_Inventory;
		}
	}

	// Token: 0x06009BDB RID: 39899 RVA: 0x003DF3AC File Offset: 0x003DD5AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.Player == null)
		{
			return;
		}
		this.Player.Register(this);
		this.TryStoreAttackMinDuration();
		this.Inventory.SetItemCap(this.ItemIdentifier.Type, 1, true);
		this.Inventory.CapsEnabled = true;
	}

	// Token: 0x06009BDC RID: 39900 RVA: 0x003DF3FE File Offset: 0x003DD5FE
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		this.TryRestoreAttackMinDuration();
		if (this.Player != null)
		{
			this.Player.Unregister(this);
		}
	}

	// Token: 0x06009BDD RID: 39901 RVA: 0x003DF420 File Offset: 0x003DD620
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.TryStoreAttackMinDuration();
		if (this.Weapon == null)
		{
			Debug.LogError("Throwing weapon setup error (" + ((this != null) ? this.ToString() : null) + ") requires a vp_Weapon or vp_FPWeapon component.");
			return;
		}
		if (this.UnitBankType == null)
		{
			Debug.LogError("Throwing weapon setup error (" + ((this != null) ? this.ToString() : null) + ") requires a vp_ItemIdentifier component with a valid UnitBank.");
			return;
		}
		if (this.Weapon.AnimationType != 3)
		{
			string[] array = new string[5];
			array[0] = "Throwing weapon setup error (";
			array[1] = ((this != null) ? this.ToString() : null);
			array[2] = ") Please set 'Animation -> Type' of '";
			int num = 3;
			vp_Weapon weapon = this.Weapon;
			array[num] = ((weapon != null) ? weapon.ToString() : null);
			array[4] = "' item type to 'Thrown'.";
			Debug.LogError(string.Concat(array));
		}
		if (this.UnitBankType.Capacity != 1)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Throwing weapon setup error (",
				(this != null) ? this.ToString() : null,
				") Please set 'Capacity' for the '",
				this.UnitBankType.name,
				"' item type to '1'."
			}));
		}
	}

	// Token: 0x06009BDE RID: 39902 RVA: 0x003DF544 File Offset: 0x003DD744
	[PublicizedFrom(EAccessModifier.Private)]
	public void TryStoreAttackMinDuration()
	{
		if (this.Player.Attack == null)
		{
			return;
		}
		if (this.m_OriginalAttackMinDuration == 0f)
		{
			return;
		}
		this.m_OriginalAttackMinDuration = this.Player.Attack.MinDuration;
		this.Player.Attack.MinDuration = this.AttackMinDuration;
	}

	// Token: 0x06009BDF RID: 39903 RVA: 0x003DF599 File Offset: 0x003DD799
	[PublicizedFrom(EAccessModifier.Private)]
	public void TryRestoreAttackMinDuration()
	{
		if (this.Player.Attack == null)
		{
			return;
		}
		if (this.m_OriginalAttackMinDuration != 0f)
		{
			return;
		}
		this.Player.Attack.MinDuration = this.m_OriginalAttackMinDuration;
	}

	// Token: 0x17001043 RID: 4163
	// (get) Token: 0x06009BE0 RID: 39904 RVA: 0x003DF5CD File Offset: 0x003DD7CD
	public bool HaveAmmoForCurrentWeapon
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.Player.CurrentWeaponAmmoCount.Get() > 0 || this.Player.CurrentWeaponClipCount.Get() > 0;
		}
	}

	// Token: 0x06009BE1 RID: 39905 RVA: 0x003DF601 File Offset: 0x003DD801
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool TryReload()
	{
		return this.UnitBank != null && this.Inventory.TryReload(this.UnitBank);
	}

	// Token: 0x06009BE2 RID: 39906 RVA: 0x003DF620 File Offset: 0x003DD820
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_Attack()
	{
		if (!this.Player.IsFirstPerson.Get())
		{
			vp_Timer.In(this.Shooter.ProjectileSpawnDelay, delegate()
			{
				this.Weapon.Weapon3rdPersonModel.GetComponent<Renderer>().enabled = false;
			}, null);
			vp_Timer.In(this.Shooter.ProjectileSpawnDelay + 1f, delegate()
			{
				if (this.HaveAmmoForCurrentWeapon)
				{
					this.Weapon.Weapon3rdPersonModel.GetComponent<Renderer>().enabled = true;
				}
			}, null);
		}
		if (this.Player.CurrentWeaponAmmoCount.Get() < 1)
		{
			this.TryReload();
		}
		vp_Timer.In(this.Shooter.ProjectileSpawnDelay + 0.5f, delegate()
		{
			this.Player.Attack.Stop(0f);
		}, null);
	}

	// Token: 0x06009BE3 RID: 39907 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanStart_Reload()
	{
		return false;
	}

	// Token: 0x06009BE4 RID: 39908 RVA: 0x003DF6C6 File Offset: 0x003DD8C6
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_Attack()
	{
		this.TryReload();
	}

	// Token: 0x06009BE5 RID: 39909 RVA: 0x003DF6CF File Offset: 0x003DD8CF
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_SetWeapon()
	{
		this.m_UnitBank = null;
	}

	// Token: 0x0400787B RID: 30843
	public float AttackMinDuration = 1f;

	// Token: 0x0400787C RID: 30844
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_OriginalAttackMinDuration;

	// Token: 0x0400787D RID: 30845
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x0400787E RID: 30846
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Root;

	// Token: 0x0400787F RID: 30847
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Weapon m_Weapon;

	// Token: 0x04007880 RID: 30848
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_WeaponShooter m_Shooter;

	// Token: 0x04007881 RID: 30849
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_UnitBankType m_UnitBankType;

	// Token: 0x04007882 RID: 30850
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_UnitBankInstance m_UnitBank;

	// Token: 0x04007883 RID: 30851
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_ItemIdentifier m_ItemIdentifier;

	// Token: 0x04007884 RID: 30852
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;

	// Token: 0x04007885 RID: 30853
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PlayerInventory m_Inventory;
}

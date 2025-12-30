using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001369 RID: 4969
public class vp_WeaponHandler : MonoBehaviour
{
	// Token: 0x1700102D RID: 4141
	// (get) Token: 0x06009B8D RID: 39821 RVA: 0x003DDDE9 File Offset: 0x003DBFE9
	// (set) Token: 0x06009B8E RID: 39822 RVA: 0x003DDDFF File Offset: 0x003DBFFF
	public List<vp_Weapon> Weapons
	{
		get
		{
			if (this.m_Weapons == null)
			{
				this.InitWeaponLists();
			}
			return this.m_Weapons;
		}
		set
		{
			this.m_Weapons = value;
		}
	}

	// Token: 0x1700102E RID: 4142
	// (get) Token: 0x06009B8F RID: 39823 RVA: 0x003DDE08 File Offset: 0x003DC008
	public vp_Weapon CurrentWeapon
	{
		get
		{
			return this.m_CurrentWeapon;
		}
	}

	// Token: 0x1700102F RID: 4143
	// (get) Token: 0x06009B90 RID: 39824 RVA: 0x003DDE10 File Offset: 0x003DC010
	[Obsolete("Please use the 'CurrentWeaponIndex' parameter instead.")]
	public int CurrentWeaponID
	{
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x17001030 RID: 4144
	// (get) Token: 0x06009B91 RID: 39825 RVA: 0x003DDE10 File Offset: 0x003DC010
	public int CurrentWeaponIndex
	{
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x06009B92 RID: 39826 RVA: 0x003DDE18 File Offset: 0x003DC018
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Player = (vp_PlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_PlayerEventHandler));
		if (this.Weapons != null)
		{
			this.StartWeapon = Mathf.Clamp(this.StartWeapon, 0, this.Weapons.Count);
		}
	}

	// Token: 0x06009B93 RID: 39827 RVA: 0x003DDE70 File Offset: 0x003DC070
	[PublicizedFrom(EAccessModifier.Protected)]
	public void InitWeaponLists()
	{
		List<vp_Weapon> list = null;
		vp_FPCamera componentInChildren = base.transform.GetComponentInChildren<vp_FPCamera>();
		if (componentInChildren != null)
		{
			list = this.GetWeaponList(Camera.main.transform);
			if (list != null && list.Count > 0)
			{
				this.m_WeaponLists.Add(list);
			}
		}
		List<vp_Weapon> list2 = new List<vp_Weapon>(base.transform.GetComponentsInChildren<vp_Weapon>());
		if (list != null && list.Count == list2.Count)
		{
			this.Weapons = this.m_WeaponLists[0];
			return;
		}
		List<Transform> list3 = new List<Transform>();
		foreach (vp_Weapon vp_Weapon in list2)
		{
			if ((!(componentInChildren != null) || !list.Contains(vp_Weapon)) && !list3.Contains(vp_Weapon.Parent))
			{
				list3.Add(vp_Weapon.Parent);
			}
		}
		foreach (Transform target in list3)
		{
			List<vp_Weapon> weaponList = this.GetWeaponList(target);
			this.DeactivateAll(weaponList);
			this.m_WeaponLists.Add(weaponList);
		}
		if (this.m_WeaponLists.Count < 1)
		{
			Debug.LogError("Error (" + ((this != null) ? this.ToString() : null) + ") WeaponHandler found no weapons in its hierarchy. Disabling self.");
			base.enabled = false;
			return;
		}
		this.Weapons = this.m_WeaponLists[0];
	}

	// Token: 0x06009B94 RID: 39828 RVA: 0x003DE004 File Offset: 0x003DC204
	public void EnableWeaponList(int index)
	{
		if (this.m_WeaponLists == null)
		{
			return;
		}
		if (this.m_WeaponLists.Count < 1)
		{
			return;
		}
		if (index < 0 || index > this.m_WeaponLists.Count - 1)
		{
			return;
		}
		this.Weapons = this.m_WeaponLists[index];
	}

	// Token: 0x06009B95 RID: 39829 RVA: 0x003DE050 File Offset: 0x003DC250
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<vp_Weapon> GetWeaponList(Transform target)
	{
		List<vp_Weapon> list = new List<vp_Weapon>();
		if (target.GetComponent<vp_Weapon>())
		{
			Debug.LogError("Error: (" + ((this != null) ? this.ToString() : null) + ") Hierarchy error. This component should sit above any vp_Weapons in the gameobject hierarchy.");
			return list;
		}
		foreach (vp_Weapon item in target.GetComponentsInChildren<vp_Weapon>(true))
		{
			list.Insert(list.Count, item);
		}
		if (list.Count == 0)
		{
			Debug.LogError("Error: (" + ((this != null) ? this.ToString() : null) + ") Hierarchy error. This component must be added to a gameobject with vp_Weapon components in child gameobjects.");
			return list;
		}
		IComparer @object = new vp_WeaponHandler.WeaponComparer();
		list.Sort(new Comparison<vp_Weapon>(@object.Compare));
		return list;
	}

	// Token: 0x06009B96 RID: 39830 RVA: 0x003DE101 File Offset: 0x003DC301
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	// Token: 0x06009B97 RID: 39831 RVA: 0x003DE11D File Offset: 0x003DC31D
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x06009B98 RID: 39832 RVA: 0x003DE139 File Offset: 0x003DC339
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		this.InitWeapon();
		this.UpdateFiring();
	}

	// Token: 0x06009B99 RID: 39833 RVA: 0x003DE148 File Offset: 0x003DC348
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateFiring()
	{
		if (!this.m_Player.IsLocal.Get() && !this.m_Player.IsAI.Get())
		{
			return;
		}
		if (!this.m_Player.Attack.Active)
		{
			return;
		}
		if (this.m_Player.SetWeapon.Active || !this.m_CurrentWeapon.Wielded)
		{
			return;
		}
		this.m_Player.Fire.Try();
	}

	// Token: 0x06009B9A RID: 39834 RVA: 0x003DE1D0 File Offset: 0x003DC3D0
	public virtual void SetWeapon(int weaponIndex)
	{
		if (this.Weapons == null || this.Weapons.Count < 1)
		{
			Debug.LogError("Error: (" + ((this != null) ? this.ToString() : null) + ") Tried to set weapon with an empty weapon list.");
			return;
		}
		if (weaponIndex < 0 || weaponIndex > this.Weapons.Count)
		{
			Debug.LogError("Error: (" + ((this != null) ? this.ToString() : null) + ") Weapon list does not have a weapon with index: " + weaponIndex.ToString());
			return;
		}
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.ResetState();
		}
		this.DeactivateAll(this.Weapons);
		this.ActivateWeapon(weaponIndex);
	}

	// Token: 0x06009B9B RID: 39835 RVA: 0x003DE27C File Offset: 0x003DC47C
	public void DeactivateAll(List<vp_Weapon> weaponList)
	{
		foreach (vp_Weapon vp_Weapon in weaponList)
		{
			vp_Weapon.ActivateGameObject(false);
			vp_FPWeapon vp_FPWeapon = vp_Weapon as vp_FPWeapon;
			if (vp_FPWeapon != null && vp_FPWeapon.Weapon3rdPersonModel != null)
			{
				vp_Utility.Activate(vp_FPWeapon.Weapon3rdPersonModel, false);
			}
		}
	}

	// Token: 0x06009B9C RID: 39836 RVA: 0x003DE2F4 File Offset: 0x003DC4F4
	public void ActivateWeapon(int index)
	{
		this.m_CurrentWeaponIndex = index;
		this.m_CurrentWeapon = null;
		if (this.m_CurrentWeaponIndex > 0)
		{
			this.m_CurrentWeapon = this.Weapons[this.m_CurrentWeaponIndex - 1];
			if (this.m_CurrentWeapon != null)
			{
				this.m_CurrentWeapon.ActivateGameObject(true);
			}
		}
	}

	// Token: 0x06009B9D RID: 39837 RVA: 0x003DE34B File Offset: 0x003DC54B
	public virtual void CancelTimers()
	{
		vp_Timer.CancelAll("EjectShell");
		this.m_DisableAttackStateTimer.Cancel();
		this.m_SetWeaponTimer.Cancel();
		this.m_SetWeaponRefreshTimer.Cancel();
	}

	// Token: 0x06009B9E RID: 39838 RVA: 0x003DE378 File Offset: 0x003DC578
	public virtual void SetWeaponLayer(int layer)
	{
		if (this.m_CurrentWeaponIndex < 1 || this.m_CurrentWeaponIndex > this.Weapons.Count)
		{
			return;
		}
		vp_Layer.Set(this.Weapons[this.m_CurrentWeaponIndex - 1].gameObject, layer, true);
	}

	// Token: 0x06009B9F RID: 39839 RVA: 0x003DE3B6 File Offset: 0x003DC5B6
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitWeapon()
	{
		if (this.m_CurrentWeaponIndex == -1)
		{
			this.SetWeapon(0);
			vp_Timer.In(this.SetWeaponDuration + 0.1f, delegate()
			{
				if (this.StartWeapon > 0 && this.StartWeapon < this.Weapons.Count + 1 && !this.m_Player.SetWeapon.TryStart<int>(this.StartWeapon))
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"Warning (",
						(this != null) ? this.ToString() : null,
						") Requested 'StartWeapon' (",
						this.Weapons[this.StartWeapon - 1].name,
						") was denied, likely by the inventory. Make sure it's present in the inventory from the beginning."
					}));
				}
			}, null);
		}
	}

	// Token: 0x06009BA0 RID: 39840 RVA: 0x003DE3E8 File Offset: 0x003DC5E8
	public void RefreshAllWeapons()
	{
		foreach (vp_Weapon vp_Weapon in this.Weapons)
		{
			vp_Weapon.Refresh();
			vp_Weapon.RefreshWeaponModel();
		}
	}

	// Token: 0x06009BA1 RID: 39841 RVA: 0x003DE440 File Offset: 0x003DC640
	public int GetWeaponIndex(vp_Weapon weapon)
	{
		return this.Weapons.IndexOf(weapon) + 1;
	}

	// Token: 0x06009BA2 RID: 39842 RVA: 0x003DE450 File Offset: 0x003DC650
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_Reload()
	{
		this.m_Player.Attack.Stop(this.m_Player.CurrentWeaponReloadDuration.Get() + this.ReloadAttackSleepDuration);
	}

	// Token: 0x06009BA3 RID: 39843 RVA: 0x003DE480 File Offset: 0x003DC680
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_SetWeapon()
	{
		this.CancelTimers();
		this.m_Player.Reload.Stop(this.SetWeaponDuration + this.SetWeaponReloadSleepDuration);
		this.m_Player.Zoom.Stop(this.SetWeaponDuration + this.SetWeaponZoomSleepDuration);
		this.m_Player.Attack.Stop(this.SetWeaponDuration + this.SetWeaponAttackSleepDuration);
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.Wield(false);
		}
		this.m_Player.SetWeapon.AutoDuration = this.SetWeaponDuration;
	}

	// Token: 0x06009BA4 RID: 39844 RVA: 0x003DE51C File Offset: 0x003DC71C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_SetWeapon()
	{
		int weapon = 0;
		if (this.m_Player.SetWeapon.Argument != null)
		{
			weapon = (int)this.m_Player.SetWeapon.Argument;
		}
		this.SetWeapon(weapon);
		if (this.m_CurrentWeapon != null)
		{
			this.m_CurrentWeapon.Wield(true);
		}
		vp_Timer.In(this.SetWeaponRefreshStatesDelay, delegate()
		{
			this.m_Player.RefreshActivityStates();
			if (this.m_CurrentWeapon != null && this.m_Player.CurrentWeaponAmmoCount.Get() == 0)
			{
				this.m_Player.AutoReload.Try();
			}
		}, this.m_SetWeaponRefreshTimer);
	}

	// Token: 0x06009BA5 RID: 39845 RVA: 0x003DE594 File Offset: 0x003DC794
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return num != this.m_CurrentWeaponIndex && num >= 0 && num <= this.Weapons.Count && !this.m_Player.Reload.Active;
	}

	// Token: 0x06009BA6 RID: 39846 RVA: 0x003DE5EC File Offset: 0x003DC7EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanStart_Attack()
	{
		return !(this.m_CurrentWeapon == null) && !this.m_Player.Attack.Active && !this.m_Player.SetWeapon.Active && !this.m_Player.Reload.Active;
	}

	// Token: 0x06009BA7 RID: 39847 RVA: 0x003DE646 File Offset: 0x003DC846
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_Attack()
	{
		vp_Timer.In(this.AttackStateDisableDelay, delegate()
		{
			if (!this.m_Player.Attack.Active && this.m_CurrentWeapon != null)
			{
				this.m_CurrentWeapon.SetState("Attack", false, false, false);
			}
		}, this.m_DisableAttackStateTimer);
	}

	// Token: 0x06009BA8 RID: 39848 RVA: 0x003DE668 File Offset: 0x003DC868
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_SetPrevWeapon()
	{
		int num = this.m_CurrentWeaponIndex - 1;
		if (num < 1)
		{
			num = this.Weapons.Count;
		}
		int num2 = 0;
		while (!this.m_Player.SetWeapon.TryStart<int>(num))
		{
			num--;
			if (num < 1)
			{
				num = this.Weapons.Count;
			}
			num2++;
			if (num2 > this.Weapons.Count)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06009BA9 RID: 39849 RVA: 0x003DE6D0 File Offset: 0x003DC8D0
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_SetNextWeapon()
	{
		int num = this.m_CurrentWeaponIndex + 1;
		int num2 = 0;
		while (!this.m_Player.SetWeapon.TryStart<int>(num))
		{
			if (num > this.Weapons.Count + 1)
			{
				num = 0;
			}
			num++;
			num2++;
			if (num2 > this.Weapons.Count)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06009BAA RID: 39850 RVA: 0x003DE728 File Offset: 0x003DC928
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_SetWeaponByName(string name)
	{
		for (int i = 0; i < this.Weapons.Count; i++)
		{
			if (this.Weapons[i].name == name)
			{
				return this.m_Player.SetWeapon.TryStart<int>(i + 1);
			}
		}
		return false;
	}

	// Token: 0x17001031 RID: 4145
	// (get) Token: 0x06009BAB RID: 39851 RVA: 0x003DE779 File Offset: 0x003DC979
	public virtual bool OnValue_CurrentWeaponWielded
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return !(this.m_CurrentWeapon == null) && this.m_CurrentWeapon.Wielded;
		}
	}

	// Token: 0x17001032 RID: 4146
	// (get) Token: 0x06009BAC RID: 39852 RVA: 0x003DE796 File Offset: 0x003DC996
	public virtual string OnValue_CurrentWeaponName
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_CurrentWeapon == null || this.Weapons == null)
			{
				return "";
			}
			return this.m_CurrentWeapon.name;
		}
	}

	// Token: 0x17001033 RID: 4147
	// (get) Token: 0x06009BAD RID: 39853 RVA: 0x003DDE10 File Offset: 0x003DC010
	public virtual int OnValue_CurrentWeaponID
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x17001034 RID: 4148
	// (get) Token: 0x06009BAE RID: 39854 RVA: 0x003DDE10 File Offset: 0x003DC010
	public virtual int OnValue_CurrentWeaponIndex
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.m_CurrentWeaponIndex;
		}
	}

	// Token: 0x17001035 RID: 4149
	// (get) Token: 0x06009BAF RID: 39855 RVA: 0x003DE7BF File Offset: 0x003DC9BF
	public virtual int OnValue_CurrentWeaponType
	{
		get
		{
			if (!(this.CurrentWeapon == null))
			{
				return this.CurrentWeapon.AnimationType;
			}
			return 0;
		}
	}

	// Token: 0x17001036 RID: 4150
	// (get) Token: 0x06009BB0 RID: 39856 RVA: 0x003DE7DC File Offset: 0x003DC9DC
	public virtual int OnValue_CurrentWeaponGrip
	{
		get
		{
			if (!(this.CurrentWeapon == null))
			{
				return this.CurrentWeapon.AnimationGrip;
			}
			return 0;
		}
	}

	// Token: 0x04007852 RID: 30802
	public int StartWeapon;

	// Token: 0x04007853 RID: 30803
	public float AttackStateDisableDelay = 0.5f;

	// Token: 0x04007854 RID: 30804
	public float SetWeaponRefreshStatesDelay = 0.5f;

	// Token: 0x04007855 RID: 30805
	public float SetWeaponDuration = 0.1f;

	// Token: 0x04007856 RID: 30806
	public float SetWeaponReloadSleepDuration = 0.3f;

	// Token: 0x04007857 RID: 30807
	public float SetWeaponZoomSleepDuration = 0.3f;

	// Token: 0x04007858 RID: 30808
	public float SetWeaponAttackSleepDuration = 0.3f;

	// Token: 0x04007859 RID: 30809
	public float ReloadAttackSleepDuration = 0.3f;

	// Token: 0x0400785A RID: 30810
	public bool ReloadAutomatically = true;

	// Token: 0x0400785B RID: 30811
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_PlayerEventHandler m_Player;

	// Token: 0x0400785C RID: 30812
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<vp_Weapon> m_Weapons;

	// Token: 0x0400785D RID: 30813
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<List<vp_Weapon>> m_WeaponLists = new List<List<vp_Weapon>>();

	// Token: 0x0400785E RID: 30814
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int m_CurrentWeaponIndex = -1;

	// Token: 0x0400785F RID: 30815
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Weapon m_CurrentWeapon;

	// Token: 0x04007860 RID: 30816
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_SetWeaponTimer = new vp_Timer.Handle();

	// Token: 0x04007861 RID: 30817
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_SetWeaponRefreshTimer = new vp_Timer.Handle();

	// Token: 0x04007862 RID: 30818
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_DisableAttackStateTimer = new vp_Timer.Handle();

	// Token: 0x04007863 RID: 30819
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_DisableReloadStateTimer = new vp_Timer.Handle();

	// Token: 0x0200136A RID: 4970
	[PublicizedFrom(EAccessModifier.Protected)]
	public class WeaponComparer : IComparer
	{
		// Token: 0x06009BB5 RID: 39861 RVA: 0x003DE9C8 File Offset: 0x003DCBC8
		[PublicizedFrom(EAccessModifier.Private)]
		public int Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((vp_Weapon)x).gameObject.name, ((vp_Weapon)y).gameObject.name);
		}
	}
}

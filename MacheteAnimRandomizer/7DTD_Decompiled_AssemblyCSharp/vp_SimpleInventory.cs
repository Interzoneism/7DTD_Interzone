using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001343 RID: 4931
public class vp_SimpleInventory : MonoBehaviour
{
	// Token: 0x17000F9E RID: 3998
	// (get) Token: 0x0600991B RID: 39195 RVA: 0x003CE706 File Offset: 0x003CC906
	// (set) Token: 0x0600991C RID: 39196 RVA: 0x003CE70E File Offset: 0x003CC90E
	public vp_SimpleInventory.InventoryWeaponStatus CurrentWeaponStatus
	{
		get
		{
			return this.m_CurrentWeaponStatus;
		}
		set
		{
			this.m_CurrentWeaponStatus = value;
		}
	}

	// Token: 0x17000F9F RID: 3999
	// (get) Token: 0x0600991D RID: 39197 RVA: 0x003CE718 File Offset: 0x003CC918
	public List<vp_SimpleInventory.InventoryItemStatus> Weapons
	{
		get
		{
			List<vp_SimpleInventory.InventoryItemStatus> list = new List<vp_SimpleInventory.InventoryItemStatus>();
			foreach (vp_SimpleInventory.InventoryItemStatus item in this.m_WeaponTypes)
			{
				list.Add(item);
			}
			return list;
		}
	}

	// Token: 0x17000FA0 RID: 4000
	// (get) Token: 0x0600991E RID: 39198 RVA: 0x003CE774 File Offset: 0x003CC974
	public List<vp_SimpleInventory.InventoryItemStatus> EquippedWeapons
	{
		get
		{
			List<vp_SimpleInventory.InventoryItemStatus> list = new List<vp_SimpleInventory.InventoryItemStatus>();
			foreach (vp_SimpleInventory.InventoryItemStatus inventoryItemStatus in this.m_ItemStatusDictionary.Values)
			{
				if (inventoryItemStatus.GetType() == typeof(vp_SimpleInventory.InventoryWeaponStatus) && inventoryItemStatus.Have == 1)
				{
					list.Add(inventoryItemStatus);
				}
			}
			return list;
		}
	}

	// Token: 0x0600991F RID: 39199 RVA: 0x003CE7F4 File Offset: 0x003CC9F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	// Token: 0x06009920 RID: 39200 RVA: 0x003CE810 File Offset: 0x003CCA10
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x06009921 RID: 39201 RVA: 0x003CE82C File Offset: 0x003CCA2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		IComparer @object = new vp_SimpleInventory.InventoryWeaponStatusComparer();
		this.m_WeaponTypes.Sort(new Comparison<vp_SimpleInventory.InventoryWeaponStatus>(@object.Compare));
	}

	// Token: 0x17000FA1 RID: 4001
	// (get) Token: 0x06009922 RID: 39202 RVA: 0x003CE87C File Offset: 0x003CCA7C
	public Dictionary<string, vp_SimpleInventory.InventoryItemStatus> ItemStatusDictionary
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_ItemStatusDictionary == null)
			{
				this.m_ItemStatusDictionary = new Dictionary<string, vp_SimpleInventory.InventoryItemStatus>();
				for (int i = this.m_ItemTypes.Count - 1; i > -1; i--)
				{
					if (!this.m_ItemStatusDictionary.ContainsKey(this.m_ItemTypes[i].Name))
					{
						this.m_ItemStatusDictionary.Add(this.m_ItemTypes[i].Name, this.m_ItemTypes[i]);
					}
					else
					{
						this.m_ItemTypes.Remove(this.m_ItemTypes[i]);
					}
				}
				for (int j = this.m_WeaponTypes.Count - 1; j > -1; j--)
				{
					if (!this.m_ItemStatusDictionary.ContainsKey(this.m_WeaponTypes[j].Name))
					{
						this.m_ItemStatusDictionary.Add(this.m_WeaponTypes[j].Name, this.m_WeaponTypes[j]);
					}
					else
					{
						this.m_WeaponTypes.Remove(this.m_WeaponTypes[j]);
					}
				}
			}
			return this.m_ItemStatusDictionary;
		}
	}

	// Token: 0x06009923 RID: 39203 RVA: 0x003CE998 File Offset: 0x003CCB98
	public bool HaveItem(object name)
	{
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		return this.ItemStatusDictionary.TryGetValue((string)name, out inventoryItemStatus) && inventoryItemStatus.Have >= 1;
	}

	// Token: 0x06009924 RID: 39204 RVA: 0x003CE9C8 File Offset: 0x003CCBC8
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_SimpleInventory.InventoryItemStatus GetItemStatus(string name)
	{
		vp_SimpleInventory.InventoryItemStatus result;
		if (!this.ItemStatusDictionary.TryGetValue(name, out result))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error: (",
				(this != null) ? this.ToString() : null,
				"). Unknown item type: '",
				name,
				"'."
			}));
		}
		return result;
	}

	// Token: 0x06009925 RID: 39205 RVA: 0x003CEA24 File Offset: 0x003CCC24
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_SimpleInventory.InventoryWeaponStatus GetWeaponStatus(string name)
	{
		if (name == null)
		{
			return null;
		}
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		if (!this.ItemStatusDictionary.TryGetValue(name, out inventoryItemStatus))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error: (",
				(this != null) ? this.ToString() : null,
				"). Unknown item type: '",
				name,
				"'."
			}));
			return null;
		}
		if (inventoryItemStatus.GetType() != typeof(vp_SimpleInventory.InventoryWeaponStatus))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error: (",
				(this != null) ? this.ToString() : null,
				"). Item is not a weapon: '",
				name,
				"'."
			}));
			return null;
		}
		return (vp_SimpleInventory.InventoryWeaponStatus)inventoryItemStatus;
	}

	// Token: 0x06009926 RID: 39206 RVA: 0x003CEAE0 File Offset: 0x003CCCE0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void RefreshWeaponStatus()
	{
		if (!this.m_Player.CurrentWeaponWielded.Get() && this.m_RefreshWeaponStatusIterations < 50)
		{
			this.m_RefreshWeaponStatusIterations++;
			vp_Timer.In(0.1f, new vp_Timer.Callback(this.RefreshWeaponStatus), null);
			return;
		}
		this.m_RefreshWeaponStatusIterations = 0;
		string text = this.m_Player.CurrentWeaponName.Get();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.m_CurrentWeaponStatus = this.GetWeaponStatus(text);
	}

	// Token: 0x17000FA2 RID: 4002
	// (get) Token: 0x06009927 RID: 39207 RVA: 0x003CEB67 File Offset: 0x003CCD67
	// (set) Token: 0x06009928 RID: 39208 RVA: 0x003CEB7E File Offset: 0x003CCD7E
	public virtual int OnValue_CurrentWeaponAmmoCount
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return 0;
			}
			return this.m_CurrentWeaponStatus.LoadedAmmo;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return;
			}
			this.m_CurrentWeaponStatus.LoadedAmmo = value;
		}
	}

	// Token: 0x17000FA3 RID: 4003
	// (get) Token: 0x06009929 RID: 39209 RVA: 0x003CEB98 File Offset: 0x003CCD98
	public virtual int OnValue_CurrentWeaponClipCount
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return 0;
			}
			vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
			if (!this.ItemStatusDictionary.TryGetValue(this.m_CurrentWeaponStatus.ClipType, out inventoryItemStatus))
			{
				return 0;
			}
			return inventoryItemStatus.Have;
		}
	}

	// Token: 0x17000FA4 RID: 4004
	// (get) Token: 0x0600992A RID: 39210 RVA: 0x003CEBD1 File Offset: 0x003CCDD1
	public virtual string OnValue_CurrentWeaponClipType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_CurrentWeaponStatus == null)
			{
				return "";
			}
			return this.m_CurrentWeaponStatus.ClipType;
		}
	}

	// Token: 0x0600992B RID: 39211 RVA: 0x003CEBEC File Offset: 0x003CCDEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual int OnMessage_GetItemCount(string name)
	{
		vp_SimpleInventory.InventoryItemStatus inventoryItemStatus;
		if (!this.ItemStatusDictionary.TryGetValue(name, out inventoryItemStatus))
		{
			return 0;
		}
		return inventoryItemStatus.Have;
	}

	// Token: 0x0600992C RID: 39212 RVA: 0x003CEC11 File Offset: 0x003CCE11
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_DepleteAmmo()
	{
		if (this.m_CurrentWeaponStatus == null)
		{
			return false;
		}
		if (this.m_CurrentWeaponStatus.LoadedAmmo < 1)
		{
			return this.m_CurrentWeaponStatus.MaxAmmo == 0;
		}
		this.m_CurrentWeaponStatus.LoadedAmmo--;
		return true;
	}

	// Token: 0x0600992D RID: 39213 RVA: 0x003CEC50 File Offset: 0x003CCE50
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_AddAmmo(object arg)
	{
		object[] array = (object[])arg;
		string name = (string)array[0];
		int num = (array.Length == 2) ? ((int)array[1]) : -1;
		vp_SimpleInventory.InventoryWeaponStatus weaponStatus = this.GetWeaponStatus(name);
		if (weaponStatus == null)
		{
			return false;
		}
		if (num == -1)
		{
			weaponStatus.LoadedAmmo = weaponStatus.MaxAmmo;
		}
		else
		{
			weaponStatus.LoadedAmmo = Mathf.Min(weaponStatus.LoadedAmmo + num, weaponStatus.MaxAmmo);
		}
		return true;
	}

	// Token: 0x0600992E RID: 39214 RVA: 0x003CECB8 File Offset: 0x003CCEB8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_AddItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length == 2) ? ((int)array[1]) : 1;
		vp_SimpleInventory.InventoryItemStatus itemStatus = this.GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		itemStatus.CanHave = Mathf.Max(1, itemStatus.CanHave);
		if (itemStatus.Have >= itemStatus.CanHave)
		{
			return false;
		}
		itemStatus.Have = Mathf.Min(itemStatus.Have + num, itemStatus.CanHave);
		return true;
	}

	// Token: 0x0600992F RID: 39215 RVA: 0x003CED30 File Offset: 0x003CCF30
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_RemoveItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length == 2) ? ((int)array[1]) : 1;
		vp_SimpleInventory.InventoryItemStatus itemStatus = this.GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		if (itemStatus.Have <= 0)
		{
			return false;
		}
		itemStatus.Have = Mathf.Max(itemStatus.Have - num, 0);
		return true;
	}

	// Token: 0x06009930 RID: 39216 RVA: 0x003CED8C File Offset: 0x003CCF8C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool OnAttempt_RemoveClip()
	{
		return this.m_CurrentWeaponStatus != null && this.GetItemStatus(this.m_CurrentWeaponStatus.ClipType) != null && this.m_CurrentWeaponStatus.LoadedAmmo < this.m_CurrentWeaponStatus.MaxAmmo && this.m_Player.RemoveItem.Try(new object[]
		{
			this.m_CurrentWeaponStatus.ClipType
		});
	}

	// Token: 0x06009931 RID: 39217 RVA: 0x003CEE00 File Offset: 0x003CD000
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanStart_SetWeapon()
	{
		int num = (int)this.m_Player.SetWeapon.Argument;
		return num == 0 || (num >= 0 && num <= this.m_WeaponTypes.Count && this.HaveItem(this.m_WeaponTypes[num - 1].Name));
	}

	// Token: 0x06009932 RID: 39218 RVA: 0x003CEE55 File Offset: 0x003CD055
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStop_SetWeapon()
	{
		this.RefreshWeaponStatus();
	}

	// Token: 0x06009933 RID: 39219 RVA: 0x003CEE60 File Offset: 0x003CD060
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnStart_Dead()
	{
		if (this.m_ItemStatusDictionary == null)
		{
			return;
		}
		foreach (vp_SimpleInventory.InventoryItemStatus inventoryItemStatus in this.m_ItemStatusDictionary.Values)
		{
			if (inventoryItemStatus.ClearOnDeath)
			{
				inventoryItemStatus.Have = 0;
				if (inventoryItemStatus.GetType() == typeof(vp_SimpleInventory.InventoryWeaponStatus))
				{
					((vp_SimpleInventory.InventoryWeaponStatus)inventoryItemStatus).LoadedAmmo = 0;
				}
			}
		}
	}

	// Token: 0x040075C8 RID: 30152
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;

	// Token: 0x040075C9 RID: 30153
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<vp_SimpleInventory.InventoryItemStatus> m_ItemTypes;

	// Token: 0x040075CA RID: 30154
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<vp_SimpleInventory.InventoryWeaponStatus> m_WeaponTypes;

	// Token: 0x040075CB RID: 30155
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<string, vp_SimpleInventory.InventoryItemStatus> m_ItemStatusDictionary;

	// Token: 0x040075CC RID: 30156
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_SimpleInventory.InventoryWeaponStatus m_CurrentWeaponStatus;

	// Token: 0x040075CD RID: 30157
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int m_RefreshWeaponStatusIterations;

	// Token: 0x02001344 RID: 4932
	[PublicizedFrom(EAccessModifier.Protected)]
	public class InventoryWeaponStatusComparer : IComparer
	{
		// Token: 0x06009935 RID: 39221 RVA: 0x003CEEEC File Offset: 0x003CD0EC
		[PublicizedFrom(EAccessModifier.Private)]
		public int Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((vp_SimpleInventory.InventoryWeaponStatus)x).Name, ((vp_SimpleInventory.InventoryWeaponStatus)y).Name);
		}
	}

	// Token: 0x02001345 RID: 4933
	[Serializable]
	public class InventoryItemStatus
	{
		// Token: 0x040075CE RID: 30158
		public string Name = "Unnamed";

		// Token: 0x040075CF RID: 30159
		public int Have;

		// Token: 0x040075D0 RID: 30160
		public int CanHave = 1;

		// Token: 0x040075D1 RID: 30161
		public bool ClearOnDeath = true;
	}

	// Token: 0x02001346 RID: 4934
	[Serializable]
	public class InventoryWeaponStatus : vp_SimpleInventory.InventoryItemStatus
	{
		// Token: 0x040075D2 RID: 30162
		public string ClipType = "";

		// Token: 0x040075D3 RID: 30163
		public int LoadedAmmo;

		// Token: 0x040075D4 RID: 30164
		public int MaxAmmo = 10;
	}
}

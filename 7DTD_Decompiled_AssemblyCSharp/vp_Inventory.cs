using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001311 RID: 4881
[Serializable]
public class vp_Inventory : MonoBehaviour
{
	// Token: 0x17000F88 RID: 3976
	// (get) Token: 0x06009801 RID: 38913 RVA: 0x003C7319 File Offset: 0x003C5519
	public Transform Transform
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (this.m_Transform == null)
			{
				this.m_Transform = base.transform;
			}
			return this.m_Transform;
		}
	}

	// Token: 0x17000F89 RID: 3977
	// (get) Token: 0x06009802 RID: 38914 RVA: 0x003C733B File Offset: 0x003C553B
	public List<vp_UnitBankInstance> UnitBankInstances
	{
		get
		{
			return this.m_UnitBankInstances;
		}
	}

	// Token: 0x17000F8A RID: 3978
	// (get) Token: 0x06009803 RID: 38915 RVA: 0x003C7343 File Offset: 0x003C5543
	public List<vp_UnitBankInstance> InternalUnitBanks
	{
		get
		{
			return this.m_InternalUnitBanks;
		}
	}

	// Token: 0x17000F8B RID: 3979
	// (get) Token: 0x06009804 RID: 38916 RVA: 0x003C734B File Offset: 0x003C554B
	// (set) Token: 0x06009805 RID: 38917 RVA: 0x003C735D File Offset: 0x003C555D
	public float TotalSpace
	{
		get
		{
			return Mathf.Max(-1f, this.m_TotalSpace);
		}
		set
		{
			this.m_TotalSpace = value;
		}
	}

	// Token: 0x17000F8C RID: 3980
	// (get) Token: 0x06009807 RID: 38919 RVA: 0x003C7379 File Offset: 0x003C5579
	// (set) Token: 0x06009806 RID: 38918 RVA: 0x003C7366 File Offset: 0x003C5566
	public float UsedSpace
	{
		get
		{
			return Mathf.Max(0f, this.m_UsedSpace);
		}
		set
		{
			this.m_UsedSpace = Mathf.Max(0f, value);
		}
	}

	// Token: 0x17000F8D RID: 3981
	// (get) Token: 0x06009808 RID: 38920 RVA: 0x003C738B File Offset: 0x003C558B
	[SerializeField]
	[HideInInspector]
	public float RemainingSpace
	{
		get
		{
			return Mathf.Max(0f, this.TotalSpace - this.UsedSpace);
		}
	}

	// Token: 0x06009809 RID: 38921 RVA: 0x003C73A4 File Offset: 0x003C55A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.SaveInitialState();
	}

	// Token: 0x0600980A RID: 38922 RVA: 0x003C73AC File Offset: 0x003C55AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		this.Refresh();
	}

	// Token: 0x0600980B RID: 38923 RVA: 0x003C73B4 File Offset: 0x003C55B4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		vp_TargetEventReturn<vp_Inventory>.Register(this.Transform, "GetInventory", new Func<vp_Inventory>(this.GetInventory));
		vp_TargetEventReturn<vp_ItemType, int, bool>.Register(this.Transform, "TryGiveItem", new Func<vp_ItemType, int, bool>(this.TryGiveItem));
		vp_TargetEventReturn<vp_ItemType, int, bool>.Register(this.Transform, "TryGiveItems", new Func<vp_ItemType, int, bool>(this.TryGiveItems));
		vp_TargetEventReturn<vp_UnitBankType, int, int, bool>.Register(this.Transform, "TryGiveUnitBank", new Func<vp_UnitBankType, int, int, bool>(this.TryGiveUnitBank));
		vp_TargetEventReturn<vp_UnitType, int, bool>.Register(this.Transform, "TryGiveUnits", new Func<vp_UnitType, int, bool>(this.TryGiveUnits));
		vp_TargetEventReturn<vp_UnitBankType, int, int, bool>.Register(this.Transform, "TryDeduct", new Func<vp_UnitBankType, int, int, bool>(this.TryDeduct));
		vp_TargetEventReturn<vp_ItemType, int>.Register(this.Transform, "GetItemCount", new Func<vp_ItemType, int>(this.GetItemCount));
	}

	// Token: 0x0600980C RID: 38924 RVA: 0x003C748C File Offset: 0x003C568C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		vp_TargetEventReturn<vp_ItemType, int, bool>.Unregister(this.Transform, "TryGiveItem", new Func<vp_ItemType, int, bool>(this.TryGiveItem));
		vp_TargetEventReturn<vp_ItemType, int, bool>.Unregister(this.Transform, "TryGiveItems", new Func<vp_ItemType, int, bool>(this.TryGiveItems));
		vp_TargetEventReturn<vp_UnitBankType, int, int, bool>.Unregister(this.Transform, "TryGiveUnitBank", new Func<vp_UnitBankType, int, int, bool>(this.TryGiveUnitBank));
		vp_TargetEventReturn<vp_UnitType, int, bool>.Unregister(this.Transform, "TryGiveUnits", new Func<vp_UnitType, int, bool>(this.TryGiveUnits));
		vp_TargetEventReturn<vp_UnitBankType, int, int, bool>.Unregister(this.Transform, "TryDeduct", new Func<vp_UnitBankType, int, int, bool>(this.TryDeduct));
		vp_TargetEventReturn<vp_ItemType, int>.Unregister(this.Transform, "GetItemCount", new Func<vp_ItemType, int>(this.GetItemCount));
		vp_TargetEventReturn<vp_Inventory>.Unregister(this.Transform, "HasInventory", new Func<vp_Inventory>(this.GetInventory));
	}

	// Token: 0x0600980D RID: 38925 RVA: 0x00112051 File Offset: 0x00110251
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual vp_Inventory GetInventory()
	{
		return this;
	}

	// Token: 0x0600980E RID: 38926 RVA: 0x003C7564 File Offset: 0x003C5764
	public virtual bool TryGiveItems(vp_ItemType type, int amount)
	{
		bool result = false;
		while (amount > 0)
		{
			if (this.TryGiveItem(type, 0))
			{
				result = true;
			}
			amount--;
		}
		return result;
	}

	// Token: 0x0600980F RID: 38927 RVA: 0x003C758C File Offset: 0x003C578C
	public virtual bool TryGiveItem(vp_ItemType itemType, int id)
	{
		if (itemType == null)
		{
			Debug.LogError("Error (" + vp_Utility.GetErrorLocation(2, false) + ") Item type was null.");
			return false;
		}
		vp_UnitType vp_UnitType = itemType as vp_UnitType;
		if (vp_UnitType != null)
		{
			return this.TryGiveUnits(vp_UnitType, id);
		}
		vp_UnitBankType vp_UnitBankType = itemType as vp_UnitBankType;
		if (vp_UnitBankType != null)
		{
			return this.TryGiveUnitBank(vp_UnitBankType, vp_UnitBankType.Capacity, id);
		}
		if (this.CapsEnabled)
		{
			int itemCap = this.GetItemCap(itemType);
			if (itemCap != -1 && this.GetItemCount(itemType) >= itemCap)
			{
				return false;
			}
		}
		if (this.SpaceEnabled && this.UsedSpace + itemType.Space > this.TotalSpace)
		{
			return false;
		}
		this.DoAddItem(itemType, id);
		return true;
	}

	// Token: 0x06009810 RID: 38928 RVA: 0x003C7640 File Offset: 0x003C5840
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DoAddItem(vp_ItemType type, int id)
	{
		this.ItemInstances.Add(new vp_ItemInstance(type, id));
		if (this.SpaceEnabled)
		{
			this.m_UsedSpace += type.Space;
		}
		this.m_FirstItemsDirty = true;
		this.m_ItemDictionaryDirty = true;
		this.SetDirty();
	}

	// Token: 0x06009811 RID: 38929 RVA: 0x003C7690 File Offset: 0x003C5890
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DoRemoveItem(vp_ItemInstance item)
	{
		if (item is vp_UnitBankInstance)
		{
			this.DoRemoveUnitBank(item as vp_UnitBankInstance);
			return;
		}
		this.ItemInstances.Remove(item);
		this.m_FirstItemsDirty = true;
		this.m_ItemDictionaryDirty = true;
		if (this.SpaceEnabled)
		{
			this.m_UsedSpace = Mathf.Max(0f, this.m_UsedSpace - item.Type.Space);
		}
		this.SetDirty();
	}

	// Token: 0x06009812 RID: 38930 RVA: 0x003C7700 File Offset: 0x003C5900
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DoAddUnitBank(vp_UnitBankType unitBankType, int id, int unitsLoaded)
	{
		vp_UnitBankInstance vp_UnitBankInstance = new vp_UnitBankInstance(unitBankType, id, this);
		this.m_UnitBankInstances.Add(vp_UnitBankInstance);
		this.m_FirstItemsDirty = true;
		this.m_ItemDictionaryDirty = true;
		if (this.SpaceEnabled && !vp_UnitBankInstance.IsInternal)
		{
			this.m_UsedSpace += unitBankType.Space;
		}
		vp_UnitBankInstance.TryGiveUnits(unitsLoaded);
		if (this.SpaceEnabled && !vp_UnitBankInstance.IsInternal && this.SpaceMode == vp_Inventory.Mode.Weight && unitBankType.Unit != null)
		{
			this.m_UsedSpace += unitBankType.Unit.Space * (float)vp_UnitBankInstance.Count;
		}
		this.SetDirty();
	}

	// Token: 0x06009813 RID: 38931 RVA: 0x003C77A8 File Offset: 0x003C59A8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void DoRemoveUnitBank(vp_UnitBankInstance bank)
	{
		if (!bank.IsInternal)
		{
			this.m_UnitBankInstances.RemoveAt(this.m_UnitBankInstances.IndexOf(bank));
			this.m_FirstItemsDirty = true;
			this.m_ItemDictionaryDirty = true;
			if (this.SpaceEnabled)
			{
				this.m_UsedSpace -= bank.Type.Space;
				if (this.SpaceMode == vp_Inventory.Mode.Weight)
				{
					this.m_UsedSpace -= bank.UnitType.Space * (float)bank.Count;
				}
			}
		}
		else
		{
			this.m_InternalUnitBanks.RemoveAt(this.m_InternalUnitBanks.IndexOf(bank));
		}
		this.SetDirty();
	}

	// Token: 0x06009814 RID: 38932 RVA: 0x003C7849 File Offset: 0x003C5A49
	public virtual bool DoAddUnits(vp_UnitBankInstance bank, int amount)
	{
		return bank.DoAddUnits(amount);
	}

	// Token: 0x06009815 RID: 38933 RVA: 0x003C7852 File Offset: 0x003C5A52
	public virtual bool DoRemoveUnits(vp_UnitBankInstance bank, int amount)
	{
		return bank.DoRemoveUnits(amount);
	}

	// Token: 0x06009816 RID: 38934 RVA: 0x003C785B File Offset: 0x003C5A5B
	public virtual bool TryGiveUnits(vp_UnitType unitType, int amount)
	{
		return this.GetItemCap(unitType) != 0 && this.TryGiveUnits(this.GetInternalUnitBank(unitType), amount);
	}

	// Token: 0x06009817 RID: 38935 RVA: 0x003C7878 File Offset: 0x003C5A78
	public virtual bool TryGiveUnits(vp_UnitBankInstance bank, int amount)
	{
		if (bank == null)
		{
			return false;
		}
		amount = Mathf.Max(0, amount);
		if (this.SpaceEnabled && (bank.IsInternal || this.SpaceMode == vp_Inventory.Mode.Weight) && this.RemainingSpace < (float)amount * bank.UnitType.Space)
		{
			amount = (int)(this.RemainingSpace / bank.UnitType.Space);
			return this.DoAddUnits(bank, amount);
		}
		return this.DoAddUnits(bank, amount);
	}

	// Token: 0x06009818 RID: 38936 RVA: 0x003C78E8 File Offset: 0x003C5AE8
	public virtual bool TryRemoveUnits(vp_UnitType unitType, int amount)
	{
		vp_UnitBankInstance internalUnitBank = this.GetInternalUnitBank(unitType);
		return internalUnitBank != null && this.DoRemoveUnits(internalUnitBank, amount);
	}

	// Token: 0x06009819 RID: 38937 RVA: 0x003C790C File Offset: 0x003C5B0C
	public virtual bool TryGiveUnitBank(vp_UnitBankType unitBankType, int unitsLoaded, int id)
	{
		if (unitBankType == null)
		{
			Debug.LogError("Error (" + vp_Utility.GetErrorLocation(1, false) + ") 'unitBankType' was null.");
			return false;
		}
		if (this.CapsEnabled)
		{
			int itemCap = this.GetItemCap(unitBankType);
			if (itemCap != -1 && this.GetItemCount(unitBankType) >= itemCap)
			{
				return false;
			}
			if (unitBankType.Capacity != -1)
			{
				unitsLoaded = Mathf.Min(unitsLoaded, unitBankType.Capacity);
			}
		}
		if (this.SpaceEnabled)
		{
			vp_Inventory.Mode spaceMode = this.SpaceMode;
			if (spaceMode != vp_Inventory.Mode.Weight)
			{
				if (spaceMode == vp_Inventory.Mode.Volume)
				{
					if (this.UsedSpace + unitBankType.Space > this.TotalSpace)
					{
						return false;
					}
				}
			}
			else
			{
				if (unitBankType.Unit == null)
				{
					Debug.LogError("Error (vp_Inventory) UnitBank item type " + ((unitBankType != null) ? unitBankType.ToString() : null) + " can't be added because its unit type has not been set.");
					return false;
				}
				if (this.UsedSpace + unitBankType.Space + unitBankType.Unit.Space * (float)unitsLoaded > this.TotalSpace)
				{
					return false;
				}
			}
		}
		this.DoAddUnitBank(unitBankType, id, unitsLoaded);
		return true;
	}

	// Token: 0x0600981A RID: 38938 RVA: 0x003C7A08 File Offset: 0x003C5C08
	public virtual bool TryRemoveItems(vp_ItemType type, int amount)
	{
		bool result = false;
		while (amount > 0)
		{
			if (this.TryRemoveItem(type, -1))
			{
				result = true;
			}
			amount--;
		}
		return result;
	}

	// Token: 0x0600981B RID: 38939 RVA: 0x003C7A2F File Offset: 0x003C5C2F
	public virtual bool TryRemoveItem(vp_ItemType type, int id)
	{
		return this.TryRemoveItem(this.GetItem(type, id));
	}

	// Token: 0x0600981C RID: 38940 RVA: 0x003C7A3F File Offset: 0x003C5C3F
	public virtual bool TryRemoveItem(vp_ItemInstance item)
	{
		if (item == null)
		{
			return false;
		}
		this.DoRemoveItem(item);
		this.SetDirty();
		return true;
	}

	// Token: 0x0600981D RID: 38941 RVA: 0x003C7A54 File Offset: 0x003C5C54
	public virtual bool TryRemoveUnitBanks(vp_UnitBankType type, int amount)
	{
		bool result = false;
		while (amount > 0)
		{
			if (this.TryRemoveUnitBank(type, -1))
			{
				result = true;
			}
			amount--;
		}
		return result;
	}

	// Token: 0x0600981E RID: 38942 RVA: 0x003C7A7B File Offset: 0x003C5C7B
	public virtual bool TryRemoveUnitBank(vp_UnitBankType type, int id)
	{
		return this.TryRemoveUnitBank(this.GetItem(type, id) as vp_UnitBankInstance);
	}

	// Token: 0x0600981F RID: 38943 RVA: 0x003C7A90 File Offset: 0x003C5C90
	public virtual bool TryRemoveUnitBank(vp_UnitBankInstance unitBank)
	{
		if (unitBank == null)
		{
			return false;
		}
		this.DoRemoveUnitBank(unitBank);
		this.SetDirty();
		return true;
	}

	// Token: 0x06009820 RID: 38944 RVA: 0x003C7AA5 File Offset: 0x003C5CA5
	public virtual bool TryReload(vp_ItemType itemType, int unitBankId)
	{
		return this.TryReload(this.GetItem(itemType, unitBankId) as vp_UnitBankInstance, -1);
	}

	// Token: 0x06009821 RID: 38945 RVA: 0x003C7ABB File Offset: 0x003C5CBB
	public virtual bool TryReload(vp_ItemType itemType, int unitBankId, int amount)
	{
		return this.TryReload(this.GetItem(itemType, unitBankId) as vp_UnitBankInstance, amount);
	}

	// Token: 0x06009822 RID: 38946 RVA: 0x003C7AD1 File Offset: 0x003C5CD1
	public virtual bool TryReload(vp_UnitBankInstance bank)
	{
		return this.TryReload(bank, -1);
	}

	// Token: 0x06009823 RID: 38947 RVA: 0x003C7ADC File Offset: 0x003C5CDC
	public virtual bool TryReload(vp_UnitBankInstance bank, int amount)
	{
		if (bank == null || bank.IsInternal || bank.ID == -1)
		{
			Debug.LogWarning("Warning (" + vp_Utility.GetErrorLocation(1, false) + ") 'TryReloadUnitBank' could not identify a target item. If you are trying to add units to the main inventory please instead use 'TryGiveUnits'.");
			return false;
		}
		int count = bank.Count;
		if (count >= bank.Capacity)
		{
			return false;
		}
		int unitCount = this.GetUnitCount(bank.UnitType);
		if (unitCount < 1)
		{
			return false;
		}
		if (amount == -1)
		{
			amount = bank.Capacity;
		}
		this.TryRemoveUnits(bank.UnitType, amount);
		int num = Mathf.Max(0, unitCount - this.GetUnitCount(bank.UnitType));
		if (!this.DoAddUnits(bank, num))
		{
			return false;
		}
		int num2 = Mathf.Max(0, bank.Count - count);
		if (num2 < 1)
		{
			return false;
		}
		if (num2 > 0 && num2 < num)
		{
			this.TryGiveUnits(bank.UnitType, num - num2);
		}
		return true;
	}

	// Token: 0x06009824 RID: 38948 RVA: 0x003C7BA8 File Offset: 0x003C5DA8
	public virtual bool TryDeduct(vp_UnitBankType unitBankType, int unitBankId, int amount)
	{
		vp_UnitBankInstance vp_UnitBankInstance = (unitBankId < 1) ? (this.GetItem(unitBankType) as vp_UnitBankInstance) : (this.GetItem(unitBankType, unitBankId) as vp_UnitBankInstance);
		if (vp_UnitBankInstance == null)
		{
			return false;
		}
		if (!this.DoRemoveUnits(vp_UnitBankInstance, amount))
		{
			return false;
		}
		if (vp_UnitBankInstance.Count <= 0 && (vp_UnitBankInstance.Type as vp_UnitBankType).RemoveWhenDepleted)
		{
			this.DoRemoveUnitBank(vp_UnitBankInstance);
		}
		return true;
	}

	// Token: 0x06009825 RID: 38949 RVA: 0x003C7C0C File Offset: 0x003C5E0C
	public virtual vp_ItemInstance GetItem(vp_ItemType itemType)
	{
		if (this.m_FirstItemsDirty)
		{
			this.m_FirstItemsOfType.Clear();
			foreach (vp_ItemInstance vp_ItemInstance in this.ItemInstances)
			{
				if (vp_ItemInstance != null && !this.m_FirstItemsOfType.ContainsKey(vp_ItemInstance.Type))
				{
					this.m_FirstItemsOfType.Add(vp_ItemInstance.Type, vp_ItemInstance);
				}
			}
			foreach (vp_UnitBankInstance vp_UnitBankInstance in this.UnitBankInstances)
			{
				if (vp_UnitBankInstance != null && !this.m_FirstItemsOfType.ContainsKey(vp_UnitBankInstance.Type))
				{
					this.m_FirstItemsOfType.Add(vp_UnitBankInstance.Type, vp_UnitBankInstance);
				}
			}
			this.m_FirstItemsDirty = false;
		}
		if (itemType == null || !this.m_FirstItemsOfType.TryGetValue(itemType, out this.m_GetFirstItemInstanceResult))
		{
			return null;
		}
		if (this.m_GetFirstItemInstanceResult == null)
		{
			this.m_FirstItemsDirty = true;
			return this.GetItem(itemType);
		}
		return this.m_GetFirstItemInstanceResult;
	}

	// Token: 0x06009826 RID: 38950 RVA: 0x003C7D3C File Offset: 0x003C5F3C
	public vp_ItemInstance GetItem(vp_ItemType itemType, int id)
	{
		if (itemType == null)
		{
			Debug.LogError("Error (" + vp_Utility.GetErrorLocation(1, true) + ") Sent a null itemType to 'GetItem'.");
			return null;
		}
		if (id < 1)
		{
			return this.GetItem(itemType);
		}
		if (this.m_ItemDictionaryDirty)
		{
			this.m_ItemDictionary.Clear();
			this.m_ItemDictionaryDirty = false;
		}
		if (!this.m_ItemDictionary.TryGetValue(id, out this.m_GetItemResult))
		{
			this.m_GetItemResult = this.GetItemFromList(itemType, id);
			if (this.m_GetItemResult != null && id > 0)
			{
				this.m_ItemDictionary.Add(id, this.m_GetItemResult);
			}
		}
		else if (this.m_GetItemResult != null)
		{
			if (this.m_GetItemResult.Type != itemType)
			{
				Debug.LogWarning("Warning: (vp_Inventory) Player has vp_FPWeapons with identical, non-zero vp_ItemIdentifier IDs! This is much slower than using zero or differing IDs.");
				this.m_GetItemResult = this.GetItemFromList(itemType, id);
			}
		}
		else
		{
			this.m_ItemDictionary.Remove(id);
			this.GetItem(itemType, id);
		}
		return this.m_GetItemResult;
	}

	// Token: 0x06009827 RID: 38951 RVA: 0x003C7E28 File Offset: 0x003C6028
	public virtual vp_ItemInstance GetItem(string itemTypeName)
	{
		for (int i = 0; i < this.InternalUnitBanks.Count; i++)
		{
			if (this.InternalUnitBanks[i].UnitType.name == itemTypeName)
			{
				return this.InternalUnitBanks[i];
			}
		}
		for (int j = 0; j < this.m_UnitBankInstances.Count; j++)
		{
			if (this.m_UnitBankInstances[j].Type.name == itemTypeName)
			{
				return this.m_UnitBankInstances[j];
			}
		}
		for (int k = 0; k < this.ItemInstances.Count; k++)
		{
			if (this.ItemInstances[k].Type.name == itemTypeName)
			{
				return this.ItemInstances[k];
			}
		}
		return null;
	}

	// Token: 0x06009828 RID: 38952 RVA: 0x003C7EFC File Offset: 0x003C60FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual vp_ItemInstance GetItemFromList(vp_ItemType itemType, int id = -1)
	{
		for (int i = 0; i < this.m_UnitBankInstances.Count; i++)
		{
			if (!(this.m_UnitBankInstances[i].Type != itemType) && (id == -1 || this.m_UnitBankInstances[i].ID == id))
			{
				return this.m_UnitBankInstances[i];
			}
		}
		for (int j = 0; j < this.ItemInstances.Count; j++)
		{
			if (!(this.ItemInstances[j].Type != itemType) && (id == -1 || this.ItemInstances[j].ID == id))
			{
				return this.ItemInstances[j];
			}
		}
		return null;
	}

	// Token: 0x06009829 RID: 38953 RVA: 0x003C7FB2 File Offset: 0x003C61B2
	public virtual bool HaveItem(vp_ItemType itemType, int id = -1)
	{
		return !(itemType == null) && this.GetItem(itemType, id) != null;
	}

	// Token: 0x0600982A RID: 38954 RVA: 0x003C7FCC File Offset: 0x003C61CC
	public virtual vp_UnitBankInstance GetInternalUnitBank(vp_UnitType unitType)
	{
		for (int i = 0; i < this.m_InternalUnitBanks.Count; i++)
		{
			if (!(this.m_InternalUnitBanks[i].GetType() != typeof(vp_UnitBankInstance)) && !(this.m_InternalUnitBanks[i].Type != null))
			{
				vp_UnitBankInstance vp_UnitBankInstance = this.m_InternalUnitBanks[i];
				if (!(vp_UnitBankInstance.UnitType != unitType))
				{
					return vp_UnitBankInstance;
				}
			}
		}
		this.SetDirty();
		vp_UnitBankInstance vp_UnitBankInstance2 = new vp_UnitBankInstance(unitType, this);
		vp_UnitBankInstance2.Capacity = this.GetItemCap(unitType);
		this.m_InternalUnitBanks.Add(vp_UnitBankInstance2);
		return vp_UnitBankInstance2;
	}

	// Token: 0x0600982B RID: 38955 RVA: 0x003C8070 File Offset: 0x003C6270
	public virtual bool HaveInternalUnitBank(vp_UnitType unitType)
	{
		for (int i = 0; i < this.m_InternalUnitBanks.Count; i++)
		{
			if (!(this.m_InternalUnitBanks[i].GetType() != typeof(vp_UnitBankInstance)) && !(this.m_InternalUnitBanks[i].Type != null) && !(this.m_InternalUnitBanks[i].UnitType != unitType))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600982C RID: 38956 RVA: 0x003C80EC File Offset: 0x003C62EC
	public virtual void Refresh()
	{
		for (int i = 0; i < this.m_InternalUnitBanks.Count; i++)
		{
			this.m_InternalUnitBanks[i].Capacity = this.GetItemCap(this.m_InternalUnitBanks[i].UnitType);
		}
		if (!this.SpaceEnabled)
		{
			return;
		}
		this.m_UsedSpace = 0f;
		for (int j = 0; j < this.ItemInstances.Count; j++)
		{
			this.m_UsedSpace += this.ItemInstances[j].Type.Space;
		}
		for (int k = 0; k < this.m_UnitBankInstances.Count; k++)
		{
			vp_Inventory.Mode spaceMode = this.SpaceMode;
			if (spaceMode != vp_Inventory.Mode.Weight)
			{
				if (spaceMode == vp_Inventory.Mode.Volume)
				{
					this.m_UsedSpace += this.m_UnitBankInstances[k].Type.Space;
				}
			}
			else
			{
				this.m_UsedSpace += this.m_UnitBankInstances[k].Type.Space + this.m_UnitBankInstances[k].UnitType.Space * (float)this.m_UnitBankInstances[k].Count;
			}
		}
		for (int l = 0; l < this.m_InternalUnitBanks.Count; l++)
		{
			this.m_UsedSpace += this.m_InternalUnitBanks[l].UnitType.Space * (float)this.m_InternalUnitBanks[l].Count;
		}
	}

	// Token: 0x0600982D RID: 38957 RVA: 0x003C8274 File Offset: 0x003C6474
	public virtual int GetItemCount(vp_ItemType type)
	{
		vp_UnitType vp_UnitType = type as vp_UnitType;
		if (vp_UnitType != null)
		{
			return this.GetUnitCount(vp_UnitType);
		}
		int num = 0;
		for (int i = 0; i < this.ItemInstances.Count; i++)
		{
			if (this.ItemInstances[i].Type == type)
			{
				num++;
			}
		}
		for (int j = 0; j < this.m_UnitBankInstances.Count; j++)
		{
			if (this.m_UnitBankInstances[j].Type == type)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0600982E RID: 38958 RVA: 0x003C8304 File Offset: 0x003C6504
	public virtual void SetItemCount(vp_ItemType type, int amount)
	{
		if (type is vp_UnitType)
		{
			this.SetUnitCount((vp_UnitType)type, amount);
			return;
		}
		bool capsEnabled = this.CapsEnabled;
		bool spaceEnabled = this.SpaceEnabled;
		this.CapsEnabled = false;
		this.SpaceEnabled = false;
		int num = amount - this.GetItemCount(type);
		if (num > 0)
		{
			this.TryGiveItems(type, amount);
		}
		else if (num < 0)
		{
			this.TryRemoveItems(type, -amount);
		}
		this.CapsEnabled = capsEnabled;
		this.SpaceEnabled = spaceEnabled;
	}

	// Token: 0x0600982F RID: 38959 RVA: 0x003C8378 File Offset: 0x003C6578
	public virtual void SetUnitCount(vp_UnitType unitType, int amount)
	{
		this.TrySetUnitCount(this.GetInternalUnitBank(unitType), amount);
	}

	// Token: 0x06009830 RID: 38960 RVA: 0x003C838C File Offset: 0x003C658C
	public virtual void SetUnitCount(vp_UnitBankInstance bank, int amount)
	{
		if (bank == null)
		{
			return;
		}
		amount = Mathf.Max(0, amount);
		if (amount == bank.Count)
		{
			return;
		}
		int count = bank.Count;
		if (!this.DoRemoveUnits(bank, bank.Count))
		{
			bank.Count = count;
		}
		if (amount == 0)
		{
			return;
		}
		if (!this.DoAddUnits(bank, amount))
		{
			bank.Count = count;
		}
	}

	// Token: 0x06009831 RID: 38961 RVA: 0x003C83E2 File Offset: 0x003C65E2
	public virtual bool TrySetUnitCount(vp_UnitType unitType, int amount)
	{
		return this.TrySetUnitCount(this.GetInternalUnitBank(unitType), amount);
	}

	// Token: 0x06009832 RID: 38962 RVA: 0x003C83F4 File Offset: 0x003C65F4
	public virtual bool TrySetUnitCount(vp_UnitBankInstance bank, int amount)
	{
		if (bank == null)
		{
			return false;
		}
		amount = Mathf.Max(0, amount);
		if (amount == bank.Count)
		{
			return true;
		}
		int count = bank.Count;
		if (!this.DoRemoveUnits(bank, bank.Count))
		{
			bank.Count = count;
		}
		if (amount == 0)
		{
			return true;
		}
		if (bank.IsInternal)
		{
			this.m_Result = this.TryGiveUnits(bank.UnitType, amount);
			if (!this.m_Result)
			{
				bank.Count = count;
			}
			return this.m_Result;
		}
		this.m_Result = this.TryGiveUnits(bank, amount);
		if (!this.m_Result)
		{
			bank.Count = count;
		}
		return this.m_Result;
	}

	// Token: 0x06009833 RID: 38963 RVA: 0x003C8490 File Offset: 0x003C6690
	public virtual int GetItemCap(vp_ItemType type)
	{
		if (!this.CapsEnabled)
		{
			return -1;
		}
		for (int i = 0; i < this.m_ItemCapInstances.Count; i++)
		{
			if (this.m_ItemCapInstances[i].Type == type)
			{
				return this.m_ItemCapInstances[i].Cap;
			}
		}
		if (this.AllowOnlyListed)
		{
			return 0;
		}
		return -1;
	}

	// Token: 0x06009834 RID: 38964 RVA: 0x003C84F4 File Offset: 0x003C66F4
	public virtual void SetItemCap(vp_ItemType type, int cap, bool clamp = false)
	{
		this.SetDirty();
		int i = 0;
		while (i < this.m_ItemCapInstances.Count)
		{
			if (this.m_ItemCapInstances[i].Type == type)
			{
				this.m_ItemCapInstances[i].Cap = cap;
				IL_5B:
				if (type is vp_UnitType)
				{
					for (int j = 0; j < this.m_InternalUnitBanks.Count; j++)
					{
						if (this.m_InternalUnitBanks[j].UnitType != null && this.m_InternalUnitBanks[j].UnitType == type)
						{
							this.m_InternalUnitBanks[j].Capacity = cap;
							if (clamp)
							{
								this.m_InternalUnitBanks[j].ClampToCapacity();
							}
						}
					}
					return;
				}
				if (clamp && this.GetItemCount(type) > cap)
				{
					this.TryRemoveItems(type, this.GetItemCount(type) - cap);
				}
				return;
			}
			else
			{
				i++;
			}
		}
		this.m_ItemCapInstances.Add(new vp_Inventory.ItemCap(type, cap));
		goto IL_5B;
	}

	// Token: 0x06009835 RID: 38965 RVA: 0x003C85F4 File Offset: 0x003C67F4
	public virtual int GetUnitCount(vp_UnitType unitType)
	{
		vp_UnitBankInstance internalUnitBank = this.GetInternalUnitBank(unitType);
		if (internalUnitBank == null)
		{
			return 0;
		}
		return internalUnitBank.Count;
	}

	// Token: 0x06009836 RID: 38966 RVA: 0x003C8614 File Offset: 0x003C6814
	public virtual void SaveInitialState()
	{
		for (int i = 0; i < this.InternalUnitBanks.Count; i++)
		{
			this.m_StartItems.Add(new vp_Inventory.StartItemRecord(this.InternalUnitBanks[i].UnitType, 0, this.InternalUnitBanks[i].Count));
		}
		for (int j = 0; j < this.m_UnitBankInstances.Count; j++)
		{
			this.m_StartItems.Add(new vp_Inventory.StartItemRecord(this.m_UnitBankInstances[j].Type, this.m_UnitBankInstances[j].ID, this.m_UnitBankInstances[j].Count));
		}
		for (int k = 0; k < this.ItemInstances.Count; k++)
		{
			this.m_StartItems.Add(new vp_Inventory.StartItemRecord(this.ItemInstances[k].Type, this.ItemInstances[k].ID, 1));
		}
	}

	// Token: 0x06009837 RID: 38967 RVA: 0x003C870C File Offset: 0x003C690C
	public virtual void Reset()
	{
		this.Clear();
		for (int i = 0; i < this.m_StartItems.Count; i++)
		{
			if (this.m_StartItems[i].Type.GetType() == typeof(vp_ItemType))
			{
				this.TryGiveItem(this.m_StartItems[i].Type, this.m_StartItems[i].ID);
			}
			else if (this.m_StartItems[i].Type.GetType() == typeof(vp_UnitBankType))
			{
				this.TryGiveUnitBank(this.m_StartItems[i].Type as vp_UnitBankType, this.m_StartItems[i].Amount, this.m_StartItems[i].ID);
			}
			else if (this.m_StartItems[i].Type.GetType() == typeof(vp_UnitType))
			{
				this.TryGiveUnits(this.m_StartItems[i].Type as vp_UnitType, this.m_StartItems[i].Amount);
			}
			else if (this.m_StartItems[i].Type.GetType().BaseType == typeof(vp_ItemType))
			{
				this.TryGiveItem(this.m_StartItems[i].Type, this.m_StartItems[i].ID);
			}
			else if (this.m_StartItems[i].Type.GetType().BaseType == typeof(vp_UnitBankType))
			{
				this.TryGiveUnitBank(this.m_StartItems[i].Type as vp_UnitBankType, this.m_StartItems[i].Amount, this.m_StartItems[i].ID);
			}
			else if (this.m_StartItems[i].Type.GetType().BaseType == typeof(vp_UnitType))
			{
				this.TryGiveUnits(this.m_StartItems[i].Type as vp_UnitType, this.m_StartItems[i].Amount);
			}
		}
	}

	// Token: 0x06009838 RID: 38968 RVA: 0x003C8978 File Offset: 0x003C6B78
	public virtual void Clear()
	{
		for (int i = this.InternalUnitBanks.Count - 1; i > -1; i--)
		{
			this.DoRemoveUnitBank(this.InternalUnitBanks[i]);
		}
		for (int j = this.m_UnitBankInstances.Count - 1; j > -1; j--)
		{
			this.DoRemoveUnitBank(this.m_UnitBankInstances[j]);
		}
		for (int k = this.ItemInstances.Count - 1; k > -1; k--)
		{
			this.DoRemoveItem(this.ItemInstances[k]);
		}
	}

	// Token: 0x06009839 RID: 38969 RVA: 0x003C8A03 File Offset: 0x003C6C03
	public virtual void SetTotalSpace(float spaceLimitation)
	{
		this.SetDirty();
		this.TotalSpace = Mathf.Max(0f, spaceLimitation);
	}

	// Token: 0x0600983A RID: 38970 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void SetDirty()
	{
	}

	// Token: 0x0600983B RID: 38971 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ClearItemDictionaries()
	{
	}

	// Token: 0x04007476 RID: 29814
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Inventory.ItemRecordsSection m_ItemRecords;

	// Token: 0x04007477 RID: 29815
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Inventory.ItemCapsSection m_ItemCaps;

	// Token: 0x04007478 RID: 29816
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Inventory.SpaceLimitSection m_SpaceLimit;

	// Token: 0x04007479 RID: 29817
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Transform m_Transform;

	// Token: 0x0400747A RID: 29818
	[SerializeField]
	[HideInInspector]
	public List<vp_ItemInstance> ItemInstances = new List<vp_ItemInstance>();

	// Token: 0x0400747B RID: 29819
	[SerializeField]
	[HideInInspector]
	public List<vp_Inventory.ItemCap> m_ItemCapInstances = new List<vp_Inventory.ItemCap>();

	// Token: 0x0400747C RID: 29820
	[SerializeField]
	[HideInInspector]
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<vp_UnitBankInstance> m_UnitBankInstances = new List<vp_UnitBankInstance>();

	// Token: 0x0400747D RID: 29821
	[SerializeField]
	[HideInInspector]
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<vp_UnitBankInstance> m_InternalUnitBanks = new List<vp_UnitBankInstance>();

	// Token: 0x0400747E RID: 29822
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int UNLIMITED = -1;

	// Token: 0x0400747F RID: 29823
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int UNIDENTIFIED = -1;

	// Token: 0x04007480 RID: 29824
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int MAXCAPACITY = -1;

	// Token: 0x04007481 RID: 29825
	[SerializeField]
	[HideInInspector]
	public bool CapsEnabled;

	// Token: 0x04007482 RID: 29826
	[SerializeField]
	[HideInInspector]
	public bool SpaceEnabled;

	// Token: 0x04007483 RID: 29827
	[SerializeField]
	[HideInInspector]
	public vp_Inventory.Mode SpaceMode;

	// Token: 0x04007484 RID: 29828
	[SerializeField]
	[HideInInspector]
	public bool AllowOnlyListed;

	// Token: 0x04007485 RID: 29829
	[SerializeField]
	[HideInInspector]
	[PublicizedFrom(EAccessModifier.Protected)]
	public float m_TotalSpace = 100f;

	// Token: 0x04007486 RID: 29830
	[SerializeField]
	[HideInInspector]
	[PublicizedFrom(EAccessModifier.Protected)]
	public float m_UsedSpace;

	// Token: 0x04007487 RID: 29831
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_Result;

	// Token: 0x04007488 RID: 29832
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<vp_Inventory.StartItemRecord> m_StartItems = new List<vp_Inventory.StartItemRecord>();

	// Token: 0x04007489 RID: 29833
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_FirstItemsDirty = true;

	// Token: 0x0400748A RID: 29834
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<vp_ItemType, vp_ItemInstance> m_FirstItemsOfType = new Dictionary<vp_ItemType, vp_ItemInstance>(100);

	// Token: 0x0400748B RID: 29835
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_ItemInstance m_GetFirstItemInstanceResult;

	// Token: 0x0400748C RID: 29836
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_ItemDictionaryDirty = true;

	// Token: 0x0400748D RID: 29837
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Dictionary<int, vp_ItemInstance> m_ItemDictionary = new Dictionary<int, vp_ItemInstance>();

	// Token: 0x0400748E RID: 29838
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_ItemInstance m_GetItemResult;

	// Token: 0x02001312 RID: 4882
	[Serializable]
	public class ItemRecordsSection
	{
	}

	// Token: 0x02001313 RID: 4883
	[Serializable]
	public class ItemCapsSection
	{
	}

	// Token: 0x02001314 RID: 4884
	[Serializable]
	public class SpaceLimitSection
	{
	}

	// Token: 0x02001315 RID: 4885
	[Serializable]
	public class ItemCap
	{
		// Token: 0x06009840 RID: 38976 RVA: 0x003C8A97 File Offset: 0x003C6C97
		[SerializeField]
		public ItemCap(vp_ItemType type, int cap)
		{
			this.Type = type;
			this.Cap = cap;
		}

		// Token: 0x0400748F RID: 29839
		[SerializeField]
		public vp_ItemType Type;

		// Token: 0x04007490 RID: 29840
		[SerializeField]
		public int Cap;
	}

	// Token: 0x02001316 RID: 4886
	public enum Mode
	{
		// Token: 0x04007492 RID: 29842
		Weight,
		// Token: 0x04007493 RID: 29843
		Volume
	}

	// Token: 0x02001317 RID: 4887
	[PublicizedFrom(EAccessModifier.Protected)]
	public struct StartItemRecord
	{
		// Token: 0x06009841 RID: 38977 RVA: 0x003C8AAD File Offset: 0x003C6CAD
		public StartItemRecord(vp_ItemType type, int id, int amount)
		{
			this.Type = type;
			this.ID = id;
			this.Amount = amount;
		}

		// Token: 0x04007494 RID: 29844
		public vp_ItemType Type;

		// Token: 0x04007495 RID: 29845
		public int ID;

		// Token: 0x04007496 RID: 29846
		public int Amount;
	}
}

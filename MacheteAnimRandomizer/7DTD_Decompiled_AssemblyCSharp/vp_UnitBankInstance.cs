using System;
using UnityEngine;

// Token: 0x0200131F RID: 4895
[Serializable]
public class vp_UnitBankInstance : vp_ItemInstance
{
	// Token: 0x17000F91 RID: 3985
	// (get) Token: 0x0600985C RID: 39004 RVA: 0x003C93EA File Offset: 0x003C75EA
	// (set) Token: 0x0600985D RID: 39005 RVA: 0x003C9416 File Offset: 0x003C7616
	public int Capacity
	{
		get
		{
			if (this.Type != null)
			{
				this.m_Capacity = ((vp_UnitBankType)this.Type).Capacity;
			}
			return this.m_Capacity;
		}
		set
		{
			this.m_Capacity = Mathf.Max(-1, value);
		}
	}

	// Token: 0x0600985E RID: 39006 RVA: 0x003C9425 File Offset: 0x003C7625
	[SerializeField]
	public vp_UnitBankInstance(vp_UnitBankType unitBankType, int id, vp_Inventory inventory) : base(unitBankType, id)
	{
		this.UnitType = unitBankType.Unit;
		this.m_Inventory = inventory;
	}

	// Token: 0x0600985F RID: 39007 RVA: 0x003C9449 File Offset: 0x003C7649
	[SerializeField]
	public vp_UnitBankInstance(vp_UnitType unitType, vp_Inventory inventory) : base(null, 0)
	{
		this.UnitType = unitType;
		this.m_Inventory = inventory;
	}

	// Token: 0x06009860 RID: 39008 RVA: 0x003C9468 File Offset: 0x003C7668
	public virtual bool TryRemoveUnits(int amount)
	{
		if (this.Count <= 0)
		{
			return false;
		}
		amount = Mathf.Max(0, amount);
		if (amount == 0)
		{
			return false;
		}
		this.Count = Mathf.Max(0, this.Count - amount);
		return true;
	}

	// Token: 0x06009861 RID: 39009 RVA: 0x003C9498 File Offset: 0x003C7698
	public virtual bool TryGiveUnits(int amount)
	{
		if (this.Type != null && !((vp_UnitBankType)this.Type).Reloadable)
		{
			return false;
		}
		if (this.Capacity != -1 && this.Count >= this.Capacity)
		{
			return false;
		}
		amount = Mathf.Max(0, amount);
		if (amount == 0)
		{
			return false;
		}
		this.Count += amount;
		if (this.Count <= this.Capacity)
		{
			return true;
		}
		if (this.Capacity == -1)
		{
			return true;
		}
		this.Count = this.Capacity;
		return true;
	}

	// Token: 0x17000F92 RID: 3986
	// (get) Token: 0x06009862 RID: 39010 RVA: 0x003C9524 File Offset: 0x003C7724
	public virtual bool IsInternal
	{
		get
		{
			return this.Type == null;
		}
	}

	// Token: 0x06009863 RID: 39011 RVA: 0x003C9534 File Offset: 0x003C7734
	public virtual bool DoAddUnits(int amount)
	{
		this.m_PrevCount = this.Count;
		this.m_Result = this.TryGiveUnits(amount);
		if (this.m_Inventory.SpaceEnabled && this.m_Result && !this.IsInternal && this.m_Inventory.SpaceMode == vp_Inventory.Mode.Weight)
		{
			this.m_Inventory.UsedSpace += (float)(this.Count - this.m_PrevCount) * this.UnitType.Space;
		}
		this.m_Inventory.SetDirty();
		return this.m_Result;
	}

	// Token: 0x06009864 RID: 39012 RVA: 0x003C95C4 File Offset: 0x003C77C4
	public virtual bool DoRemoveUnits(int amount)
	{
		this.m_PrevCount = this.Count;
		this.m_Result = this.TryRemoveUnits(amount);
		if (this.m_Inventory.SpaceEnabled && this.m_Result && !this.IsInternal && this.m_Inventory.SpaceMode == vp_Inventory.Mode.Weight)
		{
			this.m_Inventory.UsedSpace = Mathf.Max(0f, this.m_Inventory.UsedSpace - (float)(this.m_PrevCount - this.Count) * this.UnitType.Space);
		}
		this.m_Inventory.SetDirty();
		return this.m_Result;
	}

	// Token: 0x06009865 RID: 39013 RVA: 0x003C9660 File Offset: 0x003C7860
	public virtual int ClampToCapacity()
	{
		int count = this.Count;
		if (this.Capacity != -1)
		{
			this.Count = Mathf.Clamp(this.Count, 0, this.Capacity);
		}
		this.Count = Mathf.Max(0, this.Count);
		return count - this.Count;
	}

	// Token: 0x040074B5 RID: 29877
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int UNLIMITED = -1;

	// Token: 0x040074B6 RID: 29878
	[SerializeField]
	public vp_UnitType UnitType;

	// Token: 0x040074B7 RID: 29879
	[SerializeField]
	public int Count;

	// Token: 0x040074B8 RID: 29880
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public int m_Capacity = -1;

	// Token: 0x040074B9 RID: 29881
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Protected)]
	public vp_Inventory m_Inventory;

	// Token: 0x040074BA RID: 29882
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public bool m_Result;

	// Token: 0x040074BB RID: 29883
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public int m_PrevCount;
}

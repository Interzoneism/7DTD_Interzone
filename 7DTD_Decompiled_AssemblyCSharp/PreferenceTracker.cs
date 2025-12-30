using System;

// Token: 0x020004FC RID: 1276
public class PreferenceTracker
{
	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002994 RID: 10644 RVA: 0x0010F746 File Offset: 0x0010D946
	// (set) Token: 0x06002995 RID: 10645 RVA: 0x0010F74E File Offset: 0x0010D94E
	public int PlayerID { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x06002996 RID: 10646 RVA: 0x0010F757 File Offset: 0x0010D957
	// (set) Token: 0x06002997 RID: 10647 RVA: 0x0010F75F File Offset: 0x0010D95F
	public ItemStack[] toolbelt { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x06002998 RID: 10648 RVA: 0x0010F768 File Offset: 0x0010D968
	// (set) Token: 0x06002999 RID: 10649 RVA: 0x0010F770 File Offset: 0x0010D970
	public ItemStack[] bag { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x0600299A RID: 10650 RVA: 0x0010F779 File Offset: 0x0010D979
	// (set) Token: 0x0600299B RID: 10651 RVA: 0x0010F781 File Offset: 0x0010D981
	public ItemValue[] equipment { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x0600299C RID: 10652 RVA: 0x0010F78A File Offset: 0x0010D98A
	public bool AnyPreferences
	{
		get
		{
			return this.toolbelt != null || this.bag != null || this.equipment != null;
		}
	}

	// Token: 0x0600299D RID: 10653 RVA: 0x0010F7A7 File Offset: 0x0010D9A7
	public PreferenceTracker(int playerId)
	{
		this.PlayerID = playerId;
	}

	// Token: 0x0600299E RID: 10654 RVA: 0x0010F7B8 File Offset: 0x0010D9B8
	public void SetToolbelt(ItemStack[] _itemStacks, Predicate<ItemStack> _includeCondition)
	{
		if (_itemStacks == null || _itemStacks.Length == 0)
		{
			return;
		}
		this.toolbelt = new ItemStack[_itemStacks.Length];
		for (int i = 0; i < this.toolbelt.Length; i++)
		{
			if (_includeCondition(_itemStacks[i]))
			{
				this.toolbelt[i] = _itemStacks[i].Clone();
			}
			else
			{
				this.toolbelt[i] = new ItemStack();
			}
		}
	}

	// Token: 0x0600299F RID: 10655 RVA: 0x0010F818 File Offset: 0x0010DA18
	public void SetBag(ItemStack[] _itemStacks, Predicate<ItemStack> _includeCondition)
	{
		if (_itemStacks == null || _itemStacks.Length == 0)
		{
			return;
		}
		this.bag = new ItemStack[_itemStacks.Length];
		for (int i = 0; i < this.bag.Length; i++)
		{
			if (_includeCondition(_itemStacks[i]))
			{
				this.bag[i] = _itemStacks[i].Clone();
			}
			else
			{
				this.bag[i] = new ItemStack();
			}
		}
	}

	// Token: 0x060029A0 RID: 10656 RVA: 0x0010F878 File Offset: 0x0010DA78
	public void SetEquipment(ItemValue[] _itemValues, Predicate<ItemValue> _includeCondition)
	{
		if (_itemValues == null && _itemValues.Length != 0)
		{
			return;
		}
		this.equipment = new ItemValue[_itemValues.Length];
		for (int i = 0; i < this.equipment.Length; i++)
		{
			if (_includeCondition(_itemValues[i]))
			{
				this.equipment[i] = _itemValues[i].Clone();
			}
			else
			{
				this.equipment[i] = new ItemValue();
			}
		}
	}

	// Token: 0x060029A1 RID: 10657 RVA: 0x0010F8D8 File Offset: 0x0010DAD8
	public void Write(PooledBinaryWriter _bw)
	{
		_bw.Write(this.PlayerID);
		bool flag = this.toolbelt != null && this.toolbelt.Length != 0;
		_bw.Write(flag);
		if (flag)
		{
			GameUtils.WriteItemStack(_bw, this.toolbelt);
		}
		bool flag2 = this.equipment != null && this.equipment.Length != 0;
		_bw.Write(flag2);
		if (flag2)
		{
			GameUtils.WriteItemValueArray(_bw, this.equipment);
		}
		bool flag3 = this.bag != null && this.bag.Length != 0;
		_bw.Write(flag3);
		if (flag3)
		{
			GameUtils.WriteItemStack(_bw, this.bag);
		}
	}

	// Token: 0x060029A2 RID: 10658 RVA: 0x0010F978 File Offset: 0x0010DB78
	public void Read(PooledBinaryReader _br)
	{
		this.PlayerID = _br.ReadInt32();
		if (_br.ReadBoolean())
		{
			this.toolbelt = GameUtils.ReadItemStack(_br);
		}
		if (_br.ReadBoolean())
		{
			this.equipment = GameUtils.ReadItemValueArray(_br);
		}
		if (_br.ReadBoolean())
		{
			this.bag = GameUtils.ReadItemStack(_br);
		}
	}
}

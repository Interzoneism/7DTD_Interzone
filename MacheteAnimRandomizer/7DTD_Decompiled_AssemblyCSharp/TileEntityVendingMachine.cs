using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;

// Token: 0x02000B12 RID: 2834
public class TileEntityVendingMachine : TileEntityTrader, ILockable
{
	// Token: 0x060057F6 RID: 22518 RVA: 0x00239754 File Offset: 0x00237954
	public TileEntityVendingMachine(Chunk _chunk) : base(_chunk)
	{
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
		this.isLocked = true;
		this.ownerID = null;
		this.password = "";
		this.rentalEndTime = 0UL;
		this.rentalEndDay = 0;
		this.TraderData = new TraderData();
	}

	// Token: 0x060057F7 RID: 22519 RVA: 0x002397C4 File Offset: 0x002379C4
	public TileEntityVendingMachine.RentResult CanRent()
	{
		if (this.ownerID != null && !this.ownerID.Equals(PlatformManager.InternalLocalUserIdentifier))
		{
			return TileEntityVendingMachine.RentResult.AlreadyRented;
		}
		if (GameManager.Instance.World.GetPrimaryPlayer().PlayerUI.xui.PlayerInventory.CurrencyAmount < this.TraderData.TraderInfo.RentCost)
		{
			return TileEntityVendingMachine.RentResult.NotEnoughMoney;
		}
		if (this.checkAlreadyRentingVM())
		{
			return TileEntityVendingMachine.RentResult.AlreadyRentingVM;
		}
		return TileEntityVendingMachine.RentResult.Allowed;
	}

	// Token: 0x060057F8 RID: 22520 RVA: 0x00239830 File Offset: 0x00237A30
	[PublicizedFrom(EAccessModifier.Private)]
	public bool checkAlreadyRentingVM()
	{
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		Vector3i rentedVMPosition = primaryPlayer.RentedVMPosition;
		return !(rentedVMPosition == base.ToWorldPos()) && !(rentedVMPosition == Vector3i.zero) && primaryPlayer.RentalEndDay > GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
	}

	// Token: 0x060057F9 RID: 22521 RVA: 0x00239890 File Offset: 0x00237A90
	[PublicizedFrom(EAccessModifier.Private)]
	public TileEntityVendingMachine(TileEntityVendingMachine _other) : base(null)
	{
		this.allowedUserIds.AddRange(_other.allowedUserIds);
		this.isLocked = _other.isLocked;
		this.ownerID = _other.ownerID;
		this.password = _other.password;
		this.bUserAccessing = _other.bUserAccessing;
		this.rentalEndTime = _other.rentalEndTime;
		this.rentalEndDay = _other.rentalEndDay;
		this.TraderData = new TraderData(_other.TraderData);
		this.nextAutoBuy = _other.nextAutoBuy;
	}

	// Token: 0x060057FA RID: 22522 RVA: 0x00239937 File Offset: 0x00237B37
	public override TileEntity Clone()
	{
		return new TileEntityVendingMachine(this);
	}

	// Token: 0x060057FB RID: 22523 RVA: 0x0023993F File Offset: 0x00237B3F
	public bool IsLocked()
	{
		return this.isLocked;
	}

	// Token: 0x060057FC RID: 22524 RVA: 0x00239947 File Offset: 0x00237B47
	public void SetLocked(bool _isLocked)
	{
		this.isLocked = _isLocked;
		this.setModified();
	}

	// Token: 0x060057FD RID: 22525 RVA: 0x00239956 File Offset: 0x00237B56
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.ownerID = _userIdentifier;
		this.setModified();
	}

	// Token: 0x060057FE RID: 22526 RVA: 0x00239965 File Offset: 0x00237B65
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return (_userIdentifier != null && _userIdentifier.Equals(this.ownerID)) || this.allowedUserIds.Contains(_userIdentifier);
	}

	// Token: 0x060057FF RID: 22527 RVA: 0x00239989 File Offset: 0x00237B89
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x06005800 RID: 22528 RVA: 0x00239996 File Offset: 0x00237B96
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return _userIdentifier != null && _userIdentifier.Equals(this.ownerID);
	}

	// Token: 0x06005801 RID: 22529 RVA: 0x002399A9 File Offset: 0x00237BA9
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.ownerID;
	}

	// Token: 0x06005802 RID: 22530 RVA: 0x002399B1 File Offset: 0x00237BB1
	public bool HasPassword()
	{
		return !string.IsNullOrEmpty(this.password);
	}

	// Token: 0x06005803 RID: 22531 RVA: 0x002399C1 File Offset: 0x00237BC1
	public string GetPassword()
	{
		return this.password;
	}

	// Token: 0x06005804 RID: 22532 RVA: 0x002399CC File Offset: 0x00237BCC
	public bool CheckPassword(string _password, PlatformUserIdentifierAbs _userIdentifier, out bool changed)
	{
		changed = false;
		if (_userIdentifier != null && _userIdentifier.Equals(this.ownerID))
		{
			if (Utils.HashString(_password) != this.password)
			{
				changed = true;
				this.password = Utils.HashString(_password);
				this.allowedUserIds.Clear();
				this.setModified();
			}
			return true;
		}
		if (Utils.HashString(_password) == this.password)
		{
			this.allowedUserIds.Add(_userIdentifier);
			this.setModified();
			return true;
		}
		return false;
	}

	// Token: 0x06005805 RID: 22533 RVA: 0x00239A4C File Offset: 0x00237C4C
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		int num = _br.ReadInt32();
		this.isLocked = _br.ReadBoolean();
		this.ownerID = PlatformUserIdentifierAbs.FromStream(_br, false, false);
		this.password = _br.ReadString();
		this.allowedUserIds = new List<PlatformUserIdentifierAbs>();
		int num2 = _br.ReadInt32();
		for (int i = 0; i < num2; i++)
		{
			this.allowedUserIds.Add(PlatformUserIdentifierAbs.FromStream(_br, false, false));
		}
		if (num > 1)
		{
			this.rentalEndDay = _br.ReadInt32();
		}
		else
		{
			this.rentalEndTime = _br.ReadUInt64();
			this.rentalEndDay = GameUtils.WorldTimeToDays(this.rentalEndTime);
		}
		this.TraderData.Read(0, _br);
		if (this.TraderData.TraderInfo.Rentable)
		{
			this.nextAutoBuy = _br.ReadUInt64();
		}
		this.syncNeeded = false;
	}

	// Token: 0x06005806 RID: 22534 RVA: 0x00239B20 File Offset: 0x00237D20
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(2);
		_bw.Write(this.isLocked);
		this.ownerID.ToStream(_bw, false);
		_bw.Write(this.password);
		_bw.Write(this.allowedUserIds.Count);
		for (int i = 0; i < this.allowedUserIds.Count; i++)
		{
			this.allowedUserIds[i].ToStream(_bw, false);
		}
		_bw.Write(this.rentalEndDay);
		this.TraderData.Write(_bw);
		if (this.TraderData.TraderInfo.Rentable)
		{
			_bw.Write(this.nextAutoBuy);
		}
	}

	// Token: 0x06005807 RID: 22535 RVA: 0x00239BD4 File Offset: 0x00237DD4
	public override void UpgradeDowngradeFrom(TileEntity _other)
	{
		base.UpgradeDowngradeFrom(_other);
		if (_other is ILockable)
		{
			ILockable lockable = _other as ILockable;
			base.EntityId = lockable.EntityId;
			this.SetLocked(lockable.IsLocked());
			this.SetOwner(lockable.GetOwner());
			this.allowedUserIds = new List<PlatformUserIdentifierAbs>(lockable.GetUsers());
			this.password = lockable.GetPassword();
			this.setModified();
		}
	}

	// Token: 0x06005808 RID: 22536 RVA: 0x00239C3E File Offset: 0x00237E3E
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return this.allowedUserIds;
	}

	// Token: 0x06005809 RID: 22537 RVA: 0x000583BD File Offset: 0x000565BD
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.VendingMachine;
	}

	// Token: 0x0600580A RID: 22538 RVA: 0x00239C46 File Offset: 0x00237E46
	[PublicizedFrom(EAccessModifier.Internal)]
	public void ClearOwner()
	{
		this.ownerID = null;
	}

	// Token: 0x170008CA RID: 2250
	// (get) Token: 0x0600580B RID: 22539 RVA: 0x00239C4F File Offset: 0x00237E4F
	public bool IsRentable
	{
		get
		{
			return this.TraderData.TraderInfo.Rentable;
		}
	}

	// Token: 0x170008CB RID: 2251
	// (get) Token: 0x0600580C RID: 22540 RVA: 0x00239C61 File Offset: 0x00237E61
	public float RentTimeRemaining
	{
		get
		{
			return (float)(this.rentalEndDay - GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime));
		}
	}

	// Token: 0x170008CC RID: 2252
	// (get) Token: 0x0600580D RID: 22541 RVA: 0x00239C7F File Offset: 0x00237E7F
	public int RentalEndDay
	{
		get
		{
			return this.rentalEndDay;
		}
	}

	// Token: 0x0600580E RID: 22542 RVA: 0x00239C88 File Offset: 0x00237E88
	public bool Rent()
	{
		if ((this.ownerID != null && !this.ownerID.Equals(PlatformManager.InternalLocalUserIdentifier)) || !this.TraderData.TraderInfo.Rentable)
		{
			return false;
		}
		XUi xui = GameManager.Instance.World.GetPrimaryPlayer().PlayerUI.xui;
		if (xui.PlayerInventory.CurrencyAmount >= this.TraderData.TraderInfo.RentCost)
		{
			ItemStack itemStack = new ItemStack(ItemClass.GetItem(TraderInfo.CurrencyItem, false), this.TraderData.TraderInfo.RentCost);
			xui.PlayerInventory.RemoveItem(itemStack);
			if (this.ownerID == null)
			{
				this.ownerID = PlatformManager.InternalLocalUserIdentifier;
				this.rentalEndDay = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime) + 30;
				this.SetAutoBuyTime(true);
			}
			else
			{
				this.rentalEndDay += 30;
			}
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			primaryPlayer.RentedVMPosition = base.ToWorldPos();
			primaryPlayer.RentalEndDay = this.rentalEndDay;
			this.setModified();
			return true;
		}
		return false;
	}

	// Token: 0x0600580F RID: 22543 RVA: 0x00239DA4 File Offset: 0x00237FA4
	public void ClearVendingMachine()
	{
		this.TraderData.AvailableMoney = 0;
		this.TraderData.PrimaryInventory.Clear();
		this.ownerID = null;
		this.allowedUserIds.Clear();
		this.rentalEndTime = 0UL;
		this.password = "";
		this.setModified();
	}

	// Token: 0x06005810 RID: 22544 RVA: 0x00239DF8 File Offset: 0x00237FF8
	public bool TryAutoBuy(bool isInitial = true)
	{
		if (this.nextAutoBuy == 0UL)
		{
			this.SetAutoBuyTime(true);
		}
		if (GameManager.Instance.World.worldTime > this.nextAutoBuy)
		{
			GameRandom random = GameManager.Instance.lootManager.Random;
			if (random.RandomFloat < this.autoBuyThreshold && this.TraderData.PrimaryInventory.Count > this.minimumAutoBuyCount)
			{
				int num = random.RandomRange(1, Mathf.Max(this.TraderData.PrimaryInventory.Count / 10, 1));
				Log.Warning("Items Purchased: " + num.ToString());
				for (int i = 0; i < num; i++)
				{
					int num2 = 0;
					for (int j = 0; j < this.TraderData.PrimaryInventory.Count; j++)
					{
						if (this.TraderData.GetMarkupByIndex(j) <= 0)
						{
							ItemStack itemStack = this.TraderData.PrimaryInventory[j];
							if ((itemStack.itemValue.ItemClass.IsBlock() ? Block.list[itemStack.itemValue.type].EconomicValue : itemStack.itemValue.ItemClass.EconomicValue) > 0f && itemStack.itemValue.ItemClass.SellableToTrader)
							{
								num2++;
							}
						}
					}
					if (num2 > 0)
					{
						int num3 = random.RandomRange(num2);
						num2 = 0;
						for (int k = 0; k < this.TraderData.PrimaryInventory.Count; k++)
						{
							if (this.TraderData.GetMarkupByIndex(k) <= 0)
							{
								ItemStack itemStack2 = this.TraderData.PrimaryInventory[k];
								if ((itemStack2.itemValue.ItemClass.IsBlock() ? Block.list[itemStack2.itemValue.type].EconomicValue : itemStack2.itemValue.ItemClass.EconomicValue) > 0f && itemStack2.itemValue.ItemClass.SellableToTrader)
								{
									if (num2 == num3)
									{
										int count = itemStack2.count;
										int buyPrice = XUiM_Trader.GetBuyPrice(LocalPlayerUI.GetUIForPrimaryPlayer().xui, itemStack2.itemValue, count, null, k);
										this.TraderData.PrimaryInventory.RemoveAt(k);
										this.TraderData.RemoveMarkup(k);
										this.TraderData.AvailableMoney += buyPrice;
										break;
									}
									num2++;
								}
							}
						}
					}
				}
				this.autoBuyThreshold = this.autoBuyThresholdStep;
			}
			else
			{
				this.autoBuyThreshold += this.autoBuyThresholdStep;
			}
			this.SetAutoBuyTime(false);
			return this.TryAutoBuy(false);
		}
		return !isInitial;
	}

	// Token: 0x06005811 RID: 22545 RVA: 0x0023A0A8 File Offset: 0x002382A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetAutoBuyTime(bool isInitial)
	{
		uint num = 24000U;
		if (isInitial)
		{
			this.nextAutoBuy = GameManager.Instance.World.worldTime + (ulong)num;
			return;
		}
		this.nextAutoBuy += (ulong)num;
	}

	// Token: 0x06005812 RID: 22546 RVA: 0x0023A0E8 File Offset: 0x002382E8
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		if (!this.TraderData.TraderInfo.PlayerOwned && this.TraderData.TraderInfo.Rentable && this.ownerID != null && this.rentalEndDay <= GameUtils.WorldTimeToDays(world.worldTime))
		{
			this.ClearVendingMachine();
		}
	}

	// Token: 0x0400436E RID: 17262
	[PublicizedFrom(EAccessModifier.Private)]
	public new const int ver = 2;

	// Token: 0x0400436F RID: 17263
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLocked;

	// Token: 0x04004370 RID: 17264
	[PublicizedFrom(EAccessModifier.Private)]
	public PlatformUserIdentifierAbs ownerID;

	// Token: 0x04004371 RID: 17265
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PlatformUserIdentifierAbs> allowedUserIds;

	// Token: 0x04004372 RID: 17266
	[PublicizedFrom(EAccessModifier.Private)]
	public string password;

	// Token: 0x04004373 RID: 17267
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong rentalEndTime;

	// Token: 0x04004374 RID: 17268
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong nextAutoBuy;

	// Token: 0x04004375 RID: 17269
	[PublicizedFrom(EAccessModifier.Private)]
	public int rentalEndDay;

	// Token: 0x04004376 RID: 17270
	[PublicizedFrom(EAccessModifier.Private)]
	public float autoBuyThresholdStep = 0.33333334f;

	// Token: 0x04004377 RID: 17271
	[PublicizedFrom(EAccessModifier.Private)]
	public float autoBuyThreshold = 0.33333334f;

	// Token: 0x04004378 RID: 17272
	[PublicizedFrom(EAccessModifier.Private)]
	public int minimumAutoBuyCount = 5;

	// Token: 0x02000B13 RID: 2835
	public enum RentResult
	{
		// Token: 0x0400437A RID: 17274
		Allowed,
		// Token: 0x0400437B RID: 17275
		AlreadyRented,
		// Token: 0x0400437C RID: 17276
		AlreadyRentingVM,
		// Token: 0x0400437D RID: 17277
		NotEnoughMoney
	}
}

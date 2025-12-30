using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Platform;

// Token: 0x02000F40 RID: 3904
public class EntitlementManager
{
	// Token: 0x17000D05 RID: 3333
	// (get) Token: 0x06007C34 RID: 31796 RVA: 0x003248BC File Offset: 0x00322ABC
	public static EntitlementManager Instance
	{
		get
		{
			if (EntitlementManager._instance == null)
			{
				EntitlementManager._instance = new EntitlementManager();
				if (PlatformManager.MultiPlatform.EntitlementValidators != null)
				{
					PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _)
					{
						object obj = EntitlementManager.lockObj;
						lock (obj)
						{
							EntitlementManager._instance.entitlementValidators = PlatformManager.MultiPlatform.EntitlementValidators;
						}
					};
				}
			}
			return EntitlementManager._instance;
		}
	}

	// Token: 0x06007C35 RID: 31797 RVA: 0x0032491C File Offset: 0x00322B1C
	public bool HasEntitlement(object _addressableKey)
	{
		EntitlementSetEnum entitlementSet = this.GetEntitlementSet(_addressableKey);
		return this.HasEntitlement(entitlementSet);
	}

	// Token: 0x06007C36 RID: 31798 RVA: 0x00324938 File Offset: 0x00322B38
	public bool HasEntitlement(EntitlementSetEnum _set)
	{
		ValueTuple<bool, bool> valueTuple = this.CheckOverride(_set);
		bool item = valueTuple.Item1;
		bool item2 = valueTuple.Item2;
		if (item)
		{
			return item2;
		}
		if (_set == EntitlementSetEnum.None)
		{
			return true;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HasEntitlement(_set))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06007C37 RID: 31799 RVA: 0x003249D8 File Offset: 0x00322BD8
	public bool IsAvailableOnPlatform(object _addressableKey)
	{
		EntitlementSetEnum setForAsset = this.GetSetForAsset(_addressableKey);
		return this.IsAvailableOnPlatform(setForAsset);
	}

	// Token: 0x06007C38 RID: 31800 RVA: 0x003249F4 File Offset: 0x00322BF4
	public bool IsAvailableOnPlatform(EntitlementSetEnum _set)
	{
		ValueTuple<bool, bool> valueTuple = this.CheckOverride(_set);
		bool item = valueTuple.Item1;
		bool item2 = valueTuple.Item2;
		if (item)
		{
			return item2;
		}
		if (_set == EntitlementSetEnum.None)
		{
			return true;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsAvailableOnPlatform(_set))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06007C39 RID: 31801 RVA: 0x00324A94 File Offset: 0x00322C94
	public unsafe EntitlementSetEnum GetSetForAsset(object addressableKey)
	{
		string text = addressableKey as string;
		if (text == null)
		{
			EntitlementSetEnum result;
			if (EntitlementAddressablesMaps.AddressablesKeyMap.TryGetValue(addressableKey, out result))
			{
				return result;
			}
			return EntitlementSetEnum.None;
		}
		else
		{
			if (string.IsNullOrEmpty(text))
			{
				return EntitlementSetEnum.None;
			}
			StringSpan key = text.ToLowerInvariant().Trim();
			if (key.Length >= 2 && *key[0] == 64 && *key[1] == 58)
			{
				key = key.Slice(2);
			}
			while (key.Length > 0)
			{
				EntitlementSetEnum result2;
				if (EntitlementAddressablesMaps.AddressablesStringMap.TryGetValue(key, out result2))
				{
					return result2;
				}
				int num = key.LastIndexOf('/');
				if (num == -1)
				{
					break;
				}
				key = key.Slice(0, num);
			}
			return EntitlementSetEnum.None;
		}
	}

	// Token: 0x06007C3A RID: 31802 RVA: 0x00324B44 File Offset: 0x00322D44
	public bool IsEntitlementPurchasable(object _addressableKey)
	{
		EntitlementSetEnum entitlementSet = this.GetEntitlementSet(_addressableKey);
		return this.IsEntitlementPurchasable(entitlementSet);
	}

	// Token: 0x06007C3B RID: 31803 RVA: 0x00324B60 File Offset: 0x00322D60
	public bool IsEntitlementPurchasable(EntitlementSetEnum _set)
	{
		if (_set == EntitlementSetEnum.None)
		{
			return true;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsEntitlementPurchasable(_set))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06007C3C RID: 31804 RVA: 0x00324BE0 File Offset: 0x00322DE0
	public unsafe EntitlementSetEnum GetEntitlementSet(object addressableKey)
	{
		string text = addressableKey as string;
		if (text == null)
		{
			EntitlementSetEnum result;
			if (EntitlementAddressablesMaps.AddressablesKeyMap.TryGetValue(addressableKey, out result))
			{
				return result;
			}
			return EntitlementSetEnum.None;
		}
		else
		{
			if (string.IsNullOrEmpty(text))
			{
				return EntitlementSetEnum.None;
			}
			StringSpan key = text.ToLowerInvariant().Trim();
			if (key.Length >= 2 && *key[0] == 64 && *key[1] == 58)
			{
				key = key.Slice(2);
			}
			while (key.Length > 0)
			{
				EntitlementSetEnum result2;
				if (EntitlementAddressablesMaps.AddressablesStringMap.TryGetValue(key, out result2))
				{
					return result2;
				}
				int num = key.LastIndexOf('/');
				if (num == -1)
				{
					break;
				}
				key = key.Slice(0, num);
			}
			return EntitlementSetEnum.None;
		}
	}

	// Token: 0x06007C3D RID: 31805 RVA: 0x00324C90 File Offset: 0x00322E90
	public void OpenStore(EntitlementSetEnum _set, Action<EntitlementSetEnum> _onPurchased)
	{
		if (_set == EntitlementSetEnum.None)
		{
			return;
		}
		object obj = EntitlementManager.lockObj;
		lock (obj)
		{
			using (IEnumerator<IEntitlementValidator> enumerator = this.entitlementValidators.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.OpenStore(_set, _onPurchased))
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06007C3E RID: 31806 RVA: 0x00324D0C File Offset: 0x00322F0C
	[return: TupleElementNames(new string[]
	{
		"hasOverride",
		"overrideValue"
	})]
	public ValueTuple<bool, bool> CheckOverride(EntitlementSetEnum _set)
	{
		return new ValueTuple<bool, bool>(false, false);
	}

	// Token: 0x04005F12 RID: 24338
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object lockObj = new object();

	// Token: 0x04005F13 RID: 24339
	[PublicizedFrom(EAccessModifier.Private)]
	public static EntitlementManager _instance;

	// Token: 0x04005F14 RID: 24340
	[PublicizedFrom(EAccessModifier.Private)]
	public IList<IEntitlementValidator> entitlementValidators = new List<IEntitlementValidator>();
}

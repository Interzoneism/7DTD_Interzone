using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200047F RID: 1151
public class PlayerEquippedSlots : MonoBehaviour
{
	// Token: 0x06002580 RID: 9600 RVA: 0x000F29A0 File Offset: 0x000F0BA0
	public void Init(Transform outfit)
	{
		this.outfitXF = outfit;
		if (this.outfitXF != null)
		{
			this._DisableAllNASubmeshes();
		}
	}

	// Token: 0x06002581 RID: 9601 RVA: 0x000F29C0 File Offset: 0x000F0BC0
	public void ListParts()
	{
		int count = this.parts.Count;
		for (int i = 0; i < count; i++)
		{
			Log.Warning(this.parts[i].name);
		}
	}

	// Token: 0x06002582 RID: 9602 RVA: 0x000F29FC File Offset: 0x000F0BFC
	public void ListEquipment()
	{
		int count = this.equippedParts.Count;
		for (int i = 0; i < count; i++)
		{
			Log.Warning(this.equippedParts[i].name);
		}
	}

	// Token: 0x06002583 RID: 9603 RVA: 0x000F2A38 File Offset: 0x000F0C38
	public bool IsEquipped(string partName)
	{
		int count = this.equippedParts.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.equippedParts[i].name == partName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002584 RID: 9604 RVA: 0x000F2A7C File Offset: 0x000F0C7C
	public bool Equip(string partName)
	{
		if (this.IsEquipped(partName))
		{
			return false;
		}
		int count = this.parts.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerEquippedSlots.PartInfo partInfo = this.parts[i];
			if (partInfo.name == partName)
			{
				PlayerEquippedSlots.EquippedPart equippedPart = new PlayerEquippedSlots.EquippedPart();
				equippedPart.name = partName;
				equippedPart.partInfo = partInfo;
				this.equippedParts.Add(equippedPart);
				this._RunRules();
				return true;
			}
		}
		Log.Warning("Part '{0}' not equipped.", new object[]
		{
			partName
		});
		return false;
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x000F2B04 File Offset: 0x000F0D04
	public bool UnEquip(string partName)
	{
		int count = this.equippedParts.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.equippedParts[i].name == partName)
			{
				this._EnableNASubmesh(partName, false);
				this.equippedParts.RemoveAt(i);
				this._RunRules();
				return true;
			}
		}
		Log.Warning("Part '{0}' not unequipped.", new object[]
		{
			partName
		});
		return false;
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x000F2B74 File Offset: 0x000F0D74
	[PublicizedFrom(EAccessModifier.Private)]
	public void _RunRules()
	{
		int count = this.equippedParts.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerEquippedSlots.EquippedPart equippedPart = this.equippedParts[i];
			equippedPart.wasShowing = equippedPart.isShowing;
			equippedPart.isShowing = true;
		}
		for (int j = 0; j < count; j++)
		{
			PlayerEquippedSlots.EquippedPart equippedPart2 = this.equippedParts[j];
			if (equippedPart2.isShowing)
			{
				PlayerEquippedSlots.PartInfo partInfo = equippedPart2.partInfo;
				if (!string.IsNullOrEmpty(partInfo.rule))
				{
					string rule = partInfo.rule;
					for (int k = 0; k < count; k++)
					{
						if (k != j)
						{
							PlayerEquippedSlots.EquippedPart equippedPart3 = this.equippedParts[k];
							if (equippedPart3.isShowing)
							{
								PlayerEquippedSlots.PartInfo partInfo2 = equippedPart3.partInfo;
								if (partInfo2.IsInSlot(rule))
								{
									Log.Warning(" Note: Part {0} hides part {1} with rule {2}.", new object[]
									{
										partInfo.name,
										partInfo2.name,
										rule
									});
									equippedPart3.isShowing = false;
								}
							}
						}
					}
				}
			}
		}
		for (int l = 0; l < count; l++)
		{
			PlayerEquippedSlots.EquippedPart equippedPart4 = this.equippedParts[l];
			if (equippedPart4.isShowing && !equippedPart4.wasShowing)
			{
				this._EnableNASubmesh(equippedPart4.partInfo.name, true);
			}
			else if (!equippedPart4.isShowing && equippedPart4.wasShowing)
			{
				this._EnableNASubmesh(equippedPart4.partInfo.name, false);
			}
		}
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x000F2CDD File Offset: 0x000F0EDD
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform _GetNAOutfit()
	{
		return this.outfitXF;
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x000F2CE8 File Offset: 0x000F0EE8
	public void _EnableNASubmesh(string submeshName, bool enable)
	{
		Transform transform = this._GetNAOutfit();
		if (transform == null)
		{
			return;
		}
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(child.name == "Origin"))
			{
				GameObject gameObject = child.gameObject;
				if (gameObject.name == submeshName)
				{
					gameObject.SetActive(enable);
				}
			}
		}
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x000F2D54 File Offset: 0x000F0F54
	[PublicizedFrom(EAccessModifier.Private)]
	public void _DisableAllNASubmeshes()
	{
		Transform transform = this._GetNAOutfit();
		if (transform == null)
		{
			return;
		}
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!(child.name == "Origin"))
			{
				child.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x04001C81 RID: 7297
	public List<PlayerEquippedSlots.PartInfo> parts;

	// Token: 0x04001C82 RID: 7298
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<PlayerEquippedSlots.EquippedPart> equippedParts = new List<PlayerEquippedSlots.EquippedPart>();

	// Token: 0x04001C83 RID: 7299
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform outfitXF;

	// Token: 0x02000480 RID: 1152
	[Serializable]
	public class PartInfo
	{
		// Token: 0x0600258B RID: 9611 RVA: 0x000F2DBE File Offset: 0x000F0FBE
		public bool IsInSlot(string slotReference)
		{
			return PlayerEquippedSlots.PartInfo.RefMatchesSlot(slotReference, this.slot);
		}

		// Token: 0x0600258C RID: 9612 RVA: 0x000F2DCC File Offset: 0x000F0FCC
		public static bool RefMatchesSlot(string slotReference, string slotName)
		{
			int num = slotReference.IndexOf("*");
			if (num == -1)
			{
				return slotReference.Equals(slotName);
			}
			return string.Compare(slotReference, 0, slotName, 0, num) == 0;
		}

		// Token: 0x04001C84 RID: 7300
		public string name;

		// Token: 0x04001C85 RID: 7301
		public string slot;

		// Token: 0x04001C86 RID: 7302
		public string rule;
	}

	// Token: 0x02000481 RID: 1153
	[PublicizedFrom(EAccessModifier.Private)]
	public class EquippedPart
	{
		// Token: 0x04001C87 RID: 7303
		public string name;

		// Token: 0x04001C88 RID: 7304
		public PlayerEquippedSlots.PartInfo partInfo;

		// Token: 0x04001C89 RID: 7305
		public bool wasShowing;

		// Token: 0x04001C8A RID: 7306
		public bool isShowing;
	}
}

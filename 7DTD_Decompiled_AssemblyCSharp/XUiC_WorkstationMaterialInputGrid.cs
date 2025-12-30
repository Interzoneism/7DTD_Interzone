using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EB0 RID: 3760
[Preserve]
public class XUiC_WorkstationMaterialInputGrid : XUiC_WorkstationInputGrid
{
	// Token: 0x060076F4 RID: 30452 RVA: 0x0030719D File Offset: 0x0030539D
	public override void Init()
	{
		base.Init();
		this.materialWindow = this.windowGroup.Controller.GetChildByType<XUiC_WorkstationMaterialInputWindow>();
	}

	// Token: 0x060076F5 RID: 30453 RVA: 0x003071BC File Offset: 0x003053BC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		int num = 0;
		while (num < this.workstationData.TileEntity.Input.Length && num < this.itemControllers.Length)
		{
			float timerForSlot = this.workstationData.TileEntity.GetTimerForSlot(num);
			if (timerForSlot > 0f)
			{
				this.itemControllers[num].timer.IsVisible = true;
				this.itemControllers[num].timer.Text = string.Format("{0}:{1}", Mathf.Floor((timerForSlot + 0.95f) / 60f).ToCultureInvariantString("00"), Mathf.Floor((timerForSlot + 0.95f) % 60f).ToCultureInvariantString("00"));
			}
			else
			{
				this.itemControllers[num].timer.IsVisible = false;
			}
			num++;
		}
		this.workstationData.GetIsBurning();
	}

	// Token: 0x060076F6 RID: 30454 RVA: 0x00299052 File Offset: 0x00297252
	public override ItemStack[] GetSlots()
	{
		return this.getUISlots();
	}

	// Token: 0x060076F7 RID: 30455 RVA: 0x0027CA52 File Offset: 0x0027AC52
	[PublicizedFrom(EAccessModifier.Protected)]
	public override ItemStack[] getUISlots()
	{
		return this.items;
	}

	// Token: 0x060076F8 RID: 30456 RVA: 0x003072A0 File Offset: 0x003054A0
	public override bool HasRequirement(Recipe recipe)
	{
		return this.materialWindow != null && this.materialWindow.HasRequirement(recipe);
	}

	// Token: 0x060076F9 RID: 30457 RVA: 0x003072B8 File Offset: 0x003054B8
	public override void SetSlots(ItemStack[] stackList)
	{
		this.items = stackList;
		base.SetSlots(this.items);
		this.materialWindow.SetMaterialWeights(this.items);
	}

	// Token: 0x060076FA RID: 30458 RVA: 0x003072E0 File Offset: 0x003054E0
	public void SetWeight(ItemValue iv, int count)
	{
		ItemClass itemClass = iv.ItemClass;
		if (itemClass == null)
		{
			return;
		}
		string forgeCategory = itemClass.MadeOfMaterial.ForgeCategory;
		if (forgeCategory == null)
		{
			return;
		}
		for (int i = 3; i < this.items.Length; i++)
		{
			ItemClass itemClass2 = this.items[i].itemValue.ItemClass;
			if (itemClass2 == null)
			{
				if (this.materialWindow.MaterialNames[i - 3].EqualsCaseInsensitive(forgeCategory))
				{
					ItemStack itemStack = new ItemStack(iv, count);
					this.items[i] = itemStack;
					break;
				}
			}
			else if (itemClass2.MadeOfMaterial.ForgeCategory.EqualsCaseInsensitive(forgeCategory))
			{
				ItemStack itemStack2 = this.items[i].Clone();
				itemStack2.count += count;
				if (iv.ItemClass.Stacknumber.Value < itemStack2.count)
				{
					itemStack2.count = iv.ItemClass.Stacknumber.Value;
				}
				this.items[i] = itemStack2;
				break;
			}
		}
		this.materialWindow.SetMaterialWeights(this.items);
		this.UpdateBackend(this.items);
	}

	// Token: 0x060076FB RID: 30459 RVA: 0x003073F0 File Offset: 0x003055F0
	public int GetWeight(string materialName)
	{
		int result = 0;
		if (materialName == null)
		{
			return result;
		}
		for (int i = 3; i < this.items.Length; i++)
		{
			ItemClass itemClass = this.items[i].itemValue.ItemClass;
			if (itemClass != null && itemClass.MadeOfMaterial.ForgeCategory.EqualsCaseInsensitive(materialName))
			{
				result = this.items[i].count;
				break;
			}
		}
		return result;
	}

	// Token: 0x04005AA7 RID: 23207
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationMaterialInputWindow materialWindow;
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200168F RID: 5775
	[Preserve]
	public class ActionRemoveVehicles : ActionRemoveEntities
	{
		// Token: 0x0600AFF7 RID: 45047 RVA: 0x0044BD4C File Offset: 0x00449F4C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleRemoveData(GameEventManager gm, Entity ent)
		{
			if (ent is EntityVehicle)
			{
				EntityVehicle entityVehicle = ent as EntityVehicle;
				for (int i = 0; i < entityVehicle.GetAttachMaxCount(); i++)
				{
					EntityPlayer entityPlayer = entityVehicle.GetAttached(i) as EntityPlayer;
					if (entityPlayer != null)
					{
						if (entityPlayer.isEntityRemote)
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityPlayer.entityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
						}
						else
						{
							(entityPlayer as EntityPlayerLocal).PlayerUI.windowManager.CloseAllOpenWindows(null, false);
						}
						entityPlayer.SendDetach();
					}
				}
				List<ItemStack> list = new List<ItemStack>();
				ActionRemoveVehicles.ReturnVehicleTypes returnVehicleTypes = this.includeVehicle;
				if (returnVehicleTypes != ActionRemoveVehicles.ReturnVehicleTypes.Vehicle)
				{
					if (returnVehicleTypes == ActionRemoveVehicles.ReturnVehicleTypes.Parts)
					{
						ItemStack[] items = entityVehicle.GetVehicle().GetItems();
						for (int j = 0; j < items[0].itemValue.CosmeticMods.Length; j++)
						{
							ItemValue itemValue = items[0].itemValue.CosmeticMods[j];
							if (itemValue != null && !itemValue.IsEmpty())
							{
								list.Add(new ItemStack(itemValue.Clone(), 1));
							}
						}
						for (int k = 0; k < items[0].itemValue.Modifications.Length; k++)
						{
							ItemValue itemValue2 = items[0].itemValue.Modifications[k];
							if (itemValue2 != null && !itemValue2.IsEmpty())
							{
								list.Add(new ItemStack(itemValue2.Clone(), 1));
							}
						}
						if (entityVehicle.GetVehicle().RecipeName != null)
						{
							Recipe recipe = CraftingManager.GetRecipe(entityVehicle.GetVehicle().RecipeName);
							if (recipe != null)
							{
								for (int l = 0; l < recipe.ingredients.Count; l++)
								{
									ItemStack itemStack = recipe.ingredients[l].Clone();
									if (itemStack.itemValue.HasQuality)
									{
										itemStack.itemValue.Quality = 1;
									}
									list.Add(itemStack);
								}
							}
						}
					}
				}
				else
				{
					list.Add(new ItemStack(entityVehicle.GetVehicle().GetUpdatedItemValue(), 1));
				}
				if (this.returnFuel)
				{
					int fuelCount = entityVehicle.GetFuelCount();
					if (fuelCount > 0)
					{
						list.Add(new ItemStack(ItemClass.GetItem(entityVehicle.vehicle.GetFuelItem(), false), fuelCount));
					}
				}
				if (this.returnItems)
				{
					ItemStack[] slots = entityVehicle.inventory.GetSlots();
					for (int m = 0; m < slots.Length; m++)
					{
						if (!slots[m].IsEmpty())
						{
							list.Add(slots[m]);
						}
					}
					slots = entityVehicle.bag.GetSlots();
					for (int n = 0; n < slots.Length; n++)
					{
						if (!slots[n].IsEmpty())
						{
							list.Add(slots[n]);
						}
					}
				}
				if (list.Count > 0)
				{
					EntityLootContainer entityLootContainer = EntityFactory.CreateEntity("DroppedLootContainerTwitch".GetHashCode(), ent.position, Vector3.zero) as EntityLootContainer;
					if (entityLootContainer != null)
					{
						entityLootContainer.SetContent(ItemStack.Clone(list));
					}
					GameManager.Instance.World.SpawnEntityInWorld(entityLootContainer);
					return;
				}
			}
			else if (ent is EntityTurret)
			{
				EntityTurret entityTurret = ent as EntityTurret;
				for (int num = 0; num < entityTurret.GetAttachMaxCount(); num++)
				{
					EntityPlayer entityPlayer2 = entityTurret.GetAttached(num) as EntityPlayer;
					if (entityPlayer2 != null)
					{
						if (entityPlayer2.isEntityRemote)
						{
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityPlayer2.entityId), false, entityPlayer2.entityId, -1, -1, null, 192, false);
						}
						else
						{
							(entityPlayer2 as EntityPlayerLocal).PlayerUI.windowManager.CloseAllOpenWindows(null, false);
						}
						entityPlayer2.SendDetach();
					}
				}
				if (this.returnItems)
				{
					List<ItemStack> list2 = new List<ItemStack>();
					ItemStack[] slots2 = entityTurret.inventory.GetSlots();
					for (int num2 = 0; num2 < slots2.Length; num2++)
					{
						if (!slots2[num2].IsEmpty())
						{
							list2.Add(slots2[num2]);
						}
					}
					slots2 = entityTurret.bag.GetSlots();
					for (int num3 = 0; num3 < slots2.Length; num3++)
					{
						if (!slots2[num3].IsEmpty())
						{
							list2.Add(slots2[num3]);
						}
					}
					if (this.includeVehicle == ActionRemoveVehicles.ReturnVehicleTypes.Vehicle)
					{
						list2.Add(new ItemStack(entityTurret.OriginalItemValue, 1));
					}
					if (list2.Count > 0)
					{
						EntityLootContainer entityLootContainer2 = EntityFactory.CreateEntity("DroppedLootContainerTwitch".GetHashCode(), ent.position, Vector3.zero) as EntityLootContainer;
						if (entityLootContainer2 != null)
						{
							entityLootContainer2.SetContent(ItemStack.Clone(list2));
						}
						GameManager.Instance.World.SpawnEntityInWorld(entityLootContainer2);
					}
				}
			}
		}

		// Token: 0x0600AFF8 RID: 45048 RVA: 0x0044C1EA File Offset: 0x0044A3EA
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionRemoveVehicles.PropReturnItems, ref this.returnItems);
			properties.ParseBool(ActionRemoveVehicles.PropReturnFuel, ref this.returnFuel);
			properties.ParseEnum<ActionRemoveVehicles.ReturnVehicleTypes>(ActionRemoveVehicles.PropIncludeVehicle, ref this.includeVehicle);
		}

		// Token: 0x0600AFF9 RID: 45049 RVA: 0x0044C226 File Offset: 0x0044A426
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRemoveVehicles
			{
				targetGroup = this.targetGroup,
				returnItems = this.returnItems,
				includeVehicle = this.includeVehicle,
				returnFuel = this.returnFuel
			};
		}

		// Token: 0x0400896B RID: 35179
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool returnItems = true;

		// Token: 0x0400896C RID: 35180
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool returnFuel;

		// Token: 0x0400896D RID: 35181
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionRemoveVehicles.ReturnVehicleTypes includeVehicle;

		// Token: 0x0400896E RID: 35182
		public static string PropReturnItems = "return_items";

		// Token: 0x0400896F RID: 35183
		public static string PropReturnFuel = "return_fuel";

		// Token: 0x04008970 RID: 35184
		public static string PropIncludeVehicle = "return_vehicle_type";

		// Token: 0x02001690 RID: 5776
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum ReturnVehicleTypes
		{
			// Token: 0x04008972 RID: 35186
			None,
			// Token: 0x04008973 RID: 35187
			Vehicle,
			// Token: 0x04008974 RID: 35188
			Parts
		}
	}
}

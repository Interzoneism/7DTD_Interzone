using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016D5 RID: 5845
	[Preserve]
	public class ActionEmptyContainers : ActionBaseContainersAction
	{
		// Token: 0x0600B148 RID: 45384 RVA: 0x004521B8 File Offset: 0x004503B8
		public override bool CheckValidTileEntity(TileEntity te, out bool isEmpty)
		{
			TileEntityType tileEntityType = te.GetTileEntityType();
			isEmpty = true;
			if (tileEntityType <= TileEntityType.SecureLoot)
			{
				if (tileEntityType != TileEntityType.Loot && tileEntityType != TileEntityType.SecureLoot)
				{
					return false;
				}
			}
			else if (tileEntityType != TileEntityType.Workstation)
			{
				if (tileEntityType != TileEntityType.SecureLootSigned && tileEntityType != TileEntityType.Composite)
				{
					return false;
				}
			}
			else
			{
				TileEntityWorkstation tileEntityWorkstation = te as TileEntityWorkstation;
				if (tileEntityWorkstation != null)
				{
					if (this.includeInputs)
					{
						ItemStack[] input = tileEntityWorkstation.Input;
						for (int i = 0; i < input.Length; i++)
						{
							if (!input[i].IsEmpty())
							{
								isEmpty = false;
							}
						}
					}
					if (this.includeOutputs)
					{
						ItemStack[] output = tileEntityWorkstation.Output;
						for (int j = 0; j < output.Length; j++)
						{
							if (!output[j].IsEmpty())
							{
								isEmpty = false;
							}
						}
					}
					if (this.includeFuel)
					{
						tileEntityWorkstation.IsBurning = false;
						tileEntityWorkstation.ResetTickTime();
						ItemStack[] fuel = tileEntityWorkstation.Fuel;
						for (int k = 0; k < fuel.Length; k++)
						{
							if (!fuel[k].IsEmpty())
							{
								isEmpty = false;
							}
						}
					}
					if (this.includeTools)
					{
						ItemStack[] tools = tileEntityWorkstation.Tools;
						for (int l = 0; l < tools.Length; l++)
						{
							if (!tools[l].IsEmpty())
							{
								isEmpty = false;
							}
						}
					}
					return true;
				}
				return false;
			}
			ITileEntityLootable tileEntityLootable;
			if (te.TryGetSelfOrFeature(out tileEntityLootable) && tileEntityLootable.EntityId == -1)
			{
				isEmpty = tileEntityLootable.IsEmpty();
				return true;
			}
			return false;
		}

		// Token: 0x0600B149 RID: 45385 RVA: 0x004522FC File Offset: 0x004504FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleContainerAction(List<TileEntity> tileEntityList)
		{
			bool flag = false;
			int i = 0;
			while (i < tileEntityList.Count)
			{
				TileEntityType tileEntityType = tileEntityList[i].GetTileEntityType();
				if (tileEntityType <= TileEntityType.SecureLoot)
				{
					if (tileEntityType == TileEntityType.Loot || tileEntityType == TileEntityType.SecureLoot)
					{
						goto IL_3B;
					}
				}
				else if (tileEntityType != TileEntityType.Workstation)
				{
					if (tileEntityType == TileEntityType.SecureLootSigned || tileEntityType == TileEntityType.Composite)
					{
						goto IL_3B;
					}
				}
				else
				{
					TileEntityWorkstation tileEntityWorkstation = tileEntityList[i] as TileEntityWorkstation;
					if (tileEntityWorkstation != null)
					{
						if (this.includeInputs)
						{
							ItemStack[] input = tileEntityWorkstation.Input;
							for (int j = 0; j < input.Length; j++)
							{
								if (!input[j].IsEmpty())
								{
									input[j] = ItemStack.Empty;
									flag = true;
								}
							}
							tileEntityWorkstation.ClearSlotTimersForInputs();
							tileEntityWorkstation.Input = input;
						}
						if (this.includeOutputs)
						{
							ItemStack[] output = tileEntityWorkstation.Output;
							for (int k = 0; k < output.Length; k++)
							{
								if (!output[k].IsEmpty())
								{
									output[k] = ItemStack.Empty;
									flag = true;
								}
							}
							tileEntityWorkstation.Output = output;
						}
						if (this.includeFuel)
						{
							tileEntityWorkstation.IsBurning = false;
							tileEntityWorkstation.ResetTickTime();
							ItemStack[] fuel = tileEntityWorkstation.Fuel;
							for (int l = 0; l < fuel.Length; l++)
							{
								if (!fuel[l].IsEmpty())
								{
									fuel[l] = ItemStack.Empty;
									flag = true;
								}
							}
							tileEntityWorkstation.Fuel = fuel;
						}
						if (this.includeTools)
						{
							ItemStack[] tools = tileEntityWorkstation.Tools;
							for (int m = 0; m < tools.Length; m++)
							{
								if (!tools[m].IsEmpty())
								{
									tools[m] = ItemStack.Empty;
									flag = true;
								}
							}
							tileEntityWorkstation.Tools = tools;
						}
						if (this.includeTools || this.includeOutputs)
						{
							tileEntityWorkstation.ResetCraftingQueue();
						}
					}
				}
				IL_1C8:
				i++;
				continue;
				IL_3B:
				ITileEntityLootable tileEntityLootable;
				if (tileEntityList[i].TryGetSelfOrFeature(out tileEntityLootable) && tileEntityLootable.EntityId == -1 && !tileEntityLootable.IsEmpty())
				{
					tileEntityLootable.SetEmpty();
					flag = true;
					goto IL_1C8;
				}
				goto IL_1C8;
			}
			if (flag && this.changeName && base.Owner.Target != null)
			{
				PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(base.Owner.Target.entityId);
				for (int n = 0; n < tileEntityList.Count; n++)
				{
					ITileEntitySignable tileEntitySignable;
					if ((tileEntityList[n].GetTileEntityType() == TileEntityType.SecureLootSigned || tileEntityList[n].GetTileEntityType() == TileEntityType.Composite) && tileEntityList[n].TryGetSelfOrFeature(out tileEntitySignable) && tileEntitySignable.EntityId == -1)
					{
						tileEntitySignable.SetText(base.ModifiedName, true, (playerDataFromEntityID != null) ? playerDataFromEntityID.PrimaryId : null);
					}
				}
			}
			return flag;
		}

		// Token: 0x0600B14A RID: 45386 RVA: 0x00452598 File Offset: 0x00450798
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionEmptyContainers.PropIncludeInputs, ref this.includeInputs);
			properties.ParseBool(ActionEmptyContainers.PropIncludeOutputs, ref this.includeOutputs);
			properties.ParseBool(ActionEmptyContainers.PropIncludeFuel, ref this.includeFuel);
			properties.ParseBool(ActionEmptyContainers.PropIncludeTools, ref this.includeTools);
		}

		// Token: 0x0600B14B RID: 45387 RVA: 0x004525F0 File Offset: 0x004507F0
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionEmptyContainers
			{
				TargetingType = this.TargetingType,
				maxDistance = this.maxDistance,
				newName = this.newName,
				changeName = this.changeName,
				includeInputs = this.includeInputs,
				includeOutputs = this.includeOutputs,
				includeFuel = this.includeFuel,
				includeTools = this.includeTools,
				tileEntityList = this.tileEntityList
			};
		}

		// Token: 0x04008AC7 RID: 35527
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool includeInputs;

		// Token: 0x04008AC8 RID: 35528
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool includeOutputs;

		// Token: 0x04008AC9 RID: 35529
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool includeFuel;

		// Token: 0x04008ACA RID: 35530
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool includeTools;

		// Token: 0x04008ACB RID: 35531
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIncludeInputs = "include_inputs";

		// Token: 0x04008ACC RID: 35532
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIncludeOutputs = "include_outputs";

		// Token: 0x04008ACD RID: 35533
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIncludeFuel = "include_fuel";

		// Token: 0x04008ACE RID: 35534
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIncludeTools = "include_tools";
	}
}

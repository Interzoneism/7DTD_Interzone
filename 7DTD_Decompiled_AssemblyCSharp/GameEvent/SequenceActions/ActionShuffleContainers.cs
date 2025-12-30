using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016D8 RID: 5848
	[Preserve]
	public class ActionShuffleContainers : ActionBaseContainersAction
	{
		// Token: 0x0600B158 RID: 45400 RVA: 0x00452C70 File Offset: 0x00450E70
		public override bool CheckValidTileEntity(TileEntity te, out bool isEmpty)
		{
			isEmpty = false;
			TileEntityType tileEntityType = te.GetTileEntityType();
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
				if (!this.includeOutputs)
				{
					return false;
				}
				TileEntityWorkstation tileEntityWorkstation = te as TileEntityWorkstation;
				if (tileEntityWorkstation != null && tileEntityWorkstation.EntityId == -1)
				{
					isEmpty = tileEntityWorkstation.OutputEmpty();
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

		// Token: 0x0600B159 RID: 45401 RVA: 0x00452CEC File Offset: 0x00450EEC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleContainerAction(List<TileEntity> tileEntityList)
		{
			List<ItemStack> list = new List<ItemStack>();
			List<TileEntity> list2 = new List<TileEntity>();
			bool flag = false;
			int i = 0;
			while (i < tileEntityList.Count)
			{
				TileEntityType tileEntityType = tileEntityList[i].GetTileEntityType();
				if (tileEntityType <= TileEntityType.SecureLoot)
				{
					if (tileEntityType == TileEntityType.Loot || tileEntityType == TileEntityType.SecureLoot)
					{
						goto IL_50;
					}
				}
				else if (tileEntityType != TileEntityType.Workstation)
				{
					if (tileEntityType == TileEntityType.SecureLootSigned || tileEntityType == TileEntityType.Composite)
					{
						goto IL_50;
					}
				}
				else if (this.includeOutputs)
				{
					TileEntityWorkstation tileEntityWorkstation = tileEntityList[i] as TileEntityWorkstation;
					if (tileEntityWorkstation != null && tileEntityWorkstation.EntityId == -1)
					{
						list2.Add(tileEntityWorkstation);
						list.AddRange(tileEntityWorkstation.Output);
						if (!tileEntityWorkstation.OutputEmpty())
						{
							flag = true;
						}
					}
				}
				IL_D8:
				i++;
				continue;
				IL_50:
				ITileEntityLootable tileEntityLootable;
				if (!tileEntityList[i].TryGetSelfOrFeature(out tileEntityLootable) || tileEntityLootable.EntityId != -1)
				{
					goto IL_D8;
				}
				list2.Add(tileEntityList[i]);
				list.AddRange(tileEntityLootable.items);
				if (!tileEntityLootable.IsEmpty())
				{
					flag = true;
					goto IL_D8;
				}
				goto IL_D8;
			}
			if (flag && this.changeName && base.Owner.Target != null)
			{
				PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(base.Owner.Target.entityId);
				for (int j = 0; j < tileEntityList.Count; j++)
				{
					ITileEntitySignable tileEntitySignable;
					if ((tileEntityList[j].GetTileEntityType() == TileEntityType.SecureLootSigned || tileEntityList[j].GetTileEntityType() == TileEntityType.Composite) && tileEntityList[j].TryGetSelfOrFeature(out tileEntitySignable) && tileEntitySignable.EntityId == -1)
					{
						tileEntitySignable.SetText(base.ModifiedName, true, (playerDataFromEntityID != null) ? playerDataFromEntityID.PrimaryId : null);
					}
				}
			}
			GameRandom random = GameEventManager.Current.Random;
			if (list2.Count > 0)
			{
				for (int k = 0; k < list.Count * 2; k++)
				{
					int index = random.RandomRange(list.Count);
					int index2 = random.RandomRange(list.Count);
					ItemStack value = list[index];
					list[index] = list[index2];
					list[index2] = value;
				}
				int l = 0;
				while (l < list2.Count)
				{
					TileEntityType tileEntityType2 = list2[l].GetTileEntityType();
					if (tileEntityType2 <= TileEntityType.SecureLoot)
					{
						if (tileEntityType2 == TileEntityType.Loot || tileEntityType2 == TileEntityType.SecureLoot)
						{
							goto IL_251;
						}
					}
					else if (tileEntityType2 != TileEntityType.Workstation)
					{
						if (tileEntityType2 == TileEntityType.SecureLootSigned || tileEntityType2 == TileEntityType.Composite)
						{
							goto IL_251;
						}
					}
					else if (this.includeOutputs)
					{
						TileEntityWorkstation tileEntityWorkstation2 = list2[l] as TileEntityWorkstation;
						if (tileEntityWorkstation2 != null)
						{
							ItemStack[] output = tileEntityWorkstation2.Output;
							for (int m = 0; m < output.Length; m++)
							{
								output[m] = list[0];
								list.RemoveAt(0);
							}
							tileEntityWorkstation2.Output = output;
						}
					}
					IL_2EA:
					list2[l].SetModified();
					l++;
					continue;
					IL_251:
					ITileEntityLootable tileEntityLootable2;
					if (list2[l].TryGetSelfOrFeature(out tileEntityLootable2))
					{
						for (int n = 0; n < tileEntityLootable2.items.Length; n++)
						{
							tileEntityLootable2.items[n] = list[0];
							list.RemoveAt(0);
						}
						goto IL_2EA;
					}
					goto IL_2EA;
				}
			}
			return flag;
		}

		// Token: 0x0600B15A RID: 45402 RVA: 0x00453004 File Offset: 0x00451204
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionShuffleContainers.PropIncludeOutputs, ref this.includeOutputs);
		}

		// Token: 0x0600B15B RID: 45403 RVA: 0x00453020 File Offset: 0x00451220
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionShuffleContainers
			{
				TargetingType = this.TargetingType,
				maxDistance = this.maxDistance,
				newName = this.newName,
				changeName = this.changeName,
				includeOutputs = this.includeOutputs,
				tileEntityList = this.tileEntityList
			};
		}

		// Token: 0x04008AD6 RID: 35542
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool includeOutputs;

		// Token: 0x04008AD7 RID: 35543
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIncludeOutputs = "include_outputs";
	}
}

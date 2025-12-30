using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016D2 RID: 5842
	[Preserve]
	public class ActionBaseContainersAction : BaseAction
	{
		// Token: 0x170013A5 RID: 5029
		// (get) Token: 0x0600B13D RID: 45373 RVA: 0x00451DA6 File Offset: 0x0044FFA6
		public string ModifiedName
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return base.GetTextWithElements(this.newName);
			}
		}

		// Token: 0x0600B13E RID: 45374 RVA: 0x00451DB4 File Offset: 0x0044FFB4
		public override bool CanPerform(Entity target)
		{
			return base.CanPerform(target) && this.GetTileEntityList(target);
		}

		// Token: 0x0600B13F RID: 45375 RVA: 0x00451DC8 File Offset: 0x0044FFC8
		public virtual bool CheckValidTileEntity(TileEntity te, out bool isEmpty)
		{
			isEmpty = true;
			return true;
		}

		// Token: 0x0600B140 RID: 45376 RVA: 0x00451DD0 File Offset: 0x0044FFD0
		[PublicizedFrom(EAccessModifier.Private)]
		public bool GetTileEntityList(Entity target)
		{
			World world = GameManager.Instance.World;
			Vector3i blockPosition = target.GetBlockPosition();
			int num = World.toChunkXZ(blockPosition.x);
			int num2 = World.toChunkXZ(blockPosition.z);
			int @int = GameStats.GetInt(EnumGameStats.LandClaimSize);
			int num3 = @int / 16 + 1;
			int num4 = @int / 16 + 1;
			this.tileEntityList.Clear();
			bool result = false;
			for (int i = -num4; i <= num4; i++)
			{
				for (int j = -num3; j <= num3; j++)
				{
					Chunk chunk = (Chunk)world.GetChunkSync(num + j, num2 + i);
					if (chunk != null)
					{
						DictionaryList<Vector3i, TileEntity> tileEntities = chunk.GetTileEntities();
						for (int k = 0; k < tileEntities.list.Count; k++)
						{
							TileEntity tileEntity = tileEntities.list[k];
							if (tileEntity != null && tileEntity.EntityId == -1)
							{
								bool flag = false;
								ActionBaseContainersAction.TargetingTypes targetingType = this.TargetingType;
								if (targetingType != ActionBaseContainersAction.TargetingTypes.SafeZone)
								{
									if (targetingType == ActionBaseContainersAction.TargetingTypes.Distance)
									{
										if (target.GetDistanceSq(tileEntity.ToWorldPos().ToVector3()) < this.maxDistance)
										{
											ITileEntityLootable tileEntityLootable;
											if (tileEntity.TryGetSelfOrFeature(out tileEntityLootable) && !tileEntityLootable.bPlayerStorage)
											{
												goto IL_1DA;
											}
											flag = true;
										}
									}
								}
								else
								{
									ITileEntityLootable tileEntityLootable2;
									if (tileEntity.TryGetSelfOrFeature(out tileEntityLootable2) && !tileEntityLootable2.bPlayerStorage)
									{
										goto IL_1DA;
									}
									flag = world.IsMyLandProtectedBlock(tileEntity.ToWorldPos(), world.gameManager.GetPersistentPlayerList().GetPlayerDataFromEntityID(target.entityId), false);
								}
								if (flag)
								{
									bool flag2 = false;
									if (this.CheckValidTileEntity(tileEntity, out flag2))
									{
										this.tileEntityList.Add(tileEntity);
										if (!flag2)
										{
											result = true;
										}
										int entityIDForLockedTileEntity = GameManager.Instance.GetEntityIDForLockedTileEntity(tileEntity);
										if (entityIDForLockedTileEntity != -1)
										{
											EntityPlayer entityPlayer = world.GetEntity(entityIDForLockedTileEntity) as EntityPlayer;
											if (entityPlayer.isEntityRemote)
											{
												SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityIDForLockedTileEntity), false, entityIDForLockedTileEntity, -1, -1, null, 192, false);
											}
											else
											{
												(entityPlayer as EntityPlayerLocal).PlayerUI.windowManager.CloseAllOpenWindows(null, false);
											}
										}
									}
								}
							}
							IL_1DA:;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600B141 RID: 45377 RVA: 0x00451FF0 File Offset: 0x004501F0
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			World world = GameManager.Instance.World;
			for (int i = 0; i < this.tileEntityList.Count; i++)
			{
				TileEntity te = this.tileEntityList[i];
				int entityIDForLockedTileEntity = GameManager.Instance.GetEntityIDForLockedTileEntity(te);
				if (entityIDForLockedTileEntity != -1)
				{
					EntityPlayer entityPlayer = world.GetEntity(entityIDForLockedTileEntity) as EntityPlayer;
					if (entityPlayer.isEntityRemote)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityIDForLockedTileEntity), false, entityIDForLockedTileEntity, -1, -1, null, 192, false);
					}
					else
					{
						(entityPlayer as EntityPlayerLocal).PlayerUI.windowManager.CloseAllOpenWindows(null, false);
					}
					return BaseAction.ActionCompleteStates.InComplete;
				}
			}
			if (!this.HandleContainerAction(this.tileEntityList))
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B142 RID: 45378 RVA: 0x0000FB42 File Offset: 0x0000DD42
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool HandleContainerAction(List<TileEntity> tileEntityList)
		{
			return false;
		}

		// Token: 0x0600B143 RID: 45379 RVA: 0x004520AC File Offset: 0x004502AC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override string ParseTextElement(string element)
		{
			if (!(element == "viewer"))
			{
				return element;
			}
			if (base.Owner.ExtraData.Length <= 12)
			{
				return base.Owner.ExtraData;
			}
			return base.Owner.ExtraData.Insert(12, "\n");
		}

		// Token: 0x0600B144 RID: 45380 RVA: 0x00452100 File Offset: 0x00450300
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Contains(ActionBaseContainersAction.PropNewName))
			{
				this.changeName = true;
				properties.ParseString(ActionBaseContainersAction.PropNewName, ref this.newName);
			}
			properties.ParseEnum<ActionBaseContainersAction.TargetingTypes>(ActionBaseContainersAction.PropTargetingType, ref this.TargetingType);
			properties.ParseFloat(ActionBaseContainersAction.PropMaxDistance, ref this.maxDistance);
			this.maxDistance *= this.maxDistance;
		}

		// Token: 0x0600B145 RID: 45381 RVA: 0x00019766 File Offset: 0x00017966
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return null;
		}

		// Token: 0x04008AB8 RID: 35512
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxDistance = 5f;

		// Token: 0x04008AB9 RID: 35513
		[PublicizedFrom(EAccessModifier.Protected)]
		public string newName = "";

		// Token: 0x04008ABA RID: 35514
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool changeName;

		// Token: 0x04008ABB RID: 35515
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<TileEntity> tileEntityList = new List<TileEntity>();

		// Token: 0x04008ABC RID: 35516
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionBaseContainersAction.ContainerActionStates ActionState;

		// Token: 0x04008ABD RID: 35517
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionBaseContainersAction.TargetingTypes TargetingType;

		// Token: 0x04008ABE RID: 35518
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxDistance = "max_distance";

		// Token: 0x04008ABF RID: 35519
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropNewName = "new_name";

		// Token: 0x04008AC0 RID: 35520
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetingType = "targeting_type";

		// Token: 0x020016D3 RID: 5843
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum ContainerActionStates
		{
			// Token: 0x04008AC2 RID: 35522
			FindContainers,
			// Token: 0x04008AC3 RID: 35523
			Action
		}

		// Token: 0x020016D4 RID: 5844
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum TargetingTypes
		{
			// Token: 0x04008AC5 RID: 35525
			SafeZone,
			// Token: 0x04008AC6 RID: 35526
			Distance
		}
	}
}

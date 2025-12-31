using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000B16 RID: 2838
public class TileEntityWorkstation : TileEntity
{
	// Token: 0x14000084 RID: 132
	// (add) Token: 0x0600581B RID: 22555 RVA: 0x0023A144 File Offset: 0x00238344
	// (remove) Token: 0x0600581C RID: 22556 RVA: 0x0023A17C File Offset: 0x0023837C
	public event XUiEvent_InputStackChanged InputChanged;

	// Token: 0x14000085 RID: 133
	// (add) Token: 0x0600581D RID: 22557 RVA: 0x0023A1B4 File Offset: 0x002383B4
	// (remove) Token: 0x0600581E RID: 22558 RVA: 0x0023A1EC File Offset: 0x002383EC
	public event XUiEvent_FuelStackChanged FuelChanged;

	// Token: 0x170008CD RID: 2253
	// (get) Token: 0x0600581F RID: 22559 RVA: 0x0023A221 File Offset: 0x00238421
	public string[] MaterialNames
	{
		get
		{
			return this.materialNames;
		}
	}

	// Token: 0x170008CE RID: 2254
	// (get) Token: 0x06005820 RID: 22560 RVA: 0x0023A229 File Offset: 0x00238429
	// (set) Token: 0x06005821 RID: 22561 RVA: 0x0023A231 File Offset: 0x00238431
	public ItemStack[] Tools
	{
		get
		{
			return this.tools;
		}
		set
		{
			if (!this.IsToolsSame(value))
			{
				this.tools = ItemStack.Clone(value);
				this.visibleChanged = true;
				this.UpdateVisible();
				this.setModified();
			}
		}
	}

	// Token: 0x06005822 RID: 22562 RVA: 0x0023A25C File Offset: 0x0023845C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsToolsSame(ItemStack[] _tools)
	{
		if (_tools == null || _tools.Length != this.tools.Length)
		{
			return false;
		}
		for (int i = 0; i < _tools.Length; i++)
		{
			if (!_tools[i].Equals(this.tools[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x170008CF RID: 2255
	// (get) Token: 0x06005823 RID: 22563 RVA: 0x0023A29E File Offset: 0x0023849E
	// (set) Token: 0x06005824 RID: 22564 RVA: 0x0023A2A6 File Offset: 0x002384A6
	public ItemStack[] Fuel
	{
		get
		{
			return this.fuel;
		}
		set
		{
			this.fuel = ItemStack.Clone(value);
			this.setModified();
		}
	}

	// Token: 0x170008D0 RID: 2256
	// (get) Token: 0x06005825 RID: 22565 RVA: 0x0023A2BA File Offset: 0x002384BA
	// (set) Token: 0x06005826 RID: 22566 RVA: 0x0023A2C2 File Offset: 0x002384C2
	public ItemStack[] Input
	{
		get
		{
			return this.input;
		}
		set
		{
			this.input = ItemStack.Clone(value);
			this.setModified();
		}
	}

	// Token: 0x170008D1 RID: 2257
	// (get) Token: 0x06005827 RID: 22567 RVA: 0x0023A2D6 File Offset: 0x002384D6
	// (set) Token: 0x06005828 RID: 22568 RVA: 0x0023A2DE File Offset: 0x002384DE
	public ItemStack[] Output
	{
		get
		{
			return this.output;
		}
		set
		{
			this.output = ItemStack.Clone(value);
			this.setModified();
		}
	}

	// Token: 0x170008D2 RID: 2258
	// (get) Token: 0x06005829 RID: 22569 RVA: 0x0023A2F2 File Offset: 0x002384F2
	// (set) Token: 0x0600582A RID: 22570 RVA: 0x0023A2FA File Offset: 0x002384FA
	public RecipeQueueItem[] Queue
	{
		get
		{
			return this.queue;
		}
		set
		{
			this.queue = value;
			this.setModified();
		}
	}

	// Token: 0x170008D3 RID: 2259
	// (get) Token: 0x0600582B RID: 22571 RVA: 0x0023A309 File Offset: 0x00238509
	// (set) Token: 0x0600582C RID: 22572 RVA: 0x0023A311 File Offset: 0x00238511
	public bool IsBurning
	{
		get
		{
			return this.isBurning;
		}
		set
		{
			this.isBurning = value;
			this.setModified();
		}
	}

	// Token: 0x170008D4 RID: 2260
	// (get) Token: 0x0600582D RID: 22573 RVA: 0x0023A320 File Offset: 0x00238520
	public bool IsCrafting
	{
		get
		{
			return this.hasRecipeInQueue() && (!this.isModuleUsed[3] || this.isBurning);
		}
	}

	// Token: 0x170008D5 RID: 2261
	// (get) Token: 0x0600582E RID: 22574 RVA: 0x0023A33E File Offset: 0x0023853E
	// (set) Token: 0x0600582F RID: 22575 RVA: 0x0023A346 File Offset: 0x00238546
	public bool IsPlayerPlaced
	{
		get
		{
			return this.isPlayerPlaced;
		}
		set
		{
			this.isPlayerPlaced = value;
			this.setModified();
		}
	}

	// Token: 0x170008D6 RID: 2262
	// (get) Token: 0x06005830 RID: 22576 RVA: 0x0023A355 File Offset: 0x00238555
	public bool IsBesideWater
	{
		get
		{
			return this.isBesideWater;
		}
	}

	// Token: 0x170008D7 RID: 2263
	// (get) Token: 0x06005831 RID: 22577 RVA: 0x0023A35D File Offset: 0x0023855D
	public float BurnTimeLeft
	{
		get
		{
			return this.currentBurnTimeLeft;
		}
	}

	// Token: 0x170008D8 RID: 2264
	// (get) Token: 0x06005832 RID: 22578 RVA: 0x0023A365 File Offset: 0x00238565
	public float BurnTotalTimeLeft
	{
		get
		{
			return this.getTotalFuelSeconds() + this.currentBurnTimeLeft;
		}
	}

	// Token: 0x170008D9 RID: 2265
	// (get) Token: 0x06005833 RID: 22579 RVA: 0x00075C39 File Offset: 0x00073E39
	public int InputSlotCount
	{
		get
		{
			return 3;
		}
	}

	// Token: 0x06005834 RID: 22580 RVA: 0x0023A374 File Offset: 0x00238574
	public TileEntityWorkstation(Chunk _chunk) : base(_chunk)
	{
		this.fuel = ItemStack.CreateArray(3);
		this.tools = ItemStack.CreateArray(3);
		this.output = ItemStack.CreateArray(6);
		this.input = ItemStack.CreateArray(3);
		this.lastInput = ItemStack.CreateArray(3);
		this.queue = new RecipeQueueItem[4];
		this.materialNames = new string[0];
		this.isModuleUsed = new bool[5];
		this.currentMeltTimesLeft = new float[this.input.Length];
	}

	// Token: 0x06005835 RID: 22581 RVA: 0x0023A3FB File Offset: 0x002385FB
	public void ResetTickTime()
	{
		this.lastTickTime = GameTimer.Instance.ticks;
	}

	// Token: 0x06005836 RID: 22582 RVA: 0x0023A410 File Offset: 0x00238610
	public override void OnSetLocalChunkPosition()
	{
		if (base.localChunkPos == Vector3i.zero)
		{
			return;
		}
		Block block = this.chunk.GetBlock(World.toBlockXZ(base.localChunkPos.x), base.localChunkPos.y, World.toBlockXZ(base.localChunkPos.z)).Block;
		if (block.Properties.Values.ContainsKey("Workstation.InputMaterials"))
		{
			string text = block.Properties.Values["Workstation.InputMaterials"];
			if (text.Contains(","))
			{
				this.materialNames = text.Replace(" ", "").Split(',', StringSplitOptions.None);
			}
			else
			{
				this.materialNames = new string[]
				{
					text
				};
			}
			if (this.input.Length != 3 + this.materialNames.Length)
			{
				ItemStack[] array = new ItemStack[3 + this.materialNames.Length];
				for (int i = 0; i < this.input.Length; i++)
				{
					array[i] = this.input[i].Clone();
				}
				this.input = array;
				for (int j = 0; j < this.materialNames.Length; j++)
				{
					ItemClass itemClass = ItemClass.GetItemClass("unit_" + this.materialNames[j], false);
					if (itemClass != null)
					{
						int num = j + 3;
						this.input[num] = new ItemStack(new ItemValue(itemClass.Id, false), 0);
					}
				}
			}
		}
		if (block.Properties.Values.ContainsKey("Workstation.Modules"))
		{
			string text2 = block.Properties.Values["Workstation.Modules"];
			string[] array2;
			if (text2.Contains(","))
			{
				array2 = text2.Replace(" ", "").Split(',', StringSplitOptions.None);
			}
			else
			{
				array2 = new string[]
				{
					text2
				};
			}
			for (int k = 0; k < array2.Length; k++)
			{
				TileEntityWorkstation.Module module = EnumUtils.Parse<TileEntityWorkstation.Module>(array2[k], true);
				this.isModuleUsed[(int)module] = true;
			}
			if (this.isModuleUsed[4])
			{
				this.isModuleUsed[1] = true;
			}
		}
	}

	// Token: 0x06005837 RID: 22583 RVA: 0x0023A634 File Offset: 0x00238834
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLightState(World world, BlockValue blockValue)
	{
		bool flag = this.CanOperate(GameTimer.Instance.ticks);
		if (!flag && blockValue.meta != 0)
		{
			blockValue.meta = 0;
			world.SetBlockRPC(base.ToWorldPos(), blockValue);
			return;
		}
		if (flag && blockValue.meta != 15)
		{
			blockValue.meta = 15;
			world.SetBlockRPC(base.ToWorldPos(), blockValue);
		}
	}

	// Token: 0x06005838 RID: 22584 RVA: 0x0023A309 File Offset: 0x00238509
	public bool CanOperate(ulong _worldTimeInTicks)
	{
		return this.isBurning;
	}

	// Token: 0x06005839 RID: 22585 RVA: 0x0023A698 File Offset: 0x00238898
	public override bool IsActive(World world)
	{
		return this.IsBurning;
	}

	// Token: 0x0600583A RID: 22586 RVA: 0x0023A6A0 File Offset: 0x002388A0
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		bool flag = (!this.isModuleUsed[3] && this.hasRecipeInQueue()) || this.isBurning;
		float num = (GameTimer.Instance.ticks - this.lastTickTime) / 20f;
		float num2 = Mathf.Min(num, this.BurnTotalTimeLeft);
		float timePassed = this.isModuleUsed[3] ? num2 : num;
		this.isBesideWater = base.IsByWater(world, base.ToWorldPos());
		this.isBurning &= !this.isBesideWater;
		BlockValue block = world.GetBlock(base.ToWorldPos());
		this.UpdateLightState(world, block);
		if (this.isModuleUsed[3])
		{
			this.HandleFuel(world, timePassed);
		}
		else if (block.Block.HeatMapStrength > 0f && this.IsCrafting)
		{
			base.emitHeatMapEvent(world, EnumAIDirectorChunkEvent.Campfire);
		}
		this.HandleRecipeQueue(timePassed);
		this.HandleMaterialInput(timePassed);
		if (this.isModuleUsed[3])
		{
			this.isBurning &= (this.BurnTotalTimeLeft > 0f);
		}
		this.lastTickTime = GameTimer.Instance.ticks;
		if ((!this.isModuleUsed[3] && this.hasRecipeInQueue()) || this.isBurning || flag)
		{
			this.setModified();
		}
		this.UpdateVisible();
	}

	// Token: 0x0600583B RID: 22587 RVA: 0x0023A7ED File Offset: 0x002389ED
	public void SetVisibleChanged()
	{
		this.visibleChanged = true;
	}

	// Token: 0x0600583C RID: 22588 RVA: 0x0023A7F8 File Offset: 0x002389F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateVisible()
	{
		bool isCrafting = this.IsCrafting;
		if (isCrafting != this.visibleCrafting)
		{
			this.visibleCrafting = isCrafting;
			this.visibleChanged = true;
		}
		bool flag = (!this.isModuleUsed[3] && this.hasRecipeInQueue()) || this.isBurning;
		if (flag != this.visibleWorking)
		{
			this.visibleWorking = flag;
			this.visibleChanged = true;
		}
		if (this.visibleChanged)
		{
			this.visibleChanged = false;
			BlockWorkstation blockWorkstation = GameManager.Instance.World.GetBlock(base.ToWorldPos()).Block as BlockWorkstation;
			if (blockWorkstation != null)
			{
				blockWorkstation.UpdateVisible(this);
			}
		}
	}

	// Token: 0x0600583D RID: 22589 RVA: 0x0023A892 File Offset: 0x00238A92
	public float GetTimerForSlot(int inputSlot)
	{
		if (inputSlot >= this.currentMeltTimesLeft.Length)
		{
			return 0f;
		}
		return this.currentMeltTimesLeft[inputSlot];
	}

	// Token: 0x0600583E RID: 22590 RVA: 0x0023A8B0 File Offset: 0x00238AB0
	public void ClearSlotTimersForInputs()
	{
		for (int i = 0; i < this.currentMeltTimesLeft.Length; i++)
		{
			this.currentMeltTimesLeft[i] = 0f;
		}
	}

	// Token: 0x0600583F RID: 22591 RVA: 0x0023A8E0 File Offset: 0x00238AE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleMaterialInput(float timePassed)
	{
		if (this.isModuleUsed[4] && (this.isBurning || !this.isModuleUsed[3]))
		{
			for (int i = 0; i < this.input.Length - this.materialNames.Length; i++)
			{
				if (this.input[i].IsEmpty())
				{
					this.input[i].Clear();
					this.currentMeltTimesLeft[i] = -2.1474836E+09f;
					if (this.InputChanged != null)
					{
						this.InputChanged();
					}
				}
				else
				{
					ItemClass forId = ItemClass.GetForId(this.input[i].itemValue.type);
					if (forId != null)
					{
						if (this.currentMeltTimesLeft[i] >= 0f && this.input[i].count > 0)
						{
							if (this.lastInput[i].itemValue.type != this.input[i].itemValue.type)
							{
								this.currentMeltTimesLeft[i] = -2.1474836E+09f;
							}
							else
							{
								this.currentMeltTimesLeft[i] -= timePassed;
							}
						}
						if (this.currentMeltTimesLeft[i] == -2.1474836E+09f && this.input[i].count > 0)
						{
							for (int j = 0; j < this.materialNames.Length; j++)
							{
								if (forId.MadeOfMaterial.ForgeCategory != null && forId.MadeOfMaterial.ForgeCategory.EqualsCaseInsensitive(this.materialNames[j]))
								{
									ItemClass itemClass = ItemClass.GetItemClass("unit_" + this.materialNames[j], false);
									if (itemClass != null && itemClass.MadeOfMaterial.ForgeCategory != null)
									{
										float num = (float)forId.GetWeight() * ((forId.MeltTimePerUnit > 0f) ? forId.MeltTimePerUnit : 1f);
										if (this.isModuleUsed[0])
										{
											for (int k = 0; k < this.tools.Length; k++)
											{
												float num2 = 1f;
												this.tools[k].itemValue.ModifyValue(null, null, PassiveEffects.CraftingSmeltTime, ref num, ref num2, FastTags<TagGroup.Global>.Parse(forId.Name), true, false);
												num *= num2;
											}
										}
										if (num > 0f && this.currentMeltTimesLeft[i] == -2.1474836E+09f)
										{
											this.currentMeltTimesLeft[i] = num;
										}
										else
										{
											this.currentMeltTimesLeft[i] += num;
										}
									}
								}
							}
							this.lastInput[i] = this.input[i].Clone();
						}
						if (this.currentMeltTimesLeft[i] != -2.1474836E+09f)
						{
							int num3 = 0;
							int num4 = 3;
							while (num4 < this.input.Length & num3 < this.materialNames.Length)
							{
								if (forId.MadeOfMaterial.ForgeCategory != null && forId.MadeOfMaterial.ForgeCategory.EqualsCaseInsensitive(this.materialNames[num3]))
								{
									ItemClass itemClass2 = ItemClass.GetItemClass("unit_" + this.materialNames[num3], false);
									if (itemClass2 != null && itemClass2.MadeOfMaterial.ForgeCategory != null)
									{
										if (this.input[num4].itemValue.type == 0)
										{
											this.input[num4] = new ItemStack(new ItemValue(itemClass2.Id, false), this.input[num4].count);
										}
										bool flag = false;
										while (this.currentMeltTimesLeft[i] < 0f && this.currentMeltTimesLeft[i] != -2.1474836E+09f)
										{
											if (this.input[i].count <= 0)
											{
												this.input[i].Clear();
												this.currentMeltTimesLeft[i] = 0f;
												flag = true;
												if (this.InputChanged != null)
												{
													this.InputChanged();
													break;
												}
												break;
											}
											else
											{
												if (this.input[num4].count + forId.GetWeight() > itemClass2.Stacknumber.Value)
												{
													this.currentMeltTimesLeft[i] = -2.1474836E+09f;
													break;
												}
												this.input[num4].count += forId.GetWeight();
												this.input[i].count--;
												float num5 = (float)forId.GetWeight() * ((forId.MeltTimePerUnit > 0f) ? forId.MeltTimePerUnit : 1f);
												if (this.isModuleUsed[0])
												{
													for (int l = 0; l < this.tools.Length; l++)
													{
														if (!this.tools[l].IsEmpty())
														{
															float num6 = 1f;
															this.tools[l].itemValue.ModifyValue(null, null, PassiveEffects.CraftingSmeltTime, ref num5, ref num6, FastTags<TagGroup.Global>.Parse(itemClass2.Name), true, false);
															num5 *= num6;
														}
													}
												}
												this.currentMeltTimesLeft[i] += num5;
												if (this.input[i].count <= 0)
												{
													this.input[i].Clear();
													this.currentMeltTimesLeft[i] = -2.1474836E+09f;
													flag = true;
													if (this.InputChanged != null)
													{
														this.InputChanged();
														break;
													}
													break;
												}
												else
												{
													if (this.InputChanged != null)
													{
														this.InputChanged();
													}
													flag = true;
												}
											}
										}
										if (flag && this.currentMeltTimesLeft[i] < 0f && this.currentMeltTimesLeft[i] != -2.1474836E+09f)
										{
											this.currentMeltTimesLeft[i] = -2.1474836E+09f;
											break;
										}
										break;
									}
								}
								num3++;
								num4++;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06005840 RID: 22592 RVA: 0x0023AE40 File Offset: 0x00239040
	[PublicizedFrom(EAccessModifier.Private)]
	public void cycleRecipeQueue()
	{
		RecipeQueueItem recipeQueueItem = null;
		if (this.queue[this.queue.Length - 1] != null && this.queue[this.queue.Length - 1].Multiplier > 0)
		{
			return;
		}
		for (int i = 0; i < this.queue.Length; i++)
		{
			recipeQueueItem = this.queue[this.queue.Length - 1];
			if (recipeQueueItem != null && recipeQueueItem.Multiplier != 0)
			{
				break;
			}
			for (int j = this.queue.Length - 1; j >= 0; j--)
			{
				RecipeQueueItem recipeQueueItem2 = this.queue[j];
				if (j != 0)
				{
					RecipeQueueItem recipeQueueItem3 = this.queue[j - 1];
					if (recipeQueueItem3.Multiplier < 0)
					{
						recipeQueueItem3.Multiplier = 0;
					}
					recipeQueueItem2.Recipe = recipeQueueItem3.Recipe;
					recipeQueueItem2.Multiplier = recipeQueueItem3.Multiplier;
					recipeQueueItem2.CraftingTimeLeft = recipeQueueItem3.CraftingTimeLeft;
					recipeQueueItem2.IsCrafting = recipeQueueItem3.IsCrafting;
					recipeQueueItem2.Quality = recipeQueueItem3.Quality;
					recipeQueueItem2.OneItemCraftTime = recipeQueueItem3.OneItemCraftTime;
					recipeQueueItem2.StartingEntityId = recipeQueueItem3.StartingEntityId;
					this.queue[j] = recipeQueueItem2;
					recipeQueueItem3 = new RecipeQueueItem();
					recipeQueueItem3.Recipe = null;
					recipeQueueItem3.Multiplier = 0;
					recipeQueueItem3.CraftingTimeLeft = 0f;
					recipeQueueItem3.OneItemCraftTime = 0f;
					recipeQueueItem3.IsCrafting = false;
					recipeQueueItem3.Quality = 0;
					recipeQueueItem3.StartingEntityId = -1;
					this.queue[j - 1] = recipeQueueItem3;
				}
			}
		}
		if (recipeQueueItem != null && recipeQueueItem.Recipe != null && !recipeQueueItem.IsCrafting && recipeQueueItem.Multiplier != 0)
		{
			recipeQueueItem.IsCrafting = true;
		}
	}

	// Token: 0x06005841 RID: 22593 RVA: 0x0023AFD8 File Offset: 0x002391D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleRecipeQueue(float _timePassed)
	{
		if (this.bUserAccessing)
		{
			return;
		}
		if (this.queue.Length == 0)
		{
			return;
		}
		if (this.isModuleUsed[3] && !this.isBurning)
		{
			return;
		}
		RecipeQueueItem recipeQueueItem = this.queue[this.queue.Length - 1];
		if (recipeQueueItem == null)
		{
			return;
		}
		if (recipeQueueItem.CraftingTimeLeft >= 0f)
		{
			recipeQueueItem.CraftingTimeLeft -= _timePassed;
		}
		while (recipeQueueItem.CraftingTimeLeft < 0f && this.hasRecipeInQueue())
		{
			if (recipeQueueItem.Multiplier > 0)
			{
				ItemValue itemValue = new ItemValue(recipeQueueItem.Recipe.itemValueType, false);
				if (ItemClass.list[recipeQueueItem.Recipe.itemValueType] != null && ItemClass.list[recipeQueueItem.Recipe.itemValueType].HasQuality)
				{
					itemValue = new ItemValue(recipeQueueItem.Recipe.itemValueType, (int)recipeQueueItem.Quality, (int)recipeQueueItem.Quality, false, null, 1f);
				}
				if (ItemStack.AddToItemStackArray(this.output, new ItemStack(itemValue, recipeQueueItem.Recipe.count), -1) == -1)
				{
					return;
				}
				this.AddCraftComplete(recipeQueueItem.StartingEntityId, itemValue, recipeQueueItem.Recipe.GetName(), recipeQueueItem.Recipe.IsScrap ? recipeQueueItem.Recipe.ingredients[0].itemValue.ItemClass.GetItemName() : "", recipeQueueItem.Recipe.craftExpGain, recipeQueueItem.Recipe.count);
				GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.CraftedItems, itemValue.ItemClass.Name, recipeQueueItem.Recipe.count, true, GameSparksCollector.GSDataCollection.SessionUpdates);
				RecipeQueueItem recipeQueueItem2 = recipeQueueItem;
				recipeQueueItem2.Multiplier -= 1;
				recipeQueueItem.CraftingTimeLeft += recipeQueueItem.OneItemCraftTime;
			}
			if (recipeQueueItem.Multiplier <= 0)
			{
				float craftingTimeLeft = recipeQueueItem.CraftingTimeLeft;
				this.cycleRecipeQueue();
				recipeQueueItem = this.queue[this.queue.Length - 1];
				recipeQueueItem.CraftingTimeLeft += ((craftingTimeLeft < 0f) ? craftingTimeLeft : 0f);
			}
		}
	}

	// Token: 0x06005842 RID: 22594 RVA: 0x0023B1D4 File Offset: 0x002393D4
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HandleFuel(World _world, float _timePassed)
	{
		if (!this.isBurning)
		{
			return false;
		}
		base.emitHeatMapEvent(_world, EnumAIDirectorChunkEvent.Campfire);
		bool result = false;
		if (this.currentBurnTimeLeft > 0f || (this.currentBurnTimeLeft == 0f && this.getTotalFuelSeconds() > 0f))
		{
			this.currentBurnTimeLeft -= _timePassed;
			this.currentBurnTimeLeft = (float)Mathf.FloorToInt(this.currentBurnTimeLeft * 100f) / 100f;
			result = true;
		}
		while (this.currentBurnTimeLeft < 0f && this.getTotalFuelSeconds() > 0f)
		{
			if (this.fuel[0].count > 0)
			{
				this.fuel[0].count--;
				this.currentBurnTimeLeft += this.GetFuelTime(this.fuel[0]);
				result = true;
				if (this.FuelChanged != null)
				{
					this.FuelChanged();
				}
			}
			else
			{
				this.cycleFuelStacks();
				result = true;
			}
		}
		if (this.getTotalFuelSeconds() == 0f && this.currentBurnTimeLeft < 0f)
		{
			this.currentBurnTimeLeft = 0f;
			result = true;
		}
		return result;
	}

	// Token: 0x06005843 RID: 22595 RVA: 0x0023B2F4 File Offset: 0x002394F4
	[PublicizedFrom(EAccessModifier.Private)]
	public float getFuelTime(ItemStack _itemStack)
	{
		if (_itemStack == null)
		{
			return 0f;
		}
		ItemClass forId = ItemClass.GetForId(_itemStack.itemValue.type);
		float result = 0f;
		if (forId == null)
		{
			return result;
		}
		if (!forId.IsBlock())
		{
			if (forId.FuelValue != null)
			{
				result = (float)forId.FuelValue.Value;
			}
		}
		else if (forId.Id < Block.list.Length)
		{
			Block block = Block.list[forId.Id];
			if (block != null)
			{
				result = (float)block.FuelValue;
			}
		}
		return result;
	}

	// Token: 0x06005844 RID: 22596 RVA: 0x0023B370 File Offset: 0x00239570
	[PublicizedFrom(EAccessModifier.Private)]
	public void cycleFuelStacks()
	{
		if (this.fuel.Length < 2)
		{
			return;
		}
		for (int i = 0; i < this.fuel.Length - 1; i++)
		{
			for (int j = 0; j < this.fuel.Length; j++)
			{
				ItemStack itemStack = this.fuel[j];
				if (itemStack.count <= 0 && j + 1 < this.fuel.Length)
				{
					ItemStack itemStack2 = this.fuel[j + 1];
					itemStack = itemStack2.Clone();
					this.fuel[j] = itemStack;
					itemStack2 = ItemStack.Empty.Clone();
					this.fuel[j + 1] = itemStack2;
				}
			}
			if (this.fuel[0].count > 0)
			{
				break;
			}
		}
	}

	// Token: 0x06005845 RID: 22597 RVA: 0x0023B414 File Offset: 0x00239614
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setModified()
	{
		base.setModified();
	}

	// Token: 0x06005846 RID: 22598 RVA: 0x0023B41C File Offset: 0x0023961C
	[PublicizedFrom(EAccessModifier.Private)]
	public float getTotalFuelSeconds()
	{
		float num = 0f;
		for (int i = 0; i < this.fuel.Length; i++)
		{
			if (!this.fuel[i].IsEmpty())
			{
				num += (float)ItemClass.GetFuelValue(this.fuel[i].itemValue) * (float)this.fuel[i].count;
			}
		}
		return num;
	}

	// Token: 0x06005847 RID: 22599 RVA: 0x0023B478 File Offset: 0x00239678
	[PublicizedFrom(EAccessModifier.Private)]
	public float GetFuelTime(ItemStack _fuel)
	{
		if (_fuel.itemValue.type == 0)
		{
			return 0f;
		}
		return (float)ItemClass.GetFuelValue(_fuel.itemValue);
	}

	// Token: 0x170008DA RID: 2266
	// (get) Token: 0x06005848 RID: 22600 RVA: 0x0023B49C File Offset: 0x0023969C
	public bool IsEmpty
	{
		get
		{
			return !this.hasRecipeInQueue() && this.isEmpty(this.fuel) && this.isEmpty(this.tools) && this.isEmpty(this.output) && this.inputIsEmpty();
		}
	}

	// Token: 0x06005849 RID: 22601 RVA: 0x0023B4F0 File Offset: 0x002396F0
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isEmpty(ItemStack[] items)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (!items[i].IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600584A RID: 22602 RVA: 0x0023B51C File Offset: 0x0023971C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool inputIsEmpty()
	{
		int num = this.input.Length - this.materialNames.Length;
		int i;
		for (i = 0; i < num; i++)
		{
			if (!this.input[i].IsEmpty())
			{
				return false;
			}
		}
		while (i < this.input.Length)
		{
			ItemStack itemStack = this.input[i];
			if (itemStack.itemValue.type > 0 && itemStack.count >= 10)
			{
				return false;
			}
			i++;
		}
		return true;
	}

	// Token: 0x0600584B RID: 22603 RVA: 0x0023B590 File Offset: 0x00239790
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasRecipeInQueue()
	{
		for (int i = 0; i < this.queue.Length; i++)
		{
			RecipeQueueItem recipeQueueItem = this.queue[i];
			if (recipeQueueItem != null && recipeQueueItem.Multiplier > 0 && recipeQueueItem.Recipe != null)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600584C RID: 22604 RVA: 0x0023B5D0 File Offset: 0x002397D0
	public bool OutputEmpty()
	{
		for (int i = 0; i < this.output.Length; i++)
		{
			if (!this.output[i].IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600584D RID: 22605 RVA: 0x0023B604 File Offset: 0x00239804
	public void ResetCraftingQueue()
	{
		for (int i = 0; i < this.queue.Length; i++)
		{
			RecipeQueueItem recipeQueueItem = new RecipeQueueItem();
			recipeQueueItem.Recipe = null;
			recipeQueueItem.Multiplier = 0;
			recipeQueueItem.CraftingTimeLeft = 0f;
			recipeQueueItem.OneItemCraftTime = 0f;
			recipeQueueItem.IsCrafting = false;
			recipeQueueItem.Quality = 0;
			recipeQueueItem.StartingEntityId = -1;
			this.queue[i] = recipeQueueItem;
		}
	}

	// Token: 0x0600584E RID: 22606 RVA: 0x0023B66C File Offset: 0x0023986C
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		int version = (int)_br.ReadByte();
		if (_eStreamMode == TileEntity.StreamModeRead.Persistency)
		{
			this.lastTickTime = _br.ReadUInt64();
			this.readItemStackArray(_br, ref this.fuel);
			this.readItemStackArray(_br, ref this.input);
			this.readItemStackArray(_br, ref this.toolsNet);
			this.readItemStackArray(_br, ref this.output);
			this.readRecipeStackArray(_br, version, ref this.queue);
			this.readCraftCompleteData(_br, version);
			if (!this.bUserAccessing)
			{
				this.isBurning = _br.ReadBoolean();
				this.currentBurnTimeLeft = _br.ReadSingle();
				int num = (int)_br.ReadByte();
				for (int i = 0; i < num; i++)
				{
					this.currentMeltTimesLeft[i] = _br.ReadSingle();
				}
			}
			else
			{
				_br.ReadBoolean();
				_br.ReadSingle();
				int num2 = (int)_br.ReadByte();
				for (int j = 0; j < num2; j++)
				{
					_br.ReadSingle();
				}
			}
			this.isPlayerPlaced = _br.ReadBoolean();
			this.readItemStackArray(_br, ref this.lastInput);
		}
		else if (_eStreamMode == TileEntity.StreamModeRead.FromClient)
		{
			this.readItemStackArray(_br, ref this.fuel);
			this.readItemStackArray(_br, ref this.input);
			this.readItemStackArray(_br, ref this.toolsNet);
			this.readItemStackArray(_br, ref this.output);
			this.readRecipeStackArray(_br, version, ref this.queue);
			this.readCraftCompleteData(_br, version);
			this.isBurning = _br.ReadBoolean();
			this.currentBurnTimeLeft = _br.ReadSingle();
			int num3 = (int)_br.ReadByte();
			for (int k = 0; k < num3; k++)
			{
				this.currentMeltTimesLeft[k] = _br.ReadSingle();
			}
			this.isPlayerPlaced = _br.ReadBoolean();
			ulong num4 = _br.ReadUInt64();
			this.lastTickTime = GameTimer.Instance.ticks - num4;
			this.readItemStackArray(_br, ref this.lastInput);
		}
		else if (_eStreamMode == TileEntity.StreamModeRead.FromServer)
		{
			this.readItemStackArray(_br, ref this.fuel);
			this.readItemStackArray(_br, ref this.input);
			this.readItemStackArray(_br, ref this.toolsNet);
			this.readItemStackArray(_br, ref this.output);
			this.readRecipeStackArray(_br, version, ref this.queue);
			this.readCraftCompleteData(_br, version);
			if (!this.bUserAccessing)
			{
				this.isBurning = _br.ReadBoolean();
				this.currentBurnTimeLeft = _br.ReadSingle();
				int num5 = (int)_br.ReadByte();
				for (int l = 0; l < num5; l++)
				{
					this.currentMeltTimesLeft[l] = _br.ReadSingle();
				}
			}
			else
			{
				_br.ReadBoolean();
				_br.ReadSingle();
				int num6 = (int)_br.ReadByte();
				for (int m = 0; m < num6; m++)
				{
					_br.ReadSingle();
				}
			}
			this.isPlayerPlaced = _br.ReadBoolean();
			ulong num7 = _br.ReadUInt64();
			if (!this.bUserAccessing)
			{
				this.lastTickTime = GameTimer.Instance.ticks - num7;
			}
			this.readItemStackArray(_br, ref this.lastInput);
		}
		this.OnSetLocalChunkPosition();
		this.SetDataFromNet();
	}

	// Token: 0x0600584F RID: 22607 RVA: 0x0023B94C File Offset: 0x00239B4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetDataFromNet()
	{
		if (this.bUserAccessing)
		{
			return;
		}
		if (!this.IsToolsSame(this.toolsNet))
		{
			this.tools = ItemStack.Clone(this.toolsNet);
			this.visibleChanged = true;
		}
		this.UpdateVisible();
	}

	// Token: 0x06005850 RID: 22608 RVA: 0x0023B984 File Offset: 0x00239B84
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(49);
		if (_eStreamMode == TileEntity.StreamModeWrite.Persistency)
		{
			_bw.Write(this.lastTickTime);
			this.writeItemStackArray(_bw, this.fuel);
			this.writeItemStackArray(_bw, this.input);
			this.writeItemStackArray(_bw, this.tools);
			this.writeItemStackArray(_bw, this.output);
			this.writeRecipeStackArray(_bw, 49);
			this.writeCraftCompleteData(_bw, 49);
			_bw.Write(this.isBurning);
			_bw.Write(this.currentBurnTimeLeft);
			int num = this.currentMeltTimesLeft.Length;
			_bw.Write((byte)num);
			for (int i = 0; i < num; i++)
			{
				_bw.Write(this.currentMeltTimesLeft[i]);
			}
			_bw.Write(this.isPlayerPlaced);
			this.writeItemStackArray(_bw, this.lastInput);
			return;
		}
		if (_eStreamMode == TileEntity.StreamModeWrite.ToServer)
		{
			this.writeItemStackArray(_bw, this.fuel);
			this.writeItemStackArray(_bw, this.input);
			this.writeItemStackArray(_bw, this.tools);
			this.writeItemStackArray(_bw, this.output);
			this.writeRecipeStackArray(_bw, 49);
			this.writeCraftCompleteData(_bw, 49);
			_bw.Write(this.isBurning);
			_bw.Write(this.currentBurnTimeLeft);
			int num2 = this.currentMeltTimesLeft.Length;
			_bw.Write((byte)num2);
			for (int j = 0; j < num2; j++)
			{
				_bw.Write(this.currentMeltTimesLeft[j]);
			}
			_bw.Write(this.isPlayerPlaced);
			_bw.Write(GameTimer.Instance.ticks - this.lastTickTime);
			this.writeItemStackArray(_bw, this.lastInput);
			return;
		}
		if (_eStreamMode == TileEntity.StreamModeWrite.ToClient)
		{
			this.writeItemStackArray(_bw, this.fuel);
			this.writeItemStackArray(_bw, this.input);
			this.writeItemStackArray(_bw, this.tools);
			this.writeItemStackArray(_bw, this.output);
			this.writeRecipeStackArray(_bw, 49);
			this.writeCraftCompleteData(_bw, 49);
			_bw.Write(this.isBurning);
			_bw.Write(this.currentBurnTimeLeft);
			int num3 = this.currentMeltTimesLeft.Length;
			_bw.Write((byte)num3);
			for (int k = 0; k < num3; k++)
			{
				_bw.Write(this.currentMeltTimesLeft[k]);
			}
			_bw.Write(this.isPlayerPlaced);
			_bw.Write(GameTimer.Instance.ticks - this.lastTickTime);
			this.writeItemStackArray(_bw, this.lastInput);
		}
	}

	// Token: 0x06005851 RID: 22609 RVA: 0x0023BBE0 File Offset: 0x00239DE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void readItemStackArray(BinaryReader _br, ref ItemStack[] stack, ref ItemStack[] lastStack)
	{
		int num = (int)_br.ReadByte();
		if (stack == null || stack.Length != num)
		{
			stack = ItemStack.CreateArray(num);
		}
		if (!base.bWaitingForServerResponse)
		{
			for (int i = 0; i < num; i++)
			{
				stack[i].Read(_br);
			}
			lastStack = ItemStack.Clone(stack);
			return;
		}
		ItemStack itemStack = ItemStack.Empty.Clone();
		for (int j = 0; j < num; j++)
		{
			itemStack.Read(_br);
		}
	}

	// Token: 0x06005852 RID: 22610 RVA: 0x0023BC50 File Offset: 0x00239E50
	[PublicizedFrom(EAccessModifier.Private)]
	public void readItemStackArray(BinaryReader _br, ref ItemStack[] stack)
	{
		int num = (int)_br.ReadByte();
		if (stack == null || stack.Length != num)
		{
			stack = ItemStack.CreateArray(num);
		}
		if (!this.bUserAccessing)
		{
			for (int i = 0; i < num; i++)
			{
				stack[i].Read(_br);
			}
			return;
		}
		ItemStack itemStack = ItemStack.Empty.Clone();
		for (int j = 0; j < num; j++)
		{
			itemStack.Read(_br);
		}
	}

	// Token: 0x06005853 RID: 22611 RVA: 0x0023BCB8 File Offset: 0x00239EB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void readItemStackArrayDelta(BinaryReader _br, ref ItemStack[] stack)
	{
		int num = (int)_br.ReadByte();
		if (stack == null || stack.Length != num)
		{
			stack = ItemStack.CreateArray(num);
		}
		for (int i = 0; i < num; i++)
		{
			stack[i].ReadDelta(_br, stack[i]);
		}
	}

	// Token: 0x06005854 RID: 22612 RVA: 0x0023BCFC File Offset: 0x00239EFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void writeItemStackArray(BinaryWriter bw, ItemStack[] stack)
	{
		byte value = (stack != null) ? ((byte)stack.Length) : 0;
		bw.Write(value);
		if (stack == null)
		{
			return;
		}
		for (int i = 0; i < stack.Length; i++)
		{
			stack[i].Write(bw);
		}
	}

	// Token: 0x06005855 RID: 22613 RVA: 0x0023BD38 File Offset: 0x00239F38
	[PublicizedFrom(EAccessModifier.Private)]
	public void writeItemStackArrayDelta(BinaryWriter bw, ItemStack[] stack, ItemStack[] lastStack)
	{
		byte value = (stack != null) ? ((byte)stack.Length) : 0;
		bw.Write(value);
		if (stack == null)
		{
			return;
		}
		for (int i = 0; i < stack.Length; i++)
		{
			stack[i].WriteDelta(bw, (lastStack != null) ? lastStack[i] : ItemStack.Empty.Clone());
		}
	}

	// Token: 0x06005856 RID: 22614 RVA: 0x0023BD84 File Offset: 0x00239F84
	[PublicizedFrom(EAccessModifier.Private)]
	public void readRecipeStackArrayDelta(BinaryReader _br, ref RecipeQueueItem[] queueStack)
	{
		int num = (int)_br.ReadByte();
		if (queueStack == null || queueStack.Length != num)
		{
			queueStack = new RecipeQueueItem[num];
		}
		for (int i = 0; i < num; i++)
		{
			queueStack[i].ReadDelta(_br, queueStack[i]);
		}
	}

	// Token: 0x06005857 RID: 22615 RVA: 0x0023BDC8 File Offset: 0x00239FC8
	public void writeRecipeStackArrayDelta(BinaryWriter bw, RecipeQueueItem[] queueStack, RecipeQueueItem[] lastQueueStack)
	{
		byte value = (queueStack != null) ? ((byte)queueStack.Length) : 0;
		bw.Write(value);
		if (queueStack == null)
		{
			return;
		}
		for (int i = 0; i < queueStack.Length; i++)
		{
			queueStack[i].WriteDelta(bw, (lastQueueStack != null) ? lastQueueStack[i] : new RecipeQueueItem());
		}
	}

	// Token: 0x06005858 RID: 22616 RVA: 0x0023BE10 File Offset: 0x0023A010
	[PublicizedFrom(EAccessModifier.Private)]
	public void readCraftCompleteData(BinaryReader _br, int version)
	{
		if (version <= 45)
		{
			return;
		}
		int num = (int)_br.ReadInt16();
		if (this.CraftCompleteList == null)
		{
			this.CraftCompleteList = new List<CraftCompleteData>();
		}
		this.CraftCompleteList.Clear();
		for (int i = 0; i < num; i++)
		{
			CraftCompleteData craftCompleteData = new CraftCompleteData();
			craftCompleteData.Read(_br, version);
			this.CraftCompleteList.Add(craftCompleteData);
		}
	}

	// Token: 0x06005859 RID: 22617 RVA: 0x0023BE70 File Offset: 0x0023A070
	public void writeCraftCompleteData(BinaryWriter _bw, int version)
	{
		short value = (this.CraftCompleteList != null) ? ((short)this.CraftCompleteList.Count) : 0;
		_bw.Write(value);
		if (this.CraftCompleteList != null)
		{
			for (int i = 0; i < this.CraftCompleteList.Count; i++)
			{
				this.CraftCompleteList[i].Write(_bw, version);
			}
		}
	}

	// Token: 0x0600585A RID: 22618 RVA: 0x0023BED0 File Offset: 0x0023A0D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void readRecipeStackArray(BinaryReader _br, int version, ref RecipeQueueItem[] queueStack)
	{
		int num = (int)_br.ReadByte();
		if (queueStack == null || queueStack.Length != num)
		{
			queueStack = new RecipeQueueItem[num];
		}
		if (!base.bWaitingForServerResponse)
		{
			for (int i = 0; i < num; i++)
			{
				if (queueStack[i] == null)
				{
					queueStack[i] = new RecipeQueueItem();
				}
				queueStack[i].Read(_br, (uint)version);
			}
			return;
		}
		RecipeQueueItem recipeQueueItem = new RecipeQueueItem();
		for (int j = 0; j < num; j++)
		{
			recipeQueueItem.Read(_br, (uint)version);
		}
	}

	// Token: 0x0600585B RID: 22619 RVA: 0x0023BF40 File Offset: 0x0023A140
	public void writeRecipeStackArray(BinaryWriter _bw, int version)
	{
		byte value = (this.queue != null) ? ((byte)this.queue.Length) : 0;
		_bw.Write(value);
		if (this.queue == null)
		{
			return;
		}
		for (int i = 0; i < this.queue.Length; i++)
		{
			if (this.queue[i] != null)
			{
				this.queue[i].Write(_bw, (uint)version);
			}
			else
			{
				RecipeQueueItem recipeQueueItem = new RecipeQueueItem();
				recipeQueueItem.Multiplier = 0;
				recipeQueueItem.Recipe = null;
				recipeQueueItem.IsCrafting = false;
				this.queue[i] = recipeQueueItem;
				this.queue[i].Write(_bw, (uint)version);
			}
		}
	}

	// Token: 0x0600585C RID: 22620 RVA: 0x0023BFD4 File Offset: 0x0023A1D4
	public void AddCraftComplete(int crafterEntityID, ItemValue itemCrafted, string recipeName, string itemScrapped, int craftExpGain, int craftedCount)
	{
		if (this.CraftCompleteList == null)
		{
			this.CraftCompleteList = new List<CraftCompleteData>();
		}
		for (int i = 0; i < this.CraftCompleteList.Count; i++)
		{
			if (this.CraftCompleteList[i].CraftedItemStack.itemValue.GetItemId() == itemCrafted.GetItemId() && this.CraftCompleteList[i].ItemScrapped == itemScrapped)
			{
				this.CraftCompleteList[i].CraftedItemStack.count += craftedCount;
				this.setModified();
				return;
			}
		}
		this.CraftCompleteList.Add(new CraftCompleteData(crafterEntityID, new ItemStack(itemCrafted, craftedCount), recipeName, itemScrapped, craftExpGain, 1));
		this.setModified();
	}

	// Token: 0x0600585D RID: 22621 RVA: 0x0023C094 File Offset: 0x0023A294
	public void CheckForCraftComplete(EntityPlayerLocal player)
	{
		if (this.CraftCompleteList == null)
		{
			return;
		}
		bool flag = false;
		for (int i = this.CraftCompleteList.Count - 1; i >= 0; i--)
		{
			if (this.CraftCompleteList[i].CrafterEntityID == player.entityId)
			{
				player.equipment.UnlockCosmeticItem(ItemClass.GetItemClass(this.CraftCompleteList[i].ItemScrapped, false));
				player.GiveExp(this.CraftCompleteList[i]);
				this.CraftCompleteList.RemoveAt(i);
				flag = true;
			}
		}
		if (flag)
		{
			this.setModified();
		}
	}

	// Token: 0x0600585E RID: 22622 RVA: 0x0023C128 File Offset: 0x0023A328
	public override void ReplacedBy(BlockValue _bvOld, BlockValue _bvNew, TileEntity _teNew)
	{
		base.ReplacedBy(_bvOld, _bvNew, _teNew);
		TileEntityWorkstation tileEntityWorkstation;
		if (_teNew.TryGetSelfOrFeature(out tileEntityWorkstation))
		{
			return;
		}
		List<ItemStack> list = new List<ItemStack>();
		if (this.fuel != null)
		{
			list.AddRange(this.fuel);
		}
		if (this.input != null)
		{
			for (int i = 0; i < 3; i++)
			{
				if (!this.input[i].IsEmpty())
				{
					list.Add(this.input[i]);
				}
			}
			List<Recipe> allRecipes = CraftingManager.GetAllRecipes();
			for (int j = 0; j < this.materialNames.Length; j++)
			{
				int num = j + 3;
				ItemClass itemClass = ItemClass.GetItemClass("unit_" + this.materialNames[j], false);
				if (itemClass != null && itemClass.MadeOfMaterial.ForgeCategory != null)
				{
					ItemStack itemStack = this.input[num];
					if (itemStack.itemValue.type == 0)
					{
						this.input[num] = new ItemStack(new ItemValue(itemClass.Id, false), itemStack.count);
					}
					Recipe recipe = null;
					foreach (Recipe recipe2 in allRecipes)
					{
						if (recipe2.ingredients.Count == 1 && recipe2.ingredients[0].itemValue.type == itemClass.Id && (!recipe2.UseIngredientModifier || recipe == null))
						{
							recipe = recipe2;
						}
					}
					if (recipe == null)
					{
						Log.Warning("No craft out recipe found for workstation input " + itemClass.GetItemName());
					}
					else
					{
						int k = itemStack.count / recipe.ingredients[0].count;
						ItemValue itemValue = new ItemValue(recipe.itemValueType, false);
						int value = itemValue.ItemClass.Stacknumber.Value;
						while (k > 0)
						{
							int num2 = Mathf.Min(k, value);
							list.Add(new ItemStack(itemValue, num2));
							k -= num2;
						}
					}
				}
			}
		}
		if (this.tools != null)
		{
			list.AddRange(this.tools);
		}
		if (this.output != null)
		{
			list.AddRange(this.output);
		}
		Vector3 pos = base.ToWorldCenterPos();
		pos.y += 0.9f;
		GameManager.Instance.DropContentInLootContainerServer(-1, "DroppedLootContainer", pos, list.ToArray(), true);
	}

	// Token: 0x0600585F RID: 22623 RVA: 0x001666F0 File Offset: 0x001648F0
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Workstation;
	}

	// Token: 0x0400437E RID: 17278
	public const int ChangedFuel = 1;

	// Token: 0x0400437F RID: 17279
	public const int ChangedInput = 2;

	// Token: 0x04004380 RID: 17280
	public const int OutputItemAdded = 4;

	// Token: 0x04004381 RID: 17281
	public const int Version = 49;

	// Token: 0x04004384 RID: 17284
	public const int cInputSlotCount = 3;

	// Token: 0x04004385 RID: 17285
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMinInternalMatCount = 10;

	// Token: 0x04004386 RID: 17286
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] fuel;

	// Token: 0x04004387 RID: 17287
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] input;

	// Token: 0x04004388 RID: 17288
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] tools;

	// Token: 0x04004389 RID: 17289
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] toolsNet;

	// Token: 0x0400438A RID: 17290
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] output;

	// Token: 0x0400438B RID: 17291
	[PublicizedFrom(EAccessModifier.Private)]
	public RecipeQueueItem[] queue;

	// Token: 0x0400438C RID: 17292
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong lastTickTime;

	// Token: 0x0400438D RID: 17293
	[PublicizedFrom(EAccessModifier.Private)]
	public float currentBurnTimeLeft;

	// Token: 0x0400438E RID: 17294
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] currentMeltTimesLeft;

	// Token: 0x0400438F RID: 17295
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] lastInput;

	// Token: 0x04004390 RID: 17296
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBurning;

	// Token: 0x04004391 RID: 17297
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBesideWater;

	// Token: 0x04004392 RID: 17298
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPlayerPlaced;

	// Token: 0x04004393 RID: 17299
	[PublicizedFrom(EAccessModifier.Private)]
	public bool visibleChanged;

	// Token: 0x04004394 RID: 17300
	[PublicizedFrom(EAccessModifier.Private)]
	public bool visibleCrafting;

	// Token: 0x04004395 RID: 17301
	[PublicizedFrom(EAccessModifier.Private)]
	public bool visibleWorking;

	// Token: 0x04004396 RID: 17302
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] materialNames;

	// Token: 0x04004397 RID: 17303
	[PublicizedFrom(EAccessModifier.Private)]
	public List<CraftCompleteData> CraftCompleteList;

	// Token: 0x04004398 RID: 17304
	[PublicizedFrom(EAccessModifier.Private)]
	public bool[] isModuleUsed;

	// Token: 0x02000B17 RID: 2839
	[PublicizedFrom(EAccessModifier.Private)]
	public enum Module
	{
		// Token: 0x0400439A RID: 17306
		Tools,
		// Token: 0x0400439B RID: 17307
		Input,
		// Token: 0x0400439C RID: 17308
		Output,
		// Token: 0x0400439D RID: 17309
		Fuel,
		// Token: 0x0400439E RID: 17310
		Material_Input,
		// Token: 0x0400439F RID: 17311
		Count
	}
}

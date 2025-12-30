using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015EB RID: 5611
	[Preserve]
	public class ChallengeObjectiveCraft : BaseChallengeObjective
	{
		// Token: 0x1700134F RID: 4943
		// (get) Token: 0x0600AC65 RID: 44133 RVA: 0x00075CC0 File Offset: 0x00073EC0
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Craft;
			}
		}

		// Token: 0x17001350 RID: 4944
		// (get) Token: 0x0600AC66 RID: 44134 RVA: 0x0043A7C3 File Offset: 0x004389C3
		public override string DescriptionText
		{
			get
			{
				return Localization.Get("lblContextActionCraft", false) + " " + Localization.Get(this.itemClassID, false) + ":";
			}
		}

		// Token: 0x17001351 RID: 4945
		// (get) Token: 0x0600AC67 RID: 44135 RVA: 0x0043A7EB File Offset: 0x004389EB
		public override ChallengeClass.UINavTypes NavType
		{
			get
			{
				if (this.itemRecipe == null)
				{
					return ChallengeClass.UINavTypes.None;
				}
				if (!(this.itemRecipe.craftingArea == ""))
				{
					return ChallengeClass.UINavTypes.None;
				}
				return ChallengeClass.UINavTypes.Crafting;
			}
		}

		// Token: 0x0600AC68 RID: 44136 RVA: 0x0043A811 File Offset: 0x00438A11
		public override void Init()
		{
			this.expectedItem = ItemClass.GetItem(this.itemClassID, false);
			this.expectedItemClass = ItemClass.GetItemClass(this.itemClassID, false);
			this.itemRecipe = CraftingManager.GetRecipe(this.itemClassID);
		}

		// Token: 0x0600AC69 RID: 44137 RVA: 0x0043A848 File Offset: 0x00438A48
		public override void HandleOnCreated()
		{
			base.HandleOnCreated();
			this.CreateRequirements();
		}

		// Token: 0x0600AC6A RID: 44138 RVA: 0x0043A856 File Offset: 0x00438A56
		[PublicizedFrom(EAccessModifier.Private)]
		public void CreateRequirements()
		{
			if (this.itemClassIDs.Length > 1)
			{
				return;
			}
			if (!this.ShowRequirements)
			{
				return;
			}
			this.Owner.SetRequirementGroup(new RequirementObjectiveGroupGatherIngredients(this.itemClassID)
			{
				CraftObj = this
			});
		}

		// Token: 0x0600AC6B RID: 44139 RVA: 0x0043A88A File Offset: 0x00438A8A
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.CraftItem -= this.Current_CraftItem;
			QuestEventManager.Current.CraftItem += this.Current_CraftItem;
		}

		// Token: 0x0600AC6C RID: 44140 RVA: 0x0043A8B8 File Offset: 0x00438AB8
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.CraftItem -= this.Current_CraftItem;
		}

		// Token: 0x0600AC6D RID: 44141 RVA: 0x0043A8D0 File Offset: 0x00438AD0
		public override void HandleTrackingStarted()
		{
			base.HandleTrackingStarted();
		}

		// Token: 0x0600AC6E RID: 44142 RVA: 0x0043A8D8 File Offset: 0x00438AD8
		public override void HandleTrackingEnded()
		{
			base.HandleTrackingEnded();
		}

		// Token: 0x0600AC6F RID: 44143 RVA: 0x0043A8E0 File Offset: 0x00438AE0
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_CraftItem(ItemStack stack)
		{
			if (this.CheckBaseRequirements())
			{
				return;
			}
			ItemClass itemClass = stack.itemValue.ItemClass;
			if (itemClass != null && this.itemClassIDs.ContainsCaseInsensitive(itemClass.Name))
			{
				base.Current += stack.count;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600AC70 RID: 44144 RVA: 0x0043A933 File Offset: 0x00438B33
		public override bool CheckObjectiveComplete(bool handleComplete = true)
		{
			if (this.IsRequirement && this.CheckForNeededItem())
			{
				base.Complete = true;
				this.HandleRecipeListUpdate();
				return true;
			}
			base.Complete = false;
			this.HandleRecipeListUpdate();
			return base.CheckObjectiveComplete(handleComplete);
		}

		// Token: 0x0600AC71 RID: 44145 RVA: 0x0043A968 File Offset: 0x00438B68
		public override Recipe GetRecipeItem()
		{
			return this.itemRecipe;
		}

		// Token: 0x0600AC72 RID: 44146 RVA: 0x0043A970 File Offset: 0x00438B70
		public override Recipe[] GetRecipeItems()
		{
			Recipe recipeFromRequirements = this.Owner.GetRecipeFromRequirements();
			if (recipeFromRequirements != null)
			{
				return new Recipe[]
				{
					recipeFromRequirements,
					this.itemRecipe
				};
			}
			return new Recipe[]
			{
				this.itemRecipe
			};
		}

		// Token: 0x0600AC73 RID: 44147 RVA: 0x0043A9AF File Offset: 0x00438BAF
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("item"))
			{
				this.SetupItem(e.GetAttribute("item"));
			}
		}

		// Token: 0x0600AC74 RID: 44148 RVA: 0x0043A9E0 File Offset: 0x00438BE0
		public void SetupItem(string itemID)
		{
			this.itemClassID = itemID;
			if (this.itemClassID.Contains(','))
			{
				this.itemClassIDs = this.itemClassID.Split(',', StringSplitOptions.None);
				this.itemClassID = this.itemClassIDs[0];
				return;
			}
			this.itemClassIDs = new string[1];
			this.itemClassIDs[0] = this.itemClassID;
		}

		// Token: 0x0600AC75 RID: 44149 RVA: 0x0043AA40 File Offset: 0x00438C40
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckForNeededItem()
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			XUiM_PlayerInventory playerInventory = uiforPlayer.xui.PlayerInventory;
			ItemValue itemValue = new ItemValue(this.itemRecipe.itemValueType, false);
			int num = playerInventory.Backpack.GetItemCount(itemValue, -1, -1, true);
			num += playerInventory.Toolbelt.GetItemCount(itemValue, false, -1, -1, true);
			ItemStack currentStack = uiforPlayer.xui.dragAndDrop.CurrentStack;
			if (!currentStack.IsEmpty() && currentStack.itemValue.type == this.itemRecipe.itemValueType)
			{
				num += currentStack.count;
			}
			base.Current = num;
			return num >= this.MaxCount;
		}

		// Token: 0x0600AC76 RID: 44150 RVA: 0x0043AAF0 File Offset: 0x00438CF0
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleRecipeListUpdate()
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Owner.Player);
			if (uiforPlayer.xui.QuestTracker.TrackedChallenge == this.Owner)
			{
				uiforPlayer.xui.QuestTracker.HandleTrackedChallengeChanged();
			}
		}

		// Token: 0x0600AC77 RID: 44151 RVA: 0x0043AB3C File Offset: 0x00438D3C
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveCraft
			{
				itemClassIDs = this.itemClassIDs,
				itemClassID = this.itemClassID,
				itemRecipe = this.itemRecipe,
				expectedItem = this.expectedItem,
				expectedItemClass = this.expectedItemClass
			};
		}

		// Token: 0x0600AC78 RID: 44152 RVA: 0x00439BCB File Offset: 0x00437DCB
		public override void CompleteObjective(bool handleComplete = true)
		{
			base.Current = this.MaxCount;
			base.Complete = true;
			if (handleComplete)
			{
				this.Owner.HandleComplete(true);
			}
		}

		// Token: 0x0400865D RID: 34397
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemValue expectedItem = ItemValue.None.Clone();

		// Token: 0x0400865E RID: 34398
		[PublicizedFrom(EAccessModifier.Private)]
		public ItemClass expectedItemClass;

		// Token: 0x0400865F RID: 34399
		public string[] itemClassIDs;

		// Token: 0x04008660 RID: 34400
		public string itemClassID = "";

		// Token: 0x04008661 RID: 34401
		public Recipe itemRecipe;
	}
}

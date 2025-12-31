using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Twitch
{
	// Token: 0x0200151D RID: 5405
	public class TwitchAction
	{
		// Token: 0x0600A6D8 RID: 42712 RVA: 0x0041DC7D File Offset: 0x0041BE7D
		public bool HasModifiedPrice()
		{
			return this.DefaultCost != this.ModifiedCost;
		}

		// Token: 0x0600A6D9 RID: 42713 RVA: 0x0041DC90 File Offset: 0x0041BE90
		public bool HasExtraConditions()
		{
			return ((!this.SingleDayUse && !this.RandomDaily) || this.AllowedDay != -1) && !this.OnCooldown && this.OnlyUsableByType == TwitchAction.OnlyUsableTypes.Everyone;
		}

		// Token: 0x0600A6DA RID: 42714 RVA: 0x0041DCC0 File Offset: 0x0041BEC0
		public static int GetAdjustedBitPriceCeil(int price)
		{
			int num = TwitchAction.bitPrices.Find((int p) => p >= price);
			if (num == 0)
			{
				return TwitchAction.bitPrices[TwitchAction.bitPrices.Count - 1];
			}
			return num;
		}

		// Token: 0x0600A6DB RID: 42715 RVA: 0x0041DD0C File Offset: 0x0041BF0C
		public static int GetAdjustedBitPriceFloor(int price)
		{
			return TwitchAction.bitPrices.FindLast((int p) => p <= price);
		}

		// Token: 0x0600A6DC RID: 42716 RVA: 0x0041DD3C File Offset: 0x0041BF3C
		public static int GetAdjustedBitPriceFloorNoZero(int price)
		{
			return Mathf.Max(TwitchAction.GetAdjustedBitPriceFloor(price), TwitchAction.bitPrices[0]);
		}

		// Token: 0x0600A6DD RID: 42717 RVA: 0x0041DD54 File Offset: 0x0041BF54
		public int GetModifiedDiscountCost()
		{
			return TwitchAction.GetAdjustedBitPriceFloorNoZero((int)((float)this.ModifiedCost * TwitchManager.Current.BitPriceMultiplier));
		}

		// Token: 0x0600A6DE RID: 42718 RVA: 0x0041DD70 File Offset: 0x0041BF70
		public void DecreaseCost()
		{
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				this.ModifiedCost = TwitchAction.bitPrices[(int)MathUtils.Clamp((float)(TwitchAction.bitPrices.IndexOf(this.ModifiedCost) - 1), 0f, (float)(TwitchAction.bitPrices.Count - 1))];
				return;
			}
			if (this.ModifiedCost > 25)
			{
				this.ModifiedCost -= 25;
			}
		}

		// Token: 0x0600A6DF RID: 42719 RVA: 0x0041DDDC File Offset: 0x0041BFDC
		public void IncreaseCost()
		{
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				this.ModifiedCost = TwitchAction.bitPrices[(int)MathUtils.Clamp((float)(TwitchAction.bitPrices.IndexOf(this.ModifiedCost) + 1), 0f, (float)(TwitchAction.bitPrices.Count - 1))];
				return;
			}
			if (this.ModifiedCost < 2000)
			{
				this.ModifiedCost += 25;
			}
		}

		// Token: 0x0600A6E0 RID: 42720 RVA: 0x0041DE4A File Offset: 0x0041C04A
		public void ResetToDefaultCost()
		{
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				this.ModifiedCost = TwitchAction.GetAdjustedBitPriceFloorNoZero(this.DefaultCost);
				return;
			}
			this.ModifiedCost = this.DefaultCost;
		}

		// Token: 0x17001246 RID: 4678
		// (get) Token: 0x0600A6E1 RID: 42721 RVA: 0x0041DE73 File Offset: 0x0041C073
		public bool CanUse
		{
			get
			{
				return this.Enabled;
			}
		}

		// Token: 0x17001247 RID: 4679
		// (get) Token: 0x0600A6E2 RID: 42722 RVA: 0x0041DE7B File Offset: 0x0041C07B
		public bool RandomDaily
		{
			get
			{
				return this.RandomGroup != "";
			}
		}

		// Token: 0x17001248 RID: 4680
		// (get) Token: 0x0600A6E3 RID: 42723 RVA: 0x0041DE8D File Offset: 0x0041C08D
		public bool SpecialOnly
		{
			get
			{
				return this.PointType == TwitchAction.PointTypes.SP;
			}
		}

		// Token: 0x0600A6E4 RID: 42724 RVA: 0x0041DE98 File Offset: 0x0041C098
		public bool CheckUsable(TwitchIRCClient.TwitchChatMessage message)
		{
			switch (this.OnlyUsableByType)
			{
			case TwitchAction.OnlyUsableTypes.Broadcaster:
				return message.isBroadcaster;
			case TwitchAction.OnlyUsableTypes.Mods:
				return message.isMod;
			case TwitchAction.OnlyUsableTypes.VIPs:
				return message.isVIP;
			case TwitchAction.OnlyUsableTypes.Subs:
				return message.isSub;
			case TwitchAction.OnlyUsableTypes.Name:
				return this.OnlyUsableBy.ContainsCaseInsensitive(message.UserName);
			default:
				return true;
			}
		}

		// Token: 0x0600A6E5 RID: 42725 RVA: 0x0041DEFC File Offset: 0x0041C0FC
		public void Init()
		{
			if (this.CategoryNames.Count > 0)
			{
				this.MainCategory = TwitchActionManager.Current.GetCategory(this.CategoryNames[this.CategoryNames.Count - 1]);
				if (this.DisplayCategory == null)
				{
					this.DisplayCategory = this.MainCategory;
				}
			}
			this.OnInit();
		}

		// Token: 0x0600A6E6 RID: 42726 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600A6E7 RID: 42727 RVA: 0x0041DF59 File Offset: 0x0041C159
		public virtual TwitchActionEntry SetupActionEntry()
		{
			return new TwitchActionEntry();
		}

		// Token: 0x0600A6E8 RID: 42728 RVA: 0x0041DF60 File Offset: 0x0041C160
		public bool IsInPreset(TwitchActionPreset preset)
		{
			return ((!preset.IsEmpty && this.PresetNames != null && this.PresetNames.Contains(preset.Name)) || preset.AddedActions.Contains(this.Name)) && !preset.RemovedActions.Contains(this.Name);
		}

		// Token: 0x0600A6E9 RID: 42729 RVA: 0x0041DFB9 File Offset: 0x0041C1B9
		public bool IsInPresetForList(TwitchActionPreset preset)
		{
			return (!preset.IsEmpty && this.PresetNames != null && this.PresetNames.Contains(preset.Name)) || preset.AddedActions.Contains(this.Name);
		}

		// Token: 0x0600A6EA RID: 42730 RVA: 0x0041DFF1 File Offset: 0x0041C1F1
		public bool IsInPresetDefault(TwitchActionPreset preset)
		{
			return !preset.IsEmpty && this.PresetNames != null && this.PresetNames.Contains(preset.Name);
		}

		// Token: 0x0600A6EB RID: 42731 RVA: 0x0041E018 File Offset: 0x0041C218
		public bool CanPerformAction(EntityPlayer target, TwitchActionEntry entry)
		{
			if (entry.Target == null)
			{
				entry.Target = target;
			}
			return this.OnPerformAction(target, entry);
		}

		// Token: 0x0600A6EC RID: 42732 RVA: 0x0041E03C File Offset: 0x0041C23C
		public void SetQueued()
		{
			this.lastUse = Time.time;
			if (this.CooldownAdditions != null)
			{
				float actionCooldownModifier = TwitchManager.Current.ActionCooldownModifier;
				for (int i = 0; i < this.CooldownAdditions.Count; i++)
				{
					TwitchActionCooldownAddition twitchActionCooldownAddition = this.CooldownAdditions[i];
					if (twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchActions.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchAction twitchAction = TwitchActionManager.TwitchActions[twitchActionCooldownAddition.ActionName];
						twitchAction.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchAction.tempCooldownSet = Time.time;
					}
					else if (!twitchActionCooldownAddition.IsAction && TwitchActionManager.TwitchVotes.ContainsKey(twitchActionCooldownAddition.ActionName))
					{
						TwitchVote twitchVote = TwitchActionManager.TwitchVotes[twitchActionCooldownAddition.ActionName];
						twitchVote.tempCooldown = twitchActionCooldownAddition.CooldownTime * actionCooldownModifier;
						twitchVote.tempCooldownSet = Time.time;
					}
				}
			}
			if (this.VoteCooldownAddition != 0f)
			{
				TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
				votingManager.VoteStartDelayTimeRemaining = Math.Max(this.VoteCooldownAddition, votingManager.VoteStartDelayTimeRemaining);
			}
			if (this.SingleDayUse)
			{
				this.AllowedDay = -1;
			}
		}

		// Token: 0x0600A6ED RID: 42733 RVA: 0x0041E158 File Offset: 0x0041C358
		public virtual bool ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			properties.ParseString(TwitchAction.PropTitle, ref this.Title);
			if (properties.Values.ContainsKey(TwitchAction.PropTitleKey))
			{
				this.Title = Localization.Get(properties.Values[TwitchAction.PropTitleKey], false);
			}
			if (properties.Values.ContainsKey(TwitchAction.PropCommand))
			{
				this.BaseCommand = (this.Command = properties.Values[TwitchAction.PropCommand].ToLower());
				if (!Regex.IsMatch(this.Command, "^#[a-zA-Z0-9]+(_[a-zA-Z0-9]+)*$"))
				{
					return false;
				}
			}
			if (properties.Values.ContainsKey(TwitchAction.PropCommandKey))
			{
				this.Command = Localization.Get(properties.Values[TwitchAction.PropCommandKey], false).ToLower();
				if (Localization.LocalizationChecks)
				{
					if (this.Command.StartsWith("l_"))
					{
						this.Command = this.Command.Substring(3).Insert(0, "#l_");
					}
					else if (this.Command.StartsWith("ul_"))
					{
						this.Command = this.Command.Substring(3).Insert(0, "#ul_");
					}
					else if (this.Command.StartsWith("le_"))
					{
						this.Command = this.Command.Substring(3).Insert(0, "#le_");
					}
				}
				if (!Regex.IsMatch(this.Command, "^#[\\p{L}\\p{N}]+([-_][\\p{L}\\p{N}]+)*$"))
				{
					return false;
				}
			}
			if (properties.Values.ContainsKey(TwitchAction.PropCategory))
			{
				this.CategoryNames.AddRange(properties.Values[TwitchAction.PropCategory].Split(',', StringSplitOptions.None));
			}
			if (properties.Values.ContainsKey(TwitchAction.PropDisplayCategory))
			{
				this.DisplayCategory = TwitchActionManager.Current.GetCategory(properties.Values[TwitchAction.PropDisplayCategory]);
			}
			properties.ParseString(TwitchAction.PropEventName, ref this.EventName);
			properties.ParseString(TwitchAction.PropDescription, ref this.Description);
			if (properties.Values.ContainsKey(TwitchAction.PropDescriptionKey))
			{
				this.Description = Localization.Get(properties.Values[TwitchAction.PropDescriptionKey], false);
			}
			properties.ParseInt(TwitchAction.PropDefaultCost, ref this.DefaultCost);
			properties.ParseInt(TwitchAction.PropStartGameStage, ref this.StartGameStage);
			properties.ParseBool(TwitchAction.PropIsPositive, ref this.IsPositive);
			bool flag = false;
			properties.ParseBool(TwitchAction.PropSpecialOnly, ref flag);
			if (flag)
			{
				this.PointType = TwitchAction.PointTypes.SP;
			}
			properties.ParseBool(TwitchAction.PropAddCooldown, ref this.AddsToCooldown);
			properties.ParseInt(TwitchAction.PropCooldownAddAmount, ref this.CooldownAddAmount);
			properties.ParseBool(TwitchAction.PropCooldownBlocked, ref this.CooldownBlocked);
			properties.ParseBool(TwitchAction.PropWaitingBlocked, ref this.WaitingBlocked);
			if (properties.Values.ContainsKey(TwitchAction.PropCooldown))
			{
				this.OriginalCooldown = (this.Cooldown = StringParsers.ParseFloat(properties.Values[TwitchAction.PropCooldown], 0, -1, NumberStyles.Any));
				this.lastUse = Time.time - this.Cooldown;
				this.ModifiedCooldown = this.Cooldown;
			}
			properties.ParseBool(TwitchAction.PropEnabled, ref this.Enabled);
			this.OriginalEnabled = this.Enabled;
			properties.ParseBool(TwitchAction.PropSingleDayUse, ref this.SingleDayUse);
			properties.ParseString(TwitchAction.PropRandomGroup, ref this.RandomGroup);
			properties.ParseBool(TwitchAction.PropShowInActionList, ref this.ShowInActionList);
			if (properties.Values.ContainsKey(TwitchAction.PropSpecialRequirement))
			{
				string[] array = this.Properties.Values[TwitchAction.PropSpecialRequirement].Split(',', StringSplitOptions.None);
				this.SpecialRequirementList = new List<TwitchAction.SpecialRequirements>();
				foreach (string text in array)
				{
					TwitchAction.SpecialRequirements item = TwitchAction.SpecialRequirements.None;
					if (Enum.TryParse<TwitchAction.SpecialRequirements>(text, true, out item))
					{
						this.SpecialRequirementList.Add(item);
					}
					else
					{
						Log.Error("TwitchAction " + this.Title + " has unknown ShapeCategory " + text);
					}
				}
			}
			properties.ParseString(TwitchAction.PropReplaces, ref this.Replaces);
			properties.ParseBool(TwitchAction.PropTwitchNotify, ref this.TwitchNotify);
			properties.ParseBool(TwitchAction.PropDelayNotify, ref this.DelayNotify);
			properties.ParseBool(TwitchAction.PropPlayBitSound, ref this.PlayBitSound);
			properties.ParseBool(TwitchAction.PropHideOnDisable, ref this.HideOnDisable);
			properties.ParseBool(TwitchAction.PropIgnoreCooldown, ref this.IgnoreCooldown);
			properties.ParseBool(TwitchAction.PropIgnoreDiscount, ref this.IgnoreDiscount);
			properties.ParseBool(TwitchAction.PropStreamerOnly, ref this.StreamerOnly);
			properties.ParseEnum<TwitchAction.OnlyUsableTypes>(TwitchAction.PropOnlyUsableByType, ref this.OnlyUsableByType);
			string text2 = "";
			properties.ParseString(TwitchAction.PropOnlyUsableBy, ref text2);
			if (text2 != "")
			{
				this.OnlyUsableBy = text2.Split(',', StringSplitOptions.None);
			}
			properties.ParseEnum<TwitchAction.PointTypes>(TwitchAction.PropPointTypes, ref this.PointType);
			this.ResetToDefaultCost();
			this.UpdateCost(1f);
			if (this.CooldownAddAmount == -1)
			{
				this.CooldownAddAmount = this.DefaultCost;
			}
			properties.ParseFloat(TwitchAction.PropVoteCooldownAddition, ref this.VoteCooldownAddition);
			if (properties.Values.ContainsKey(TwitchAction.PropPresets))
			{
				this.PresetNames = new List<string>();
				this.PresetNames.AddRange(properties.Values[TwitchAction.PropPresets].Split(',', StringSplitOptions.None));
			}
			properties.ParseBool(TwitchAction.PropOnlyShowInPreset, ref this.OnlyShowInPreset);
			if (properties.Contains(TwitchAction.PropMinRespawnCount) || properties.Contains(TwitchAction.PropMaxRespawnCount))
			{
				properties.ParseInt(TwitchAction.PropMinRespawnCount, ref this.MinRespawnCount);
				properties.ParseInt(TwitchAction.PropMaxRespawnCount, ref this.MaxRespawnCount);
				this.RespawnCountType = TwitchAction.RespawnCountTypes.Both;
			}
			else
			{
				this.RespawnCountType = TwitchAction.RespawnCountTypes.None;
			}
			properties.ParseEnum<TwitchAction.RespawnCountTypes>(TwitchAction.PropRespawnCountType, ref this.RespawnCountType);
			properties.ParseInt(TwitchAction.PropRespawnThreshold, ref this.RespawnThreshold);
			return true;
		}

		// Token: 0x0600A6EE RID: 42734 RVA: 0x0041E730 File Offset: 0x0041C930
		public bool UpdateCost(float bitPriceModifier = 1f)
		{
			int currentCost = this.CurrentCost;
			if (this.PointType == TwitchAction.PointTypes.Bits)
			{
				if (!this.IgnoreDiscount && bitPriceModifier != 1f)
				{
					this.CurrentCost = TwitchAction.GetAdjustedBitPriceFloorNoZero((int)((float)this.ModifiedCost * bitPriceModifier));
				}
				else
				{
					this.CurrentCost = TwitchAction.GetAdjustedBitPriceFloorNoZero(this.ModifiedCost);
				}
			}
			else
			{
				this.CurrentCost = this.ModifiedCost;
			}
			return currentCost != this.CurrentCost;
		}

		// Token: 0x0600A6EF RID: 42735 RVA: 0x0041E7A0 File Offset: 0x0041C9A0
		public virtual bool IsReady(TwitchManager twitchManager)
		{
			if (this.SpecialRequirementList != null)
			{
				for (int i = 0; i < this.SpecialRequirementList.Count; i++)
				{
					switch (this.SpecialRequirementList[i])
					{
					case TwitchAction.SpecialRequirements.HasSpawnedEntities:
						if (twitchManager.actionSpawnLiveList.Count == 0)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NoSpawnedEntities:
						if (twitchManager.actionSpawnLiveList.Count > 0)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.Bloodmoon:
						if (!twitchManager.isBMActive)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotBloodmoon:
						if (twitchManager.isBMActive)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotBloodmoonDay:
						if (SkyManager.IsBloodMoonVisible())
						{
							return false;
						}
						if (GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime) == GameStats.GetInt(EnumGameStats.BloodMoonDay))
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.EarlyDay:
					{
						int num = GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime);
						if ((float)num > SkyManager.GetDuskTime() - 5f || (float)num < SkyManager.GetDawnTime())
						{
							return false;
						}
						break;
					}
					case TwitchAction.SpecialRequirements.Daytime:
					{
						int num2 = GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime);
						if ((float)num2 > SkyManager.GetDuskTime() || (float)num2 < SkyManager.GetDawnTime())
						{
							return false;
						}
						break;
					}
					case TwitchAction.SpecialRequirements.Night:
						GameUtils.WorldTimeToHours(GameManager.Instance.World.worldTime);
						if (!SkyManager.IsDark())
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.IsCooldown:
						if (!TwitchManager.HasInstance || !twitchManager.IsReady)
						{
							return false;
						}
						if (twitchManager.CurrentCooldownPreset.CooldownType != CooldownPreset.CooldownTypes.Fill)
						{
							return false;
						}
						if (twitchManager.CooldownType != TwitchManager.CooldownTypes.MaxReached)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.InLandClaim:
						if (!twitchManager.LocalPlayerInLandClaim)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotInLandClaim:
						if (twitchManager.LocalPlayerInLandClaim)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.Safe:
						if (!twitchManager.LocalPlayer.TwitchSafe)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotSafe:
						if (twitchManager.LocalPlayer.TwitchSafe)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NoFullProgression:
						if (!twitchManager.IsReady)
						{
							return false;
						}
						if (!twitchManager.UseProgression || twitchManager.OverrideProgession)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotOnVehicle:
						if (!twitchManager.IsReady)
						{
							return false;
						}
						if (twitchManager.LocalPlayer.AttachedToEntity != null)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotInTrader:
						if (twitchManager.LocalPlayer.IsInTrader)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.Encumbrance:
						if ((int)EffectManager.GetValue(PassiveEffects.CarryCapacity, null, 0f, twitchManager.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) <= 30)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.WeatherGracePeriod:
						if (GameManager.Instance.World.GetWorldTime() <= 30000UL)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.NotOnQuest:
						if (QuestEventManager.Current.QuestBounds.width != 0f)
						{
							return false;
						}
						break;
					case TwitchAction.SpecialRequirements.OnQuest:
						if (QuestEventManager.Current.QuestBounds.width == 0f)
						{
							return false;
						}
						break;
					}
				}
			}
			bool flag = (!this.SingleDayUse && !this.RandomDaily) || this.AllowedDay == GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
			if (this.tempCooldown > 0f && twitchManager.CurrentUnityTime - this.tempCooldownSet < this.tempCooldown)
			{
				return false;
			}
			this.tempCooldown = 0f;
			this.tempCooldownSet = 0f;
			return twitchManager.CurrentUnityTime - this.lastUse >= this.ModifiedCooldown && flag;
		}

		// Token: 0x0600A6F0 RID: 42736 RVA: 0x0041EAE1 File Offset: 0x0041CCE1
		public void UpdateModifiedCooldown(float modifier)
		{
			this.ModifiedCooldown = modifier * this.Cooldown;
		}

		// Token: 0x0600A6F1 RID: 42737 RVA: 0x0000FB42 File Offset: 0x0000DD42
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual bool OnPerformAction(EntityPlayer Target, TwitchActionEntry entry)
		{
			return false;
		}

		// Token: 0x0600A6F2 RID: 42738 RVA: 0x0041EAF1 File Offset: 0x0041CCF1
		public void AddCooldownAddition(TwitchActionCooldownAddition newCooldown)
		{
			if (this.CooldownAdditions == null)
			{
				this.CooldownAdditions = new List<TwitchActionCooldownAddition>();
			}
			this.CooldownAdditions.Add(newCooldown);
		}

		// Token: 0x0600A6F3 RID: 42739 RVA: 0x0041EB12 File Offset: 0x0041CD12
		public void ResetCooldown(float currentUnityTime)
		{
			this.lastUse = currentUnityTime - this.ModifiedCooldown;
			this.tempCooldown = 0f;
			this.tempCooldownSet = 0f;
		}

		// Token: 0x0600A6F4 RID: 42740 RVA: 0x0041EB38 File Offset: 0x0041CD38
		public void SetCooldown(float currentUnityTime, float newCooldownTime)
		{
			this.lastUse = currentUnityTime - this.ModifiedCooldown;
			this.tempCooldown = newCooldownTime;
			this.tempCooldownSet = currentUnityTime;
		}

		// Token: 0x04008127 RID: 33063
		public string Name = "";

		// Token: 0x04008128 RID: 33064
		public string Title = "";

		// Token: 0x04008129 RID: 33065
		public string BaseCommand = "";

		// Token: 0x0400812A RID: 33066
		public string Command = "";

		// Token: 0x0400812B RID: 33067
		public string EventName = "";

		// Token: 0x0400812C RID: 33068
		public List<string> CategoryNames = new List<string>();

		// Token: 0x0400812D RID: 33069
		public int DefaultCost = 5;

		// Token: 0x0400812E RID: 33070
		public int ModifiedCost = 5;

		// Token: 0x0400812F RID: 33071
		public int CurrentCost = 5;

		// Token: 0x04008130 RID: 33072
		public int StartGameStage;

		// Token: 0x04008131 RID: 33073
		public string Description;

		// Token: 0x04008132 RID: 33074
		public float OriginalCooldown;

		// Token: 0x04008133 RID: 33075
		public float Cooldown;

		// Token: 0x04008134 RID: 33076
		public bool IsPositive;

		// Token: 0x04008135 RID: 33077
		public bool AddsToCooldown;

		// Token: 0x04008136 RID: 33078
		public int CooldownAddAmount = -1;

		// Token: 0x04008137 RID: 33079
		public bool CooldownBlocked;

		// Token: 0x04008138 RID: 33080
		public bool WaitingBlocked;

		// Token: 0x04008139 RID: 33081
		public bool OnCooldown;

		// Token: 0x0400813A RID: 33082
		public bool Enabled = true;

		// Token: 0x0400813B RID: 33083
		public bool SingleDayUse;

		// Token: 0x0400813C RID: 33084
		public bool DelayNotify;

		// Token: 0x0400813D RID: 33085
		public bool TwitchNotify = true;

		// Token: 0x0400813E RID: 33086
		public bool PlayBitSound = true;

		// Token: 0x0400813F RID: 33087
		public string RandomGroup = "";

		// Token: 0x04008140 RID: 33088
		public string Replaces = "";

		// Token: 0x04008141 RID: 33089
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastUse;

		// Token: 0x04008142 RID: 33090
		public int AllowedDay = -1;

		// Token: 0x04008143 RID: 33091
		public int groupIndex;

		// Token: 0x04008144 RID: 33092
		public float tempCooldownSet;

		// Token: 0x04008145 RID: 33093
		public float tempCooldown;

		// Token: 0x04008146 RID: 33094
		public bool IgnoreCooldown;

		// Token: 0x04008147 RID: 33095
		public bool IgnoreDiscount;

		// Token: 0x04008148 RID: 33096
		public float ModifiedCooldown;

		// Token: 0x04008149 RID: 33097
		public List<TwitchActionCooldownAddition> CooldownAdditions;

		// Token: 0x0400814A RID: 33098
		public bool ShowInActionList = true;

		// Token: 0x0400814B RID: 33099
		public List<TwitchAction.SpecialRequirements> SpecialRequirementList;

		// Token: 0x0400814C RID: 33100
		public bool HideOnDisable;

		// Token: 0x0400814D RID: 33101
		public TwitchAction.OnlyUsableTypes OnlyUsableByType;

		// Token: 0x0400814E RID: 33102
		public string[] OnlyUsableBy;

		// Token: 0x0400814F RID: 33103
		public bool OriginalEnabled = true;

		// Token: 0x04008150 RID: 33104
		public TwitchAction.PointTypes PointType;

		// Token: 0x04008151 RID: 33105
		public TwitchActionManager.ActionCategory MainCategory;

		// Token: 0x04008152 RID: 33106
		public TwitchActionManager.ActionCategory DisplayCategory;

		// Token: 0x04008153 RID: 33107
		public List<string> PresetNames;

		// Token: 0x04008154 RID: 33108
		public bool OnlyShowInPreset;

		// Token: 0x04008155 RID: 33109
		public float VoteCooldownAddition;

		// Token: 0x04008156 RID: 33110
		public bool StreamerOnly;

		// Token: 0x04008157 RID: 33111
		public TwitchAction.RespawnCountTypes RespawnCountType;

		// Token: 0x04008158 RID: 33112
		public int MinRespawnCount = -1;

		// Token: 0x04008159 RID: 33113
		public int MaxRespawnCount = -1;

		// Token: 0x0400815A RID: 33114
		public int RespawnThreshold;

		// Token: 0x0400815B RID: 33115
		public DynamicProperties Properties;

		// Token: 0x0400815C RID: 33116
		public static string PropCommand = "command";

		// Token: 0x0400815D RID: 33117
		public static string PropCommandKey = "command_key";

		// Token: 0x0400815E RID: 33118
		public static string PropTitle = "title";

		// Token: 0x0400815F RID: 33119
		public static string PropTitleKey = "title_key";

		// Token: 0x04008160 RID: 33120
		public static string PropCategory = "category";

		// Token: 0x04008161 RID: 33121
		public static string PropDisplayCategory = "display_category";

		// Token: 0x04008162 RID: 33122
		public static string PropEventName = "event_name";

		// Token: 0x04008163 RID: 33123
		public static string PropDefaultCost = "default_cost";

		// Token: 0x04008164 RID: 33124
		public static string PropDescription = "description";

		// Token: 0x04008165 RID: 33125
		public static string PropDescriptionKey = "description_key";

		// Token: 0x04008166 RID: 33126
		public static string PropStartGameStage = "start_gamestage";

		// Token: 0x04008167 RID: 33127
		public static string PropCooldown = "cooldown";

		// Token: 0x04008168 RID: 33128
		public static string PropIsPositive = "is_positive";

		// Token: 0x04008169 RID: 33129
		public static string PropSpecialOnly = "special_only";

		// Token: 0x0400816A RID: 33130
		public static string PropAddCooldown = "add_cooldown";

		// Token: 0x0400816B RID: 33131
		public static string PropCooldownAddAmount = "cooldown_add_amount";

		// Token: 0x0400816C RID: 33132
		public static string PropCooldownBlocked = "cooldown_blocked";

		// Token: 0x0400816D RID: 33133
		public static string PropWaitingBlocked = "waiting_blocked";

		// Token: 0x0400816E RID: 33134
		public static string PropEnabled = "enabled";

		// Token: 0x0400816F RID: 33135
		public static string PropSingleDayUse = "single_day";

		// Token: 0x04008170 RID: 33136
		public static string PropRandomGroup = "random_group";

		// Token: 0x04008171 RID: 33137
		public static string PropReplaces = "replaces";

		// Token: 0x04008172 RID: 33138
		public static string PropShowInActionList = "show_in_action_list";

		// Token: 0x04008173 RID: 33139
		public static string PropPlayBitSound = "play_bit_sound";

		// Token: 0x04008174 RID: 33140
		public static string PropSpecialRequirement = "special_requirement";

		// Token: 0x04008175 RID: 33141
		public static string PropTwitchNotify = "twitch_notify";

		// Token: 0x04008176 RID: 33142
		public static string PropDelayNotify = "delay_notify";

		// Token: 0x04008177 RID: 33143
		public static string PropHideOnDisable = "hide_on_disable";

		// Token: 0x04008178 RID: 33144
		public static string PropOnlyUsableByType = "only_usable_type";

		// Token: 0x04008179 RID: 33145
		public static string PropOnlyUsableBy = "only_usable_by";

		// Token: 0x0400817A RID: 33146
		public static string PropIgnoreCooldown = "ignore_cooldown";

		// Token: 0x0400817B RID: 33147
		public static string PropIgnoreDiscount = "ignore_discount";

		// Token: 0x0400817C RID: 33148
		public static string PropPointTypes = "point_type";

		// Token: 0x0400817D RID: 33149
		public static string PropPresets = "presets";

		// Token: 0x0400817E RID: 33150
		public static string PropOnlyShowInPreset = "only_show_in_preset";

		// Token: 0x0400817F RID: 33151
		public static string PropVoteCooldownAddition = "vote_cooldown_add";

		// Token: 0x04008180 RID: 33152
		public static string PropStreamerOnly = "streamer_only";

		// Token: 0x04008181 RID: 33153
		public static string PropMinRespawnCount = "min_respawn_count";

		// Token: 0x04008182 RID: 33154
		public static string PropMaxRespawnCount = "max_respawn_count";

		// Token: 0x04008183 RID: 33155
		public static string PropRespawnCountType = "respawn_count_type";

		// Token: 0x04008184 RID: 33156
		public static string PropRespawnThreshold = "respawn_threshold";

		// Token: 0x04008185 RID: 33157
		public static HashSet<string> ExtendsExcludes = new HashSet<string>
		{
			TwitchAction.PropShowInActionList,
			TwitchAction.PropCommandKey,
			TwitchAction.PropCommand,
			TwitchAction.PropTitleKey,
			TwitchAction.PropDescriptionKey
		};

		// Token: 0x04008186 RID: 33158
		[PublicizedFrom(EAccessModifier.Private)]
		public static List<int> bitPrices = new List<int>
		{
			10,
			15,
			20,
			25,
			30,
			35,
			40,
			45,
			50,
			55,
			60,
			65,
			70,
			75,
			80,
			85,
			90,
			95,
			100,
			105,
			110,
			115,
			120,
			125,
			130,
			135,
			140,
			145,
			150,
			155,
			160,
			165,
			170,
			175,
			180,
			185,
			190,
			195,
			200,
			250,
			300,
			350,
			400,
			450,
			500,
			550,
			600,
			650,
			700,
			750,
			800,
			850,
			900,
			950,
			1000,
			1100,
			1200,
			1250,
			1300,
			1400,
			1500,
			1600,
			1700,
			1750,
			1800,
			1900,
			2000,
			2250,
			2500,
			2750,
			3000,
			3500,
			4000,
			4500,
			5000,
			5500,
			6000,
			6500,
			7000,
			7500,
			8000,
			8500,
			9000,
			9500,
			10000
		};

		// Token: 0x0200151E RID: 5406
		public enum SpecialRequirements
		{
			// Token: 0x04008188 RID: 33160
			None,
			// Token: 0x04008189 RID: 33161
			HasSpawnedEntities,
			// Token: 0x0400818A RID: 33162
			NoSpawnedEntities,
			// Token: 0x0400818B RID: 33163
			Bloodmoon,
			// Token: 0x0400818C RID: 33164
			NotBloodmoon,
			// Token: 0x0400818D RID: 33165
			NotBloodmoonDay,
			// Token: 0x0400818E RID: 33166
			EarlyDay,
			// Token: 0x0400818F RID: 33167
			Daytime,
			// Token: 0x04008190 RID: 33168
			Night,
			// Token: 0x04008191 RID: 33169
			IsCooldown,
			// Token: 0x04008192 RID: 33170
			InLandClaim,
			// Token: 0x04008193 RID: 33171
			NotInLandClaim,
			// Token: 0x04008194 RID: 33172
			Safe,
			// Token: 0x04008195 RID: 33173
			NotSafe,
			// Token: 0x04008196 RID: 33174
			NoFullProgression,
			// Token: 0x04008197 RID: 33175
			NotOnVehicle,
			// Token: 0x04008198 RID: 33176
			NotInTrader,
			// Token: 0x04008199 RID: 33177
			Encumbrance,
			// Token: 0x0400819A RID: 33178
			WeatherGracePeriod,
			// Token: 0x0400819B RID: 33179
			NotOnQuest,
			// Token: 0x0400819C RID: 33180
			OnQuest
		}

		// Token: 0x0200151F RID: 5407
		public enum OnlyUsableTypes
		{
			// Token: 0x0400819E RID: 33182
			Everyone,
			// Token: 0x0400819F RID: 33183
			Broadcaster,
			// Token: 0x040081A0 RID: 33184
			Mods,
			// Token: 0x040081A1 RID: 33185
			VIPs,
			// Token: 0x040081A2 RID: 33186
			Subs,
			// Token: 0x040081A3 RID: 33187
			Name
		}

		// Token: 0x02001520 RID: 5408
		public enum PointTypes
		{
			// Token: 0x040081A5 RID: 33189
			PP,
			// Token: 0x040081A6 RID: 33190
			SP,
			// Token: 0x040081A7 RID: 33191
			Bits
		}

		// Token: 0x02001521 RID: 5409
		public enum RespawnCountTypes
		{
			// Token: 0x040081A9 RID: 33193
			None,
			// Token: 0x040081AA RID: 33194
			Both,
			// Token: 0x040081AB RID: 33195
			BlocksOnly,
			// Token: 0x040081AC RID: 33196
			SpawnsOnly
		}
	}
}

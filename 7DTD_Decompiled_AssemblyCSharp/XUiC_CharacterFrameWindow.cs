using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C31 RID: 3121
[Preserve]
public class XUiC_CharacterFrameWindow : XUiController
{
	// Token: 0x0600600A RID: 24586 RVA: 0x0026F288 File Offset: 0x0026D488
	public override void Init()
	{
		base.Init();
		this.previewFrame = base.GetChildById("playerPreviewSDCS");
		this.previewFrame.OnPress += this.PreviewFrame_OnPress;
		this.previewFrame.OnHover += this.PreviewFrame_OnHover;
		this.lblLevel = (XUiV_Label)base.GetChildById("levelNumber").ViewComponent;
		this.lblName = (XUiV_Label)base.GetChildById("characterName").ViewComponent;
		this.textPreview = (XUiV_Texture)base.GetChildById("playerPreviewSDCS").ViewComponent;
		this.isDirty = true;
		this.characterButton = base.GetChildById("characterButton");
		if (this.characterButton != null)
		{
			this.characterButton.OnPress += this.CharacterButton_OnPress;
		}
		this.statsButton = base.GetChildById("statButton");
		if (this.statsButton != null)
		{
			this.statsButton.OnPress += this.StatsButton_OnPress;
		}
		this.coreStatsButton = base.GetChildById("coreStatButton");
		if (this.coreStatsButton != null)
		{
			this.coreStatsButton.OnPress += this.CoreStatsButton_OnPress;
		}
		this.coreStatsMovementIdleButton = base.GetChildById("coreStatsMovementIdleButton");
		if (this.coreStatsMovementIdleButton != null)
		{
			this.coreStatsMovementIdleButton.OnPress += this.coreStatsMovementButton_OnPress;
		}
		this.coreStatsMovementWalkingButton = base.GetChildById("coreStatsMovementWalkingButton");
		if (this.coreStatsMovementWalkingButton != null)
		{
			this.coreStatsMovementWalkingButton.OnPress += this.coreStatsMovementButton_OnPress;
		}
		this.coreStatsMovementRunningButton = base.GetChildById("coreStatsMovementRunningButton");
		if (this.coreStatsMovementRunningButton != null)
		{
			this.coreStatsMovementRunningButton.OnPress += this.coreStatsMovementButton_OnPress;
		}
		XUiM_PlayerEquipment.HandleRefreshEquipment += this.XUiM_PlayerEquipment_HandleRefreshEquipment;
		this.levelLabel = Localization.Get("lblLevel", false);
		this.biomeBadgeSlot = (base.GetChildById("biomeBadgeSlot1") as XUiC_EquipmentStack);
		this.biomeBadgeSlot2 = (base.GetChildById("biomeBadgeSlot2") as XUiC_EquipmentStack);
		this.biomeBadgeSlot3 = (base.GetChildById("biomeBadgeSlot3") as XUiC_EquipmentStack);
		this.biomeBadgeSlot4 = (base.GetChildById("biomeBadgeSlot4") as XUiC_EquipmentStack);
		XUiC_EquipmentStackGrid childByType = base.GetChildByType<XUiC_EquipmentStackGrid>();
		if (childByType != null)
		{
			childByType.ExtraSlot = this.biomeBadgeSlot;
			childByType.ExtraSlot2 = this.biomeBadgeSlot2;
			childByType.ExtraSlot3 = this.biomeBadgeSlot3;
			childByType.ExtraSlot4 = this.biomeBadgeSlot4;
		}
		base.xui.playerUI.OnUIShutdown += this.HandleUIShutdown;
		base.xui.OnShutdown += this.HandleUIShutdown;
		base.GetChildById("btnCosmetic").GetChildById("clickable").ViewComponent.Controller.OnPress += this.btnCosmetic_OnPress;
	}

	// Token: 0x0600600B RID: 24587 RVA: 0x0026F569 File Offset: 0x0026D769
	[PublicizedFrom(EAccessModifier.Private)]
	public void btnCosmetic_OnPress(XUiController _sender, int _mouseButton)
	{
		XUiC_CharacterCosmeticWindowGroup.Open(base.xui, EquipmentSlots.Head);
	}

	// Token: 0x0600600C RID: 24588 RVA: 0x0026F578 File Offset: 0x0026D778
	[PublicizedFrom(EAccessModifier.Private)]
	public void StatsButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.currentTab = XUiC_CharacterFrameWindow.Tabs.Stats;
		if (this.characterButton != null)
		{
			((XUiV_Button)this.characterButton.ViewComponent).Selected = false;
		}
		if (this.statsButton != null)
		{
			((XUiV_Button)this.statsButton.ViewComponent).Selected = true;
		}
		if (this.coreStatsButton != null)
		{
			((XUiV_Button)this.coreStatsButton.ViewComponent).Selected = false;
		}
		this.isDirty = true;
	}

	// Token: 0x0600600D RID: 24589 RVA: 0x0026F5F0 File Offset: 0x0026D7F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CharacterButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.currentTab = XUiC_CharacterFrameWindow.Tabs.Character;
		if (this.characterButton != null)
		{
			((XUiV_Button)this.characterButton.ViewComponent).Selected = true;
		}
		if (this.statsButton != null)
		{
			((XUiV_Button)this.statsButton.ViewComponent).Selected = false;
		}
		if (this.coreStatsButton != null)
		{
			((XUiV_Button)this.coreStatsButton.ViewComponent).Selected = false;
		}
		this.isDirty = true;
	}

	// Token: 0x0600600E RID: 24590 RVA: 0x0026F668 File Offset: 0x0026D868
	[PublicizedFrom(EAccessModifier.Private)]
	public void CoreStatsButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.currentTab = XUiC_CharacterFrameWindow.Tabs.CoreStats;
		if (this.characterButton != null)
		{
			((XUiV_Button)this.characterButton.ViewComponent).Selected = false;
		}
		if (this.statsButton != null)
		{
			((XUiV_Button)this.statsButton.ViewComponent).Selected = false;
		}
		if (this.coreStatsButton != null)
		{
			((XUiV_Button)this.coreStatsButton.ViewComponent).Selected = true;
		}
		this.isDirty = true;
	}

	// Token: 0x0600600F RID: 24591 RVA: 0x0026F6E0 File Offset: 0x0026D8E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void coreStatsMovementButton_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.currentMovementTag.Equals(EntityAlive.MovementTagIdle))
		{
			this.currentMovementTag = EntityAlive.MovementTagWalking;
			((XUiV_Button)this.coreStatsMovementIdleButton.ViewComponent).Enabled = false;
			((XUiV_Button)this.coreStatsMovementIdleButton.ViewComponent).IsVisible = false;
			((XUiV_Button)this.coreStatsMovementWalkingButton.ViewComponent).Enabled = true;
			((XUiV_Button)this.coreStatsMovementWalkingButton.ViewComponent).IsVisible = true;
			((XUiV_Button)this.coreStatsMovementRunningButton.ViewComponent).Enabled = false;
			((XUiV_Button)this.coreStatsMovementRunningButton.ViewComponent).IsVisible = false;
			this.coreStatsMovementWalkingButton.SelectCursorElement(false, false);
			return;
		}
		if (this.currentMovementTag.Equals(EntityAlive.MovementTagWalking))
		{
			this.currentMovementTag = EntityAlive.MovementTagRunning;
			((XUiV_Button)this.coreStatsMovementIdleButton.ViewComponent).Enabled = false;
			((XUiV_Button)this.coreStatsMovementIdleButton.ViewComponent).IsVisible = false;
			((XUiV_Button)this.coreStatsMovementWalkingButton.ViewComponent).Enabled = false;
			((XUiV_Button)this.coreStatsMovementWalkingButton.ViewComponent).IsVisible = false;
			((XUiV_Button)this.coreStatsMovementRunningButton.ViewComponent).Enabled = true;
			((XUiV_Button)this.coreStatsMovementRunningButton.ViewComponent).IsVisible = true;
			this.coreStatsMovementRunningButton.SelectCursorElement(false, false);
			return;
		}
		this.currentMovementTag = EntityAlive.MovementTagIdle;
		((XUiV_Button)this.coreStatsMovementIdleButton.ViewComponent).Enabled = true;
		((XUiV_Button)this.coreStatsMovementIdleButton.ViewComponent).IsVisible = true;
		((XUiV_Button)this.coreStatsMovementWalkingButton.ViewComponent).Enabled = false;
		((XUiV_Button)this.coreStatsMovementWalkingButton.ViewComponent).IsVisible = false;
		((XUiV_Button)this.coreStatsMovementRunningButton.ViewComponent).Enabled = false;
		((XUiV_Button)this.coreStatsMovementRunningButton.ViewComponent).IsVisible = true;
		this.coreStatsMovementIdleButton.SelectCursorElement(false, false);
	}

	// Token: 0x06006010 RID: 24592 RVA: 0x0026F8F0 File Offset: 0x0026DAF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleUIShutdown()
	{
		base.xui.playerUI.OnUIShutdown -= this.HandleUIShutdown;
		base.xui.OnShutdown -= this.HandleUIShutdown;
		XUiM_PlayerEquipment.HandleRefreshEquipment -= this.XUiM_PlayerEquipment_HandleRefreshEquipment;
	}

	// Token: 0x06006011 RID: 24593 RVA: 0x0026F941 File Offset: 0x0026DB41
	[PublicizedFrom(EAccessModifier.Private)]
	public void PreviewFrame_OnHover(XUiController _sender, bool _isOver)
	{
		this.renderTextureSystem.RotateTarget(Time.deltaTime * 10f);
	}

	// Token: 0x06006012 RID: 24594 RVA: 0x0026F95C File Offset: 0x0026DB5C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PreviewFrame_OnPress(XUiController _sender, int _mouseButton)
	{
		if (base.xui.dragAndDrop.CurrentStack != ItemStack.Empty)
		{
			ItemStack itemStack = base.xui.PlayerEquipment.EquipItem(base.xui.dragAndDrop.CurrentStack);
			if (base.xui.dragAndDrop.CurrentStack != itemStack)
			{
				base.xui.dragAndDrop.CurrentStack = itemStack;
				base.xui.dragAndDrop.PickUpType = XUiC_ItemStack.StackLocationTypes.Equipment;
			}
		}
	}

	// Token: 0x06006013 RID: 24595 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiM_PlayerEquipment_HandleRefreshEquipment(XUiM_PlayerEquipment _playerEquipment)
	{
	}

	// Token: 0x06006014 RID: 24596 RVA: 0x0026F9D8 File Offset: 0x0026DBD8
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		if (this.ep == null)
		{
			this.ep = base.xui.playerUI.entityPlayer;
		}
		if (this.currentTab != XUiC_CharacterFrameWindow.Tabs.Character && Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 0.25f;
			base.RefreshBindings(this.isDirty);
		}
		if (this.isDirty)
		{
			if (this.player == null)
			{
				return;
			}
			if (this.biomeBadgeSlot != null)
			{
				this.biomeBadgeSlot.EquipSlot = EquipmentSlots.BiomeBadge;
			}
			if (this.biomeBadgeSlot2 != null)
			{
				this.biomeBadgeSlot2.EquipSlot = EquipmentSlots.BiomeBadge2;
			}
			if (this.biomeBadgeSlot3 != null)
			{
				this.biomeBadgeSlot3.EquipSlot = EquipmentSlots.BiomeBadge3;
			}
			if (this.biomeBadgeSlot4 != null)
			{
				this.biomeBadgeSlot4.EquipSlot = EquipmentSlots.BiomeBadge4;
			}
			this.lblLevel.Text = string.Format(this.levelLabel, this.player.Progression.GetLevel());
			this.lblName.Text = this.player.PlayerDisplayName;
			this.isDirty = false;
			base.RefreshBindings(false);
		}
		if (this.isPreviewDirty)
		{
			this.MakePreview();
		}
		this.textPreview.Texture = this.renderTextureSystem.RenderTex;
		if (this.previewSDCSObj != null)
		{
			this.previewSDCSObj.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		base.Update(_dt);
	}

	// Token: 0x06006015 RID: 24597 RVA: 0x0026FB6C File Offset: 0x0026DD6C
	public override void OnOpen()
	{
		base.OnOpen();
		this.isDirty = true;
		this.isPreviewDirty = true;
		this.player = base.xui.playerUI.entityPlayer;
		if (this.previewFrame != null)
		{
			this.previewFrame.OnPress -= this.PreviewFrame_OnPress;
			this.previewFrame.OnHover -= this.PreviewFrame_OnHover;
		}
		this.previewFrame = base.GetChildById("previewFrameSDCS");
		this.previewFrame.OnPress += this.PreviewFrame_OnPress;
		this.previewFrame.OnHover += this.PreviewFrame_OnHover;
		this.textPreview = (XUiV_Texture)base.GetChildById("playerPreviewSDCS").ViewComponent;
		if (this.renderTextureSystem.ParentGO == null)
		{
			this.renderTextureSystem.Create("playerpreview", new GameObject(), new Vector3(0f, -0.5f, 3f), new Vector3(0f, -0.2f, 7.5f), this.textPreview.Size, true, false, 1f);
		}
		this.displayInfoEntries = UIDisplayInfoManager.Current.GetCharacterDisplayInfo();
		if (this.player as EntityPlayerLocal != null && this.player.emodel as EModelSDCS != null)
		{
			XUiM_PlayerEquipment.HandleRefreshEquipment += this.XUiM_PlayerEquipment_HandleRefreshEquipment1;
		}
	}

	// Token: 0x06006016 RID: 24598 RVA: 0x0026FCE1 File Offset: 0x0026DEE1
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiM_PlayerEquipment_HandleRefreshEquipment1(XUiM_PlayerEquipment playerEquipment)
	{
		if (!base.IsOpen)
		{
			return;
		}
		this.MakePreview();
	}

	// Token: 0x06006017 RID: 24599 RVA: 0x0026FCF2 File Offset: 0x0026DEF2
	public override void OnClose()
	{
		base.OnClose();
		XUiM_PlayerEquipment.HandleRefreshEquipment -= this.XUiM_PlayerEquipment_HandleRefreshEquipment1;
		SDCSUtils.DestroyViz(this.previewSDCSObj, false);
		this.renderTextureSystem.Cleanup();
	}

	// Token: 0x06006018 RID: 24600 RVA: 0x0026FD24 File Offset: 0x0026DF24
	public void MakePreview()
	{
		if (this.ep == null)
		{
			return;
		}
		if (this.ep.emodel == null)
		{
			return;
		}
		EModelSDCS emodelSDCS = this.ep.emodel as EModelSDCS;
		if (emodelSDCS != null)
		{
			this.isPreviewDirty = false;
			SDCSUtils.CreateVizUI(emodelSDCS.Archetype, ref this.previewSDCSObj, ref this.transformCatalog, this.ep, false);
			Utils.SetLayerRecursively(this.previewSDCSObj, 11);
			Transform transform = this.previewSDCSObj.transform;
			transform.SetParent(this.renderTextureSystem.ParentGO.transform, false);
			transform.localPosition = new Vector3(0.022f, -2.9f, 12f);
			transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			this.renderTextureSystem.SetOrtho(true, 0.95f);
		}
	}

	// Token: 0x06006019 RID: 24601 RVA: 0x0026FE04 File Offset: 0x0026E004
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 2347010444U)
		{
			if (num <= 732973786U)
			{
				if (num <= 234495987U)
				{
					if (num <= 75296162U)
					{
						if (num != 8937094U)
						{
							if (num == 75296162U)
							{
								if (bindingName == "playerfoodtitle")
								{
									value = Localization.Get("xuiFood", false);
									return true;
								}
							}
						}
						else if (bindingName == "playerhealth")
						{
							value = ((this.player != null) ? this.playerHealthFormatter.Format((int)XUiM_Player.GetHealth(this.player)) : "");
							return true;
						}
					}
					else if (num != 125916223U)
					{
						if (num != 145239280U)
						{
							if (num == 234495987U)
							{
								if (bindingName == "playerdeathstitle")
								{
									value = Localization.Get("xuiDeaths", false);
									return true;
								}
							}
						}
						else if (bindingName == "playerarmorratingtitle")
						{
							value = Localization.Get("statPhysicalDamageResist", false);
							return true;
						}
					}
					else if (bindingName == "playercurrentlife")
					{
						value = ((this.player != null) ? XUiM_Player.GetCurrentLife(this.player) : "");
						return true;
					}
				}
				else if (num <= 672688503U)
				{
					if (num != 304163417U)
					{
						if (num != 526317834U)
						{
							if (num == 672688503U)
							{
								if (bindingName == "playercurrentlifetitle")
								{
									value = Localization.Get("xuiCurrentLife", false);
									return true;
								}
							}
						}
						else if (bindingName == "playergamestagetitle")
						{
							value = Localization.Get("xuiGameStage", false);
							return true;
						}
					}
					else if (bindingName == "playercoretemp")
					{
						value = ((this.player != null) ? XUiM_Player.GetCoreTemp(this.player) : "");
						return true;
					}
				}
				else if (num != 696376978U)
				{
					if (num != 699921418U)
					{
						if (num == 732973786U)
						{
							if (bindingName == "showcharactersdcs")
							{
								value = (this.currentTab == XUiC_CharacterFrameWindow.Tabs.Character && this.player != null && this.player.emodel as EModelSDCS != null).ToString();
								return true;
							}
						}
					}
					else if (bindingName == "movementmoderunning")
					{
						value = this.currentMovementTag.Equals(EntityAlive.MovementTagRunning).ToString();
						return true;
					}
				}
				else if (bindingName == "playerlootstagetitle")
				{
					value = Localization.Get("xuiLootstage", false);
					return true;
				}
			}
			else if (num <= 1169126906U)
			{
				if (num <= 885900949U)
				{
					if (num != 782575427U)
					{
						if (num == 885900949U)
						{
							if (bindingName == "playertravelledtitle")
							{
								value = Localization.Get("xuiKMTravelled", false);
								return true;
							}
						}
					}
					else if (bindingName == "playerdeaths")
					{
						value = ((this.player != null) ? this.playerDeathsFormatter.Format(XUiM_Player.GetDeaths(this.player)) : "");
						return true;
					}
				}
				else if (num != 965025103U)
				{
					if (num != 1009276468U)
					{
						if (num == 1169126906U)
						{
							if (bindingName == "movementmodewalking")
							{
								value = this.currentMovementTag.Equals(EntityAlive.MovementTagWalking).ToString();
								return true;
							}
						}
					}
					else if (bindingName == "playerxptonextleveltitle")
					{
						value = Localization.Get("xuiXPToNextLevel", false);
						return true;
					}
				}
				else if (bindingName == "playerwater")
				{
					value = ((this.player != null) ? this.playerWaterFormatter.Format(XUiM_Player.GetWater(this.player)) : "");
					return true;
				}
			}
			else if (num <= 2023588471U)
			{
				if (num != 1477941828U)
				{
					if (num != 1811778199U)
					{
						if (num == 2023588471U)
						{
							if (bindingName == "playerzombiekillstitle")
							{
								value = Localization.Get("xuiZombieKills", false);
								return true;
							}
						}
					}
					else if (bindingName == "playeritemscraftedtitle")
					{
						value = Localization.Get("xuiItemsCrafted", false);
						return true;
					}
				}
				else if (bindingName == "playerlongestlife")
				{
					value = ((this.player != null) ? XUiM_Player.GetLongestLife(this.player) : "");
					return true;
				}
			}
			else if (num != 2186126559U)
			{
				if (num != 2219475343U)
				{
					if (num == 2347010444U)
					{
						if (bindingName == "playergamestage")
						{
							value = ((this.player != null) ? this.player.gameStage.ToString() : "");
							return true;
						}
					}
				}
				else if (bindingName == "playermaxstamina")
				{
					value = ((this.player != null) ? this.playerMaxStaminaFormatter.Format((int)XUiM_Player.GetMaxStamina(this.player)) : "");
					return true;
				}
			}
			else if (bindingName == "playeritemscrafted")
			{
				value = ((this.player != null) ? this.playerItemsCraftedFormatter.Format(XUiM_Player.GetItemsCrafted(this.player)) : "");
				return true;
			}
		}
		else if (num <= 3484390642U)
		{
			if (num <= 2974192615U)
			{
				if (num <= 2473306113U)
				{
					if (num != 2395478116U)
					{
						if (num == 2473306113U)
						{
							if (bindingName == "movementmodeidle")
							{
								value = this.currentMovementTag.Equals(EntityAlive.MovementTagIdle).ToString();
								return true;
							}
						}
					}
					else if (bindingName == "playerlootstage")
					{
						value = ((this.player != null) ? this.player.GetLootStage(0f, 0f).ToString() : "");
						return true;
					}
				}
				else if (num != 2532548756U)
				{
					if (num != 2587631291U)
					{
						if (num == 2974192615U)
						{
							if (bindingName == "playerwatertitle")
							{
								value = Localization.Get("xuiWater", false);
								return true;
							}
						}
					}
					else if (bindingName == "showcore")
					{
						value = (this.currentTab == XUiC_CharacterFrameWindow.Tabs.CoreStats).ToString();
						return true;
					}
				}
				else if (bindingName == "playerfood")
				{
					value = ((this.player != null) ? this.playerFoodFormatter.Format(XUiM_Player.GetFood(this.player)) : "");
					return true;
				}
			}
			else if (num <= 3257770903U)
			{
				if (num != 3042900123U)
				{
					if (num != 3249756066U)
					{
						if (num == 3257770903U)
						{
							if (bindingName == "showstats")
							{
								value = (this.currentTab == XUiC_CharacterFrameWindow.Tabs.Stats).ToString();
								return true;
							}
						}
					}
					else if (bindingName == "playerfoodmax")
					{
						value = ((this.player != null) ? this.playerFoodMaxFormatter.Format(XUiM_Player.GetFoodMax(this.player)) : "");
						return true;
					}
				}
				else if (bindingName == "playerpvpkills")
				{
					value = ((this.player != null) ? this.playerPvpKillsFormatter.Format(XUiM_Player.GetPlayerKills(this.player)) : "");
					return true;
				}
			}
			else if (num != 3275992332U)
			{
				if (num != 3371877161U)
				{
					if (num == 3484390642U)
					{
						if (bindingName == "playerlongestlifetitle")
						{
							value = Localization.Get("xuiLongestLife", false);
							return true;
						}
					}
				}
				else if (bindingName == "playerstaminatitle")
				{
					value = Localization.Get("lblStamina", false);
					return true;
				}
			}
			else if (bindingName == "playermaxhealth")
			{
				value = ((this.player != null) ? this.playerMaxHealthFormatter.Format((int)XUiM_Player.GetMaxHealth(this.player)) : "");
				return true;
			}
		}
		else if (num <= 3900606022U)
		{
			if (num <= 3705263762U)
			{
				if (num != 3537464933U)
				{
					if (num == 3705263762U)
					{
						if (bindingName == "playerxptonextlevel")
						{
							value = ((this.player != null) ? this.playerXpToNextLevelFormatter.Format(XUiM_Player.GetXPToNextLevel(this.player) + this.player.Progression.ExpDeficit) : "");
							return true;
						}
					}
				}
				else if (bindingName == "playerstamina")
				{
					value = ((this.player != null) ? this.playerStaminaFormatter.Format((int)XUiM_Player.GetStamina(this.player)) : "");
					return true;
				}
			}
			else if (num != 3712331684U)
			{
				if (num != 3887827771U)
				{
					if (num == 3900606022U)
					{
						if (bindingName == "playerarmorrating")
						{
							value = ((this.player != null) ? this.playerArmorRatingFormatter.Format((int)EffectManager.GetValue(PassiveEffects.PhysicalDamageResist, null, 0f, this.player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)) : "");
							return true;
						}
					}
				}
				else if (bindingName == "playerpvpkillstitle")
				{
					value = Localization.Get("xuiPlayerKills", false);
					return true;
				}
			}
			else if (bindingName == "playermodifiedcurrentfood")
			{
				value = ((this.player != null) ? this.playerFoodFormatter.Format(XUiM_Player.GetModifiedCurrentFood(this.player)) : "");
				return true;
			}
		}
		else if (num <= 4031300656U)
		{
			if (num != 3931175545U)
			{
				if (num != 4025935093U)
				{
					if (num == 4031300656U)
					{
						if (bindingName == "playerhealthtitle")
						{
							value = Localization.Get("lblHealth", false);
							return true;
						}
					}
				}
				else if (bindingName == "playercoretemptitle")
				{
					value = Localization.Get("xuiFeelsLike", false);
					return true;
				}
			}
			else if (bindingName == "playertravelled")
			{
				value = ((this.player != null) ? XUiM_Player.GetKMTraveled(this.player) : "");
				return true;
			}
		}
		else if (num != 4077864767U)
		{
			if (num != 4107995367U)
			{
				if (num == 4159374943U)
				{
					if (bindingName == "playermodifiedcurrentwater")
					{
						value = ((this.player != null) ? this.playerWaterFormatter.Format(XUiM_Player.GetModifiedCurrentWater(this.player)) : "");
						return true;
					}
				}
			}
			else if (bindingName == "playerwatermax")
			{
				value = ((this.player != null) ? this.playerWaterMaxFormatter.Format(XUiM_Player.GetWaterMax(this.player)) : "");
				return true;
			}
		}
		else if (bindingName == "playerzombiekills")
		{
			value = ((this.player != null) ? this.playerZombieKillsFormatter.Format(XUiM_Player.GetZombieKills(this.player)) : "");
			return true;
		}
		if (bindingName.StartsWith("playerstattitle"))
		{
			if (this.player != null)
			{
				int index = Convert.ToInt32(bindingName.Replace("playerstattitle", "")) - 1;
				value = this.GetStatTitle(index);
			}
			else
			{
				value = "";
			}
			return true;
		}
		if (bindingName.StartsWith("playerstat"))
		{
			if (this.player != null)
			{
				int index2 = Convert.ToInt32(bindingName.Replace("playerstat", "")) - 1;
				value = this.GetStatValue(index2);
			}
			else
			{
				value = "";
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600601A RID: 24602 RVA: 0x00270AB8 File Offset: 0x0026ECB8
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetStatTitle(int index)
	{
		if (this.displayInfoEntries.Count <= index)
		{
			return "";
		}
		if (this.displayInfoEntries[index].TitleOverride != null)
		{
			return this.displayInfoEntries[index].TitleOverride;
		}
		return UIDisplayInfoManager.Current.GetLocalizedName(this.displayInfoEntries[index].StatType);
	}

	// Token: 0x0600601B RID: 24603 RVA: 0x00270B1C File Offset: 0x0026ED1C
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetStatValue(int index)
	{
		if (this.displayInfoEntries.Count <= index)
		{
			return "";
		}
		DisplayInfoEntry displayInfoEntry = this.displayInfoEntries[index];
		return XUiM_Player.GetStatValue(displayInfoEntry.StatType, base.xui.playerUI.entityPlayer, displayInfoEntry, this.currentMovementTag);
	}

	// Token: 0x0400485C RID: 18524
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblLevel;

	// Token: 0x0400485D RID: 18525
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblName;

	// Token: 0x0400485E RID: 18526
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController previewFrame;

	// Token: 0x0400485F RID: 18527
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController characterButton;

	// Token: 0x04004860 RID: 18528
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController statsButton;

	// Token: 0x04004861 RID: 18529
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController coreStatsButton;

	// Token: 0x04004862 RID: 18530
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController coreStatsMovementIdleButton;

	// Token: 0x04004863 RID: 18531
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController coreStatsMovementWalkingButton;

	// Token: 0x04004864 RID: 18532
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController coreStatsMovementRunningButton;

	// Token: 0x04004865 RID: 18533
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CharacterFrameWindow.Tabs currentTab;

	// Token: 0x04004866 RID: 18534
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture textPreview;

	// Token: 0x04004867 RID: 18535
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer ep;

	// Token: 0x04004868 RID: 18536
	[PublicizedFrom(EAccessModifier.Private)]
	public Camera cam;

	// Token: 0x04004869 RID: 18537
	public RuntimeAnimatorController animationController;

	// Token: 0x0400486A RID: 18538
	public float atlasResolutionScale;

	// Token: 0x0400486B RID: 18539
	[PublicizedFrom(EAccessModifier.Private)]
	public RenderTextureSystem renderTextureSystem = new RenderTextureSystem();

	// Token: 0x0400486C RID: 18540
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x0400486D RID: 18541
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPreviewDirty;

	// Token: 0x0400486E RID: 18542
	[PublicizedFrom(EAccessModifier.Private)]
	public string levelLabel;

	// Token: 0x0400486F RID: 18543
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer player;

	// Token: 0x04004870 RID: 18544
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DisplayInfoEntry> displayInfoEntries;

	// Token: 0x04004871 RID: 18545
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_EquipmentStack biomeBadgeSlot;

	// Token: 0x04004872 RID: 18546
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_EquipmentStack biomeBadgeSlot2;

	// Token: 0x04004873 RID: 18547
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_EquipmentStack biomeBadgeSlot3;

	// Token: 0x04004874 RID: 18548
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_EquipmentStack biomeBadgeSlot4;

	// Token: 0x04004875 RID: 18549
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global> currentMovementTag = EntityAlive.MovementTagRunning;

	// Token: 0x04004876 RID: 18550
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;

	// Token: 0x04004877 RID: 18551
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject previewSDCSObj;

	// Token: 0x04004878 RID: 18552
	[PublicizedFrom(EAccessModifier.Private)]
	public SDCSUtils.TransformCatalog transformCatalog;

	// Token: 0x04004879 RID: 18553
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerDeathsFormatter = new CachedStringFormatterInt();

	// Token: 0x0400487A RID: 18554
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerHealthFormatter = new CachedStringFormatterInt();

	// Token: 0x0400487B RID: 18555
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerStaminaFormatter = new CachedStringFormatterInt();

	// Token: 0x0400487C RID: 18556
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerMaxHealthFormatter = new CachedStringFormatterInt();

	// Token: 0x0400487D RID: 18557
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerMaxStaminaFormatter = new CachedStringFormatterInt();

	// Token: 0x0400487E RID: 18558
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat playerFoodFormatter = new CachedStringFormatterFloat("0");

	// Token: 0x0400487F RID: 18559
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat playerWaterFormatter = new CachedStringFormatterFloat("0");

	// Token: 0x04004880 RID: 18560
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerFoodMaxFormatter = new CachedStringFormatterInt();

	// Token: 0x04004881 RID: 18561
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerWaterMaxFormatter = new CachedStringFormatterInt();

	// Token: 0x04004882 RID: 18562
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerItemsCraftedFormatter = new CachedStringFormatterInt();

	// Token: 0x04004883 RID: 18563
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerPvpKillsFormatter = new CachedStringFormatterInt();

	// Token: 0x04004884 RID: 18564
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerZombieKillsFormatter = new CachedStringFormatterInt();

	// Token: 0x04004885 RID: 18565
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerXpToNextLevelFormatter = new CachedStringFormatterInt();

	// Token: 0x04004886 RID: 18566
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt playerArmorRatingFormatter = new CachedStringFormatterInt();

	// Token: 0x02000C32 RID: 3122
	[PublicizedFrom(EAccessModifier.Private)]
	public enum Tabs
	{
		// Token: 0x04004888 RID: 18568
		Character,
		// Token: 0x04004889 RID: 18569
		Stats,
		// Token: 0x0400488A RID: 18570
		CoreStats
	}
}

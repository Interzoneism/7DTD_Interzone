using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C12 RID: 3090
[Preserve]
public class XUiC_BuffPopoutList : XUiController, IEntityUINotificationChanged
{
	// Token: 0x170009CD RID: 2509
	// (get) Token: 0x06005EDF RID: 24287 RVA: 0x002678F1 File Offset: 0x00265AF1
	// (set) Token: 0x06005EE0 RID: 24288 RVA: 0x002678F9 File Offset: 0x00265AF9
	public EntityPlayer LocalPlayer { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x06005EE1 RID: 24289 RVA: 0x00267904 File Offset: 0x00265B04
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("item");
		this.PrefabItems = childById.ViewComponent.UiTransform;
		this.height = (float)(childById.ViewComponent.Size.y + 2);
		childById.xui.BuffPopoutList = this;
	}

	// Token: 0x06005EE2 RID: 24290 RVA: 0x0026795C File Offset: 0x00265B5C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.LocalPlayer == null && XUi.IsGameRunning())
		{
			this.LocalPlayer = base.xui.playerUI.entityPlayer;
		}
		GUIWindowManager windowManager = base.xui.playerUI.windowManager;
		if (windowManager.IsHUDEnabled() || (base.xui.dragAndDrop.InMenu && windowManager.IsHUDPartialHidden()))
		{
			if (base.ViewComponent.IsVisible && this.LocalPlayer.IsDead())
			{
				base.ViewComponent.IsVisible = false;
			}
			else if (!base.ViewComponent.IsVisible && !this.LocalPlayer.IsDead())
			{
				base.ViewComponent.IsVisible = true;
			}
		}
		else
		{
			base.ViewComponent.IsVisible = false;
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			XUiC_BuffPopoutList.Data data = this.items[i];
			if (data.Notification.Buff != null && data.Notification.Buff.Paused)
			{
				this.removeEntry(data.Notification, i);
				this.disabledItems.Add(data.Notification);
			}
			else
			{
				if (data.Notification.DisplayMode == EnumEntityUINotificationDisplayMode.IconPlusCurrentValue)
				{
					UILabel component = data.Item.transform.Find("TextContent").GetComponent<UILabel>();
					string units = data.Notification.Units;
					if (!(units == "%"))
					{
						if (!(units == "°"))
						{
							if (!(units == "cvar"))
							{
								if (!(units == "duration"))
								{
									if (data.Notification.Buff.BuffClass.DisplayValueKey != null)
									{
										if (data.Notification.Buff.BuffClass.DisplayValueFormat == BuffClass.CVarDisplayFormat.Time)
										{
											component.text = string.Format(Localization.Get(data.Notification.Buff.BuffClass.DisplayValueKey, false), XUiC_BuffPopoutList.GetCVarValueAsTimeString(data.Notification.CurrentValue));
										}
										else
										{
											component.text = string.Format(Localization.Get(data.Notification.Buff.BuffClass.DisplayValueKey, false), data.Notification.CurrentValue);
										}
									}
									else
									{
										component.text = ((int)data.Notification.CurrentValue).ToString();
									}
								}
								else
								{
									component.text = XUiC_BuffPopoutList.GetCVarValueAsTimeString(data.Notification.Buff.BuffClass.DurationMax - data.Notification.Buff.DurationInSeconds);
								}
							}
							else if (data.Notification.Buff.BuffClass.DisplayValueKey != null)
							{
								if (data.Notification.Buff.BuffClass.DisplayValueFormat == BuffClass.CVarDisplayFormat.Time)
								{
									component.text = string.Format(Localization.Get(data.Notification.Buff.BuffClass.DisplayValueKey, false), XUiC_BuffPopoutList.GetCVarValueAsTimeString(data.Notification.CurrentValue));
								}
								else
								{
									component.text = string.Format(Localization.Get(data.Notification.Buff.BuffClass.DisplayValueKey, false), data.Notification.CurrentValue);
								}
							}
							else if (data.Notification.Buff.BuffClass.DisplayValueFormat == BuffClass.CVarDisplayFormat.Time)
							{
								component.text = XUiC_BuffPopoutList.GetCVarValueAsTimeString(data.Notification.CurrentValue);
							}
							else
							{
								component.text = ((int)data.Notification.CurrentValue).ToString();
							}
						}
						else if (component != null)
						{
							component.text = ValueDisplayFormatters.Temperature(data.Notification.CurrentValue, 0);
						}
					}
					else if (component != null)
					{
						component.text = ((int)(data.Notification.CurrentValue * 100f)).ToString() + "%";
					}
				}
				else
				{
					EnumEntityUINotificationDisplayMode displayMode = data.Notification.DisplayMode;
				}
				bool flag = false;
				if (data.Notification.Buff != null)
				{
					flag = (EffectManager.GetValue(PassiveEffects.BuffBlink, null, 0f, this.LocalPlayer, null, data.Notification.Buff.BuffClass.NameTag, false, false, false, true, true, 1, true, false) >= 1f);
				}
				if (data.Notification.Buff != null && (data.Notification.Buff.BuffClass.IconBlink || flag))
				{
					Color color = data.Notification.GetColor();
					float num = Mathf.PingPong(Time.time, 0.5f);
					data.Sprite.color = Color.Lerp(Color.grey, color, num * 4f);
					float num2 = 1f;
					if (num > 0.25f)
					{
						num2 = 1f + num - 0.25f;
					}
					data.Sprite.SetDimensions((int)(this.spriteSize.x * num2), (int)(this.spriteSize.y * num2));
				}
				else
				{
					data.Sprite.color = data.Notification.GetColor();
					data.Sprite.SetDimensions((int)this.spriteSize.x, (int)this.spriteSize.y);
				}
			}
		}
		if (this.disabledItems.Count > 0)
		{
			for (int j = this.disabledItems.Count - 1; j >= 0; j--)
			{
				EntityUINotification entityUINotification = this.disabledItems[j];
				if (!entityUINotification.Buff.Paused)
				{
					this.AddNotification(entityUINotification);
					this.disabledItems.RemoveAt(j);
				}
			}
			this.updateEntries();
		}
	}

	// Token: 0x06005EE3 RID: 24291 RVA: 0x00267F1A File Offset: 0x0026611A
	public static string GetCVarValueAsTimeString(float cvarValue)
	{
		return XUiM_PlayerBuffs.GetCVarValueAsTimeString(cvarValue);
	}

	// Token: 0x06005EE4 RID: 24292 RVA: 0x00267F24 File Offset: 0x00266124
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.initialized)
		{
			this.PrefabItems.gameObject.SetActive(false);
			EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
			List<EntityUINotification> notifications = entityPlayer.PlayerStats.Notifications;
			for (int i = 0; i < notifications.Count; i++)
			{
				this.AddNotification(notifications[i]);
			}
			entityPlayer.PlayerStats.AddUINotificationChangedDelegate(this);
			this.initialized = true;
		}
	}

	// Token: 0x06005EE5 RID: 24293 RVA: 0x00267FA0 File Offset: 0x002661A0
	public override void OnClose()
	{
		base.OnClose();
		EntityPlayer entityPlayer = base.xui.playerUI.entityPlayer;
		if (entityPlayer != null)
		{
			entityPlayer.PlayerStats.RemoveUINotificationChangedDelegate(this);
		}
		this.initialized = false;
		for (int i = 0; i < this.items.Count; i++)
		{
			UnityEngine.Object.Destroy(this.items[i].Item.gameObject);
		}
		this.items.Clear();
	}

	// Token: 0x06005EE6 RID: 24294 RVA: 0x0026801C File Offset: 0x0026621C
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeEntry(EntityUINotification notification, int currentIndex = -1)
	{
		int num = (currentIndex == -1) ? this.GetNotificationIndex(notification) : currentIndex;
		if (num == -1)
		{
			return;
		}
		TemporaryObject temporaryObject = this.items[num].Item.transform.GetComponent<TemporaryObject>();
		if (temporaryObject == null)
		{
			temporaryObject = this.items[num].Item.transform.gameObject.AddComponent<TemporaryObject>();
		}
		temporaryObject.enabled = true;
		TweenColor tweenColor = this.items[num].Item.transform.GetComponent<TweenColor>();
		if (tweenColor == null)
		{
			tweenColor = this.items[num].Item.transform.gameObject.AddComponent<TweenColor>();
		}
		tweenColor.from = Color.white;
		tweenColor.to = new Color(1f, 1f, 1f, 0f);
		tweenColor.enabled = true;
		tweenColor.duration = 0.4f;
		TweenScale tweenScale = this.items[num].Item.gameObject.AddComponent<TweenScale>();
		tweenScale.from = Vector3.one;
		tweenScale.to = Vector3.zero;
		tweenScale.enabled = true;
		tweenScale.duration = 0.5f;
		this.items.RemoveAt(num);
		this.updateEntries();
	}

	// Token: 0x06005EE7 RID: 24295 RVA: 0x00268160 File Offset: 0x00266360
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetNotificationIndex(EntityUINotification notification)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].Notification.Subject == notification.Subject)
			{
				if (notification.Subject != EnumEntityUINotificationSubject.Buff)
				{
					return i;
				}
				if (this.items[i].Notification.Buff.BuffClass.ShowOnHUD && this.items[i].Notification.Buff.BuffClass.Name == notification.Buff.BuffClass.Name)
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x06005EE8 RID: 24296 RVA: 0x00268210 File Offset: 0x00266410
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateEntries()
	{
		int num = 0;
		for (int i = 0; i < this.items.Count; i++)
		{
			if (!this.items[i].Notification.Buff.Paused)
			{
				TweenPosition tweenPosition = this.items[i].Item.GetComponent<TweenPosition>();
				if (tweenPosition)
				{
					UnityEngine.Object.Destroy(tweenPosition);
				}
				tweenPosition = this.items[i].Item.AddComponent<TweenPosition>();
				tweenPosition.from = this.items[i].Item.transform.localPosition;
				tweenPosition.to = new Vector3(this.items[i].Item.transform.localPosition.x, (float)num * this.height + (float)this.yOffset, this.items[i].Item.transform.localPosition.z);
				tweenPosition.enabled = true;
				num++;
			}
		}
	}

	// Token: 0x06005EE9 RID: 24297 RVA: 0x0026831F File Offset: 0x0026651F
	public void EntityUINotificationAdded(EntityUINotification _notification)
	{
		this.AddNotification(_notification);
	}

	// Token: 0x06005EEA RID: 24298 RVA: 0x00268328 File Offset: 0x00266528
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddNotification(EntityUINotification _notification)
	{
		int notificationIndex = this.GetNotificationIndex(_notification);
		if (notificationIndex == -1)
		{
			if (_notification.Icon != "")
			{
				if (_notification.Buff != null)
				{
					if (!_notification.Buff.BuffClass.ShowOnHUD)
					{
						return;
					}
					if (_notification.Buff.Paused)
					{
						this.disabledItems.Add(_notification);
					}
				}
				GameObject gameObject = base.ViewComponent.UiTransform.gameObject.AddChild(this.PrefabItems.gameObject);
				gameObject.SetActive(true);
				gameObject.GetComponent<BoxCollider>().center = Vector3.zero;
				gameObject.GetComponent<UIPanel>();
				UIEventListener uieventListener = UIEventListener.Get(gameObject.gameObject);
				uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.OnNotificationClicked));
				gameObject.transform.Find("Background").GetComponent<UISprite>().color = Color.white;
				UISprite component = gameObject.transform.Find("Icon").GetComponent<UISprite>();
				component.atlas = base.xui.GetAtlasByName(((UnityEngine.Object)component.atlas).name, _notification.Icon);
				component.spriteName = _notification.Icon;
				component.color = _notification.GetColor();
				if (this.spriteSize == Vector2.zero)
				{
					this.spriteSize = new Vector2((float)component.width, (float)component.height);
				}
				UILabel component2 = gameObject.transform.Find("TextContent").GetComponent<UILabel>();
				if (_notification.DisplayMode == EnumEntityUINotificationDisplayMode.IconPlusCurrentValue)
				{
					string units = _notification.Units;
					if (!(units == "%"))
					{
						if (!(units == "°"))
						{
							if (component2 != null)
							{
								component2.text = _notification.CurrentValue.ToCultureInvariantString("0");
							}
						}
						else if (component2 != null)
						{
							component2.text = _notification.CurrentValue.ToCultureInvariantString("0") + "°";
						}
					}
					else if (component2 != null)
					{
						component2.text = (_notification.CurrentValue * 100f).ToCultureInvariantString("0") + "%";
					}
				}
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, (float)this.items.Count * this.height + (float)this.yOffset, gameObject.transform.localPosition.z);
				XUiC_BuffPopoutList.Data data = new XUiC_BuffPopoutList.Data();
				data.Item = gameObject;
				data.TimeAdded = Time.time;
				data.Notification = _notification;
				data.Sprite = component;
				if (_notification.Buff != null && _notification.Buff.BuffClass.TooltipKey != null)
				{
					GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, _notification.Buff.BuffClass.TooltipKey, false, false, 0f);
				}
				this.items.Add(data);
				return;
			}
		}
		else
		{
			this.items[notificationIndex].Notification = _notification;
			this.items[notificationIndex].TimeAdded = Time.time;
		}
	}

	// Token: 0x06005EEB RID: 24299 RVA: 0x0026864C File Offset: 0x0026684C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnNotificationClicked(GameObject go)
	{
		this.HandleClickForItem(go);
	}

	// Token: 0x06005EEC RID: 24300 RVA: 0x00268658 File Offset: 0x00266858
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleClickForItem(GameObject go)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].Item == go)
			{
				Manager.PlayInsidePlayerHead("craft_click_craft", -1, 0f, false, false);
				if (!base.xui.playerUI.windowManager.IsWindowOpen("character"))
				{
					XUiC_WindowSelector.OpenSelectorAndWindow(base.xui.playerUI.entityPlayer, "character");
				}
				this.SelectedNotification = this.items[i].Notification;
				return;
			}
		}
	}

	// Token: 0x06005EED RID: 24301 RVA: 0x002686F7 File Offset: 0x002668F7
	public void EntityUINotificationRemoved(EntityUINotification _notification)
	{
		this.removeEntry(_notification, -1);
	}

	// Token: 0x06005EEE RID: 24302 RVA: 0x00268701 File Offset: 0x00266901
	public void SetYOffset(int _yOffset)
	{
		if (_yOffset != this.yOffset)
		{
			this.yOffset = _yOffset;
			this.updateEntries();
		}
	}

	// Token: 0x04004796 RID: 18326
	[PublicizedFrom(EAccessModifier.Private)]
	public bool initialized;

	// Token: 0x04004797 RID: 18327
	[PublicizedFrom(EAccessModifier.Private)]
	public float height;

	// Token: 0x04004798 RID: 18328
	[PublicizedFrom(EAccessModifier.Private)]
	public int yOffset;

	// Token: 0x04004799 RID: 18329
	public Transform PrefabItems;

	// Token: 0x0400479B RID: 18331
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2 spriteSize;

	// Token: 0x0400479C RID: 18332
	public EntityUINotification SelectedNotification;

	// Token: 0x0400479D RID: 18333
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_BuffPopoutList.Data> items = new List<XUiC_BuffPopoutList.Data>();

	// Token: 0x0400479E RID: 18334
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityUINotification> disabledItems = new List<EntityUINotification>();

	// Token: 0x02000C13 RID: 3091
	[PublicizedFrom(EAccessModifier.Private)]
	public class Data
	{
		// Token: 0x0400479F RID: 18335
		public GameObject Item;

		// Token: 0x040047A0 RID: 18336
		public float TimeAdded;

		// Token: 0x040047A1 RID: 18337
		public EntityUINotification Notification;

		// Token: 0x040047A2 RID: 18338
		public UISprite Sprite;
	}
}

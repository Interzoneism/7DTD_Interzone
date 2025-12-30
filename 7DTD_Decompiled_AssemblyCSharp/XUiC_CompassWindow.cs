using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C5B RID: 3163
[Preserve]
public class XUiC_CompassWindow : XUiController
{
	// Token: 0x06006168 RID: 24936 RVA: 0x00277FE0 File Offset: 0x002761E0
	public override void Init()
	{
		base.Init();
		XUiC_CompassWindow.ID = base.WindowGroup.ID;
		for (int i = 0; i < 50; i++)
		{
			UISprite uisprite = new GameObject
			{
				transform = 
				{
					parent = base.ViewComponent.UiTransform
				}
			}.AddComponent<UISprite>();
			this.waypointSpriteList.Add(uisprite);
			uisprite.atlas = base.xui.GetAtlasByName("UIAtlas", "menu_empty");
			uisprite.transform.localScale = Vector3.one;
			uisprite.spriteName = "menu_empty";
			uisprite.SetDimensions(20, 20);
			uisprite.color = Color.clear;
			uisprite.pivot = UIWidget.Pivot.Center;
			uisprite.depth = 12;
			uisprite.gameObject.layer = 12;
		}
	}

	// Token: 0x06006169 RID: 24937 RVA: 0x002780AC File Offset: 0x002762AC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!this.localPlayer)
		{
			this.localPlayer = base.xui.playerUI.entityPlayer;
			if (!this.localPlayer)
			{
				return;
			}
		}
		base.ViewComponent.IsVisible = (!this.localPlayer.IsDead() && base.xui.playerUI.windowManager.IsHUDEnabled());
		if (this.localPlayer.playerCamera != null)
		{
			World world = GameManager.Instance.World;
			this.showSleeperVolumes = true;
			int num = 0;
			this.updateNavObjects(this.localPlayer, ref num);
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.SleepingBag));
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.LandClaim));
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.MapMarker));
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.MapQuickMarker));
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.Backpack));
			if (this.showSleeperVolumes)
			{
				this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.Quest));
			}
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.TreasureChest));
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.FetchItem));
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.HiddenCache));
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.RestorePower));
			if (this.showSleeperVolumes)
			{
				this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.SleeperVolume));
			}
			this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.VendingMachine));
			if (GameStats.GetBool(EnumGameStats.AirDropMarker))
			{
				this.updateMarkers(this.localPlayer, ref num, world.GetObjectOnMapList(EnumMapObjectType.SupplyDrop));
			}
			Color clear = Color.clear;
			for (int i = num; i < this.waypointSpriteList.Count; i++)
			{
				this.waypointSpriteList[i].color = clear;
			}
		}
		if (XUi.IsGameRunning())
		{
			base.RefreshBindings(false);
		}
	}

	// Token: 0x0600616A RID: 24938 RVA: 0x002782C8 File Offset: 0x002764C8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1586863888U)
		{
			if (num <= 1021729756U)
			{
				if (num != 85644157U)
				{
					if (num == 1021729756U)
					{
						if (bindingName == "daytime")
						{
							value = "";
							if (XUi.IsGameRunning())
							{
								value = this.daytimeFormatter.Format(GameManager.Instance.World.worldTime);
							}
							return true;
						}
					}
				}
				else if (bindingName == "showtime")
				{
					if (this.localPlayer != null)
					{
						value = (EffectManager.GetValue(PassiveEffects.NoTimeDisplay, null, 0f, this.localPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) == 0f).ToString();
					}
					else
					{
						value = "true";
					}
					return true;
				}
			}
			else if (num != 1564253156U)
			{
				if (num == 1586863888U)
				{
					if (bindingName == "daycolor")
					{
						value = "FFFFFF";
						if (XUi.IsGameRunning())
						{
							ulong worldTime = GameManager.Instance.World.worldTime;
							int @int = GameStats.GetInt(EnumGameStats.BloodMoonWarning);
							ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(worldTime);
							int item = valueTuple.Item1;
							int item2 = valueTuple.Item2;
							if (@int != -1 && GameStats.GetInt(EnumGameStats.BloodMoonDay) == item && @int <= item2)
							{
								value = "FF0000";
							}
						}
						return true;
					}
				}
			}
			else if (bindingName == "time")
			{
				value = "";
				if (XUi.IsGameRunning())
				{
					ValueTuple<int, int, int> valueTuple2 = GameUtils.WorldTimeToElements(GameManager.Instance.World.worldTime);
					int item3 = valueTuple2.Item2;
					int item4 = valueTuple2.Item3;
					value = this.timeFormatter.Format(item3, item4);
				}
				return true;
			}
		}
		else if (num <= 1978910129U)
		{
			if (num != 1971063990U)
			{
				if (num == 1978910129U)
				{
					if (bindingName == "daytitle")
					{
						value = Localization.Get("xuiDay", false);
						return true;
					}
				}
			}
			else if (bindingName == "compass_rotation")
			{
				if (this.localPlayer != null && this.localPlayer.playerCamera != null)
				{
					value = this.localPlayer.playerCamera.transform.eulerAngles.y.ToString();
				}
				else
				{
					value = "0.0";
				}
				return true;
			}
		}
		else if (num != 2235205906U)
		{
			if (num != 2899617242U)
			{
				if (num == 3830391293U)
				{
					if (bindingName == "day")
					{
						value = "0";
						if (XUi.IsGameRunning())
						{
							int v = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
							value = this.dayFormatter.Format(v);
						}
						return true;
					}
				}
			}
			else if (bindingName == "compass_language")
			{
				if (GamePrefs.GetBool(EnumGamePrefs.OptionsUiCompassUseEnglishCardinalDirections))
				{
					value = Localization.DefaultLanguage;
				}
				else
				{
					value = Localization.language;
				}
				return true;
			}
		}
		else if (bindingName == "timetitle")
		{
			value = Localization.Get("xuiTime", false);
			return true;
		}
		return base.GetBindingValueInternal(ref value, bindingName);
	}

	// Token: 0x0600616B RID: 24939 RVA: 0x00278604 File Offset: 0x00276804
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateMarkers(EntityPlayerLocal localPlayer, ref int waypointSpriteIndex, List<MapObject> _mapObjectList)
	{
		int count = _mapObjectList.Count;
		if (count == 0)
		{
			return;
		}
		float num = (float)base.ViewComponent.Size.x * 0.5f;
		float num2 = num * 1.15f;
		Transform cameraTransform = localPlayer.cameraTransform;
		Entity entity = (localPlayer.AttachedToEntity != null) ? localPlayer.AttachedToEntity : localPlayer;
		Vector3 position = entity.GetPosition();
		Vector2 b = new Vector2(position.x, position.z);
		Vector3 forward = cameraTransform.forward;
		Vector2 rhs = new Vector2(forward.x, forward.z);
		rhs.Normalize();
		Vector3 right = cameraTransform.right;
		Vector2 rhs2 = new Vector2(right.x, right.z);
		rhs2.Normalize();
		for (int i = 0; i < count; i++)
		{
			MapObject mapObject = _mapObjectList[i];
			mapObject.RefreshData();
			if (waypointSpriteIndex >= this.waypointSpriteList.Count)
			{
				break;
			}
			if (mapObject.IsOnCompass())
			{
				if (mapObject is MapObjectZombie)
				{
					this.showSleeperVolumes = false;
				}
				Vector3 position2 = mapObject.GetPosition();
				Vector2 vector = new Vector2(position2.x, position2.z) - b;
				float magnitude = vector.magnitude;
				bool flag = true;
				if (mapObject.type == EnumMapObjectType.TreasureChest)
				{
					float num3 = (float)(mapObject as MapObjectTreasureChest).DefaultRadius;
					float num4 = EffectManager.GetValue(PassiveEffects.TreasureRadius, null, num3, localPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
					num4 = Utils.FastClamp(num4, 0f, num3);
					if (magnitude < num4)
					{
						float num5 = Mathf.PingPong(Time.time, 0.25f);
						float num6 = 1.25f + num5;
						this.waypointSpriteList[waypointSpriteIndex].atlas = base.xui.GetAtlasByName("UIAtlas", mapObject.GetMapIcon());
						this.waypointSpriteList[waypointSpriteIndex].spriteName = mapObject.GetMapIcon();
						this.waypointSpriteList[waypointSpriteIndex].SetDimensions((int)(25f * num6), (int)(25f * num6));
						this.waypointSpriteList[waypointSpriteIndex].transform.localPosition = new Vector3(num, -24f);
						Color mapIconColor = mapObject.GetMapIconColor();
						this.waypointSpriteList[waypointSpriteIndex].color = Color.Lerp(mapIconColor, Color.red, num5 * 4f);
						waypointSpriteIndex++;
						flag = false;
					}
				}
				string spriteName = mapObject.GetCompassIcon();
				if (mapObject.type == EnumMapObjectType.HiddenCache)
				{
					this.waypointSpriteList[waypointSpriteIndex].flip = UIBasicSprite.Flip.Nothing;
					if (position2.y < localPlayer.GetPosition().y - 2f)
					{
						spriteName = mapObject.GetCompassDownIcon();
					}
					else if (position2.y > localPlayer.GetPosition().y + 2f)
					{
						spriteName = mapObject.GetCompassUpIcon();
					}
					this.waypointSpriteList[waypointSpriteIndex].depth = 100;
					this.waypointSpriteList[waypointSpriteIndex].atlas = base.xui.GetAtlasByName("UIAtlas", spriteName);
					this.waypointSpriteList[waypointSpriteIndex].spriteName = spriteName;
					if ((position2 - entity.GetPosition()).magnitude < 10f)
					{
						float num7 = Mathf.PingPong(Time.time, 0.25f);
						float num8 = 1.25f + num7;
						this.waypointSpriteList[waypointSpriteIndex].SetDimensions((int)(25f * num8), (int)(25f * num8));
						this.waypointSpriteList[waypointSpriteIndex].transform.localPosition = new Vector3(num, -24f);
						Color mapIconColor2 = mapObject.GetMapIconColor();
						this.waypointSpriteList[waypointSpriteIndex].color = Color.Lerp(mapIconColor2, Color.grey, num7 * 4f);
						waypointSpriteIndex++;
						flag = false;
					}
				}
				if (mapObject.UseUpDownCompassIcons())
				{
					this.waypointSpriteList[waypointSpriteIndex].flip = UIBasicSprite.Flip.Nothing;
					if (position2.y < localPlayer.GetPosition().y - 2f)
					{
						spriteName = mapObject.GetCompassDownIcon();
					}
					else if (position2.y > localPlayer.GetPosition().y + 3f)
					{
						spriteName = mapObject.GetCompassUpIcon();
					}
					this.waypointSpriteList[waypointSpriteIndex].depth = 100;
					this.waypointSpriteList[waypointSpriteIndex].atlas = base.xui.GetAtlasByName("UIAtlas", spriteName);
					this.waypointSpriteList[waypointSpriteIndex].spriteName = spriteName;
				}
				if (flag)
				{
					Vector2 normalized = vector.normalized;
					if (!mapObject.IsCompassIconClamped() && Vector2.Dot(normalized, rhs) < 0.75f)
					{
						this.waypointSpriteList[waypointSpriteIndex].color = Color.clear;
					}
					else
					{
						float num9 = mapObject.GetCompassIconScale(magnitude);
						this.waypointSpriteList[waypointSpriteIndex].color = mapObject.GetMapIconColor();
						if (mapObject.IsTracked() && mapObject.NearbyCompassBlink() && (position2 - entity.GetPosition()).magnitude <= 6f)
						{
							Color mapIconColor3 = mapObject.GetMapIconColor();
							float num10 = Mathf.PingPong(Time.time, 0.5f);
							this.waypointSpriteList[waypointSpriteIndex].color = Color.Lerp(Color.grey, mapIconColor3, num10 * 4f);
							if (num10 > 0.25f)
							{
								num9 += num10 - 0.25f;
							}
						}
						this.waypointSpriteList[waypointSpriteIndex].atlas = base.xui.GetAtlasByName("UIAtlas", spriteName);
						this.waypointSpriteList[waypointSpriteIndex].spriteName = spriteName;
						this.waypointSpriteList[waypointSpriteIndex].SetDimensions((int)(25f * num9), (int)(25f * num9));
						if (Vector2.Dot(normalized, rhs) >= 0.75f)
						{
							this.waypointSpriteList[waypointSpriteIndex].transform.localPosition = new Vector3(num + Vector2.Dot(normalized, rhs2) * num2, -16f);
						}
						else
						{
							this.waypointSpriteList[waypointSpriteIndex].transform.localPosition = new Vector3(num + ((Vector2.Dot(normalized, rhs2) < 0f) ? -0.675f : 0.675f) * num2, -16f);
						}
						if (mapObject.type == EnumMapObjectType.Entity)
						{
							this.waypointSpriteList[waypointSpriteIndex].depth = 12 + (int)(num9 * 100f);
						}
						if (!mapObject.IsTracked())
						{
							Color mapIconColor4 = mapObject.GetMapIconColor();
							this.waypointSpriteList[waypointSpriteIndex].color = new Color(mapIconColor4.r * 0.75f, mapIconColor4.g * 0.75f, mapIconColor4.b * 0.75f) * num9;
						}
						waypointSpriteIndex++;
					}
				}
			}
		}
	}

	// Token: 0x0600616C RID: 24940 RVA: 0x00278CF0 File Offset: 0x00276EF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateNavObjects(EntityPlayerLocal localPlayer, ref int waypointSpriteIndex)
	{
		float num = (float)base.ViewComponent.Size.x * 0.5f;
		float num2 = num * 1.15f;
		Transform cameraTransform = localPlayer.cameraTransform;
		Entity entity = (localPlayer.AttachedToEntity != null) ? localPlayer.AttachedToEntity : localPlayer;
		Vector3 position = entity.GetPosition();
		Vector2 b = new Vector2(position.x, position.z);
		Vector3 forward = cameraTransform.forward;
		Vector2 rhs = new Vector2(forward.x, forward.z);
		rhs.Normalize();
		Vector3 right = cameraTransform.right;
		Vector2 rhs2 = new Vector2(right.x, right.z);
		rhs2.Normalize();
		List<NavObject> navObjectList = NavObjectManager.Instance.NavObjectList;
		for (int i = 0; i < navObjectList.Count; i++)
		{
			NavObject navObject = navObjectList[i];
			if (!navObject.hiddenOnCompass && navObject.IsValid())
			{
				if (waypointSpriteIndex >= this.waypointSpriteList.Count)
				{
					break;
				}
				NavObjectCompassSettings currentCompassSettings = navObject.CurrentCompassSettings;
				if (currentCompassSettings != null)
				{
					Vector3 position2 = navObject.GetPosition();
					Vector2 vector = new Vector2(position2.x + Origin.position.x, position2.z + Origin.position.z) - b;
					float magnitude = vector.magnitude;
					if (magnitude >= currentCompassSettings.MinDistance)
					{
						float maxDistance = navObject.GetMaxDistance(currentCompassSettings, localPlayer);
						if (maxDistance == -1f || magnitude <= maxDistance)
						{
							bool flag = true;
							string spriteName = navObject.GetSpriteName(currentCompassSettings);
							this.waypointSpriteList[waypointSpriteIndex].depth = 12 + currentCompassSettings.DepthOffset;
							if (currentCompassSettings.HotZone != null)
							{
								float num3 = 1f;
								if (currentCompassSettings.HotZone.HotZoneType == NavObjectCompassSettings.HotZoneSettings.HotZoneTypes.Treasure)
								{
									float extraData = navObject.ExtraData;
									num3 = EffectManager.GetValue(PassiveEffects.TreasureRadius, null, extraData, localPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
									num3 = Utils.FastClamp(num3, 0f, extraData);
								}
								else if (currentCompassSettings.HotZone.HotZoneType == NavObjectCompassSettings.HotZoneSettings.HotZoneTypes.Custom)
								{
									num3 = currentCompassSettings.HotZone.CustomDistance;
								}
								if (magnitude < num3)
								{
									float num4 = Mathf.PingPong(Time.time, 0.25f);
									float num5 = 1.25f + num4;
									this.waypointSpriteList[waypointSpriteIndex].atlas = base.xui.GetAtlasByName("UIAtlas", currentCompassSettings.HotZone.SpriteName);
									this.waypointSpriteList[waypointSpriteIndex].spriteName = currentCompassSettings.HotZone.SpriteName;
									this.waypointSpriteList[waypointSpriteIndex].SetDimensions((int)(25f * num5), (int)(25f * num5));
									this.waypointSpriteList[waypointSpriteIndex].transform.localPosition = new Vector3(num, -24f);
									Color color = currentCompassSettings.HotZone.Color;
									this.waypointSpriteList[waypointSpriteIndex].color = Color.Lerp(color, Color.red, num4 * 4f);
									waypointSpriteIndex++;
									flag = false;
								}
							}
							if (currentCompassSettings.ShowVerticalCompassIcons)
							{
								this.waypointSpriteList[waypointSpriteIndex].flip = UIBasicSprite.Flip.Nothing;
								float num6 = localPlayer.GetPosition().y - Origin.position.y;
								if (position2.y < num6 + currentCompassSettings.ShowDownOffset)
								{
									spriteName = currentCompassSettings.DownSpriteName;
								}
								else if (position2.y > num6 + currentCompassSettings.ShowUpOffset)
								{
									spriteName = currentCompassSettings.UpSpriteName;
								}
								this.waypointSpriteList[waypointSpriteIndex].depth = 100;
								this.waypointSpriteList[waypointSpriteIndex].atlas = base.xui.GetAtlasByName("UIAtlas", spriteName);
								this.waypointSpriteList[waypointSpriteIndex].spriteName = spriteName;
							}
							if (flag)
							{
								Vector2 normalized = vector.normalized;
								if (!currentCompassSettings.IconClamped && Vector2.Dot(normalized, rhs) < 0.75f)
								{
									this.waypointSpriteList[waypointSpriteIndex].color = Color.clear;
								}
								else
								{
									float num7 = navObject.GetCompassIconScale(magnitude);
									this.waypointSpriteList[waypointSpriteIndex].color = (navObject.UseOverrideColor ? navObject.OverrideColor : currentCompassSettings.Color);
									if (currentCompassSettings.HasPulse && (position2 - entity.GetPosition()).magnitude <= 6f)
									{
										Color b2 = navObject.UseOverrideColor ? navObject.OverrideColor : currentCompassSettings.Color;
										float num8 = Mathf.PingPong(Time.time, 0.5f);
										this.waypointSpriteList[waypointSpriteIndex].color = Color.Lerp(Color.grey, b2, num8 * 4f);
										if (num8 > 0.25f)
										{
											num7 += num8 - 0.25f;
										}
									}
									this.waypointSpriteList[waypointSpriteIndex].atlas = base.xui.GetAtlasByName("UIAtlas", spriteName);
									this.waypointSpriteList[waypointSpriteIndex].spriteName = spriteName;
									this.waypointSpriteList[waypointSpriteIndex].SetDimensions((int)(25f * num7), (int)(25f * num7));
									if (Vector2.Dot(normalized, rhs) >= 0.75f)
									{
										this.waypointSpriteList[waypointSpriteIndex].transform.localPosition = new Vector3(num + Vector2.Dot(normalized, rhs2) * num2, -16f);
									}
									else
									{
										this.waypointSpriteList[waypointSpriteIndex].transform.localPosition = new Vector3(num + ((Vector2.Dot(normalized, rhs2) < 0f) ? -0.675f : 0.675f) * num2, -16f);
									}
									if (!navObject.IsActive)
									{
										Color color2 = navObject.UseOverrideColor ? navObject.OverrideColor : currentCompassSettings.Color;
										if (currentCompassSettings.MinFadePercent != -1f)
										{
											if (currentCompassSettings.MinFadePercent > num7)
											{
												num7 = currentCompassSettings.MinFadePercent;
											}
											this.waypointSpriteList[waypointSpriteIndex].color = color2 * num7;
										}
										else
										{
											this.waypointSpriteList[waypointSpriteIndex].color = color2;
										}
									}
									waypointSpriteIndex++;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600616D RID: 24941 RVA: 0x00279336 File Offset: 0x00277536
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
	}

	// Token: 0x04004941 RID: 18753
	public static string ID = "";

	// Token: 0x04004942 RID: 18754
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal localPlayer;

	// Token: 0x04004943 RID: 18755
	public List<UISprite> waypointSpriteList = new List<UISprite>();

	// Token: 0x04004944 RID: 18756
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<ulong> daytimeFormatter = new CachedStringFormatter<ulong>((ulong _worldTime) => ValueDisplayFormatters.WorldTime(_worldTime, Localization.Get("xuiDayTimeLong", false)));

	// Token: 0x04004945 RID: 18757
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt dayFormatter = new CachedStringFormatterInt();

	// Token: 0x04004946 RID: 18758
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int, int> timeFormatter = new CachedStringFormatter<int, int>((int _hour, int _min) => string.Format("{0:00}:{1:00}", _hour, _min));

	// Token: 0x04004947 RID: 18759
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showSleeperVolumes;
}

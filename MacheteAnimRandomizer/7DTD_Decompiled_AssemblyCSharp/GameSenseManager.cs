using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SteelSeries.GameSense;
using SteelSeries.GameSense.DeviceZone;
using UnityEngine;

// Token: 0x02000F8D RID: 3981
public class GameSenseManager : IEntityBuffsChanged
{
	// Token: 0x17000D3E RID: 3390
	// (get) Token: 0x06007EDE RID: 32478 RVA: 0x003387E7 File Offset: 0x003369E7
	// (set) Token: 0x06007EDD RID: 32477 RVA: 0x003387DF File Offset: 0x003369DF
	public static GameSenseManager Instance { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06007EDF RID: 32479 RVA: 0x003387EE File Offset: 0x003369EE
	[PublicizedFrom(EAccessModifier.Private)]
	static GameSenseManager()
	{
		if (GameManager.IsDedicatedServer || Application.isEditor)
		{
			return;
		}
		if (GameUtils.GetLaunchArgument("nogamesense") != null)
		{
			return;
		}
		GameSenseManager.Instance = new GameSenseManager();
	}

	// Token: 0x17000D3F RID: 3391
	// (get) Token: 0x06007EE0 RID: 32480 RVA: 0x00338816 File Offset: 0x00336A16
	public static bool GameSenseInstalled
	{
		get
		{
			return SdFile.Exists(GSClient._getPropsPath());
		}
	}

	// Token: 0x06007EE1 RID: 32481 RVA: 0x00338824 File Offset: 0x00336A24
	public void Init()
	{
		if (!GameSenseManager.GameSenseInstalled)
		{
			Log.Out("GameSense server not found (no props file), disabling");
			GameSenseManager.Instance = null;
			return;
		}
		new GameObject("GameSense")
		{
			hideFlags = HideFlags.HideAndDontSave
		}.AddComponent<GSClient>();
		GSClient.Instance.RegisterGame("7dtd", "7 Days to Die", "The Fun Pimps");
		GameStats.OnChangedDelegates += this.GameStats_OnChanged;
		ThreadManager.StartCoroutine(this.BindEventsCo());
	}

	// Token: 0x06007EE2 RID: 32482 RVA: 0x00338897 File Offset: 0x00336A97
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator BindEventsCo()
	{
		yield return new WaitForSeconds(0.1f);
		this.BindEventTime();
		this.BindEventAmmo();
		this.BindEventHealth();
		this.BindEventDurability();
		this.BindEventStealth();
		this.BindEventBleeding();
		this.BindEventBloodmoon();
		this.BindEventCompass();
		this.BindEventHit();
		this.BindEventDukes();
		yield break;
	}

	// Token: 0x06007EE3 RID: 32483 RVA: 0x003388A8 File Offset: 0x00336AA8
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventTime()
	{
		ColorRanges color = ColorRanges.Create(new ColorRange[]
		{
			new ColorRange(0U, 16U, ColorStatic.Create(byte.MaxValue, 0, 0)),
			new ColorRange(17U, 32U, ColorStatic.Create(byte.MaxValue, 160, 0)),
			new ColorRange(33U, 49U, ColorStatic.Create(byte.MaxValue, byte.MaxValue, 0)),
			new ColorRange(50U, 66U, ColorStatic.Create(byte.MaxValue, byte.MaxValue, 0)),
			new ColorRange(67U, 74U, ColorStatic.Create(byte.MaxValue, 160, 0)),
			new ColorRange(75U, 82U, ColorStatic.Create(byte.MaxValue, 80, 0)),
			new ColorRange(83U, 91U, ColorStatic.Create(byte.MaxValue, 40, 0)),
			new ColorRange(92U, 100U, ColorStatic.Create(byte.MaxValue, 0, 0))
		});
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneFunctionKeys>(), IlluminationMode.Percent, color);
		this.BindEventWrapper("TIME", 0, 100, EventIconId.Clock, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EE4 RID: 32484 RVA: 0x003389BC File Offset: 0x00336BBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventAmmo()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneNumberKeys>(), IlluminationMode.Percent, ColorGradient.Create(new RGB(160, 0, byte.MaxValue), new RGB(0, 0, byte.MaxValue)));
		this.BindEventWrapper("AMMO", 0, 100, EventIconId.Ammo, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EE5 RID: 32485 RVA: 0x00338A10 File Offset: 0x00336C10
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventHealth()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneRowQ>(), IlluminationMode.Percent, ColorGradient.Create(new RGB(byte.MaxValue, 0, 0), new RGB(0, byte.MaxValue, 0)), RateRange.Create(new FreqRepeatLimitPair[]
		{
			new FreqRepeatLimitPair(0U, 10U, 2U, 0U),
			new FreqRepeatLimitPair(11U, 20U, 1U, 0U)
		}));
		this.BindEventWrapper("HEALTH", 0, 100, EventIconId.Health, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EE6 RID: 32486 RVA: 0x00338A90 File Offset: 0x00336C90
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventDurability()
	{
		RGBPerkeyZoneCustom rgbperkeyZoneCustom = ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
		rgbperkeyZoneCustom.zone = new byte[]
		{
			83,
			84,
			85,
			86
		};
		AbstractHandler abstractHandler = ColorHandler.Create(rgbperkeyZoneCustom, IlluminationMode.Percent, ColorGradient.Create(new RGB(130, 130, 0), new RGB(0, byte.MaxValue, 200)));
		this.BindEventWrapper("DURABILITY", 0, 100, EventIconId.Item, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EE7 RID: 32487 RVA: 0x00338B04 File Offset: 0x00336D04
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventStealth()
	{
		RGBPerkeyZoneCustom rgbperkeyZoneCustom = ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
		rgbperkeyZoneCustom.zone = new byte[]
		{
			224,
			225,
			57,
			43,
			53
		};
		AbstractHandler abstractHandler = ColorHandler.Create(rgbperkeyZoneCustom, IlluminationMode.Percent, ColorGradient.Create(new RGB(50, 120, 50), new RGB(0, 250, 250)));
		this.BindEventWrapper("STEALTH", 0, 100, EventIconId.Default, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EE8 RID: 32488 RVA: 0x00338B70 File Offset: 0x00336D70
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventBleeding()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneNavCluster>(), IlluminationMode.Color, ColorStatic.Create(byte.MaxValue, 0, 0), RateStatic.Create(5U, 0U));
		this.BindEventWrapper("BLEEDING", 0, 1, EventIconId.Default, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EE9 RID: 32489 RVA: 0x00338BB8 File Offset: 0x00336DB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventBloodmoon()
	{
		ColorRanges color = ColorRanges.Create(new ColorRange[]
		{
			new ColorRange(0U, 0U, ColorStatic.Create(0, 0, 0)),
			new ColorRange(1U, 10U, ColorStatic.Create(60, 0, 0)),
			new ColorRange(11U, 20U, ColorStatic.Create(70, 0, 0)),
			new ColorRange(21U, 30U, ColorStatic.Create(80, 0, 0)),
			new ColorRange(31U, 40U, ColorStatic.Create(90, 0, 0)),
			new ColorRange(41U, 50U, ColorStatic.Create(105, 0, 0)),
			new ColorRange(51U, 60U, ColorStatic.Create(120, 0, 0)),
			new ColorRange(61U, 70U, ColorStatic.Create(135, 0, 0)),
			new ColorRange(71U, 80U, ColorStatic.Create(150, 0, 0)),
			new ColorRange(81U, 90U, ColorStatic.Create(190, 0, 0)),
			new ColorRange(91U, 100U, ColorStatic.Create(byte.MaxValue, 0, 0))
		});
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<RGBPerkeyZoneRowA>(), IlluminationMode.Color, color, RateRange.Create(new FreqRepeatLimitPair[]
		{
			new FreqRepeatLimitPair(81U, 90U, 1U, 0U)
		}));
		this.BindEventWrapper("BLOODMOON", 0, 100, EventIconId.Default, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EEA RID: 32490 RVA: 0x00338D08 File Offset: 0x00336F08
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventCompass()
	{
		foreach (Tuple<string, byte> tuple in new Tuple<string, byte>[]
		{
			new Tuple<string, byte>("NORTH", 96),
			new Tuple<string, byte>("NORTHEAST", 97),
			new Tuple<string, byte>("EAST", 94),
			new Tuple<string, byte>("SOUTHEAST", 91),
			new Tuple<string, byte>("SOUTH", 90),
			new Tuple<string, byte>("SOUTHWEST", 89),
			new Tuple<string, byte>("WEST", 92),
			new Tuple<string, byte>("NORTHWEST", 95)
		})
		{
			RGBPerkeyZoneCustom rgbperkeyZoneCustom = ScriptableObject.CreateInstance<RGBPerkeyZoneCustom>();
			rgbperkeyZoneCustom.zone = new byte[]
			{
				tuple.Item2
			};
			AbstractHandler abstractHandler = ColorHandler.Create(rgbperkeyZoneCustom, IlluminationMode.Color, ColorStatic.Create(50, byte.MaxValue, 0));
			this.BindEventWrapper(tuple.Item1, 0, 1, EventIconId.Compass, false, new AbstractHandler[]
			{
				abstractHandler
			});
		}
	}

	// Token: 0x06007EEB RID: 32491 RVA: 0x00338DF4 File Offset: 0x00336FF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventHit()
	{
		AbstractHandler abstractHandler = ColorHandler.Create(ScriptableObject.CreateInstance<MouseZoneAll>(), IlluminationMode.Color, ColorStatic.Create(byte.MaxValue, 0, 0), RateStatic.Create(8U, 0U));
		this.BindEventWrapper("HIT", 0, 1, EventIconId.Headshot, false, new AbstractHandler[]
		{
			abstractHandler
		});
	}

	// Token: 0x06007EEC RID: 32492 RVA: 0x00338E3C File Offset: 0x0033703C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventDukes()
	{
		LineData lineData = LineData.Create(LineDataText.Create("Dukes: ", "", true, 0U), LineDataAccessor.ContextFrameKey("dukesstring"));
		FrameDataMultiLine frameDataMultiLine = FrameDataMultiLine.Create(new LineData[]
		{
			lineData
		}, new FrameModifiers(10000U, false), EventIconId.Money);
		AbstractScreenDevice_Zone dz = ScriptableObject.CreateInstance<ScreenedZoneOne>();
		ScreenMode mode = ScreenMode.screen;
		AbstractFrameData[] datas = new FrameDataMultiLine[]
		{
			frameDataMultiLine
		};
		ScreenHandler screenHandler = ScreenHandler.Create(dz, mode, datas);
		this.BindEventWrapper("DUKES", 0, 1, EventIconId.Money, true, new AbstractHandler[]
		{
			screenHandler
		});
	}

	// Token: 0x06007EED RID: 32493 RVA: 0x00338EB7 File Offset: 0x003370B7
	public void Cleanup()
	{
		GameStats.OnChangedDelegates -= this.GameStats_OnChanged;
	}

	// Token: 0x06007EEE RID: 32494 RVA: 0x00338ECA File Offset: 0x003370CA
	[PublicizedFrom(EAccessModifier.Private)]
	public void BindEventWrapper(string _eventName, int _minValue, int _maxValue, EventIconId _icon, bool _optionalValue, params AbstractHandler[] _handlers)
	{
		GSClient.Instance.BindEvent(_eventName, _minValue, _maxValue, _icon, _handlers, _optionalValue);
		this.registeredEvents.Add(_eventName);
	}

	// Token: 0x06007EEF RID: 32495 RVA: 0x00338EEC File Offset: 0x003370EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetEventValues()
	{
		foreach (string eventName in this.registeredEvents)
		{
			GSClient.Instance.SendEvent(eventName, 0, null);
		}
	}

	// Token: 0x06007EF0 RID: 32496 RVA: 0x00338F48 File Offset: 0x00337148
	public void SessionStarted(EntityPlayerLocal _playerEntity)
	{
		this.ResetEventValues();
		this.entityPlayer = new WeakReference<EntityPlayerLocal>(_playerEntity);
		_playerEntity.PlayerStats.AddBuffChangedDelegate(this);
		this.lastTimePercent = -1;
		this.previousAmmoPercentage = -1;
		this.previousSlotIndex = -1;
		this.previousDurability = -1;
		this.previousStealth = -1;
		this.bloodMoonWarning = GameStats.GetInt(EnumGameStats.BloodMoonWarning);
		this.duskDawnTime = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
		this.lastBloodMoonValue = -1;
		this.lastDirection = GameSenseManager.EDirection45.None;
		this.dukesItem = ItemClass.GetItem(TraderInfo.CurrencyItem, false);
	}

	// Token: 0x06007EF1 RID: 32497 RVA: 0x00338FD4 File Offset: 0x003371D4
	public void SessionEnded()
	{
		this.ResetEventValues();
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer.TryGetTarget(out entityPlayerLocal))
		{
			entityPlayerLocal.PlayerStats.RemoveBuffChangedDelegate(this);
		}
		this.entityPlayer = null;
		this.dukesItem = null;
	}

	// Token: 0x06007EF2 RID: 32498 RVA: 0x00339010 File Offset: 0x00337210
	[PublicizedFrom(EAccessModifier.Private)]
	public void GameStats_OnChanged(EnumGameStats _gameStat, object _newValue)
	{
		if (_gameStat == EnumGameStats.BloodMoonDay)
		{
			this.bloodMoonDay = GameStats.GetInt(EnumGameStats.BloodMoonDay);
		}
	}

	// Token: 0x06007EF3 RID: 32499 RVA: 0x00339024 File Offset: 0x00337224
	public void Update()
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		this.UpdateEventAmmo();
		this.UpdateEventDurability();
		this.UpdateEventStealth();
		this.UpdateEventDukes();
	}

	// Token: 0x06007EF4 RID: 32500 RVA: 0x0033904C File Offset: 0x0033724C
	public void UpdateEventTime(ulong _worldTime)
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		this.UpdateEventBloodmoon(_worldTime);
		int num = (int)(_worldTime % 24000UL) / 240;
		if (this.lastTimePercent == num)
		{
			return;
		}
		this.lastTimePercent = num;
		GSClient.Instance.SendEvent("TIME", num, null);
	}

	// Token: 0x06007EF5 RID: 32501 RVA: 0x003390A0 File Offset: 0x003372A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventAmmo()
	{
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int focusedItemIdx = entityPlayerLocal.inventory.GetFocusedItemIdx();
		ItemValue itemValue = entityPlayerLocal.inventory.GetItem(focusedItemIdx).itemValue;
		ItemClass itemClass;
		if (itemValue == null || itemValue.type == 0 || (itemClass = itemValue.ItemClass) == null)
		{
			this.previousAmmoPercentage = 0;
			GSClient.Instance.SendEvent("AMMO", 0, null);
			return;
		}
		ItemActionAttack itemActionAttack = itemClass.Actions[0] as ItemActionAttack;
		int num;
		if (itemActionAttack == null || itemActionAttack is ItemActionMelee || (num = (int)EffectManager.GetValue(PassiveEffects.MagazineSize, itemValue, 0f, entityPlayerLocal, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)) <= 0)
		{
			this.previousAmmoPercentage = 0;
			GSClient.Instance.SendEvent("AMMO", 0, null);
			return;
		}
		int meta = itemValue.Meta;
		int num2 = 100 * meta / num;
		if (num2 == this.previousAmmoPercentage)
		{
			return;
		}
		this.previousAmmoPercentage = num2;
		GSClient.Instance.SendEvent("AMMO", num2, null);
	}

	// Token: 0x06007EF6 RID: 32502 RVA: 0x003391AE File Offset: 0x003373AE
	public void UpdateEventHealth(int _value)
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		GSClient.Instance.SendEvent("HEALTH", _value, null);
	}

	// Token: 0x06007EF7 RID: 32503 RVA: 0x003391D0 File Offset: 0x003373D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventDurability()
	{
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int focusedItemIdx = entityPlayerLocal.inventory.GetFocusedItemIdx();
		ItemValue itemValue = entityPlayerLocal.inventory.GetItem(focusedItemIdx).itemValue;
		ItemClass itemClass;
		if (itemValue == null || itemValue.type == 0 || (itemClass = itemValue.ItemClass) == null || !itemClass.ShowQualityBar)
		{
			this.previousDurability = 0;
			GSClient.Instance.SendEvent("DURABILITY", 0, null);
			return;
		}
		int num = (int)(100f * itemValue.PercentUsesLeft);
		if (num == this.previousDurability)
		{
			return;
		}
		this.previousDurability = num;
		GSClient.Instance.SendEvent("DURABILITY", num, null);
	}

	// Token: 0x06007EF8 RID: 32504 RVA: 0x00339288 File Offset: 0x00337488
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventStealth()
	{
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int num = (int)(100f * entityPlayerLocal.Stealth.ValuePercentUI);
		if (num == this.previousStealth)
		{
			return;
		}
		this.previousStealth = num;
		GSClient.Instance.SendEvent("STEALTH", num, null);
	}

	// Token: 0x06007EF9 RID: 32505 RVA: 0x003392EB File Offset: 0x003374EB
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventBleeding(bool _isBleeding)
	{
		GSClient.Instance.SendEvent("BLEEDING", _isBleeding ? 1 : 0, null);
	}

	// Token: 0x06007EFA RID: 32506 RVA: 0x00339304 File Offset: 0x00337504
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventBloodmoon(ulong _worldTime)
	{
		int num = 0;
		ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(_worldTime);
		int item = valueTuple.Item1;
		int item2 = valueTuple.Item2;
		int item3 = valueTuple.Item3;
		if (GameUtils.IsBloodMoonTime(_worldTime, this.duskDawnTime, this.bloodMoonDay))
		{
			num = 100;
		}
		else if (this.bloodMoonDay == item && this.bloodMoonWarning >= 0 && this.bloodMoonWarning <= item2)
		{
			int num2 = this.duskDawnTime.Item1 - 2;
			if (item2 > num2)
			{
				num = 85;
			}
			else
			{
				float num3 = (float)item2 + (float)item3 / 60f - (float)this.bloodMoonWarning;
				float num4 = (float)(num2 - this.bloodMoonWarning);
				num = (int)(80f * num3 / num4);
			}
		}
		if (num == this.lastBloodMoonValue)
		{
			return;
		}
		this.lastBloodMoonValue = num;
		GSClient.Instance.SendEvent("BLOODMOON", num, null);
	}

	// Token: 0x06007EFB RID: 32507 RVA: 0x003393CC File Offset: 0x003375CC
	public void UpdateEventCompass(float _rotation)
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		GameSenseManager.EDirection45 edirection;
		if ((double)_rotation > 337.5 || (double)_rotation <= 22.5)
		{
			edirection = GameSenseManager.EDirection45.N;
		}
		else if ((double)_rotation <= 67.5)
		{
			edirection = GameSenseManager.EDirection45.NE;
		}
		else if ((double)_rotation <= 112.5)
		{
			edirection = GameSenseManager.EDirection45.E;
		}
		else if ((double)_rotation <= 157.5)
		{
			edirection = GameSenseManager.EDirection45.SE;
		}
		else if ((double)_rotation <= 202.5)
		{
			edirection = GameSenseManager.EDirection45.S;
		}
		else if ((double)_rotation <= 247.5)
		{
			edirection = GameSenseManager.EDirection45.SW;
		}
		else if ((double)_rotation <= 292.5)
		{
			edirection = GameSenseManager.EDirection45.W;
		}
		else
		{
			edirection = GameSenseManager.EDirection45.NW;
		}
		if (this.lastDirection == edirection)
		{
			return;
		}
		this.lastDirection = edirection;
		GSClient.Instance.SendEvent("NORTH", (edirection == GameSenseManager.EDirection45.N) ? 1 : 0, null);
		GSClient.Instance.SendEvent("NORTHEAST", (edirection == GameSenseManager.EDirection45.NE) ? 1 : 0, null);
		GSClient.Instance.SendEvent("EAST", (edirection == GameSenseManager.EDirection45.E) ? 1 : 0, null);
		GSClient.Instance.SendEvent("SOUTHEAST", (edirection == GameSenseManager.EDirection45.SE) ? 1 : 0, null);
		GSClient.Instance.SendEvent("SOUTH", (edirection == GameSenseManager.EDirection45.S) ? 1 : 0, null);
		GSClient.Instance.SendEvent("SOUTHWEST", (edirection == GameSenseManager.EDirection45.SW) ? 1 : 0, null);
		GSClient.Instance.SendEvent("WEST", (edirection == GameSenseManager.EDirection45.W) ? 1 : 0, null);
		GSClient.Instance.SendEvent("NORTHWEST", (edirection == GameSenseManager.EDirection45.NW) ? 1 : 0, null);
	}

	// Token: 0x06007EFC RID: 32508 RVA: 0x0033953C File Offset: 0x0033773C
	public void UpdateEventHit()
	{
		if (!GSClient.Instance.isClientActive())
		{
			return;
		}
		this.stopHitEventTime = Time.unscaledTime + 0.5f;
		if (this.stopHitEventCoroutine == null)
		{
			GSClient.Instance.SendEvent("HIT", 1, null);
			this.stopHitEventCoroutine = ThreadManager.StartCoroutine(this.StopHitCo());
		}
	}

	// Token: 0x06007EFD RID: 32509 RVA: 0x00339591 File Offset: 0x00337791
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator StopHitCo()
	{
		while (Time.unscaledTime < this.stopHitEventTime)
		{
			yield return null;
		}
		GSClient.Instance.SendEvent("HIT", 0, null);
		this.stopHitEventCoroutine = null;
		yield break;
	}

	// Token: 0x06007EFE RID: 32510 RVA: 0x003395A0 File Offset: 0x003377A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEventDukes()
	{
		float unscaledTime = Time.unscaledTime;
		if (unscaledTime < this.dukesNextTime)
		{
			return;
		}
		this.dukesNextTime = unscaledTime + 5f;
		EntityPlayerLocal entityPlayerLocal;
		if (this.entityPlayer == null || !this.entityPlayer.TryGetTarget(out entityPlayerLocal) || entityPlayerLocal == null)
		{
			return;
		}
		int num = entityPlayerLocal.inventory.GetItemCount(this.dukesItem, false, -1, -1, true);
		num += entityPlayerLocal.bag.GetItemCount(this.dukesItem, -1, -1, true);
		ContextFrameObject contextFrameObject = new ContextFrameObject();
		contextFrameObject["dukesstring"] = ((double)num).FormatNumberWithMetricPrefix(true, 2);
		ContextFrameObject frame = contextFrameObject;
		GSClient.Instance.SendEvent("DUKES", num, frame);
	}

	// Token: 0x06007EFF RID: 32511 RVA: 0x00339648 File Offset: 0x00337848
	public void EntityBuffAdded(BuffValue _buff)
	{
		if (_buff.BuffClass.Name.EqualsCaseInsensitive("buffInjuryBleeding"))
		{
			this.UpdateEventBleeding(true);
		}
	}

	// Token: 0x06007F00 RID: 32512 RVA: 0x00339668 File Offset: 0x00337868
	public void EntityBuffRemoved(BuffValue _buff)
	{
		if (_buff.BuffClass.Name.EqualsCaseInsensitive("buffInjuryBleeding"))
		{
			this.UpdateEventBleeding(false);
		}
	}

	// Token: 0x040061D1 RID: 25041
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<string> registeredEvents = new HashSet<string>();

	// Token: 0x040061D2 RID: 25042
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameSenseGameName = "7dtd";

	// Token: 0x040061D3 RID: 25043
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameSenseGameNameFull = "7 Days to Die";

	// Token: 0x040061D4 RID: 25044
	[PublicizedFrom(EAccessModifier.Private)]
	public const string GameSenseGameDeveloper = "The Fun Pimps";

	// Token: 0x040061D5 RID: 25045
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventHealth = "HEALTH";

	// Token: 0x040061D6 RID: 25046
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventAmmo = "AMMO";

	// Token: 0x040061D7 RID: 25047
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventTime = "TIME";

	// Token: 0x040061D8 RID: 25048
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventDurability = "DURABILITY";

	// Token: 0x040061D9 RID: 25049
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventStealth = "STEALTH";

	// Token: 0x040061DA RID: 25050
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventBleeding = "BLEEDING";

	// Token: 0x040061DB RID: 25051
	[PublicizedFrom(EAccessModifier.Private)]
	public const string BleedingBuffName = "buffInjuryBleeding";

	// Token: 0x040061DC RID: 25052
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventBloodmoon = "BLOODMOON";

	// Token: 0x040061DD RID: 25053
	[PublicizedFrom(EAccessModifier.Private)]
	public const int BloodmoonBlinkWarningHours = 2;

	// Token: 0x040061DE RID: 25054
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventHit = "HIT";

	// Token: 0x040061DF RID: 25055
	[PublicizedFrom(EAccessModifier.Private)]
	public const float HitEventDuration = 0.5f;

	// Token: 0x040061E0 RID: 25056
	[PublicizedFrom(EAccessModifier.Private)]
	public const string EventDukes = "DUKES";

	// Token: 0x040061E1 RID: 25057
	[PublicizedFrom(EAccessModifier.Private)]
	public const string DukesStringValueKey = "dukesstring";

	// Token: 0x040061E2 RID: 25058
	[PublicizedFrom(EAccessModifier.Private)]
	public const string DukesLabelLocKey = "gamesenseDukesLabel";

	// Token: 0x040061E3 RID: 25059
	[PublicizedFrom(EAccessModifier.Private)]
	public WeakReference<EntityPlayerLocal> entityPlayer;

	// Token: 0x040061E4 RID: 25060
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastTimePercent = -1;

	// Token: 0x040061E5 RID: 25061
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousSlotIndex = -1;

	// Token: 0x040061E6 RID: 25062
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousAmmoPercentage = -1;

	// Token: 0x040061E7 RID: 25063
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousDurability = -1;

	// Token: 0x040061E8 RID: 25064
	[PublicizedFrom(EAccessModifier.Private)]
	public int previousStealth = -1;

	// Token: 0x040061E9 RID: 25065
	[PublicizedFrom(EAccessModifier.Private)]
	public int bloodMoonDay;

	// Token: 0x040061EA RID: 25066
	[PublicizedFrom(EAccessModifier.Private)]
	public int bloodMoonWarning;

	// Token: 0x040061EB RID: 25067
	[TupleElementNames(new string[]
	{
		"duskHour",
		"dawnHour"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public ValueTuple<int, int> duskDawnTime;

	// Token: 0x040061EC RID: 25068
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastBloodMoonValue = -1;

	// Token: 0x040061ED RID: 25069
	[PublicizedFrom(EAccessModifier.Private)]
	public GameSenseManager.EDirection45 lastDirection = GameSenseManager.EDirection45.None;

	// Token: 0x040061EE RID: 25070
	[PublicizedFrom(EAccessModifier.Private)]
	public float stopHitEventTime = -1f;

	// Token: 0x040061EF RID: 25071
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine stopHitEventCoroutine;

	// Token: 0x040061F0 RID: 25072
	[PublicizedFrom(EAccessModifier.Private)]
	public float dukesNextTime;

	// Token: 0x040061F1 RID: 25073
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemValue dukesItem;

	// Token: 0x02000F8E RID: 3982
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EDirection45
	{
		// Token: 0x040061F3 RID: 25075
		N,
		// Token: 0x040061F4 RID: 25076
		NE,
		// Token: 0x040061F5 RID: 25077
		E,
		// Token: 0x040061F6 RID: 25078
		SE,
		// Token: 0x040061F7 RID: 25079
		S,
		// Token: 0x040061F8 RID: 25080
		SW,
		// Token: 0x040061F9 RID: 25081
		W,
		// Token: 0x040061FA RID: 25082
		NW,
		// Token: 0x040061FB RID: 25083
		None
	}
}

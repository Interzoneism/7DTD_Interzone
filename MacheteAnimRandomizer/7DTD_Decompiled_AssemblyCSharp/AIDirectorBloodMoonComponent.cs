using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003B1 RID: 945
[Preserve]
public class AIDirectorBloodMoonComponent : AIDirectorComponent
{
	// Token: 0x06001CD8 RID: 7384 RVA: 0x000B4864 File Offset: 0x000B2A64
	public override void InitNewGame()
	{
		base.InitNewGame();
		int num = GameUtils.WorldTimeToDays(this.Director.World.worldTime);
		this.bmDayLast = (num - 1) / 7 * 7;
		this.CalcNextDay(false);
		this.ComputeDawnAndDuskTimes();
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x000B48A8 File Offset: 0x000B2AA8
	public override void Tick(double _dt)
	{
		base.Tick(_dt);
		World world = this.Director.World;
		bool flag = this.isBloodMoon;
		this.isBloodMoon = this.IsBloodMoonTime(world.worldTime);
		if (this.isBloodMoon != flag)
		{
			if (this.isBloodMoon)
			{
				this.StartBloodMoon();
			}
			else
			{
				this.EndBloodMoon();
			}
		}
		if (!this.isBloodMoon)
		{
			int @int = GameStats.GetInt(EnumGameStats.BloodMoonDay);
			if (@int != this.bmDay)
			{
				this.bmDay = @int;
				this.bmDayLast = @int - 1;
				Log.Warning("Blood Moon day stat changed {0}", new object[]
				{
					@int
				});
			}
		}
		if (this.isBloodMoon && GameStats.GetBool(EnumGameStats.IsSpawnEnemies))
		{
			this.delay -= (float)_dt;
			for (int i = 0; i < this.players.Count; i++)
			{
				EntityPlayer entityPlayer = this.players[i];
				if (entityPlayer.bloodMoonParty == null && entityPlayer.IsSpawned())
				{
					this.AddPlayerToParty(entityPlayer);
				}
			}
			for (int j = 0; j < this.parties.Count; j++)
			{
				if (this.nextParty >= this.parties.Count)
				{
					this.nextParty = 0;
				}
				AIDirectorBloodMoonParty aidirectorBloodMoonParty = this.parties[j];
				bool flag2 = j == this.nextParty && this.delay <= 0f;
				if (aidirectorBloodMoonParty.IsEmpty)
				{
					aidirectorBloodMoonParty.KillPartyZombies();
					if (flag2)
					{
						this.nextParty++;
					}
				}
				else if (aidirectorBloodMoonParty.Tick(world, _dt, flag2) && flag2)
				{
					this.delay = 1f / (float)this.parties.Count;
					this.nextParty++;
				}
			}
		}
	}

	// Token: 0x17000332 RID: 818
	// (get) Token: 0x06001CDA RID: 7386 RVA: 0x000B4A68 File Offset: 0x000B2C68
	public bool BloodMoonActive
	{
		get
		{
			return this.isBloodMoon;
		}
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x000B4A70 File Offset: 0x000B2C70
	public bool SetForToday(bool _keepNextDay)
	{
		int num = GameUtils.WorldTimeToDays(this.Director.World.worldTime);
		if (num == this.bmDay)
		{
			return false;
		}
		if (_keepNextDay)
		{
			this.bmDayNextOverride = this.bmDay;
		}
		this.SetDay(num);
		return true;
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x000B4AB8 File Offset: 0x000B2CB8
	public override void Read(BinaryReader _stream, int _version)
	{
		base.Read(_stream, _version);
		if (_version >= 8)
		{
			this.bmDayLast = _stream.ReadInt32();
			int day = _stream.ReadInt32();
			int num = (int)_stream.ReadInt16();
			int num2 = (int)_stream.ReadInt16();
			int @int = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
			int int2 = GamePrefs.GetInt(EnumGamePrefs.BloodMoonRange);
			if (@int != num || int2 != num2)
			{
				this.CalcNextDay(false);
			}
			else
			{
				this.SetDay(day);
			}
		}
		this.ComputeDawnAndDuskTimes();
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x000B4B1E File Offset: 0x000B2D1E
	public override void Write(BinaryWriter _stream)
	{
		base.Write(_stream);
		_stream.Write(this.bmDayLast);
		_stream.Write(this.bmDay);
		_stream.Write((short)GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency));
		_stream.Write((short)GamePrefs.GetInt(EnumGamePrefs.BloodMoonRange));
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x000B4B5B File Offset: 0x000B2D5B
	public void AddPlayer(EntityPlayer _player)
	{
		this.players.Add(_player);
	}

	// Token: 0x06001CDF RID: 7391 RVA: 0x000B4B6C File Offset: 0x000B2D6C
	public void RemovePlayer(EntityPlayer _player)
	{
		if (this.players.Remove(_player))
		{
			for (int i = 0; i < this.parties.Count; i++)
			{
				this.parties[i].PlayerLoggedOut(_player);
			}
		}
	}

	// Token: 0x06001CE0 RID: 7392 RVA: 0x000B4BB0 File Offset: 0x000B2DB0
	public void TimeChanged(bool isSeek = false)
	{
		if (this.isBloodMoon && !this.IsBloodMoonTime(this.Director.World.worldTime))
		{
			this.EndBloodMoon();
		}
		if (this.bmDay != GameUtils.WorldTimeToElements(this.Director.World.worldTime).Item1 && !this.isBloodMoon && !this.IsBloodMoonTime(this.Director.World.worldTime))
		{
			this.CalcNextDay(isSeek);
		}
	}

	// Token: 0x06001CE1 RID: 7393 RVA: 0x000B4C2C File Offset: 0x000B2E2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartBloodMoon()
	{
		Log.Out("BloodMoon starting for day " + GameUtils.WorldTimeToDays(this.Director.World.worldTime).ToString());
		this.ClearParties();
		for (int i = 0; i < this.players.Count; i++)
		{
			this.players[i].IsBloodMoonDead = false;
		}
		this.delay = 0f;
		DictionaryList<int, Entity> entities = this.Director.World.Entities;
		for (int j = 0; j < entities.Count; j++)
		{
			EntityEnemy entityEnemy = entities.list[j] as EntityEnemy;
			if (entityEnemy != null)
			{
				entityEnemy.IsBloodMoon = true;
				entityEnemy.timeStayAfterDeath /= 3;
			}
		}
	}

	// Token: 0x06001CE2 RID: 7394 RVA: 0x000B4CF4 File Offset: 0x000B2EF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void EndBloodMoon()
	{
		Log.Out("Blood moon is over!");
		this.isBloodMoon = false;
		if (this.bmDayNextOverride > 0)
		{
			this.bmDay = this.bmDayNextOverride;
			this.bmDayNextOverride = 0;
			this.SetDay(this.bmDay);
		}
		if (GameUtils.WorldTimeToDays(this.Director.World.worldTime) > this.bmDay)
		{
			this.bmDayLast = this.bmDay;
			this.CalcNextDay(false);
		}
		this.ClearParties();
		DictionaryList<int, Entity> entities = this.Director.World.Entities;
		for (int i = 0; i < entities.Count; i++)
		{
			EntityEnemy entityEnemy = entities.list[i] as EntityEnemy;
			if (entityEnemy != null)
			{
				entityEnemy.bIsChunkObserver = false;
				entityEnemy.IsHordeZombie = false;
				entityEnemy.IsBloodMoon = false;
			}
		}
	}

	// Token: 0x06001CE3 RID: 7395 RVA: 0x000B4DC4 File Offset: 0x000B2FC4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearParties()
	{
		this.nextParty = 0;
		this.parties.Clear();
		for (int i = 0; i < this.players.Count; i++)
		{
			this.players[i].bloodMoonParty = null;
		}
	}

	// Token: 0x06001CE4 RID: 7396 RVA: 0x000B4E0C File Offset: 0x000B300C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddPlayerToParty(EntityPlayer _player)
	{
		for (int i = 0; i < this.parties.Count; i++)
		{
			AIDirectorBloodMoonParty aidirectorBloodMoonParty = this.parties[i];
			if (aidirectorBloodMoonParty.IsMemberOfParty(_player.entityId))
			{
				aidirectorBloodMoonParty.AddPlayer(_player);
				break;
			}
		}
		if (_player.bloodMoonParty == null)
		{
			int num = 0;
			while (num < this.parties.Count && !this.parties[num].TryAddPlayer(_player))
			{
				num++;
			}
		}
		if (_player.bloodMoonParty == null)
		{
			this.CreateNewParty(_player);
		}
	}

	// Token: 0x06001CE5 RID: 7397 RVA: 0x000B4E94 File Offset: 0x000B3094
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateNewParty(EntityPlayer _player)
	{
		this.parties.Add(new AIDirectorBloodMoonParty(_player, this, GameStats.GetInt(EnumGameStats.BloodMoonEnemyCount)));
	}

	// Token: 0x06001CE6 RID: 7398 RVA: 0x000B4EB0 File Offset: 0x000B30B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ComputeDawnAndDuskTimes()
	{
		ValueTuple<int, int> valueTuple = GameUtils.CalcDuskDawnHours(GameStats.GetInt(EnumGameStats.DayLightLength));
		this.duskHour = valueTuple.Item1;
		this.dawnHour = valueTuple.Item2;
	}

	// Token: 0x06001CE7 RID: 7399 RVA: 0x000B4EE2 File Offset: 0x000B30E2
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsBloodMoonTime(ulong worldTime)
	{
		return GameUtils.IsBloodMoonTime(worldTime, new ValueTuple<int, int>(this.duskHour, this.dawnHour), this.bmDay);
	}

	// Token: 0x06001CE8 RID: 7400 RVA: 0x000B4F04 File Offset: 0x000B3104
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcNextDay(bool isSeek = false)
	{
		int @int = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
		int num;
		if (@int <= 0)
		{
			num = 0;
		}
		else
		{
			int int2 = GamePrefs.GetInt(EnumGamePrefs.BloodMoonRange);
			int num2 = @int + base.Random.RandomRange(0, int2 + 1);
			int i = GameUtils.WorldTimeToDays(this.Director.World.worldTime);
			while (i <= this.bmDayLast)
			{
				this.bmDayLast -= num2;
			}
			if (this.bmDayLast < 0)
			{
				this.bmDayLast = 0;
			}
			num = this.bmDayLast;
			do
			{
				num += num2;
			}
			while (num < i);
			this.bmDayLast = num - num2;
			if (isSeek && this.bmDay > this.bmDayLast && this.bmDay <= this.bmDayLast + @int + int2)
			{
				num = this.bmDay;
			}
		}
		this.SetDay(num);
	}

	// Token: 0x06001CE9 RID: 7401 RVA: 0x000B4FCC File Offset: 0x000B31CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetDay(int day)
	{
		if (GameManager.Instance != null && GameManager.Instance.gameStateManager != null)
		{
			GameManager.Instance.gameStateManager.SetBloodMoonDay(day);
		}
		if (this.bmDay != day)
		{
			this.bmDay = day;
			Log.Out("BloodMoon SetDay: day {0}, last day {1}, freq {2}, range {3}", new object[]
			{
				this.bmDay,
				this.bmDayLast,
				GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency),
				GamePrefs.GetInt(EnumGamePrefs.BloodMoonRange)
			});
		}
	}

	// Token: 0x06001CEA RID: 7402 RVA: 0x000B505C File Offset: 0x000B325C
	public void LogBM(string format, params object[] args)
	{
		format = string.Format("{0} BM {1}", Time.frameCount, format);
		Log.Warning(format, args);
	}

	// Token: 0x0400138A RID: 5002
	public const int cPartyEnemyMax = 30;

	// Token: 0x0400138B RID: 5003
	public const int cTimeStayAfterDeathScale = 3;

	// Token: 0x0400138C RID: 5004
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSpawnDelay = 1f;

	// Token: 0x0400138D RID: 5005
	[PublicizedFrom(EAccessModifier.Private)]
	public int bmDay;

	// Token: 0x0400138E RID: 5006
	[PublicizedFrom(EAccessModifier.Private)]
	public int bmDayLast;

	// Token: 0x0400138F RID: 5007
	[PublicizedFrom(EAccessModifier.Private)]
	public int bmDayNextOverride;

	// Token: 0x04001390 RID: 5008
	[PublicizedFrom(EAccessModifier.Private)]
	public int dawnHour;

	// Token: 0x04001391 RID: 5009
	[PublicizedFrom(EAccessModifier.Private)]
	public int duskHour;

	// Token: 0x04001392 RID: 5010
	[PublicizedFrom(EAccessModifier.Private)]
	public int nextParty;

	// Token: 0x04001393 RID: 5011
	[PublicizedFrom(EAccessModifier.Private)]
	public float delay;

	// Token: 0x04001394 RID: 5012
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isBloodMoon;

	// Token: 0x04001395 RID: 5013
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AIDirectorBloodMoonParty> parties = new List<AIDirectorBloodMoonParty>();

	// Token: 0x04001396 RID: 5014
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityPlayer> players = new List<EntityPlayer>();
}

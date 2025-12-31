using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000983 RID: 2435
public class EntitySpawnerClass
{
	// Token: 0x06004968 RID: 18792 RVA: 0x001D0614 File Offset: 0x001CE814
	public void Init()
	{
		if (!this.Properties.Values.ContainsKey(EntitySpawnerClass.PropEntityGroupName))
		{
			throw new Exception(string.Concat(new string[]
			{
				"Mandatory property '",
				EntitySpawnerClass.PropEntityGroupName,
				"' missing in entityspawnerclass '",
				this.name,
				"'"
			}));
		}
		this.entityGroupName = this.Properties.Values[EntitySpawnerClass.PropEntityGroupName];
		if (!EntityGroups.list.ContainsKey(this.entityGroupName))
		{
			throw new Exception("Entity spawner '" + this.name + "' contains invalid group " + this.entityGroupName);
		}
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropStartSound))
		{
			this.startSound = this.Properties.Values[EntitySpawnerClass.PropStartSound];
		}
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropStartText))
		{
			this.startText = this.Properties.Values[EntitySpawnerClass.PropStartText];
		}
		this.spawnAtTimeOfDay = EDaytime.Any;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTime))
		{
			this.spawnAtTimeOfDay = EnumUtils.Parse<EDaytime>(this.Properties.Values[EntitySpawnerClass.PropTime], false);
		}
		this.delayBetweenSpawns = 0f;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropDelayBetweenSpawns))
		{
			this.delayBetweenSpawns = StringParsers.ParseFloat(this.Properties.Values[EntitySpawnerClass.PropDelayBetweenSpawns], 0, -1, NumberStyles.Any);
		}
		this.totalAlive = 1;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTotalAlive))
		{
			this.totalAlive = int.Parse(this.Properties.Values[EntitySpawnerClass.PropTotalAlive]);
		}
		this.totalPerWaveMin = 1;
		this.totalPerWaveMax = 1;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTotalPerWave))
		{
			StringParsers.ParseMinMaxCount(this.Properties.Values[EntitySpawnerClass.PropTotalPerWave], out this.totalPerWaveMin, out this.totalPerWaveMax);
		}
		this.delayToNextWave = 1f;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropDelayToNextWave))
		{
			this.delayToNextWave = StringParsers.ParseFloat(this.Properties.Values[EntitySpawnerClass.PropDelayToNextWave], 0, -1, NumberStyles.Any);
		}
		this.bAttackPlayerImmediately = false;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropAttackPlayerAtOnce))
		{
			this.bAttackPlayerImmediately = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropAttackPlayerAtOnce], 0, -1, true);
		}
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropNumberOfWaves))
		{
			this.numberOfWaves = int.Parse(this.Properties.Values[EntitySpawnerClass.PropNumberOfWaves]);
		}
		this.bTerritorial = false;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTerritorial))
		{
			this.bTerritorial = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropTerritorial], 0, -1, true);
		}
		this.territorialRange = 10;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropTerritorialRange))
		{
			this.territorialRange = int.Parse(this.Properties.Values[EntitySpawnerClass.PropTerritorialRange]);
		}
		this.bSpawnOnGround = true;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropSpawnOnGround))
		{
			this.bSpawnOnGround = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropSpawnOnGround], 0, -1, true);
		}
		this.bIgnoreTrigger = false;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropIgnoreTrigger))
		{
			this.bIgnoreTrigger = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropIgnoreTrigger], 0, -1, true);
		}
		this.bPropResetToday = true;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropResetToday))
		{
			this.bPropResetToday = StringParsers.ParseBool(this.Properties.Values[EntitySpawnerClass.PropResetToday], 0, -1, true);
		}
		this.daysToRespawnIfPlayerLeft = 0;
		if (this.Properties.Values.ContainsKey(EntitySpawnerClass.PropDaysToRespawnIfPlayerLeft))
		{
			this.daysToRespawnIfPlayerLeft = Mathf.RoundToInt(StringParsers.ParseFloat(this.Properties.Values[EntitySpawnerClass.PropDaysToRespawnIfPlayerLeft], 0, -1, NumberStyles.Any));
		}
		if (EntitySpawnerClass.DefaultClassName == null)
		{
			EntitySpawnerClass.DefaultClassName = this;
		}
	}

	// Token: 0x06004969 RID: 18793 RVA: 0x001D0A90 File Offset: 0x001CEC90
	public static void Cleanup()
	{
		EntitySpawnerClass.list.Clear();
	}

	// Token: 0x0400388C RID: 14476
	public static string PropStartSound = "StartSound";

	// Token: 0x0400388D RID: 14477
	public static string PropStartText = "StartText";

	// Token: 0x0400388E RID: 14478
	public static string PropEntityGroupName = "EntityGroupName";

	// Token: 0x0400388F RID: 14479
	public static string PropTime = "Time";

	// Token: 0x04003890 RID: 14480
	public static string PropDelayBetweenSpawns = "DelayBetweenSpawns";

	// Token: 0x04003891 RID: 14481
	public static string PropTotalAlive = "TotalAlive";

	// Token: 0x04003892 RID: 14482
	public static string PropTotalPerWave = "TotalPerWave";

	// Token: 0x04003893 RID: 14483
	public static string PropDelayToNextWave = "DelayToNextWave";

	// Token: 0x04003894 RID: 14484
	public static string PropAttackPlayerAtOnce = "AttackPlayerAtOnce";

	// Token: 0x04003895 RID: 14485
	public static string PropNumberOfWaves = "NumberOfWaves";

	// Token: 0x04003896 RID: 14486
	public static string PropTerritorial = "Territorial";

	// Token: 0x04003897 RID: 14487
	public static string PropTerritorialRange = "TerritorialRange";

	// Token: 0x04003898 RID: 14488
	public static string PropSpawnOnGround = "SpawnOnGround";

	// Token: 0x04003899 RID: 14489
	public static string PropIgnoreTrigger = "IgnoreTrigger";

	// Token: 0x0400389A RID: 14490
	public static string PropResetToday = "ResetToday";

	// Token: 0x0400389B RID: 14491
	public static string PropDaysToRespawnIfPlayerLeft = "DaysToRespawnIfPlayerLeft";

	// Token: 0x0400389C RID: 14492
	public static DictionarySave<string, EntitySpawnerClassForDay> list = new DictionarySave<string, EntitySpawnerClassForDay>();

	// Token: 0x0400389D RID: 14493
	public static EntitySpawnerClass DefaultClassName;

	// Token: 0x0400389E RID: 14494
	public DynamicProperties Properties = new DynamicProperties();

	// Token: 0x0400389F RID: 14495
	public string name;

	// Token: 0x040038A0 RID: 14496
	public string entityGroupName;

	// Token: 0x040038A1 RID: 14497
	public EDaytime spawnAtTimeOfDay;

	// Token: 0x040038A2 RID: 14498
	public float delayBetweenSpawns;

	// Token: 0x040038A3 RID: 14499
	public int totalAlive;

	// Token: 0x040038A4 RID: 14500
	public float delayToNextWave;

	// Token: 0x040038A5 RID: 14501
	public int totalPerWaveMin;

	// Token: 0x040038A6 RID: 14502
	public int totalPerWaveMax;

	// Token: 0x040038A7 RID: 14503
	public int numberOfWaves;

	// Token: 0x040038A8 RID: 14504
	public bool bAttackPlayerImmediately;

	// Token: 0x040038A9 RID: 14505
	public bool bSpawnOnGround;

	// Token: 0x040038AA RID: 14506
	public bool bIgnoreTrigger;

	// Token: 0x040038AB RID: 14507
	public bool bTerritorial;

	// Token: 0x040038AC RID: 14508
	public int territorialRange;

	// Token: 0x040038AD RID: 14509
	public bool bPropResetToday;

	// Token: 0x040038AE RID: 14510
	public int daysToRespawnIfPlayerLeft;

	// Token: 0x040038AF RID: 14511
	public string startSound;

	// Token: 0x040038B0 RID: 14512
	public string startText;
}

using System;
using System.Collections.Generic;
using MusicUtils.Enums;
using UnityEngine;

namespace DynamicMusic.Legacy
{
	// Token: 0x02001780 RID: 6016
	public class ThreatLevelTracker
	{
		// Token: 0x1700143D RID: 5181
		// (get) Token: 0x0600B45E RID: 46174 RVA: 0x0045B3D4 File Offset: 0x004595D4
		// (set) Token: 0x0600B45F RID: 46175 RVA: 0x0045B3DC File Offset: 0x004595DC
		public int DeadEnemies { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700143E RID: 5182
		// (get) Token: 0x0600B460 RID: 46176 RVA: 0x0045B3E5 File Offset: 0x004595E5
		// (set) Token: 0x0600B461 RID: 46177 RVA: 0x0045B3ED File Offset: 0x004595ED
		public int InactiveEnemies { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700143F RID: 5183
		// (get) Token: 0x0600B462 RID: 46178 RVA: 0x0045B3F6 File Offset: 0x004595F6
		// (set) Token: 0x0600B463 RID: 46179 RVA: 0x0045B3FE File Offset: 0x004595FE
		public int SleepingEnemies { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001440 RID: 5184
		// (get) Token: 0x0600B464 RID: 46180 RVA: 0x0045B407 File Offset: 0x00459607
		public int ActiveEnemies
		{
			get
			{
				if (this.enemies == null)
				{
					return 0;
				}
				return this.enemies.Count - this.DeadEnemies - this.InactiveEnemies - this.SleepingEnemies;
			}
		}

		// Token: 0x17001441 RID: 5185
		// (get) Token: 0x0600B465 RID: 46181 RVA: 0x0045B433 File Offset: 0x00459633
		// (set) Token: 0x0600B466 RID: 46182 RVA: 0x0045B43B File Offset: 0x0045963B
		public float NumericalThreatLevel { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001442 RID: 5186
		// (get) Token: 0x0600B467 RID: 46183 RVA: 0x0045B444 File Offset: 0x00459644
		public ThreatLevelLegacyType ThreatLevel
		{
			get
			{
				if (this.NumericalThreatLevel < 0.25f)
				{
					return ThreatLevelLegacyType.Exploration;
				}
				return ThreatLevelLegacyType.Suspense;
			}
		}

		// Token: 0x17001443 RID: 5187
		// (get) Token: 0x0600B468 RID: 46184 RVA: 0x0045B456 File Offset: 0x00459656
		public bool IsMusicPlayingThisTick
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.dynamicMusicManager.IsMusicPlayingThisTick;
			}
		}

		// Token: 0x17001444 RID: 5188
		// (get) Token: 0x0600B469 RID: 46185 RVA: 0x0045B463 File Offset: 0x00459663
		public bool IsThreatLevelInExploration
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.NumericalThreatLevel < 0.25f;
			}
		}

		// Token: 0x17001445 RID: 5189
		// (get) Token: 0x0600B46A RID: 46186 RVA: 0x0045B472 File Offset: 0x00459672
		public bool IsTargetInExploration
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.threatLevelTarget <= 0.25f;
			}
		}

		// Token: 0x17001446 RID: 5190
		// (get) Token: 0x0600B46B RID: 46187 RVA: 0x0045B484 File Offset: 0x00459684
		public bool IsTargetAboveThreatLevel
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.threatLevelTarget > this.NumericalThreatLevel;
			}
		}

		// Token: 0x0600B46C RID: 46188 RVA: 0x0045B494 File Offset: 0x00459694
		public static void Init(DynamicMusicManager _dmManager)
		{
			_dmManager.ThreatLevelTracker = new ThreatLevelTracker();
			_dmManager.ThreatLevelTracker.dynamicMusicManager = _dmManager;
			_dmManager.ThreatLevelTracker.somePlayer = GameManager.Instance.World.GetPrimaryPlayer();
			_dmManager.ThreatLevelTracker.epLocal = _dmManager.PrimaryLocalPlayer;
			_dmManager.ThreatLevelTracker.NumericalThreatLevel = 0f;
			_dmManager.ThreatLevelTracker.enemies = new List<Entity>();
		}

		// Token: 0x0600B46D RID: 46189 RVA: 0x0045B503 File Offset: 0x00459703
		public void Tick()
		{
			if (GameTimer.Instance.ticks % 20UL == 0UL)
			{
				this.TickTrackThreatLevel();
			}
			this.TickMoveThreatLevel();
		}

		// Token: 0x0600B46E RID: 46190 RVA: 0x0045B524 File Offset: 0x00459724
		[PublicizedFrom(EAccessModifier.Private)]
		public void TickMoveThreatLevel()
		{
			if (!this.IsMusicPlayingThisTick || (this.IsTargetInExploration && this.IsThreatLevelInExploration))
			{
				this.NumericalThreatLevel = this.threatLevelTarget;
				return;
			}
			if (!this.IsTargetAboveThreatLevel)
			{
				this.NumericalThreatLevel = Utils.FastClamp(this.NumericalThreatLevel - 0.003f, this.threatLevelTarget, this.NumericalThreatLevel);
				return;
			}
			if (this.IsThreatLevelInExploration)
			{
				this.NumericalThreatLevel = 0.25f;
				return;
			}
			this.NumericalThreatLevel = Utils.FastClamp(this.NumericalThreatLevel + 0.0015f, 0f, this.threatLevelTarget);
		}

		// Token: 0x0600B46F RID: 46191 RVA: 0x0045B5B8 File Offset: 0x004597B8
		[PublicizedFrom(EAccessModifier.Private)]
		public void TickTrackThreatLevel()
		{
			GameManager.Instance.World.GetEntitiesInBounds(typeof(EntityEnemy), new Bounds(this.epLocal.position, ThreatLevelTracker.boundingBoxRange), this.enemies);
			this.DeadEnemies = (this.SleepingEnemies = (this.InactiveEnemies = 0));
			this.threatLevelTarget = 0f;
			for (int i = 0; i < this.enemies.Count; i++)
			{
				EntityEnemy entityEnemy = this.enemies[i] as EntityEnemy;
				if (entityEnemy.IsDead())
				{
					int num = this.DeadEnemies;
					this.DeadEnemies = num + 1;
				}
				else if (entityEnemy.IsSleeping)
				{
					this.threatLevelTarget += 0.03125f;
					int num = this.SleepingEnemies;
					this.SleepingEnemies = num + 1;
				}
				else if (this.EnemyIsTargetingPlayer(entityEnemy))
				{
					this.threatLevelTarget += 0.25f;
				}
				else if (entityEnemy.IsAlert)
				{
					this.threatLevelTarget += 0.125f;
				}
				else
				{
					this.threatLevelTarget += 0.0625f;
					int num = this.InactiveEnemies;
					this.InactiveEnemies = num + 1;
				}
			}
			this.threatLevelTarget = Utils.FastClamp(this.threatLevelTarget, 0f, 0.5f);
			this.enemies.Clear();
		}

		// Token: 0x0600B470 RID: 46192 RVA: 0x0045B718 File Offset: 0x00459918
		[PublicizedFrom(EAccessModifier.Private)]
		public bool EnemyIsTargetingPlayer(EntityEnemy _enemy)
		{
			EntityAlive attackTarget = _enemy.GetAttackTarget();
			return attackTarget != null && attackTarget.Equals(this.epLocal);
		}

		// Token: 0x0600B471 RID: 46193 RVA: 0x00002914 File Offset: 0x00000B14
		public void Event(MinEventTypes _eventType, MinEventParams _eventParms)
		{
		}

		// Token: 0x04008CC3 RID: 36035
		[PublicizedFrom(EAccessModifier.Private)]
		public DynamicMusicManager dynamicMusicManager;

		// Token: 0x04008CC4 RID: 36036
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cSleeperIncrement = 0.03125f;

		// Token: 0x04008CC5 RID: 36037
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cTargetIncrement = 0.25f;

		// Token: 0x04008CC6 RID: 36038
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cAlertIncrement = 0.125f;

		// Token: 0x04008CC7 RID: 36039
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cInactiveIncrement = 0.0625f;

		// Token: 0x04008CC8 RID: 36040
		[PublicizedFrom(EAccessModifier.Private)]
		public const float cBaseIncrement = 0.0015f;

		// Token: 0x04008CCD RID: 36045
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal epLocal;

		// Token: 0x04008CCE RID: 36046
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal somePlayer;

		// Token: 0x04008CCF RID: 36047
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Entity> enemies;

		// Token: 0x04008CD0 RID: 36048
		[PublicizedFrom(EAccessModifier.Private)]
		public float threatLevelTarget;

		// Token: 0x04008CD1 RID: 36049
		[PublicizedFrom(EAccessModifier.Private)]
		public static Vector3 boundingBoxRange = new Vector3(50f, 50f, 50f);
	}
}

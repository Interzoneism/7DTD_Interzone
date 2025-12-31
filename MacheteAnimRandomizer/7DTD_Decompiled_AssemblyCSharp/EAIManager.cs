using System;
using System.Collections.Generic;
using GamePath;
using UnityEngine;

// Token: 0x020003E7 RID: 999
public class EAIManager
{
	// Token: 0x06001E35 RID: 7733 RVA: 0x000BBE04 File Offset: 0x000BA004
	public EAIManager(EntityAlive _entity)
	{
		this.entity = _entity;
		this.random = _entity.world.aiDirector.random;
		this.entity.rand = this.random;
		this.tasks = new EAITaskList(this);
		this.targetTasks = new EAITaskList(this);
		this.interestDistance = 10f;
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x000BBE74 File Offset: 0x000BA074
	public void CopyPropertiesFromEntityClass(EntityClass ec)
	{
		ec.Properties.ParseFloat(EntityClass.PropAIFeralSense, ref this.feralSense);
		ec.Properties.ParseFloat(EntityClass.PropAIGroupCircle, ref this.groupCircle);
		ec.Properties.ParseFloat(EntityClass.PropAINoiseSeekDist, ref this.noiseSeekDist);
		ec.Properties.ParseFloat(EntityClass.PropAISeeOffset, ref this.seeOffset);
		Vector2 vector = new Vector2(1f, 1f);
		ec.Properties.ParseVec(EntityClass.PropAIPathCostScale, ref vector);
		this.pathCostScale = this.random.RandomRange(vector.x, vector.y);
		this.partialPathHeightScale = 1f - this.pathCostScale;
		string @string = ec.Properties.GetString("AITask");
		if (@string.Length <= 0)
		{
			int num = 1;
			string text;
			for (;;)
			{
				string key = EntityClass.PropAITask + num.ToString();
				if (!ec.Properties.Values.TryGetValue(key, out text) || text.Length == 0)
				{
					goto IL_194;
				}
				EAIBase eaibase = EAIManager.CreateInstance(text);
				if (eaibase == null)
				{
					break;
				}
				eaibase.Init(this.entity);
				DictionarySave<string, string> dictionarySave = ec.Properties.ParseKeyData(key);
				if (dictionarySave != null)
				{
					try
					{
						eaibase.SetData(dictionarySave);
					}
					catch (Exception ex)
					{
						Log.Error("EAIManager {0} SetData error {1}", new object[]
						{
							text,
							ex
						});
					}
				}
				this.tasks.AddTask(num, eaibase);
				num++;
			}
			throw new Exception("Class '" + text + "' not found!");
		}
		this.ParseTasks(@string, this.tasks);
		IL_194:
		string string2 = ec.Properties.GetString("AITarget");
		if (string2.Length > 0)
		{
			this.ParseTasks(string2, this.targetTasks);
			return;
		}
		int num2 = 1;
		string text2;
		for (;;)
		{
			string key2 = EntityClass.PropAITargetTask + num2.ToString();
			if (!ec.Properties.Values.TryGetValue(key2, out text2) || text2.Length == 0)
			{
				return;
			}
			EAIBase eaibase2 = EAIManager.CreateInstance(text2);
			if (eaibase2 == null)
			{
				break;
			}
			eaibase2.Init(this.entity);
			DictionarySave<string, string> dictionarySave2 = ec.Properties.ParseKeyData(key2);
			if (dictionarySave2 != null)
			{
				try
				{
					eaibase2.SetData(dictionarySave2);
				}
				catch (Exception ex2)
				{
					Log.Error("EAIManager {0} SetData error {1}", new object[]
					{
						text2,
						ex2
					});
				}
			}
			this.targetTasks.AddTask(num2, eaibase2);
			num2++;
		}
		throw new Exception("Class '" + text2 + "' not found!");
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x000BC11C File Offset: 0x000BA31C
	[PublicizedFrom(EAccessModifier.Private)]
	public void ParseTasks(string _str, EAITaskList _list)
	{
		int num = 1;
		for (int i = 0; i < _str.Length; i++)
		{
			if (char.IsLetter(_str[i]))
			{
				int num2 = _str.IndexOf('|', i + 1);
				if (num2 < 0)
				{
					num2 = _str.Length;
				}
				string text = _str.Substring(i, num2 - i);
				string text2 = text;
				string text3 = null;
				int num3 = text.IndexOf(' ');
				if (num3 >= 0)
				{
					text2 = text.Substring(0, num3);
					text3 = text.Substring(num3 + 1);
				}
				EAIBase eaibase = EAIManager.CreateInstance(text2);
				if (eaibase == null)
				{
					throw new Exception("Class '" + text2 + "' not found!");
				}
				eaibase.Init(this.entity);
				if (text3 != null)
				{
					DictionarySave<string, string> dictionarySave = DynamicProperties.ParseData(text3);
					if (dictionarySave != null)
					{
						try
						{
							eaibase.SetData(dictionarySave);
						}
						catch (Exception ex)
						{
							Log.Error("EAIManager {0} SetData error {1}", new object[]
							{
								text2,
								ex
							});
						}
					}
				}
				_list.AddTask(num, eaibase);
				num++;
				i = num2;
			}
		}
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x000BC22C File Offset: 0x000BA42C
	[PublicizedFrom(EAccessModifier.Private)]
	public static EAIBase CreateInstance(string _className)
	{
		return (EAIBase)Activator.CreateInstance(EAIManager.GetType(_className));
	}

	// Token: 0x06001E39 RID: 7737 RVA: 0x000BC240 File Offset: 0x000BA440
	[PublicizedFrom(EAccessModifier.Private)]
	public static Type GetType(string _className)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_className);
		if (num <= 2294454340U)
		{
			if (num <= 1005439377U)
			{
				if (num <= 220555081U)
				{
					if (num != 87276885U)
					{
						if (num == 220555081U)
						{
							if (_className == "SetNearestEntityAsTarget")
							{
								return typeof(EAISetNearestEntityAsTarget);
							}
						}
					}
					else if (_className == "BlockIf")
					{
						return typeof(EAIBlockIf);
					}
				}
				else if (num != 244691017U)
				{
					if (num == 1005439377U)
					{
						if (_className == "ApproachSpot")
						{
							return typeof(EAIApproachSpot);
						}
					}
				}
				else if (_className == "BreakBlock")
				{
					return typeof(EAIBreakBlock);
				}
			}
			else if (num <= 1728706612U)
			{
				if (num != 1340592684U)
				{
					if (num == 1728706612U)
					{
						if (_className == "Wander")
						{
							return typeof(EAIWander);
						}
					}
				}
				else if (_className == "Territorial")
				{
					return typeof(EAITerritorial);
				}
			}
			else if (num != 1771441078U)
			{
				if (num != 1994098438U)
				{
					if (num == 2294454340U)
					{
						if (_className == "DestroyArea")
						{
							return typeof(EAIDestroyArea);
						}
					}
				}
				else if (_className == "BlockingTargetTask")
				{
					return typeof(EAIBlockingTargetTask);
				}
			}
			else if (_className == "Look")
			{
				return typeof(EAILook);
			}
		}
		else if (num <= 3546899167U)
		{
			if (num <= 2423584467U)
			{
				if (num != 2414274217U)
				{
					if (num == 2423584467U)
					{
						if (_className == "Leap")
						{
							return typeof(EAILeap);
						}
					}
				}
				else if (_className == "RunawayWhenHurt")
				{
					return typeof(EAIRunawayWhenHurt);
				}
			}
			else if (num != 2454737095U)
			{
				if (num != 3085126581U)
				{
					if (num == 3546899167U)
					{
						if (_className == "ApproachAndAttackTarget")
						{
							return typeof(EAIApproachAndAttackTarget);
						}
					}
				}
				else if (_className == "TakeCover")
				{
					return typeof(EAITakeCover);
				}
			}
			else if (_className == "ApproachDistraction")
			{
				return typeof(EAIApproachDistraction);
			}
		}
		else if (num <= 3618649518U)
		{
			if (num != 3549489919U)
			{
				if (num == 3618649518U)
				{
					if (_className == "SetNearestCorpseAsTarget")
					{
						return typeof(EAISetNearestCorpseAsTarget);
					}
				}
			}
			else if (_className == "RangedAttackTarget")
			{
				return typeof(EAIRangedAttackTarget);
			}
		}
		else if (num != 3938759995U)
		{
			if (num != 4112963184U)
			{
				if (num == 4183380984U)
				{
					if (_className == "SetAsTargetIfHurt")
					{
						return typeof(EAISetAsTargetIfHurt);
					}
				}
			}
			else if (_className == "Dodge")
			{
				return typeof(EAIDodge);
			}
		}
		else if (_className == "RunawayFromEntity")
		{
			return typeof(EAIRunawayFromEntity);
		}
		Log.Warning("EAIManager GetType slow lookup for {0}", new object[]
		{
			_className
		});
		return Type.GetType("EAI" + _className);
	}

	// Token: 0x06001E3A RID: 7738 RVA: 0x000BC606 File Offset: 0x000BA806
	public void Update()
	{
		this.interestDistance = Utils.FastMoveTowards(this.interestDistance, 10f, 0.008333334f);
		this.targetTasks.OnUpdateTasks();
		this.tasks.OnUpdateTasks();
		this.UpdateDebugName();
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x000BC640 File Offset: 0x000BA840
	public void UpdateDebugName()
	{
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuShowTasks))
		{
			EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			this.entity.DebugNameInfo = this.MakeDebugName(primaryPlayer);
		}
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x000BC678 File Offset: 0x000BA878
	public string MakeDebugName(EntityPlayer player)
	{
		EntityMoveHelper moveHelper = this.entity.moveHelper;
		string str = string.Empty;
		if (this.entity.IsSleeper)
		{
			str += string.Format("\nSleeper {0}{1}", this.entity.IsSleeping ? "Sleep " : "", this.entity.IsSleeperPassive ? "Passive" : "");
		}
		str += string.Format("\nHealth {0} / {1}, PCost {2}, InterestD {3}", new object[]
		{
			this.entity.Health,
			this.entity.GetMaxHealth(),
			this.pathCostScale.ToCultureInvariantString(".00"),
			this.interestDistance.ToCultureInvariantString("0.000")
		});
		string text = string.Format("\n{0}{1}", this.entity.IsAlert ? string.Format("Alert {0}, ", ((float)this.entity.GetAlertTicks() / 20f).ToCultureInvariantString("0.00")) : "", this.entity.HasInvestigatePosition ? string.Format("Investigate {0}, ", ((float)this.entity.GetInvestigatePositionTicks() / 20f).ToCultureInvariantString("0.00")) : "");
		if (text.Length > 1)
		{
			str += text;
		}
		string text2 = string.Format("\n{0}{1}{2}{3}{4}{5}", new object[]
		{
			moveHelper.IsActive ? string.Format("Move {0} {1},", this.entity.GetMoveSpeedAggro().ToCultureInvariantString(".00"), this.entity.GetSpeedModifier().ToCultureInvariantString(".00")) : "",
			(moveHelper.BlockedFlags > 0) ? string.Format("Blocked {0}, {1}", moveHelper.BlockedFlags, moveHelper.BlockedTime.ToCultureInvariantString("0.00")) : "",
			moveHelper.CanBreakBlocks ? "CanBrk, " : "",
			moveHelper.IsUnreachableAbove ? "UnreachAbove, " : "",
			moveHelper.IsUnreachableSide ? "UnreachSide, " : "",
			moveHelper.IsUnreachableSideJump ? "UnreachSideJump" : ""
		});
		if (text2.Length > 1)
		{
			str += text2;
		}
		if (this.entity.bodyDamage.CurrentStun != EnumEntityStunType.None)
		{
			str += string.Format("\nStun {0}, {1}", this.entity.bodyDamage.CurrentStun.ToStringCached<EnumEntityStunType>(), this.entity.bodyDamage.StunDuration.ToCultureInvariantString("0.00"));
		}
		if (this.entity.emodel && this.entity.emodel.IsRagdollActive)
		{
			str = str + "\nRagdoll " + this.entity.emodel.GetRagdollDebugInfo();
		}
		for (int i = 0; i < this.tasks.GetExecutingTasks().Count; i++)
		{
			EAITaskEntry eaitaskEntry = this.tasks.GetExecutingTasks()[i];
			str = str + "\n1 " + eaitaskEntry.action.ToString();
		}
		for (int j = 0; j < this.targetTasks.GetExecutingTasks().Count; j++)
		{
			EAITaskEntry eaitaskEntry2 = this.targetTasks.GetExecutingTasks()[j];
			str = str + "\n2 " + eaitaskEntry2.action.ToString();
		}
		if (this.entity.IsSleeping)
		{
			float distance = this.entity.GetDistance(player);
			float value;
			float value2;
			this.entity.GetSleeperDebugScale(distance, out value, out value2);
			string str2 = string.Format("\nLight {0:0} groan{1:0} wake{2:0}, Noise {3:0} groan{4:0} wake{5:0}", new object[]
			{
				player.Stealth.lightLevel.ToCultureInvariantString(),
				value2.ToCultureInvariantString(),
				value.ToCultureInvariantString(),
				this.entity.noisePlayerVolume.ToCultureInvariantString(),
				this.entity.sleeperNoiseToSense.ToCultureInvariantString(),
				this.entity.sleeperNoiseToWake.ToCultureInvariantString()
			});
			str += str2;
		}
		else
		{
			float seeDistance = this.GetSeeDistance(player);
			float seeStealthDebugScale = this.entity.GetSeeStealthDebugScale(seeDistance);
			string str3 = string.Format("\nLight {0:0} sight {1:0}, noise {2:0} dist {3:0}", new object[]
			{
				player.Stealth.lightLevel.ToCultureInvariantString(),
				seeStealthDebugScale.ToCultureInvariantString(),
				this.entity.noisePlayerVolume.ToCultureInvariantString(),
				this.entity.noisePlayerDistance.ToCultureInvariantString()
			});
			str += str3;
		}
		return str + this.entity.MakeDebugNameInfo();
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x000BCB34 File Offset: 0x000BAD34
	public bool CheckPath(PathInfo pathInfo)
	{
		List<EAITaskEntry> executingTasks = this.tasks.GetExecutingTasks();
		for (int i = 0; i < executingTasks.Count; i++)
		{
			if (executingTasks[i].action.IsPathUsageBlocked(pathInfo.path))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x000BCB7C File Offset: 0x000BAD7C
	public void DamagedByEntity()
	{
		EntityMoveHelper moveHelper = this.entity.moveHelper;
		if (moveHelper != null)
		{
			moveHelper.IsDestroyAreaTryUnreachable = false;
		}
		EAIDestroyArea task = this.tasks.GetTask<EAIDestroyArea>();
		if (task == null)
		{
			return;
		}
		task.Stop();
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x000BCBB4 File Offset: 0x000BADB4
	public void SleeperWokeUp()
	{
		for (int i = 0; i < this.targetTasks.Tasks.Count; i++)
		{
			this.targetTasks.Tasks[i].executeTime = 0f;
		}
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x000BCBF8 File Offset: 0x000BADF8
	public void FallHitGround(float distance)
	{
		if (distance >= 0.8f)
		{
			this.entity.ConditionalTriggerSleeperWakeUp();
		}
		if (distance >= 2.5f)
		{
			EntityMoveHelper moveHelper = this.entity.moveHelper;
			if (moveHelper.IsActive && (moveHelper.IsUnreachableSide || moveHelper.IsMoveToAbove()))
			{
				this.ClearTaskDelay<EAIDestroyArea>(this.tasks);
				moveHelper.UnreachablePercent += 0.3f;
				moveHelper.IsDestroyAreaTryUnreachable = true;
				Bounds bb = new Bounds(this.entity.position, new Vector3(20f, 10f, 20f));
				this.entity.world.GetEntitiesInBounds(typeof(EntityHuman), bb, this.allies);
				if (this.allies.Count >= 3)
				{
					for (int i = 0; i < 2; i++)
					{
						int index = this.entity.rand.RandomRange(this.allies.Count);
						EntityHuman entityHuman = (EntityHuman)this.allies[index];
						entityHuman.moveHelper.UnreachablePercent += 0.12f;
						entityHuman.moveHelper.IsDestroyAreaTryUnreachable = true;
					}
				}
				this.allies.Clear();
			}
		}
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x000BCD2E File Offset: 0x000BAF2E
	public float GetSeeDistance(Entity _seeEntity)
	{
		return this.entity.GetDistance(_seeEntity) - this.seeOffset;
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000BCD44 File Offset: 0x000BAF44
	public static float CalcSenseScale()
	{
		switch (GamePrefs.GetInt(EnumGamePrefs.ZombieFeralSense))
		{
		case 1:
			if (GameManager.Instance.World.IsDaytime())
			{
				return 1f;
			}
			break;
		case 2:
			if (GameManager.Instance.World.IsDark())
			{
				return 1f;
			}
			break;
		case 3:
			return 1f;
		}
		return 0f;
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000BCDA8 File Offset: 0x000BAFA8
	public void SetTargetOnlyPlayers(float _distance)
	{
		List<EAITaskEntry> list = this.tasks.Tasks;
		for (int i = 0; i < list.Count; i++)
		{
			EAIApproachAndAttackTarget eaiapproachAndAttackTarget = list[i].action as EAIApproachAndAttackTarget;
			if (eaiapproachAndAttackTarget != null)
			{
				eaiapproachAndAttackTarget.SetTargetOnlyPlayers();
			}
		}
		List<EAITaskEntry> list2 = this.targetTasks.Tasks;
		for (int j = 0; j < list2.Count; j++)
		{
			EAISetNearestEntityAsTarget eaisetNearestEntityAsTarget = list2[j].action as EAISetNearestEntityAsTarget;
			if (eaisetNearestEntityAsTarget != null)
			{
				eaisetNearestEntityAsTarget.SetTargetOnlyPlayers(_distance);
			}
		}
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x000BCE2E File Offset: 0x000BB02E
	public List<T> GetTasks<T>() where T : class
	{
		return this.getTaskTypes<T>(this.tasks);
	}

	// Token: 0x06001E45 RID: 7749 RVA: 0x000BCE3C File Offset: 0x000BB03C
	public List<T> GetTargetTasks<T>() where T : class
	{
		return this.getTaskTypes<T>(this.targetTasks);
	}

	// Token: 0x06001E46 RID: 7750 RVA: 0x000BCE4C File Offset: 0x000BB04C
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T> getTaskTypes<T>(EAITaskList taskList) where T : class
	{
		List<T> list = new List<T>();
		for (int i = 0; i < taskList.Tasks.Count; i++)
		{
			EAITaskEntry eaitaskEntry = taskList.Tasks[i];
			if (eaitaskEntry.action is T)
			{
				list.Add(eaitaskEntry.action as T);
			}
		}
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}

	// Token: 0x06001E47 RID: 7751 RVA: 0x000BCEB4 File Offset: 0x000BB0B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearTaskDelay<T>(EAITaskList taskList) where T : class
	{
		for (int i = 0; i < taskList.Tasks.Count; i++)
		{
			EAITaskEntry eaitaskEntry = taskList.Tasks[i];
			if (eaitaskEntry.action is T)
			{
				eaitaskEntry.executeTime = 0f;
			}
		}
	}

	// Token: 0x06001E48 RID: 7752 RVA: 0x000BCEFC File Offset: 0x000BB0FC
	public static void ToggleAnimFreeze()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		EAIManager.isAnimFreeze = !EAIManager.isAnimFreeze;
		List<Entity> list = world.Entities.list;
		for (int i = 0; i < list.Count; i++)
		{
			EntityAlive entityAlive = list[i] as EntityAlive;
			if (entityAlive && entityAlive.aiManager != null && !entityAlive.emodel.IsRagdollActive && entityAlive.emodel.avatarController)
			{
				Animator animator = entityAlive.emodel.avatarController.GetAnimator();
				if (animator)
				{
					animator.enabled = !EAIManager.isAnimFreeze;
				}
			}
		}
	}

	// Token: 0x040014CB RID: 5323
	public const float cInterestDistanceMax = 10f;

	// Token: 0x040014CC RID: 5324
	public float interestDistance;

	// Token: 0x040014CD RID: 5325
	public float lookTime;

	// Token: 0x040014CE RID: 5326
	public const float cSenseScaleMax = 1.6f;

	// Token: 0x040014CF RID: 5327
	public float feralSense;

	// Token: 0x040014D0 RID: 5328
	public float groupCircle;

	// Token: 0x040014D1 RID: 5329
	public float noiseSeekDist;

	// Token: 0x040014D2 RID: 5330
	public float pathCostScale;

	// Token: 0x040014D3 RID: 5331
	public float partialPathHeightScale;

	// Token: 0x040014D4 RID: 5332
	public float seeOffset;

	// Token: 0x040014D5 RID: 5333
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive entity;

	// Token: 0x040014D6 RID: 5334
	public GameRandom random;

	// Token: 0x040014D7 RID: 5335
	[PublicizedFrom(EAccessModifier.Private)]
	public EAITaskList tasks;

	// Token: 0x040014D8 RID: 5336
	[PublicizedFrom(EAccessModifier.Private)]
	public EAITaskList targetTasks;

	// Token: 0x040014D9 RID: 5337
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Entity> allies = new List<Entity>();

	// Token: 0x040014DA RID: 5338
	public static bool isAnimFreeze;
}

using System;
using System.Collections.Generic;
using ExtUtilsForEnt;
using GamePath;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003F3 RID: 1011
[Preserve]
public class EAITakeCover : EAIBase
{
	// Token: 0x06001E90 RID: 7824 RVA: 0x000BED14 File Offset: 0x000BCF14
	public EAITakeCover()
	{
		this.MutexBits = 1;
		World world = GameManager.Instance.World;
		if (world != null)
		{
			this.world = world;
		}
		PathFinderThread instance = PathFinderThread.Instance;
		if (instance != null)
		{
			this.pathFinder = instance;
		}
		this.ecm = EntityCoverManager.Instance;
	}

	// Token: 0x06001E91 RID: 7825 RVA: 0x000BED71 File Offset: 0x000BCF71
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
	}

	// Token: 0x06001E92 RID: 7826 RVA: 0x000BED7C File Offset: 0x000BCF7C
	public override bool CanExecute()
	{
		if (!EntityCoverManager.DebugModeEnabled)
		{
			return false;
		}
		if (this.theEntity.sleepingOrWakingUp || this.theEntity.bodyDamage.CurrentStun != EnumEntityStunType.None || (this.theEntity.Jumping && !this.theEntity.isSwimming))
		{
			return false;
		}
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (attackTarget)
		{
			this.threatTarget = attackTarget;
		}
		return !(this.threatTarget == null) && !this.stopSeekingCover && (this.theEntity.Health < this.theEntity.GetMaxHealth() && Vector3.Distance(this.theEntity.position, this.threatTarget.position) > 5f);
	}

	// Token: 0x06001E93 RID: 7827 RVA: 0x000BEE3F File Offset: 0x000BD03F
	public override void Start()
	{
		this.timeoutTicks = 800;
		this.retryPathTicks = 60f;
		this.fleeTicks = 0;
		PathFinderThread.Instance.RemovePathsFor(this.theEntity.entityId);
		this.stopSeekingCover = false;
	}

	// Token: 0x06001E94 RID: 7828 RVA: 0x000BEE7C File Offset: 0x000BD07C
	public override bool Continue()
	{
		return (this.theEntity.Health >= this.theEntity.GetMaxHealth() || Vector3.Distance(this.theEntity.position, this.threatTarget.position) >= 3f) && !this.stopSeekingCover;
	}

	// Token: 0x06001E95 RID: 7829 RVA: 0x000BEED0 File Offset: 0x000BD0D0
	[PublicizedFrom(EAccessModifier.Private)]
	public void setState(EAITakeCover.State _state)
	{
		this.state = _state;
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000BEEDC File Offset: 0x000BD0DC
	public override void Update()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (this.world == null)
		{
			return;
		}
		EntityAlive attackTarget = this.theEntity.GetAttackTarget();
		if (attackTarget)
		{
			this.threatTarget = attackTarget;
		}
		if (this.threatTarget == null)
		{
			return;
		}
		if (this.updateCover())
		{
			return;
		}
		switch (this.state)
		{
		case EAITakeCover.State.Idle:
			if (!this.findingPath)
			{
				this.findingPath = true;
				this.setState(EAITakeCover.State.FindPath);
				return;
			}
			break;
		case EAITakeCover.State.FindPath:
			if (!this.pathFinder.IsCalculatingPath(this.theEntity.entityId))
			{
				Vector3 a = this.findCoverDir(false);
				this.pathFinder.RemovePathsFor(this.theEntity.entityId);
				this.pathFinder.FindPath(this.theEntity, this.threatTarget.getHipPosition() + a * 10f * 2f, this.theEntity.moveSpeedAggro, false, this);
				this.setState(EAITakeCover.State.PreProcessPath);
				return;
			}
			break;
		case EAITakeCover.State.PreProcessPath:
			this.pathInfo = this.pathFinder.GetPath(this.theEntity.entityId);
			if (!this.pathFinder.IsCalculatingPath(this.theEntity.entityId))
			{
				PathInfo pathInfo = this.pathInfo;
				if (((pathInfo != null) ? pathInfo.path : null) != null)
				{
					this.currentPath.Clear();
					bool flag = false;
					int num = 0;
					List<Vector3> list = new List<Vector3>();
					for (int i = 0; i < this.pathInfo.path.points.Length; i++)
					{
						Vector3 projectedLocation = this.pathInfo.path.points[i].projectedLocation;
						this.currentPath.Add(projectedLocation);
						Vector3 vector = this.matchHipHeight(projectedLocation);
						List<EAITakeCover.CoverCastInfo> bestCoverDirection = this.getBestCoverDirection(vector, this.threatTarget.getHipPosition(), 10f, false);
						Vector3 dir = Vector3.zero;
						Vector3 v = Vector3.zero;
						if (bestCoverDirection.Count > 0)
						{
							dir = bestCoverDirection[0].Dir;
							v = bestCoverDirection[0].HitPoint;
						}
						Vector3 vector2 = new Vector3i(v).ToVector3CenterXZ();
						if (EUtils.isPositionBlocked(vector, this.threatTarget.getChestPosition(), 65536, false) && vector2 != this.pathEnd)
						{
							list.Add(vector2);
							if (num > 3 || i >= this.pathInfo.path.points.Length - 1)
							{
								int index = 0;
								float num2 = float.MaxValue;
								for (int j = 0; j < list.Count; j++)
								{
									EUtils.DrawBounds(new Vector3i(list[j]), Color.red * Color.yellow * 0.5f, 10f, 1f);
									float num3 = Vector3.Distance(list[j], this.theEntity.position);
									if (num3 < num2 && EUtils.isPositionBlocked(list[j], this.threatTarget.getChestPosition(), 65536, false) && this.ecm.IsFree(list[j]))
									{
										index = j;
										num2 = num3;
									}
								}
								Vector3 vector3 = list[index];
								this.pathEnd = new Vector3i(vector3).ToVector3CenterXZ();
								this.ecm.AddCover(this.pathEnd, dir);
								this.ecm.MarkReserved(this.theEntity.entityId, this.pathEnd);
								EUtils.DrawLine(vector, vector3, Color.red, 10f);
								EUtils.DrawBounds(new Vector3i(vector3), Color.green, 10f, 1f);
								this.pathFinder.FindPath(this.theEntity, this.theEntity.position, this.pathEnd, this.theEntity.moveSpeedAggro, false, this);
								flag = true;
								break;
							}
							num++;
						}
					}
					if (flag && this.currentPath.Count > 0)
					{
						EUtils.DrawPath(this.currentPath, Color.white, Color.yellow);
						this.setState(EAITakeCover.State.ProcessPath);
						return;
					}
					this.freeCover();
					this.retryPathTicks = 60f;
					this.setState(EAITakeCover.State.FindPath);
					return;
				}
			}
			break;
		case EAITakeCover.State.ProcessPath:
			if (this.retryPathTicks > 0f)
			{
				this.retryPathTicks -= 1f;
				if (this.retryPathTicks <= 0f)
				{
					this.freeCover();
					this.retryPathTicks = 60f;
					this.setState(EAITakeCover.State.FindPath);
					return;
				}
			}
			if (this.currentPath.Count > 0)
			{
				if (Vector3.Distance(this.theEntity.position, this.pathEnd) < 0.5f)
				{
					this.pathFinder.RemovePathsFor(this.theEntity.entityId);
					this.theEntity.SetLookPosition(this.threatTarget.getHeadPosition());
					this.ecm.UseCover(this.theEntity.entityId, this.pathEnd);
					this.theEntity.navigator.clearPath();
					this.theEntity.moveHelper.Stop();
					this.coverTicks = 20 * base.Random.RandomRange(4);
					this.findingPath = false;
					this.setState(EAITakeCover.State.Idle);
					return;
				}
			}
			else
			{
				this.freeCover();
				this.retryPathTicks = 60f;
				this.setState(EAITakeCover.State.FindPath);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000BF440 File Offset: 0x000BD640
	[PublicizedFrom(EAccessModifier.Private)]
	public bool updateCover()
	{
		if (!this.ecm.HasCover(this.theEntity.entityId))
		{
			return false;
		}
		if (this.ecm.GetCoverPos(this.theEntity.entityId) == null)
		{
			return false;
		}
		if (this.coverTicks > 0)
		{
			this.coverTicks--;
			if (this.coverTicks <= 0)
			{
				if (base.Random.RandomRange(2) < 1)
				{
					this.freeCover();
					if (base.Random.RandomRange(2) < 1)
					{
						this.stopSeekingCover = true;
					}
				}
				else
				{
					this.coverTicks = 60;
				}
			}
		}
		return true;
	}

	// Token: 0x06001E98 RID: 7832 RVA: 0x000BF4D7 File Offset: 0x000BD6D7
	[PublicizedFrom(EAccessModifier.Private)]
	public void freeCover()
	{
		this.ecm.FreeCover(this.theEntity.entityId);
		this.coverTicks = 60;
	}

	// Token: 0x06001E99 RID: 7833 RVA: 0x000BF4F7 File Offset: 0x000BD6F7
	[PublicizedFrom(EAccessModifier.Private)]
	public void addCover(Vector3 pos, Vector3 dir, bool debugDraw = false)
	{
		if (debugDraw)
		{
			EUtils.DrawBounds(new Vector3i(pos), Color.cyan, 10f, 1f);
		}
		this.ecm.AddCover(pos, dir);
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x000BF524 File Offset: 0x000BD724
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 matchHipHeight(Vector3 point)
	{
		Vector3 vector = this.theEntity.getHipPosition();
		float y = vector.y;
		vector = point;
		vector.y = y;
		return vector;
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x000BF54F File Offset: 0x000BD74F
	[PublicizedFrom(EAccessModifier.Private)]
	public void setCrouching(bool value)
	{
		this.theEntity.Crouching = value;
		this.theEntity.GetComponentInChildren<Animator>().SetBool("IsCrouching", value);
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x000BF574 File Offset: 0x000BD774
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 rotateToDir(Vector3 dir)
	{
		return Quaternion.Lerp(this.theEntity.transform.rotation, Quaternion.LookRotation(dir), (1f - Vector3.Angle(this.theEntity.transform.forward, dir) / 180f) * 7f * 0.05f).eulerAngles;
	}

	// Token: 0x06001E9D RID: 7837 RVA: 0x000BF5D4 File Offset: 0x000BD7D4
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 findCoverDir(bool debugDraw = false)
	{
		RaycastHit raycastHit;
		Vector3 vector = this.getBestCoverDirection(this.threatTarget.getHipPosition(), 10f, out raycastHit, debugDraw);
		if (vector == Vector3.zero)
		{
			vector = (this.theEntity.position - this.threatTarget.position).normalized;
		}
		return vector;
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x000BF630 File Offset: 0x000BD830
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 getBestCoverDirection(Vector3 point, float dist, out RaycastHit hit, bool debugDraw = false)
	{
		List<EAITakeCover.PosData> list = new List<EAITakeCover.PosData>();
		hit = default(RaycastHit);
		for (int i = 0; i < EAITakeCover.mainBlockAxis.Length; i++)
		{
			if (EUtils.isPositionBlocked(point, point + EAITakeCover.mainBlockAxis[i] * dist, out hit, 65536, debugDraw))
			{
				Vector3 b = new Vector3(0f, 0.5f, 0f);
				RaycastHit raycastHit = default(RaycastHit);
				float cover = 0.5f;
				if (EUtils.isPositionBlocked(point + b, point + b + EAITakeCover.mainBlockAxis[i] * dist, out raycastHit, 65536, debugDraw))
				{
					cover = 1f;
				}
				list.Add(new EAITakeCover.PosData(EAITakeCover.mainBlockAxis[i], hit.distance, cover));
			}
		}
		float num = float.MaxValue;
		int index = 0;
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].Dist < num)
			{
				num = list[j].Dist;
				index = j;
			}
		}
		if (list.Count > 0)
		{
			if (debugDraw)
			{
				EUtils.DrawLine(point, point + list[0].Dir * dist, Color.green, 10f);
			}
			return list[index].Dir;
		}
		return Vector3.zero;
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000BF794 File Offset: 0x000BD994
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAITakeCover.CoverCastInfo> getBestCoverDirection(Vector3 point, Vector3 target, float dist, bool debugDraw = false)
	{
		List<EAITakeCover.CoverCastInfo> list = new List<EAITakeCover.CoverCastInfo>();
		Vector3 vector = new Vector3i(point).ToVector3Center();
		vector.y += 0.15f;
		for (int i = 0; i < EAITakeCover.mainBlockAxis.Length; i++)
		{
			Vector3 b = new Vector3i(EAITakeCover.mainBlockAxis[i] * dist) - EAITakeCover.halfBlockOffset;
			RaycastHit raycastHit;
			if (EUtils.isPositionBlocked(vector, vector + b, out raycastHit, 65536, debugDraw))
			{
				list.Add(new EAITakeCover.CoverCastInfo(vector, EAITakeCover.mainBlockAxis[i], raycastHit.point + Origin.position + raycastHit.normal * 0.1f, Vector3.Distance(raycastHit.point + Origin.position, target)));
			}
		}
		list.Sort((EAITakeCover.CoverCastInfo x, EAITakeCover.CoverCastInfo y) => x.ThreatDistance.CompareTo(y.ThreatDistance));
		return list;
	}

	// Token: 0x06001EA0 RID: 7840 RVA: 0x000BF89F File Offset: 0x000BDA9F
	public override string ToString()
	{
		return string.Format("{0}, state {1}, coverTicks {2}", base.ToString(), this.state, this.coverTicks);
	}

	// Token: 0x04001517 RID: 5399
	[PublicizedFrom(EAccessModifier.Private)]
	public int timeoutTicks;

	// Token: 0x04001518 RID: 5400
	[PublicizedFrom(EAccessModifier.Private)]
	public int fleeTicks;

	// Token: 0x04001519 RID: 5401
	[PublicizedFrom(EAccessModifier.Private)]
	public int coverTicks;

	// Token: 0x0400151A RID: 5402
	[PublicizedFrom(EAccessModifier.Private)]
	public int fleeDistance = 12;

	// Token: 0x0400151B RID: 5403
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector3 halfBlockOffset = Vector3.one * 0.5f;

	// Token: 0x0400151C RID: 5404
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x0400151D RID: 5405
	[PublicizedFrom(EAccessModifier.Private)]
	public PathFinderThread pathFinder;

	// Token: 0x0400151E RID: 5406
	[PublicizedFrom(EAccessModifier.Private)]
	public PathInfo pathInfo;

	// Token: 0x0400151F RID: 5407
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPathing;

	// Token: 0x04001520 RID: 5408
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3> currentPath = new List<Vector3>();

	// Token: 0x04001521 RID: 5409
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 pathEnd;

	// Token: 0x04001522 RID: 5410
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityCoverManager ecm;

	// Token: 0x04001523 RID: 5411
	[PublicizedFrom(EAccessModifier.Private)]
	public bool targetViewBlocked;

	// Token: 0x04001524 RID: 5412
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive threatTarget;

	// Token: 0x04001525 RID: 5413
	[PublicizedFrom(EAccessModifier.Private)]
	public float retryPathTicks;

	// Token: 0x04001526 RID: 5414
	[PublicizedFrom(EAccessModifier.Private)]
	public EAITakeCover.State state;

	// Token: 0x04001527 RID: 5415
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cCoverDist = 10f;

	// Token: 0x04001528 RID: 5416
	[PublicizedFrom(EAccessModifier.Private)]
	public bool findingPath;

	// Token: 0x04001529 RID: 5417
	[PublicizedFrom(EAccessModifier.Private)]
	public bool stopSeekingCover;

	// Token: 0x0400152A RID: 5418
	[PublicizedFrom(EAccessModifier.Private)]
	public static Vector3[] mainBlockAxis = new Vector3[]
	{
		new Vector3(0f, 0f, 1f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 0f, -1f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(0.5f, 0f, 0.5f),
		new Vector3(0.5f, 0f, -0.5f),
		new Vector3(-0.5f, 0f, -0.5f),
		new Vector3(-0.5f, 0f, 0.5f)
	};

	// Token: 0x020003F4 RID: 1012
	public enum State
	{
		// Token: 0x0400152C RID: 5420
		Idle,
		// Token: 0x0400152D RID: 5421
		FindPath,
		// Token: 0x0400152E RID: 5422
		PreProcessPath,
		// Token: 0x0400152F RID: 5423
		ProcessPath,
		// Token: 0x04001530 RID: 5424
		Empty
	}

	// Token: 0x020003F5 RID: 1013
	[Preserve]
	public class CoverNode
	{
		// Token: 0x17000362 RID: 866
		// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x000BF9CC File Offset: 0x000BDBCC
		public Vector3i BlockPos { get; }

		// Token: 0x06001EA3 RID: 7843 RVA: 0x000BF9D4 File Offset: 0x000BDBD4
		public CoverNode(Vector3 _pos)
		{
			this.BlockPos = new Vector3i(_pos);
		}

		// Token: 0x04001531 RID: 5425
		[PublicizedFrom(EAccessModifier.Private)]
		public List<EAITakeCover.CoverNode> neighbors = new List<EAITakeCover.CoverNode>();
	}

	// Token: 0x020003F6 RID: 1014
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public class PosData
	{
		// Token: 0x17000363 RID: 867
		// (get) Token: 0x06001EA4 RID: 7844 RVA: 0x000BF9F3 File Offset: 0x000BDBF3
		// (set) Token: 0x06001EA5 RID: 7845 RVA: 0x000BF9FB File Offset: 0x000BDBFB
		public Vector3 Dir { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17000364 RID: 868
		// (get) Token: 0x06001EA6 RID: 7846 RVA: 0x000BFA04 File Offset: 0x000BDC04
		// (set) Token: 0x06001EA7 RID: 7847 RVA: 0x000BFA0C File Offset: 0x000BDC0C
		public float Dist { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06001EA8 RID: 7848 RVA: 0x000BFA15 File Offset: 0x000BDC15
		// (set) Token: 0x06001EA9 RID: 7849 RVA: 0x000BFA1D File Offset: 0x000BDC1D
		public float Cover { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x06001EAA RID: 7850 RVA: 0x000BFA26 File Offset: 0x000BDC26
		public PosData(Vector3 _dir, float _dist, float _cover = 0.5f)
		{
			this.Dir = _dir;
			this.Dist = _dist;
			this.Cover = _cover;
		}
	}

	// Token: 0x020003F7 RID: 1015
	[Preserve]
	public class CoverCastInfo
	{
		// Token: 0x06001EAB RID: 7851 RVA: 0x000BFA43 File Offset: 0x000BDC43
		public CoverCastInfo(Vector3 _pos, Vector3 _dir, Vector3 _hitPoint, float _threatDist)
		{
			this.Set(_pos, _dir, _hitPoint, _threatDist);
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x000BFA56 File Offset: 0x000BDC56
		public void Set(Vector3 _pos, Vector3 _dir, Vector3 _hitPoint, float _threatDist)
		{
			this.Pos = _pos;
			this.Dir = _dir;
			this.HitPoint = _hitPoint;
			this.ThreatDistance = _threatDist;
		}

		// Token: 0x04001536 RID: 5430
		public Vector3 Pos;

		// Token: 0x04001537 RID: 5431
		public Vector3 Dir;

		// Token: 0x04001538 RID: 5432
		public Vector3 HitPoint;

		// Token: 0x04001539 RID: 5433
		public float ThreatDistance;
	}
}

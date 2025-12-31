using System;
using System.Collections.Generic;
using Audio;
using Unity.Collections;
using UnityEngine;

// Token: 0x02001047 RID: 4167
public class Origin : MonoBehaviour
{
	// Token: 0x060083CC RID: 33740 RVA: 0x00354338 File Offset: 0x00352538
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		Origin.Instance = this;
		this.isAuto = true;
		Shader.SetGlobalVector("_OriginPos", Origin.position);
		this.particles = new NativeArray<ParticleSystem.Particle>(512, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		this.physicsCheckT = base.transform.GetChild(0);
	}

	// Token: 0x060083CD RID: 33741 RVA: 0x0035438A File Offset: 0x0035258A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		this.particles.Dispose();
	}

	// Token: 0x060083CE RID: 33742 RVA: 0x00354397 File Offset: 0x00352597
	public static void Cleanup()
	{
		Origin.position = Vector3.zero;
		Shader.SetGlobalVector("_OriginPos", Origin.position);
	}

	// Token: 0x060083CF RID: 33743 RVA: 0x003543B7 File Offset: 0x003525B7
	public static void Add(Transform _t, int _level)
	{
		Origin.RepositionObjects.Add(new Origin.TransformLevel(_t, _level));
	}

	// Token: 0x060083D0 RID: 33744 RVA: 0x003543CC File Offset: 0x003525CC
	public static void Remove(Transform _t)
	{
		for (int i = Origin.RepositionObjects.Count - 1; i >= 0; i--)
		{
			if (Origin.RepositionObjects[i].transform == _t)
			{
				Origin.RepositionObjects.RemoveAt(i);
			}
		}
	}

	// Token: 0x060083D1 RID: 33745 RVA: 0x00354413 File Offset: 0x00352613
	public void Reposition(Vector3 _newOrigin)
	{
		this.DoReposition(_newOrigin);
		Physics.simulationMode = SimulationMode.Script;
		Physics.Simulate(0.01f);
		Physics.simulationMode = SimulationMode.FixedUpdate;
	}

	// Token: 0x060083D2 RID: 33746 RVA: 0x00354434 File Offset: 0x00352634
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoReposition(Vector3 _newOrigin)
	{
		_newOrigin.x = (float)((int)_newOrigin.x & -16);
		_newOrigin.y = (float)((int)_newOrigin.y & -16);
		_newOrigin.z = (float)((int)_newOrigin.z & -16);
		Log.Out("{0}+{1} Origin Reposition {2} to {3}", new object[]
		{
			GameManager.frameCount,
			GameManager.fixedUpdateCount,
			Origin.position.ToCultureInvariantString(),
			_newOrigin.ToCultureInvariantString()
		});
		Vector3 vector = Origin.position - _newOrigin;
		Origin.position = _newOrigin;
		this.OriginPos = _newOrigin;
		this.physicsCheckPos = -Origin.position;
		this.physicsCheckPos.y = this.physicsCheckPos.y - 256f;
		this.physicsCheckT.position = this.physicsCheckPos;
		this.checkRepositionDelay = 0;
		Shader.SetGlobalVector("_OriginPos", Origin.position);
		for (int i = 0; i < Origin.RepositionObjects.Count; i++)
		{
			Origin.RepositionTransform(vector, Origin.RepositionObjects[i].transform, Origin.RepositionObjects[i].level);
		}
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
		if (primaryPlayer)
		{
			vp_FPController component = primaryPlayer.GetComponent<vp_FPController>();
			if (component)
			{
				component.Reposition(vector);
			}
		}
		List<Entity> list = world.Entities.list;
		for (int j = list.Count - 1; j >= 0; j--)
		{
			list[j].OriginChanged(vector);
		}
		this.RepositionParticles(vector);
		if (AstarManager.Instance != null)
		{
			AstarManager.Instance.OriginChanged();
		}
		if (world.m_ChunkManager != null)
		{
			world.m_ChunkManager.OriginChanged(vector);
		}
		if (DecoManager.Instance != null)
		{
			DecoManager.Instance.OriginChanged(vector);
		}
		if (OcclusionManager.Instance)
		{
			OcclusionManager.Instance.OriginChanged(vector);
		}
		Manager.OriginChanged(vector);
		DynamicMeshManager.OriginUpdate();
		Action<Vector3> originChanged = Origin.OriginChanged;
		if (originChanged == null)
		{
			return;
		}
		originChanged(_newOrigin);
	}

	// Token: 0x060083D3 RID: 33747 RVA: 0x00354640 File Offset: 0x00352840
	[PublicizedFrom(EAccessModifier.Private)]
	public void RepositionParticles(Vector3 _deltaV)
	{
		for (int i = Origin.particleSystemTs.Count - 1; i >= 0; i--)
		{
			Transform transform = Origin.particleSystemTs[i];
			if (!transform)
			{
				Origin.particleSystemTs.RemoveAt(i);
			}
			else
			{
				transform.GetComponentsInChildren<ParticleSystem>(this.particleSystems);
				for (int j = this.particleSystems.Count - 1; j >= 0; j--)
				{
					ParticleSystem particleSystem = this.particleSystems[j];
					if (particleSystem.isPlaying && particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
					{
						int num = particleSystem.GetParticles(this.particles);
						for (int k = 0; k < num; k++)
						{
							ParticleSystem.Particle value = this.particles[k];
							value.position += _deltaV;
							this.particles[k] = value;
						}
						particleSystem.SetParticles(this.particles, num);
						particleSystem.Simulate(0f, false, false);
						particleSystem.Play(false);
					}
				}
				this.particleSystems.Clear();
			}
		}
	}

	// Token: 0x060083D4 RID: 33748 RVA: 0x00354760 File Offset: 0x00352960
	[PublicizedFrom(EAccessModifier.Private)]
	public static void RepositionTransform(Vector3 _deltaV, Transform _t, int _level)
	{
		if (!_t)
		{
			return;
		}
		if (_level < 0)
		{
			_t.position += _deltaV;
			return;
		}
		int childCount = _t.childCount;
		if (_level == 0)
		{
			for (int i = 0; i < childCount; i++)
			{
				_t.GetChild(i).position += _deltaV;
			}
			return;
		}
		for (int j = 0; j < childCount; j++)
		{
			Transform child = _t.GetChild(j);
			Origin.RepositionTransform(_deltaV, child, _level - 1);
		}
	}

	// Token: 0x060083D5 RID: 33749 RVA: 0x003547DC File Offset: 0x003529DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdate()
	{
		if (GameManager.IsDedicatedServer || !GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		if (this.isAuto)
		{
			List<EntityPlayerLocal> localPlayers = world.GetLocalPlayers();
			if (localPlayers.Count > 0)
			{
				EntityPlayerLocal player = localPlayers[0];
				this.UpdateLocalPlayer(player);
			}
		}
		else
		{
			if (this.timedMove > 0f)
			{
				this.timedMoveTime += Time.deltaTime;
				if (this.timedMoveTime >= this.timedMove)
				{
					this.timedMoveTime = 0f;
					this.timedMoveCount++;
					Vector3 newOrigin;
					newOrigin.x = (float)(this.timedMoveCount & 3) * this.timedMoveDistance.x;
					newOrigin.y = (float)(this.timedMoveCount >> 2 & 1) * this.timedMoveDistance.y;
					newOrigin.z = (float)(this.timedMoveCount >> 1 & 1) * this.timedMoveDistance.z;
					this.Reposition(newOrigin);
				}
			}
			if (this.isMoveOriginNow)
			{
				this.Reposition(this.MoveOriginTo);
				this.MoveOriginTo = Vector3.zero;
				this.isMoveOriginNow = false;
			}
		}
		if (this.checkRepositionDelay >= 0)
		{
			int num = this.checkRepositionDelay - 1;
			this.checkRepositionDelay = num;
			if (num < 0)
			{
				for (int i = 0; i < 2; i++)
				{
					bool flag = true;
					Vector3 vector = this.physicsCheckPos;
					vector.y += 10f;
					if (!Physics.Raycast(vector, Vector3.down, 3.4028235E+38f, 65536))
					{
						flag = false;
						Log.Warning("{0}+{1} Origin ray fail {2}", new object[]
						{
							GameManager.frameCount,
							GameManager.fixedUpdateCount,
							vector.ToCultureInvariantString()
						});
					}
					if (world != null)
					{
						List<EntityPlayerLocal> localPlayers2 = world.GetLocalPlayers();
						if (localPlayers2.Count > 0)
						{
							EntityPlayerLocal entityPlayerLocal = localPlayers2[0];
							if (entityPlayerLocal.IsSpawned() && !entityPlayerLocal.IsFlyMode.Value)
							{
								Vector3 vector2 = entityPlayerLocal.transform.position;
								vector2.y += 1.5f;
								if (!Physics.Raycast(vector2, Vector3.down, 3.4028235E+38f, 65536))
								{
									flag = false;
									Log.Warning("{0}+{1} Origin player ray fail {2}", new object[]
									{
										GameManager.frameCount,
										GameManager.fixedUpdateCount,
										vector2.ToCultureInvariantString()
									});
								}
							}
						}
					}
					if (flag)
					{
						this.checkRepositionDelay = -1;
						break;
					}
					Vector3 newOrigin2 = Origin.position;
					newOrigin2.x += 16f;
					this.Reposition(newOrigin2);
				}
				if (this.checkRepositionDelay >= 0)
				{
					this.checkRepositionDelay = 3;
				}
			}
		}
	}

	// Token: 0x060083D6 RID: 33750 RVA: 0x00354A9C File Offset: 0x00352C9C
	public void UpdateLocalPlayer(EntityPlayerLocal player)
	{
		if (!this.isAuto)
		{
			return;
		}
		if (player.IsSpawned() && (player.position - Origin.position).sqrMagnitude > 67600f)
		{
			this.Reposition(player.position);
		}
	}

	// Token: 0x040065C1 RID: 26049
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cAutoRepositionDistanceSq = 67600f;

	// Token: 0x040065C2 RID: 26050
	public static Origin Instance;

	// Token: 0x040065C3 RID: 26051
	public static Action<Vector3> OriginChanged;

	// Token: 0x040065C4 RID: 26052
	public static Vector3 position;

	// Token: 0x040065C5 RID: 26053
	public bool isAuto;

	// Token: 0x040065C6 RID: 26054
	public Vector3 OriginPos;

	// Token: 0x040065C7 RID: 26055
	[Tooltip("Force a move every x seconds")]
	public float timedMove;

	// Token: 0x040065C8 RID: 26056
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timedMoveTime;

	// Token: 0x040065C9 RID: 26057
	public Vector3 timedMoveDistance = new Vector3(16f, 0f, 0f);

	// Token: 0x040065CA RID: 26058
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int timedMoveCount;

	// Token: 0x040065CB RID: 26059
	public Vector3 MoveOriginTo;

	// Token: 0x040065CC RID: 26060
	public bool isMoveOriginNow;

	// Token: 0x040065CD RID: 26061
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly List<Origin.TransformLevel> RepositionObjects = new List<Origin.TransformLevel>();

	// Token: 0x040065CE RID: 26062
	public static List<Transform> particleSystemTs = new List<Transform>();

	// Token: 0x040065CF RID: 26063
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	// Token: 0x040065D0 RID: 26064
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NativeArray<ParticleSystem.Particle> particles;

	// Token: 0x040065D1 RID: 26065
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform physicsCheckT;

	// Token: 0x040065D2 RID: 26066
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 physicsCheckPos;

	// Token: 0x040065D3 RID: 26067
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int checkRepositionDelay = -1;

	// Token: 0x02001048 RID: 4168
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TransformLevel
	{
		// Token: 0x060083D9 RID: 33753 RVA: 0x00354B2F File Offset: 0x00352D2F
		public TransformLevel(Transform _transform, int _level)
		{
			this.transform = _transform;
			this.level = _level;
			this.name = this.transform.name;
		}

		// Token: 0x040065D4 RID: 26068
		public readonly Transform transform;

		// Token: 0x040065D5 RID: 26069
		public readonly int level;

		// Token: 0x040065D6 RID: 26070
		public readonly string name;
	}
}

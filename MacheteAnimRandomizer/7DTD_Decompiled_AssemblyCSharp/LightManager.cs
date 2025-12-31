using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

// Token: 0x02000FF4 RID: 4084
public class LightManager
{
	// Token: 0x06008199 RID: 33177 RVA: 0x00348394 File Offset: 0x00346594
	public static void Init()
	{
		for (int i = 0; i < 512; i++)
		{
			LightManager.regLightBins[i] = new Dictionary<Vector3i, LightManager.RegLightGroup>();
		}
	}

	// Token: 0x0600819A RID: 33178 RVA: 0x003483C0 File Offset: 0x003465C0
	public static void RegisterMovingLight(Entity owner, Light _light)
	{
		int key = (owner == null) ? -1 : owner.entityId;
		List<Light> list;
		if (!LightManager.movingLights.dict.TryGetValue(key, out list))
		{
			list = new List<Light>();
			LightManager.movingLights.Add(key, list);
		}
		list.Add(_light);
	}

	// Token: 0x0600819B RID: 33179 RVA: 0x00348410 File Offset: 0x00346610
	public static void UnRegisterMovingLight(Entity owner, Light _light)
	{
		int key = (owner == null) ? -1 : owner.entityId;
		List<Light> list;
		if (LightManager.movingLights.dict.TryGetValue(key, out list))
		{
			list.Remove(_light);
			if (list.Count < 1)
			{
				LightManager.movingLights.Remove(key);
			}
		}
	}

	// Token: 0x0600819C RID: 33180 RVA: 0x00348464 File Offset: 0x00346664
	public static float RegisterLight(Light _light)
	{
		float range = _light.range;
		if (range > 0f)
		{
			Vector3 vector = _light.transform.position + Origin.position;
			LightManager.RegisterLight(vector - Vector3.one * range, _light, range, vector, LightManager.SearchDir.All, true);
		}
		return range;
	}

	// Token: 0x0600819D RID: 33181 RVA: 0x003484B2 File Offset: 0x003466B2
	public static void UnRegisterLight(Vector3 _worldPosition, float _lightRange)
	{
		if (_lightRange <= 0f)
		{
			return;
		}
		LightManager.RegisterLight(_worldPosition - Vector3.one * _lightRange, null, _lightRange, _worldPosition, LightManager.SearchDir.All, false);
	}

	// Token: 0x0600819E RID: 33182 RVA: 0x003484D8 File Offset: 0x003466D8
	public static void Clear()
	{
		for (int i = 0; i < 512; i++)
		{
			LightManager.regLightBins[i] = new Dictionary<Vector3i, LightManager.RegLightGroup>();
		}
		LightManager.movingLights.Clear();
		foreach (KeyValuePair<Vector3i, GameObject> keyValuePair in LightManager.debugPoints)
		{
			UnityEngine.Object.Destroy(keyValuePair.Value);
		}
		LightManager.debugPoints.Clear();
	}

	// Token: 0x0600819F RID: 33183 RVA: 0x00348560 File Offset: 0x00346760
	public static void ShowSearchPattern(bool _bShow = true)
	{
		LightManager.ShowSearchPatternOn = _bShow;
		foreach (KeyValuePair<Vector3i, GameObject> keyValuePair in LightManager.debugPoints)
		{
			keyValuePair.Value.SetActive(LightManager.ShowSearchPatternOn);
		}
	}

	// Token: 0x060081A0 RID: 33184 RVA: 0x003485C4 File Offset: 0x003467C4
	public static void ShowLightLevel(bool _bShow = true)
	{
		LightManager.ShowLightLevelOn = _bShow;
	}

	// Token: 0x060081A1 RID: 33185 RVA: 0x00002914 File Offset: 0x00000B14
	public static void UpdateUI()
	{
	}

	// Token: 0x060081A2 RID: 33186 RVA: 0x003485CC File Offset: 0x003467CC
	[PublicizedFrom(EAccessModifier.Private)]
	public static float GetLightLevelFromMovingLights(int _excludeEntityId, Vector3 _worldPosition)
	{
		float num = 0f;
		foreach (KeyValuePair<int, List<Light>> keyValuePair in LightManager.movingLights.dict)
		{
			if (keyValuePair.Key != _excludeEntityId)
			{
				for (int i = 0; i < keyValuePair.Value.Count; i++)
				{
					num += LightManager.GetLightLevel(keyValuePair.Value[i], _worldPosition, true);
					if (num >= 1f)
					{
						return 1f;
					}
				}
			}
		}
		return Utils.FastClamp01(num);
	}

	// Token: 0x060081A3 RID: 33187 RVA: 0x00348674 File Offset: 0x00346874
	[PublicizedFrom(EAccessModifier.Private)]
	public static float GetLightLevel(Light _light, Vector3 _worldPosition, bool _bAllowPointToSpot = false)
	{
		if (_light == null || !_light.isActiveAndEnabled)
		{
			return 0f;
		}
		Vector3 vector = _light.transform.position + Origin.position;
		Vector3 vector2 = _worldPosition - vector;
		float range = _light.range;
		float magnitude = vector2.magnitude;
		if (magnitude >= range)
		{
			return 0f;
		}
		Vector3 vector3 = vector;
		vector3.x = Mathf.Floor(vector3.x) + 0.02f;
		vector3.y = Mathf.Floor(vector3.y) + 0.02f;
		vector3.z = Mathf.Floor(vector3.z) + 0.02f;
		if (_worldPosition.x > vector3.x)
		{
			vector3.x += 0.96f;
		}
		if (_worldPosition.y > vector3.y)
		{
			vector3.y += 0.96f;
		}
		if (_worldPosition.z > vector3.z)
		{
			vector3.z += 0.96f;
		}
		Vector3 direction = _worldPosition - vector3;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(vector3 - Origin.position, direction), out raycastHit, range + 0.25f, 65536) && raycastHit.distance < direction.magnitude - 0.25f)
		{
			return 0f;
		}
		LightType lightType = _light.type;
		float num = _light.spotAngle;
		if (_bAllowPointToSpot && _light.cookie)
		{
			float cookieSize = _light.cookieSize;
			float num2 = Mathf.Sqrt(Mathf.Pow(cookieSize * 0.5f, 2f) + Mathf.Pow(range, 2f));
			num = Mathf.Acos(cookieSize * 0.5f / num2) * 57.295776f;
			lightType = LightType.Spot;
		}
		Color color = _light.color;
		if (lightType != LightType.Spot)
		{
			return (1f - magnitude / range) * Utils.FastClamp01(_light.intensity) * SkyManager.GetLuma(color) * color.a;
		}
		float num3 = Vector3.Dot(_light.transform.forward, vector2.normalized);
		float num4 = 1f - num / 180f;
		num4 = Utils.FastClamp01(num4 * 1.1f);
		if (num3 < num4)
		{
			return 0f;
		}
		float num5 = Utils.FastClamp01(num3);
		if (num4 < 1f)
		{
			num5 = (num3 - num4) / (1f - num4);
		}
		num5 = Mathf.Pow(num5, 1.25f);
		return (1f - magnitude / range) * Utils.FastClamp01(_light.intensity) * SkyManager.GetLuma(color) * num5;
	}

	// Token: 0x060081A4 RID: 33188 RVA: 0x003488F8 File Offset: 0x00346AF8
	[PublicizedFrom(EAccessModifier.Private)]
	public static float BlockLight(Vector3 _worldPosition)
	{
		float num = 1f;
		Vector3i vector3i = World.worldToBlockPos(_worldPosition);
		IChunk chunkFromWorldPos = GameManager.Instance.World.GetChunkFromWorldPos(vector3i);
		if (chunkFromWorldPos != null && vector3i.y >= 0 && vector3i.y < 255)
		{
			num = (float)Utils.FastMax(chunkFromWorldPos.GetLight(vector3i.x, vector3i.y, vector3i.z, Chunk.LIGHT_TYPE.SUN), chunkFromWorldPos.GetLight(vector3i.x, vector3i.y + 1, vector3i.z, Chunk.LIGHT_TYPE.SUN));
			num /= 15f;
		}
		return num;
	}

	// Token: 0x060081A5 RID: 33189 RVA: 0x00348980 File Offset: 0x00346B80
	[PublicizedFrom(EAccessModifier.Private)]
	public static float CalcShadeLight(Vector3 _worldPosition)
	{
		bool flag = false;
		Vector3 vector;
		if (SkyManager.GetSunIntensity() < 0.05f)
		{
			vector = SkyManager.GetMoonDirection();
			if (vector.y > -0.087f)
			{
				return 0f;
			}
			flag = true;
		}
		else
		{
			vector = SkyManager.GetSunLightDirection();
		}
		float result = (float)(flag ? -1 : 1);
		Vector3 origin = _worldPosition - Origin.position;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(origin, -vector), out raycastHit, float.PositiveInfinity, 8454417) && raycastHit.distance < float.PositiveInfinity)
		{
			result = 0f;
		}
		return result;
	}

	// Token: 0x060081A6 RID: 33190 RVA: 0x00348A08 File Offset: 0x00346C08
	[PublicizedFrom(EAccessModifier.Private)]
	public static float GetLightLevel(Vector3 _worldPosition)
	{
		float num = 0f;
		Dictionary<Vector3, Light> lightsEffecting = LightManager.GetLightsEffecting(_worldPosition);
		if (lightsEffecting != null && lightsEffecting.Count > 0)
		{
			LightManager.lightsEffectingRemovals.Clear();
			foreach (KeyValuePair<Vector3, Light> keyValuePair in lightsEffecting)
			{
				Light value = keyValuePair.Value;
				if (value == null)
				{
					LightManager.lightsEffectingRemovals.Add(keyValuePair.Key);
				}
				else
				{
					num += LightManager.GetLightLevel(value, _worldPosition, false);
					if (num >= 1f)
					{
						break;
					}
				}
			}
			for (int i = LightManager.lightsEffectingRemovals.Count - 1; i >= 0; i--)
			{
				lightsEffecting.Remove(LightManager.lightsEffectingRemovals[i]);
			}
		}
		if (num >= 1f)
		{
			return 1f;
		}
		float num2 = Mathf.Pow(WorldEnvironment.AmbientTotal, 0.6f);
		num += num2 * 0.5f;
		num += LightManager.BlockLight(_worldPosition) * num2 * 0.5f;
		if (num >= 1f)
		{
			return 1f;
		}
		float num3 = LightManager.CalcShadeLight(_worldPosition);
		if (num3 >= 0f)
		{
			num += num3;
		}
		else
		{
			num += SkyManager.GetMoonBrightness();
		}
		return Utils.FastClamp01(num);
	}

	// Token: 0x060081A7 RID: 33191 RVA: 0x00348B50 File Offset: 0x00346D50
	public static Dictionary<Vector3, Light> GetLightsEffecting(Vector3 _worldPosition)
	{
		Dictionary<Vector3i, LightManager.RegLightGroup> dictionary = LightManager.regLightBins[Utils.Fastfloor(_worldPosition.x) >> 3 & 511];
		Vector3i vector3i = World.worldToBlockPos(_worldPosition);
		vector3i &= -8;
		LightManager.RegLightGroup regLightGroup;
		if (!dictionary.TryGetValue(vector3i, out regLightGroup))
		{
			return null;
		}
		return regLightGroup.lights;
	}

	// Token: 0x060081A8 RID: 33192 RVA: 0x00348B98 File Offset: 0x00346D98
	[PublicizedFrom(EAccessModifier.Private)]
	public static void RegisterLight(Vector3 _searchPosition, Light _light, float _lightRange, Vector3 _lightPosition, LightManager.SearchDir _, bool _bRegister)
	{
		int num = Mathf.CeilToInt(_lightRange * 0.89f);
		if (num <= 0)
		{
			return;
		}
		int num2 = num * 2;
		Vector3i vector3i = World.worldToBlockPos(_searchPosition);
		Vector3i vector3i2 = vector3i + new Vector3i(num2, num2, num2);
		Vector3i vector3i3 = vector3i & -8;
		Vector3i vector3i4;
		vector3i4.y = vector3i3.y;
		while (vector3i4.y <= vector3i2.y)
		{
			vector3i4.z = vector3i3.z;
			while (vector3i4.z <= vector3i2.z)
			{
				vector3i4.x = vector3i3.x;
				while (vector3i4.x <= vector3i2.x)
				{
					Dictionary<Vector3i, LightManager.RegLightGroup> dictionary = LightManager.regLightBins[vector3i4.x >> 3 & 511];
					LightManager.RegLightGroup regLightGroup;
					if (!dictionary.TryGetValue(vector3i4, out regLightGroup))
					{
						regLightGroup.lights = new Dictionary<Vector3, Light>();
						dictionary.Add(vector3i4, regLightGroup);
					}
					if (_bRegister)
					{
						regLightGroup.lights[_lightPosition] = _light;
					}
					else
					{
						regLightGroup.lights.Remove(_lightPosition);
						if (regLightGroup.lights.Count == 0)
						{
							dictionary.Remove(vector3i4);
							regLightGroup.lights = null;
						}
					}
					vector3i4.x += 8;
				}
				vector3i4.z += 8;
			}
			vector3i4.y += 8;
		}
	}

	// Token: 0x060081A9 RID: 33193 RVA: 0x00348CE5 File Offset: 0x00346EE5
	public static void SetPlayerLightLevel(float _lightLevel)
	{
		LightManager.PlayerLightLevel = _lightLevel;
	}

	// Token: 0x060081AA RID: 33194 RVA: 0x00348CF0 File Offset: 0x00346EF0
	public static float GetStealthLightLevel(EntityAlive _entity, out float selfLight)
	{
		if (LightManager.myServer != null)
		{
			Vector3 position = _entity.position;
			position.y += 1.68f;
			float v = LightManager.GetLightLevel(position) + LightManager.GetLightLevelFromMovingLights(_entity.entityId, position);
			selfLight = _entity.GetLightLevel();
			return Utils.FastClamp01(v);
		}
		selfLight = 0f;
		return 0f;
	}

	// Token: 0x060081AB RID: 33195 RVA: 0x00348D48 File Offset: 0x00346F48
	public static float GetWorldLightLevelInRange(Vector3 pos, float distanceMax)
	{
		float lightLevel = LightManager.GetLightLevel(pos);
		if (lightLevel >= 1f)
		{
			return 1f;
		}
		return lightLevel;
	}

	// Token: 0x060081AC RID: 33196 RVA: 0x00348D6C File Offset: 0x00346F6C
	public static void LightChanged(Vector3 lightPos)
	{
		List<EntityPlayerLocal> localPlayers = GameManager.Instance.World.GetLocalPlayers();
		for (int i = localPlayers.Count - 1; i >= 0; i--)
		{
			localPlayers[i].renderManager.reflectionManager.LightChanged(lightPos);
		}
	}

	// Token: 0x060081AD RID: 33197 RVA: 0x00348DB3 File Offset: 0x00346FB3
	public static void Dispose()
	{
		if (LightManager.myServer != null)
		{
			LightManager.myServer.Dispose();
			LightManager.myServer = null;
		}
	}

	// Token: 0x060081AE RID: 33198 RVA: 0x00348DCC File Offset: 0x00346FCC
	public static void AttachLocalPlayer(EntityPlayerLocal localPlayer, World world)
	{
		if (LightManager.myServer != null)
		{
			LightManager.myServer.AttachLocalPlayer(localPlayer);
		}
	}

	// Token: 0x060081AF RID: 33199 RVA: 0x00348DE0 File Offset: 0x00346FE0
	public static void EntityAddedToWorld(Entity entity, World world)
	{
		if (LightManager.myServer != null)
		{
			LightManager.myServer.EntityAddedToWorld(entity, world);
		}
	}

	// Token: 0x060081B0 RID: 33200 RVA: 0x00348DF5 File Offset: 0x00346FF5
	public static void EntityRemovedFromWorld(Entity entity, World world)
	{
		if (entity == null)
		{
			return;
		}
		if (LightManager.myServer != null)
		{
			LightManager.myServer.EntityRemovedFromWorld(entity, world);
		}
	}

	// Token: 0x060081B1 RID: 33201 RVA: 0x00348E14 File Offset: 0x00347014
	public static void CreateServer()
	{
		if (LightManager.myServer == null && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			LightManager.myServer = new LightManager.Server();
		}
	}

	// Token: 0x04006444 RID: 25668
	[PublicizedFrom(EAccessModifier.Private)]
	public const int raycastMask = 65536;

	// Token: 0x04006445 RID: 25669
	[PublicizedFrom(EAccessModifier.Private)]
	public const int enclosureRaycastMask = 8454417;

	// Token: 0x04006446 RID: 25670
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRegGridSize = 8;

	// Token: 0x04006447 RID: 25671
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRegPosMask = -8;

	// Token: 0x04006448 RID: 25672
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRegBins = 512;

	// Token: 0x04006449 RID: 25673
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRegBinShift = 3;

	// Token: 0x0400644A RID: 25674
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<Vector3i, LightManager.RegLightGroup>[] regLightBins = new Dictionary<Vector3i, LightManager.RegLightGroup>[512];

	// Token: 0x0400644B RID: 25675
	[PublicizedFrom(EAccessModifier.Private)]
	public static DictionaryList<int, List<Light>> movingLights = new DictionaryList<int, List<Light>>();

	// Token: 0x0400644C RID: 25676
	[PublicizedFrom(EAccessModifier.Private)]
	public static LightManager.Server myServer = null;

	// Token: 0x0400644D RID: 25677
	[PublicizedFrom(EAccessModifier.Private)]
	public static float PlayerLightLevel = 0f;

	// Token: 0x0400644E RID: 25678
	public static bool ShowSearchPatternOn = false;

	// Token: 0x0400644F RID: 25679
	public static bool ShowLightLevelOn = false;

	// Token: 0x04006450 RID: 25680
	[PublicizedFrom(EAccessModifier.Private)]
	public static Canvas debugUI = null;

	// Token: 0x04006451 RID: 25681
	[PublicizedFrom(EAccessModifier.Private)]
	public static Text debugText;

	// Token: 0x04006452 RID: 25682
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<Vector3i, GameObject> debugPoints = new Dictionary<Vector3i, GameObject>();

	// Token: 0x04006453 RID: 25683
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject LightLevelDebugPoints = new GameObject("LightLevelDebugPoints");

	// Token: 0x04006454 RID: 25684
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Vector3> lightsEffectingRemovals = new List<Vector3>();

	// Token: 0x02000FF5 RID: 4085
	[PublicizedFrom(EAccessModifier.Private)]
	public enum SearchDir
	{
		// Token: 0x04006456 RID: 25686
		All,
		// Token: 0x04006457 RID: 25687
		Forward,
		// Token: 0x04006458 RID: 25688
		Right
	}

	// Token: 0x02000FF6 RID: 4086
	[PublicizedFrom(EAccessModifier.Private)]
	public struct RegLightGroup
	{
		// Token: 0x04006459 RID: 25689
		public Dictionary<Vector3, Light> lights;
	}

	// Token: 0x02000FF7 RID: 4087
	[Preserve]
	public class NetPackageLight : NetPackage
	{
		// Token: 0x17000D80 RID: 3456
		// (get) Token: 0x060081B4 RID: 33204 RVA: 0x000282C0 File Offset: 0x000264C0
		public override NetPackageDirection PackageDirection
		{
			get
			{
				return NetPackageDirection.ToClient;
			}
		}

		// Token: 0x060081B5 RID: 33205 RVA: 0x00348E9F File Offset: 0x0034709F
		public LightManager.NetPackageLight Setup(int _entityId, float _lightLevel = 0f)
		{
			this.entityId = _entityId;
			this.lightLevel = _lightLevel;
			return this;
		}

		// Token: 0x060081B6 RID: 33206 RVA: 0x00348EB0 File Offset: 0x003470B0
		public override void read(PooledBinaryReader _reader)
		{
			this.entityId = _reader.ReadInt32();
			this.lightLevel = _reader.ReadSingle();
		}

		// Token: 0x060081B7 RID: 33207 RVA: 0x00348ECA File Offset: 0x003470CA
		public override void write(PooledBinaryWriter _writer)
		{
			base.write(_writer);
			_writer.Write(this.entityId);
			_writer.Write(this.lightLevel);
		}

		// Token: 0x060081B8 RID: 33208 RVA: 0x00348EEC File Offset: 0x003470EC
		public override void ProcessPackage(World _world, GameManager _callbacks)
		{
			if (GameManager.Instance == null || GameManager.Instance.World == null)
			{
				return;
			}
			if (LightManager.myServer == null)
			{
				LightManager.SetPlayerLightLevel(this.lightLevel);
				return;
			}
			Entity entity = GameManager.Instance.World.GetEntity(this.entityId);
			if (entity == null)
			{
				return;
			}
			float num = LightManager.GetLightLevel(entity.position + Vector3.up * 1.68f) + LightManager.GetLightLevelFromMovingLights(this.entityId, entity.position + Vector3.up * 1.68f);
			num += ((EntityAlive)entity).GetLightLevel();
			LightManager.myServer.SendLightLevel(this.entityId, num);
		}

		// Token: 0x060081B9 RID: 33209 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override int GetLength()
		{
			return 0;
		}

		// Token: 0x0400645A RID: 25690
		public int entityId;

		// Token: 0x0400645B RID: 25691
		public float lightLevel;
	}

	// Token: 0x02000FF8 RID: 4088
	public class Server : IDisposable
	{
		// Token: 0x060081BC RID: 33212 RVA: 0x00348FC4 File Offset: 0x003471C4
		public void SendLightLevel(int entityId, float lightLevel)
		{
			LightManager.Client client;
			if (this.m_players.TryGetValue(entityId, out client))
			{
				client.SetPlayerLightLevel(lightLevel);
			}
		}

		// Token: 0x060081BD RID: 33213 RVA: 0x00348FE8 File Offset: 0x003471E8
		public void AttachLocalPlayer(EntityPlayerLocal localPlayer)
		{
			this.m_localPlayer = localPlayer;
		}

		// Token: 0x060081BE RID: 33214 RVA: 0x00348FF4 File Offset: 0x003471F4
		public void EntityAddedToWorld(Entity entity, World world)
		{
			if (entity is EntityPlayer && (this.m_localPlayer == null || entity.entityId != this.m_localPlayer.entityId))
			{
				LightManager.Client value;
				if (this.m_players.TryGetValue(entity.entityId, out value))
				{
					return;
				}
				value = new LightManager.Client(entity.entityId);
				this.m_players[entity.entityId] = value;
			}
		}

		// Token: 0x060081BF RID: 33215 RVA: 0x00349060 File Offset: 0x00347260
		public void EntityRemovedFromWorld(Entity entity, World world)
		{
			LightManager.Client client;
			if (this.m_players.TryGetValue(entity.entityId, out client))
			{
				this.m_players.Remove(entity.entityId);
				client.Dispose();
			}
		}

		// Token: 0x060081C0 RID: 33216 RVA: 0x0034909C File Offset: 0x0034729C
		public void Dispose()
		{
			foreach (KeyValuePair<int, LightManager.Client> keyValuePair in this.m_players)
			{
				keyValuePair.Value.Dispose();
			}
			this.m_players = null;
			this.m_localPlayer = null;
		}

		// Token: 0x0400645C RID: 25692
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal m_localPlayer;

		// Token: 0x0400645D RID: 25693
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<int, LightManager.Client> m_players = new Dictionary<int, LightManager.Client>();
	}

	// Token: 0x02000FF9 RID: 4089
	public class Client : IDisposable
	{
		// Token: 0x060081C1 RID: 33217 RVA: 0x00349104 File Offset: 0x00347304
		public Client(int _entityId)
		{
			this.entityId = _entityId;
		}

		// Token: 0x060081C2 RID: 33218 RVA: 0x00002914 File Offset: 0x00000B14
		public void Dispose()
		{
		}

		// Token: 0x060081C3 RID: 33219 RVA: 0x00349114 File Offset: 0x00347314
		public void SetPlayerLightLevel(float _lightLevel)
		{
			LightManager.NetPackageLight package = NetPackageManager.GetPackage<LightManager.NetPackageLight>().Setup(this.entityId, _lightLevel);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x0400645E RID: 25694
		public int entityId;
	}
}

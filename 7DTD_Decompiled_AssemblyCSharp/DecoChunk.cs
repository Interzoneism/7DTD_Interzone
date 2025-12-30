using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000A9B RID: 2715
public class DecoChunk
{
	// Token: 0x060053D8 RID: 21464 RVA: 0x0021B23C File Offset: 0x0021943C
	public DecoChunk(int _x, int _z, int _drawX, int _drawZ)
	{
		this.Reset(_x, _z, _drawX, _drawZ);
	}

	// Token: 0x060053D9 RID: 21465 RVA: 0x0021B288 File Offset: 0x00219488
	public void Reset(int _x, int _z, int _drawX, int _drawZ)
	{
		this.decoChunkX = _x;
		this.decoChunkZ = _z;
		this.drawX = _drawX;
		this.drawZ = _drawZ;
		this.decosPerSmallChunks.Clear();
		this.isDecorated = false;
		this.isModelsUpdated = false;
		this.isGameObjectUpdated = false;
	}

	// Token: 0x060053DA RID: 21466 RVA: 0x0021B2C8 File Offset: 0x002194C8
	public void RestoreGeneratedDecos(Predicate<DecoObject> decoObjectValidator = null)
	{
		foreach (long smallChunkKey in this.decosPerSmallChunks.Keys)
		{
			this.RestoreGeneratedDecos(smallChunkKey, decoObjectValidator);
		}
	}

	// Token: 0x060053DB RID: 21467 RVA: 0x0021B324 File Offset: 0x00219524
	public void RestoreGeneratedDecos(long smallChunkKey, Predicate<DecoObject> decoObjectValidator = null)
	{
		List<DecoObject> list;
		if (this.decosPerSmallChunks.TryGetValue(smallChunkKey, out list))
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				DecoObject decoObject = list[i];
				if (decoObjectValidator == null || decoObjectValidator(decoObject))
				{
					switch (decoObject.state)
					{
					case DecoState.GeneratedInactive:
						decoObject.state = DecoState.GeneratedActive;
						this.isModelsUpdated = false;
						break;
					case DecoState.Dynamic:
						this.RemoveDecoObject(decoObject);
						break;
					}
				}
			}
		}
	}

	// Token: 0x060053DC RID: 21468 RVA: 0x002015FC File Offset: 0x001FF7FC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int MakeKey16(int _x, int _z)
	{
		return _x << 16 | (_z & 65535);
	}

	// Token: 0x060053DD RID: 21469 RVA: 0x0021B39A File Offset: 0x0021959A
	public static int ToDecoChunkPos(float _worldPos)
	{
		return Utils.Fastfloor(_worldPos / 128f);
	}

	// Token: 0x060053DE RID: 21470 RVA: 0x0021B3A8 File Offset: 0x002195A8
	public static int ToDecoChunkPos(int _worldPos)
	{
		if (_worldPos >= 0)
		{
			return _worldPos / 128;
		}
		return (_worldPos - 128 + 1) / 128;
	}

	// Token: 0x060053DF RID: 21471 RVA: 0x0021B3C8 File Offset: 0x002195C8
	public void UpdateGameObject()
	{
		if (!this.rootObj)
		{
			this.rootObj = new GameObject();
		}
		this.SetVisible(true);
		this.rootObj.name = "DecoC_" + this.decoChunkX.ToString() + "_" + this.decoChunkZ.ToString();
		this.rootObj.transform.position = new Vector3((float)(this.drawX * 128), 0f, (float)(this.drawZ * 128)) - Origin.position;
		this.isGameObjectUpdated = true;
	}

	// Token: 0x060053E0 RID: 21472 RVA: 0x0021B469 File Offset: 0x00219669
	public IEnumerator UpdateModels(World _world, MicroStopwatch ms)
	{
		this.SetVisible(true);
		foreach (KeyValuePair<long, List<DecoObject>> keyValuePair in this.decosPerSmallChunks)
		{
			List<DecoObject> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				DecoObject decoObject = value[i];
				if (decoObject.state != DecoState.GeneratedInactive && !decoObject.go && decoObject.asyncItem == null)
				{
					string modelName = decoObject.GetModelName();
					List<DecoObject> list;
					if (!this.models.TryGetValue(modelName, out list))
					{
						list = new List<DecoObject>();
						this.models.Add(modelName, list);
					}
					list.Add(decoObject);
				}
			}
		}
		foreach (KeyValuePair<string, List<DecoObject>> keyValuePair2 in this.models)
		{
			List<DecoObject> value2 = keyValuePair2.Value;
			GameObjectPool.AsyncItem objectsForTypeAsync = GameObjectPool.Instance.GetObjectsForTypeAsync(keyValuePair2.Key, value2.Count, new GameObjectPool.CreateAsyncCallback(this.CreateGameObjectCallback), value2);
			if (objectsForTypeAsync != null)
			{
				this.asyncItems.Add(objectsForTypeAsync);
				for (int j = 0; j < value2.Count; j++)
				{
					value2[j].asyncItem = objectsForTypeAsync;
				}
			}
			if (ms.ElapsedMicroseconds > 900L)
			{
				yield return null;
				ms.ResetAndRestart();
			}
		}
		Dictionary<string, List<DecoObject>>.Enumerator enumerator2 = default(Dictionary<string, List<DecoObject>>.Enumerator);
		this.models.Clear();
		this.isModelsUpdated = true;
		yield break;
		yield break;
	}

	// Token: 0x060053E1 RID: 21473 RVA: 0x0021B480 File Offset: 0x00219680
	public void CreateGameObjectCallback(object _userData, UnityEngine.Object[] _objs, int _objsCount, bool _isAsync)
	{
		List<DecoObject> list = (List<DecoObject>)_userData;
		Transform transform = this.rootObj.transform;
		for (int i = 0; i < _objsCount; i++)
		{
			GameObject gameObject = (GameObject)_objs[i];
			list[i].CreateGameObjectCallback(gameObject, transform, _isAsync);
			this.occlusionTs.Add(gameObject.transform);
		}
		if (this.occlusionTs.Count > 0)
		{
			if (OcclusionManager.Instance.cullDecorations)
			{
				OcclusionManager.Instance.AddDeco(this, this.occlusionTs);
			}
			this.occlusionTs.Clear();
		}
	}

	// Token: 0x060053E2 RID: 21474 RVA: 0x0021B50C File Offset: 0x0021970C
	public void AddDecoObject(DecoObject _decoObject, bool _tryInstantiate = false)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(_decoObject.pos.x), World.toChunkXZ(_decoObject.pos.z));
		List<DecoObject> list;
		if (!this.decosPerSmallChunks.TryGetValue(key, out list))
		{
			list = new List<DecoObject>(64);
			this.decosPerSmallChunks.Add(key, list);
		}
		list.Add(_decoObject);
		if (_tryInstantiate)
		{
			if (ThreadManager.IsMainThread() && this.rootObj)
			{
				_decoObject.CreateGameObject(this, this.rootObj.transform);
				if (OcclusionManager.Instance.cullDecorations && _decoObject.go)
				{
					this.occlusionTs.Add(_decoObject.go.transform);
					OcclusionManager.Instance.AddDeco(this, this.occlusionTs);
					this.occlusionTs.Clear();
					return;
				}
			}
			else
			{
				this.isModelsUpdated = false;
			}
		}
	}

	// Token: 0x060053E3 RID: 21475 RVA: 0x0021B5E8 File Offset: 0x002197E8
	public DecoObject GetDecoObjectAt(Vector3i _worldBlockPos)
	{
		long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(_worldBlockPos.x), World.toChunkXZ(_worldBlockPos.z));
		List<DecoObject> list;
		if (!this.decosPerSmallChunks.TryGetValue(key, out list))
		{
			return null;
		}
		foreach (DecoObject decoObject in list)
		{
			if (decoObject.pos.x == _worldBlockPos.x && decoObject.pos.z == _worldBlockPos.z && decoObject.state != DecoState.GeneratedInactive)
			{
				return decoObject;
			}
		}
		return null;
	}

	// Token: 0x060053E4 RID: 21476 RVA: 0x0021B698 File Offset: 0x00219898
	public bool RemoveDecoObject(Vector3i _worldBlockPos)
	{
		DecoObject decoObjectAt = this.GetDecoObjectAt(_worldBlockPos);
		if (decoObjectAt == null)
		{
			return false;
		}
		this.RemoveDecoObject(decoObjectAt);
		return true;
	}

	// Token: 0x060053E5 RID: 21477 RVA: 0x0021B6BC File Offset: 0x002198BC
	public void RemoveDecoObject(DecoObject deco)
	{
		if (deco.state == DecoState.Dynamic)
		{
			long key = WorldChunkCache.MakeChunkKey(World.toChunkXZ(deco.pos.x), World.toChunkXZ(deco.pos.z));
			List<DecoObject> list;
			if (this.decosPerSmallChunks.TryGetValue(key, out list))
			{
				list.Remove(deco);
			}
		}
		else
		{
			deco.state = DecoState.GeneratedInactive;
		}
		if (OcclusionManager.Instance.cullDecorations && deco.go)
		{
			OcclusionManager.Instance.RemoveDeco(this, deco.go.transform);
		}
		deco.Destroy();
	}

	// Token: 0x060053E6 RID: 21478 RVA: 0x0021B750 File Offset: 0x00219950
	public void Destroy()
	{
		if (OcclusionManager.Instance.cullDecorations)
		{
			OcclusionManager.Instance.RemoveDecoChunk(this);
		}
		foreach (KeyValuePair<long, List<DecoObject>> keyValuePair in this.decosPerSmallChunks)
		{
			List<DecoObject> value = keyValuePair.Value;
			for (int i = 0; i < value.Count; i++)
			{
				value[i].Destroy();
			}
		}
		for (int j = 0; j < this.asyncItems.Count; j++)
		{
			GameObjectPool.Instance.CancelAsync(this.asyncItems[j]);
		}
		this.asyncItems.Clear();
		this.isModelsUpdated = false;
		this.isGameObjectUpdated = false;
		UnityEngine.Object.Destroy(this.rootObj);
	}

	// Token: 0x060053E7 RID: 21479 RVA: 0x0021B830 File Offset: 0x00219A30
	public void SetVisible(bool _bVisible)
	{
		if (this.rootObj && this.rootObj.activeSelf != _bVisible)
		{
			this.rootObj.SetActive(_bVisible);
		}
	}

	// Token: 0x060053E8 RID: 21480 RVA: 0x0021B859 File Offset: 0x00219A59
	public override string ToString()
	{
		return string.Format("DecoChunk {0},{1}", this.decoChunkX, this.decoChunkZ);
	}

	// Token: 0x04004020 RID: 16416
	public int decoChunkX;

	// Token: 0x04004021 RID: 16417
	public int decoChunkZ;

	// Token: 0x04004022 RID: 16418
	public int drawX;

	// Token: 0x04004023 RID: 16419
	public int drawZ;

	// Token: 0x04004024 RID: 16420
	public bool isDecorated;

	// Token: 0x04004025 RID: 16421
	public bool isModelsUpdated;

	// Token: 0x04004026 RID: 16422
	public bool isGameObjectUpdated;

	// Token: 0x04004027 RID: 16423
	public GameObject rootObj;

	// Token: 0x04004028 RID: 16424
	public Dictionary<long, List<DecoObject>> decosPerSmallChunks = new Dictionary<long, List<DecoObject>>(64);

	// Token: 0x04004029 RID: 16425
	public OcclusionManager.OccludeeZone occludeeZone;

	// Token: 0x0400402A RID: 16426
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<Transform> occlusionTs = new List<Transform>();

	// Token: 0x0400402B RID: 16427
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<GameObjectPool.AsyncItem> asyncItems = new List<GameObjectPool.AsyncItem>();

	// Token: 0x0400402C RID: 16428
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, List<DecoObject>> models = new Dictionary<string, List<DecoObject>>();
}

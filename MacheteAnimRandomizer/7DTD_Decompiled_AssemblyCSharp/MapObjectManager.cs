using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000598 RID: 1432
public class MapObjectManager
{
	// Token: 0x1400003E RID: 62
	// (add) Token: 0x06002E25 RID: 11813 RVA: 0x00131C38 File Offset: 0x0012FE38
	// (remove) Token: 0x06002E26 RID: 11814 RVA: 0x00131C70 File Offset: 0x0012FE70
	public event MapObjectManager.MapObjectListChangedDelegate ChangedDelegates;

	// Token: 0x06002E27 RID: 11815 RVA: 0x00131CA5 File Offset: 0x0012FEA5
	public static void Reset()
	{
		MapObjectManager.entityList.Clear();
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x00131CB4 File Offset: 0x0012FEB4
	public MapObjectManager()
	{
		for (int i = 0; i < 17; i++)
		{
			this.mapObjects.Add(new DictionaryList<int, MapObject>());
		}
		foreach (MapObject mapObject in MapObjectManager.entityList)
		{
			if (mapObject is MapObjectVehicle)
			{
				this.Add(new MapObjectVehicle(mapObject as MapObjectVehicle));
			}
			else
			{
				this.Add(new MapObject(mapObject));
			}
		}
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x00131CA5 File Offset: 0x0012FEA5
	public static void ClearEntityList()
	{
		MapObjectManager.entityList.Clear();
	}

	// Token: 0x06002E2A RID: 11818 RVA: 0x00131D54 File Offset: 0x0012FF54
	public void Add(MapObject _mapObject)
	{
		if (this.mapObjects[(int)_mapObject.type].dict.ContainsKey((int)_mapObject.key))
		{
			this.Remove(_mapObject.type, (int)_mapObject.key);
		}
		this.mapObjects[(int)_mapObject.type].Add((int)_mapObject.key, _mapObject);
		if (this.ChangedDelegates != null)
		{
			this.ChangedDelegates(_mapObject.type, _mapObject, true);
		}
		if (_mapObject.type == EnumMapObjectType.Entity && !MapObjectManager.entityList.Contains(_mapObject))
		{
			MapObjectManager.entityList.Add(_mapObject);
		}
	}

	// Token: 0x06002E2B RID: 11819 RVA: 0x00131DF4 File Offset: 0x0012FFF4
	public void Remove(EnumMapObjectType _type, int _key)
	{
		if (this.mapObjects[(int)_type].dict.ContainsKey(_key))
		{
			MapObject mapObject = this.mapObjects[(int)_type].dict[_key];
			if (mapObject.type == EnumMapObjectType.Entity && MapObjectManager.entityList.Contains(mapObject))
			{
				MapObjectManager.entityList.Remove(mapObject);
			}
			this.mapObjects[(int)_type].Remove(_key);
			if (this.ChangedDelegates != null)
			{
				this.ChangedDelegates(_type, mapObject, false);
			}
		}
	}

	// Token: 0x06002E2C RID: 11820 RVA: 0x00131E7C File Offset: 0x0013007C
	public void RemoveByPosition(EnumMapObjectType _type, Vector3 _position)
	{
		for (int i = this.mapObjects[(int)_type].list.Count - 1; i >= 0; i--)
		{
			Vector3 position = this.mapObjects[(int)_type].list[i].GetPosition();
			if (position.x == _position.x && position.z == _position.z)
			{
				this.mapObjects[(int)_type].list.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002E2D RID: 11821 RVA: 0x00131EFC File Offset: 0x001300FC
	public void RemoveByType(EnumMapObjectType _type)
	{
		for (int i = this.mapObjects[(int)_type].list.Count - 1; i >= 0; i--)
		{
			if (this.mapObjects[(int)_type].list[i].type == _type)
			{
				this.mapObjects[(int)_type].list.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002E2E RID: 11822 RVA: 0x00131F62 File Offset: 0x00130162
	public void Clear()
	{
		this.mapObjects.Clear();
	}

	// Token: 0x06002E2F RID: 11823 RVA: 0x00131F6F File Offset: 0x0013016F
	public List<MapObject> GetList(EnumMapObjectType _type)
	{
		return this.mapObjects[(int)_type].list;
	}

	// Token: 0x04002498 RID: 9368
	[PublicizedFrom(EAccessModifier.Private)]
	public List<DictionaryList<int, MapObject>> mapObjects = new List<DictionaryList<int, MapObject>>();

	// Token: 0x0400249A RID: 9370
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<MapObject> entityList = new List<MapObject>();

	// Token: 0x02000599 RID: 1433
	// (Invoke) Token: 0x06002E32 RID: 11826
	public delegate void MapObjectListChangedDelegate(EnumMapObjectType _type, MapObject _mapObject, bool _bAdded);
}

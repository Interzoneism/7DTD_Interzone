using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02001099 RID: 4249
public class WaypointCollection
{
	// Token: 0x06008609 RID: 34313 RVA: 0x00367014 File Offset: 0x00365214
	public void Read(BinaryReader _br)
	{
		this.Collection.Clear();
		int version = (int)_br.ReadByte();
		int num = (int)_br.ReadUInt16();
		for (int i = 0; i < num; i++)
		{
			Waypoint waypoint = new Waypoint();
			waypoint.Read(_br, version);
			this.Collection.Add(waypoint);
		}
	}

	// Token: 0x0600860A RID: 34314 RVA: 0x00367060 File Offset: 0x00365260
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(7);
		int num = 0;
		for (int i = 0; i < this.Collection.list.Count; i++)
		{
			if (this.Collection.list[i].IsSaved)
			{
				num++;
			}
		}
		_bw.Write((ushort)num);
		for (int j = 0; j < this.Collection.list.Count; j++)
		{
			if (this.Collection.list[j].IsSaved)
			{
				this.Collection.list[j].Write(_bw);
			}
		}
	}

	// Token: 0x0600860B RID: 34315 RVA: 0x00367100 File Offset: 0x00365300
	public WaypointCollection Clone()
	{
		WaypointCollection waypointCollection = new WaypointCollection();
		for (int i = 0; i < this.Collection.list.Count; i++)
		{
			waypointCollection.Collection.Add(this.Collection.list[i].Clone());
		}
		return waypointCollection;
	}

	// Token: 0x0600860C RID: 34316 RVA: 0x00367150 File Offset: 0x00365350
	public bool ContainsWaypoint(Waypoint _wp)
	{
		return this.Collection.hashSet.Contains(_wp);
	}

	// Token: 0x0600860D RID: 34317 RVA: 0x00367164 File Offset: 0x00365364
	public bool ContainsLastKnownPositionWaypoint(int _entityId)
	{
		using (List<Waypoint>.Enumerator enumerator = this.Collection.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.lastKnownPositionEntityId == _entityId)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600860E RID: 34318 RVA: 0x003671C4 File Offset: 0x003653C4
	public Waypoint GetLastKnownPositionWaypoint(int _entityID)
	{
		foreach (Waypoint waypoint in this.Collection.list)
		{
			if (waypoint.lastKnownPositionEntityId == _entityID)
			{
				return waypoint;
			}
		}
		return null;
	}

	// Token: 0x0600860F RID: 34319 RVA: 0x00367228 File Offset: 0x00365428
	public void UpdateEntityVehicleWayPoint(EntityVehicle vehicle, bool unloaded = false)
	{
		if (!vehicle.LocalPlayerIsOwner())
		{
			return;
		}
		Waypoint waypoint = null;
		foreach (Waypoint waypoint2 in this.Collection.list)
		{
			if (waypoint2.lastKnownPositionEntityId == vehicle.entityId)
			{
				waypoint = waypoint2;
				break;
			}
		}
		if (waypoint != null)
		{
			Vector3i vector3i = Vector3i.FromVector3Rounded(vehicle.position);
			if (waypoint.pos != vector3i)
			{
				this.Collection.Remove(waypoint);
				waypoint.pos = vector3i;
				waypoint.navObject.TrackedPosition = vehicle.position;
				this.Collection.Add(waypoint);
			}
			waypoint.navObject.hiddenOnCompass = !unloaded;
			this.SetWaypointHiddenOnMap(vehicle.entityId, !unloaded);
			return;
		}
		((XUiC_MapArea)GameManager.Instance.World.GetPrimaryPlayer().PlayerUI.xui.GetWindow("mapArea").Controller).RefreshVehiclePositionWaypoint(vehicle, false);
	}

	// Token: 0x06008610 RID: 34320 RVA: 0x00367338 File Offset: 0x00365538
	public void SetEntityVehicleWaypointFromVehicleManager([TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})] List<ValueTuple<int, Vector3>> _positions)
	{
		List<Waypoint> list = new List<Waypoint>();
		for (int i = 0; i < this.Collection.list.Count; i++)
		{
			if (this.Collection.list[i].lastKnownPositionEntityId != -1 && this.Collection.list[i].lastKnownPositionEntityType == eLastKnownPositionEntityType.Vehicle)
			{
				Waypoint waypoint = this.Collection.list[i];
				bool flag = false;
				int j = 0;
				while (j < _positions.Count)
				{
					if (waypoint.lastKnownPositionEntityId == _positions[j].Item1)
					{
						flag = true;
						Vector3i vector3i = Vector3i.FromVector3Rounded(_positions[j].Item2);
						if (waypoint.pos != vector3i)
						{
							this.Collection.Remove(waypoint);
							waypoint.pos = vector3i;
							waypoint.navObject.TrackedPosition = _positions[j].Item2;
							this.Collection.Add(waypoint);
							break;
						}
						break;
					}
					else
					{
						j++;
					}
				}
				if (!flag)
				{
					list.Add(waypoint);
				}
			}
		}
		foreach (Waypoint waypoint2 in list)
		{
			this.Collection.Remove(waypoint2);
			NavObjectManager.Instance.UnRegisterNavObject(waypoint2.navObject);
		}
	}

	// Token: 0x06008611 RID: 34321 RVA: 0x003674A8 File Offset: 0x003656A8
	public void SetDroneWaypointsFromDroneManager([TupleElementNames(new string[]
	{
		"entityId",
		"position"
	})] List<ValueTuple<int, Vector3>> _drones)
	{
		for (int i = 0; i < _drones.Count; i++)
		{
			Waypoint waypoint = null;
			foreach (Waypoint waypoint2 in this.Collection.list)
			{
				if (waypoint2.lastKnownPositionEntityId == _drones[i].Item1)
				{
					waypoint = waypoint2;
					break;
				}
			}
			if (waypoint != null)
			{
				Vector3i vector3i = Vector3i.FromVector3Rounded(_drones[i].Item2);
				if (waypoint.pos != vector3i)
				{
					this.Collection.Remove(waypoint);
					waypoint.pos = vector3i;
					waypoint.navObject.TrackedPosition = _drones[i].Item2;
					this.Collection.Add(waypoint);
				}
			}
			else
			{
				((XUiC_MapArea)GameManager.Instance.World.GetPrimaryPlayer().PlayerUI.xui.GetWindow("mapArea").Controller).RefreshDronePositionWaypoint(_drones[i].Item1, Vector3i.FromVector3Rounded(_drones[i].Item2), true);
			}
		}
		List<Waypoint> list = new List<Waypoint>();
		for (int j = 0; j < this.Collection.list.Count; j++)
		{
			if (this.Collection.list[j].lastKnownPositionEntityId != -1 && this.Collection.list[j].lastKnownPositionEntityType == eLastKnownPositionEntityType.Drone)
			{
				Waypoint waypoint3 = this.Collection.list[j];
				bool flag = false;
				for (int k = 0; k < _drones.Count; k++)
				{
					if (waypoint3.lastKnownPositionEntityId == _drones[k].Item1)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(waypoint3);
				}
			}
		}
		foreach (Waypoint waypoint4 in list)
		{
			this.Collection.Remove(waypoint4);
			NavObjectManager.Instance.UnRegisterNavObject(waypoint4.navObject);
		}
	}

	// Token: 0x06008612 RID: 34322 RVA: 0x003676E8 File Offset: 0x003658E8
	public void UpdateEntityDroneWayPoint(EntityDrone drone, bool following, bool unloaded = false)
	{
		if (!drone.LocalPlayerIsOwner())
		{
			return;
		}
		Waypoint waypoint = null;
		foreach (Waypoint waypoint2 in this.Collection.list)
		{
			if (waypoint2.lastKnownPositionEntityId == drone.entityId)
			{
				waypoint = waypoint2;
				break;
			}
		}
		if (waypoint != null)
		{
			Vector3i vector3i = Vector3i.FromVector3Rounded(drone.position);
			if (waypoint.pos != vector3i)
			{
				this.Collection.Remove(waypoint);
				waypoint.pos = vector3i;
				waypoint.navObject.TrackedPosition = drone.position;
				this.Collection.Add(waypoint);
			}
			waypoint.navObject.hiddenOnCompass = !unloaded;
			this.SetWaypointHiddenOnMap(drone.entityId, !unloaded);
			return;
		}
		((XUiC_MapArea)GameManager.Instance.World.GetPrimaryPlayer().PlayerUI.xui.GetWindow("mapArea").Controller).RefreshDronePositionWaypoint(drone, false);
	}

	// Token: 0x06008613 RID: 34323 RVA: 0x003677F8 File Offset: 0x003659F8
	public void SetWaypointHiddenOnMap(int _entityId, bool _hidden)
	{
		foreach (Waypoint waypoint in this.Collection.list)
		{
			if (waypoint.lastKnownPositionEntityId == _entityId)
			{
				waypoint.HiddenOnMap = _hidden;
				waypoint.navObject.hiddenOnMap = _hidden;
				break;
			}
		}
	}

	// Token: 0x06008614 RID: 34324 RVA: 0x00367868 File Offset: 0x00365A68
	public bool TryRemoveLastKnownPositionWaypoint(int _entityId)
	{
		Waypoint lastKnownPositionWaypoint = this.GetLastKnownPositionWaypoint(_entityId);
		if (lastKnownPositionWaypoint != null)
		{
			this.Collection.Remove(lastKnownPositionWaypoint);
			NavObjectManager.Instance.UnRegisterNavObject(lastKnownPositionWaypoint.navObject);
			return true;
		}
		return false;
	}

	// Token: 0x06008615 RID: 34325 RVA: 0x003678A0 File Offset: 0x00365AA0
	public Waypoint GetWaypointForNavObject(NavObject nav)
	{
		foreach (Waypoint waypoint in this.Collection.list)
		{
			if (nav == waypoint.navObject)
			{
				return waypoint;
			}
		}
		return null;
	}

	// Token: 0x0400682C RID: 26668
	public const int cCurrentSaveVersion = 7;

	// Token: 0x0400682D RID: 26669
	public HashSetList<Waypoint> Collection = new HashSetList<Waypoint>();
}

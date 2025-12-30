using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200105D RID: 4189
public static class ProjectileManager
{
	// Token: 0x06008483 RID: 33923 RVA: 0x0035E3DE File Offset: 0x0035C5DE
	[PublicizedFrom(EAccessModifier.Private)]
	public static void init()
	{
		ProjectileManager.nextId = 0;
		ProjectileManager.projectiles = new DictionaryKeyValueList<int, Transform>();
		ProjectileManager.keysToRemove = new List<int>();
		ProjectileManager.blockHitParent = new GameObject("WorldProjectileContainer").transform;
		Origin.Add(ProjectileManager.blockHitParent, 0);
	}

	// Token: 0x06008484 RID: 33924 RVA: 0x0035E41C File Offset: 0x0035C61C
	public static void Update()
	{
		if (ProjectileManager.projectiles == null)
		{
			ProjectileManager.init();
		}
		for (int i = 0; i < ProjectileManager.projectiles.keyList.Count; i++)
		{
			int num = ProjectileManager.projectiles.keyList[i];
			Transform transform = ProjectileManager.projectiles.dict[num];
			if (transform == null)
			{
				ProjectileManager.keysToRemove.Add(num);
			}
			else if (transform.GetComponent<ProjectileMoveScript>() != null && !GameManager.Instance.World.IsChunkAreaLoaded(Mathf.CeilToInt(transform.position.x + Origin.position.x), Mathf.CeilToInt(transform.position.y + Origin.position.y), Mathf.CeilToInt(transform.position.z + Origin.position.z)))
			{
				ProjectileManager.keysToRemove.Add(num);
			}
		}
		for (int j = 0; j < ProjectileManager.keysToRemove.Count; j++)
		{
			ProjectileManager.RemoveProjectile(ProjectileManager.keysToRemove[j]);
		}
		ProjectileManager.keysToRemove.Clear();
	}

	// Token: 0x06008485 RID: 33925 RVA: 0x0035E538 File Offset: 0x0035C738
	public static void Cleanup()
	{
		ProjectileManager.keysToRemove = new List<int>();
		if (ProjectileManager.projectiles == null)
		{
			return;
		}
		for (int i = 0; i < ProjectileManager.projectiles.keyList.Count; i++)
		{
			ProjectileManager.keysToRemove.Add(ProjectileManager.projectiles.keyList[i]);
		}
		for (int j = 0; j < ProjectileManager.keysToRemove.Count; j++)
		{
			ProjectileManager.RemoveProjectile(ProjectileManager.keysToRemove[j]);
		}
	}

	// Token: 0x06008486 RID: 33926 RVA: 0x0035E5B0 File Offset: 0x0035C7B0
	public static int AddProjectileItem(Transform _transform = null, int _pId = -1, Vector3 _position = default(Vector3), Vector3 _movementLastFrame = default(Vector3), int _itemValueType = -1)
	{
		if (ProjectileManager.projectiles == null)
		{
			ProjectileManager.init();
		}
		ProjectileManager.Update();
		if (_pId == -1)
		{
			_pId = ProjectileManager.nextId;
			ProjectileManager.nextId++;
		}
		if (_transform == null && _itemValueType != -1)
		{
			_transform = ProjectileManager.instantiateProjectile(_pId, _itemValueType);
		}
		float num = ItemClass.GetForId(_itemValueType).StickyOffset;
		Renderer renderer = _transform.GetComponent<Renderer>();
		if (renderer == null)
		{
			LODGroup component = _transform.GetComponent<LODGroup>();
			if (component != null)
			{
				LOD[] lods = component.GetLODs();
				for (int i = 0; i < component.lodCount; i++)
				{
					Renderer[] renderers = lods[i].renderers;
					if (i == 0)
					{
						renderer = renderers[i];
					}
					for (int j = 0; j < renderers.Length; j++)
					{
						renderers[j].material = DataLoader.LoadAsset<Material>(ItemClass.GetForId(_itemValueType).StickyMaterial, false);
					}
				}
			}
			else
			{
				renderer = _transform.GetComponentInChildren<Renderer>();
			}
		}
		else if (ItemClass.GetForId(_itemValueType).StickyMaterial != null)
		{
			renderer.material = DataLoader.LoadAsset<Material>(ItemClass.GetForId(_itemValueType).StickyMaterial, false);
		}
		Mesh mesh = _transform.GetComponent<Mesh>();
		MeshFilter meshFilter = _transform.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = _transform.GetComponentInChildren<MeshFilter>();
		}
		if (mesh == null)
		{
			SkinnedMeshRenderer componentInChildren = _transform.GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren != null)
			{
				mesh = componentInChildren.sharedMesh;
				renderer = componentInChildren;
			}
		}
		if (mesh == null && meshFilter != null)
		{
			mesh = meshFilter.mesh;
		}
		if (renderer != null || (meshFilter != null && meshFilter.mesh != null))
		{
			Bounds bounds = renderer.bounds;
			if (meshFilter != null && meshFilter.mesh != null)
			{
				bounds = meshFilter.mesh.bounds;
			}
			if (mesh != null)
			{
				bounds = mesh.bounds;
			}
			if (!Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
			{
				CapsuleCollider capsuleCollider = _transform.gameObject.AddComponent<CapsuleCollider>();
				capsuleCollider.center = bounds.center;
				float num2 = Mathf.Max(Mathf.Max(bounds.size.x, bounds.size.y), bounds.size.z);
				if (ItemClass.GetForId(_itemValueType).StickyColliderUp == -1)
				{
					if (num2 == bounds.size.x)
					{
						capsuleCollider.direction = 2;
					}
					else if (num2 == bounds.size.y)
					{
						capsuleCollider.direction = 1;
					}
					else if (num2 == bounds.size.z)
					{
						capsuleCollider.direction = 2;
					}
				}
				else
				{
					capsuleCollider.direction = ItemClass.GetForId(_itemValueType).StickyColliderUp;
				}
				capsuleCollider.height = ((ItemClass.GetForId(_itemValueType).StickyColliderLength > 0f) ? ItemClass.GetForId(_itemValueType).StickyColliderLength : num2);
				capsuleCollider.radius = ((ItemClass.GetForId(_itemValueType).StickyColliderRadius > 0f) ? ItemClass.GetForId(_itemValueType).StickyColliderRadius : 0.05f);
				if (num == 0f)
				{
					num = capsuleCollider.height / 2f;
				}
			}
		}
		if (Voxel.voxelRayHitInfo.tag.StartsWith("E_"))
		{
			_transform.parent = Voxel.voxelRayHitInfo.transform;
		}
		else
		{
			_transform.parent = ProjectileManager.blockHitParent;
		}
		_transform.localScale = Vector3.one * 0.75f;
		ProjectileMoveScript component2;
		ThrownWeaponMoveScript component3;
		if ((component2 = _transform.GetComponent<ProjectileMoveScript>()) != null)
		{
			component2.ProjectileID = _pId;
			_transform.name = string.Format("temp_Projectile[item:{0},owner:{1},projectileId:{2}", component2.itemProjectile.GetLocalizedItemName(), GameManager.Instance.World.GetEntity(component2.ProjectileOwnerID).name, component2.ProjectileID.ToString());
			component2.FinalPosition = _position - _transform.forward * num;
			_transform.position = component2.FinalPosition - Origin.position;
		}
		else if ((component3 = _transform.GetComponent<ThrownWeaponMoveScript>()) != null)
		{
			component3.ProjectileID = _pId;
			_transform.name = string.Format("temp_Projectile[item:{0},owner:{1},projectileId:{2}", component3.itemWeapon.GetLocalizedItemName(), GameManager.Instance.World.GetEntity(component3.ProjectileOwnerID).name, component3.ProjectileID.ToString());
			component3.FinalPosition = _position - component3.velocity.normalized * num;
			_transform.position = component3.FinalPosition - Origin.position;
		}
		Transform transform = _transform.Find("Trail");
		if (transform != null)
		{
			transform.gameObject.SetActive(false);
		}
		_transform.tag = "Item";
		_transform.gameObject.layer = 13;
		ProjectileManager.projectiles.Add(_pId, _transform);
		return _pId;
	}

	// Token: 0x06008487 RID: 33927 RVA: 0x0035EA84 File Offset: 0x0035CC84
	public static void RemoveProjectile(int _id)
	{
		if (ProjectileManager.projectiles == null)
		{
			ProjectileManager.init();
			return;
		}
		if (ProjectileManager.projectiles.keyList.Contains(_id) && ProjectileManager.projectiles.dict[_id] != null)
		{
			UnityEngine.Object.Destroy(ProjectileManager.projectiles.dict[_id].gameObject);
			ProjectileManager.projectiles.Remove(_id);
		}
	}

	// Token: 0x06008488 RID: 33928 RVA: 0x0035EAF0 File Offset: 0x0035CCF0
	public static Transform GetProjectile(int _id)
	{
		Transform result = null;
		if (ProjectileManager.projectiles.keyList.Contains(_id) && ProjectileManager.projectiles.dict[_id] != null)
		{
			result = ProjectileManager.projectiles.dict[_id];
		}
		return result;
	}

	// Token: 0x06008489 RID: 33929 RVA: 0x0035EB3C File Offset: 0x0035CD3C
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform instantiateProjectile(int _id, int _itemValueType)
	{
		ItemClass forId = ItemClass.GetForId(_itemValueType);
		if (forId == null)
		{
			return null;
		}
		ItemValue itemValue = new ItemValue(forId.Id, false);
		Transform transform = forId.CloneModel(GameManager.Instance.World, itemValue, Vector3.zero, null, BlockShape.MeshPurpose.World, default(TextureFullArray));
		Utils.SetLayerRecursively(transform.gameObject, 13);
		transform.gameObject.SetActive(true);
		return transform;
	}

	// Token: 0x04006691 RID: 26257
	[PublicizedFrom(EAccessModifier.Private)]
	public static int nextId;

	// Token: 0x04006692 RID: 26258
	[PublicizedFrom(EAccessModifier.Private)]
	public static DictionaryKeyValueList<int, Transform> projectiles;

	// Token: 0x04006693 RID: 26259
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<int> keysToRemove;

	// Token: 0x04006694 RID: 26260
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform blockHitParent;
}

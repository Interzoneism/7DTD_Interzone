using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200132C RID: 4908
[Serializable]
public class vp_SpawnPoint : MonoBehaviour
{
	// Token: 0x17000F95 RID: 3989
	// (get) Token: 0x06009896 RID: 39062 RVA: 0x003CAD58 File Offset: 0x003C8F58
	public static List<vp_SpawnPoint> SpawnPoints
	{
		get
		{
			if (vp_SpawnPoint.m_SpawnPoints == null)
			{
				vp_SpawnPoint.m_SpawnPoints = new List<vp_SpawnPoint>(UnityEngine.Object.FindObjectsOfType(typeof(vp_SpawnPoint)) as vp_SpawnPoint[]);
			}
			return vp_SpawnPoint.m_SpawnPoints;
		}
	}

	// Token: 0x06009897 RID: 39063 RVA: 0x003CAD84 File Offset: 0x003C8F84
	public static vp_Placement GetRandomPlacement()
	{
		return vp_SpawnPoint.GetRandomPlacement(0f, null);
	}

	// Token: 0x06009898 RID: 39064 RVA: 0x003CAD91 File Offset: 0x003C8F91
	public static vp_Placement GetRandomPlacement(float physicsCheckRadius)
	{
		return vp_SpawnPoint.GetRandomPlacement(physicsCheckRadius, null);
	}

	// Token: 0x06009899 RID: 39065 RVA: 0x003CAD9A File Offset: 0x003C8F9A
	public static vp_Placement GetRandomPlacement(string tag)
	{
		return vp_SpawnPoint.GetRandomPlacement(0f, tag);
	}

	// Token: 0x0600989A RID: 39066 RVA: 0x003CADA8 File Offset: 0x003C8FA8
	public static vp_Placement GetRandomPlacement(float physicsCheckRadius, string tag)
	{
		if (vp_SpawnPoint.SpawnPoints == null || vp_SpawnPoint.SpawnPoints.Count < 1)
		{
			return null;
		}
		vp_SpawnPoint randomSpawnPoint;
		if (string.IsNullOrEmpty(tag))
		{
			randomSpawnPoint = vp_SpawnPoint.GetRandomSpawnPoint();
		}
		else
		{
			randomSpawnPoint = vp_SpawnPoint.GetRandomSpawnPoint(tag);
			if (randomSpawnPoint == null)
			{
				randomSpawnPoint = vp_SpawnPoint.GetRandomSpawnPoint();
				Debug.LogWarning("Warning (vp_SpawnPoint --> GetRandomPlacement) Could not find a spawnpoint tagged '" + tag + "'. Falling back to 'any random spawnpoint'.");
			}
		}
		if (randomSpawnPoint == null)
		{
			Debug.LogError("Error (vp_SpawnPoint --> GetRandomPlacement) Could not find a spawnpoint" + ((!string.IsNullOrEmpty(tag)) ? (" tagged '" + tag + "'") : ".") + " Reverting to world origin.");
			return null;
		}
		vp_Placement vp_Placement = new vp_Placement();
		vp_Placement.Position = randomSpawnPoint.transform.position;
		if (randomSpawnPoint.Radius > 0f)
		{
			Vector3 vector = UnityEngine.Random.insideUnitSphere * randomSpawnPoint.Radius;
			vp_Placement vp_Placement2 = vp_Placement;
			vp_Placement2.Position.x = vp_Placement2.Position.x + vector.x;
			vp_Placement vp_Placement3 = vp_Placement;
			vp_Placement3.Position.z = vp_Placement3.Position.z + vector.z;
		}
		if (physicsCheckRadius != 0f)
		{
			if (!vp_Placement.AdjustPosition(vp_Placement, physicsCheckRadius, 1000))
			{
				return null;
			}
			vp_Placement.SnapToGround(vp_Placement, physicsCheckRadius, randomSpawnPoint.GroundSnapThreshold);
		}
		if (randomSpawnPoint.RandomDirection)
		{
			vp_Placement.Rotation = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f));
		}
		else
		{
			vp_Placement.Rotation = randomSpawnPoint.transform.rotation;
		}
		return vp_Placement;
	}

	// Token: 0x0600989B RID: 39067 RVA: 0x003CAF0A File Offset: 0x003C910A
	public static vp_SpawnPoint GetRandomSpawnPoint()
	{
		if (vp_SpawnPoint.SpawnPoints.Count < 1)
		{
			return null;
		}
		return vp_SpawnPoint.SpawnPoints[UnityEngine.Random.Range(0, vp_SpawnPoint.SpawnPoints.Count)];
	}

	// Token: 0x0600989C RID: 39068 RVA: 0x003CAF38 File Offset: 0x003C9138
	public static vp_SpawnPoint GetRandomSpawnPoint(string tag)
	{
		vp_SpawnPoint.m_MatchingSpawnPoints.Clear();
		for (int i = 0; i < vp_SpawnPoint.SpawnPoints.Count; i++)
		{
			if (vp_SpawnPoint.m_SpawnPoints[i].tag == tag)
			{
				vp_SpawnPoint.m_MatchingSpawnPoints.Add(vp_SpawnPoint.m_SpawnPoints[i]);
			}
		}
		if (vp_SpawnPoint.m_MatchingSpawnPoints.Count < 1)
		{
			return null;
		}
		if (vp_SpawnPoint.m_MatchingSpawnPoints.Count == 1)
		{
			return vp_SpawnPoint.m_MatchingSpawnPoints[0];
		}
		return vp_SpawnPoint.m_MatchingSpawnPoints[UnityEngine.Random.Range(0, vp_SpawnPoint.m_MatchingSpawnPoints.Count)];
	}

	// Token: 0x0600989D RID: 39069 RVA: 0x003CAFD3 File Offset: 0x003C91D3
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		SceneManager.sceneLoaded += this.NotifyLevelWasLoaded;
	}

	// Token: 0x0600989E RID: 39070 RVA: 0x003CAFE7 File Offset: 0x003C91E7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		SceneManager.sceneLoaded -= this.NotifyLevelWasLoaded;
	}

	// Token: 0x0600989F RID: 39071 RVA: 0x003CAFFB File Offset: 0x003C91FB
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void NotifyLevelWasLoaded(Scene scene, LoadSceneMode mode)
	{
		vp_SpawnPoint.m_SpawnPoints = null;
	}

	// Token: 0x0400751D RID: 29981
	public bool RandomDirection;

	// Token: 0x0400751E RID: 29982
	public float Radius;

	// Token: 0x0400751F RID: 29983
	public float GroundSnapThreshold = 2.5f;

	// Token: 0x04007520 RID: 29984
	public bool LockGroundSnapToRadius = true;

	// Token: 0x04007521 RID: 29985
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static List<vp_SpawnPoint> m_MatchingSpawnPoints = new List<vp_SpawnPoint>(50);

	// Token: 0x04007522 RID: 29986
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static List<vp_SpawnPoint> m_SpawnPoints = null;
}

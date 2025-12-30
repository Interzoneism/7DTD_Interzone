using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001110 RID: 4368
public class WireNode : MonoBehaviour, IWireNode
{
	// Token: 0x0600894C RID: 35148 RVA: 0x0037A320 File Offset: 0x00378520
	public void Init()
	{
		if (WireNode.rootShockPrefab == null)
		{
			WireNode.rootShockPrefab = Resources.Load<GameObject>("Prefabs/ElectricShock");
		}
		this.prevParent = (this.parent = base.transform.parent);
		if (this.parent != null)
		{
			this.prevParentGO = (this.parentGO = this.parent.gameObject);
			this.usingLocalPosition = false;
		}
		this.shockSpots = new List<WireNode.ShockSpot>();
		base.transform.parent = null;
		base.transform.position = Vector3.zero;
		base.transform.localEulerAngles = Vector3.zero;
		base.transform.localScale = Vector3.one;
		this.meshFilter = base.GetComponent<MeshFilter>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.meshCollider = base.GetComponent<MeshCollider>();
		this.meshCollider.convex = true;
		this.meshCollider.isTrigger = true;
		this.mesh = new Mesh();
		this.mesh.MarkDynamic();
		this.shockNodes = new List<bool>();
		this.points = new List<Vector3>();
		this.forces = new List<Vector3>();
		this.verts = new List<Vector3>();
		this.uvs = new List<Vector2>();
		this.uvs2 = new List<Vector2>();
		this.normals = new List<Vector3>();
		this.indices = new int[24 * (1 + this.numNodes) + 12];
		if (this.personalMaterial == null)
		{
			this.personalMaterial = UnityEngine.Object.Instantiate<Material>(this.material);
		}
		this.meshRenderer.material = this.personalMaterial;
	}

	// Token: 0x0600894D RID: 35149 RVA: 0x0037A4C4 File Offset: 0x003786C4
	public void OnDestroy()
	{
		if (this.personalMaterial != null)
		{
			UnityEngine.Object.Destroy(this.personalMaterial);
			this.personalMaterial = null;
		}
		for (int i = 0; i < this.shockSpots.Count; i++)
		{
			UnityEngine.Object.Destroy(this.shockSpots[i].shockPrefab);
		}
	}

	// Token: 0x0600894E RID: 35150 RVA: 0x0037A51D File Offset: 0x0037871D
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.Init();
	}

	// Token: 0x0600894F RID: 35151 RVA: 0x0037A525 File Offset: 0x00378725
	public void ToggleMeshCollider(bool _bOn)
	{
		this.meshCollider.enabled = _bOn;
	}

	// Token: 0x06008950 RID: 35152 RVA: 0x0037A533 File Offset: 0x00378733
	public void SetPulseSpeed(float speed)
	{
		if (this.personalMaterial == null)
		{
			return;
		}
		this.personalMaterial.SetFloat("_PulseSpeed", speed);
	}

	// Token: 0x06008951 RID: 35153 RVA: 0x0037A555 File Offset: 0x00378755
	public void TogglePulse(bool isOn)
	{
		if (this.personalMaterial == null)
		{
			return;
		}
		this.personalMaterial.SetColor("_PulseColor", isOn ? this.pulseColor : Color.black);
	}

	// Token: 0x06008952 RID: 35154 RVA: 0x0037A586 File Offset: 0x00378786
	public void SetPulseColor(Color color)
	{
		this.pulseColor = color;
	}

	// Token: 0x06008953 RID: 35155 RVA: 0x0037A590 File Offset: 0x00378790
	public void PlayShockAtPosition(Vector3 shockPosition)
	{
		if (shockPosition.x < this.min.x - 1f)
		{
			return;
		}
		if (shockPosition.y < this.min.y - 1f)
		{
			return;
		}
		if (shockPosition.z < this.min.z - 1f)
		{
			return;
		}
		if (shockPosition.x > this.max.x + 1f)
		{
			return;
		}
		if (shockPosition.y > this.max.y + 1f)
		{
			return;
		}
		if (shockPosition.z > this.max.z + 1f)
		{
			return;
		}
		int num = 99999;
		float num2 = 99999f;
		for (int i = 1; i < this.points.Count - 1; i++)
		{
			Vector3 vector = this.points[i] - shockPosition;
			if (vector.magnitude < num2)
			{
				num2 = vector.magnitude;
				num = i;
			}
		}
		if (num2 > 1f)
		{
			return;
		}
		if (num < this.shockNodes.Count && this.shockNodes[num])
		{
			return;
		}
		WireNode.ShockSpot shockSpot;
		shockSpot.timer = Time.time;
		shockSpot.vertex = num;
		shockSpot.shockDataIndex = 0;
		shockSpot.shockPrefab = UnityEngine.Object.Instantiate<GameObject>(WireNode.rootShockPrefab);
		shockSpot.shockPrefab.transform.position = this.points[num];
		shockSpot.shockPrefab.transform.parent = base.transform;
		this.shockSpots.Add(shockSpot);
	}

	// Token: 0x06008954 RID: 35156 RVA: 0x0037A718 File Offset: 0x00378918
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdatePlayerIntersection()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (GameManager.Instance.World == null)
		{
			return;
		}
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer == null)
		{
			return;
		}
		if (Time.time > this.playerShockTimer + 0.25f)
		{
			this.playerShockTimer = Time.time;
			this.PlayShockAtPosition(primaryPlayer.GetPosition());
		}
	}

	// Token: 0x06008955 RID: 35157 RVA: 0x0037A784 File Offset: 0x00378984
	[PublicizedFrom(EAccessModifier.Private)]
	public void RayCastHeightAt(ref Vector3 position)
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(position + Vector3.up - Origin.position, Vector3.down), out raycastHit, 1.75f, 65537) && raycastHit.point.y > position.y)
		{
			position.y = raycastHit.point.y + 0.01f - Origin.position.y;
		}
	}

	// Token: 0x06008956 RID: 35158 RVA: 0x0037A800 File Offset: 0x00378A00
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateForces()
	{
		if (this.points.Count < 3)
		{
			return;
		}
		float magnitude = (this.points[this.points.Count - 1] - this.points[0]).magnitude;
		if (magnitude == 0f)
		{
			return;
		}
		float num = magnitude / 10000f;
		num = Mathf.Clamp(num, 1E-05f, 10f);
		float d = (this.weightMod > 0f) ? (num * this.weightMod) : num;
		Vector3 a = Vector3.down * 9.81f * d;
		float num2 = 0.35f * ((this.weightMod > 0f) ? (1f - this.weightMod * num) : (1f - num)) * this.springMod;
		num2 = Mathf.Clamp(num2, 0.001f, 0.4999f);
		for (int i = 1; i < this.points.Count - 1; i++)
		{
			Vector3 a2 = this.points[i - 1] - this.points[i];
			a2 *= this.tensionMultiplier;
			a2 *= Mathf.Clamp01(a2.magnitude - this.minSegmentLength);
			Vector3 vector = this.points[i + 1] - this.points[i];
			vector *= this.tensionMultiplier;
			vector *= Mathf.Clamp01(vector.magnitude - this.minSegmentLength);
			Vector3 a3 = a2 + vector;
			float num3 = (i >= this.points.Count - 4) ? Mathf.Clamp01(1.1f - (float)(i - (this.points.Count - 5)) / 3f) : 1f;
			float d2 = num2 * num3;
			Vector3 b = a * (1f + (1f - num3));
			this.forces[i] = this.forces[i] * this.drag + (a3 + b) * d2;
		}
		this.currentTotalForces = 0f;
		for (int j = 1; j < this.points.Count - 1; j++)
		{
			RaycastHit raycastHit;
			if (this.forces[j].magnitude > 0f && this.forces[j].magnitude < this.snagThreshold && Physics.Raycast(new Ray(this.points[j] - Origin.position, this.forces[j].normalized), out raycastHit, this.forces[j].magnitude, 65537) && raycastHit.distance < this.forces[j].magnitude)
			{
				this.forces[j] = Vector3.zero;
			}
			List<Vector3> list = this.points;
			int index = j;
			list[index] += this.forces[j];
			this.currentTotalForces += this.forces[j].magnitude;
		}
	}

	// Token: 0x06008957 RID: 35159 RVA: 0x0037AB8C File Offset: 0x00378D8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreatePoints()
	{
		Vector3 a = this.currSourcePosition - this.currLocalPosition;
		if (a.magnitude == 0f)
		{
			return;
		}
		Vector3 b = a / (float)(1 + this.numNodes);
		if (b.magnitude == 0f)
		{
			return;
		}
		Vector3 vector = this.currLocalPosition + b;
		if (this.points.Count == 0)
		{
			Dictionary<Vector3, bool> dictionary = new Dictionary<Vector3, bool>(Vector3EqualityComparer.Instance);
			dictionary.Add(this.currLocalPosition, true);
			this.points.Add(this.currLocalPosition);
			this.forces.Add(Vector3.zero);
			this.shockNodes.Add(false);
			for (int i = 0; i < this.numNodes; i++)
			{
				this.forces.Add(Vector3.zero);
				this.shockNodes.Add(false);
				if (dictionary.ContainsKey(vector))
				{
					this.points.Clear();
					this.forces.Clear();
					this.shockNodes.Clear();
					return;
				}
				this.points.Add(vector);
				dictionary.Add(vector, true);
				vector += b;
			}
		}
		if (this.points.Count > 1)
		{
			this.points[0] = this.currLocalPosition;
			this.points[this.points.Count - 1] = this.currSourcePosition;
		}
	}

	// Token: 0x06008958 RID: 35160 RVA: 0x0037ACF4 File Offset: 0x00378EF4
	public void BuildMesh()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		this.usingSourcePosition = (this.Source == null);
		if (!this.usingLocalPosition && this.parentGO == null)
		{
			this.usingLocalPosition = true;
			return;
		}
		Vector3 b = this.currLocalPosition;
		Vector3 a = this.currSourcePosition;
		if (this.prevWireColor != this.wireColor)
		{
			this.prevWireColor = this.wireColor;
			this.personalMaterial.SetColor("_WireColor", this.wireColor);
		}
		for (int i = this.shockSpots.Count - 1; i >= 0; i--)
		{
			if (Time.time > this.shockSpots[i].timer + this.shocks[this.shockSpots[i].shockDataIndex].x)
			{
				WireNode.ShockSpot shockSpot = this.shockSpots[i];
				shockSpot.timer = Time.time;
				shockSpot.shockDataIndex++;
				if (shockSpot.shockDataIndex >= this.shocks.Length)
				{
					UnityEngine.Object.Destroy(shockSpot.shockPrefab);
					this.shockSpots.RemoveAt(i);
					this.shockNodes[i] = false;
				}
				else
				{
					this.shockSpots[i] = shockSpot;
				}
			}
		}
		this.currLocalPosition = this.localOffset + (this.usingLocalPosition ? this.LocalPosition : this.parent.position);
		Vector3 a2 = this.sourceOffset;
		if (!this.usingSourcePosition)
		{
			a2 = this.sourceOffset.x * Camera.main.transform.right;
			a2 += this.sourceOffset.y * Camera.main.transform.up;
			a2 += this.sourceOffset.z * Camera.main.transform.forward;
		}
		this.currSourcePosition = a2 + (this.usingSourcePosition ? this.SourcePosition : this.cachedSourcePosition);
		if (this.currSourcePosition == this.currLocalPosition)
		{
			return;
		}
		this.prevLocalPosition = this.currLocalPosition;
		this.prevSourcePosition = this.currSourcePosition;
		this.prevWeightMod = this.weightMod;
		this.prevSpringMod = this.springMod;
		this.prevSize = this.size;
		this.mesh.Clear(false);
		this.verts.Clear();
		this.uvs.Clear();
		this.uvs2.Clear();
		this.normals.Clear();
		this.CreatePoints();
		this.UpdateForces();
		this.uvs.Add(Vector2.zero);
		this.uvs.Add(Vector2.zero);
		this.uvs.Add(Vector2.zero);
		this.uvs.Add(Vector2.zero);
		this.uvs2.Add(Vector2.zero);
		this.uvs2.Add(Vector2.zero);
		this.uvs2.Add(Vector2.zero);
		this.uvs2.Add(Vector2.zero);
		for (int j = 1; j < this.points.Count; j++)
		{
			this.uvs.Add(Vector2.right * (float)j / (float)this.points.Count);
			this.uvs.Add(Vector2.right * (float)j / (float)this.points.Count + Vector2.up);
			this.uvs.Add(Vector2.right * (float)j / (float)this.points.Count);
			this.uvs.Add(Vector2.right * (float)j / (float)this.points.Count + Vector2.up);
			bool flag = false;
			for (int k = 0; k < this.shockSpots.Count; k++)
			{
				if (this.shockSpots[k].vertex == j)
				{
					this.uvs2.Add(Vector2.right * this.shocks[this.shockSpots[k].shockDataIndex].y);
					this.uvs2.Add(Vector2.right * this.shocks[this.shockSpots[k].shockDataIndex].y);
					this.uvs2.Add(Vector2.right * this.shocks[this.shockSpots[k].shockDataIndex].y);
					this.uvs2.Add(Vector2.right * this.shocks[this.shockSpots[k].shockDataIndex].y);
					flag = true;
					this.shockNodes[j] = true;
					break;
				}
			}
			if (!flag)
			{
				this.uvs2.Add(Vector2.zero);
				this.uvs2.Add(Vector2.zero);
				this.uvs2.Add(Vector2.zero);
				this.uvs2.Add(Vector2.zero);
			}
		}
		Vector3 vector = a - b;
		this.min = Vector3.one * float.PositiveInfinity;
		this.max = -Vector3.one * float.PositiveInfinity;
		for (int l = 0; l < this.points.Count; l++)
		{
			if (l > 0)
			{
				vector = this.points[l] - this.points[l - 1];
			}
			Vector3 normalized = Vector3.Cross(Vector3.up, vector.normalized).normalized;
			Vector3 normalized2 = Vector3.Cross(vector.normalized, normalized).normalized;
			this.verts.Add(normalized2 * this.size + this.points[l]);
			this.verts.Add(-normalized2 * this.size + this.points[l]);
			this.verts.Add(normalized * this.size + this.points[l]);
			this.verts.Add(-normalized * this.size + this.points[l]);
			if (l == 0)
			{
				normalized2 = Vector3.Lerp(normalized2, (a - b).normalized, 0.5f).normalized;
				normalized = Vector3.Lerp(normalized, (a - b).normalized, 0.5f).normalized;
			}
			else if (l == this.points.Count - 1)
			{
				normalized2 = Vector3.Lerp(normalized2, -(a - b).normalized, 0.5f).normalized;
				normalized = Vector3.Lerp(normalized, -(a - b).normalized, 0.5f).normalized;
			}
			this.normals.Add(normalized2);
			this.normals.Add(-normalized2);
			this.normals.Add(normalized);
			this.normals.Add(-normalized);
			if (this.points[l].x < this.min.x)
			{
				this.min.x = this.points[l].x;
			}
			if (this.points[l].x > this.max.x)
			{
				this.max.x = this.points[l].x;
			}
			if (this.points[l].y < this.min.y)
			{
				this.min.y = this.points[l].y;
			}
			if (this.points[l].y > this.max.y)
			{
				this.max.y = this.points[l].y;
			}
			if (this.points[l].z < this.min.z)
			{
				this.min.z = this.points[l].z;
			}
			if (this.points[l].z > this.max.z)
			{
				this.max.z = this.points[l].z;
			}
		}
		if (((this.min.x == this.max.x) ? 1 : ((0f + this.min.y == this.max.y) ? 1 : ((0f + this.min.z == this.max.z) ? 1 : 0))) >= 2)
		{
			return;
		}
		int num = 0;
		for (int m = 0; m < this.numNodes; m++)
		{
			this.indices[num++] = 4 * m;
			this.indices[num++] = 4 + 4 * m;
			this.indices[num++] = 7 + 4 * m;
			this.indices[num++] = 7 + 4 * m;
			this.indices[num++] = 3 + 4 * m;
			this.indices[num++] = 4 * m;
			this.indices[num++] = 4 + 4 * m;
			this.indices[num++] = 4 * m;
			this.indices[num++] = 2 + 4 * m;
			this.indices[num++] = 2 + 4 * m;
			this.indices[num++] = 6 + 4 * m;
			this.indices[num++] = 4 + 4 * m;
			this.indices[num++] = 3 + 4 * m;
			this.indices[num++] = 7 + 4 * m;
			this.indices[num++] = 5 + 4 * m;
			this.indices[num++] = 5 + 4 * m;
			this.indices[num++] = 1 + 4 * m;
			this.indices[num++] = 3 + 4 * m;
			this.indices[num++] = 6 + 4 * m;
			this.indices[num++] = 2 + 4 * m;
			this.indices[num++] = 1 + 4 * m;
			this.indices[num++] = 1 + 4 * m;
			this.indices[num++] = 5 + 4 * m;
			this.indices[num++] = 6 + 4 * m;
		}
		this.indices[num++] = 0;
		this.indices[num++] = 3;
		this.indices[num++] = 1;
		this.indices[num++] = 1;
		this.indices[num++] = 2;
		this.indices[num++] = 0;
		this.indices[num++] = 4 + 4 * (this.numNodes - 1);
		this.indices[num++] = 6 + 4 * (this.numNodes - 1);
		this.indices[num++] = 5 + 4 * (this.numNodes - 1);
		this.indices[num++] = 5 + 4 * (this.numNodes - 1);
		this.indices[num++] = 7 + 4 * (this.numNodes - 1);
		this.indices[num++] = 4 + 4 * (this.numNodes - 1);
		if (this.verts.Count < 3)
		{
			return;
		}
		if (this.uvs.Count < 3)
		{
			return;
		}
		if (this.uvs2.Count < 3)
		{
			return;
		}
		if (this.normals.Count < 3)
		{
			return;
		}
		if (this.indices.Length < 3)
		{
			return;
		}
		this.mesh.SetVertices(this.verts);
		this.mesh.uv = this.uvs.ToArray();
		this.mesh.uv2 = this.uvs2.ToArray();
		this.mesh.SetNormals(this.normals);
		this.mesh.SetIndices(this.indices, MeshTopology.Triangles, 0);
		this.meshFilter.mesh = this.mesh;
	}

	// Token: 0x06008959 RID: 35161 RVA: 0x00002914 File Offset: 0x00000B14
	public void Update()
	{
	}

	// Token: 0x0600895A RID: 35162 RVA: 0x0037BA40 File Offset: 0x00379C40
	[PublicizedFrom(EAccessModifier.Private)]
	public void MyUpdate()
	{
		if (this.Source != null)
		{
			if (this.cachedSourcePosition != this.Source.transform.position)
			{
				this.cachedSourcePosition = this.Source.transform.position;
				this.BuildMesh();
				return;
			}
		}
		else
		{
			this.BuildMesh();
		}
	}

	// Token: 0x0600895B RID: 35163 RVA: 0x0037BA9B File Offset: 0x00379C9B
	public void FixedUpdate()
	{
		this.MyUpdate();
	}

	// Token: 0x0600895C RID: 35164 RVA: 0x0037BA9B File Offset: 0x00379C9B
	public void LateUpdate()
	{
		this.MyUpdate();
	}

	// Token: 0x0600895D RID: 35165 RVA: 0x0037BAA3 File Offset: 0x00379CA3
	public Vector3 GetStartPosition()
	{
		return this.SourcePosition;
	}

	// Token: 0x0600895E RID: 35166 RVA: 0x0037BAAB File Offset: 0x00379CAB
	public Vector3 GetStartPositionOffset()
	{
		return this.sourceOffset;
	}

	// Token: 0x0600895F RID: 35167 RVA: 0x0037BAB3 File Offset: 0x00379CB3
	public void SetStartPosition(Vector3 pos)
	{
		this.SourcePosition = pos;
	}

	// Token: 0x06008960 RID: 35168 RVA: 0x0037BABC File Offset: 0x00379CBC
	public void SetStartPositionOffset(Vector3 pos)
	{
		this.sourceOffset = pos;
	}

	// Token: 0x06008961 RID: 35169 RVA: 0x0037BAC5 File Offset: 0x00379CC5
	public Vector3 GetEndPosition()
	{
		return this.LocalPosition;
	}

	// Token: 0x06008962 RID: 35170 RVA: 0x0037BACD File Offset: 0x00379CCD
	public Vector3 GetEndPositionOffset()
	{
		return this.localOffset;
	}

	// Token: 0x06008963 RID: 35171 RVA: 0x0037BAD5 File Offset: 0x00379CD5
	public void SetEndPosition(Vector3 pos)
	{
		this.LocalPosition = pos;
	}

	// Token: 0x06008964 RID: 35172 RVA: 0x0037BADE File Offset: 0x00379CDE
	public void SetEndPositionOffset(Vector3 pos)
	{
		this.localOffset = pos;
	}

	// Token: 0x06008965 RID: 35173 RVA: 0x0036C8E9 File Offset: 0x0036AAE9
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06008966 RID: 35174 RVA: 0x0037BAE7 File Offset: 0x00379CE7
	public Bounds GetBounds()
	{
		return this.mesh.bounds;
	}

	// Token: 0x06008967 RID: 35175 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetWireDip(float _dist)
	{
	}

	// Token: 0x06008968 RID: 35176 RVA: 0x0002E7B0 File Offset: 0x0002C9B0
	public float GetWireDip()
	{
		return 0f;
	}

	// Token: 0x06008969 RID: 35177 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetWireRadius(float _radius)
	{
	}

	// Token: 0x0600896A RID: 35178 RVA: 0x0037BAF4 File Offset: 0x00379CF4
	public void Reset()
	{
		this.pulseColor = new Color32(0, 97, byte.MaxValue, byte.MaxValue);
	}

	// Token: 0x0600896B RID: 35179 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetWireCanHide(bool _canHide)
	{
	}

	// Token: 0x0600896C RID: 35180 RVA: 0x00002914 File Offset: 0x00000B14
	public void SetVisible(bool _visible)
	{
	}

	// Token: 0x04006B81 RID: 27521
	public const int cLayerMaskRayCast = 65537;

	// Token: 0x04006B82 RID: 27522
	public Vector2[] shocks;

	// Token: 0x04006B83 RID: 27523
	public Material material;

	// Token: 0x04006B84 RID: 27524
	public GameObject Source;

	// Token: 0x04006B85 RID: 27525
	public GameObject parentGO;

	// Token: 0x04006B86 RID: 27526
	public Color pulseColor = new Color32(0, 97, byte.MaxValue, byte.MaxValue);

	// Token: 0x04006B87 RID: 27527
	public Color wireColor = Color.black;

	// Token: 0x04006B88 RID: 27528
	public float size;

	// Token: 0x04006B89 RID: 27529
	public float weightMod;

	// Token: 0x04006B8A RID: 27530
	public float springMod;

	// Token: 0x04006B8B RID: 27531
	public float minSegmentLength = 0.25f;

	// Token: 0x04006B8C RID: 27532
	public float snagThreshold = 0.5f;

	// Token: 0x04006B8D RID: 27533
	public float tensionMultiplier = 2f;

	// Token: 0x04006B8E RID: 27534
	public float playerHeight = 1.8f;

	// Token: 0x04006B8F RID: 27535
	public Vector3 SourcePosition = Vector3.zero;

	// Token: 0x04006B90 RID: 27536
	public Vector3 LocalPosition = Vector3.one;

	// Token: 0x04006B91 RID: 27537
	public Vector3 localOffset = Vector3.zero;

	// Token: 0x04006B92 RID: 27538
	public Vector3 sourceOffset = Vector3.zero;

	// Token: 0x04006B93 RID: 27539
	public Vector3 cameraOffset;

	// Token: 0x04006B94 RID: 27540
	public bool attatchSoureToCamera;

	// Token: 0x04006B95 RID: 27541
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float playerShockTimer;

	// Token: 0x04006B96 RID: 27542
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentTotalForces = 1000f;

	// Token: 0x04006B97 RID: 27543
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 prevSourcePos;

	// Token: 0x04006B98 RID: 27544
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float currentSegmentLength;

	// Token: 0x04006B99 RID: 27545
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameObject rootShockPrefab;

	// Token: 0x04006B9A RID: 27546
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] shockTimers = new float[8];

	// Token: 0x04006B9B RID: 27547
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] shockIndices = new int[8];

	// Token: 0x04006B9C RID: 27548
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] currentShockingVertex = new int[8];

	// Token: 0x04006B9D RID: 27549
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<WireNode.ShockSpot> shockSpots;

	// Token: 0x04006B9E RID: 27550
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 min = Vector3.one * float.PositiveInfinity;

	// Token: 0x04006B9F RID: 27551
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 max = -Vector3.one * float.PositiveInfinity;

	// Token: 0x04006BA0 RID: 27552
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 prevSourcePosition = Vector3.zero;

	// Token: 0x04006BA1 RID: 27553
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 prevLocalPosition = Vector3.zero;

	// Token: 0x04006BA2 RID: 27554
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject prevParentGO;

	// Token: 0x04006BA3 RID: 27555
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float prevSize = 0.01f;

	// Token: 0x04006BA4 RID: 27556
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Material personalMaterial;

	// Token: 0x04006BA5 RID: 27557
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int numNodes = 15;

	// Token: 0x04006BA6 RID: 27558
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color prevWireColor = Color.black;

	// Token: 0x04006BA7 RID: 27559
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float prevWeightMod = 1f;

	// Token: 0x04006BA8 RID: 27560
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float prevSpringMod = 1f;

	// Token: 0x04006BA9 RID: 27561
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform parent;

	// Token: 0x04006BAA RID: 27562
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform prevParent;

	// Token: 0x04006BAB RID: 27563
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Mesh mesh;

	// Token: 0x04006BAC RID: 27564
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshFilter meshFilter;

	// Token: 0x04006BAD RID: 27565
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshRenderer meshRenderer;

	// Token: 0x04006BAE RID: 27566
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshCollider meshCollider;

	// Token: 0x04006BAF RID: 27567
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> points;

	// Token: 0x04006BB0 RID: 27568
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> forces;

	// Token: 0x04006BB1 RID: 27569
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> verts;

	// Token: 0x04006BB2 RID: 27570
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector2> uvs;

	// Token: 0x04006BB3 RID: 27571
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector2> uvs2;

	// Token: 0x04006BB4 RID: 27572
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> normals;

	// Token: 0x04006BB5 RID: 27573
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<bool> shockNodes;

	// Token: 0x04006BB6 RID: 27574
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] indices;

	// Token: 0x04006BB7 RID: 27575
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool usingLocalPosition = true;

	// Token: 0x04006BB8 RID: 27576
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool usingSourcePosition = true;

	// Token: 0x04006BB9 RID: 27577
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float checkPlayerConnectTime;

	// Token: 0x04006BBA RID: 27578
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int attachedNode = -1;

	// Token: 0x04006BBB RID: 27579
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int prevAttachedNode = -1;

	// Token: 0x04006BBC RID: 27580
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float slopeSpeed = 1f;

	// Token: 0x04006BBD RID: 27581
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 currLocalPosition;

	// Token: 0x04006BBE RID: 27582
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 currSourcePosition;

	// Token: 0x04006BBF RID: 27583
	public float drag = 0.89f;

	// Token: 0x04006BC0 RID: 27584
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 cachedSourcePosition;

	// Token: 0x02001111 RID: 4369
	[PublicizedFrom(EAccessModifier.Private)]
	public struct ShockSpot
	{
		// Token: 0x04006BC1 RID: 27585
		public float timer;

		// Token: 0x04006BC2 RID: 27586
		public int shockDataIndex;

		// Token: 0x04006BC3 RID: 27587
		public int vertex;

		// Token: 0x04006BC4 RID: 27588
		public GameObject shockPrefab;
	}
}

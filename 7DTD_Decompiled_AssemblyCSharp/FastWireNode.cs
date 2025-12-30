using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010CD RID: 4301
public class FastWireNode : MonoBehaviour, IWireNode
{
	// Token: 0x17000E29 RID: 3625
	// (get) Token: 0x06008735 RID: 34613 RVA: 0x0036BE5D File Offset: 0x0036A05D
	// (set) Token: 0x06008736 RID: 34614 RVA: 0x0036BE65 File Offset: 0x0036A065
	public Vector3 StartPosition
	{
		get
		{
			return this.startPosition;
		}
		set
		{
			this.startPosition = value;
		}
	}

	// Token: 0x17000E2A RID: 3626
	// (get) Token: 0x06008737 RID: 34615 RVA: 0x0036BE6E File Offset: 0x0036A06E
	// (set) Token: 0x06008738 RID: 34616 RVA: 0x0036BE76 File Offset: 0x0036A076
	public Vector3 EndPosition
	{
		get
		{
			return this.endPosition;
		}
		set
		{
			this.endPosition = value;
		}
	}

	// Token: 0x06008739 RID: 34617 RVA: 0x0036BE80 File Offset: 0x0036A080
	public void Awake()
	{
		if (FastWireNode.BaseMaterial == null)
		{
			FastWireNode.BaseMaterial = Resources.Load<Material>("Materials/WireMaterial");
		}
		if (this.meshFilter == null)
		{
			this.meshFilter = base.transform.gameObject.AddMissingComponent<MeshFilter>();
		}
		if (this.meshCollider == null)
		{
			this.meshCollider = base.transform.gameObject.AddMissingComponent<MeshCollider>();
			this.meshCollider.convex = true;
			this.meshCollider.isTrigger = true;
		}
		if (this.meshRenderer == null)
		{
			this.meshRenderer = base.transform.gameObject.AddMissingComponent<MeshRenderer>();
			this.meshRenderer.material = FastWireNode.BaseMaterial;
		}
		Utils.SetColliderLayerRecursively(base.gameObject, 29);
	}

	// Token: 0x0600873A RID: 34618 RVA: 0x0036BF4C File Offset: 0x0036A14C
	public void BuildMesh()
	{
		Vector3 vector = this.EndPosition + this.EndOffset;
		Vector3 vector2 = this.StartPosition + this.StartOffset;
		float num = Vector3.Distance(vector, vector2);
		if (num > 256f)
		{
			return;
		}
		if (num < 0.01f)
		{
			return;
		}
		Vector3 a = vector2 - vector;
		if (a.magnitude == 0f)
		{
			return;
		}
		Vector3 b = a / 16f;
		if (b.magnitude == 0f)
		{
			return;
		}
		Vector3 vector3 = vector + b;
		List<Vector3> list = new List<Vector3>();
		list.Add(vector);
		for (int i = 0; i < 15; i++)
		{
			if (a.normalized != Vector3.up && a.normalized != Vector3.down)
			{
				float num2 = Mathf.Abs((8f - (float)(i + 1)) / 8f);
				num2 *= num2;
				list.Add(vector3 - new Vector3(0f, Mathf.Lerp(0f, this.maxWireDip, 1f - num2), 0f));
			}
			else
			{
				list.Add(vector3);
			}
			vector3 += b;
		}
		if (list.Count > 1)
		{
			list[0] = vector;
			list[list.Count - 1] = vector2;
		}
		Vector3 vector4 = Vector3.one * float.PositiveInfinity;
		Vector3 vector5 = -Vector3.one * float.PositiveInfinity;
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		List<Vector2> list4 = new List<Vector2>();
		List<Vector2> list5 = new List<Vector2>();
		int[] array = new int[396];
		for (int j = 0; j < list.Count; j++)
		{
			float d = (float)j / (float)list.Count * (num * 0.25f);
			list4.Add(Vector2.right * d);
			list4.Add(Vector2.right * d + Vector2.up);
			list4.Add(Vector2.right * d);
			list4.Add(Vector2.right * d + Vector2.up);
			list5.Add(Vector2.zero);
			list5.Add(Vector2.zero);
			list5.Add(Vector2.zero);
			list5.Add(Vector2.zero);
			if (j > 0)
			{
				a = list[j] - list[j - 1];
			}
			Vector3 normalized = Vector3.Cross(Vector3.up, a.normalized).normalized;
			Vector3 normalized2 = Vector3.Cross(a.normalized, normalized).normalized;
			if (a.normalized == Vector3.up || a.normalized == Vector3.down)
			{
				normalized = Vector3.Cross(Vector3.forward, a.normalized).normalized;
				normalized2 = Vector3.Cross(a.normalized, normalized).normalized;
			}
			list2.Add(normalized2 * this.wireRadius + list[j]);
			list2.Add(-normalized2 * this.wireRadius + list[j]);
			list2.Add(normalized * this.wireRadius + list[j]);
			list2.Add(-normalized * this.wireRadius + list[j]);
			if (j == 0)
			{
				normalized2 = Vector3.Lerp(normalized2, (vector - vector2).normalized, 0.5f).normalized;
				normalized = Vector3.Lerp(normalized, (vector - vector2).normalized, 0.5f).normalized;
			}
			else if (j == list.Count - 1)
			{
				normalized2 = Vector3.Lerp(normalized2, -(vector - vector2).normalized, 0.5f).normalized;
				normalized = Vector3.Lerp(normalized, -(vector - vector2).normalized, 0.5f).normalized;
			}
			list3.Add(normalized2);
			list3.Add(-normalized2);
			list3.Add(normalized);
			list3.Add(-normalized);
			if (list[j].x < vector4.x)
			{
				vector4.x = list[j].x;
			}
			if (list[j].x > vector5.x)
			{
				vector5.x = list[j].x;
			}
			if (list[j].y < vector4.y)
			{
				vector4.y = list[j].y;
			}
			if (list[j].y > vector5.y)
			{
				vector5.y = list[j].y;
			}
			if (list[j].z < vector4.z)
			{
				vector4.z = list[j].z;
			}
			if (list[j].z > vector5.z)
			{
				vector5.z = list[j].z;
			}
		}
		int num3 = 0;
		for (int k = 0; k < 15; k++)
		{
			array[num3++] = 4 * k;
			array[num3++] = 4 + 4 * k;
			array[num3++] = 7 + 4 * k;
			array[num3++] = 7 + 4 * k;
			array[num3++] = 3 + 4 * k;
			array[num3++] = 4 * k;
			array[num3++] = 4 + 4 * k;
			array[num3++] = 4 * k;
			array[num3++] = 2 + 4 * k;
			array[num3++] = 2 + 4 * k;
			array[num3++] = 6 + 4 * k;
			array[num3++] = 4 + 4 * k;
			array[num3++] = 3 + 4 * k;
			array[num3++] = 7 + 4 * k;
			array[num3++] = 5 + 4 * k;
			array[num3++] = 5 + 4 * k;
			array[num3++] = 1 + 4 * k;
			array[num3++] = 3 + 4 * k;
			array[num3++] = 6 + 4 * k;
			array[num3++] = 2 + 4 * k;
			array[num3++] = 1 + 4 * k;
			array[num3++] = 1 + 4 * k;
			array[num3++] = 5 + 4 * k;
			array[num3++] = 6 + 4 * k;
		}
		array[num3++] = 0;
		array[num3++] = 3;
		array[num3++] = 1;
		array[num3++] = 1;
		array[num3++] = 2;
		array[num3++] = 0;
		array[num3++] = 60;
		array[num3++] = 62;
		array[num3++] = 61;
		array[num3++] = 61;
		array[num3++] = 63;
		array[num3++] = 60;
		if (list2.Count < 3)
		{
			return;
		}
		if (array.Length < 3)
		{
			return;
		}
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
		}
		this.mesh.SetVertices(list2);
		this.mesh.uv = list4.ToArray();
		this.mesh.uv2 = list5.ToArray();
		this.mesh.SetNormals(list3);
		this.mesh.SetIndices(array, MeshTopology.Triangles, 0);
		this.mesh.RecalculateBounds();
		this.meshFilter.mesh = this.mesh;
		this.meshCollider.sharedMesh = this.mesh;
		if (this.prevWireColor != this.wireColor)
		{
			this.prevWireColor = this.wireColor;
			this.SetWireColor(this.wireColor);
		}
	}

	// Token: 0x0600873B RID: 34619 RVA: 0x0036C7DE File Offset: 0x0036A9DE
	public void SetWireColor(Color color)
	{
		if (this.meshRenderer.material == null)
		{
			return;
		}
		this.meshRenderer.material.SetColor("_Color", color);
		this.wireColor = color;
	}

	// Token: 0x0600873C RID: 34620 RVA: 0x0036C811 File Offset: 0x0036AA11
	public void SetPulseSpeed(float speed)
	{
		if (this.meshRenderer.material == null)
		{
			return;
		}
		this.meshRenderer.material.SetFloat("_PulseSpeed", speed);
	}

	// Token: 0x0600873D RID: 34621 RVA: 0x0036C83D File Offset: 0x0036AA3D
	public void SetPulseColor(Color color)
	{
		this.pulseColor = color;
	}

	// Token: 0x0600873E RID: 34622 RVA: 0x0036C846 File Offset: 0x0036AA46
	public void TogglePulse(bool isOn)
	{
		if (this.meshRenderer.material == null)
		{
			return;
		}
		this.meshRenderer.material.SetColor("_PulseColor", isOn ? this.pulseColor : this.wireColor);
	}

	// Token: 0x0600873F RID: 34623 RVA: 0x0036C882 File Offset: 0x0036AA82
	public void SetStartPosition(Vector3 pos)
	{
		this.StartPosition = pos;
	}

	// Token: 0x06008740 RID: 34624 RVA: 0x0036C88B File Offset: 0x0036AA8B
	public void SetStartPositionOffset(Vector3 pos)
	{
		this.StartOffset = pos;
	}

	// Token: 0x06008741 RID: 34625 RVA: 0x0036C894 File Offset: 0x0036AA94
	public void SetEndPosition(Vector3 pos)
	{
		this.EndPosition = pos;
	}

	// Token: 0x06008742 RID: 34626 RVA: 0x0036C89D File Offset: 0x0036AA9D
	public void SetEndPositionOffset(Vector3 pos)
	{
		this.EndOffset = pos;
	}

	// Token: 0x06008743 RID: 34627 RVA: 0x0036C8A6 File Offset: 0x0036AAA6
	public void SetWireDip(float _dist)
	{
		this.maxWireDip = _dist;
	}

	// Token: 0x06008744 RID: 34628 RVA: 0x0036C8AF File Offset: 0x0036AAAF
	public float GetWireDip()
	{
		return this.maxWireDip;
	}

	// Token: 0x06008745 RID: 34629 RVA: 0x0036C8B7 File Offset: 0x0036AAB7
	public void SetWireRadius(float _radius)
	{
		this.wireRadius = _radius;
	}

	// Token: 0x06008746 RID: 34630 RVA: 0x0036C8C0 File Offset: 0x0036AAC0
	public void SetWireCanHide(bool _canHide)
	{
		this.canHide = _canHide;
	}

	// Token: 0x06008747 RID: 34631 RVA: 0x0036C8C9 File Offset: 0x0036AAC9
	public Vector3 GetStartPosition()
	{
		return this.StartPosition;
	}

	// Token: 0x06008748 RID: 34632 RVA: 0x0036C8D1 File Offset: 0x0036AAD1
	public Vector3 GetStartPositionOffset()
	{
		return this.StartOffset;
	}

	// Token: 0x06008749 RID: 34633 RVA: 0x0036C8D9 File Offset: 0x0036AAD9
	public Vector3 GetEndPosition()
	{
		return this.EndPosition;
	}

	// Token: 0x0600874A RID: 34634 RVA: 0x0036C8E1 File Offset: 0x0036AAE1
	public Vector3 GetEndPositionOffset()
	{
		return this.EndOffset;
	}

	// Token: 0x0600874B RID: 34635 RVA: 0x0036C8E9 File Offset: 0x0036AAE9
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600874C RID: 34636 RVA: 0x0036C8F1 File Offset: 0x0036AAF1
	public void SetVisible(bool _visible)
	{
		if (this.canHide)
		{
			base.gameObject.SetActive(_visible);
			return;
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600874D RID: 34637 RVA: 0x0036C914 File Offset: 0x0036AB14
	public Bounds GetBounds()
	{
		return this.mesh.bounds;
	}

	// Token: 0x0600874E RID: 34638 RVA: 0x0036C921 File Offset: 0x0036AB21
	public void Reset()
	{
		this.maxWireDip = 0.25f;
		this.wireRadius = 0.01f;
		this.pulseColor = Color.yellow;
	}

	// Token: 0x0400691A RID: 26906
	public const int cLayerMaskRayCast = 65537;

	// Token: 0x0400691B RID: 26907
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int NODE_COUNT = 15;

	// Token: 0x0400691C RID: 26908
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float BASE_WIRE_RADIUS = 0.01f;

	// Token: 0x0400691D RID: 26909
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float BASE_MIN_WIRE_DIP = 0f;

	// Token: 0x0400691E RID: 26910
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float BASE_MAX_WIRE_DIP = 0.25f;

	// Token: 0x0400691F RID: 26911
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static Material BaseMaterial;

	// Token: 0x04006920 RID: 26912
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float maxWireDip = 0.25f;

	// Token: 0x04006921 RID: 26913
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wireRadius = 0.01f;

	// Token: 0x04006922 RID: 26914
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 startPosition;

	// Token: 0x04006923 RID: 26915
	public Vector3 StartOffset;

	// Token: 0x04006924 RID: 26916
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 endPosition;

	// Token: 0x04006925 RID: 26917
	public Vector3 EndOffset;

	// Token: 0x04006926 RID: 26918
	public Color pulseColor = Color.yellow;

	// Token: 0x04006927 RID: 26919
	public Color wireColor = Color.black;

	// Token: 0x04006928 RID: 26920
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canHide = true;

	// Token: 0x04006929 RID: 26921
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Mesh mesh;

	// Token: 0x0400692A RID: 26922
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshFilter meshFilter;

	// Token: 0x0400692B RID: 26923
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshCollider meshCollider;

	// Token: 0x0400692C RID: 26924
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshRenderer meshRenderer;

	// Token: 0x0400692D RID: 26925
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color prevWireColor = Color.white;
}

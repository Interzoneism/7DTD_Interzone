using System;
using UnityEngine;

// Token: 0x020012EA RID: 4842
public class vp_WaypointGizmo : MonoBehaviour
{
	// Token: 0x060096E1 RID: 38625 RVA: 0x003BE880 File Offset: 0x003BCA80
	public void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = this.m_GizmoColor;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x060096E2 RID: 38626 RVA: 0x003BE8E4 File Offset: 0x003BCAE4
	public void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = this.m_SelectedGizmoColor;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x040072AC RID: 29356
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_GizmoColor = new Color(1f, 1f, 1f, 0.4f);

	// Token: 0x040072AD RID: 29357
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_SelectedGizmoColor = new Color32(160, byte.MaxValue, 100, 100);
}

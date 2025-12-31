using System;
using UnityEngine;

// Token: 0x020012D7 RID: 4823
public class vp_Gizmo : MonoBehaviour
{
	// Token: 0x06009640 RID: 38464 RVA: 0x003BC39C File Offset: 0x003BA59C
	public void OnDrawGizmos()
	{
		Vector3 center = base.GetComponent<Collider>().bounds.center;
		Vector3 size = base.GetComponent<Collider>().bounds.size;
		Gizmos.color = this.gizmoColor;
		Gizmos.DrawCube(center, size);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x06009641 RID: 38465 RVA: 0x003BC410 File Offset: 0x003BA610
	public void OnDrawGizmosSelected()
	{
		Vector3 center = base.GetComponent<Collider>().bounds.center;
		Vector3 size = base.GetComponent<Collider>().bounds.size;
		Gizmos.color = this.selectedGizmoColor;
		Gizmos.DrawCube(center, size);
		Gizmos.color = new Color(0f, 0f, 0f, 1f);
		Gizmos.DrawLine(Vector3.zero, Vector3.forward);
	}

	// Token: 0x04007257 RID: 29271
	public Color gizmoColor = new Color(1f, 1f, 1f, 0.4f);

	// Token: 0x04007258 RID: 29272
	public Color selectedGizmoColor = new Color(1f, 1f, 1f, 0.4f);
}

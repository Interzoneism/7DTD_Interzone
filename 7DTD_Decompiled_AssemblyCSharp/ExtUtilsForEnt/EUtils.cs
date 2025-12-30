using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace ExtUtilsForEnt
{
	// Token: 0x020016F1 RID: 5873
	[Preserve]
	public class EUtils
	{
		// Token: 0x0600B1C5 RID: 45509 RVA: 0x00434DB4 File Offset: 0x00432FB4
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 1f)
		{
			Utils.DrawLine(start - Origin.position, end - Origin.position, color, color, 10, duration);
		}

		// Token: 0x0600B1C6 RID: 45510 RVA: 0x0045454C File Offset: 0x0045274C
		public static void DrawPath(List<Vector3> path, Color start, Color end)
		{
			for (int i = 0; i < path.Count - 1; i++)
			{
				Utils.DrawLine(path[i] - Origin.position, path[i + 1] - Origin.position, start, end, 10, 10f);
			}
		}

		// Token: 0x0600B1C7 RID: 45511 RVA: 0x004545A0 File Offset: 0x004527A0
		public static void DrawBounds(Vector3 pos, Color color, float duration, float scale = 1f)
		{
			Utils.DrawBoxLines(new Vector3(pos.x, pos.y, pos.z) - Origin.position, new Vector3(pos.x + scale, pos.y + scale, pos.z + scale) - Origin.position, color, duration);
		}

		// Token: 0x0600B1C8 RID: 45512 RVA: 0x004545FC File Offset: 0x004527FC
		public static void DrawBounds(Vector3i pos, Color color, float duration, float scale = 1f)
		{
			Utils.DrawBoxLines(new Vector3((float)pos.x, (float)pos.y, (float)pos.z) - Origin.position, new Vector3((float)pos.x + scale, (float)pos.y + scale, (float)pos.z + scale) - Origin.position, color, duration);
		}

		// Token: 0x0600B1C9 RID: 45513 RVA: 0x00454660 File Offset: 0x00452860
		public static bool isPositionBlocked(Vector3 start, Vector3 end, int layerMask = 0, bool debugDraw = false)
		{
			Vector3 direction = end - start;
			float magnitude = direction.magnitude;
			Ray ray = new Ray(start - Origin.position, direction);
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, out raycastHit, magnitude, layerMask);
			if (debugDraw)
			{
				if (flag)
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * raycastHit.distance, Color.magenta, Color.red, 1, 5f);
				}
				else
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * magnitude, Color.cyan, Color.blue, 1, 5f);
				}
			}
			return flag;
		}

		// Token: 0x0600B1CA RID: 45514 RVA: 0x00454718 File Offset: 0x00452918
		public static bool isPositionBlocked(Vector3 start, Vector3 end, out RaycastHit hit, int layerMask = 0, bool debugDraw = false)
		{
			Vector3 direction = end - start;
			float magnitude = direction.magnitude;
			Ray ray = new Ray(start - Origin.position, direction);
			bool flag = Physics.Raycast(ray, out hit, magnitude, layerMask);
			if (debugDraw)
			{
				if (flag)
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, Color.magenta, Color.red, 1, 5f);
				}
				else
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * magnitude, Color.cyan, Color.blue, 1, 5f);
				}
			}
			return flag;
		}
	}
}

using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015C5 RID: 5573
	[Preserve]
	public class RaycastPathUtils
	{
		// Token: 0x0600AB0E RID: 43790 RVA: 0x00434BD0 File Offset: 0x00432DD0
		public static bool IsPositionBlocked(Vector3 start, Vector3 end, out RaycastHit hit, int layerMask = 0, bool debugDraw = false)
		{
			Vector3 direction = end - start;
			return RaycastPathUtils.IsPositionBlocked(new Ray(start - Origin.position, direction), out hit, layerMask, debugDraw, direction.magnitude + 1f);
		}

		// Token: 0x0600AB0F RID: 43791 RVA: 0x00434C0C File Offset: 0x00432E0C
		public static bool IsPositionBlocked(Vector3 start, Vector3 end, int layerMask = 0, bool debugDraw = false)
		{
			RaycastHit raycastHit;
			return RaycastPathUtils.IsPositionBlocked(start, end, out raycastHit, layerMask, debugDraw);
		}

		// Token: 0x0600AB10 RID: 43792 RVA: 0x00434C24 File Offset: 0x00432E24
		public static bool IsPointBlocked(Vector3 start, Vector3 end, int layerMask = 0, bool debugDraw = false, float duration = 0f)
		{
			RaycastHit raycastHit;
			return RaycastPathUtils.CheckPositionBlocked(start, end, out raycastHit, layerMask, debugDraw, duration);
		}

		// Token: 0x0600AB11 RID: 43793 RVA: 0x00434C40 File Offset: 0x00432E40
		public static bool CheckPositionBlocked(Vector3 start, Vector3 end, out RaycastHit hit, int layerMask = 0, bool debugDraw = false, float duration = 0f)
		{
			Vector3 direction = end - start;
			return RaycastPathUtils.CheckPositionBlocked(new Ray(start - Origin.position, direction), out hit, layerMask, debugDraw, direction.magnitude, duration);
		}

		// Token: 0x0600AB12 RID: 43794 RVA: 0x00434C78 File Offset: 0x00432E78
		public static bool CheckPositionBlocked(Ray ray, out RaycastHit hit, int layerMask = 0, bool debugDraw = false, float maxDist = 100f, float duration = 0f)
		{
			bool flag = Physics.Raycast(ray, out hit, maxDist, layerMask);
			if (debugDraw)
			{
				if (flag)
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, Color.magenta, Color.red, 1, duration);
				}
				else
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * maxDist, Color.cyan, Color.blue, 1, duration);
				}
			}
			return flag;
		}

		// Token: 0x0600AB13 RID: 43795 RVA: 0x00434D04 File Offset: 0x00432F04
		public static bool IsPositionBlocked(Ray ray, out RaycastHit hit, int layerMask = 0, bool debugDraw = false, float maxDist = 100f)
		{
			bool flag = Physics.Raycast(ray, out hit, maxDist, layerMask);
			if (debugDraw)
			{
				if (flag)
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * hit.distance, Color.magenta, Color.red, 1, 5f);
				}
				else
				{
					Utils.DrawLine(ray.origin, ray.origin + ray.direction * maxDist, Color.cyan, Color.blue, 1, 5f);
				}
			}
			return flag;
		}

		// Token: 0x0600AB14 RID: 43796 RVA: 0x00434D98 File Offset: 0x00432F98
		public static bool IsPositionBlocked(Ray ray, int layerMask = 0, bool debugDraw = false, float maxDist = 100f)
		{
			RaycastHit raycastHit;
			return RaycastPathUtils.IsPositionBlocked(ray, out raycastHit, layerMask, debugDraw, 100f);
		}

		// Token: 0x0600AB15 RID: 43797 RVA: 0x00434DB4 File Offset: 0x00432FB4
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 1f)
		{
			Utils.DrawLine(start - Origin.position, end - Origin.position, color, color, 10, duration);
		}

		// Token: 0x0600AB16 RID: 43798 RVA: 0x00434DD8 File Offset: 0x00432FD8
		public static void DrawBounds(Vector3i pos, Color color, float duration, float scale = 1f)
		{
			Utils.DrawBoxLines(new Vector3((float)pos.x, (float)pos.y, (float)pos.z) - Origin.position, new Vector3((float)pos.x + scale, (float)pos.y + scale, (float)pos.z + scale) - Origin.position, color, duration);
		}

		// Token: 0x0600AB17 RID: 43799 RVA: 0x00434E3C File Offset: 0x0043303C
		public static void DrawBounds(Vector3 pos, Color color, float duration, float scale = 1f)
		{
			Utils.DrawBoxLines(new Vector3i(pos.x, pos.y, pos.z) - Origin.position, new Vector3i(pos.x + scale, pos.y + scale, pos.z + scale) - Origin.position, color, duration);
		}

		// Token: 0x0600AB18 RID: 43800 RVA: 0x00434EA2 File Offset: 0x004330A2
		public static void DrawNode(RaycastNode node, Color color, float duration)
		{
			RaycastPathUtils.DrawNode(node.Min, node.Max, color, duration);
		}

		// Token: 0x0600AB19 RID: 43801 RVA: 0x00434EB7 File Offset: 0x004330B7
		public static void DrawNode(Vector3 min, Vector3 max, Color color, float duration)
		{
			RaycastPathUtils.DrawVolume(min - Origin.position, max - Origin.position, color, duration);
		}

		// Token: 0x0600AB1A RID: 43802 RVA: 0x00434ED8 File Offset: 0x004330D8
		[PublicizedFrom(EAccessModifier.Private)]
		public static void DrawVolume(Vector3 min, Vector3 max, Color color, float duration)
		{
			Vector3 vector = new Vector3(max.x, min.y, min.z);
			Vector3 vector2 = new Vector3(min.x, max.y, min.z);
			Vector3 end = new Vector3(min.x, min.y, max.z);
			Vector3 vector3 = new Vector3(min.x, max.y, max.z);
			Vector3 vector4 = new Vector3(max.x, min.y, max.z);
			Vector3 end2 = new Vector3(max.x, max.y, min.z);
			Debug.DrawLine(min, vector, color, duration);
			Debug.DrawLine(min, vector2, color, duration);
			Debug.DrawLine(min, end, color, duration);
			Debug.DrawLine(max, vector3, color, duration);
			Debug.DrawLine(max, vector4, color, duration);
			Debug.DrawLine(max, end2, color, duration);
			Debug.DrawLine(vector3, vector2, color, duration);
			Debug.DrawLine(vector, vector4, color, duration);
			Debug.DrawLine(vector4, end, color, duration);
			Debug.DrawLine(vector2, end2, color, duration);
			Debug.DrawLine(vector3, end, color, duration);
			Debug.DrawLine(vector, end2, color, duration);
		}

		// Token: 0x04008581 RID: 34177
		[PublicizedFrom(EAccessModifier.Private)]
		public const int cMaxRayDist = 100;
	}
}

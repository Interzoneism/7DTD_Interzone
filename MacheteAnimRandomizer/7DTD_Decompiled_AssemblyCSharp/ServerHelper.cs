using System;
using UnityEngine;

// Token: 0x0200120F RID: 4623
public static class ServerHelper
{
	// Token: 0x0600903F RID: 36927 RVA: 0x00398924 File Offset: 0x00396B24
	public static void SetupForServer(GameObject obj)
	{
		Component[] array = obj.GetComponentsInChildren<Renderer>();
		array = array;
		for (int i = 0; i < array.Length; i++)
		{
			((Renderer)array[i]).enabled = false;
		}
	}
}

using System;
using UnityEngine;

// Token: 0x020012BF RID: 4799
public static class vp_TargetEvent<T>
{
	// Token: 0x060095B5 RID: 38325 RVA: 0x003B9BE0 File Offset: 0x003B7DE0
	public static void Register(object target, string eventName, Action<T> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 1);
	}

	// Token: 0x060095B6 RID: 38326 RVA: 0x003B9B1B File Offset: 0x003B7D1B
	public static void Unregister(object target, string eventName, Action<T> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x060095B7 RID: 38327 RVA: 0x003B9B25 File Offset: 0x003B7D25
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x060095B8 RID: 38328 RVA: 0x003B9BEC File Offset: 0x003B7DEC
	public static void Send(object target, string eventName, T arg, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 1, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action<T>)callback)(arg);
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
	}

	// Token: 0x060095B9 RID: 38329 RVA: 0x003B9C40 File Offset: 0x003B7E40
	public static void SendUpwards(Component target, string eventName, T arg, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 1, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action<T>)callback)(arg);
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
	}
}

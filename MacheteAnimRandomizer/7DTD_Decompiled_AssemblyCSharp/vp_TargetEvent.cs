using System;
using UnityEngine;

// Token: 0x020012BE RID: 4798
public static class vp_TargetEvent
{
	// Token: 0x060095AF RID: 38319 RVA: 0x003B9B10 File Offset: 0x003B7D10
	public static void Register(object target, string eventName, Action callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 0);
	}

	// Token: 0x060095B0 RID: 38320 RVA: 0x003B9B1B File Offset: 0x003B7D1B
	public static void Unregister(object target, string eventName, Action callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x060095B1 RID: 38321 RVA: 0x003B9B25 File Offset: 0x003B7D25
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x060095B2 RID: 38322 RVA: 0x003B9B2F File Offset: 0x003B7D2F
	public static void Unregister(Component component)
	{
		vp_TargetEventHandler.Unregister(component);
	}

	// Token: 0x060095B3 RID: 38323 RVA: 0x003B9B38 File Offset: 0x003B7D38
	public static void Send(object target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 0, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action)callback)();
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

	// Token: 0x060095B4 RID: 38324 RVA: 0x003B9B8C File Offset: 0x003B7D8C
	public static void SendUpwards(Component target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 0, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				((Action)callback)();
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

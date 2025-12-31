using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020012AC RID: 4780
public static class vp_GlobalEvent<T>
{
	// Token: 0x0600955A RID: 38234 RVA: 0x003B81FC File Offset: 0x003B63FC
	[Preserve]
	public static void Register(string name, vp_GlobalCallback<T> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T>> list = (List<vp_GlobalCallback<T>>)vp_GlobalEvent<T>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallback<T>>();
			vp_GlobalEvent<T>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x0600955B RID: 38235 RVA: 0x003B8258 File Offset: 0x003B6458
	[Preserve]
	public static void Unregister(string name, vp_GlobalCallback<T> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T>> list = (List<vp_GlobalCallback<T>>)vp_GlobalEvent<T>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x0600955C RID: 38236 RVA: 0x003B82A9 File Offset: 0x003B64A9
	public static void Send(string name, T arg1)
	{
		vp_GlobalEvent<T>.Send(name, arg1, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x0600955D RID: 38237 RVA: 0x003B82B4 File Offset: 0x003B64B4
	public static void Send(string name, T arg1, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (arg1 == null)
		{
			throw new ArgumentNullException("arg1");
		}
		List<vp_GlobalCallback<T>> list = (List<vp_GlobalCallback<T>>)vp_GlobalEvent<T>.m_Callbacks[name];
		if (list != null)
		{
			using (List<vp_GlobalCallback<T>>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					vp_GlobalCallback<T> vp_GlobalCallback = enumerator.Current;
					vp_GlobalCallback(arg1);
				}
				return;
			}
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
	}

	// Token: 0x040071F9 RID: 29177
	[PublicizedFrom(EAccessModifier.Private)]
	public static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}

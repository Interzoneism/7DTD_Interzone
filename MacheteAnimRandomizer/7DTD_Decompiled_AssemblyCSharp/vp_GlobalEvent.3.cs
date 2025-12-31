using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020012AD RID: 4781
public static class vp_GlobalEvent<T, U>
{
	// Token: 0x0600955F RID: 38239 RVA: 0x003B8354 File Offset: 0x003B6554
	[Preserve]
	public static void Register(string name, vp_GlobalCallback<T, U> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T, U>> list = (List<vp_GlobalCallback<T, U>>)vp_GlobalEvent<T, U>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallback<T, U>>();
			vp_GlobalEvent<T, U>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06009560 RID: 38240 RVA: 0x003B83B0 File Offset: 0x003B65B0
	[Preserve]
	public static void Unregister(string name, vp_GlobalCallback<T, U> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallback<T, U>> list = (List<vp_GlobalCallback<T, U>>)vp_GlobalEvent<T, U>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x06009561 RID: 38241 RVA: 0x003B8401 File Offset: 0x003B6601
	public static void Send(string name, T arg1, U arg2)
	{
		vp_GlobalEvent<T, U>.Send(name, arg1, arg2, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x06009562 RID: 38242 RVA: 0x003B840C File Offset: 0x003B660C
	public static void Send(string name, T arg1, U arg2, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (arg1 == null)
		{
			throw new ArgumentNullException("arg1");
		}
		if (arg2 == null)
		{
			throw new ArgumentNullException("arg2");
		}
		List<vp_GlobalCallback<T, U>> list = (List<vp_GlobalCallback<T, U>>)vp_GlobalEvent<T, U>.m_Callbacks[name];
		if (list != null)
		{
			using (List<vp_GlobalCallback<T, U>>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					vp_GlobalCallback<T, U> vp_GlobalCallback = enumerator.Current;
					vp_GlobalCallback(arg1, arg2);
				}
				return;
			}
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
	}

	// Token: 0x040071FA RID: 29178
	[PublicizedFrom(EAccessModifier.Private)]
	public static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}

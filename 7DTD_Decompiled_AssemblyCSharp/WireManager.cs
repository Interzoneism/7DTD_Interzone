using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200110F RID: 4367
public class WireManager
{
	// Token: 0x17000E4A RID: 3658
	// (get) Token: 0x0600893B RID: 35131 RVA: 0x00379CEB File Offset: 0x00377EEB
	public static WireManager Instance
	{
		get
		{
			if (WireManager.instance == null)
			{
				WireManager.instance = new WireManager();
			}
			return WireManager.instance;
		}
	}

	// Token: 0x17000E4B RID: 3659
	// (get) Token: 0x0600893C RID: 35132 RVA: 0x00379D03 File Offset: 0x00377F03
	public static bool HasInstance
	{
		get
		{
			return WireManager.instance != null;
		}
	}

	// Token: 0x17000E4C RID: 3660
	// (get) Token: 0x0600893D RID: 35133 RVA: 0x00379D0D File Offset: 0x00377F0D
	public Transform WireManagerRoot
	{
		get
		{
			return this.wireManagerRoot;
		}
	}

	// Token: 0x0600893E RID: 35134 RVA: 0x00379D18 File Offset: 0x00377F18
	public void Init()
	{
		this.activeWires = new HashSet<IWireNode>();
		this.activePulseObjects = new HashSet<GameObject>();
		GameObject gameObject = GameObject.Find("WireManager");
		if (gameObject == null)
		{
			this.wireManagerRoot = new GameObject("WireManager").transform;
		}
		else
		{
			this.wireManagerRoot = gameObject.transform;
		}
		this.wirePool = this.wireManagerRoot.Find("Pool");
		if (this.wirePool == null)
		{
			this.wirePool = new GameObject("Pool").transform;
			this.wirePool.parent = this.wireManagerRoot;
		}
		Origin.Add(this.wireManagerRoot.transform, 0);
		if (this.wirePool.transform.childCount == 0)
		{
			for (int i = 0; i < 200; i++)
			{
				this.addNewNode();
			}
		}
	}

	// Token: 0x0600893F RID: 35135 RVA: 0x00379DF8 File Offset: 0x00377FF8
	[PublicizedFrom(EAccessModifier.Private)]
	public void addNewNode()
	{
		GameObject gameObject;
		if (WireManager.USE_FAST_WIRE_NODES)
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/WireNode2"));
		}
		else
		{
			gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/WireNode"));
		}
		UnityEngine.Object @object = gameObject;
		string format = "WireNode_{0}";
		int num = this.wireIndex;
		this.wireIndex = num + 1;
		@object.name = string.Format(format, num.ToString());
		gameObject.SetActive(false);
		gameObject.transform.parent = this.wirePool;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06008940 RID: 35136 RVA: 0x00379E98 File Offset: 0x00378098
	public void ReturnToPool(IWireNode wireNode)
	{
		this.activeWires.Remove(wireNode);
		GameObject gameObject = wireNode.GetGameObject();
		gameObject.transform.parent = this.wirePool;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.SetActive(false);
		wireNode.Reset();
	}

	// Token: 0x06008941 RID: 35137 RVA: 0x00379EF8 File Offset: 0x003780F8
	public IWireNode GetWireNodeFromPool()
	{
		if (this.wirePool.childCount < 1)
		{
			this.addNewNode();
		}
		Transform child = this.wirePool.GetChild(this.wirePool.childCount - 1);
		child.gameObject.SetActive(true);
		child.parent = this.wireManagerRoot;
		IWireNode component;
		if (WireManager.USE_FAST_WIRE_NODES)
		{
			component = child.gameObject.GetComponent<FastWireNode>();
		}
		else
		{
			component = child.gameObject.GetComponent<WireNode>();
		}
		this.activeWires.Add(component);
		return component;
	}

	// Token: 0x06008942 RID: 35138 RVA: 0x00379F7B File Offset: 0x0037817B
	public bool AddActiveWire(IWireNode wire)
	{
		return this.activeWires.Add(wire);
	}

	// Token: 0x06008943 RID: 35139 RVA: 0x00379F89 File Offset: 0x00378189
	public bool RemoveActiveWire(IWireNode wire)
	{
		return this.activeWires.Remove(wire);
	}

	// Token: 0x06008944 RID: 35140 RVA: 0x00379F97 File Offset: 0x00378197
	public bool AddPulseObject(GameObject pulseObject)
	{
		return this.activePulseObjects.Add(pulseObject);
	}

	// Token: 0x06008945 RID: 35141 RVA: 0x00379FA5 File Offset: 0x003781A5
	public bool RemovePulseObject(GameObject pulseObject)
	{
		return this.activePulseObjects.Remove(pulseObject);
	}

	// Token: 0x06008946 RID: 35142 RVA: 0x00379FB4 File Offset: 0x003781B4
	public void ToggleAllWirePulse(bool isPulseOn)
	{
		World world = GameManager.Instance.World;
		this.ShowPulse = isPulseOn;
		this.WiresShowing = isPulseOn;
		if (this.ShowPulse)
		{
			Dictionary<Vector3, bool> dictionary = new Dictionary<Vector3, bool>(Vector3EqualityComparer.Instance);
			foreach (IWireNode wireNode in this.activeWires)
			{
				Vector3 startPosition = wireNode.GetStartPosition();
				bool flag;
				if (dictionary.ContainsKey(startPosition))
				{
					flag = dictionary[startPosition];
				}
				else
				{
					flag = world.CanPlaceBlockAt(new Vector3i(startPosition), world.gameManager.GetPersistentLocalPlayer(), false);
					dictionary[startPosition] = flag;
				}
				if (flag)
				{
					wireNode.TogglePulse(isPulseOn);
					wireNode.SetVisible(this.WiresShowing);
				}
				else
				{
					wireNode.SetVisible(false);
				}
			}
			dictionary.Clear();
			using (HashSet<GameObject>.Enumerator enumerator2 = this.activePulseObjects.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					GameObject gameObject = enumerator2.Current;
					Vector3i blockPos = new Vector3i(gameObject.transform.position);
					if (world.CanPlaceBlockAt(blockPos, world.gameManager.GetPersistentLocalPlayer(), false))
					{
						gameObject.SetActive(isPulseOn);
					}
					gameObject.layer = 0;
				}
				return;
			}
		}
		foreach (IWireNode wireNode2 in this.activeWires)
		{
			wireNode2.TogglePulse(false);
			wireNode2.SetVisible(false);
		}
		foreach (GameObject gameObject2 in this.activePulseObjects)
		{
			gameObject2.SetActive(isPulseOn);
			gameObject2.layer = 11;
		}
	}

	// Token: 0x06008947 RID: 35143 RVA: 0x0037A1A4 File Offset: 0x003783A4
	public void SetWirePulse(IWireNode node)
	{
		node.TogglePulse(this.ShowPulse);
	}

	// Token: 0x06008948 RID: 35144 RVA: 0x0037A1B4 File Offset: 0x003783B4
	public void RefreshPulseObjects()
	{
		foreach (GameObject gameObject in this.activePulseObjects)
		{
			gameObject.SetActive(this.ShowPulse);
			gameObject.layer = (this.ShowPulse ? 0 : 11);
		}
	}

	// Token: 0x06008949 RID: 35145 RVA: 0x0037A220 File Offset: 0x00378420
	public void Cleanup()
	{
		foreach (IWireNode wireNode in this.activeWires)
		{
			UnityEngine.Object.Destroy(wireNode.GetGameObject());
		}
		this.activeWires.Clear();
		for (int i = this.wirePool.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.Destroy(this.wirePool.GetChild(i).gameObject);
		}
		UnityEngine.Object.Destroy(this.wirePool.gameObject);
		Origin.Remove(this.wireManagerRoot);
		UnityEngine.Object.Destroy(this.wireManagerRoot.gameObject);
		WireManager.instance = null;
	}

	// Token: 0x04006B76 RID: 27510
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool USE_FAST_WIRE_NODES = true;

	// Token: 0x04006B77 RID: 27511
	[PublicizedFrom(EAccessModifier.Private)]
	public static WireManager instance = null;

	// Token: 0x04006B78 RID: 27512
	[PublicizedFrom(EAccessModifier.Private)]
	public int wireIndex;

	// Token: 0x04006B79 RID: 27513
	public bool ShowPulse;

	// Token: 0x04006B7A RID: 27514
	[PublicizedFrom(EAccessModifier.Private)]
	public Color standardPulseColor = new Color32(0, 97, byte.MaxValue, byte.MaxValue);

	// Token: 0x04006B7B RID: 27515
	[PublicizedFrom(EAccessModifier.Private)]
	public Color tripWirePulseColor = Color.magenta;

	// Token: 0x04006B7C RID: 27516
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<IWireNode> activeWires;

	// Token: 0x04006B7D RID: 27517
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<GameObject> activePulseObjects;

	// Token: 0x04006B7E RID: 27518
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform wireManagerRoot;

	// Token: 0x04006B7F RID: 27519
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform wirePool;

	// Token: 0x04006B80 RID: 27520
	public bool WiresShowing;
}

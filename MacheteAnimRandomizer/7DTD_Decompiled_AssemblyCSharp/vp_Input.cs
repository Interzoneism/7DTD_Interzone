using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020012D8 RID: 4824
public class vp_Input : MonoBehaviour
{
	// Token: 0x17000F56 RID: 3926
	// (get) Token: 0x06009643 RID: 38467 RVA: 0x003BC4D8 File Offset: 0x003BA6D8
	public static vp_Input Instance
	{
		get
		{
			if (vp_Input.mIsDirty)
			{
				vp_Input.mIsDirty = false;
				if (vp_Input.m_Instance == null)
				{
					if (Application.isPlaying)
					{
						GameObject gameObject = Resources.Load("Input/vp_Input") as GameObject;
						if (gameObject == null)
						{
							vp_Input.m_Instance = new GameObject("vp_Input").AddComponent<vp_Input>();
						}
						else
						{
							vp_Input.m_Instance = gameObject.GetComponent<vp_Input>();
							if (vp_Input.m_Instance == null)
							{
								vp_Input.m_Instance = gameObject.AddComponent<vp_Input>();
							}
						}
					}
					vp_Input.m_Instance.SetupDefaults("");
				}
			}
			return vp_Input.m_Instance;
		}
	}

	// Token: 0x06009644 RID: 38468 RVA: 0x00002914 File Offset: 0x00000B14
	public static void CreateIfNoExist()
	{
	}

	// Token: 0x06009645 RID: 38469 RVA: 0x003BC56C File Offset: 0x003BA76C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		if (vp_Input.m_Instance == null)
		{
			vp_Input.m_Instance = vp_Input.Instance;
		}
	}

	// Token: 0x06009646 RID: 38470 RVA: 0x003BC585 File Offset: 0x003BA785
	public virtual void SetDirty(bool dirty)
	{
		vp_Input.mIsDirty = dirty;
	}

	// Token: 0x06009647 RID: 38471 RVA: 0x003BC590 File Offset: 0x003BA790
	public virtual void SetupDefaults(string type = "")
	{
		if ((type == "" || type == "Buttons") && this.ButtonKeys.Count == 0)
		{
			this.AddButton("Attack", KeyCode.Mouse0);
			this.AddButton("SetNextWeapon", KeyCode.E);
			this.AddButton("SetPrevWeapon", KeyCode.Q);
			this.AddButton("ClearWeapon", KeyCode.Backspace);
			this.AddButton("Zoom", KeyCode.Mouse1);
			this.AddButton("Reload", KeyCode.R);
			this.AddButton("Jump", KeyCode.Space);
			this.AddButton("Crouch", KeyCode.C);
			this.AddButton("Run", KeyCode.LeftShift);
			this.AddButton("Interact", KeyCode.F);
			this.AddButton("Accept1", KeyCode.Return);
			this.AddButton("Accept2", KeyCode.KeypadEnter);
			this.AddButton("Pause", KeyCode.P);
			this.AddButton("Menu", KeyCode.Escape);
		}
		if ((type == "" || type == "Axis") && this.AxisKeys.Count == 0)
		{
			this.AddAxis("Vertical", KeyCode.W, KeyCode.S);
			this.AddAxis("Horizontal", KeyCode.D, KeyCode.A);
		}
		if ((type == "" || type == "UnityAxis") && this.UnityAxis.Count == 0)
		{
			this.AddUnityAxis("Mouse X");
			this.AddUnityAxis("Mouse Y");
		}
		this.UpdateDictionaries();
	}

	// Token: 0x06009648 RID: 38472 RVA: 0x003BC714 File Offset: 0x003BA914
	public virtual void AddButton(string n, KeyCode k = KeyCode.None)
	{
		if (this.ButtonKeys.Contains(n))
		{
			this.ButtonValues[this.ButtonKeys.IndexOf(n)] = k;
			return;
		}
		this.ButtonKeys.Add(n);
		this.ButtonValues.Add(k);
	}

	// Token: 0x06009649 RID: 38473 RVA: 0x003BC760 File Offset: 0x003BA960
	public virtual void AddAxis(string n, KeyCode pk = KeyCode.None, KeyCode nk = KeyCode.None)
	{
		if (this.AxisKeys.Contains(n))
		{
			this.AxisValues[this.AxisKeys.IndexOf(n)] = new vp_Input.vp_InputAxis
			{
				Positive = pk,
				Negative = nk
			};
			return;
		}
		this.AxisKeys.Add(n);
		this.AxisValues.Add(new vp_Input.vp_InputAxis
		{
			Positive = pk,
			Negative = nk
		});
	}

	// Token: 0x0600964A RID: 38474 RVA: 0x003BC7D0 File Offset: 0x003BA9D0
	public virtual void AddUnityAxis(string n)
	{
		if (this.UnityAxis.Contains(n))
		{
			this.UnityAxis[this.UnityAxis.IndexOf(n)] = n;
			return;
		}
		this.UnityAxis.Add(n);
	}

	// Token: 0x0600964B RID: 38475 RVA: 0x003BC808 File Offset: 0x003BAA08
	public virtual void UpdateDictionaries()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.Buttons.Clear();
		for (int i = 0; i < this.ButtonKeys.Count; i++)
		{
			this.Buttons.Add(this.ButtonKeys[i], this.ButtonValues[i]);
		}
		this.Axis.Clear();
		for (int j = 0; j < this.AxisKeys.Count; j++)
		{
			this.Axis.Add(this.AxisKeys[j], new vp_Input.vp_InputAxis
			{
				Positive = this.AxisValues[j].Positive,
				Negative = this.AxisValues[j].Negative
			});
		}
	}

	// Token: 0x0600964C RID: 38476 RVA: 0x003BC8CC File Offset: 0x003BAACC
	public static bool GetButtonAny(string button)
	{
		return vp_Input.Instance.DoGetButtonAny(button);
	}

	// Token: 0x0600964D RID: 38477 RVA: 0x003BC8DC File Offset: 0x003BAADC
	public virtual bool DoGetButtonAny(string button)
	{
		if (this.Buttons.ContainsKey(button))
		{
			return Input.GetKey(this.Buttons[button]) || Input.GetKeyDown(this.Buttons[button]) || Input.GetKeyUp(this.Buttons[button]);
		}
		Debug.LogError("\"" + button + "\" is not in VP Input Manager's Buttons. You must add it for this Button to work.");
		return false;
	}

	// Token: 0x0600964E RID: 38478 RVA: 0x003BC947 File Offset: 0x003BAB47
	public static bool GetButton(string button)
	{
		return vp_Input.Instance.DoGetButton(button);
	}

	// Token: 0x0600964F RID: 38479 RVA: 0x003BC954 File Offset: 0x003BAB54
	public virtual bool DoGetButton(string button)
	{
		if (this.Buttons.ContainsKey(button))
		{
			return Input.GetKey(this.Buttons[button]);
		}
		Debug.LogError("\"" + button + "\" is not in VP Input Manager's Buttons. You must add it for this Button to work.");
		return false;
	}

	// Token: 0x06009650 RID: 38480 RVA: 0x003BC98C File Offset: 0x003BAB8C
	public static bool GetButtonDown(string button)
	{
		return vp_Input.Instance.DoGetButtonDown(button);
	}

	// Token: 0x06009651 RID: 38481 RVA: 0x003BC999 File Offset: 0x003BAB99
	public virtual bool DoGetButtonDown(string button)
	{
		if (this.Buttons.ContainsKey(button))
		{
			return Input.GetKeyDown(this.Buttons[button]);
		}
		Debug.LogError("\"" + button + "\" is not in VP Input Manager's Buttons. You must add it for this Button to work.");
		return false;
	}

	// Token: 0x06009652 RID: 38482 RVA: 0x003BC9D1 File Offset: 0x003BABD1
	public static bool GetButtonUp(string button)
	{
		return vp_Input.Instance.DoGetButtonUp(button);
	}

	// Token: 0x06009653 RID: 38483 RVA: 0x003BC9DE File Offset: 0x003BABDE
	public virtual bool DoGetButtonUp(string button)
	{
		if (this.Buttons.ContainsKey(button))
		{
			return Input.GetKeyUp(this.Buttons[button]);
		}
		Debug.LogError("\"" + button + "\" is not in VP Input Manager's Buttons. You must add it for this Button to work.");
		return false;
	}

	// Token: 0x06009654 RID: 38484 RVA: 0x003BCA16 File Offset: 0x003BAC16
	public static float GetAxisRaw(string axis)
	{
		return vp_Input.Instance.DoGetAxisRaw(axis);
	}

	// Token: 0x06009655 RID: 38485 RVA: 0x003BCA24 File Offset: 0x003BAC24
	public virtual float DoGetAxisRaw(string axis)
	{
		if (this.Axis.ContainsKey(axis) && this.ControlType == 0)
		{
			float result = 0f;
			if (Input.GetKey(this.Axis[axis].Positive))
			{
				result = 1f;
			}
			if (Input.GetKey(this.Axis[axis].Negative))
			{
				result = -1f;
			}
			return result;
		}
		if (this.UnityAxis.Contains(axis))
		{
			return Input.GetAxisRaw(axis);
		}
		Debug.LogError("\"" + axis + "\" is not in VP Input Manager's Unity Axis. You must add it for this Axis to work.");
		return 0f;
	}

	// Token: 0x06009656 RID: 38486 RVA: 0x003BCABC File Offset: 0x003BACBC
	public static void ChangeButtonKey(string button, KeyCode keyCode, bool save = false)
	{
		if (!vp_Input.Instance.Buttons.ContainsKey(button))
		{
			Debug.LogWarning("The Button \"" + button + "\" Doesn't Exist");
			return;
		}
		if (save)
		{
			vp_Input.Instance.ButtonValues[vp_Input.Instance.ButtonKeys.IndexOf(button)] = keyCode;
		}
		vp_Input.Instance.Buttons[button] = keyCode;
	}

	// Token: 0x06009657 RID: 38487 RVA: 0x003BCB28 File Offset: 0x003BAD28
	public static void ChangeAxis(string n, KeyCode pk = KeyCode.None, KeyCode nk = KeyCode.None, bool save = false)
	{
		if (!vp_Input.Instance.AxisKeys.Contains(n))
		{
			Debug.LogWarning("The Axis \"" + n + "\" Doesn't Exist");
			return;
		}
		if (save)
		{
			vp_Input.Instance.AxisValues[vp_Input.Instance.AxisKeys.IndexOf(n)] = new vp_Input.vp_InputAxis
			{
				Positive = pk,
				Negative = nk
			};
		}
		vp_Input.Instance.Axis[n] = new vp_Input.vp_InputAxis
		{
			Positive = pk,
			Negative = nk
		};
	}

	// Token: 0x04007259 RID: 29273
	public int ControlType;

	// Token: 0x0400725A RID: 29274
	public Dictionary<string, KeyCode> Buttons = new Dictionary<string, KeyCode>();

	// Token: 0x0400725B RID: 29275
	public List<string> ButtonKeys = new List<string>();

	// Token: 0x0400725C RID: 29276
	public List<KeyCode> ButtonValues = new List<KeyCode>();

	// Token: 0x0400725D RID: 29277
	public Dictionary<string, vp_Input.vp_InputAxis> Axis = new Dictionary<string, vp_Input.vp_InputAxis>();

	// Token: 0x0400725E RID: 29278
	public List<string> AxisKeys = new List<string>();

	// Token: 0x0400725F RID: 29279
	public List<vp_Input.vp_InputAxis> AxisValues = new List<vp_Input.vp_InputAxis>();

	// Token: 0x04007260 RID: 29280
	public List<string> UnityAxis = new List<string>();

	// Token: 0x04007261 RID: 29281
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static string m_FolderPath = "UltimateFPS/Content/Resources/Input";

	// Token: 0x04007262 RID: 29282
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static string m_PrefabPath = "Assets/UltimateFPS/Content/Resources/Input/vp_Input.prefab";

	// Token: 0x04007263 RID: 29283
	public static bool mIsDirty = true;

	// Token: 0x04007264 RID: 29284
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static vp_Input m_Instance;

	// Token: 0x020012D9 RID: 4825
	[Serializable]
	public class vp_InputAxis
	{
		// Token: 0x04007265 RID: 29285
		public KeyCode Positive;

		// Token: 0x04007266 RID: 29286
		public KeyCode Negative;
	}
}

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000F3C RID: 3900
public sealed class TransformDebug : MonoBehaviour
{
	// Token: 0x06007C28 RID: 31784 RVA: 0x00323F84 File Offset: 0x00322184
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		try
		{
			if (this.m_stack.Count <= 0)
			{
				GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
				for (int i = rootGameObjects.Length - 1; i >= 0; i--)
				{
					GameObject gameObject = rootGameObjects[i];
					if (gameObject)
					{
						this.PushTransform(gameObject.transform);
					}
				}
			}
			int num = 0;
			while (num < 256 && this.m_stack.Count > 0)
			{
				this.DoOperation();
				num++;
			}
		}
		finally
		{
		}
	}

	// Token: 0x06007C29 RID: 31785 RVA: 0x00324010 File Offset: 0x00322210
	[PublicizedFrom(EAccessModifier.Private)]
	public void PushTransform(Transform t)
	{
		if (!t)
		{
			return;
		}
		this.m_stack.Push(t);
	}

	// Token: 0x06007C2A RID: 31786 RVA: 0x00324028 File Offset: 0x00322228
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoOperation()
	{
		Transform transform = this.m_stack.Pop();
		if (!transform)
		{
			return;
		}
		TransformDebug.TransformState orCreateValue = this.m_states.GetOrCreateValue(transform);
		orCreateValue.Update(transform);
		string text = null;
		if (transform.localPosition != orCreateValue.Position)
		{
			text = this.DescribeTransformHierarchy(transform);
			Log.Error(string.Format("[{0}] Invalid local position {1} correcting to {2}: {3}", new object[]
			{
				"TransformDebug",
				transform.localPosition,
				orCreateValue.Position,
				text
			}));
			transform.localPosition = orCreateValue.Position;
		}
		if (transform.localRotation != orCreateValue.Rotation)
		{
			if (text == null)
			{
				text = this.DescribeTransformHierarchy(transform);
			}
			Log.Error(string.Format("[{0}] Invalid local rotation {1} correcting to {2}: {3}", new object[]
			{
				"TransformDebug",
				transform.localRotation,
				orCreateValue.Rotation,
				text
			}));
			transform.localRotation = orCreateValue.Rotation;
		}
		if (transform.localScale != orCreateValue.Scale)
		{
			if (text == null)
			{
				text = this.DescribeTransformHierarchy(transform);
			}
			Log.Error(string.Format("[{0}] Invalid local scale {1} correcting to {2}: {3}", new object[]
			{
				"TransformDebug",
				transform.localScale,
				orCreateValue.Scale,
				text
			}));
			transform.localScale = orCreateValue.Scale;
		}
		int childCount = transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			this.PushTransform(child);
		}
	}

	// Token: 0x06007C2B RID: 31787 RVA: 0x003241BC File Offset: 0x003223BC
	[PublicizedFrom(EAccessModifier.Private)]
	public string DescribeTransformHierarchy(Transform t)
	{
		string result;
		try
		{
			this.m_describeStack.Push(t.name);
			Transform transform = t;
			while (transform.parent)
			{
				transform = transform.parent;
				this.m_describeStack.Push(transform.name);
			}
			while (this.m_describeStack.Count > 0)
			{
				if (this.m_describeBuilder.Length > 0)
				{
					this.m_describeBuilder.Append('/');
				}
				this.m_describeBuilder.Append(this.m_describeStack.Pop());
			}
			result = this.m_describeBuilder.ToString();
		}
		finally
		{
			this.m_describeStack.Clear();
			this.m_describeBuilder.Clear();
		}
		return result;
	}

	// Token: 0x06007C2C RID: 31788 RVA: 0x00324280 File Offset: 0x00322480
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool IsValid(float f)
	{
		return float.IsFinite(f);
	}

	// Token: 0x06007C2D RID: 31789 RVA: 0x00324288 File Offset: 0x00322488
	public static void Test()
	{
		Scene activeScene = SceneManager.GetActiveScene();
		Stack<Transform> stack = new Stack<Transform>();
		foreach (GameObject gameObject in activeScene.GetRootGameObjects())
		{
			if (gameObject && gameObject.name.EqualsCaseInsensitive("Entities"))
			{
				foreach (object obj in gameObject.transform)
				{
					Transform item = (Transform)obj;
					stack.Push(item);
				}
			}
		}
		if (stack.Count == 0)
		{
			Log.Error("No entities found (try testing while in-game).");
		}
		while (stack.Count > 0)
		{
			Transform transform = stack.Pop();
			if (transform)
			{
				transform.localPosition = new Vector3(TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localPosition.x), TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localPosition.y), TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localPosition.z));
				transform.localRotation = new Quaternion(TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localRotation.x), TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localRotation.y), TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localRotation.z), TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localRotation.w));
				transform.localScale = new Vector3(TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localScale.x), TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localScale.y), TransformDebug.<Test>g__MaybeCorruptFloat|10_0(transform.localScale.z));
				foreach (object obj2 in transform)
				{
					Transform item2 = (Transform)obj2;
					stack.Push(item2);
				}
			}
		}
	}

	// Token: 0x06007C2F RID: 31791 RVA: 0x003244B8 File Offset: 0x003226B8
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static float <Test>g__MaybeCorruptFloat|10_0(float originalValue)
	{
		if (UnityEngine.Random.value >= 0.05f)
		{
			return originalValue;
		}
		float value = UnityEngine.Random.value;
		float result;
		if (value >= 0.33333334f)
		{
			if (value >= 0.6666667f)
			{
				result = float.NaN;
			}
			else
			{
				result = float.PositiveInfinity;
			}
		}
		else
		{
			result = float.NegativeInfinity;
		}
		return result;
	}

	// Token: 0x04005EED RID: 24301
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int MAX_OPERATIONS_PER_FRAME = 256;

	// Token: 0x04005EEE RID: 24302
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly ConditionalWeakTable<Transform, TransformDebug.TransformState> m_states = new ConditionalWeakTable<Transform, TransformDebug.TransformState>();

	// Token: 0x04005EEF RID: 24303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Stack<Transform> m_stack = new Stack<Transform>();

	// Token: 0x04005EF0 RID: 24304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly Stack<string> m_describeStack = new Stack<string>();

	// Token: 0x04005EF1 RID: 24305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly StringBuilder m_describeBuilder = new StringBuilder();

	// Token: 0x02000F3D RID: 3901
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class TransformState
	{
		// Token: 0x06007C30 RID: 31792 RVA: 0x00324504 File Offset: 0x00322704
		public void Update(Transform t)
		{
			Vector3 localPosition = t.localPosition;
			this.Position = new Vector3(TransformDebug.IsValid(localPosition.x) ? localPosition.x : this.Position.x, TransformDebug.IsValid(localPosition.y) ? localPosition.y : this.Position.y, TransformDebug.IsValid(localPosition.z) ? localPosition.z : this.Position.z);
			Quaternion localRotation = t.localRotation;
			this.Rotation = new Quaternion(TransformDebug.IsValid(localRotation.x) ? localRotation.x : this.Rotation.x, TransformDebug.IsValid(localRotation.y) ? localRotation.y : this.Rotation.y, TransformDebug.IsValid(localRotation.z) ? localRotation.z : this.Rotation.z, TransformDebug.IsValid(localRotation.w) ? localRotation.w : this.Rotation.w);
			if (this.Rotation == TransformDebug.TransformState.zeroQuaternion)
			{
				this.Rotation = Quaternion.identity;
			}
			Vector3 localScale = t.localScale;
			this.Scale = new Vector3(TransformDebug.IsValid(localScale.x) ? localScale.x : this.Scale.x, TransformDebug.IsValid(localScale.y) ? localScale.y : this.Scale.y, TransformDebug.IsValid(localScale.z) ? localScale.z : this.Scale.z);
		}

		// Token: 0x04005EF2 RID: 24306
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Quaternion zeroQuaternion = new Quaternion(0f, 0f, 0f, 0f);

		// Token: 0x04005EF3 RID: 24307
		public Vector3 Position = Vector3.zero;

		// Token: 0x04005EF4 RID: 24308
		public Quaternion Rotation = Quaternion.identity;

		// Token: 0x04005EF5 RID: 24309
		public Vector3 Scale = Vector3.one;
	}
}

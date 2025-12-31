using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200132E RID: 4910
public class vp_FootstepManager : MonoBehaviour
{
	// Token: 0x17000F96 RID: 3990
	// (get) Token: 0x060098A4 RID: 39076 RVA: 0x003CB0E8 File Offset: 0x003C92E8
	public static vp_FootstepManager[] FootstepManagers
	{
		get
		{
			if (vp_FootstepManager.mIsDirty)
			{
				vp_FootstepManager.mIsDirty = false;
				vp_FootstepManager.m_FootstepManagers = (UnityEngine.Object.FindObjectsOfType(typeof(vp_FootstepManager)) as vp_FootstepManager[]);
				if (vp_FootstepManager.m_FootstepManagers == null)
				{
					vp_FootstepManager.m_FootstepManagers = (Resources.FindObjectsOfTypeAll(typeof(vp_FootstepManager)) as vp_FootstepManager[]);
				}
			}
			return vp_FootstepManager.m_FootstepManagers;
		}
	}

	// Token: 0x17000F97 RID: 3991
	// (get) Token: 0x060098A5 RID: 39077 RVA: 0x003CB140 File Offset: 0x003C9340
	public bool IsDirty
	{
		get
		{
			return vp_FootstepManager.mIsDirty;
		}
	}

	// Token: 0x060098A6 RID: 39078 RVA: 0x003CB148 File Offset: 0x003C9348
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.m_Player = base.transform.root.GetComponentInChildren<vp_FPPlayerEventHandler>();
		this.m_Camera = base.transform.root.GetComponentInChildren<vp_FPCamera>();
		this.m_Controller = base.transform.root.GetComponentInChildren<vp_FPController>();
		this.m_Audio = base.gameObject.AddComponent<AudioSource>();
	}

	// Token: 0x060098A7 RID: 39079 RVA: 0x003CB1A8 File Offset: 0x003C93A8
	public virtual void SetDirty(bool dirty)
	{
		vp_FootstepManager.mIsDirty = dirty;
	}

	// Token: 0x060098A8 RID: 39080 RVA: 0x003CB1B0 File Offset: 0x003C93B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.m_Camera.BobStepCallback == null)
		{
			vp_FPCamera camera = this.m_Camera;
			camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
		}
	}

	// Token: 0x060098A9 RID: 39081 RVA: 0x003CB1E7 File Offset: 0x003C93E7
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
	}

	// Token: 0x060098AA RID: 39082 RVA: 0x003CB211 File Offset: 0x003C9411
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		vp_FPCamera camera = this.m_Camera;
		camera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(camera.BobStepCallback, new vp_FPCamera.BobStepDelegate(this.Footstep));
	}

	// Token: 0x060098AB RID: 39083 RVA: 0x003CB23C File Offset: 0x003C943C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Footstep()
	{
		if (!this.m_Controller.Grounded)
		{
			return;
		}
		if (this.m_Player.GroundTexture.Get() == null && this.m_Player.SurfaceType.Get() == null)
		{
			return;
		}
		if (this.m_Player.SurfaceType.Get() != null)
		{
			this.PlaySound(this.SurfaceTypes[this.m_Player.SurfaceType.Get().SurfaceID]);
			return;
		}
		foreach (vp_FootstepManager.vp_SurfaceTypes vp_SurfaceTypes in this.SurfaceTypes)
		{
			using (List<Texture>.Enumerator enumerator2 = vp_SurfaceTypes.Textures.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == this.m_Player.GroundTexture.Get())
					{
						this.PlaySound(vp_SurfaceTypes);
						break;
					}
				}
			}
		}
	}

	// Token: 0x060098AC RID: 39084 RVA: 0x003CB37C File Offset: 0x003C957C
	public virtual void PlaySound(vp_FootstepManager.vp_SurfaceTypes st)
	{
		if (st.Sounds == null || st.Sounds.Count == 0)
		{
			return;
		}
		for (;;)
		{
			this.m_SoundToPlay = st.Sounds[UnityEngine.Random.Range(0, st.Sounds.Count)];
			if (this.m_SoundToPlay == null)
			{
				break;
			}
			if (!(this.m_SoundToPlay == this.m_LastPlayedSound) || st.Sounds.Count <= 1)
			{
				goto IL_68;
			}
		}
		return;
		IL_68:
		this.m_Audio.pitch = UnityEngine.Random.Range(st.RandomPitch.x, st.RandomPitch.y) * Time.timeScale;
		this.m_Audio.clip = this.m_SoundToPlay;
		this.m_Audio.Play();
		this.m_LastPlayedSound = this.m_SoundToPlay;
	}

	// Token: 0x060098AD RID: 39085 RVA: 0x003CB448 File Offset: 0x003C9648
	public static int GetMainTerrainTexture(Vector3 worldPos, Terrain terrain)
	{
		TerrainData terrainData = terrain.terrainData;
		Vector3 position = terrain.transform.position;
		int x = (int)((worldPos.x - position.x) / terrainData.size.x * (float)terrainData.alphamapWidth);
		int y = (int)((worldPos.z - position.z) / terrainData.size.z * (float)terrainData.alphamapHeight);
		float[,,] alphamaps = terrainData.GetAlphamaps(x, y, 1, 1);
		float[] array = new float[alphamaps.GetUpperBound(2) + 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = alphamaps[0, 0, i];
		}
		float num = 0f;
		int result = 0;
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j] > num)
			{
				result = j;
				num = array[j];
			}
		}
		return result;
	}

	// Token: 0x04007525 RID: 29989
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static vp_FootstepManager[] m_FootstepManagers;

	// Token: 0x04007526 RID: 29990
	public static bool mIsDirty = true;

	// Token: 0x04007527 RID: 29991
	public List<vp_FootstepManager.vp_SurfaceTypes> SurfaceTypes = new List<vp_FootstepManager.vp_SurfaceTypes>();

	// Token: 0x04007528 RID: 29992
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;

	// Token: 0x04007529 RID: 29993
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPCamera m_Camera;

	// Token: 0x0400752A RID: 29994
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_FPController m_Controller;

	// Token: 0x0400752B RID: 29995
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioSource m_Audio;

	// Token: 0x0400752C RID: 29996
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioClip m_SoundToPlay;

	// Token: 0x0400752D RID: 29997
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public AudioClip m_LastPlayedSound;

	// Token: 0x0200132F RID: 4911
	[Serializable]
	public class vp_SurfaceTypes
	{
		// Token: 0x0400752E RID: 29998
		public Vector2 RandomPitch = new Vector2(1f, 1.5f);

		// Token: 0x0400752F RID: 29999
		public bool Foldout = true;

		// Token: 0x04007530 RID: 30000
		public bool SoundsFoldout = true;

		// Token: 0x04007531 RID: 30001
		public bool TexturesFoldout = true;

		// Token: 0x04007532 RID: 30002
		public string SurfaceName = "";

		// Token: 0x04007533 RID: 30003
		public List<AudioClip> Sounds = new List<AudioClip>();

		// Token: 0x04007534 RID: 30004
		public List<Texture> Textures = new List<Texture>();
	}
}

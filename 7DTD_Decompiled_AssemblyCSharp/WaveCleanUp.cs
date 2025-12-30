using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200037A RID: 890
public class WaveCleanUp : MonoBehaviour
{
	// Token: 0x06001A5F RID: 6751 RVA: 0x000A3E93 File Offset: 0x000A2093
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		base.StartCoroutine(this.FormatHeader());
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x000A3EA2 File Offset: 0x000A20A2
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator FormatHeader()
	{
		yield return new WaitUntil(() => this.FilePath != null);
		UnityWebRequest getAudioFile = UnityWebRequestMultimedia.GetAudioClip("file://" + this.FilePath, AudioType.WAV);
		getAudioFile.disposeDownloadHandlerOnDispose = true;
		yield return getAudioFile.SendWebRequest();
		if (getAudioFile.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log(getAudioFile.error);
		}
		else
		{
			AudioClip content = DownloadHandlerAudioClip.GetContent(getAudioFile);
			float[] array = new float[content.samples * content.channels];
			content.GetData(array, 0);
			byte[] array2 = WaveCleanUp.PCMDataToByteArray(array);
			int num = content.samples * (int)WaveCleanUp.Channels * (int)WaveCleanUp.BitsPerSample / 8;
			int value = 36 + num;
			using (Stream stream = SdFile.Open(this.FilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				stream.Write(WaveCleanUp.ChunkID, 0, 4);
				stream.Write(BitConverter.GetBytes(value), 0, 4);
				stream.Write(WaveCleanUp.Format, 0, 4);
				stream.Write(WaveCleanUp.Subchunk1ID, 0, 4);
				stream.Write(BitConverter.GetBytes(WaveCleanUp.Subchunk1Size), 0, 4);
				stream.Write(BitConverter.GetBytes(WaveCleanUp.AudioFormat), 0, 2);
				stream.Write(BitConverter.GetBytes(WaveCleanUp.Channels), 0, 2);
				stream.Write(BitConverter.GetBytes(WaveCleanUp.SampleRate), 0, 4);
				stream.Write(BitConverter.GetBytes(WaveCleanUp.ByteRate), 0, 4);
				stream.Write(BitConverter.GetBytes(WaveCleanUp.BlockAlign), 0, 2);
				stream.Write(BitConverter.GetBytes(WaveCleanUp.BitsPerSample), 0, 2);
				stream.Write(WaveCleanUp.Subchunk2ID, 0, 4);
				stream.Write(BitConverter.GetBytes(num), 0, 4);
				stream.Write(array2, 0, array2.Length);
			}
		}
		getAudioFile.Dispose();
		Log.Out("Cleaned up: " + this.FilePath);
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x000A3EB4 File Offset: 0x000A20B4
	[PublicizedFrom(EAccessModifier.Private)]
	public static byte[] PCMDataToByteArray(float[] _pcmData)
	{
		byte[] array = new byte[2 * _pcmData.Length];
		for (int i = 0; i < _pcmData.Length; i++)
		{
			byte[] bytes = BitConverter.GetBytes((short)(_pcmData[i] * 32767f));
			for (int j = 0; j < 2; j++)
			{
				array[2 * i + j] = bytes[j];
			}
		}
		return array;
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x000A3F01 File Offset: 0x000A2101
	public static GameObject Create()
	{
		if (WaveCleanUp.PrefabWaveCleanUp == null)
		{
			WaveCleanUp.PrefabWaveCleanUp = Resources.Load<GameObject>("Prefabs/prefabDMSWaveCleanup");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(WaveCleanUp.PrefabWaveCleanUp);
		gameObject.name = "WaveCleanUp";
		return gameObject;
	}

	// Token: 0x04001116 RID: 4374
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameObject PrefabWaveCleanUp;

	// Token: 0x04001117 RID: 4375
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static byte[] ChunkID = Encoding.ASCII.GetBytes("RIFF");

	// Token: 0x04001118 RID: 4376
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static byte[] Format = Encoding.ASCII.GetBytes("WAVE");

	// Token: 0x04001119 RID: 4377
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static byte[] Subchunk1ID = Encoding.ASCII.GetBytes("fmt ");

	// Token: 0x0400111A RID: 4378
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static byte[] Subchunk2ID = Encoding.ASCII.GetBytes("data");

	// Token: 0x0400111B RID: 4379
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static short AudioFormat = 1;

	// Token: 0x0400111C RID: 4380
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int SampleRate = 44100;

	// Token: 0x0400111D RID: 4381
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static short Channels = 2;

	// Token: 0x0400111E RID: 4382
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static short BitsPerSample = 16;

	// Token: 0x0400111F RID: 4383
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int ByteRate = WaveCleanUp.SampleRate * (int)WaveCleanUp.Channels * (int)WaveCleanUp.BitsPerSample / 8;

	// Token: 0x04001120 RID: 4384
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static short BlockAlign = WaveCleanUp.Channels * WaveCleanUp.BitsPerSample / 8;

	// Token: 0x04001121 RID: 4385
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int Subchunk1Size = 16;

	// Token: 0x04001122 RID: 4386
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool IsFinished;

	// Token: 0x04001123 RID: 4387
	public string FilePath;
}

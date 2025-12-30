using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200083B RID: 2107
public class PlayerInputRecordingSystem
{
	// Token: 0x17000635 RID: 1589
	// (get) Token: 0x06003C78 RID: 15480 RVA: 0x001852ED File Offset: 0x001834ED
	public static PlayerInputRecordingSystem Instance
	{
		get
		{
			if (PlayerInputRecordingSystem.mInstance == null)
			{
				PlayerInputRecordingSystem.mInstance = new PlayerInputRecordingSystem();
			}
			return PlayerInputRecordingSystem.mInstance;
		}
	}

	// Token: 0x06003C79 RID: 15481 RVA: 0x00002914 File Offset: 0x00000B14
	public void Record(MovementInput _movement, int _frameNr)
	{
	}

	// Token: 0x06003C7A RID: 15482 RVA: 0x00185308 File Offset: 0x00183508
	public void Record(EntityPlayer _player, ulong _ticks)
	{
		if (this.startTickTime == 0UL)
		{
			this.startTickTime = GameTimer.Instance.ticks;
		}
		this.recording.Add(new PlayerInputRecordingSystem.SPosRot
		{
			pos = _player.position,
			rot = _player.rotation,
			ticks = (int)(_ticks - this.startTickTime)
		});
	}

	// Token: 0x06003C7B RID: 15483 RVA: 0x0018536B File Offset: 0x0018356B
	public void Reset(bool _bClearRecordings = false)
	{
		this.index = 0;
		if (_bClearRecordings)
		{
			this.recording.Clear();
			this.startTickTime = 0UL;
		}
		this.relativeStartTickTime = 0UL;
		this.autoSaveFilename = null;
	}

	// Token: 0x06003C7C RID: 15484 RVA: 0x0018539C File Offset: 0x0018359C
	public bool Play(EntityPlayer _player, bool _bPlayRelativeToNow = false)
	{
		if (this.relativeStartTickTime == 0UL)
		{
			if (_bPlayRelativeToNow)
			{
				this.relativeStartTickTime = GameTimer.Instance.ticks;
			}
			else
			{
				this.relativeStartTickTime = this.startTickTime;
			}
			this.startFrameCount = Time.frameCount;
			this.startTime = Time.time;
		}
		while (this.index < this.recording.Count)
		{
			int ticks = this.recording[this.index].ticks;
			if (GameTimer.Instance.ticks < this.relativeStartTickTime + (ulong)((long)ticks))
			{
				break;
			}
			_player.SetPosition(this.recording[this.index].pos, true);
			_player.SetRotation(this.recording[this.index].rot);
			this.index++;
		}
		if (this.index == this.recording.Count)
		{
			Log.Out("Playing ended. Frames=" + (Time.frameCount - this.startFrameCount).ToString() + " avg fps=" + ((float)(Time.frameCount - this.startFrameCount) / (Time.time - this.startTime)).ToString("0.0"));
			GameManager.Instance.SetConsoleWindowVisible(true);
			this.index++;
		}
		return this.index < this.recording.Count;
	}

	// Token: 0x06003C7D RID: 15485 RVA: 0x00185504 File Offset: 0x00183704
	public void SetStartPosition(EntityPlayer _player)
	{
		if (this.recording.Count > 0)
		{
			_player.SetPosition(this.recording[0].pos, true);
			_player.SetRotation(this.recording[0].rot);
		}
	}

	// Token: 0x06003C7E RID: 15486 RVA: 0x00185544 File Offset: 0x00183744
	public void Load(string _filename)
	{
		using (BinaryReader binaryReader = new BinaryReader(SdFile.OpenRead(GameIO.GetSaveGameDir() + "/" + _filename + ".rec")))
		{
			this.recording.Clear();
			binaryReader.ReadByte();
			this.startTickTime = binaryReader.ReadUInt64();
			int num = (int)binaryReader.ReadUInt32();
			for (int i = 0; i < num; i++)
			{
				PlayerInputRecordingSystem.SPosRot item = default(PlayerInputRecordingSystem.SPosRot);
				item.Read(binaryReader);
				this.recording.Add(item);
			}
		}
	}

	// Token: 0x06003C7F RID: 15487 RVA: 0x001855DC File Offset: 0x001837DC
	public void SetAutoSaveTo(string _filename)
	{
		this.autoSaveFilename = _filename;
	}

	// Token: 0x06003C80 RID: 15488 RVA: 0x001855E5 File Offset: 0x001837E5
	public bool AutoSave()
	{
		if (this.autoSaveFilename != null)
		{
			this.doSave(this.autoSaveFilename);
			this.autoSaveFilename = null;
			return true;
		}
		return false;
	}

	// Token: 0x06003C81 RID: 15489 RVA: 0x00185605 File Offset: 0x00183805
	public void Save(string _filename)
	{
		this.doSave(GameIO.GetSaveGameDir() + "/" + _filename);
	}

	// Token: 0x06003C82 RID: 15490 RVA: 0x00185620 File Offset: 0x00183820
	[PublicizedFrom(EAccessModifier.Private)]
	public void doSave(string _filename)
	{
		using (BinaryWriter binaryWriter = new BinaryWriter(SdFile.Open(_filename + ".rec", FileMode.Create, FileAccess.Write, FileShare.Read)))
		{
			binaryWriter.Write(1);
			binaryWriter.Write(this.startTickTime);
			binaryWriter.Write((uint)this.recording.Count);
			for (int i = 0; i < this.recording.Count; i++)
			{
				this.recording[i].Write(binaryWriter);
			}
		}
	}

	// Token: 0x040030F1 RID: 12529
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CurrentSaveVersion = 1;

	// Token: 0x040030F2 RID: 12530
	[PublicizedFrom(EAccessModifier.Private)]
	public static PlayerInputRecordingSystem mInstance;

	// Token: 0x040030F3 RID: 12531
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PlayerInputRecordingSystem.SPosRot> recording = new List<PlayerInputRecordingSystem.SPosRot>();

	// Token: 0x040030F4 RID: 12532
	[PublicizedFrom(EAccessModifier.Private)]
	public int index;

	// Token: 0x040030F5 RID: 12533
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong startTickTime;

	// Token: 0x040030F6 RID: 12534
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong relativeStartTickTime;

	// Token: 0x040030F7 RID: 12535
	[PublicizedFrom(EAccessModifier.Private)]
	public int startFrameCount;

	// Token: 0x040030F8 RID: 12536
	[PublicizedFrom(EAccessModifier.Private)]
	public float startTime;

	// Token: 0x040030F9 RID: 12537
	[PublicizedFrom(EAccessModifier.Private)]
	public string autoSaveFilename;

	// Token: 0x0200083C RID: 2108
	[PublicizedFrom(EAccessModifier.Private)]
	public struct SPosRot
	{
		// Token: 0x06003C84 RID: 15492 RVA: 0x001856C7 File Offset: 0x001838C7
		public void Write(BinaryWriter _bw)
		{
			_bw.Write(this.ticks);
			StreamUtils.Write(_bw, this.pos);
			StreamUtils.Write(_bw, this.rot);
		}

		// Token: 0x06003C85 RID: 15493 RVA: 0x001856ED File Offset: 0x001838ED
		public void Read(BinaryReader _br)
		{
			this.ticks = _br.ReadInt32();
			this.pos = StreamUtils.ReadVector3(_br);
			this.rot = StreamUtils.ReadVector3(_br);
		}

		// Token: 0x040030FA RID: 12538
		public Vector3 pos;

		// Token: 0x040030FB RID: 12539
		public Vector3 rot;

		// Token: 0x040030FC RID: 12540
		public int ticks;
	}
}

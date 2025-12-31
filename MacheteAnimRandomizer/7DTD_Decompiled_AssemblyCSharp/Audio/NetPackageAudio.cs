using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Audio
{
	// Token: 0x020017A7 RID: 6055
	[Preserve]
	public class NetPackageAudio : NetPackage
	{
		// Token: 0x0600B53A RID: 46394 RVA: 0x00461812 File Offset: 0x0045FA12
		public NetPackageAudio Setup(int _playOnEntityId, string _soundGroupName, float _occlusion, bool _play, bool _signalOnly = false)
		{
			this.playOnEntity = true;
			this.playOnEntityId = _playOnEntityId;
			this.soundGroupName = _soundGroupName;
			this.play = _play;
			this.occlusion = _occlusion;
			this.signalOnly = _signalOnly;
			return this;
		}

		// Token: 0x0600B53B RID: 46395 RVA: 0x00461841 File Offset: 0x0045FA41
		public NetPackageAudio Setup(Vector3 _position, string _soundGroupName, float _occlusion, bool _play, int entityId = -1)
		{
			this.playOnEntity = false;
			this.position = _position;
			this.playOnEntityId = entityId;
			this.soundGroupName = _soundGroupName;
			this.play = _play;
			this.occlusion = _occlusion;
			return this;
		}

		// Token: 0x0600B53C RID: 46396 RVA: 0x00461870 File Offset: 0x0045FA70
		public override void read(PooledBinaryReader _reader)
		{
			this.playOnEntityId = _reader.ReadInt32();
			this.soundGroupName = _reader.ReadString();
			this.play = _reader.ReadBoolean();
			float x = _reader.ReadSingle();
			float y = _reader.ReadSingle();
			float z = _reader.ReadSingle();
			this.position.x = x;
			this.position.y = y;
			this.position.z = z;
			this.playOnEntity = _reader.ReadBoolean();
			this.occlusion = _reader.ReadSingle();
			this.signalOnly = _reader.ReadBoolean();
		}

		// Token: 0x0600B53D RID: 46397 RVA: 0x00461900 File Offset: 0x0045FB00
		public override void write(PooledBinaryWriter _writer)
		{
			base.write(_writer);
			_writer.Write(this.playOnEntityId);
			_writer.Write((this.soundGroupName != null) ? this.soundGroupName : "");
			_writer.Write(this.play);
			_writer.Write(this.position.x);
			_writer.Write(this.position.y);
			_writer.Write(this.position.z);
			_writer.Write(this.playOnEntity);
			_writer.Write(this.occlusion);
			_writer.Write(this.signalOnly);
		}

		// Token: 0x0600B53E RID: 46398 RVA: 0x004619A4 File Offset: 0x0045FBA4
		public override void ProcessPackage(World _world, GameManager _callbacks)
		{
			if (_world == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.soundGroupName))
			{
				return;
			}
			if (this.playOnEntity && this.playOnEntityId >= 0)
			{
				Entity entity = _world.GetEntity(this.playOnEntityId);
				if (entity == null)
				{
					return;
				}
				if (GameManager.IsDedicatedServer && Manager.ServerAudio != null)
				{
					if (this.play)
					{
						Manager.ServerAudio.Play(entity, this.soundGroupName, this.occlusion, this.signalOnly);
						return;
					}
					Manager.ServerAudio.Stop(this.playOnEntityId, this.soundGroupName);
					return;
				}
				else if (!GameManager.IsDedicatedServer && Manager.ServerAudio != null)
				{
					if (this.play)
					{
						Manager.Play(entity, this.soundGroupName, 1f, false);
						Manager.ServerAudio.Play(entity, this.soundGroupName, this.occlusion, this.signalOnly);
						return;
					}
					Manager.Stop(this.playOnEntityId, this.soundGroupName);
					Manager.ServerAudio.Stop(this.playOnEntityId, this.soundGroupName);
					return;
				}
				else if (Manager.ServerAudio == null)
				{
					if (!this.play)
					{
						Manager.Stop(this.playOnEntityId, this.soundGroupName);
						return;
					}
					if (!this.signalOnly)
					{
						Manager.Play(entity, this.soundGroupName, 1f, false);
						return;
					}
				}
			}
			else if (GameManager.IsDedicatedServer && Manager.ServerAudio != null)
			{
				if (this.play)
				{
					Manager.ServerAudio.Play(this.position, this.soundGroupName, this.occlusion, this.playOnEntityId);
					return;
				}
				Manager.ServerAudio.Stop(this.position, this.soundGroupName);
				return;
			}
			else if (!GameManager.IsDedicatedServer && Manager.ServerAudio != null)
			{
				if (this.play)
				{
					Manager.Play(this.position, this.soundGroupName, this.playOnEntityId, false);
					Manager.ServerAudio.Play(this.position, this.soundGroupName, this.occlusion, this.playOnEntityId);
					return;
				}
				Manager.Stop(this.position, this.soundGroupName);
				Manager.ServerAudio.Stop(this.position, this.soundGroupName);
				return;
			}
			else if (Manager.ServerAudio == null)
			{
				if (this.play)
				{
					Manager.Play(this.position, this.soundGroupName, this.playOnEntityId, false);
					return;
				}
				Manager.Stop(this.position, this.soundGroupName);
			}
		}

		// Token: 0x0600B53F RID: 46399 RVA: 0x000768A9 File Offset: 0x00074AA9
		public override int GetLength()
		{
			return 10;
		}

		// Token: 0x04008DFD RID: 36349
		public int playOnEntityId;

		// Token: 0x04008DFE RID: 36350
		public string soundGroupName;

		// Token: 0x04008DFF RID: 36351
		public bool play;

		// Token: 0x04008E00 RID: 36352
		public Vector3 position;

		// Token: 0x04008E01 RID: 36353
		public bool playOnEntity;

		// Token: 0x04008E02 RID: 36354
		public float occlusion;

		// Token: 0x04008E03 RID: 36355
		public bool signalOnly;
	}
}

using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;

// Token: 0x02000AFF RID: 2815
public class TileEntityPowered : TileEntity, IPowered
{
	// Token: 0x170008A1 RID: 2209
	// (get) Token: 0x060056DD RID: 22237 RVA: 0x00235B8C File Offset: 0x00233D8C
	// (set) Token: 0x060056DE RID: 22238 RVA: 0x00235BE0 File Offset: 0x00233DE0
	public int RequiredPower
	{
		get
		{
			if (this.needBlockData && !SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				ushort valuesFromBlock = (ushort)GameManager.Instance.World.GetBlock(base.ToWorldPos()).type;
				this.SetValuesFromBlock(valuesFromBlock);
				this.needBlockData = false;
			}
			return this.requiredPower;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			this.requiredPower = value;
		}
	}

	// Token: 0x170008A2 RID: 2210
	// (get) Token: 0x060056DF RID: 22239 RVA: 0x00235BE9 File Offset: 0x00233DE9
	public virtual int PowerUsed
	{
		get
		{
			return this.RequiredPower;
		}
	}

	// Token: 0x170008A3 RID: 2211
	// (get) Token: 0x060056E0 RID: 22240 RVA: 0x00235BF1 File Offset: 0x00233DF1
	public int ChildCount
	{
		get
		{
			return this.wireDataList.Count;
		}
	}

	// Token: 0x170008A4 RID: 2212
	// (get) Token: 0x060056E1 RID: 22241 RVA: 0x00235BFE File Offset: 0x00233DFE
	// (set) Token: 0x060056E2 RID: 22242 RVA: 0x00235C06 File Offset: 0x00233E06
	public bool IsPlayerPlaced
	{
		get
		{
			return this.isPlayerPlaced;
		}
		set
		{
			this.isPlayerPlaced = value;
			this.setModified();
		}
	}

	// Token: 0x170008A5 RID: 2213
	// (get) Token: 0x060056E3 RID: 22243 RVA: 0x00235C15 File Offset: 0x00233E15
	public bool IsPowered
	{
		get
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return this.PowerItem != null && this.PowerItem.IsPowered;
			}
			return this.isPowered;
		}
	}

	// Token: 0x170008A6 RID: 2214
	// (get) Token: 0x060056E4 RID: 22244 RVA: 0x00235C3F File Offset: 0x00233E3F
	// (set) Token: 0x060056E5 RID: 22245 RVA: 0x00235C48 File Offset: 0x00233E48
	public Transform BlockTransform
	{
		get
		{
			return this.blockTransform;
		}
		set
		{
			this.blockTransform = value;
			BlockValue block = GameManager.Instance.World.GetBlock(base.ToWorldPos());
			if (this.blockTransform != null)
			{
				Transform transform = this.blockTransform.Find("WireOffset");
				if (transform != null)
				{
					Vector3 wireOffset = block.Block.shape.GetRotation(block) * transform.localPosition;
					this.WireOffset = wireOffset;
					return;
				}
			}
			if (block.Block.Properties.Values.ContainsKey("WireOffset"))
			{
				Vector3 wireOffset2 = block.Block.shape.GetRotation(block) * StringParsers.ParseVector3(block.Block.Properties.Values["WireOffset"], 0, -1);
				this.WireOffset = wireOffset2;
			}
		}
	}

	// Token: 0x060056E6 RID: 22246 RVA: 0x00235D20 File Offset: 0x00233F20
	public TileEntityPowered(Chunk _chunk) : base(_chunk)
	{
	}

	// Token: 0x060056E7 RID: 22247 RVA: 0x00235D81 File Offset: 0x00233F81
	public Vector3i GetParent()
	{
		return this.parentPosition;
	}

	// Token: 0x060056E8 RID: 22248 RVA: 0x00235D89 File Offset: 0x00233F89
	public bool HasParent()
	{
		return this.parentPosition.y != -9999;
	}

	// Token: 0x060056E9 RID: 22249 RVA: 0x00235DA0 File Offset: 0x00233FA0
	public PowerItem GetPowerItem()
	{
		return this.PowerItem;
	}

	// Token: 0x060056EA RID: 22250 RVA: 0x00235DA8 File Offset: 0x00233FA8
	public override void OnReadComplete()
	{
		base.OnReadComplete();
		this.InitializePowerData();
		this.CheckForNewWires();
	}

	// Token: 0x060056EB RID: 22251 RVA: 0x00235DBC File Offset: 0x00233FBC
	public override void read(PooledBinaryReader _br, TileEntity.StreamModeRead _eStreamMode)
	{
		base.read(_br, _eStreamMode);
		int num = _br.ReadInt32();
		this.isPlayerPlaced = _br.ReadBoolean();
		this.PowerItemType = (PowerItem.PowerItemTypes)_br.ReadByte();
		this.needBlockData = true;
		int num2 = (int)_br.ReadByte();
		this.wireDataList.Clear();
		for (int i = 0; i < num2; i++)
		{
			Vector3i item = StreamUtils.ReadVector3i(_br);
			this.wireDataList.Add(item);
		}
		this.parentPosition = StreamUtils.ReadVector3i(_br);
		if (_eStreamMode == TileEntity.StreamModeRead.FromServer)
		{
			this.isPowered = _br.ReadBoolean();
		}
		this.activateDirty = true;
		this.wiresDirty = true;
		if (num > 0)
		{
			if (_eStreamMode == TileEntity.StreamModeRead.FromServer)
			{
				bool flag = false;
				if (LocalPlayerUI.GetUIForPrimaryPlayer().windowManager.HasWindow(XUiC_PowerCameraWindowGroup.ID))
				{
					XUiC_PowerCameraWindowGroup xuiC_PowerCameraWindowGroup = (XUiC_PowerCameraWindowGroup)((XUiWindowGroup)LocalPlayerUI.GetUIForPrimaryPlayer().windowManager.GetWindow(XUiC_PowerCameraWindowGroup.ID)).Controller;
					flag = (base.IsUserAccessing() && xuiC_PowerCameraWindowGroup != null && xuiC_PowerCameraWindowGroup.TileEntity == this);
				}
				if (!flag)
				{
					this.CenteredPitch = _br.ReadSingle();
					this.CenteredYaw = _br.ReadSingle();
					return;
				}
				_br.ReadSingle();
				_br.ReadSingle();
				return;
			}
			else
			{
				this.CenteredPitch = _br.ReadSingle();
				this.CenteredYaw = _br.ReadSingle();
			}
		}
	}

	// Token: 0x060056EC RID: 22252 RVA: 0x00235F00 File Offset: 0x00234100
	public override void write(PooledBinaryWriter _bw, TileEntity.StreamModeWrite _eStreamMode)
	{
		base.write(_bw, _eStreamMode);
		_bw.Write(1);
		_bw.Write(this.isPlayerPlaced);
		_bw.Write((byte)this.PowerItemType);
		_bw.Write((byte)this.wireDataList.Count);
		for (int i = 0; i < this.wireDataList.Count; i++)
		{
			StreamUtils.Write(_bw, this.wireDataList[i]);
		}
		StreamUtils.Write(_bw, this.parentPosition);
		if (_eStreamMode == TileEntity.StreamModeWrite.ToClient)
		{
			_bw.Write(this.IsPowered);
		}
		_bw.Write(this.CenteredPitch);
		_bw.Write(this.CenteredYaw);
	}

	// Token: 0x060056ED RID: 22253 RVA: 0x00235FA4 File Offset: 0x002341A4
	public void CheckForNewWires()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			for (int i = 0; i < this.wireDataList.Count; i++)
			{
				Vector3 childPosition = this.wireDataList[i].ToVector3();
				if (this.PowerItem.GetChild(childPosition) == null)
				{
					PowerItem powerItemByWorldPos = PowerManager.Instance.GetPowerItemByWorldPos(this.wireDataList[i]);
					PowerManager.Instance.SetParent(powerItemByWorldPos, this.PowerItem);
				}
			}
		}
	}

	// Token: 0x060056EE RID: 22254 RVA: 0x0023602C File Offset: 0x0023422C
	public void DrawWires()
	{
		if (this.BlockTransform == null)
		{
			this.wiresDirty = true;
			return;
		}
		WireManager instance = WireManager.Instance;
		bool flag = instance.ShowPulse;
		bool wiresShowing = instance.WiresShowing;
		if (this.wireDataList.Count > 0)
		{
			World world = GameManager.Instance.World;
			if (flag)
			{
				flag = world.CanPlaceBlockAt(base.ToWorldPos(), world.gameManager.GetPersistentLocalPlayer(), false);
			}
		}
		for (int i = 0; i < this.wireDataList.Count; i++)
		{
			Vector3i blockPos = this.wireDataList[i];
			Chunk chunk = GameManager.Instance.World.GetChunkFromWorldPos(blockPos) as Chunk;
			if (chunk != null)
			{
				TileEntityPowered tileEntityPowered = GameManager.Instance.World.GetTileEntity(chunk.ClrIdx, blockPos) as TileEntityPowered;
				bool flag2 = false;
				if (tileEntityPowered != null && tileEntityPowered.BlockTransform != null)
				{
					flag2 = true;
				}
				if (!flag2)
				{
					this.wiresDirty = true;
					return;
				}
			}
		}
		int num = 0;
		for (int j = 0; j < this.wireDataList.Count; j++)
		{
			Vector3i blockPos2 = this.wireDataList[j];
			Chunk chunk2 = GameManager.Instance.World.GetChunkFromWorldPos(blockPos2) as Chunk;
			if (chunk2 != null)
			{
				TileEntityPowered tileEntityPowered2 = GameManager.Instance.World.GetTileEntity(chunk2.ClrIdx, blockPos2) as TileEntityPowered;
				if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || !GameManager.IsDedicatedServer || tileEntityPowered2 == null || (this.PowerItemType == PowerItem.PowerItemTypes.TripWireRelay && tileEntityPowered2.PowerItemType == PowerItem.PowerItemTypes.TripWireRelay))
				{
					if (num >= this.currentWireNodes.Count)
					{
						IWireNode wireNodeFromPool = WireManager.Instance.GetWireNodeFromPool();
						this.currentWireNodes.Add(wireNodeFromPool);
					}
					this.currentWireNodes[num].SetStartPosition(this.BlockTransform.position + Origin.position);
					this.currentWireNodes[num].SetStartPositionOffset(this.WireOffset);
					if (tileEntityPowered2 != null)
					{
						if (this.PowerItemType == PowerItem.PowerItemTypes.ElectricWireRelay && tileEntityPowered2.PowerItemType == PowerItem.PowerItemTypes.ElectricWireRelay)
						{
							this.currentWireNodes[num].SetPulseColor(new Color32(0, 97, byte.MaxValue, byte.MaxValue));
							this.currentWireNodes[num].SetWireRadius(0.005f);
							this.currentWireNodes[num].SetWireDip(0f);
							ElectricWireController electricWireController = this.currentWireNodes[num].GetGameObject().GetComponent<ElectricWireController>();
							if (electricWireController == null)
							{
								electricWireController = this.currentWireNodes[num].GetGameObject().AddComponent<ElectricWireController>();
							}
							electricWireController.TileEntityParent = (this as TileEntityPoweredMeleeTrap);
							electricWireController.TileEntityChild = (tileEntityPowered2 as TileEntityPoweredMeleeTrap);
							electricWireController.WireNode = this.currentWireNodes[num];
							electricWireController.Init(this.chunk.GetBlock(base.localChunkPos).Block.Properties);
							electricWireController.WireNode.SetWireCanHide(false);
						}
						else if (this.PowerItemType == PowerItem.PowerItemTypes.TripWireRelay && tileEntityPowered2.PowerItemType == PowerItem.PowerItemTypes.TripWireRelay)
						{
							this.currentWireNodes[num].SetPulseColor(Color.magenta);
							this.currentWireNodes[num].SetWireRadius(0.0035f);
							this.currentWireNodes[num].SetWireDip(0f);
							TripWireController tripWireController = this.currentWireNodes[num].GetGameObject().GetComponent<TripWireController>();
							if (tripWireController == null)
							{
								tripWireController = this.currentWireNodes[num].GetGameObject().AddComponent<TripWireController>();
							}
							tripWireController.TileEntityParent = (this as TileEntityPoweredTrigger);
							tripWireController.TileEntityChild = (tileEntityPowered2 as TileEntityPoweredTrigger);
							tripWireController.WireNode = this.currentWireNodes[num];
							tripWireController.WireNode.SetWireCanHide(false);
						}
						else
						{
							UnityEngine.Object.Destroy(this.currentWireNodes[num].GetGameObject().GetComponent<ElectricWireController>());
							UnityEngine.Object.Destroy(this.currentWireNodes[num].GetGameObject().GetComponent<TripWireController>());
							this.currentWireNodes[num].SetWireCanHide(true);
						}
					}
					this.currentWireNodes[num].SetEndPosition(blockPos2.ToVector3());
					if (tileEntityPowered2 != null)
					{
						this.currentWireNodes[num].SetEndPositionOffset(tileEntityPowered2.WireOffset + new Vector3(0.5f, 0.5f, 0.5f));
					}
					this.currentWireNodes[num].BuildMesh();
					this.currentWireNodes[num].TogglePulse(flag);
					this.currentWireNodes[num].SetVisible(wiresShowing);
					num++;
				}
			}
		}
		for (int k = num; k < this.currentWireNodes.Count; k++)
		{
			IWireNode wireNode = this.currentWireNodes[num];
			WireManager.Instance.ReturnToPool(wireNode);
			this.currentWireNodes.Remove(wireNode);
		}
		this.wiresDirty = false;
	}

	// Token: 0x060056EF RID: 22255 RVA: 0x00236528 File Offset: 0x00234728
	public void AddWireData(Vector3i child)
	{
		this.wireDataList.Add(child);
		this.SendWireData();
	}

	// Token: 0x060056F0 RID: 22256 RVA: 0x0023653C File Offset: 0x0023473C
	public void SendWireData()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageWireActions>().Setup(NetPackageWireActions.WireActions.SendWires, base.ToWorldPos(), this.wireDataList, -1), false, -1, -1, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireActions>().Setup(NetPackageWireActions.WireActions.SendWires, base.ToWorldPos(), this.wireDataList, -1), false);
	}

	// Token: 0x060056F1 RID: 22257 RVA: 0x002365B0 File Offset: 0x002347B0
	public void CreateWireDataFromPowerItem()
	{
		this.wireDataList.Clear();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			for (int i = 0; i < this.PowerItem.Children.Count; i++)
			{
				this.wireDataList.Add(this.PowerItem.Children[i].Position);
			}
		}
	}

	// Token: 0x060056F2 RID: 22258 RVA: 0x00236610 File Offset: 0x00234810
	public void RemoveWires()
	{
		for (int i = 0; i < this.currentWireNodes.Count; i++)
		{
			WireManager.Instance.ReturnToPool(this.currentWireNodes[i]);
		}
		this.currentWireNodes.Clear();
	}

	// Token: 0x060056F3 RID: 22259 RVA: 0x00236654 File Offset: 0x00234854
	public void MarkWireDirty()
	{
		this.wiresDirty = true;
	}

	// Token: 0x060056F4 RID: 22260 RVA: 0x0023665D File Offset: 0x0023485D
	public void MarkChanged()
	{
		base.SetModified();
	}

	// Token: 0x060056F5 RID: 22261 RVA: 0x00236668 File Offset: 0x00234868
	public void InitializePowerData()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		ushort num = (ushort)GameManager.Instance.World.GetBlock(base.ToWorldPos()).type;
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.PowerItem = PowerManager.Instance.GetPowerItemByWorldPos(base.ToWorldPos());
			if (this.PowerItem == null)
			{
				this.CreatePowerItemForTileEntity(num);
			}
			else
			{
				num = this.PowerItem.BlockID;
			}
			this.PowerItem.AddTileEntity(this);
			base.SetModified();
			this.activateDirty = true;
		}
		this.SetValuesFromBlock(num);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.DrawWires();
		}
	}

	// Token: 0x060056F6 RID: 22262 RVA: 0x00236714 File Offset: 0x00234914
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetValuesFromBlock(ushort blockID)
	{
		if (Block.list[(int)blockID].Properties.Values.ContainsKey("RequiredPower"))
		{
			this.RequiredPower = Convert.ToInt32(Block.list[(int)blockID].Properties.Values["RequiredPower"]);
			return;
		}
		this.RequiredPower = 5;
	}

	// Token: 0x060056F7 RID: 22263 RVA: 0x0023676C File Offset: 0x0023496C
	public override void UpdateTick(World world)
	{
		base.UpdateTick(world);
		if (this.BlockTransform != null)
		{
			if (this.wiresDirty)
			{
				this.DrawWires();
			}
			if (this.activateDirty)
			{
				this.Activate(this.PowerItem.IsPowered);
				this.activateDirty = false;
			}
		}
	}

	// Token: 0x060056F8 RID: 22264 RVA: 0x002367C0 File Offset: 0x002349C0
	public PowerItem CreatePowerItemForTileEntity(ushort blockID)
	{
		if (this.PowerItem == null)
		{
			this.PowerItem = this.CreatePowerItem();
			this.PowerItem.Position = base.ToWorldPos();
			this.PowerItem.BlockID = blockID;
			this.PowerItem.SetValuesFromBlock();
			PowerManager.Instance.AddPowerNode(this.PowerItem, null);
		}
		return this.PowerItem;
	}

	// Token: 0x060056F9 RID: 22265 RVA: 0x00236820 File Offset: 0x00234A20
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual PowerItem CreatePowerItem()
	{
		return PowerItem.CreateItem(this.PowerItemType);
	}

	// Token: 0x060056FA RID: 22266 RVA: 0x0023682D File Offset: 0x00234A2D
	public override void OnUnload(World world)
	{
		base.OnUnload(world);
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			this.PowerItem.RemoveTileEntity(this);
		}
		this.RemoveWires();
	}

	// Token: 0x060056FB RID: 22267 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool Activate(bool activated)
	{
		return false;
	}

	// Token: 0x060056FC RID: 22268 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public virtual bool ActivateOnce()
	{
		return false;
	}

	// Token: 0x060056FD RID: 22269 RVA: 0x00236854 File Offset: 0x00234A54
	public Vector3 GetWireOffset()
	{
		return this.WireOffset;
	}

	// Token: 0x060056FE RID: 22270 RVA: 0x00235BE9 File Offset: 0x00233DE9
	public int GetRequiredPower()
	{
		return this.RequiredPower;
	}

	// Token: 0x060056FF RID: 22271 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanHaveParent(IPowered powered)
	{
		return true;
	}

	// Token: 0x06005700 RID: 22272 RVA: 0x0023685C File Offset: 0x00234A5C
	public void SetParentWithWireTool(IPowered newParentTE, int wiringEntityID)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			PowerItem powerItem = newParentTE.GetPowerItem();
			PowerItem parent = this.PowerItem.Parent;
			PowerManager.Instance.SetParent(this.PowerItem, powerItem);
			if (parent != null && parent.TileEntity != null)
			{
				parent.TileEntity.CreateWireDataFromPowerItem();
				parent.TileEntity.SendWireData();
				parent.TileEntity.RemoveWires();
				parent.TileEntity.DrawWires();
			}
			newParentTE.CreateWireDataFromPowerItem();
			newParentTE.SendWireData();
			newParentTE.RemoveWires();
			newParentTE.DrawWires();
			Manager.BroadcastPlay(base.ToWorldPos().ToVector3(), powerItem.IsPowered ? "wire_live_connect" : "wire_dead_connect", 0f);
		}
		else
		{
			this.parentPosition = ((TileEntity)newParentTE).ToWorldPos();
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireActions>().Setup(NetPackageWireActions.WireActions.SetParent, base.ToWorldPos(), new List<Vector3i>
			{
				this.parentPosition
			}, wiringEntityID), false);
			Manager.BroadcastPlay(base.ToWorldPos().ToVector3(), this.IsPowered ? "wire_live_connect" : "wire_dead_connect", 0f);
		}
		base.SetModified();
	}

	// Token: 0x06005701 RID: 22273 RVA: 0x0023698C File Offset: 0x00234B8C
	public void RemoveParentWithWiringTool(int wiringEntityID)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (this.PowerItem.Parent != null)
			{
				Vector3i position = this.PowerItem.Parent.Position;
				PowerItem parent = this.PowerItem.Parent;
				this.PowerItem.RemoveSelfFromParent();
				if (parent.TileEntity != null)
				{
					parent.TileEntity.CreateWireDataFromPowerItem();
					parent.TileEntity.SendWireData();
					parent.TileEntity.RemoveWires();
					parent.TileEntity.DrawWires();
				}
				Manager.BroadcastPlay(position.ToVector3(), this.PowerItem.IsPowered ? "wire_live_break" : "wire_dead_break", 0f);
			}
		}
		else
		{
			Vector3i tileEntityPosition = base.ToWorldPos();
			Vector3 position2 = tileEntityPosition.ToVector3();
			this.parentPosition = new Vector3i(-9999, -9999, -9999);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageWireActions>().Setup(NetPackageWireActions.WireActions.RemoveParent, tileEntityPosition, new List<Vector3i>(), wiringEntityID), false);
			Manager.BroadcastPlay(position2, this.IsPowered ? "wire_live_break" : "wire_dead_break", 0f);
		}
		base.SetModified();
	}

	// Token: 0x06005702 RID: 22274 RVA: 0x00236AA9 File Offset: 0x00234CA9
	public void SetWireData(List<Vector3i> wireChildren)
	{
		this.wireDataList = wireChildren;
		this.RemoveWires();
		this.DrawWires();
	}

	// Token: 0x06005703 RID: 22275 RVA: 0x00198AC2 File Offset: 0x00196CC2
	public override TileEntityType GetTileEntityType()
	{
		return TileEntityType.Powered;
	}

	// Token: 0x04004317 RID: 17175
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int ver = 1;

	// Token: 0x04004318 RID: 17176
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool wiresDirty;

	// Token: 0x04004319 RID: 17177
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isPlayerPlaced;

	// Token: 0x0400431A RID: 17178
	public PowerItem.PowerItemTypes PowerItemType = PowerItem.PowerItemTypes.Consumer;

	// Token: 0x0400431B RID: 17179
	[PublicizedFrom(EAccessModifier.Protected)]
	public PowerItem PowerItem;

	// Token: 0x0400431C RID: 17180
	public Vector3 WireOffset = Vector3.zero;

	// Token: 0x0400431D RID: 17181
	public float CenteredPitch;

	// Token: 0x0400431E RID: 17182
	public float CenteredYaw;

	// Token: 0x0400431F RID: 17183
	public string WindowGroupToOpen = string.Empty;

	// Token: 0x04004320 RID: 17184
	[PublicizedFrom(EAccessModifier.Private)]
	public bool needBlockData;

	// Token: 0x04004321 RID: 17185
	[PublicizedFrom(EAccessModifier.Private)]
	public int requiredPower;

	// Token: 0x04004322 RID: 17186
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPowered;

	// Token: 0x04004323 RID: 17187
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform blockTransform;

	// Token: 0x04004324 RID: 17188
	[PublicizedFrom(EAccessModifier.Private)]
	public List<IWireNode> currentWireNodes = new List<IWireNode>();

	// Token: 0x04004325 RID: 17189
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3i> wireDataList = new List<Vector3i>();

	// Token: 0x04004326 RID: 17190
	[PublicizedFrom(EAccessModifier.Private)]
	public bool activateDirty;

	// Token: 0x04004327 RID: 17191
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i parentPosition = new Vector3i(-9999, -9999, -9999);
}

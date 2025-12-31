using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;

// Token: 0x02000A80 RID: 2688
public abstract class WorldBase : IBlockAccess
{
	// Token: 0x060052D2 RID: 21202
	public abstract IChunk GetChunkSync(Vector3i chunkPos);

	// Token: 0x060052D3 RID: 21203
	public abstract IChunk GetChunkFromWorldPos(int x, int y, int z);

	// Token: 0x060052D4 RID: 21204
	public abstract IChunk GetChunkFromWorldPos(Vector3i _blockPos);

	// Token: 0x060052D5 RID: 21205
	public abstract void GetChunkFromWorldPos(int _blockX, int _blockZ, ref IChunk _chunk);

	// Token: 0x060052D6 RID: 21206
	public abstract bool GetChunkFromWorldPos(Vector3i _blockPos, ref IChunk _chunk);

	// Token: 0x060052D7 RID: 21207
	public abstract void AddFallingBlocks(IList<Vector3i> _list);

	// Token: 0x060052D8 RID: 21208
	public abstract byte GetStability(int worldX, int worldY, int worldZ);

	// Token: 0x060052D9 RID: 21209
	public abstract byte GetStability(Vector3i _pos);

	// Token: 0x060052DA RID: 21210
	public abstract void SetStability(int worldX, int worldY, int worldZ, byte stab);

	// Token: 0x060052DB RID: 21211
	public abstract void SetStability(Vector3i _pos, byte stab);

	// Token: 0x060052DC RID: 21212
	public abstract byte GetHeight(int worldX, int worldZ);

	// Token: 0x060052DD RID: 21213
	public abstract bool IsWater(int _x, int _y, int _z);

	// Token: 0x060052DE RID: 21214
	public abstract bool IsWater(Vector3i _pos);

	// Token: 0x060052DF RID: 21215
	public abstract bool IsWater(Vector3 _pos);

	// Token: 0x060052E0 RID: 21216
	public abstract bool IsAir(int _x, int _y, int _z);

	// Token: 0x060052E1 RID: 21217
	public abstract IGameManager GetGameManager();

	// Token: 0x060052E2 RID: 21218
	public abstract Manager GetAudioManager();

	// Token: 0x060052E3 RID: 21219
	public abstract AIDirector GetAIDirector();

	// Token: 0x060052E4 RID: 21220
	public abstract void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue);

	// Token: 0x060052E5 RID: 21221
	public abstract void SetBlockRPC(Vector3i _blockPos, BlockValue _blockValue);

	// Token: 0x060052E6 RID: 21222
	public abstract void SetBlockRPC(Vector3i _blockPos, BlockValue _blockValue, sbyte _density);

	// Token: 0x060052E7 RID: 21223
	public abstract void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, sbyte _density);

	// Token: 0x060052E8 RID: 21224
	public abstract void SetBlockRPC(Vector3i _blockPos, sbyte _density);

	// Token: 0x060052E9 RID: 21225
	public abstract void SetBlocksRPC(List<BlockChangeInfo> _blockChangeInfo);

	// Token: 0x060052EA RID: 21226
	public abstract void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, sbyte _density, int _changingEntityId);

	// Token: 0x060052EB RID: 21227
	public abstract void SetBlockRPC(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _changingEntityId);

	// Token: 0x060052EC RID: 21228
	public abstract BlockValue GetBlock(int _x, int _y, int _z);

	// Token: 0x060052ED RID: 21229
	public abstract BlockValue GetBlock(int _clrIdx, int _x, int _y, int _z);

	// Token: 0x060052EE RID: 21230
	public abstract BlockValue GetBlock(Vector3i _pos);

	// Token: 0x060052EF RID: 21231
	public abstract BlockValue GetBlock(int _clrIdx, Vector3i _pos);

	// Token: 0x060052F0 RID: 21232
	public abstract sbyte GetDensity(int _clrIdx, Vector3i _blockPos);

	// Token: 0x060052F1 RID: 21233
	public abstract sbyte GetDensity(int _clrIdx, int _x, int _y, int _z);

	// Token: 0x060052F2 RID: 21234
	public abstract Entity GetEntity(int _entityId);

	// Token: 0x060052F3 RID: 21235
	public abstract void ChangeClientEntityIdToServer(int _clientEntityId, int _serverEntityId);

	// Token: 0x060052F4 RID: 21236
	public abstract bool IsRemote();

	// Token: 0x060052F5 RID: 21237
	public abstract WorldBlockTicker GetWBT();

	// Token: 0x060052F6 RID: 21238
	public abstract bool IsOpenSkyAbove(int _clrIdx, int _x, int _y, int _z);

	// Token: 0x060052F7 RID: 21239
	public abstract int GetBlockLightValue(int _clrIdx, Vector3i blockPos);

	// Token: 0x060052F8 RID: 21240
	public abstract float GetLightBrightness(Vector3i blockPos);

	// Token: 0x060052F9 RID: 21241
	public abstract bool IsEditor();

	// Token: 0x060052FA RID: 21242
	public abstract TileEntity GetTileEntity(Vector3i _blockPos);

	// Token: 0x060052FB RID: 21243
	public abstract TileEntity GetTileEntity(int _clrIdx, Vector3i _blockPos);

	// Token: 0x060052FC RID: 21244
	public abstract List<EntityPlayer> GetPlayers();

	// Token: 0x060052FD RID: 21245
	public abstract EntityPlayerLocal GetPrimaryPlayer();

	// Token: 0x060052FE RID: 21246
	public abstract void AddLocalPlayer(EntityPlayerLocal _localPlayer);

	// Token: 0x060052FF RID: 21247
	public abstract void RemoveLocalPlayer(EntityPlayerLocal _localPlayer);

	// Token: 0x06005300 RID: 21248
	public abstract List<EntityPlayerLocal> GetLocalPlayers();

	// Token: 0x06005301 RID: 21249
	public abstract bool IsLocalPlayer(int _playerId);

	// Token: 0x06005302 RID: 21250
	public abstract EntityPlayerLocal GetLocalPlayerFromID(int _playerId);

	// Token: 0x06005303 RID: 21251
	public abstract EntityPlayerLocal GetClosestLocalPlayer(Vector3 _position);

	// Token: 0x06005304 RID: 21252
	public abstract Vector3 GetVectorToClosestLocalPlayer(Vector3 _position);

	// Token: 0x06005305 RID: 21253
	public abstract float GetSquaredDistanceToClosestLocalPlayer(Vector3 _position);

	// Token: 0x06005306 RID: 21254
	public abstract float GetDistanceToClosestLocalPlayer(Vector3 _position);

	// Token: 0x06005307 RID: 21255
	public abstract bool CanPlaceLandProtectionBlockAt(Vector3i blockPos, PersistentPlayerData lpRelative);

	// Token: 0x06005308 RID: 21256
	public abstract bool CanPlaceBlockAt(Vector3i blockPos, PersistentPlayerData lpRelative, bool traderAllowed = false);

	// Token: 0x06005309 RID: 21257
	public abstract bool CanPickupBlockAt(Vector3i blockPos, PersistentPlayerData lpRelative);

	// Token: 0x0600530A RID: 21258
	public abstract float GetLandProtectionHardnessModifier(Vector3i blockPos, EntityAlive lpRelative, PersistentPlayerData ppData);

	// Token: 0x0600530B RID: 21259
	public abstract bool IsMyLandProtectedBlock(Vector3i worldBlockPos, PersistentPlayerData lpRelative, bool traderAllowed = false);

	// Token: 0x0600530C RID: 21260
	public abstract EnumLandClaimOwner GetLandClaimOwner(Vector3i worldBlockPos, PersistentPlayerData lpRelative);

	// Token: 0x0600530D RID: 21261
	public abstract ulong GetWorldTime();

	// Token: 0x0600530E RID: 21262
	public abstract WorldCreationData GetWorldCreationData();

	// Token: 0x0600530F RID: 21263
	public abstract void UnloadEntities(List<Entity> _entityList, bool _forceUnload = false);

	// Token: 0x06005310 RID: 21264
	public abstract Entity RemoveEntity(int _entityId, EnumRemoveEntityReason _reason);

	// Token: 0x06005311 RID: 21265
	public abstract int AddSleeperVolume(SleeperVolume _volume);

	// Token: 0x06005312 RID: 21266
	public abstract int FindSleeperVolume(Vector3i mins, Vector3i maxs);

	// Token: 0x06005313 RID: 21267
	public abstract void ResetSleeperVolumes(long chunkKey);

	// Token: 0x06005314 RID: 21268
	public abstract int GetSleeperVolumeCount();

	// Token: 0x06005315 RID: 21269
	public abstract SleeperVolume GetSleeperVolume(int index);

	// Token: 0x06005316 RID: 21270
	public abstract int AddTriggerVolume(TriggerVolume _volume);

	// Token: 0x06005317 RID: 21271
	public abstract int FindTriggerVolume(Vector3i mins, Vector3i maxs);

	// Token: 0x06005318 RID: 21272
	public abstract void ResetTriggerVolumes(long chunkKey);

	// Token: 0x06005319 RID: 21273
	public abstract int GetTriggerVolumeCount();

	// Token: 0x0600531A RID: 21274
	public abstract TriggerVolume GetTriggerVolume(int index);

	// Token: 0x0600531B RID: 21275
	public abstract int AddWallVolume(WallVolume _volume);

	// Token: 0x0600531C RID: 21276
	public abstract int FindWallVolume(Vector3i mins, Vector3i maxs);

	// Token: 0x0600531D RID: 21277
	public abstract int GetWallVolumeCount();

	// Token: 0x0600531E RID: 21278
	public abstract List<WallVolume> GetAllWallVolumes();

	// Token: 0x0600531F RID: 21279
	public abstract WallVolume GetWallVolume(int index);

	// Token: 0x06005320 RID: 21280
	public abstract GameRandom GetGameRandom();

	// Token: 0x06005321 RID: 21281
	public abstract void AddPendingDowngradeBlock(Vector3i _blockPos);

	// Token: 0x06005322 RID: 21282
	public abstract bool TryRetrieveAndRemovePendingDowngradeBlock(Vector3i _blockPos);

	// Token: 0x06005323 RID: 21283 RVA: 0x00214F5E File Offset: 0x0021315E
	[PublicizedFrom(EAccessModifier.Protected)]
	public WorldBase()
	{
	}

	// Token: 0x04003F24 RID: 16164
	public ChunkClusterList ChunkClusters = new ChunkClusterList();

	// Token: 0x04003F25 RID: 16165
	public ChunkCluster ChunkCache;
}

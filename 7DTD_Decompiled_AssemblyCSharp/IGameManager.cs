using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F4E RID: 3918
public interface IGameManager
{
	// Token: 0x06007C6E RID: 31854
	void SetBlocksRPC(List<BlockChangeInfo> _changes, PlatformUserIdentifierAbs _persistentPlayerId = null);

	// Token: 0x06007C6F RID: 31855
	void SimpleRPC(int _entityId, SimpleRPCType _rpcType, bool _bExeLocal, bool _bOnlyLocal);

	// Token: 0x06007C70 RID: 31856
	void SpawnBlockParticleEffect(Vector3i _blockPos, ParticleEffect _pe);

	// Token: 0x06007C71 RID: 31857
	bool HasBlockParticleEffect(Vector3i _blockPos);

	// Token: 0x06007C72 RID: 31858
	Transform GetBlockParticleEffect(Vector3i _blockPos);

	// Token: 0x06007C73 RID: 31859
	void RemoveBlockParticleEffect(Vector3i _blockPos);

	// Token: 0x06007C74 RID: 31860
	ChunkManager.ChunkObserver AddChunkObserver(Vector3 _initialPosition, bool _bBuildVisualMeshAround, int _viewDim, int _entityIdToSendChunksTo);

	// Token: 0x06007C75 RID: 31861
	void RemoveChunkObserver(ChunkManager.ChunkObserver _observer);

	// Token: 0x06007C76 RID: 31862
	PersistentPlayerList GetPersistentPlayerList();

	// Token: 0x06007C77 RID: 31863
	PersistentPlayerData GetPersistentLocalPlayer();

	// Token: 0x06007C78 RID: 31864
	void HandlePersistentPlayerDisconnected(int _entityId);

	// Token: 0x06007C79 RID: 31865
	void ItemActionEffectsServer(int _entityId, int _slotIdx, int _itemActionIdx, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0);

	// Token: 0x06007C7A RID: 31866
	void ItemActionEffectsClient(int _entityId, int _slotIdx, int _itemActionIdx, int _firingState, Vector3 _startPos, Vector3 _direction, int _userData = 0);

	// Token: 0x06007C7B RID: 31867
	void SpawnParticleEffectServer(ParticleEffect _pe, int _entityIdThatCausedIt, bool _forceCreation = false, bool _worldSpawn = false);

	// Token: 0x06007C7C RID: 31868
	void SpawnParticleEffectClient(ParticleEffect _pe, int _entityIdThatCausedIt, bool _forceCreation = false, bool _worldSpawn = false);

	// Token: 0x06007C7D RID: 31869
	Transform SpawnParticleEffectClientForceCreation(ParticleEffect _pe, int _entityIdThatCausedIt, bool worldSpawn);

	// Token: 0x06007C7E RID: 31870
	void ExplosionServer(int _clrIdx, Vector3 _worldPos, Vector3i _blockPos, Quaternion _rotation, ExplosionData _explosionData, int _playerIdx, float _delay, bool _bRemoveBlockAtExplPosition, ItemValue _itemValueExplosive = null);

	// Token: 0x06007C7F RID: 31871
	GameObject ExplosionClient(int _clrIdx, Vector3 _center, Quaternion _rotation, int _index, int _blastPower, float _blastRadius, float _blockDamage, int _entityId, List<BlockChangeInfo> _explosionChanges);

	// Token: 0x06007C80 RID: 31872
	void ItemReloadServer(int _entityId);

	// Token: 0x06007C81 RID: 31873
	void ItemReloadClient(int _entityId);

	// Token: 0x06007C82 RID: 31874
	void ItemDropServer(ItemStack _itemStack, Vector3 _dropPos, Vector3 _randomPosAdd, int _entityId = -1, float _lifetime = 60f, bool _bDropPosIsRelativeToHead = false);

	// Token: 0x06007C83 RID: 31875
	void ItemDropServer(ItemStack _itemStack, Vector3 _dropPos, Vector3 _randomPosAdd, Vector3 _initialMotion, int _entityId = -1, float _lifetime = 60f, bool _bDropPosIsRelativeToHead = false, int _clientInstanceId = 0);

	// Token: 0x06007C84 RID: 31876
	void RequestToSpawnEntityServer(EntityCreationData _ecd);

	// Token: 0x06007C85 RID: 31877
	void DropContentOfLootContainerServer(BlockValue _bvOld, Vector3i _worldPos, int _entityId, ITileEntityLootable _teOld = null);

	// Token: 0x06007C86 RID: 31878
	EntityLootContainer DropContentInLootContainerServer(int _droppedByID, string _containerEntity, Vector3 _pos, ItemStack[] items, bool _skipIfEmpty = false);

	// Token: 0x06007C87 RID: 31879
	void CollectEntityServer(int _entityId, int _playerId);

	// Token: 0x06007C88 RID: 31880
	void CollectEntityClient(int _entityId, int _playerId);

	// Token: 0x06007C89 RID: 31881
	void PickupBlockServer(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerId, PlatformUserIdentifierAbs persistentPlayerId = null);

	// Token: 0x06007C8A RID: 31882
	void PickupBlockClient(int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _playerId);

	// Token: 0x06007C8B RID: 31883
	void PlaySoundAtPositionServer(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance);

	// Token: 0x06007C8C RID: 31884
	void PlaySoundAtPositionServer(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance, int _entityId);

	// Token: 0x06007C8D RID: 31885
	void PlaySoundAtPositionClient(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance);

	// Token: 0x06007C8E RID: 31886
	void SetWorldTime(ulong _worldTime);

	// Token: 0x06007C8F RID: 31887
	void AddVelocityToEntityServer(int _entityId, Vector3 _velToAdd);

	// Token: 0x06007C90 RID: 31888
	void AddExpServer(int _entityId, string _skill, int _experience);

	// Token: 0x06007C91 RID: 31889
	void AddScoreServer(int _entityId, int _zombieKills, int _playerKills, int _otherTeamnumber, int _conditions);

	// Token: 0x06007C92 RID: 31890
	IEnumerator ResetWindowsAndLocksByPlayer(int _playerId);

	// Token: 0x06007C93 RID: 31891
	IEnumerator ResetWindowsAndLocksByChunks(HashSetLong chunks);

	// Token: 0x06007C94 RID: 31892
	void TELockServer(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt, string _customUi = null);

	// Token: 0x06007C95 RID: 31893
	void TEUnlockServer(int _clrIdx, Vector3i _blockPos, int _lootEntityId, bool allowContainerDestroy = true);

	// Token: 0x06007C96 RID: 31894
	void TEAccessClient(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt, string _customUi = null);

	// Token: 0x06007C97 RID: 31895
	void TEDeniedAccessClient(int _clrIdx, Vector3i _blockPos, int _lootEntityId, int _entityIdThatOpenedIt);
}

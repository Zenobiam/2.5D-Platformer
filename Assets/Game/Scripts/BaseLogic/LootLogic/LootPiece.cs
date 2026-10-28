using System.Collections;
using Game.Scripts.Data;
using Game.Scripts.Data.Extensions;
using Game.Scripts.Infrastructure;
using Game.Scripts.Services.PersistentProgress.Interfaces;
using Game.Scripts.Services.SaveLoad.Interfaces;
using TMPro;
using UnityEngine;

namespace Game.Scripts.BaseLogic.LootLogic
{
    public class LootPiece : MonoBehaviour, ISaveProgress
    {
        public GameObject LootModel;
        public GameObject PickableFxPrefab;
        public TextMeshPro LootText;
        public GameObject PickupPopup;

        private Loot _loot;
        private bool _isPicked;
        private WorldData _worldData;
        private ISaveLoadService _saveLoadService;

        private string _id;

        public void Construct(WorldData worldData)
        {
            _worldData = worldData;
        }

        public void Initialize(Loot loot)
        {
            _loot = loot;
        }

        /// <summary>
        /// Как SaveTrigger: сервис сохранения из ServiceLocator (фабрика его не знает — SaveLoadService сам зависит от фабрики).
        /// </summary>
        private void Awake() =>
            _saveLoadService = ServiceLocator.Container.Single<ISaveLoadService>();

        private void Start() =>
            _id = GetComponent<UniqueId>().Id;

        /// <summary>
        /// Пусто, как в курсе (урок 11): лут из сохранения восстанавливает LoadSceneState.InitLootPieces —
        /// id, значение и позицию он выставляет сразу после CreateLoot.
        /// </summary>
        public void LoadProgress(PlayerProgress progress)
        {
        }

        /// <summary>
        /// Неподобранный лут записывает себя в прогресс один раз: позиция и значение не меняются.
        /// </summary>
        public void UpdateProgress(PlayerProgress progress)
        {
            if (_isPicked)
                return;

            LootPieceDataDictionary lootPiecesOnScene = progress.WorldData.LootData.LootPiecesOnScene;

            if (!lootPiecesOnScene.Dictionary.ContainsKey(_id))
                lootPiecesOnScene.Dictionary.Add(_id, new LootPieceData(transform.position.AsVector3Data(), _loot));
        }

        private void OnTriggerEnter(Collider other) =>
            Pickup();

        private void Pickup()
        {
            if (_isPicked)
                return;
            _isPicked = true;
            
            UpdateWorldData();
            HideLootModel();
            PlayPickupFx();
            ShowText();
            StartCoroutine(StartDestroyTimer());
        }

        /// <summary>
        /// Души сохраняются сразу при подборе (решение пользователя 2026-10-27, в курсе — только на SaveTrigger).
        /// SaveProgress пишет весь прогресс, в том числе позицию игрока: подбор работает как чекпоинт.
        /// </summary>
        private void UpdateWorldData()
        {
            _worldData.LootData.Collect(_loot);
            RemoveFromSavedPieces();
            _saveLoadService.SaveProgress();
        }

        private void RemoveFromSavedPieces()
        {
            LootPieceDataDictionary savedPieces = _worldData.LootData.LootPiecesOnScene;

            if (savedPieces.Dictionary.ContainsKey(_id))
                savedPieces.Dictionary.Remove(_id);
        }

        private void HideLootModel() => 
            LootModel.SetActive(false);

        private void PlayPickupFx() => 
            Instantiate(PickableFxPrefab, transform.position, Quaternion.identity);

        private void ShowText()
        {
            LootText.text = $"{_loot.Value}";
            PickupPopup.SetActive(true);
        }

        private IEnumerator StartDestroyTimer()
        {
            yield return  new WaitForSeconds(1.5f);
            
            Destroy(gameObject);
        }
    }
}
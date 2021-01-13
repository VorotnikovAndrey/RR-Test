using Events;
using UnityEngine;
using Zenject;
using GameStage;
using Defong.Events;
using Defong.GameStageSystem;
using Defong.ObjectPool;

namespace Defong
{
    public class GameEnterPoint : MonoBehaviour
    {
        [Inject] private EventAggregator _eventAggregator;
        [Inject] private StageController _stageController;

        private void Start()
        {
            ViewGenerator.SetUnitPool(new UnitPool());
            _stageController.Init(StageName.Gameplay);
            Application.targetFrameRate = 60;
        }
    }
}
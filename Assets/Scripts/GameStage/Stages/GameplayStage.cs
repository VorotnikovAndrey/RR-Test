using Defong;
using Defong.Events;
using Defong.GameStageSystem;
using Defong.ObjectPool;
using Defong.PopupSystem;
using Defong.Utils;
using PopupSystem;
using UnityEngine;
using Zenject;

namespace GameStage.Stages
{
    public class GameplayStage : AbstractStageBase
    {
        public override object StageId => StageName.Gameplay;

        private readonly PopupManager<PopupType> _popupManager;
        private readonly EventAggregator _eventAggregator;
        private readonly TimeTicker _timeTicker;
        private readonly IconLoader _iconLoader;

        private LevelView _currentLevelView;
        private CardsComponent _cardsComponent;
        private MainCanvas _canvas;
        private PlayerCamera _cam;

        [Inject]
        public GameplayStage(PopupManager<PopupType> popupManager, EventAggregator eventAggregator, TimeTicker timeTicker, IconLoader iconLoader)
        {
            _popupManager = popupManager;
            _eventAggregator = eventAggregator;
            _timeTicker = timeTicker;
            _iconLoader = iconLoader;
        }

        public override void Initialize(object data)
        {
            if (_canvas == null)
            {
                _canvas = ProjectContext.Instance.Container.Resolve<MainCanvas>();
                _popupManager.SetMainCanvas(_canvas);
            }

            _eventAggregator.Add<ImageLoadingCompletedEvent>(CreateLevel);

            base.Initialize(data);
        }

        public override void DeInitialize()
        {
            _eventAggregator.Remove<ImageLoadingCompletedEvent>(CreateLevel);

            _cardsComponent.DeInitialize();
            _timeTicker.OnTick -= _cardsComponent.OnUpdate;
            _cardsComponent = null;
            _currentLevelView.ReleaseItemView();
            _currentLevelView = null;

            base.DeInitialize();
        }

        public override void Show()
        {
        }

        public override void Hide()
        {
        }

        private void CreateLevel(ImageLoadingCompletedEvent sender)
        {
            _currentLevelView = ViewGenerator.GetOrCreateItemView<LevelView>(string.Format(GameConstants.LevelFormat, "Demo"));
            _currentLevelView.SetPosition(Vector3.zero);

            if (_cam == null)
            {
                _cam = ProjectContext.Instance.Container.Resolve<PlayerCamera>();
                _cam.SetTarget(_currentLevelView.Background);
            }

            _cardsComponent = new CardsComponent();
            _cardsComponent.Initialize();
            _timeTicker.OnTick += _cardsComponent.OnUpdate;
        }
    }
}
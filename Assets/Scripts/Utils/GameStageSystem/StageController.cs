using System;
using System.Collections.Generic;
using Zenject;

namespace Defong.GameStageSystem
{
    public class StageController : IDisposable
    {
        public event Action StageChanged;
        public IStage Stage { get; private set; }
        
        private Dictionary<object, AbstractStageBase> _stages = new Dictionary<object, AbstractStageBase>();
        
        [Inject]
        public StageController()
        {
            var stages = ProjectContext.Instance.Container.ResolveAll<AbstractStageBase>();
            foreach (var stageBase in stages)
            {
                _stages.Add(stageBase.StageId, stageBase);
                stageBase.OnStageFinished += HandleStageFinished;
            }
        }

        public void Dispose()
        {
            foreach (var stageValue in _stages.Values)
            {
                stageValue.OnStageFinished -= HandleStageFinished;
            }
        }

        public void Init(object stageId)
        {
            ShowStage(stageId, null);
        }
        
        private void ShowStage(object stageId, object data)
        {
            Stage = _stages[stageId];
            Stage.Initialize(data);
            Stage.Show();
        }
        
        private void OnCloseStage(object target, object data)
        {
            Stage.DeInitialize();
            ShowStage(target, data);
        }

        private void HandleStageFinished(object sender, StageCallbackEventArgs e)
        {
            StageChanged?.Invoke();
        }
    }
}

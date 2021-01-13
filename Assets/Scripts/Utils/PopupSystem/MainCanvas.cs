using System;
using Defong.Events;
using UnityEngine;
using Zenject;

namespace Defong.PopupSystem
{
    public class MainCanvas : MonoBehaviour
    {
        public event Action OnBackButtonPress = default;

        private EventAggregator _eventAggregator;

        private EventAggregator EventAggregator
        {
            get
            {
                if (_eventAggregator == null)
                {
                    _eventAggregator = ProjectContext.Instance.Container.Resolve<EventAggregator>();
                }

                return _eventAggregator;
            }
        }

        private void Awake()
        {
            ProjectContext.Instance.Container.BindInstances(this);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackButtonPress?.Invoke();
            }
        }

        public void OnRestartPressed()
        {
            EventAggregator.SendEvent(new ButtonRestartEvent());
        }

        public void OnButtonPressed()
        {
            EventAggregator.SendEvent(new ButtonPressedEvent());
        }
    }
}
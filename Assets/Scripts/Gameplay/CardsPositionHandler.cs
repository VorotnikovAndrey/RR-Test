using System.Collections.Generic;
using Defong.Events;
using UnityEngine;
using Zenject;

namespace Defong
{
    public class CardsPositionHandler : MonoBehaviour
    {
        [SerializeField] private float _offsetX = 2f;
        [SerializeField] private AnimationCurve _curveY = default;

        private readonly List<CardView> _cards = new List<CardView>();

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
            EventAggregator.Add<CardCreatedEvent>(OnCardCreated);
            EventAggregator.Add<CardRemovedEvent>(OnCardRemoved);
        }

        private void OnDestroy()
        {
            EventAggregator.Remove<CardCreatedEvent>(OnCardCreated);
            EventAggregator.Remove<CardRemovedEvent>(OnCardRemoved);
        }

        private void OnCardCreated(CardCreatedEvent sender)
        {
            Add(sender.View);
        }

        private void OnCardRemoved(CardRemovedEvent sender)
        {
            Remove(sender.View);
        }

        public void Add(CardView view)
        {
            if (_cards.Contains(view))
            {
                return;
            }

            _cards.Add(view);
            Refresh();
        }

        public void Remove(CardView view)
        {
            if (!_cards.Contains(view))
            {
                return;
            }

            _cards.Remove(view);
            Refresh();
        }

        private void Refresh()
        {
            var origin = transform.position - new Vector3(_offsetX * _cards.Count / 2 - _offsetX / 2f, 0f, 0f);

            for (int i = 0; i < _cards.Count; i++)
            {
                var position = origin + new Vector3(_offsetX * i, 0f, 0f);
                position.y += _curveY.Evaluate(1f / (_cards.Count + 1) * (i + 1));
                position.z += 0.1f * i;

                float angle = -45f;
                float rotation = -1f * (angle / 2f) + (i + 1) * (angle / (_cards.Count + 1));

                _cards[i].MoveTo(position);
                _cards[i].RotateTo(new Vector3(0f, 0f, rotation));
            }
        }
    }
}
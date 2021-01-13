using System.Collections.Generic;
using Defong.Events;
using Defong.ObjectPool;
using Defong.Utils;
using UnityEngine;
using Zenject;

namespace Defong
{
    public class CardsComponent
    {
        private readonly List<Card> _cards = new List<Card>();

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

        private IconLoader _iconLoader;
        protected IconLoader IconLoader
        {
            get
            {
                if (_iconLoader == null)
                {
                    _iconLoader = ProjectContext.Instance.Container.Resolve<IconLoader>();
                }

                return _iconLoader;
            }
        }

        public void Initialize()
        {
            EventAggregator.Add<ButtonPressedEvent>(OnButtonPressedEvent);
            EventAggregator.Add<ButtonRestartEvent>(OnButtonRestartEvent);

            CreateCard(GameConstants.LimitCardsInHand);
        }

        public void DeInitialize()
        {
            EventAggregator.Remove<ButtonPressedEvent>(OnButtonPressedEvent);
            EventAggregator.Remove<ButtonRestartEvent>(OnButtonRestartEvent);

            foreach (var card in _cards)
            {
                card.GetOrCreateItemView().ReleaseItemView();
            }

            _cards.Clear();
        }

        public void OnUpdate()
        {
            foreach (var card in _cards.ToArray())
            {
                if (card == null || card.Health > 0)
                {
                    continue;
                }

                DestroyCard(card);
            }
        }

        private void OnButtonPressedEvent(ButtonPressedEvent sender)
        {
            foreach (var card in _cards)
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        card.Health.Value += Random.Range(-2, 2);
                        break;
                    case 1:
                        card.Damage.Value += Random.Range(-2, 2);
                        break;
                    case 2:
                        card.Cost.Value += Random.Range(-2, 2);
                        break;
                }
            }
        }

        private void OnButtonRestartEvent(ButtonRestartEvent sender)
        {
            foreach (var card in _cards.ToArray())
            {
                DestroyCard(card);
            }

            _cards.Clear();

            CreateCard(GameConstants.LimitCardsInHand);
        }

        private void CreateCard(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                if (_cards.Count >= GameConstants.LimitCardsInHand)
                {
                    break;
                }

                var model = CardFabric.CreateCard(CardType.Default);
                var view = model.GetOrCreateItemView<CardView>();
                view.SetPosition(Vector3.zero);
                view.SetIcon(IconLoader.GetRandomSprite());

                model.Title.AddListener(x => view.TitleText.text = x);
                model.Description.AddListener(x => view.DescriptionText.text = x);
                model.Damage.AddListener(x => view.DamageText.text = x.ToString());
                model.Health.AddListener(x => view.HealthText.text = x.ToString());
                model.Cost.AddListener(x => view.CostText.text = x.ToString());

                _cards.Add(model);

                EventAggregator.SendEvent(new CardCreatedEvent
                {
                    View = view
                });
            }
        }

        private void DestroyCard(Card card)
        {
            var view = card.GetOrCreateItemView<CardView>();
            EventAggregator.SendEvent(new CardRemovedEvent { View = view });
            view.ReleaseItemView();
            _cards.Remove(card);
        }
    }
}
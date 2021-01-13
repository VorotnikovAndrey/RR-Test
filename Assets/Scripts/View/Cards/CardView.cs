using DG.Tweening;
using Defong.ObjectPool;
using TMPro;
using UnityEngine;

namespace Defong
{
    public class CardView : AbstractBaseView
    {
        [SerializeField] private SpriteRenderer _icon = default;

        public TMP_Text DamageText = default;
        public TMP_Text HealthText = default;
        public TMP_Text CostText = default;
        public TMP_Text TitleText = default;
        public TMP_Text DescriptionText = default;

        [Space]

        [SerializeField] private float _movementDuration = 1f;
        [SerializeField] private Ease _movementEase = Ease.Unset;
        [SerializeField] private float _rotationDuration = 1f;
        [SerializeField] private Ease _rotationEase = Ease.Unset;

        private Tweener _movementTweener;
        private Tweener _rotationTweener;

        public void SetIcon(Sprite value)
        {
            _icon.sprite = value;
        }

        public void MoveTo(Vector3 position)
        {
            _movementTweener?.Kill();
            _movementTweener = Transform.DOMove(position, _movementDuration).SetEase(_movementEase).OnKill(() => _movementTweener = null);
        }

        public void RotateTo(Vector3 rotation)
        {
            _rotationTweener?.Kill();
            _rotationTweener = Transform.DORotate(rotation, _rotationDuration).SetEase(_rotationEase).OnKill(() => _rotationTweener = null);
        }

        protected override void SwitchOff()
        {
            _rotationTweener?.Kill();
            _movementTweener?.Kill();

            base.SwitchOff();
        }
    }
}
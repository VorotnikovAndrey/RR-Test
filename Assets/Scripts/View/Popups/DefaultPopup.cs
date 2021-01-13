using Defong.PopupSystem;
using PopupSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Defong
{
    public class DefaultPopup : AbstractPopupBase<PopupType>
    {
        public override PopupType Type => PopupType.Default;

        [SerializeField] private Image _icon = default;

        protected override void OnShow(object args = null)
        {
            base.OnShow(args);

            var loader = ProjectContext.Instance.Container.Resolve<IconLoader>();
            _icon.sprite = loader.GetRandomSprite();
        }
    }
}
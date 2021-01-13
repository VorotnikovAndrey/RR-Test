using Defong.PopupSystem;
using PopupSystem;
using TMPro;
using UnityEngine;

namespace Defong
{
    public class WaitingImagesLoadPopup : AbstractPopupBase<PopupType>
    {
        public override PopupType Type => PopupType.WaitingImagesLoad;

        [SerializeField] private TextMeshProUGUI _totalProgressText = default;

        private const string _pattern = "Remaining to download {0} files";

        protected override void OnShow(object args = null)
        {
            base.OnShow(args);

            _totalProgressText.text = string.Empty;
        }

        public void SetTotalProgressValue(int value)
        {
            _totalProgressText.text = string.Format(_pattern, value);
        }
    }
}
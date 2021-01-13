using System;
using System.Collections.Generic;
using System.IO;
using Asyncoroutine;
using Defong.Events;
using Defong.PopupSystem;
using Defong.Utils;
using PopupSystem;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Defong
{
    public sealed class IconLoader
    {
        private readonly PopupManager<PopupType> _popupManager;
        private readonly EventAggregator _eventAggregator;

        private readonly string _url = "https://picsum.photos/300";
        private readonly string _path = Application.streamingAssetsPath;
        private const int _limit = 10;

        private readonly List<Sprite> _data = new List<Sprite>();

        [Inject]
        public IconLoader(PopupManager<PopupType> popupManager, EventAggregator eventAggregator)
        {
            _popupManager = popupManager;
            _eventAggregator = eventAggregator;

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            FindFiles();
            Download();
        }

        private async void Download()
        {
            await new WaitForNextFrame();

            int count = _limit - _data.Count;
            if (count > 0)
            {
                Debug.Log($"IconLoader: Start uploading {count.AddColorTag(Color.green)} files".AddColorTag(Color.cyan));

                var popup = _popupManager.GetPopupByType<WaitingImagesLoadPopup>(PopupType.WaitingImagesLoad);
                popup.Show(count);

                for (int i = 0; i < count; i++)
                {
                    var www = new WWW(_url);
                    await www;

                    Texture2D texture = www.texture;
                    Sprite result = CreateSprite(texture);
                    _data.Add(result);

                    File.WriteAllBytes(_path + $"/{Guid.NewGuid()}.jpg", texture.EncodeToJPG());

                    popup.SetTotalProgressValue(_limit - _data.Count);

                    Debug.Log($"IconLoader: Download completed! Count {_data.Count.AddColorTag(Color.green)}".AddColorTag(Color.cyan));

#if UNITY_EDITOR
                    AssetDatabase.Refresh();
#endif
                }

                popup.Hide();
            }

            _eventAggregator.SendEvent(new ImageLoadingCompletedEvent());
        }

        public Sprite GetRandomSprite()
        {
            return _data.Count == 0 ? null : _data.GetRandom();
        }

        private Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2f);
        }

        private void FindFiles()
        {
            foreach (string file in Directory.GetFiles(_path))
            {
                var info = new FileInfo(file);
                string extension = info.Extension;

                if (extension != ".png" &&
                    extension != ".jpg" &&
                    extension != ".jpeg")
                {
                    continue;
                }

                if (file.IndexOf(".DS_Store", StringComparison.Ordinal) != -1 ||
                    file.IndexOf("Solutions~", StringComparison.Ordinal) != -1)
                {
                    continue;
                }

                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(File.ReadAllBytes(file));

                _data.Add(CreateSprite(texture));
            }

            Debug.Log($"IconLoader: Found {_data.Count.AddColorTag(Color.green)} files".AddColorTag(Color.cyan));
        }
    }
}
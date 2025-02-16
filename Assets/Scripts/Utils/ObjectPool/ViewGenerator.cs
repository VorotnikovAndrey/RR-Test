﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Defong.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Defong.ObjectPool
{
    public static class ViewGenerator
    {
        private static readonly Dictionary<string, AbstractBaseView> LoadedItemsDict = new Dictionary<string, AbstractBaseView>();
        private static readonly Dictionary<int, IView> ActiveViews = new Dictionary<int, IView>();

        private static UnitPool _unitPool = new UnitPool();


        public static void SetUnitPool(UnitPool unitPool)
        {
            _unitPool = unitPool;
        }
        public static IView GetOrCreateItemView(this IPrefable item, bool isActiveDefault = true)
        {
            return GetOrCreateItemView<IView>(item, isActiveDefault);
        }
        
        public static T GetOrCreateItemView<T>(this IPrefable item, bool isActiveDefault = true) where T : IView
        {
            if (item.GlobalIndex.HasValue)
            {
                ActiveViews.TryGetValue(item.GlobalIndex.Value, value: out var activeView);
                if (activeView != null)
                    return (T) activeView;
            }

            var prefabPath = string.Format(GameConstants.DefaultFormat, item.PrefabPrefix + "/" + item.PrefabID);

            var view = GetOrCreateItemView<T>(prefabPath, isActiveDefault);

            if (view == null)
                return view;
            
            item.GlobalIndex = view.Index;

            return view;
        }

        public static void BindItemView<T>(this IPrefable item, T view) where T : IView
        {
            if (item.GlobalIndex.HasValue)
            {
                throw new InvalidDataException("Item already have view");
            }

            if (ActiveViews.ContainsValue(view))
            {
                throw new InvalidDataException("View already binded");
            }

            item.GlobalIndex = view.Index;
            view.Name = string.Format("{0}#{1}", view.Name, view.Index);
            view.isActive = true;
            ActiveViews.Add(item.GlobalIndex.Value, view);
            item.GlobalIndex = view.Index;
        }


        public static IView GetOrCreateItemView(string prefabPath, bool isActiveDefault = true)
        {
            return GetOrCreateItemView<IView>(prefabPath, isActiveDefault);
        }

        public static T GetOrCreateItemView<T>(string prefabPath, bool isActiveDefault = true) where T : IView
        {
            var prefabId = Path.GetFileName(prefabPath);

            T view = GetFromPool<T>(prefabId);

            if (view == null)
            {
                AbstractBaseView pref;
                if (LoadedItemsDict.ContainsKey(prefabId))
                {
                    pref = LoadedItemsDict[prefabId];
                }
                else
                {
                    pref = Resources.Load<AbstractBaseView>(prefabPath);
                    LoadedItemsDict.Add(prefabId, pref);
                }

                if (pref == null)
                {
                    Debug.Log(prefabId + " in absence");
                    return default(T);
                }

                var viewObj = (IView) Object.Instantiate(pref);
                view = (T) viewObj;
            }


            view.Name = string.Format("{0}#{1}", prefabId, view.Index);
            view.SetViewActive(isActiveDefault);
            ActiveViews.Add(view.Index, view);

            view.isActive = true;

            return view;
        }

        private static T GetFromPool<T>(string prefabID) where T : IView
        {
            return GetFromItemPool<T>(prefabID);
        }

        public static void ReleaseItemView<T>(this T view) where T : IView
        {
            if (!view.isActive)
                return;
            view.isActive = false;

            view.SetParent(null);

             ReleaseItem(view);

            view.SetViewActive(false);
        }

        private static void ReleaseItem<T>(T view) where T : IView
        {
            _unitPool.AddViewByName(view.Name.Split('#')[0], view);
            ActiveViews.Remove(view.Index);
        }
        
        public static void DestroyAndRemoveFromPool<T>(this T view) where T : IView
        {
            ReleaseItem(view);
            _unitPool.RemoveFromPool(view.Name.Split('#')[0], view);
            ActiveViews.Remove(view.Index);
            
            Object.Destroy(view.GameObject);
        }

        private static T GetFromItemPool<T>(string name) where T : IView
        {
            var view = _unitPool.GetViewByName<T>(name);
            //view?.SetViewActive(true);
            return view;
        }

        public static void ReleaseAllViews()
        {
            var views = ActiveViews.Values.ToArray();
            foreach (var activeView in views)
            {
                activeView.ReleaseItemView();
            }
        }

        public static void DestroyAndRemoveAllViews()
        {
            ReleaseAllViews();
            var views =  _unitPool.GetAndRemoveFromPool<IView>();
            foreach (var view in views)
            {
                Object.Destroy(view.GameObject);
            }
        }

        public static void ReleaseAllViewsExcept(List<IView> listToExclude)
        {
            Debug.Log($"Releasing All {ActiveViews.Count} active views, except list of {listToExclude.Count} views");
            var listToRelease = (
                from kvp in ActiveViews 
                where listToExclude.FirstOrDefault(x => x.Index == kvp.Key) == null 
                select kvp.Value).ToList();

            for (int i = 0; i < listToRelease.Count; i++)
            {
                listToRelease[i].ReleaseItemView();
            }
        }
    }
}
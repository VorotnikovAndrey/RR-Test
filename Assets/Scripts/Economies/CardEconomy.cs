using System;
using System.Collections.Generic;
using Defong.Utils;
using UnityEngine;

namespace Defong
{
    [CreateAssetMenu(fileName = "CardEconomyData", menuName = "Economy/Card Economy Data")]
    public class CardEconomy : ScriptableObject
    {
        [SerializeField] private List<CardData> _data = new List<CardData>();

        public CardData GetRandom()
        {
            return _data.GetRandom();
        }
    }

    [Serializable]
    public class CardData
    {
        public string Title;
        [TextArea(3, 10)] public string Description;
        [Space]
        public int Cost;
        public int Damage;
        public int Health;
    }
}
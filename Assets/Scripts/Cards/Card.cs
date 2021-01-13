using Defong.ObjectPool;
using Source;

namespace Defong
{
    public class Card : IPrefable
    {
        #region iPrefable
        public string PrefabID => $"{CardType}Card";

        public string PrefabPrefix => "Cards";

        public int? GlobalIndex { get; set; }
        #endregion

        public EventVariable<string> Title = new EventVariable<string>();
        public EventVariable<string> Description = new EventVariable<string>();
        public EventVariable<int> Health = new EventVariable<int>();
        public EventVariable<int> Cost = new EventVariable<int>();
        public EventVariable<int> Damage = new EventVariable<int>();

        public CardType CardType { get; private set; }

        public void SetData(CardData data)
        {
            Title.Value = data.Title;
            Description.Value = data.Description;
            Health.Value = data.Health;
            Cost.Value = data.Cost;
            Damage.Value = data.Damage;
        }

        public void SetType(CardType type)
        {
            CardType = type;
        }
    }
}
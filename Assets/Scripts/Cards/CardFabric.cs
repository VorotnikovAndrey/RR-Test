using Content;

namespace Defong
{
    public static class CardFabric
    {
        public static Card CreateCard(CardType type)
        {
            var card = ResolveCardType(type);
            var data = ContentProvider.CardEconomy.GetRandom();

            card.SetData(data);
            card.SetType(type);

            return card;
        }

        private static Card ResolveCardType(CardType type)
        {
            Card value;

            switch (type)
            {
                default:
                    value = new Card();
                    break;
            }

            return value;
        }
    }
}
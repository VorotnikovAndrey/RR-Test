using Defong;
using Zenject;

namespace Content
{
    public static class ContentProvider
    {
        private static CardEconomy _cardEconomy;

        public static CardEconomy CardEconomy
        {
            get
            {
                if (_cardEconomy == null)
                {
                    _cardEconomy = ProjectContext.Instance.Container.Resolve<CardEconomy>();
                }

                return _cardEconomy;
            }
        }
    }
}
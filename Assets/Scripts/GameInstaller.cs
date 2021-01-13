using GameStage.Stages;
using Defong.Events;
using Defong.GameStageSystem;
using Defong.PopupSystem;
using Defong.Utils;
using PopupSystem;
using Zenject;

namespace Defong
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EventAggregator>().AsSingle();
            Container.Bind<PopupManager<PopupType>>().AsSingle();
            Container.Bind<IconLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<TimeTicker>().AsSingle();

            Container.Bind<AbstractStageBase>().To<GameplayStage>().AsSingle();
            Container.Bind<StageController>().AsSingle();

            Container.Bind<CardEconomy>().FromScriptableObjectResource("CardEconomyData").AsSingle();
        }
    }
}
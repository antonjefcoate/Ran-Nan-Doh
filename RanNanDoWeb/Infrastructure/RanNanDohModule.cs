using Ninject.Web.Common;
using RanNanDoh.DataViews;
using RanNanDohUi.Infrastructure.WebUtils;
using RanNanDohUi.Models;
using Ninject;
using Ninject.Modules;
using RanNanDoh.Domain.Services;
using RanNanDoh.Infrastructure;
using RanNanDoh.ReadModels;
using SimpleCQRS;

namespace RanNanDohUi.Infrastructure
{
    public class RanNanDoModule : NinjectModule
    {
        public override void Load()
        {
            Bind<FakeBus>().ToSelf().InSingletonScope();
            Bind<ICommandSender>().ToMethod(ctx => ctx.Kernel.Get<FakeBus>());
            Bind<IEventPublisher>().ToMethod(ctx => ctx.Kernel.Get<FakeBus>());
            Bind<IEventStore>().To<MongoEventStore>().InSingletonScope();
            Bind<IPlayerReadModel>().To<PlayerReadModel>().InSingletonScope();

            Bind(typeof(IRepository<>)).To(typeof(Repository<>)).InSingletonScope();
            Bind<IChallengeNotifier>().To<FacebookChallengeNotifier>().InRequestScope(); 

            Bind(typeof(ISessionState<>)).To(typeof(SessionState<>)).InSingletonScope();
            Bind<IPlayerSession>().To<PlayerSession>().InSingletonScope();

            Bind<IFacebookClientProvider>().To<FacebookClientProvider>().InSingletonScope();
            Bind<IUserActionProvider>().To<UserActionProvider>().InRequestScope();

            Bind<IViewModelReader<PlayerDto>>().To<BulshitDbPlayerDtoReader>();
            Bind<IViewModelWriter<PlayerDto>>().To<BulshitDbPlayerDtoWriter>();

            //Bind<IViewModelReader<UserRoundResultListDto>>().To<BulshitDbUserRoundListDtoReader>();
            //Bind<IViewModelWriter<UserRoundResultListDto>>().To<BulshitDbUserRoundListDtoWriter>();
            Bind<IViewModelReader<UserRoundResultListDto>>().To<MongoUserRoundListDtoReader>()
                .WithConstructorArgument("mongoConn", System.Configuration.ConfigurationManager.AppSettings["MONGOLAB_URI"]);
            Bind<IViewModelWriter<UserRoundResultListDto>>().To<MongoUserRoundListDtoWriter>()
                .WithConstructorArgument("mongoConn", System.Configuration.ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        }
    }
}
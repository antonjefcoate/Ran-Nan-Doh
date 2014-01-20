using System;

using Ninject.Modules;
using Ninject.Web.Common;

using RanNanDoh.Infrastructure;
using RanNanDoh.ReadModels;

using RanNanDohReadModel.Replay;

using SimpleCQRS;

namespace RanNanDohUi.App_Start
{
    public class ReadModelModule : NinjectModule 
    {
        public override void Load()
        {
            Bind(typeof(TypeScanner<IViewModel>))
                .ToMethod(_ => new TypeScanner<IViewModel>(AppDomain.CurrentDomain.GetAssemblies()))
                .InSingletonScope();

            this.Bind<IEventHistory>().To<MongoEventStore>()
                .InSingletonScope()
                .WithConstructorArgument("conn", System.Configuration.ConfigurationManager.AppSettings["MONGOLAB_URI"]);
            this.Bind<ViewModelRebuilder>().ToSelf().InRequestScope();

        }
    }
}
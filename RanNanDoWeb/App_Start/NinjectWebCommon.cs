using Ninject.Modules;

[assembly: WebActivator.PreApplicationStartMethod(typeof(RanNanDohUi.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(RanNanDohUi.App_Start.NinjectWebCommon), "Stop")]

namespace RanNanDohUi.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using RanNanDohUi.Infrastructure;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServiceLocator(kernel);
            RegisterServices(kernel);
            return kernel;
        }

        private static void RegisterServiceLocator(StandardKernel kernel)
        {
            // Set up the service locator for use in static extension classes
            var ninjectServiceLocator = new NinjectServiceLocator(kernel);
            ServiceLocator.SetLocatorProvider(() => ninjectServiceLocator);
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new RanNanDoModule());
            kernel.Load(new ReadModelModule());
        }        
    }
}

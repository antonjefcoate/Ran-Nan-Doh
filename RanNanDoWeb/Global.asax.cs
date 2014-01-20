using System.Web.Mvc;
using System.Web.Routing;
using RanNanDoh.CommandHandlers;
using RanNanDoh.Commands;
using RanNanDoh.DataViews;
using RanNanDoh.Domain;
using RanNanDoh.Domain.Messages;
using RanNanDoh.Domain.Services;

using RanNanDohUi.Models.ModelBinders;
using RanNanDoh.ReadModels.Handlers;
using SimpleCQRS;
using Microsoft.Practices.ServiceLocation;
using RanNanDoh.Infrastructure;
using RanNanDoh.ReadModels;

namespace RanNanDohUi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Player", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        private FakeBus Bus
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FakeBus>();
            }
        }

        protected void Application_Start()
        {
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            AreaRegistration.RegisterAllAreas();
            
            var storage = new MongoEventStore(Bus, System.Configuration.ConfigurationManager.AppSettings["MONGOLAB_URI"]);
            
            // RanNanDoh: Command registration (for domain)
            var commands2 = new PlayerCommandHandlers(new Repository<Player>(storage));
            Bus.RegisterHandler<CreateNativePlayer>(commands2.Handle);
            Bus.RegisterHandler<CreateFacebookPlayer>(commands2.Handle);

            var commands3 = new RoundCommandHandlers(new Repository<Round>(storage), new Repository<Player>(storage), new WinnerCalculator());
            Bus.RegisterHandler<CreateRound>(commands3.Handle);
            Bus.RegisterHandler<PlayMoves>(commands3.Handle);
            Bus.RegisterHandler<ChallengeRegisteredFriend>(commands3.Handle);

            // RanNanDoh: Message registration (for read models)
            var playerDtoWriter = new BulshitDbPlayerDtoWriter();
            var playerDtoReader= new BulshitDbPlayerDtoReader();
            var playerDetailView = new PlayerDtoHandler(playerDtoWriter, playerDtoReader);
            Bus.RegisterHandler<PlayerCreated>(playerDetailView.Handle);
            Bus.RegisterHandler<Player1MovesPlayed>(playerDetailView.Handle);
            Bus.RegisterHandler<Player2MovesPlayed>(playerDetailView.Handle);
            Bus.RegisterHandler<PlayerChallenged>(playerDetailView.Handle);

            var userRoundListDtoWriter = ServiceLocator.Current.GetInstance<IViewModelWriter<UserRoundResultListDto>>();
            var userRoundListDtoReader = ServiceLocator.Current.GetInstance<IViewModelReader<UserRoundResultListDto>>();
            var userRoundListDtoHandler = new UserRoundListDtoHandler(userRoundListDtoWriter, userRoundListDtoReader, playerDtoReader);
            Bus.RegisterHandler<Player1MovesPlayed>(userRoundListDtoHandler.Handle);
            Bus.RegisterHandler<Player2MovesPlayed>(userRoundListDtoHandler.Handle);
            Bus.RegisterHandler<PlayerChallenged>(userRoundListDtoHandler.Handle);
            Bus.RegisterHandler<RoundWon>(userRoundListDtoHandler.Handle);
            Bus.RegisterHandler<RoundDraw>(userRoundListDtoHandler.Handle);

            ModelBinders.Binders.DefaultBinder = new EnumModelBinder();
        }
    }
}
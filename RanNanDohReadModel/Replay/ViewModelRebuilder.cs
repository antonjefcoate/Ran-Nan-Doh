using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using RanNanDoh.ReadModels;

using SimpleCQRS;

namespace RanNanDohReadModel.Replay
{
    public class ViewModelRebuilder
    {
        private TypeScanner<IViewModel> _vmScanner;
        private readonly IEventHistory _eventHistory;

        public ViewModelRebuilder(TypeScanner<IViewModel> vmScanner, IEventHistory eventHistory, IEventPublisher eventPublisher)
        {
            this._vmScanner = vmScanner;
            this._eventHistory = eventHistory;
        }

        public void Rebuild(string viewModelName)
        {
            //1- retrieve all handlers for this viewModel
            var viewmodel= _vmScanner.Scan().First(x => x.FullName == viewModelName);
            var handlers = GetHandlers(viewmodel);

            //2- Drop the existing view
            foreach (Type handlerType in handlers)
            {
                var handler = ServiceLocator.Current.GetInstance(handlerType) as IViewModelDropper;
                handler.Drop(); 
            }

            //3- Setup event the bus for publishing events only for requested handlers
            IEventPublisher bus = new LocalEventBus(handlers);
            
            //4- Stream domain events to all retrieved handlers);
            foreach (var @event in _eventHistory.GetEvents())
            {
                bus.Publish(@event);
            }
        }

        private static IEnumerable<Type> GetHandlers(Type viewmodel)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(Handles<>)))
                .Where(handler => handler.GetConstructors().Any(c =>
                    c.GetParameters().Any(cp => cp.ParameterType.IsInterface && cp.ParameterType.IsGenericType && cp.ParameterType.GetGenericArguments()
                        .Any(ga => ga == viewmodel))));
        }
    }

    public class LocalEventBus : IEventPublisher
    {
        private IDictionary<Type, EventHandlerInvoker> eventHandlerInvokers;

        public LocalEventBus(IEnumerable<Type> eventHandlerTypes)
        {
            BuildEventInvokers(eventHandlerTypes);
        }

        public void Publish<T>(T @event) where T : Event
        {
            var domainEventType = @event.GetType();
            var invokers = (from entry in eventHandlerInvokers
                            where entry.Key.IsAssignableFrom(domainEventType)
                            select entry.Value).ToList();

            invokers.ForEach(i => i.Publish(@event));
        }

        private void BuildEventInvokers(IEnumerable<Type> eventHandlerTypes)
        {
            eventHandlerInvokers = new Dictionary<Type, EventHandlerInvoker>();
            foreach (var eventHandlerType in eventHandlerTypes)
            {
                foreach (var domainEventType in GetDomainEventTypes(eventHandlerType))
                {
                    EventHandlerInvoker eventInvoker;
                    if (!eventHandlerInvokers.TryGetValue(domainEventType, out eventInvoker))
                        eventInvoker = new EventHandlerInvoker(domainEventType);

                    eventInvoker.AddEventHandlerType(eventHandlerType);
                    eventHandlerInvokers[domainEventType] = eventInvoker;
                }
            }
        }

        private static IEnumerable<Type> GetDomainEventTypes(Type eventHandlerType)
        {
            return from interfaceType in eventHandlerType.GetInterfaces()
                   where interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(Handles<>)
                   select interfaceType.GetGenericArguments()[0];
        }

        private class EventHandlerInvoker
        {
            private readonly Type domainEventType;
            private readonly List<Type> eventHandlerTypes;

            public EventHandlerInvoker(Type domainEventType)
            {
                this.domainEventType = domainEventType;
                eventHandlerTypes = new List<Type>();
            }

            public void AddEventHandlerType(Type eventHandlerType)
            {
                eventHandlerTypes.Add(eventHandlerType);
            }

            public void Publish(Event domainEvent)
            {
                var handleMethod = typeof(Handles<>).MakeGenericType(domainEventType).GetMethod("Handle");
                foreach (var eventHandlerType in eventHandlerTypes)
                {
                    var eventHandler = ServiceLocator.Current.GetInstance(eventHandlerType);
                    handleMethod.Invoke(eventHandler, new object[] { domainEvent });
                }
            }
        }

    }
}

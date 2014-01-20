using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleCQRS;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace RanNanDoh.Infrastructure
{
    public static class Mongo
    {
        public const string CollectionName = "RanNanDohDomainEvents";
    }

    public class MongoEventStore : IEventStore, IEventHistory
    {
        private readonly string _conn;       
        private readonly IEventPublisher _publisher;

        public MongoEventStore(IEventPublisher publisher, string conn)
        {
            _publisher = publisher;
            _conn = conn;
            //ClearEventStore();
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            var database = MongoDatabase.Create(_conn);
            var collection = database.GetCollection<EventDescriptor>(Mongo.CollectionName);

            // 1. Get Events
            var eventList = GetEvents(aggregateId) ?? Enumerable.Empty<EventDescriptor>();

            // 2. blow up if concurrency issue found
            if(eventList.Count() > 0 &&
                eventList.Max(x => x.Version) != expectedVersion
                && expectedVersion != -1)
                throw new ConcurrencyException();

            // 3. Save that shit
            var i = expectedVersion;
            foreach (var @event in events)
            {
                i++;
                @event.Version = i;
                collection.Insert(new EventDescriptor(aggregateId, @event, @event.Version));
                
                // 4. Publish that shit
                _publisher.Publish(@event);
            }

            var evts = GetEvents(aggregateId);

        }

        public List<Event> GetEventsForAggregate(Guid aggregateId)
        {
            var eventList = GetEvents(aggregateId);
            if (eventList == null || eventList.Count == 0)
            {
                throw new AggregateNotFoundException();
            }
            return eventList.Select(desc => desc.EventData).OrderBy(x => x.Version).ToList();
        }

        private List<EventDescriptor> GetEvents(Guid aggregateId)
        {
            var database = MongoDatabase.Create(_conn);
            var collection = database.GetCollection<EventDescriptor>(Mongo.CollectionName);

            var query = Query.EQ("AggregateId", aggregateId.ToString());
            var events = collection.Find(query).ToList();

            return events;
        }

        private void ClearEventStore()
        {
            var database = MongoDatabase.Create(_conn);
            database.GetCollection<EventDescriptor>(Mongo.CollectionName).RemoveAll();
            throw new Exception("Cleared all event stores");
        }

        public List<Event> GetEvents()
        {
            var database = MongoDatabase.Create(_conn);
            var collection = database.GetCollection<EventDescriptor>(Mongo.CollectionName);

            return collection.FindAll().Select(desc => desc.EventData).OrderBy(x => x.Version).ToList();
        }
    }
}

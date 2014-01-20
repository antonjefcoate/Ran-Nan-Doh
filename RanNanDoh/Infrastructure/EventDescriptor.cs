using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SimpleCQRS;

namespace RanNanDoh.Infrastructure
{
    internal class EventDescriptor
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string AggregateId { get; set; }
        public Event EventData { get; set; }
        public int Version { get; set; }

        public EventDescriptor(Guid id, Event eventData, int version)
        {
            AggregateId = id.ToString();
            EventData = eventData;
            Version = version;
        }
    }
}
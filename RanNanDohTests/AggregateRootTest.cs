using System;
using System.Collections.Generic;

using NUnit.Framework;

using SimpleCQRS;

namespace RanNanDohUiTests
{
    
    public abstract class AggregateRootTestFixture<T> where T :IAggregateRoot, new()
    {
        protected Exception caught = null;
        protected T target;
        protected List<Event> events;
        protected abstract IEnumerable<Event> Given();
        protected abstract void When();

        protected abstract void SetUp();

        [SetUp]
        public void SetUpMethod()
        {
            this.SetUp();
            target = CreateSut();
            target.LoadsFromHistory(this.Given());
            try
            {
                this.When();
                events = new List<Event>(target.GetUncommittedChanges());
                
            }catch(Exception ex)
            {
                caught = ex;
            }
        }

        protected virtual T CreateSut()
        {
            return new T();
        }
    }
}

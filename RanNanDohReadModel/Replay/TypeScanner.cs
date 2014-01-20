using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RanNanDohReadModel.Replay
{
    public class TypeScanner<T> 
    {
        private readonly Assembly[] _assemblies;
        
        public TypeScanner(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public IEnumerable<Type> Scan()
        {
            Type desiredType = typeof(T);
            return _assemblies
                .SelectMany(a => a.GetTypes())
                .Where(desiredType.IsAssignableFrom)
                .Where(x => !x.IsAbstract);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using NUnit.Framework;

using RanNanDoh.ReadModels;

using RanNanDohReadModel.Replay;

namespace RanNanDohReadModelTests
{
    [TestFixture]
    public class TypeScannerTests
    {
        private Assembly _assembly;
        [SetUp]
        public void SetUp()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        [Test]
        public void CanScanViewModels()
        {
            //Arrange
            var target = new TypeScanner<IViewModel>(new[] { _assembly });

            //Act
            var handlers = target.Scan();

            //Assert
            CollectionAssert.Contains(handlers, typeof(FakeDto));
        }

        [Test]
        public void DoesNotReturnNonViewModels()
        {
            //Arrange
            var target = new TypeScanner<IViewModel>(new[] { _assembly });

            //Act
            var handlers = target.Scan();

            //Assert
            CollectionAssert.DoesNotContain(handlers, typeof(Dummy));
        }

        [Test]
        public void CanScanGenericTypes()
        {
            //Arrange
            var target = new TypeScanner<IViewModelWriter<FakeDto>>(new[] { this._assembly });

            //Act
            var handlers = target.Scan();

            //Assert
            CollectionAssert.Contains(handlers, typeof(FakeDtoWriter));
        }

        [Test]
        public void Scan_ShouldNotReturnAbstractTypes()
        {
            var target = new TypeScanner<IViewModel>(new[] { this._assembly });

            //Act
            var handlers = target.Scan();

            //Assert
            CollectionAssert.DoesNotContain(handlers, typeof(AbstractFakeDto));
            CollectionAssert.DoesNotContain(handlers, typeof(IViewModel));
        }

        [Test]
        public void MyTestMethod()
        {
            //Arrange
            var viewmodel = typeof(FakeDto);
            Type vmWriter = typeof(IViewModelWriter<>).MakeGenericType(viewmodel);
            Type scanner = typeof(TypeScanner<>).MakeGenericType(vmWriter);
            dynamic x = Activator.CreateInstance(scanner, new object[] { new[] { _assembly } });
            var result = x.Scan();
        }
    }

    public abstract class AbstractFakeDto : IViewModel
    {
        public Guid Id
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
    public class FakeDto : AbstractFakeDto{}
    public class Dummy{}

    public  class FakeDtoWriter : IViewModelWriter<FakeDto>
    {
        public void Update(Guid id, FakeDto viewModel)
        {
            throw new NotImplementedException();
        }

        public void Add(Guid id, FakeDto viewModel)
        {
            throw new NotImplementedException();
        }

        public void Drop()
        {
            throw new NotImplementedException();
        }
    }
   
}

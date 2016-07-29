using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkCrm.Core.Injector;
using Xunit;

namespace ThinkCrm.Core.Tests.InjectorTests
{
    public class InjectorServiceTest
    {
        [Fact]
        public void Prove_Injector_Creation_Works()
        {
            var injector = new InjectorService();

            injector.RegisterType<IDummyService, DummyService>();

            var getObj = injector.GetObject<IDummyService>();

            Assert.IsType<DummyService>(getObj);
            Assert.True(getObj.Callme());

        }

        [Fact]
        public void Prove_Injector_Implemented_Work()
        {
            var injector = new InjectorService();

            injector.RegisterType<IDummyService>(new DummyService());

            var getObj = injector.GetObject<IDummyService>();

            Assert.IsType<DummyService>(getObj);
            Assert.True(getObj.Callme());
        }

        [Fact]
        public void Prove_Double_Injection_Implemented_Throws_Argument_Exception()
        {
            var injector = new InjectorService();

            injector.RegisterType<IDummyService>(new DummyService());

            Assert.Throws<ArgumentException>(() => injector.RegisterType<IDummyService>(new DummyService()));      

        }

        [Fact]
        public void Prove_Double_Injection_Creation_Throws_Argument_Exception()
        {
            var injector = new InjectorService();

            injector.RegisterType<IDummyService, DummyService>();

            Assert.Throws<ArgumentException>(() => injector.RegisterType<IDummyService, DummyService>());


        }
    }
}

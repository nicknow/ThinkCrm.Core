using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkCrm.Core.Tests.InjectorTests
{
    class DummyService : IDummyService
    {
        public bool Callme()
        {
            return true;
        }
    }
}

using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
    public class DoSomething : ICommand
    {
        public string SomeProperty { get; set; }

    }
}

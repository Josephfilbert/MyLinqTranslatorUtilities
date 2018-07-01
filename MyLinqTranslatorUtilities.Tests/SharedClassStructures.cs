using System;
using System.Collections.Generic;
using System.Text;

namespace MyLinqTranslatorUtilities.Tests
{
    internal class SharedClassStructures
    {
        internal class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
        }

        internal class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Address Address { get; set; }

            public string SayHello() => "Hello world";
        }
    }
}

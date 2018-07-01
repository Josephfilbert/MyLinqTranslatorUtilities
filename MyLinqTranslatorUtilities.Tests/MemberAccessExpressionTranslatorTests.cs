using System;
using System.Linq.Expressions;
using Xunit;

namespace MyLinqTranslatorUtilities.Tests
{
    public class MemberAccessExpressionTranslatorTests
    {
        private class Address
        {
            public string Street { get; set; }
            public string City { get; set; }
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Address Address { get; set; }

            public string SayHello() => "Hello world";
        }

        [Fact]
        public void SingleLevelMemberAccessTest()
        {
            Expression<Func<Person, string>> personNameExpression = p => p.Name;
            var actual = MemberAccessExpressionTranslator.GetMemberExpression(personNameExpression);
            Assert.Equal("Name", actual);
        }

        [Fact]
        public void TwoLevelMemberAccessTest()
        {
            Expression<Func<Person, string>> addressStreetExpression = p => p.Address.Street;
            var actual = MemberAccessExpressionTranslator.GetMemberExpression(addressStreetExpression);
            Assert.Equal("Address.Street", actual);
        }

        [Fact]
        public void NoMemberAccessTest()
        {
            Expression<Func<Person, Person>> personExpression = p => p;
            Assert.Empty(MemberAccessExpressionTranslator.GetMemberExpression(personExpression));
        }

        [Fact]
        public void MethodCallExpression_ShouldThrowException()
        {
            Expression<Func<Person, string>> sayHelloExpression = p => p.SayHello();
            Assert.Throws<ArgumentException>(() => MemberAccessExpressionTranslator.GetMemberExpression(sayHelloExpression));
        }
    }
}

using System;
using System.Linq.Expressions;
using Xunit;

namespace MyLinqTranslatorUtilities.Tests
{
    public class MemberAccessExpressionTranslatorTests
    {
        [Fact]
        public void SingleLevelMemberAccessTest()
        {
            Expression<Func<SharedClassStructures.Person, string>> personNameExpression = p => p.Name;
            var actual = MemberAccessExpressionTranslator.GetMemberExpression(personNameExpression);
            Assert.Equal("Name", actual);
        }

        [Fact]
        public void TwoLevelMemberAccessTest()
        {
            Expression<Func<SharedClassStructures.Person, string>> addressStreetExpression = p => p.Address.Street;
            var actual = MemberAccessExpressionTranslator.GetMemberExpression(addressStreetExpression);
            Assert.Equal("Address.Street", actual);
        }

        [Fact]
        public void NoMemberAccessTest()
        {
            Expression<Func<SharedClassStructures.Person, SharedClassStructures.Person>> personExpression = p => p;
            Assert.Empty(MemberAccessExpressionTranslator.GetMemberExpression(personExpression));
        }

        [Fact]
        public void MethodCallExpression_ShouldThrowException()
        {
            Expression<Func<SharedClassStructures.Person, string>> sayHelloExpression = p => p.SayHello();
            Assert.Throws<ArgumentException>(() => MemberAccessExpressionTranslator.GetMemberExpression(sayHelloExpression));
        }
    }
}

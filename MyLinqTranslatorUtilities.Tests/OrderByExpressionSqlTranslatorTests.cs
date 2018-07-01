using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace MyLinqTranslatorUtilities.Tests
{
    public class OrderByExpressionSqlTranslatorTests
    {
        [Fact]
        public void OrderBySingleMemberTest()
        {
            Expression<Func<IQueryable<SharedClassStructures.Person>, IOrderedQueryable<SharedClassStructures.Person>>>
                sortExpression = p => p.OrderBy(p1 => p1.Name);
            var expected = "ORDER BY" + Environment.NewLine + "Name ASC" + Environment.NewLine;
            var actual = new OrderByExpressionSqlTranslator<SharedClassStructures.Person>(sortExpression).GetOrderBySqlStatement();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OrderByTwoMembersTest()
        {
            Expression<Func<IQueryable<SharedClassStructures.Person>, IOrderedQueryable<SharedClassStructures.Person>>>
                sortExpression = p => p.OrderBy(p1 => p1.Name).ThenByDescending(p1 => p1.Age);
            var expected = "ORDER BY" + Environment.NewLine + "Name ASC," + Environment.NewLine + "Age DESC" + Environment.NewLine;
            var actual = new OrderByExpressionSqlTranslator<SharedClassStructures.Person>(sortExpression).GetOrderBySqlStatement();
            Assert.Equal(expected, actual);
        }
    }
}

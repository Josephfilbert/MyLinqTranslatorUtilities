using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MyLinqTranslatorUtilities
{
    public static class ExpressionUtilities
    {
        /// <summary>
        /// Returns actual <see cref="Expression"/> from quoted Expression Tree
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> which its <see cref="ExpressionType"/> is <see cref="ExpressionType.Quote"/></param>
        /// <returns>An actual quoted <see cref="Expression"/></returns>
        public static Expression StripQuotes(this Expression expression)
        {
            var current = expression;
            while (current.NodeType == ExpressionType.Quote)
            {
                current = ((UnaryExpression) current).Operand;
            }

            return current;
        }
    }
}

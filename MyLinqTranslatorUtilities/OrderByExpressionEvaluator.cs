using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MyLinqTranslatorUtilities
{
    public class OrderByExpressionSqlTranslator<TModel>
    {
        private enum SortOrder
        {
            Ascending,
            Descending
        }

        private readonly Expression _expression;
        private readonly Queue<KeyValuePair<string, SortOrder>> _sortQueue;

        public OrderByExpressionSqlTranslator(
            Expression<Func<IQueryable<TModel>, IOrderedQueryable<TModel>>> expression)
        {
            _expression = expression;
            _sortQueue = new Queue<KeyValuePair<string, SortOrder>>();
        }

        public string GetOrderBySqlStatement()
        {
            //the initial statement must be a lambda
            var lambdaInput = (LambdaExpression) _expression;
            var lambdaBody = lambdaInput.Body;

            VisitExpression(lambdaBody);

            var resultBuilder = new StringBuilder();
            if (_sortQueue.Count > 0)
                resultBuilder.AppendLine("ORDER BY");

            while (_sortQueue.Count > 0)
            {
                var current = _sortQueue.Dequeue();
                resultBuilder.Append($"{current.Key} {GetSqlOrderDirectionFromEnum(current.Value)}");

                resultBuilder.AppendLine(_sortQueue.Count > 0 ? ", " : string.Empty);
            }

            return resultBuilder.ToString();
        }

        private static SortOrder GetSortOrderEnum(Expression expression)
        {
            if (!IsOrderByCall(expression, out var methodCall))
                throw new NotSupportedException("The expression contains method call other than Queryable.OrderBy, Queryable.ThenBy, Queryable.OrderByDescending, Queryable.ThenByDescending");

            switch (methodCall.Method.Name)
            {
                case "OrderBy":
                case "ThenBy":
                    return SortOrder.Ascending;

                case "OrderByDescending":
                case "ThenByDescending":
                    return SortOrder.Descending;

                default:
                    return SortOrder.Ascending;
            }
        }

        private static string GetSqlOrderDirectionFromEnum(SortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    return "ASC";
                case SortOrder.Descending:
                    return "DESC";
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOrder));
            }
        }

        //TODO: Try to refactor this, merge with VisitExpression method
        //private void BuildOrderByExpression(Expression node)
        //{
        //    MethodCallExpression orderByCall;
        //    if (!IsOrderByCall(node, out orderByCall))
        //        throw new NotSupportedException("The expression contains method call other than Queryable.OrderBy, Queryable.ThenBy, Queryable.OrderByDescending, Queryable.ThenByDescending");

        //    VisitExpression(orderByCall);
        //}

        private void VisitExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Parameter)
                return;

            if (!IsOrderByCall(expression, out var currentMethodCall))
                throw new NotSupportedException("The expression contains method call other than Queryable.OrderBy, Queryable.ThenBy, Queryable.OrderByDescending, Queryable.ThenByDescending");

            VisitExpression(currentMethodCall.Arguments[0]);

            var orderMember = (MemberExpression) ((LambdaExpression) currentMethodCall.Arguments[1].StripQuotes()).Body;

            //don't use MemberExpressionEvaluator we just need single level evaluation
            //var orderMemberName = MemberExpressionEvaluator.GetMemberExpression(orderMember);
            var orderMemberName = orderMember.Member.Name;

            //example it must be p1 => p1.Name, thus the Expression property must be a ParameterExpression type
            //if happened to be p1 => p1.Address.Street, The Expression Property of first access will be MemberExpression which isn't valid
            if (orderMember.Expression.NodeType != ExpressionType.Parameter)
                throw new NotSupportedException("Nested member access is not supported");

            _sortQueue.Enqueue(
                new KeyValuePair<string, SortOrder>(orderMemberName, GetSortOrderEnum(currentMethodCall)));
        }

        private static bool IsOrderByCall(Expression node, out MethodCallExpression methodCallExpression)
        {
            if (!(node is MethodCallExpression orderByCall))
            {
                methodCallExpression = null;
                return false;
            }

            var methodInfo = orderByCall.Method;

            methodCallExpression = orderByCall;
            return methodInfo.IsStatic && methodInfo.DeclaringType == typeof(Queryable)
                                       && ValidOrderByMethodNames.Contains(methodInfo.Name);
        }

        private static readonly ISet<string> ValidOrderByMethodNames = new HashSet<string>()
        {
            "OrderBy",
            "ThenBy",
            "OrderByDescending",
            "ThenByDescending"
        };

        public static string GetOrderBySqlStatement(
            Expression<Func<IQueryable<TModel>, IOrderedQueryable<TModel>>> sortExpression)
        {
            return new OrderByExpressionSqlTranslator<TModel>(sortExpression).GetOrderBySqlStatement();
        }
    }
}

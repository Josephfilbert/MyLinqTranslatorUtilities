using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MyLinqTranslatorUtilities
{
    /// <summary>
    /// Performs evaluation of accessed members and returns those members as string
    /// </summary>
    public class MemberAccessExpressionTranslator: ExpressionVisitor
    {
        private readonly Expression _expression;
        private readonly Stack<string> _members;
        private string _cachedResult;

        public MemberAccessExpressionTranslator(Expression memberExpression)
        {
            _expression = memberExpression;
            _members = new Stack<string>();
        }

        public override Expression Visit(Expression node)
        {
            if(!(node.NodeType == ExpressionType.MemberAccess || node.NodeType == ExpressionType.Parameter || node.NodeType == ExpressionType.Lambda))
                throw new ArgumentException(
                    "No other expression is allowed other than member access", nameof(node));

            return base.Visit(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _members.Push(node.Member.Name);
            Visit(node.Expression);
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Visit(node.Body);
            return node;
        }

        /// <summary>
        /// Returns accessed members expression in form of string.
        /// </summary>
        /// <returns><see cref="String"/> of member access expression</returns>
        public string GetMemberExpression()
        {
            if (_cachedResult != null)
                return _cachedResult;

            _members.Clear();
           
            
            Visit(_expression);

            var builder = new StringBuilder();
            while (_members.Count > 0)
            {
                builder.Append(_members.Pop());
                if (_members.Count > 0)
                    builder.Append('.');
            }

            var result = builder.ToString();
            _cachedResult = result;
            return result;
        }

        public static string GetMemberExpression(Expression memberExpression)
        {
            return new MemberAccessExpressionTranslator(memberExpression).GetMemberExpression();
        }
    }
}

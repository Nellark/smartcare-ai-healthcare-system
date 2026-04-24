using System.Linq.Expressions;

namespace SmartCare.Domain.Specifications;

public interface ISpecification<T, TResult>
{
    Expression<Func<T, bool>> ToExpression();
    Func<T, TResult> Selector { get; }
}

public interface ISpecification<T> : ISpecification<T, T>
{
}

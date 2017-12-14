using System;

namespace Xer.Cqrs.QueryStack.Resolvers
{
    public class ContainerQueryAsyncHandlerResolver : IQueryHandlerResolver
    {
        private readonly IContainerAdapter _containerAdapter;

        public ContainerQueryAsyncHandlerResolver(IContainerAdapter containerAdapter)
        {
            _containerAdapter = containerAdapter;
        }

        /// <summary>
        /// <para>Resolves an instance of IQueryAsyncHandler<TQuery, TResult> from the container</para>
        /// <para>and converts it to a query handler delegate which can be invoked to process the query.</para>
        /// </summary>
        /// <typeparam name="TQuery">Type of query which is handled by the query handler.</typeparam>
        /// <typeparam name="TResult">Type of query's result.</typeparam>
        /// <returns>Instance of <see cref="QueryHandlerDelegate{TResult}"/> which executes the query handler processing.</returns>
        public QueryHandlerDelegate<TResult> ResolveQueryHandler<TQuery, TResult>() where TQuery : class, IQuery<TResult>
        {
            try
            {
                IQueryAsyncHandler<TQuery, TResult> queryAsyncHandler = _containerAdapter.Resolve<IQueryAsyncHandler<TQuery, TResult>>();

                if (queryAsyncHandler == null)
                {
                    // No handlers are resolved. Throw exception.
                    throw NoQueryHandlerResolvedException<TQuery>();
                }

                return QueryHandlerDelegateBuilder.FromQueryHandler(queryAsyncHandler);
            }
            catch(Exception ex)
            {
                // No handlers are resolved. Throw exception.
                throw NoQueryHandlerResolvedException<TQuery>(ex);
            }
        }
            
        private static NoQueryHandlerResolvedException NoQueryHandlerResolvedException<TQuery>(Exception ex = null)
        {
            Type queryType = typeof(TQuery);

            if(ex != null)
            {
                return new NoQueryHandlerResolvedException($"Error occurred while trying to resolve command handler from the container to handle command of type: { queryType.Name }.", queryType, ex);
            }
            else
            {
                return new NoQueryHandlerResolvedException($"Unable to resolve command handler from the container to handle command of type: { queryType.Name }.", queryType);
            }
        }
    }
}
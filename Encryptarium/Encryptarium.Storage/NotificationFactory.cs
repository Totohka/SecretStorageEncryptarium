using BusinessLogic.Services.NotificationSenders.Implemention;
using BusinessLogic.Services.NotificationSenders.Interface;
using System.Linq.Expressions;
using System.Reflection.Metadata;

namespace Encryptarium.Storage
{
    public static class ChainConfigurator
    {
        public static IChainConfigurator<T> Chain<T>(this IServiceCollection services) where T : class
        {
            return new ChainConfiguratorImpl<T>(services);
        }

        public interface IChainConfigurator<T>
        {
            IChainConfigurator<T> Add<TImplementation>() where TImplementation : T;
            void Configure();
        }

        private class ChainConfiguratorImpl<T> : IChainConfigurator<T> where T : class
        {
            private readonly IServiceCollection _services;
            private List<Type> _types;
            private Type _interfaceType;

            public ChainConfiguratorImpl(IServiceCollection services)
            {
                _services = services;
                _types = new List<Type>();
                _interfaceType = typeof(T);
            }

            public IChainConfigurator<T> Add<TImplementation>() where TImplementation : T
            {
                var type = typeof(TImplementation);
                _types.Add(type);
                return this;
            }

            public void Configure()
            {
                if (_types.Count == 0)
                    throw new InvalidOperationException($"No implementation defined for {_interfaceType.Name}");

                foreach (var type in _types)
                {
                    ConfigureType(type);
                }
            }

            private void ConfigureType(Type currentType)
            {
                // gets the next type, as that will be injected in the current type
                var nextType = _types.SkipWhile(x => x != currentType).SkipWhile(x => x == currentType).FirstOrDefault();

                // Makes a parameter expression, that is the IServiceProvider x 
                var parameter = Expression.Parameter(typeof(IServiceProvider), "x");

                // получить конструктор с наибольшим количеством параметров. В идеале должен быть только 1 конструктор, но лучше перестраховаться.
                var ctor = currentType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).First();

                // для каждого параметра в конструкторе
                var ctorParameters = ctor.GetParameters().Select(p =>
                {
                    // проверьте, реализован ли в нем интерфейс. Вот как мы определяем, какой параметр ввести в следующий обработчик.
                    if (_interfaceType.IsAssignableFrom(p.ParameterType))
                    {
                        if (nextType is null)
                        {
                            // если следующего типа нет, текущий тип является последним в цепочке, поэтому он просто получает значение null
                            return Expression.Constant(null, _interfaceType);
                        }
                        else
                        {
                            // если есть, то мы вызываем Serviceprovider.GetRequiredService, чтобы разрешить следующий тип для нас
                            return Expression.Call(typeof(ServiceProviderServiceExtensions), "GetRequiredService", new Type[] { nextType }, parameter);
                        }
                    }
                    // этот параметр нас не волнует, поэтому мы просто просим GetRequiredService разрешить его за нас
                    return (Expression)Expression.Call(typeof(ServiceProviderServiceExtensions), "GetRequiredService", new Type[] { p.ParameterType }, parameter);
                });

                // круто, у нас установлены все параметры наших конструкторов, поэтому мы создаем "новое" выражение для его вызова.
                var body = Expression.New(ctor, ctorParameters.ToArray());

                // если текущий тип является первым в нашем списке, то мы регистрируем его по интерфейсу, в противном случае по конкретному типу
                var first = _types[0] == currentType;
                var resolveType = first ? _interfaceType : currentType;
                var expressionType = Expression.GetFuncType(typeof(IServiceProvider), resolveType);

                // наконец, мы можем создать наше выражение
                var expression = Expression.Lambda(expressionType, body, parameter);

                // скомпилировать его
                var compiledExpression = (Func<IServiceProvider, object>)expression.Compile();

                // и зарегистрируйте его в коллекции сервисов как transient
                _services.AddTransient(resolveType, compiledExpression);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RandomGenerator
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FromDistribution : Attribute
    {
        private Type type;
        private object[] args;

        public FromDistribution(Type distributionType, params object[] args)
        {
            this.type = distributionType;
            this.args = args;
        }
        //здесь создается экземпляр класса
        public IContinousDistribution Create()
        {
            try
            {
                return (IContinousDistribution)Activator.CreateInstance(type, args);
            }
            catch
            {
                throw new ArgumentException(type.FullName);
            }
        }
    }
    public class Generator<T>
    {
        public static Dictionary<PropertyInfo, Lazy<IContinousDistribution>> LazyDictionary;
        public Dictionary<PropertyInfo, IContinousDistribution> Dictionary;
        static Generator()
        {
            LazyDictionary = new Dictionary<PropertyInfo, Lazy<IContinousDistribution>>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (Attribute.GetCustomAttribute(property, typeof(FromDistribution)) is FromDistribution attribute)
                {
                    Lazy<IContinousDistribution> lazyDistribution = new Lazy<IContinousDistribution>(() => attribute.Create());
                    LazyDictionary.Add(property, lazyDistribution);
                }
                else
                {
                    LazyDictionary.Add(property, null);
                }
            }
        }
        public Generator()
        {
            this.Dictionary = new Dictionary<PropertyInfo, IContinousDistribution>();
        }
        public T Generate(Random random)
        {
            var generator = Activator.CreateInstance(typeof(T));
            var distibutionValue = 0.0;

            foreach (var key in LazyDictionary.Keys)
            {
                if (Dictionary.ContainsKey(key))
                {
                    distibutionValue = Dictionary[key].Generate(random);
                    key.SetValue(generator, distibutionValue);
                }
                else if (LazyDictionary[key] != null)
                {
                    distibutionValue = LazyDictionary[key].Value.Generate(random);
                    key.SetValue(generator, distibutionValue);
                }
            }

            return (T)generator;
        }
        public Temp<T> For(Expression<Func<T, object>> expression)
        {
            try
            {
                Type type = expression.GetType();
                Expression expressionBody = expression.Body;
                UnaryExpression unaryExpression = (UnaryExpression)expressionBody;
                MemberExpression memberExpression = (MemberExpression)unaryExpression.Operand;
                var name = memberExpression.Member.Name;
                PropertyInfo prop = typeof(T).GetProperty(name);
                if (prop == null)
                    throw new ArgumentException();
                return new Temp<T>(prop, this);
            }
            catch
            {
                throw new ArgumentException();
            }
        }
    }
    public class Temp<T>
    {
        private readonly PropertyInfo property;
        private readonly Generator<T> generator;
        public Temp(PropertyInfo property, Generator<T> generator)
        {
            this.property = property;
            this.generator = generator;
        }

        public Generator<T> Set(IContinousDistribution distribution)
        {
            generator.Dictionary.Add(this.property, distribution);
            return generator;
        }
    }
}
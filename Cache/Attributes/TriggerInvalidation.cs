using System;
using System.Reflection;
using PostSharp.Aspects;

namespace CacheAspect.Attributes
{

    #region

    #endregion

    public static partial class Cache
    {
        [Serializable]
        public class TriggerInvalidation : OnMethodBoundaryAspect
        {
            #region Fields

            private KeyBuilder _keyBuilder;

            #endregion

            #region Public Properties

            public KeyBuilder KeyBuilder => _keyBuilder ?? (_keyBuilder = new KeyBuilder());

            #endregion

            #region Constructors and Destructors

            public TriggerInvalidation(string groupName, CacheSettings settings, string parameterProperty)
            {
                KeyBuilder.GroupName = groupName;
                KeyBuilder.Settings = settings;
                KeyBuilder.ParameterProperty = parameterProperty;
            }

            public TriggerInvalidation(string groupName, CacheSettings settings) : this(groupName, settings, string.Empty)
            {
            }

            public TriggerInvalidation(string groupName) : this(groupName, CacheSettings.Default, string.Empty)
            {
            }

            public TriggerInvalidation() : this(string.Empty)
            {
            }

            #endregion

            #region Public Methods and Operators

            public override void CompileTimeInitialize(MethodBase method, AspectInfo aspectInfo)
            {
                KeyBuilder.MethodParameters = method.GetParameters();
                KeyBuilder.MethodName = $"{method.DeclaringType.FullName}.{method.Name}";
            }

            public sealed override void OnExit(MethodExecutionArgs args)
            {
                var key = KeyBuilder.BuildCacheKey(args.Instance, args.Arguments);
                if (CacheService.Cache.Contains(key))
                {
                    CacheService.Cache.Delete(key);
                }
                base.OnExit(args);
            }

            #endregion
        }
    }
}
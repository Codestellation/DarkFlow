﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Codestellation.DarkFlow.Misc;

namespace Codestellation.DarkFlow.Matchers
{
    public class ContentMatcher : IMatcher
    {
        private readonly string _template;
        private readonly List<Func<ITask, string>> _orderedGetters;
        private readonly Dictionary<Tuple<Type, string>, Func<ITask, string>> _cachedGetters;

        public ContentMatcher(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentException("Should be non empty not null string.", "template");
            }

            _template = template;
            _cachedGetters = new Dictionary<Tuple<Type, string>, Func<ITask, string>>();
            var matches = Regex.Matches(template, @"\{([^}]*)\}");

            var tokens = new List<string>(matches.Count);

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];

                tokens.Add(match.Value.Substring(1, match.Value.Length - 2).Trim());
                _template = _template.Replace(match.Value, "{" + i + "}");
            }

            _orderedGetters = new List<Func<ITask, string>>();

            foreach (var token in tokens)
            {
                if (token.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
                {
                    var environmentVariable = token.Substring(4, token.Length - 4);

                    environmentVariable = Environment
                        .GetEnvironmentVariables()
                        .Keys
                        .OfType<string>()
                        .SingleOrDefault(x => x.Equals(environmentVariable, StringComparison.OrdinalIgnoreCase));

                    if (environmentVariable == null)
                    {
                        var message = string.Format("Environment variable '{0}' from template '{1}' was not found", token, template);
                        throw new ArgumentException(message, template);
                    }

                    _orderedGetters.Add(task => Environment.GetEnvironmentVariable(environmentVariable));
                }
                else
                {
                    //Prevents closure on foreach variable
                    var memberName = token;
                    _orderedGetters.Add(task => GetMemberValue(task, memberName));
                }
            }

        }

        public MatchResult TryMatch(ITask task)
        {
            string[] args = _orderedGetters.Select(getFrom => getFrom(task)).ToArray();

            return MatchResult.Matches(string.Format(_template, args));
        }

        private string GetMemberValue(ITask task, string memberName)
        {
            var getter = _cachedGetters.GetOrAddThreadSafe(Tuple.Create(task.GetType(), memberName), BuildGetter);
            
            return getter(task);
        }

        private Func<ITask, string> BuildGetter(Tuple<Type,string> typeAndMemberName)
        {
            const BindingFlags bindingFlags =
                BindingFlags.Public |
                //BindingFlags.NonPublic | //Note sure it should be done this way.
                BindingFlags.FlattenHierarchy |
                BindingFlags.GetField |
                BindingFlags.GetProperty |
                BindingFlags.Instance;

            var memberName = typeAndMemberName.Item2;

            var memberInfo =
                typeAndMemberName.Item1
                    .GetMembers(bindingFlags)
                    .Where(x => x.MemberType == MemberTypes.Property || x.MemberType == MemberTypes.Field)
                    .SingleOrDefault(member => member.Name.Equals(memberName, StringComparison.OrdinalIgnoreCase));

            if (memberInfo != null)
            {
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        return BuildFieldGetter((FieldInfo)memberInfo);
                    case MemberTypes.Property:
                        return BuildPropertyGetter((PropertyInfo)memberInfo);
                }
                
            }
            var message = string.Format("Member '{0}' not found in '{1}'", memberName, typeAndMemberName.Item1);
            throw new InvalidOperationException(message);
        }

        private Func<ITask, string> BuildPropertyGetter(PropertyInfo property)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");

            var taskType  = property.DeclaringType;
            var castedTarget = Expression.Convert(instanceParameter, taskType);

            MemberExpression member = Expression.Property(castedTarget, property);

            var toStringMethodInfo = taskType.GetMethod("ToString");

            var toString = Expression.Call(member, toStringMethodInfo);

            var lambda = Expression.Lambda<Func<object, string>>(toString, instanceParameter);

            return lambda.Compile();
        }

        private Func<ITask, string> BuildFieldGetter(FieldInfo field)
        {
            var taskType = field.DeclaringType;

            ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");
            
            var castedTarget = Expression.Convert(instanceParameter, taskType);

            MemberExpression member = Expression.Field(castedTarget, field);

            var toStringMethodInfo = taskType.GetMethod("ToString");

            var toString = Expression.Call(member, toStringMethodInfo);

            var lambda = Expression.Lambda<Func<object, string>>(toString, instanceParameter);

            return lambda.Compile();
        }
    }
}
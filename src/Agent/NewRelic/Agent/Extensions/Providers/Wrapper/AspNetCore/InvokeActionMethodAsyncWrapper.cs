// Copyright 2020 New Relic, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using NewRelic.Agent.Api;
using NewRelic.Agent.Extensions.Providers.Wrapper;
using NewRelic.Reflection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewRelic.Providers.Wrapper.AspNetCore
{
    public class InvokeActionMethodAsyncWrapper : IWrapper
    {
        private Func<object, ControllerContext> _getControllerContext;
        private Func<object, ControllerContext> GetControllerContext(string typeName) { return _getControllerContext ?? (_getControllerContext = VisibilityBypasser.Instance.GenerateFieldReadAccessor<ControllerContext>("Microsoft.AspNetCore.Mvc.Core", typeName, "_controllerContext")); }

        private Func<object, ILogger> _getLogger;
        private Func<object, ILogger> GetLogger() { return _getLogger ?? (_getLogger = VisibilityBypasser.Instance.GenerateFieldReadAccessor<ILogger>("Microsoft.AspNetCore.Mvc.Core", "Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", "_logger")); }

        public bool IsTransactionRequired => true;

        public CanWrapResponse CanWrap(InstrumentedMethodInfo methodInfo)
        {
            return new CanWrapResponse("NewRelic.Providers.Wrapper.AspNetCore.InvokeActionMethodAsync".Equals(methodInfo.RequestedWrapperName));
        }

        public AfterWrappedMethodDelegate BeforeWrappedMethod(InstrumentedMethodCall instrumentedMethodCall, IAgent agent, ITransaction transaction)
        {
            if (instrumentedMethodCall.IsAsync)
            {
                transaction.AttachToAsync();
            }

            //handle the .NetCore 3.0 case where the namespace is Infrastructure instead of Internal.
            var controllerContext = GetControllerContext(instrumentedMethodCall.MethodCall.Method.Type.FullName).Invoke(instrumentedMethodCall.MethodCall.InvocationTarget);
            var actionDescriptor = controllerContext.ActionDescriptor;

            var transactionName = CreateTransactionName(actionDescriptor);

            transaction.SetWebTransactionName(WebTransactionType.MVC, transactionName, TransactionNamePriority.FrameworkHigh);

            //Framework uses ControllerType.Action for these metrics & transactions. WebApi is Controller.Action for both
            //Taking opinionated stance to do ControllerType.MethodName for segments. Controller/Action for transactions
            var controllerTypeName = controllerContext.ActionDescriptor.ControllerTypeInfo.Name;
            var methodName = controllerContext.ActionDescriptor.MethodInfo.Name;

            var segment = transaction.StartMethodSegment(instrumentedMethodCall.MethodCall, controllerTypeName, methodName);

            var logger = GetLogger().Invoke(instrumentedMethodCall.MethodCall.InvocationTarget);

            IDisposable handler = agent.Configuration.LogDecoratorEnabled ? InjectMetadata(logger, agent) : null;

            void onComplete(Task task)
            {
                handler.Dispose();
            }

            return Delegates.GetAsyncDelegateFor<Task>(agent, segment, false, onComplete, TaskContinuationOptions.None);
        }

        private static string CreateTransactionName(ControllerActionDescriptor actionDescriptor)
        {
            var controllerName = actionDescriptor.ControllerName;
            var actionName = actionDescriptor.ActionName;

            var transactionName = $"{controllerName}/{actionName}";

            foreach (var parameter in actionDescriptor.Parameters)
            {
                transactionName += "/{" + parameter.Name + "}";
            }

            return transactionName;
        }

        static IDisposable InjectMetadata(ILogger logger, IAgent agent)
        {
            if (logger != null)
            {
                var metaData = agent.GetLinkingMetadata();

                if (metaData != null)
                {
                    metaData.TryGetValue("entity.guid", out string entityGuid);
                    metaData.TryGetValue("hostname", out string hostName);
                    metaData.TryGetValue("trace.id", out string traceId);
                    metaData.TryGetValue("span.id", out string spanId);

                    var formattedString = $"NR-LINKING|{entityGuid}|{hostName}|{traceId}|{spanId}";

                    return logger.BeginScope(new Dictionary<string, string>()
                        {
                            {"NR-LINKING", formattedString}
                        });
                }
            }

            return null;
        }

    }
}

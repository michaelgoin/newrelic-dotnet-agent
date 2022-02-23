// Copyright 2020 New Relic, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using NewRelic.Agent.Api;
using NewRelic.Agent.Api.Experimental;
using NewRelic.Agent.Extensions.Providers.Wrapper;
using Microsoft.Extensions.Logging;
using NewRelic.Core;
using System.Collections.Generic;

namespace NewRelic.Providers.Wrapper.MicrosoftExtensionsLogging
{
    public class MicrosoftLoggingWrapper : IWrapper
    {
        public bool IsTransactionRequired => false;

        private const string WrapperName = "MicrosoftLoggingWrapper";

        public CanWrapResponse CanWrap(InstrumentedMethodInfo methodInfo)
        {
            return new CanWrapResponse(WrapperName.Equals(methodInfo.RequestedWrapperName));
        }

        public AfterWrappedMethodDelegate BeforeWrappedMethod(InstrumentedMethodCall instrumentedMethodCall, IAgent agent, ITransaction transaction)
        {
            var logLevel = (LogLevel)instrumentedMethodCall.MethodCall.MethodArguments[0];

            var formattedLogValues = instrumentedMethodCall.MethodCall.MethodArguments[2];

            var renderedMessage = formattedLogValues.ToString();
            var xapi = agent.GetExperimentalApi();

            xapi.RecordLogMessage("Microsoft.Logging.Extensions", DateTime.UtcNow, logLevel.ToString(), renderedMessage, agent.TraceMetadata.SpanId, agent.TraceMetadata.TraceId);

            IDisposable handle = null;

            if (agent.Configuration.LogDecoratorEnabled)
            {
                var logger = (ILogger)instrumentedMethodCall.MethodCall.InvocationTarget;
                var metaData = agent.GetLinkingMetadata();

                if (metaData != null)
                {
                    metaData.TryGetValue("entity.guid", out string entityGuid);
                    metaData.TryGetValue("hostname", out string hostName);
                    metaData.TryGetValue("trace.id", out string traceId);
                    metaData.TryGetValue("span.id", out string spanId);

                    var formattedString = $"NR-LINKING|{entityGuid}|{hostName}|{traceId}|{spanId}";

                    handle = logger.BeginScope(new Dictionary<string, string>()
                        {
                            {"NR-LINKING", formattedString}
                        });
                }
            }

            return Delegates.GetDelegateFor(onComplete: () => handle?.Dispose());
        }
    }
}

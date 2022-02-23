// Copyright 2020 New Relic, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using NewRelic.Agent.Api;
using NewRelic.Agent.Extensions.Providers.Wrapper;
using Microsoft.Extensions.Logging;
using NewRelic.Agent.Extensions.Logging;

namespace NewRelic.Providers.Wrapper.MicrosoftExtensionsLogging
{
    public class LoggerFactoryWrapper : IWrapper
    {
        public bool IsTransactionRequired => false;

        private const string WrapperName = "LoggerFactoryWrapper";

        public CanWrapResponse CanWrap(InstrumentedMethodInfo methodInfo)
        {
            return new CanWrapResponse(WrapperName.Equals(methodInfo.RequestedWrapperName));
        }

        public AfterWrappedMethodDelegate BeforeWrappedMethod(InstrumentedMethodCall instrumentedMethodCall, IAgent agent, ITransaction transaction)
        {
            var providers = (ILoggerProvider[]) instrumentedMethodCall.MethodCall.MethodArguments[0];

            if (providers.Length != 0)
            {
                foreach (var provider in providers)
                {
                    if (provider.ToString() == LogProviders.Log4NetProviderName)
                    {
                        LogProviders.RegisteredLogProvider[(int)LogProvider.Log4Net] = true;
                    }
                }
            }

            return Delegates.NoOp;
        }

    }
}

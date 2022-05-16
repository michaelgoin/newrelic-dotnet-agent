// Copyright 2020 New Relic, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2.0


using System.Collections.Generic;
using System.Linq;
using NewRelic.Agent.IntegrationTestHelpers;
using NewRelic.Agent.IntegrationTestHelpers.Models;
using NewRelic.Testing.Assertions;
using Xunit;
using Xunit.Abstractions;

namespace NewRelic.Agent.IntegrationTests.CodeLevelMetrics
{
    [NetFrameworkTest]
    public class MvcSpanAttributesTest : NewRelicIntegrationTest<RemoteServiceFixtures.DTBasicMVCApplicationFixture>
    {
        private readonly RemoteServiceFixtures.DTBasicMVCApplicationFixture _fixture;

        public MvcSpanAttributesTest(RemoteServiceFixtures.DTBasicMVCApplicationFixture fixture, ITestOutputHelper output) : base(fixture)
        {
            _fixture = fixture;
            _fixture.TestLogger = output;

            _fixture.Actions
            (
                setupConfiguration: () =>
                {
                    var configPath = fixture.DestinationNewRelicConfigFilePath;
                    var configModifier = new NewRelicConfigModifier(configPath);
                    configModifier.ForceTransactionTraces();
                },
                exerciseApplication: () =>
                {
                    //System.Threading.Thread.Sleep(20000);
                    _fixture.Initiate();
                }
            );
            _fixture.Initialize();
        }

        [Fact]
        public void Test()
        {
            var expectedUserCodeAttributes = new Dictionary<string, string>
            {
                { "code.namespace", "BasicMvcApplication.Controllers.DistributedTracingController" },
                { "code.function", "Initiate" }
            };

            var spanEvents = _fixture.AgentLog.GetSpanEvents().ToList();
            var rootSpanEvent = spanEvents.FirstOrDefault(se => se.IntrinsicAttributes.ContainsKey("nr.entryPoint"));

            // TODO: sync and async endpoints

            var userActionSpan = spanEvents.FirstOrDefault(se => se.IntrinsicAttributes["name"].ToString() == "DotNet/DistributedTracingController/Initiate");

            NrAssert.Multiple(
                () => Assertions.SpanEventHasAttributes(expectedUserCodeAttributes, SpanEventAttributeType.Agent, userActionSpan)
            );
        }
    }
}

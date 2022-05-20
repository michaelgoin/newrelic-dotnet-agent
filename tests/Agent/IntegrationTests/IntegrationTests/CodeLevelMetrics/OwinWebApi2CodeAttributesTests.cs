﻿// Copyright 2020 New Relic, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2.0


using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using NewRelic.Agent.IntegrationTestHelpers;
using NewRelic.Agent.IntegrationTestHelpers.Models;
using NewRelic.Testing.Assertions;

namespace NewRelic.Agent.IntegrationTests.CodeLevelMetrics
{
    [NetFrameworkTest]
    public abstract class OwinWebApi2CodeAttributesTestsBase<TFixture> : NewRelicIntegrationTest<TFixture>
        where TFixture : RemoteServiceFixtures.OwinWebApiFixture
    {
        private readonly RemoteServiceFixtures.OwinWebApiFixture _fixture;

        // The base test class runs tests for Owin 2; the derived classes test Owin 3 and 4
        protected OwinWebApi2CodeAttributesTestsBase(TFixture fixture, ITestOutputHelper output) : base(fixture)
        {
            _fixture = fixture;
            _fixture.TestLogger = output;
            _fixture.Actions
            (
                setupConfiguration: () =>
                {
                    var configModifier = new NewRelicConfigModifier(_fixture.DestinationNewRelicConfigFilePath);
                    configModifier.SetCodeLevelMetricsEnabled();
                },
                exerciseApplication: () =>
                {
                    _fixture.Get();
                    _fixture.Get404();
                    _fixture.Post();
                }
            );
            _fixture.Initialize();
        }

        [Fact]
        public void Test()
        {
            var expectedNamespace = $"{_fixture.AssemblyName}.Controllers.ValuesController";

            var expectedValuesGetAttributes = GetExpectedAttributesDefinition(expectedNamespace, "Get");
            var expectedValuesPostAttributes = GetExpectedAttributesDefinition(expectedNamespace, "Post");
            var expectedValuesGet404Attributes = GetExpectedAttributesDefinition(expectedNamespace, "Get404");

            var spanEvents = _fixture.AgentLog.GetSpanEvents().ToList();

            var valuesGetSpan = spanEvents.FirstOrDefault(se => se.IntrinsicAttributes["name"].ToString() == "DotNet/Values/Get");
            var valuesPostSpan = spanEvents.FirstOrDefault(se => se.IntrinsicAttributes["name"].ToString() == "DotNet/Values/Post");
            var valuesGet404Span = spanEvents.FirstOrDefault(se => se.IntrinsicAttributes["name"].ToString() == "DotNet/Values/Get404");

            NrAssert.Multiple
            (
                () => Assertions.SpanEventHasAttributes(expectedValuesGetAttributes, SpanEventAttributeType.Agent, valuesGetSpan),
                () => Assertions.SpanEventHasAttributes(expectedValuesPostAttributes, SpanEventAttributeType.Agent, valuesPostSpan),
                () => Assertions.SpanEventHasAttributes(expectedValuesGet404Attributes, SpanEventAttributeType.Agent, valuesGet404Span)
            );
        }

        private Dictionary<string, string> GetExpectedAttributesDefinition(string fullType, string methodName)
        {
            var expectedAttributes = new Dictionary<string, string>()
            {
                { "code.namespace", fullType },
                { "code.function", methodName }
            };

            return expectedAttributes;
        }
    }

    public class OwinWebApi2CodeAttributesTests : OwinWebApi2CodeAttributesTestsBase<RemoteServiceFixtures.OwinWebApiFixture>
    {
        public OwinWebApi2CodeAttributesTests(RemoteServiceFixtures.OwinWebApiFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }
    }

    public class Owin3WebApi2CodeAttributesTests : OwinWebApi2CodeAttributesTestsBase<RemoteServiceFixtures.Owin3WebApiFixture>
    {
        public Owin3WebApi2CodeAttributesTests(RemoteServiceFixtures.Owin3WebApiFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }
    }

    public class Owin4WebApi2CodeAttributesTests : OwinWebApi2CodeAttributesTestsBase<RemoteServiceFixtures.Owin4WebApiFixture>
    {
        public Owin4WebApi2CodeAttributesTests(RemoteServiceFixtures.Owin4WebApiFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }
    }

}

﻿// Copyright 2020 New Relic, Inc. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System.Text.RegularExpressions;
using MultiFunctionApplicationHelpers;
using NewRelic.Agent.IntegrationTestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace NewRelic.Agent.IntegrationTests.Logging
{
    public enum LayoutType
    {
        Pattern,
        Json
    }

    public abstract class LocalDecorationTestsBase<TFixture> : NewRelicIntegrationTest<TFixture>
        where TFixture : ConsoleDynamicMethodFixture
    {
        private readonly TFixture _fixture;
        private readonly bool _decorationEnabled;

        public LocalDecorationTestsBase(TFixture fixture, ITestOutputHelper output, bool decorationEnabled, LayoutType layoutType, LoggingFramework loggingFramework) : base(fixture)
        {
            _decorationEnabled = decorationEnabled;
            _fixture = fixture;
            _fixture.SetTimeout(System.TimeSpan.FromMinutes(2));
            _fixture.TestLogger = output;

            _fixture.AddCommand($"LoggingTester SetFramework {loggingFramework}");
            _fixture.AddCommand($"LoggingTester Configure{layoutType}LayoutAppenderForDecoration");
            _fixture.AddCommand($"LoggingTester CreateSingleLogMessage DecorateMe DEBUG");

            _fixture.Actions
            (
                setupConfiguration: () =>
                {
                    var configModifier = new NewRelicConfigModifier(fixture.DestinationNewRelicConfigFilePath);

                    configModifier
                    .EnableApplicationLogging()
                    .EnableLogDecoration(_decorationEnabled)
                    .EnableDistributedTrace()
                    .SetLogLevel("debug");
                }
            );

            _fixture.Initialize();
        }

        [Fact]
        public void LogIsDecorated()
        {
            // Sample decorated data we are looking for:
            // "NR-LINKING|MjczMDcwfEFQTXxBUFBMSUNBVElPTnwxODQyMg|blah.hsd1.ca.comcast.net|45f120972d61834b96fb890d2a8f97e7|840d9a82e8bc18a8|"
            var regex = new Regex(@"NR-LINKING\|[a-zA-Z0-9]*\|[a-zA-Z0-9._-]*\|[a-zA-Z0-9]*\|[a-zA-Z0-9]*\|");
            if (_decorationEnabled)
            {
                Assert.Matches(regex, _fixture.RemoteApplication.CapturedOutput.StandardOutput);
            }
            else
            {
                Assert.DoesNotMatch(regex, _fixture.RemoteApplication.CapturedOutput.StandardOutput);
            }
        }
    }

    #region log4net

    // Json layout, decoration enabled
    [NetFrameworkTest]
    public class Log4netJsonLayoutDecorationEnabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public Log4netJsonLayoutDecorationEnabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetFrameworkTest]
    public class Log4netJsonLayoutDecorationEnabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public Log4netJsonLayoutDecorationEnabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationEnabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public Log4netJsonLayoutDecorationEnabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationEnabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public Log4netJsonLayoutDecorationEnabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationEnabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public Log4netJsonLayoutDecorationEnabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationEnabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public Log4netJsonLayoutDecorationEnabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    // Json layout, decoration disabled
    [NetFrameworkTest]
    public class Log4netJsonLayoutDecorationDisabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public Log4netJsonLayoutDecorationDisabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetFrameworkTest]
    public class Log4netJsonLayoutDecorationDisabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public Log4netJsonLayoutDecorationDisabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationDisabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public Log4netJsonLayoutDecorationDisabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationDisabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public Log4netJsonLayoutDecorationDisabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationDisabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public Log4netJsonLayoutDecorationDisabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netJsonLayoutDecorationDisabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public Log4netJsonLayoutDecorationDisabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Log4net)
        {
        }
    }

    // Pattern layout, decoration enabled
    [NetFrameworkTest]
    public class Log4netPatternLayoutDecorationEnabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public Log4netPatternLayoutDecorationEnabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetFrameworkTest]
    public class Log4netPatternLayoutDecorationEnabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public Log4netPatternLayoutDecorationEnabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationEnabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public Log4netPatternLayoutDecorationEnabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationEnabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public Log4netPatternLayoutDecorationEnabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationEnabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public Log4netPatternLayoutDecorationEnabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationEnabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public Log4netPatternLayoutDecorationEnabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    // Pattern layout, decoration disabled
    [NetFrameworkTest]
    public class Log4netPatternLayoutDecorationDisabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public Log4netPatternLayoutDecorationDisabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetFrameworkTest]
    public class Log4netPatternLayoutDecorationDisabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public Log4netPatternLayoutDecorationDisabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationDisabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public Log4netPatternLayoutDecorationDisabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationDisabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public Log4netPatternLayoutDecorationDisabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationDisabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public Log4netPatternLayoutDecorationDisabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    [NetCoreTest]
    public class Log4netPatternLayoutDecorationDisabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public Log4netPatternLayoutDecorationDisabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Log4net)
        {
        }
    }

    #endregion

    #region Serilog

    #region Json layout, decoration enabled
    [NetFrameworkTest]
    public class SerilogJsonLayoutDecorationEnabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public SerilogJsonLayoutDecorationEnabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetFrameworkTest]
    public class SerilogJsonLayoutDecorationEnabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public SerilogJsonLayoutDecorationEnabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationEnabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public SerilogJsonLayoutDecorationEnabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationEnabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public SerilogJsonLayoutDecorationEnabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationEnabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public SerilogJsonLayoutDecorationEnabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationEnabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public SerilogJsonLayoutDecorationEnabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }
    #endregion

    #region Json layout, decoration disabled
    [NetFrameworkTest]
    public class SerilogJsonLayoutDecorationDisabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public SerilogJsonLayoutDecorationDisabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetFrameworkTest]
    public class SerilogJsonLayoutDecorationDisabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public SerilogJsonLayoutDecorationDisabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationDisabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public SerilogJsonLayoutDecorationDisabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationDisabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public SerilogJsonLayoutDecorationDisabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationDisabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public SerilogJsonLayoutDecorationDisabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogJsonLayoutDecorationDisabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public SerilogJsonLayoutDecorationDisabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Json, LoggingFramework.Serilog)
        {
        }
    }
    #endregion

    #region Pattern layout, decoration enabled

    [NetFrameworkTest]
    public class SerilogPatternLayoutDecorationEnabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public SerilogPatternLayoutDecorationEnabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetFrameworkTest]
    public class SerilogPatternLayoutDecorationEnabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public SerilogPatternLayoutDecorationEnabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationEnabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public SerilogPatternLayoutDecorationEnabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationEnabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public SerilogPatternLayoutDecorationEnabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationEnabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public SerilogPatternLayoutDecorationEnabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationEnabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public SerilogPatternLayoutDecorationEnabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, true, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    #endregion

    #region Pattern Layout, decoration disabled

    [NetFrameworkTest]
    public class SerilogPatternLayoutDecorationDisabledTestsFWLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFWLatest>
    {
        public SerilogPatternLayoutDecorationDisabledTestsFWLatestTests(ConsoleDynamicMethodFixtureFWLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetFrameworkTest]
    public class SerilogPatternLayoutDecorationDisabledTestsFW471Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureFW471>
    {
        public SerilogPatternLayoutDecorationDisabledTestsFW471Tests(ConsoleDynamicMethodFixtureFW471 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationDisabledTestsNetCoreLatestTests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCoreLatest>
    {
        public SerilogPatternLayoutDecorationDisabledTestsNetCoreLatestTests(ConsoleDynamicMethodFixtureCoreLatest fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationDisabledTestsNetCore50Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore50>
    {
        public SerilogPatternLayoutDecorationDisabledTestsNetCore50Tests(ConsoleDynamicMethodFixtureCore50 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationDisabledTestsNetCore31Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore31>
    {
        public SerilogPatternLayoutDecorationDisabledTestsNetCore31Tests(ConsoleDynamicMethodFixtureCore31 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    [NetCoreTest]
    public class SerilogPatternLayoutDecorationDisabledTestsNetCore22Tests : LocalDecorationTestsBase<ConsoleDynamicMethodFixtureCore22>
    {
        public SerilogPatternLayoutDecorationDisabledTestsNetCore22Tests(ConsoleDynamicMethodFixtureCore22 fixture, ITestOutputHelper output)
            : base(fixture, output, false, LayoutType.Pattern, LoggingFramework.Serilog)
        {
        }
    }

    #endregion


    #endregion



}

using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PowerBuild.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    public class MSBuildHelperTests
    {
        [Fact]
        public async Task ProcessRecord_Build()
        {
            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");

            var parameters = new InvokeMSBuildParameters
            {
                Project = project,
                Target = new[] { "Build" },
                ToolsVersion = "4.0"
            };

            var helper = new MSBuildHelper
            {
                Parameters = parameters,
            };

            helper.BeginProcessing();

            var buildResults = await helper.ProcessRecordAsync();
            helper.StopProcessing();

            var buildResult = buildResults.FirstOrDefault();
            Assert.NotNull(buildResult);
            Assert.True(buildResult.HasResultsForTarget("Build"));
            Assert.Equal(3, buildResult.Items.Count());
        }

        [Fact]
        public async Task ProcessRecord_BuildCrossDomain()
        {
            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");

            var factory = new Factory();
            var invokeAppDomain = factory.CreateInvokeAppDomain();
            var invokeFactory = factory.CreateInvokeFactory(invokeAppDomain);

            var helper = invokeFactory.CreateMSBuildHelper();

            var parameters = new InvokeMSBuildParameters
            {
                Project = project,
                Target = new[] { "Build" }
            };

            helper.Parameters = parameters;
            helper.BeginProcessing();

            await MarshalTask.FromAsync(helper.BeginProcessRecord, helper.EndProcessRecord);

            helper.StopProcessing();
            AppDomain.Unload(invokeAppDomain);
        }

        [Fact]
        public async Task ProcessRecord_ReturnProperty()
        {
            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");

            var itemSpec = "ExpectedReturnPropertyValue";
            var parameters = new InvokeMSBuildParameters
            {
                Project = project,
                Properties = new Dictionary<string, string>
                {
                    ["ReturnPropertyValue"] = itemSpec
                },
                Target = new[] { "ReturnProperty" },
                ToolsVersion = "4.0"
            };

            var helper = new MSBuildHelper
            {
                Parameters = parameters,
            };

            helper.BeginProcessing();

            var buildResults = await helper.ProcessRecordAsync();
            helper.StopProcessing();

            var buildResult = buildResults.Single();
            Assert.Equal(1, buildResult["ReturnProperty"].Items.Length);
            Assert.Equal(itemSpec, buildResult["ReturnProperty"].Items[0].ItemSpec);
        }
    }
}
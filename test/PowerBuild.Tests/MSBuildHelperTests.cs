using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Xunit;

namespace PowerBuild.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    public class MSBuildHelperTests
    {
        [Fact]
        public async Task ProcessRecord_BuildCrossDomain()
        {
            Directory.GetCurrentDirectory();

            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");
            var configurationFile = Path.Combine(Directory.GetCurrentDirectory(), Assembly.GetExecutingAssembly().GetName().Name + ".dll.config");
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = Directory.GetCurrentDirectory(),
                ConfigurationFile = configurationFile
            };
            var appDomain = AppDomain.CreateDomain("powerbuild", AppDomain.CurrentDomain.Evidence, appDomainSetup);

            var helper = (MSBuildHelper)appDomain.CreateInstanceAndUnwrap(typeof(MSBuildHelper).Assembly.FullName, typeof(MSBuildHelper).FullName);

            var parameters = new InvokeMSBuildParameters
            {
                Project = project,
                Target = new[] { "Build" }
            };

            helper.Parameters = parameters;
            helper.BeginProcessing();

            await MarshalTask.FromAsync(helper.BeginProcessRecord, helper.EndProcessRecord);

            helper.StopProcessing();
            AppDomain.Unload(appDomain);
        }

        [Fact]
        public async Task ProcessRecord_Build()
        {
            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");

            var parameters = new InvokeMSBuildParameters
            {
                Project = project,
                Target = new[] { "Build" }
            };

            var helper = new MSBuildHelper
            {
                Parameters = parameters
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
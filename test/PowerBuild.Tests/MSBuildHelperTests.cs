using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Xunit;

namespace PowerBuild.Tests
{
    public class MSBuildHelperTests
    {
        [Fact]
        public async Task ProcessRecordCrossDomainTest()
        {
            Directory.GetCurrentDirectory();

            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");
            var configurationFile = Path.Combine(Directory.GetCurrentDirectory(), Assembly.GetExecutingAssembly().GetName().Name + ".dll.config");
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(typeof(MSBuildHelper).Assembly.Location),
                ConfigurationFile = configurationFile
            };
            var appDomain = AppDomain.CreateDomain("powerbuild", AppDomain.CurrentDomain.Evidence, appDomainSetup);

            var helper = (MSBuildHelper)appDomain.CreateInstanceAndUnwrap(typeof(MSBuildHelper).Assembly.FullName, typeof(MSBuildHelper).FullName);

            var parameters = new InvokeMSBuildParameters
            {
                Project = new[] { project },
                Target = new[] { "Build" },
                ToolsVersion = "14.0"
            };

            helper.Parameters = parameters;
            helper.BeginProcessing();

            await MarshalTask.FromAsync(helper.BeginProcessRecord, helper.EndProcessRecord);

            helper.StopProcessing();
            AppDomain.Unload(appDomain);
        }

        [Fact]
        public async Task ProcessRecordTest()
        {
            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");

            var parameters = new InvokeMSBuildParameters
            {
                Project = new[] { project },
                Target = new[] { "Build" },
                ToolsVersion = "14.0"
            };

            var helper = new MSBuildHelper
            {
                Parameters = parameters
            };

            helper.BeginProcessing();

            await helper.ProcessRecordAsync();

            helper.StopProcessing();
        }
    }
}
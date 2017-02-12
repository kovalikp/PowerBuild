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
            var cmdletHelper = new CmdletHelper();
            Directory.GetCurrentDirectory();

            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");
            var configurationPath = Path.Combine(Directory.GetCurrentDirectory(), Assembly.GetExecutingAssembly().GetName().Name + ".dll.config");
            AppDomain appDomain;
            var helper = MSBuildHelper.CreateCrossDomain(configurationPath, out appDomain);

            helper.Project = new[] { project };
            helper.Target = new[] { "Build" };
            helper.ToolVersion = "14.0";

            helper.BeginProcessing();

            await MarshalTask.FromAsync(helper.BeginProcessRecord, helper.EndProcessRecord);

            helper.StopProcessing();
            AppDomain.Unload(appDomain);
        }

        [Fact]
        public async Task ProcessRecordTest()
        {
            var cmdletHelper = new CmdletHelper();

            var project = Path.Combine(Environment.CurrentDirectory, "PowerBuild.Tests.targets");

            var helper = new MSBuildHelper
            {
                Project = new[] { project },
                Target = new[] { "Build" },
                ToolVersion = "14.0"
            };

            helper.BeginProcessing();

            await helper.ProcessRecordAsync();

            helper.StopProcessing();
        }
    }
}
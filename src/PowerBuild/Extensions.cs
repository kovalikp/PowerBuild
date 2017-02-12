using System.Threading.Tasks;
using Microsoft.Build.Execution;

namespace PowerBuild
{
    internal static class Extensions
    {
        internal static Task<BuildResult> ExecuteAsync(this BuildSubmission submission)
        {
            var tcs = new TaskCompletionSource<BuildResult>();
            submission.ExecuteAsync(s => tcs.SetResult(s.BuildResult), null);
            return tcs.Task;
        }
    }
}
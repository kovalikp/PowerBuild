using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PowerBuild
{
    public class CmdletHelper : MarshalByRefObject
    {
        private BlockingCollection<MessageContainer> _buildEvents = new BlockingCollection<MessageContainer>();

        public void AddBuildEvent(MessageContainer buildEventArgs)
        {
            _buildEvents.Add(buildEventArgs);
        }

        public IEnumerable<MessageContainer> ConsumeBuildEvents()
        {
            return _buildEvents.GetConsumingEnumerable();
        }

        public void SetCanceled()
        {
        }

        public void SetCompleted()
        {
            _buildEvents.CompleteAdding();
        }

        public void SetFaulted(Exception exception)
        {
        }
    }
}
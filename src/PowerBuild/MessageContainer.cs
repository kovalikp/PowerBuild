using System;
using Microsoft.Build.Framework;

namespace PowerBuild
{
    [Serializable]
    public class MessageContainer
    {
        public BuildEventArgs BuildEvent { get; set; }

        public ConsoleColor Color { get; set; }

        public string FormattedMessage { get; set; }
    }
}
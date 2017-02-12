using System;
using Microsoft.Build.BackEnd.Logging;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;

namespace PowerBuild
{
    internal class PowerShellLogger : SerialConsoleLogger
    {
        private readonly CmdletHelper _cmdletHelper;
        private readonly ConsoleColor _defaultForegroundColor;
        private BuildEventArgs _buildEventArgs;
        private ConsoleColor _consoleColor;

        public PowerShellLogger(LoggerVerbosity verbosity, CmdletHelper cmdletHelper, ConsoleColor defaultForegroundColor)
            : base(verbosity)
        {
            _cmdletHelper = cmdletHelper;
            _defaultForegroundColor = defaultForegroundColor;
            WriteHandler = PSWriteHandler;
            setColor = PSSetColor;
            resetColor = PSResetColor;
        }

        public override void ErrorHandler(object sender, BuildErrorEventArgs e)
        {
            _buildEventArgs = e;
            base.ErrorHandler(sender, e);
            _buildEventArgs = null;
        }

        /// <summary>
        /// Prints a warning event
        /// </summary>
        public override void WarningHandler(object sender, BuildWarningEventArgs e)
        {
            _buildEventArgs = e;
            base.WarningHandler(sender, e);
            _buildEventArgs = null;
        }

        private void PSResetColor()
        {
            _consoleColor = _defaultForegroundColor;
        }

        private void PSSetColor(ConsoleColor consoleColor)
        {
            _consoleColor = consoleColor;
        }

        private void PSWriteHandler(string message)
        {
            _cmdletHelper.AddBuildEvent(new MessageContainer
            {
                FormattedMessage = message,
                Color = _consoleColor,
                BuildEvent = _buildEventArgs
            });
        }
    }
}
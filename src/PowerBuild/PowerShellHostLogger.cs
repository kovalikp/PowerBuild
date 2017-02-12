namespace PowerBuild
{
    using System;

    using System.Management.Automation;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;

    public class PowerShellHostLogger : INodeLogger
    {
        private readonly PSCmdlet _cmdlet;
        private ConsoleLogger _consoleLogger;
        private ConsoleColor _defaultForegroundColor;
        private ConsoleColor _foregroundColor;

        public PowerShellHostLogger(LoggerVerbosity verbosity, PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
            _defaultForegroundColor = cmdlet.Host.UI.RawUI.ForegroundColor;
            _consoleLogger = new ConsoleLogger(verbosity, WriteHandler, ColorSet, ColorReset);
        }

        public string Parameters
        {
            get { return _consoleLogger.Parameters; }
            set { _consoleLogger.Parameters = value; }
        }

        public LoggerVerbosity Verbosity
        {
            get { return _consoleLogger.Verbosity; }
            set { _consoleLogger.Verbosity = value; }
        }

        public void Initialize(IEventSource eventSource)
        {
            _consoleLogger.Initialize(eventSource);
        }

        public void Initialize(IEventSource eventSource, int nodeCount)
        {
            _consoleLogger.Initialize(eventSource, nodeCount);
        }

        public void Shutdown()
        {
            _consoleLogger.Shutdown();
        }

        private void ColorReset()
        {
            _foregroundColor = _defaultForegroundColor;
        }

        private void ColorSet(ConsoleColor color)
        {
            _foregroundColor = color;
        }

        private void WriteHandler(string message)
        {
            _cmdlet.Host.UI.Write(_foregroundColor, _cmdlet.Host.UI.RawUI.BackgroundColor, message);
        }
    }
}
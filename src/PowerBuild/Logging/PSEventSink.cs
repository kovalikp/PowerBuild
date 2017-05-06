// Copyright (c) 2017 Pavol Kovalik. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PowerBuild.Logging
{
    using System;
    using System.Collections.Concurrent;
    using System.Management.Automation;

    internal class PSEventSink : IPSEventSink
    {
        private readonly BlockingCollection<object> _eventActions = new BlockingCollection<object>();
        private ConsoleColor _backgroundColor;
        private PSCmdlet _cmdlet;
        private ConsoleColor _foregroundColor;

        public PSEventSink(PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
            _backgroundColor = cmdlet.Host.UI.RawUI.BackgroundColor;
            _foregroundColor = cmdlet.Host.UI.RawUI.ForegroundColor;
        }

        private enum MessageType
        {
            Host,
            Verbose,
            Warning
        }

        public void ColorReset()
        {
            _eventActions.Add(new SetColor(_foregroundColor));
        }

        public void ColorSet(ConsoleColor color)
        {
            _eventActions.Add(new SetColor(color));
        }

        public void CompleteWriting()
        {
            _eventActions.CompleteAdding();
        }

        public void ConsumeEvents()
        {
            var color = _foregroundColor;

            foreach (var eventAction in _eventActions.GetConsumingEnumerable())
            {
                switch (eventAction)
                {
                    case Message message when message.Type == MessageType.Verbose:
                        _cmdlet.WriteVerbose(message.Text);
                        break;

                    case Message message when message.Type == MessageType.Host:
                        _cmdlet.Host.UI.Write(color, _backgroundColor, message.Text);
                        break;

                    case Message message when message.Type == MessageType.Warning:
                        _cmdlet.WriteWarning(message.Text);
                        break;

                    case SetColor setColor:
                        color = setColor.Color;
                        break;

                    case ErrorMessage error:
                        _cmdlet.WriteError(error.Error);
                        break;
                }
            }
        }

        public void WriteError(ErrorRecord error)
        {
            _eventActions.Add(new ErrorMessage(error));
        }

        public void WriteHost(string message)
        {
            _eventActions.Add(new Message(MessageType.Host, message));
        }

        public void WriteVerbose(string message)
        {
            _eventActions.Add(new Message(MessageType.Verbose, message));
        }

        public void WriteWarning(string message)
        {
            _eventActions.Add(new Message(MessageType.Warning, message));
        }

        private class ErrorMessage
        {
            public ErrorMessage(ErrorRecord error)
            {
                Error = error;
            }

            public ErrorRecord Error { get; }
        }

        private class Message
        {
            public Message(MessageType type, string text)
            {
                Text = text;
                Type = type;
            }

            public string Text { get; }

            public MessageType Type { get; }
        }

        private class SetColor
        {
            public SetColor(ConsoleColor color)
            {
                Color = color;
            }

            public ConsoleColor Color { get; }
        }
    }
}
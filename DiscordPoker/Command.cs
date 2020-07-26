using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;

namespace DiscordPoker {

    public class Command {

        public string Activator { get; private set; }
        public Action<SocketMessage> Function { get; private set; }

        public Command(string activator, Action<SocketMessage> func) {
            Function = func;
            Activator = activator;
        }
    }
}

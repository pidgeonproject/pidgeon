using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gtk;

namespace Client
{
    public class PidgeonTc : Client.Extension
    {
        public override void Initialise()
        {
            Name = "Input tab completion for the pidgeon";
            Description = "This plugin enable you to use tab completion like in terminal";
            Version = "1.0.60";
            base.Initialise();
        }

        public override void Hook_InputOnTab(ref string prev, ref string text, ref int caret, ref bool restore)
        {
            if (caret < 1) return;
            if (text == "") return;
            int x = caret - 1;
            if (x > text.Length)
            { x = text.Length - 2; }
            while (x >= 0)
            {
                if (text[x] == '\n' || text[x] == ' ')
                {
                    x++;
                    break;
                }
                x--;
            }
            if (x < 0)
            { x = 0; }
            string text2 = text.Substring(x);
            if (text2.Contains(" "))
            {
                text2 = text2.Substring(0, text2.IndexOf(" "));
            }

            // check if it's a command :)

            List<string> commands = new List<string>();

            lock (Commands.commands)
            {
                foreach (KeyValuePair<string, Commands.Command> cm in Commands.commands)
                {
                    if (cm.Value._Type == Commands.Type.System || cm.Value._Type == Commands.Type.Plugin)
                    {
                        commands.Add(cm.Key);
                    }
                }

                if (Core.network != null && Core.network.IsConnected)
                {
                    foreach (KeyValuePair<string, Commands.Command> cm in Commands.commands)
                    {
                        if (cm.Value._Type == Commands.Type.SystemSv || cm.Value._Type == Commands.Type.Network)
                        {
                            commands.Add(cm.Key);
                        }
                    }
                }

                if (Core.SystemForm.Chat._Protocol != null)
                {
                    foreach (KeyValuePair<string, Commands.Command> cm in Commands.commands)
                    {
                        if (cm.Value._Type == Commands.Type.Services)
                        {
                            commands.Add(cm.Key);
                        }
                    }
                }

                if (text2.StartsWith(Configuration.CommandPrefix))
                {
                    List<string> Results = new List<string>();
                    string Resd = "";
                    foreach (var item in commands)
                    {
                        if (item.StartsWith(text2.Substring(1).ToLower()))
                        {
                            Resd += item + ", ";
                            Results.Add(item);
                        }
                    }
                    if (Results.Count > 1)
                    {
                        Core.SystemForm.Chat.scrollback.InsertText(messages.get("autocomplete-result", Core.SelectedLanguage, new List<string> { Resd }), ContentLine.MessageStyle.System);
                        string part = "";
                        int curr = 0;
                        bool match = true;
                        while (match)
                        {
                            char diff = ' ';
                            foreach (var item in Results)
                            {
                                if (item.Length > curr && diff == ' ')
                                {
                                    diff = item[curr];
                                    continue;
                                }
                                if (item.Length <= curr || diff != item[curr])
                                {
                                    match = false;
                                    break;
                                }
                            }
                            if (match)
                            {
                                curr = curr + 1;
                                part += diff.ToString();
                            }
                        }
                        string result = text;
                        result = result.Substring(0, x + 1);
                        result = result + part + text.Substring(x + text2.Length);
                        text = result;
                        caret = result.Length;
                        prev = result;
                        return;
                    }
                    if (Results.Count == 1)
                    {
                        string result = text;
                        result = result.Substring(0, x + 1);
                        result = result + Results[0] + " " + text.Substring(x + text2.Length);
                        text = result;
                        caret = result.Length;
                        prev = result;
                        return;
                    }
                }

                if (Core.network == null)
                    return;

                ////


                if (text2.StartsWith(Core.network.channel_prefix))
                {
                    if (Core.network.IsConnected)
                    {

                        if (text2.StartsWith(Core.SystemForm.Chat._Network.channel_prefix))
                        {
                            List<string> Channels = new List<string>();
                            foreach (Channel n in Core.SystemForm.Chat._Network.Channels)
                            {
                                Channels.Add(n.Name);
                            }
                            List<string> Results = new List<string>();
                            string Resd = "";
                            foreach (var item in Channels)
                            {
                                if (item.StartsWith(text2))
                                {
                                    Resd += item + ", ";
                                    Results.Add(item);
                                }
                            }
                            if (Results.Count > 1)
                            {
                                Core.SystemForm.Chat.scrollback.InsertText(messages.get("autocomplete-result", Core.SelectedLanguage, new List<string> { Resd }), ContentLine.MessageStyle.System);
                                string part = "";
                                int curr = 0;
                                bool match = true;
                                while (match)
                                {
                                    char diff = ' ';
                                    foreach (var item in Results)
                                    {
                                        if (item.Length > curr && diff == ' ')
                                        {
                                            diff = item[curr];
                                            continue;
                                        }
                                        if (item.Length <= curr || diff != item[curr])
                                        {
                                            match = false;
                                            break;
                                        }
                                    }
                                    if (match)
                                    {
                                        curr = curr + 1;
                                        part += diff.ToString();
                                    }
                                }
                                string result = text;
                                result = result.Substring(0, x);
                                result = result + part + text.Substring(x + text2.Length);
                                text = result;
                                caret = result.Length;
                                prev = result;
                                return;
                            }
                            if (Results.Count == 1)
                            {
                                string result = text;
                                result = result.Substring(0, x);
                                result = result + Results[0] + " " + text.Substring(x + text2.Length);
                                text = result;
                                caret = result.Length;
                                prev = result;
                                return;
                            }
                        }
                    }

                }


                // check if it's a nick

                List<string> Results2 = new List<string>();
                string Resd2 = "";
                if (Core.network.RenderedChannel == null) { return; }
                lock (Core.network.RenderedChannel.UserList)
                {
                    foreach (var item in Core.network.RenderedChannel.UserList)
                    {
                        if ((item.Nick.ToUpper()).StartsWith(text2.ToUpper()))
                        {
                            Resd2 += item.Nick + ", ";
                            Results2.Add(item.Nick);
                        }
                    }
                }
                if (Results2.Count == 1)
                {
                    string result = text;
                    result = result.Substring(0, x);
                    result = result + Results2[0] + text.Substring(x + text2.Length);
                    text = result;

                    caret = result.Length;

                    prev = result;
                    return;
                }

                if (Results2.Count > 1)
                {
                    Core.SystemForm.Chat.scrollback.InsertText(messages.get("autocomplete-result", Core.SelectedLanguage, new List<string> { Resd2 }), ContentLine.MessageStyle.System);
                    string part = "";
                    int curr = 0;
                    char orig = ' ';
                    bool match = true;
                    while (match)
                    {
                        char diff = ' ';
                        foreach (string item in Results2)
                        {
                            string value = item.ToLower();
                            if (diff == ' ' && item.Length > curr)
                            {
                                orig = item[curr];
                                diff = value[curr];
                                continue;
                            }
                            if (item.Length <= curr || diff != value[curr])
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match)
                        {
                            curr = curr + 1;
                            part += orig.ToString();
                        }
                    }
                    string result = text;
                    result = result.Substring(0, x);
                    result = result + part + text.Substring(x + text2.Length);
                    text = result;
                    caret = result.Length;
                    prev = result;


                    return;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class SearchText : Extension
    {
        string text = null;

        public override void Initialise()
        {
            Name = "Search";
            Description = "Search using wikipedia etc menu when you right click selected text";
            Version = "1.0.0.0";
            base.Initialise();
        }

        public void SearchGoogle(object sender, EventArgs e)
        {
            if (text == null)
            {
                return;
            }

            Hyperlink.OpenLink("http://google.com/?q=" + System.Web.HttpUtility.UrlEncode(text));
        }

        public void SearchWiki(object sender, EventArgs e)
        {
            if (text == null)
            {
                return;
            }

            Hyperlink.OpenLink("http://en.wikipedia.org/w/index.php?search=" + System.Web.HttpUtility.UrlEncode(text));
        }

        public override void Hook_BeforeTextMenu(Extension.ScrollbackArgs Args)
        {
            text = Args.scrollback.SelectedText;
            Gtk.SeparatorMenuItem xx = new Gtk.SeparatorMenuItem();
            xx.Show();
            Args.menu.Add(xx);
            Gtk.MenuItem wiki = new Gtk.MenuItem("Search using wiki");
            wiki.Activated += new EventHandler(SearchWiki);
            wiki.Show();
            Args.menu.Add(wiki);
            Gtk.MenuItem goog = new Gtk.MenuItem("Search using google");
            goog.Show();
            goog.Activated += new EventHandler(SearchGoogle);
            Args.menu.Add(goog);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public partial class SBABox
    {
        public class Link
        {
            public int X = 0;
            public int Y = 0;
            public int Width = 0;
            public int Height = 0;
            private SBABox Parent = null;
            public string Name = null;
            /// <summary>
            /// URL
            /// </summary>
            public string Text = null;
            public ContentText linkedtext = null;

            public Link(int x, int y, System.Drawing.Color normal, int width, int height, SBABox SBAB, string http, string label, ContentText text)
            {
                X = x;
                Y = y;
                Width = width;
                Name = label;
                Text = http;
                Parent = SBAB;
                linkedtext = text;
                Height = height;
            }

            ~Link()
            {
                Dispose();
            }

            public void Dispose()
            {
                Parent = null;
                linkedtext = null;
                Name = null;
                Text = null;
            }
        }
    }
}

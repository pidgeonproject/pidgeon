/***************************************************************************
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) version 3.                                           *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   GNU General Public License for more details.                          *
 *                                                                         *
 *   You should have received a copy of the GNU General Public License     *
 *   along with this program; if not, write to the                         *
 *   Free Software Foundation, Inc.,                                       *
 *   51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.         *
 ***************************************************************************/


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
	public partial class SBABox
    {
		/*
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                lock (vScrollBar1)
                {
                    if (Configuration.Scrollback.DynamicBars)
                    {
                        if (e.NewValue == 0 && scrollback != null)
                        {
                            if (scrollback.IncreaseOffset())
                            {
                                if (vScrollBar1.Maximum > Configuration.Scrollback.DynamicSize + 10)
                                {
                                    vScrollBar1.Value = Configuration.Scrollback.DynamicSize;
                                    ScrolltoX(100);
                                    Redraw();
                                }
                                return;
                            }
                        }
                    }
                    ScrolltoX(e.NewValue);
                    Redraw();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            try
            {
                lock (hsBar)
                {
                    currentX = hsBar.Value;
                    Redraw();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void ChangeSize(object sender, EventArgs e)
        {
            try
            {
                InvalidateCaches();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        protected void RepaintWindow(object sender, PaintEventArgs e)
        {
            try
            {
                if (isDisposing)
                {
                    return;
                }
                if (backbufferGraphics != null)
                {
                    backbufferGraphics.Render(e.Graphics);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void Wheeled(Object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta != 0)
                {
                    try
                    {
                        lock (vScrollBar1)
                        {
                            int delta = (e.Delta / 16) * -1;
                            if (delta < 0)
                            {
                                if ((vScrollBar1.Value + delta) > vScrollBar1.Minimum)
                                {
                                    vScrollBar1.Value = vScrollBar1.Value + delta;
                                }
                                else
                                {
                                    vScrollBar1.Value = vScrollBar1.Minimum;
                                }
                            }

                            if (delta > 0)
                            {
                                if ((vScrollBar1.Value + delta) < vScrollBar1.Maximum)
                                {
                                    vScrollBar1.Value = vScrollBar1.Value + delta;
                                }
                                else
                                {
                                    vScrollBar1.Value = vScrollBar1.Maximum;
                                }
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        vScrollBar1.Value = vScrollBar1.Maximum;
                    }
                    if (Configuration.Scrollback.DynamicBars)
                    {
                        if (vScrollBar1.Value == 0 && scrollback != null)
                        {
                            if (scrollback.IncreaseOffset())
                            {
                                if (vScrollBar1.Maximum > Configuration.Scrollback.DynamicSize + 10)
                                {
                                    vScrollBar1.Value = Configuration.Scrollback.DynamicSize;
                                    ScrolltoX(100);
                                    Redraw();
                                }
                                return;
                            }
                        }
                        else if (vScrollBar1.Value == vScrollBar1.Maximum && scrollback != null)
                        {
                            if (scrollback.RestoreOffset())
                            {
                                return;
                            }
                        }

                    }
                    ScrolltoX(vScrollBar1.Value);
                    Redraw();
                    return;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            try
            {
                base.OnResize(e);
                RecreateBuffers();
                Redraw();
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        public void ClickHandler(Object sender, MouseEventArgs e)
        {
            try
            {
                Link httparea = null;
                httparea = getLink(e.X, e.Y);
                if (httparea != null)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        scrollback.Click_R(httparea.Name, httparea.Text);
                        return;
                    }

                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        scrollback.Click_L(httparea.Text);
                        return;
                    }
                }
                if (scrollback.owner != null)
                {
                    scrollback.owner.textbox.setFocus();
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
        */
    }
}

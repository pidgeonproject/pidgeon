//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or   
//  (at your option) version 3.                                         

//  This program is distributed in the hope that it will be useful,     
//  but WITHOUT ANY WARRANTY; without even the implied warranty of      
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the       
//  GNU General Public License for more details.                        

//  You should have received a copy of the GNU General Public License   
//  along with this program; if not, write to the                       
//  Free Software Foundation, Inc.,                                     
//  51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

// updater - this code works only in windows, also it isn't capable to handle modern versions of pidgeon, so we need
// to implement a lot of new functions here

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace Client
{
    /// <summary>
    /// Form
    /// </summary>
    public partial class Updater : Form
    {
        /// <summary>
        /// Creates new instance of this updater
        /// </summary>
        public Updater()
        {
            InitializeComponent();
        }

        private static string info = "";
        /// <summary>
        /// Finalize
        /// </summary>
        public bool finalize = false;
        private static string temporarydir = "";
        private static string message = null;
        private static string link = null;

        private void Updater_Load(object sender, EventArgs e)
        {
            try
            {
                Focus();
                BringToFront();
                if (finalize)
                {
                    progressBar1.Visible = false;
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private static bool Download(string file, string where)
        {
            try
            {
                System.Net.WebClient _b = new System.Net.WebClient();
                _b.DownloadFile(file, where);
                return true;
            }
            catch (Exception fail)
            {
                Core.DebugLog("Failed to download " + fail.ToString());
            }
            return false;
        }

        /// <summary>
        /// Run
        /// </summary>
        public static void Run()
        {
            try
            {
                if (Configuration.CurrentPlatform == Core.Platform.Windowsx64 ||Configuration.CurrentPlatform == Core.Platform.Windowsx86 ||
                    Configuration.CurrentPlatform == Core.Platform.Linuxx64 || Configuration.CurrentPlatform == Core.Platform.Linuxx86)
                {
                    if (Configuration.Kernel.CheckUpdate)
                    {
                        Core.Ringlog("UPTH: Checking for updates...");
                        temporarydir = System.IO.Path.GetTempPath() + "pidgeon" + DateTime.Now.ToBinary().ToString();
                        if (System.IO.Directory.Exists(temporarydir))
                        {
                            return;
                        }
                        System.IO.Directory.CreateDirectory(temporarydir);
                        if (Download(Configuration.Kernel.UpdaterUrl + "&type=" + System.Web.HttpUtility.UrlEncode(Configuration.CurrentPlatform.ToString()), temporarydir + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat"))
                        {
                            info = System.IO.File.ReadAllText(temporarydir + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat");
                            if (info.Contains("[update-need]"))
                            {
                                Core.Ringlog("UPTH: update is needed");
                                string vr = info.Substring(info.IndexOf("version:") + "version:".Length);
                                vr = vr.Substring(0, vr.IndexOf("^"));
                                Updater updater = new Updater();
                                if (info.Contains("message|"))
                                {
                                    string[]lines = System.IO.File.ReadAllLines(temporarydir + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat");
                                    foreach (string line in lines)
                                    {
                                        if (line.StartsWith("link|"))
                                        {
                                            link = line.Substring(5);
                                            continue;
                                        }
                                        if (line.StartsWith("message|"))
                                        {
                                            message = line.Substring(8);
                                            continue;
                                        }
                                    }
                                    if (message != null && link != null)
                                    {
                                        updater.update.Text = messages.get("update-update");
                                        updater.lStatus.Text = "New version of pidgeon: " + vr + " is available! " + message;
                                        updater.update.Enabled = false;
                                        updater.lUpdateLink.Text = link;
                                        updater.lUpdateLink.Visible = true;
                                        System.Windows.Forms.Application.Run(updater);
                                        return;
                                    }
                                }
                                updater.update.Text = messages.get("update-update");
                                updater.lStatus.Text = messages.get("update1", Core.SelectedLanguage, new List<string> { vr });
                                System.Windows.Forms.Application.Run(updater);
                                return;
                            }
                            Core.Ringlog("UPTH: No update is needed");
                        }
                        System.IO.Directory.Delete(temporarydir, true);
                    }
                    else
                    {
                        Core.Ringlog("UPTH: Updater is not enabled, shutting down");
                    }
                }
            }
            catch (Exception _t)
            {
                Core.handleException(_t);
            }
        }

        private void UpdateLink_Click(object sender, EventArgs e)
        {
            try
            {
                Hyperlink.OpenLink(link);
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void update_Click(object sender, EventArgs e)
        {
            try
            {
                update.Enabled = false;
                progressBar1.Maximum = info.Split('\n').Length;
                string tempdir_ = System.IO.Path.GetTempPath() + "pidgeon_" + DateTime.Now.ToBinary().ToString();
                progressBar1.Value = 1;
                System.IO.Directory.CreateDirectory(tempdir_);
                foreach (string line in info.Split('\n'))
                {
                    if (line.Contains("|"))
                    {
                        string[] x = line.Split('|');
                        switch (x[0])
                        {
                            case "copy":
                                System.IO.File.Copy(x[1].Replace("!1", tempdir_).Replace("!2", temporarydir).Replace("/",
                                    System.IO.Path.DirectorySeparatorChar.ToString()), x[2].Replace("!1", tempdir_).Replace("!2", temporarydir).Replace("/",
                                    System.IO.Path.DirectorySeparatorChar.ToString()));
                                break;
                            case "deleteall":
                                System.IO.Directory.Delete(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + x[1], true);
                                break;
                            case "deletefile":
                                System.IO.File.Delete(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + x[1]);
                                break;
                            case "download":
                                lStatus.Text += "/n Downloading " + x[2] + "";
                                if (!Download(x[2], temporarydir + System.IO.Path.DirectorySeparatorChar + x[1].Replace("!1", tempdir_).Replace("!2", temporarydir)))
                                {
                                    lStatus.Text += "/n Unable to get " + x[2] + "";
                                    return;
                                }
                                break;
                            case "md":
                                System.IO.Directory.CreateDirectory(System.IO.Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + x[1]);
                                break;
                            case "done":
                                if (!System.IO.File.Exists(tempdir_ + System.IO.Path.DirectorySeparatorChar + "pidgeon.exe"))
                                {
                                    System.IO.File.Copy(System.Windows.Forms.Application.ExecutablePath, tempdir_ + System.IO.Path.DirectorySeparatorChar + "pidgeon.exe");
                                }
                                info = info + "\n\n\n\ntemp2: " + tempdir_ + "^";
                                info = info + "temp: " + temporarydir + "^";
                                info = info + "previous: " + System.Windows.Forms.Application.StartupPath;
                                System.IO.File.WriteAllText(tempdir_ + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat", info);
                                System.Diagnostics.Process.Start(tempdir_ + System.IO.Path.DirectorySeparatorChar + "pidgeon.exe");
                                // we need to quit application as fast as possible
                                Core.IgnoreErrors = true;
                                Core._Configuration.ConfigSave();
                                Environment.Exit(0);
                                return;
                        }
                        if (progressBar1.Value < progressBar1.Maximum)
                        {
                            progressBar1.Value++;
                        }
                    }
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                if (finalize)
                {
                    string up = System.IO.File.ReadAllText(System.Windows.Forms.Application.StartupPath + System.IO.Path.DirectorySeparatorChar + "pidgeon.dat");
                    string td = up.Substring(up.IndexOf("temp: ") + 6);
                    td = td.Substring(0, td.IndexOf("^"));
                    string main = up.Substring(up.IndexOf("previous: ") + 10);
                    System.Threading.Thread.Sleep(10000);
                    foreach (string line in up.Split('\n'))
                    {
                        if (line.Contains("|"))
                        {
                            string[] x = line.Split('|');
                            if (x[0] == "aftercopy")
                            {
                                System.IO.File.Copy(x[1].Replace("!1", System.Windows.Forms.Application.StartupPath).Replace("!0", main).Replace("!2", td).Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()),
                                    x[2].Replace("!1", System.Windows.Forms.Application.StartupPath).Replace("!2", td).Replace("!0", main).Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()), true);
                            }
                            if (x[0] == "afterdeleteall")
                            {
                                if (System.IO.Directory.Exists(x[1].Replace("!1", System.Windows.Forms.Application.StartupPath).Replace("!2", td).Replace("!0", main).Replace("/", System.IO.Path.DirectorySeparatorChar.ToString())))
                                {
                                    System.IO.Directory.Delete(x[1].Replace("!1", System.Windows.Forms.Application.StartupPath).Replace("!2", td).Replace("!0", main).Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()), true);
                                }
                            }
                            if (x[0] == "afterdeletefile")
                            {
                                System.IO.File.Delete(x[1].Replace("!1", System.Windows.Forms.Application.StartupPath).Replace("!2", td).Replace("!0", main).Replace("/", System.IO.Path.DirectorySeparatorChar.ToString()));
                            }
                        }
                    }

                    Process _pro = new Process();
                    if (Configuration.CurrentPlatform == Core.Platform.Windowsx86 || Configuration.CurrentPlatform == Core.Platform.Windowsx64)
                    {
                        _pro.StartInfo.Verb = "runas";
                        _pro.StartInfo.UseShellExecute = true;
                        _pro = System.Diagnostics.Process.Start(main + System.IO.Path.DirectorySeparatorChar + "pidgeon.exe");
                    }
                    if (Configuration.CurrentPlatform == Core.Platform.MacOSx86)
                    {
                        _pro = System.Diagnostics.Process.Start(main + System.IO.Path.DirectorySeparatorChar + "pidgeon");
                    }
                    if (Configuration.CurrentPlatform == Core.Platform.Linuxx86 || Configuration.CurrentPlatform == Core.Platform.Linuxx64)
                    {
                        _pro.StartInfo.UseShellExecute = true;
                        _pro = System.Diagnostics.Process.Start(main + System.IO.Path.DirectorySeparatorChar + "pidgeon");
                    }
                    Core.IgnoreErrors = true;
                    Environment.Exit(0);
                }
            }
            catch (Exception fail)
            {
                Core.handleException(fail);
            }
        }
    }
}

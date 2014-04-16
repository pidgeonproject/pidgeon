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

namespace Stetic
{
    internal static class Gui
    {
        private static bool initialized;
        
        internal static void Initialize (Gtk.Widget iconRenderer)
        {
            if ((Stetic.Gui.initialized == false)) {
                Stetic.Gui.initialized = true;
            }
        }
    }
    
    internal class BinContainer
    {
        private Gtk.Widget child;
        private Gtk.UIManager uimanager;
        
        public static BinContainer Attach (Gtk.Bin bin)
        {
            BinContainer bc = new BinContainer ();
            bin.SizeRequested += new Gtk.SizeRequestedHandler (bc.OnSizeRequested);
            bin.SizeAllocated += new Gtk.SizeAllocatedHandler (bc.OnSizeAllocated);
            bin.Added += new Gtk.AddedHandler (bc.OnAdded);
            return bc;
        }
        
        private void OnSizeRequested (object sender, Gtk.SizeRequestedArgs args)
        {
            if ((this.child != null)) {
                args.Requisition = this.child.SizeRequest ();
            }
        }
        
        private void OnSizeAllocated (object sender, Gtk.SizeAllocatedArgs args)
        {
            if ((this.child != null)) {
                this.child.Allocation = args.Allocation;
            }
        }
        
        private void OnAdded (object sender, Gtk.AddedArgs args)
        {
            this.child = args.Widget;
        }
        
        public void SetUiManager (Gtk.UIManager uim)
        {
            this.uimanager = uim;
            this.child.Realized += new System.EventHandler (this.OnRealized);
        }
        
        private void OnRealized (object sender, System.EventArgs args)
        {
            if ((this.uimanager != null)) {
                Gtk.Widget w;
                w = this.child.Toplevel;
                if (((w != null) && typeof(Gtk.Window).IsInstanceOfType (w))) {
                    ((Gtk.Window)(w)).AddAccelGroup (this.uimanager.AccelGroup);
                    this.uimanager = null;
                }
            }
        }
    }
    
    internal class ActionGroups
    {
        public static Gtk.ActionGroup GetActionGroup (System.Type type)
        {
            return Stetic.ActionGroups.GetActionGroup (type.FullName);
        }
        
        public static Gtk.ActionGroup GetActionGroup (string name)
        {
            return null;
        }
    }
}

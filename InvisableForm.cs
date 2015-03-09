using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Management.Instrumentation;
using System.Collections.Specialized;
using System.Threading;

namespace HDDLED_v2
{
    public partial class InvisableForm : Form
    {

        #region Members
        NotifyIcon hddNotifyIcon;
        Icon busyIcon;
        Icon idleIcon;
        Thread hddInfoWorker;
        #endregion

        #region Main Form (Entry Point)
        public InvisableForm()
        {
            InitializeComponent();

            // Load icons from files into objects
            busyIcon = new Icon("HDD_Busy.ico");
            idleIcon = new Icon("HDD_Idle.ico");

            // Create notify icons and assign idle icon and show it
            hddNotifyIcon = new NotifyIcon();
            hddNotifyIcon.Icon = idleIcon;
            hddNotifyIcon.Visible = true;

            // Create all context menu items and add them to notification tray icon
            MenuItem progNameMenuItem = new MenuItem("HDD LED by: Fergoman123");
            MenuItem breakMenuItem = new MenuItem("-");
            MenuItem quitMenuItem = new MenuItem("Quit");
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(progNameMenuItem);
            contextMenu.MenuItems.Add(breakMenuItem);
            contextMenu.MenuItems.Add(quitMenuItem);
            hddNotifyIcon.ContextMenu = contextMenu;

            // Wire up quit button to close application
            quitMenuItem.Click += quitMenuItem_Click;

            // 
            //  Hide the form because we don't need it, this is a notification tray application
            //
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Opacity = 0.0f;

            // Start worker thread that pulls HDD activity
            hddInfoWorker = new Thread(new ThreadStart(HddActivityThread));
            hddInfoWorker.Start();
        }
        #endregion

        void quitMenuItem_Click(object sender, EventArgs e)
        {
            hddInfoWorker.Abort();
            hddNotifyIcon.Dispose();
            this.Close();
        }

        public void HddActivityThread()
        {
            ManagementClass driveData = new ManagementClass("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");

            try
            {
                while(true)
                {
                    ManagementObjectCollection driveDataCollection = driveData.GetInstances();
                    foreach(ManagementObject obj in driveDataCollection)
                    {
                        if(obj["Name"].ToString() == "_Total")
                        {
                            if(Convert.ToUInt64(obj["DiskBytesPersec"]) > 0)
                            {
                                hddNotifyIcon.Icon = busyIcon;
                            }
                            else
                            {
                                hddNotifyIcon.Icon = idleIcon;
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
            }
            catch(ThreadAbortException e)
            {
                driveData.Dispose();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams pm = base.CreateParams;
                pm.ExStyle |= 0x80;
                return pm;
            }
        }

    }
}

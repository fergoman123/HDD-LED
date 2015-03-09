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
        }
        #endregion

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

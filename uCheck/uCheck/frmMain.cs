using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uCheck
{
    public partial class frmMain : Form
    {
        
        public static Boolean autoExport = true;
        public static List<String> users = new List<String>(); // Unchecked users.
        public static List<String> availUsers = new List<String>(); // Checked users.

        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Aesthetics
            MaximumSize = Size;
            MinimumSize = Size;

            // Preload Configs
            enumConfigs();
            if(cbConfig.Items.Count > 0)
            {
                cbConfig.Text = cbConfig.Items[0].ToString();
            }
            else
            {
                cbConfig.Text = "";
                logEvent("WARNING: No Configurations Loaded.");
                Program.Form1.btnStart.Hide();
            }

        }

        // Load Configurations
        private void enumConfigs()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => enumConfigs()));
            }else
            {
                try
                {
                    checkConfig.globalConfigs = new List<checkConfig.checkConf>(); // Reset the global configurations
                    foreach (String config in System.IO.Directory.GetFiles(Application.StartupPath, "*.ini"))
                    {
                        checkConfig.loadConfig(config);
                    }
                    foreach(checkConfig.checkConf Config in checkConfig.globalConfigs)
                    {
                        cbConfig.Items.Add(Config.Site);
                    }
                }
                catch (Exception ex)
                {
                    eventLogger.logEvent(String.Format("{0} - {1} [{2}] | {3}", ex.Message, "Configuration Precaching", ex.StackTrace, ex.TargetSite));
                }
            }
        }

        public static void logEvent(String eventDetails) // Keeps the user Informed.
        {
            if (Program.Form1.InvokeRequired)
            {
                Program.Form1.BeginInvoke(new MethodInvoker(() => frmMain.logEvent(eventDetails)));
            }
            else
            {
                ListViewItem logEvent = new ListViewItem(DateTime.Now.ToString("hh:mm:ss tt"));
                logEvent.SubItems.Add(eventDetails);
                Program.Form1.listView1.Items.Add(logEvent);
                if(Program.Form1.listView1.Items.Count == 34)
                {
                    Program.Form1.listView1.Items[0].Remove();
                }
            }
        }

        private void autoExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoExport = autoExportToolStripMenuItem.Checked;
            toolStripMenuItem1.Enabled = !autoExportToolStripMenuItem.Checked;
            logEvent(String.Format("Auto Export: {0}", autoExport));
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) // Manual Exporting
        {
            export(cbConfig.Text);
            logEvent("Users Exported!");
        }

        private void export(String configName)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => export(configName)));
            }else
            {
                try
                {
                    if(availUsers.Count == 0)
                    {
                        logEvent("No Available Accounts!");
                    }
                    else
                    {
                        foreach(String Checked in availUsers)
                        {
                            Exporter.logAvailable(Checked, configName);
                        }
                        logEvent("Accounts Logged!");
                    }
                }catch(Exception ex)
                {
                    eventLogger.logEvent(String.Format("{0} - {1} [{2}] | {3}", ex.Message, "Manual Export Module", ex.StackTrace, ex.TargetSite));
                }
                
             }
        }

        private void loadListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //Threaded Loading, Otherwise GL with large lists.
                    Thread thread = new Thread(() => enumArray(ofd.FileName));
                    thread.IsBackground = true; 
                    thread.Start();
                }
            }
        }

        private void enumArray(String filePath) // Loads users
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => enumArray(filePath)));
            }
            else
            {
                foreach (String entry in System.IO.File.ReadAllLines(filePath))
                {
                    users.Add(entry);
                }
                frmMain.logEvent("Population complete!");
                btnStart.Enabled = true;
                toolStripStatusLabel1.Text = String.Format("{0} Names Loaded", users.Count);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            pbVal.Value = 0; // Resets so the user doesn't have to restart the program.

            if (users.Count != 0 & cbConfig.Text != "")
            {
                btnStart.Enabled = false;
                
                logEvent("Checking!");
                foreach (checkConfig.checkConf config in checkConfig.globalConfigs)
                {
                    if(cbConfig.Text == config.Site)
                    {
                        Invoke(new MethodInvoker(() => checker.doCheck(config)));
                    }
                }
            }
        }

        public static void increment()
        {
            if (Program.Form1.InvokeRequired)
            {
                Program.Form1.pbVal.Invoke(new MethodInvoker(increment));
            }
            else
            {
                Program.Form1.pbVal.Increment(1);

            }
        }

        private void clearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            users.Clear();
            availUsers.Clear();
            btnStart.Enabled = false;
        }

        public static void fillBar()
        {
            if (Program.Form1.InvokeRequired)
            {
                Program.Form1.pbVal.Invoke(new MethodInvoker(fillBar));
            }
            else
            {
                Program.Form1.btnStart.Enabled = true;
                Program.Form1.pbVal.Value = Program.Form1.pbVal.Maximum;
            }
        }

        private void refreshConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cbConfig.Items.Clear();
            enumConfigs();
            if (cbConfig.Items.Count > 0)
            {
                cbConfig.Text = cbConfig.Items[0].ToString();
            }
            else
            {
                cbConfig.Text = "";
                logEvent("WARNING: No Configurations Loaded.");
                Program.Form1.btnStart.Hide();
            }
        }
    }
 }

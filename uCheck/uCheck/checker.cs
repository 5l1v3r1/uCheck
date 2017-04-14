using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uCheck
{
    // Yet another random snippette from another project.
    class Exporter
    {
        public static void logAvailable(String user, String Config)
        {
            try
            {
                System.IO.File.AppendAllText(String.Format("{0}\\Available.{1}.txt", Environment.CurrentDirectory, Config), String.Format("{0}{1}", user, Environment.NewLine));
            }
            catch (Exception ex)
            {
                eventLogger.logEvent(String.Format("{0} - {1} [{2}] | {3}", ex.Message, "Logging Module", ex.StackTrace, ex.TargetSite));
            }
        }
    }

    class checker
    {
        
        // Our checking loop.
        public static void check(checkConfig.checkConf Config)
        {
            try
            {
                foreach (String name in frmMain.users)
                {
                    if ((Boolean)WebRequest_Wrapper.Request(Config, name)) // If the name is available.
                    {
                        if (frmMain.autoExport)
                        {
                            Exporter.logAvailable(name, Config.Site);
                        }
                        else
                        {
                            frmMain.availUsers.Add(name);
                        }
                    }
                    frmMain.increment(); // Moves our ProgressBar.
                }

                frmMain.fillBar(); // Fills the ProgressBar.

                frmMain.logEvent("Done!");
            }
            
            catch (Exception ex)
            {
                eventLogger.logEvent(String.Format("{0} - {1} [{2}] | {3}", ex.Message, "Checking Module", ex.StackTrace, ex.TargetSite));
            }
        }

        // Starts a thread for the Checker.
        public static void doCheck(checkConfig.checkConf conf)
        {
            System.Threading.Thread thdCheck = new System.Threading.Thread(() => check(conf));
            thdCheck.IsBackground = true;
            thdCheck.Start();
        }


    }
}

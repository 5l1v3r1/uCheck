using System;

namespace uCheck
{
    // Random snippette from another project.
    class eventLogger
    {
        public static void logEvent(String eventDetails)
        {
            System.IO.File.AppendAllText(String.Format("{0}\\{1}.txt", Environment.CurrentDirectory, DateTime.Now.ToString("MM-dd-yy")), String.Format("[{0}] - {1}{2}", DateTime.Now.ToString("hh:mm:ss tt"), eventDetails, Environment.NewLine));
        }
    }
}

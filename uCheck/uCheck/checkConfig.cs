using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace uCheck
{
    class checkConfig
    {

        // Lazy but quick way to store Configurations.
        public static List<checkConf> globalConfigs;

        // The structure of our Configurations.
        public struct checkConf
        {
            public String Site;
            public String URL;
            public String SuccessString;
            public String FailString;
            public String POSTData;
            public Boolean successOn404;
        }

        public static void loadConfig(String path)
        {
            // Shouldn't have any issues. Shouldn't. But, Just incase.
            try
            {

                // Declare a new Configuration.
                checkConf newConf = new checkConf();

                // Reading the Configuration in it's entirety.
                String confData = System.IO.File.ReadAllText(path);

                // Parses the name of the Configuration. RegEx because why not?
                String confName = Regex.Match(confData, @"(\[(\d|\w|\s)*\])").ToString().Replace("[", null).Replace("]", null);
                if(confName != "")
                {
                    newConf.Site = confName;
                }
                else
                { // Catches blank config names.
                    String[] name = path.Split(new String[] { @"\" }, StringSplitOptions.None);
                    frmMain.logEvent(String.Format("{0} - Invalid Configuration - No Name detected.", name[name.Count() -1]));
                    return;
                }


                foreach (String text in confData.Split(new String[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries /* Ignore blank lines. */))
                {
                    if (!text.StartsWith("//")) // Ignore comments.
                    {

                      /* There are better ways to do this than having a thousand if statements.
                       * But, The entire point of posting this for free is so people modify this
                       * and make it more effecient whilst learning. */


                        #region  URL

                        // Catch missing URL's.
                        if (!confData.Contains("url="))
                        {
                            frmMain.logEvent(String.Format("{0} - Invalid Configuration - No URL detected.", confName));
                            break;
                        }

                        if (text.Contains("url="))
                        {
                            newConf.URL = text.ToLower().Replace("url=", "");
                            if (newConf.URL == "")
                            {
                                frmMain.logEvent("Invalid Configuration - No URL detected.");
                                break;
                            }
                        }
                        #endregion
                      
                        #region Differentiation
                        
                        if (text.ToLower().Contains("successon404="))
                        {
                            newConf.successOn404 = Convert.ToBoolean(text.Split(Convert.ToChar("="))[1]);
                        }
                        if (text.ToLower().Contains("successstring="))
                        {
                            newConf.SuccessString = text.ToLower().Replace("successstring=", "");
                        }
                        if (text.ToLower().Contains("failstring="))
                        {
                            newConf.FailString = text.ToLower().Replace("failstring=", "");
                        }
                        if (text.ToLower().Contains("postdata="))
                        {
                            newConf.POSTData = text.ToLower().Replace("postdata=", "");
                        }

                        #endregion
                    }
                }

                // Catches Incomplete Configurations
                if (newConf.SuccessString == null)
                {
                    if (newConf.FailString == null)
                    {
                        if (newConf.successOn404 == false)
                        {
                            frmMain.logEvent("Invalid Configuration - No Failure/Success Indicators detected.");
                        }
                        else
                        {
                            globalConfigs.Add(newConf);
                        }
                    }
                else
                {
                    globalConfigs.Add(newConf);
                }
            }
            else
            {
                globalConfigs.Add(newConf);
            }

            }
            catch (Exception ex)
            {
                eventLogger.logEvent(String.Format("{0} - {1} [{2}] | {3}", ex.Message, "Configuration Loader", ex.StackTrace, ex.TargetSite));
            }
        }
    }
}

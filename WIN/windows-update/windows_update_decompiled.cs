// Decompiled with JetBrains decompiler
// Type: WindowsUpdate.Program
// Assembly: WindowsUpdate, Version=2.1.7629.27749, Culture=neutral, PublicKeyToken=null
// MVID: D6D3112D-9FBC-427B-9BD4-270E84B16DE5
// Assembly location: C:\Program Files (x86)\Monitoring Client\plugins\windowsupdate\WindowsUpdate.exe

using PluginHelper;
using System;
using System.Text;
using WUApiLib;

namespace WindowsUpdate
{
  internal class Program
  {
    private static readonly Plugin WindowsUpdate = new Plugin()
    {
      emptySummary = "Windows is up to date"
    };

    private static void Main()
    {
      try
      {
        Logger.Write("INFO", "Windows Update plugin is running (extended run)");
        bool flag = false;
        int alertAfter = Settings.IntLoad("windows_update", "DaysBeforeAlert", 1);
        try
        {
          Logger.Write("DEBUG", "Gathering Automatic Updates Class information");
          AutomaticUpdatesClass automaticUpdatesClass = new AutomaticUpdatesClass();
          if (automaticUpdatesClass.ServiceEnabled)
          {
            Plugin.Metadata metadata = new Plugin.Metadata()
            {
              name = "automatic_update_status"
            };
            switch (automaticUpdatesClass.Settings.NotificationLevel)
            {
              case AutomaticUpdatesNotificationLevel.aunlNotConfigured:
                Program.WindowsUpdate.ExitCode = 20;
                Program.WindowsUpdate.Summary = "Automatic Updates are not configured";
                metadata.value = (object) "Automatic Updates are not configured";
                break;
              case AutomaticUpdatesNotificationLevel.aunlDisabled:
                flag = true;
                break;
              case AutomaticUpdatesNotificationLevel.aunlNotifyBeforeDownload:
                Program.WindowsUpdate.ExitCode = 20;
                Program.WindowsUpdate.Summary = "Automatic Updates will notify user but are not automatically downloaded";
                metadata.value = (object) "Notify before downloading";
                break;
              case AutomaticUpdatesNotificationLevel.aunlNotifyBeforeInstallation:
                Program.WindowsUpdate.ExitCode = 20;
                Program.WindowsUpdate.Summary = "Automatic Updates will download and notify user but are not automatically installed";
                metadata.value = (object) "Notify before installation";
                break;
              case AutomaticUpdatesNotificationLevel.aunlScheduledInstallation:
                metadata.value = (object) "Automatic Updates are enabled";
                break;
              default:
                Program.WindowsUpdate.ExitCode = 1;
                Program.WindowsUpdate.Summary = "Entered unexpected part of code when checking AutomaticUpdatesNotificationLevel";
                metadata.value = (object) "Unexpected notification level found";
                break;
            }
            Program.WindowsUpdate.metadata.Add(metadata);
            if (!flag)
            {
              Logger.Write("DEBUG", "Using AUC to check last update check date");
              if (automaticUpdatesClass.Results.LastSearchSuccessDate != null)
              {
                DateTime localTime = ((DateTime) automaticUpdatesClass.Results.LastSearchSuccessDate).ToLocalTime();
                if (localTime > DateTime.Now.AddYears(-1))
                {
                  Program.WindowsUpdate.AddMetadata("last_update_check", (object) localTime.ToStandardString(true));
                  if (localTime < DateTime.Now.AddDays((double) -alertAfter))
                  {
                    Program.WindowsUpdate.ExitCode = 20;
                    Program.WindowsUpdate.Summary = "Last check for new Windows Updates is over " + alertAfter.ToString() + " day(s) old";
                  }
                }
              }
            }
          }
          else
            flag = true;
        }
        catch (Exception ex)
        {
          Logger.Write("ERROR", "There was an issue with AUC: " + ex.Message);
          flag = true;
        }
        if (flag)
        {
          Program.WindowsUpdate.ExitCode = 20;
          Program.WindowsUpdate.Summary = "The update client is disabled";
        }
        else
        {
          Plugin.Metadata metadata = new Plugin.Metadata()
          {
            name = "available_updates",
            datatype = "array"
          };
          Logger.Write("DEBUG", "Gathering updates information");
          UpdateResults updateCounts = Program.UpdatesAvaliable();
          if (!Settings.Load("Windows_Update", "ShowAllList", (object) "0").ToBool())
          {
            Logger.Write("INFO", "Plugin set to trim down list to only 5 updates per list");
            updateCounts.TrimLists();
          }
          metadata.AddChildren("high_priority_updates", (object) updateCounts.PriHigh(), "integer");
          metadata.AddChildren("low_priority_updates", (object) updateCounts.PriLow(), "integer");
          metadata.AddChildren("driver_updates", (object) updateCounts.Drivers, "integer");
          metadata.AddChildren("hidden_updates", (object) updateCounts.Hidden, "integer");
          if (Settings.Load("Windows_Update", "IncludeMajorList", (object) "1").ToBool())
            metadata.AddChildren("list_of_critical_updates", (object) Common.ToMetadataList<string>(updateCounts.CriticalList));
          if (Settings.Load("Windows_Update", "IncludeMinorList", (object) "0").ToBool())
            metadata.AddChildren("list_of_minor_updates", (object) Common.ToMetadataList<string>(updateCounts.MinorList));
          if (Settings.Load("Windows_Update", "IncludeDriverList", (object) "0").ToBool())
            metadata.AddChildren("list_of_drivers_updates", (object) Common.ToMetadataList<string>(updateCounts.DriverList));
          Program.WindowsUpdate.AddMetadata("total_updates", (object) updateCounts.Total());
          StringBuilder summaryString;
          if (Program.CreateSummaryAndAlerts(updateCounts, alertAfter, out summaryString))
            Program.WindowsUpdate.Summary = summaryString.ToString();
          Program.CheckAndResetLastStatus(updateCounts);
          Program.WindowsUpdate.metadata.Add(metadata);
        }
      }
      catch (Exception ex)
      {
        Logger.Write("ERROR", Program.WindowsUpdate.DefaultTraceback(ex));
      }
      Program.WindowsUpdate.Exit();
    }

    private static UpdateResults UpdatesAvaliable()
    {
      UpdateResults updateResults = new UpdateResults();
      try
      {
        IUpdateSearcher updateSearcher = ((IUpdateSession3) new UpdateSessionClass()).CreateUpdateSearcher();
        updateSearcher.Online = false;
        foreach (IUpdate update in (IUpdateCollection) updateSearcher.Search("IsInstalled=0 And IsHidden=0").Updates)
        {
          try
          {
            if (update.IsHidden)
            {
              ++updateResults.Hidden;
            }
            else
            {
              switch (update.Categories[0].Name.ToString())
              {
                case "Critical Updates":
                  ++updateResults.Critical;
                  updateResults.CriticalList.Add(update.Title);
                  continue;
                case "Definition Updates":
                case "Service Packs":
                case "Update Rollups":
                  ++updateResults.Moderate;
                  updateResults.MinorList.Add(update.Title);
                  continue;
                case "Drivers":
                  ++updateResults.Drivers;
                  updateResults.DriverList.Add(update.Title);
                  continue;
                case "Feature Packs":
                  ++updateResults.Low;
                  updateResults.MinorList.Add(update.Title);
                  continue;
                case "Security Updates":
                  updateResults.Security = true;
                  ++updateResults.Critical;
                  updateResults.CriticalList.Add(update.Title);
                  continue;
                case "Updates":
                  ++updateResults.Other;
                  updateResults.MinorList.Add(update.Title);
                  continue;
                default:
                  switch (update.MsrcSeverity)
                  {
                    case "Critical":
                      ++updateResults.Critical;
                      updateResults.CriticalList.Add(update.Title);
                      continue;
                    case "Important":
                      ++updateResults.Important;
                      updateResults.CriticalList.Add(update.Title);
                      continue;
                    case "Moderate":
                      ++updateResults.Moderate;
                      updateResults.MinorList.Add(update.Title);
                      continue;
                    case "Low":
                      ++updateResults.Low;
                      updateResults.MinorList.Add(update.Title);
                      continue;
                    default:
                      ++updateResults.Other;
                      updateResults.MinorList.Add(update.Title);
                      continue;
                  }
              }
            }
          }
          catch (Exception ex)
          {
            Logger.Write("ERROR", "Could not iterate update info: " + ex?.ToString());
          }
        }
        Logger.Write("INFO", "Important: " + updateResults.Important.ToString() + " Critical: " + updateResults.Critical.ToString() + " Optional: " + updateResults.Optional.ToString() + " Low: " + updateResults.Low.ToString() + " Moderate: " + updateResults.Moderate.ToString() + " Other: " + updateResults.Other.ToString() + " Hidden: " + updateResults.Hidden.ToString() + " Drivers: " + updateResults.Drivers.ToString());
      }
      catch (Exception ex)
      {
        Logger.Write("ERROR", "Could not check for updates " + ex.Message);
        updateResults = new UpdateResults();
      }
      return updateResults;
    }

    public static bool CreateSummaryAndAlerts(
      UpdateResults updateCounts,
      int alertAfter,
      out StringBuilder summaryString)
    {
      bool flag = false;
      summaryString = new StringBuilder();
      int num1 = Settings.IntLoad("windows_update", "LastStatus", 0);
      int num2 = Settings.IntLoad("windows_update", "LastStatus", 0);
      summaryString.Append("Windows has found");
      DateTime now;
      if (updateCounts.PriHigh() > 0)
      {
        summaryString.Append(" Critical");
        flag = true;
        Program.WindowsUpdate.ExitCode = 20;
        if (Settings.Load("Windows_Update", "ReportHighPri", (object) "1").ToBool())
        {
          Settings.Update("windows_update", "LastStatus", "2");
          if (num2 != 2)
          {
            now = DateTime.Now;
            Settings.Update("windows_update", "LastAlert", now.ToString());
          }
          else if (num1 >= alertAfter && (num2 == 2 || alertAfter == 0))
            Program.WindowsUpdate.ExitCode = 2;
        }
      }
      if (updateCounts.PriLow() > 0)
      {
        if (flag)
          summaryString.Append(updateCounts.Drivers == 0 ? " and" : ",");
        summaryString.Append(" Minor");
        flag = true;
        if (Settings.Load("Windows_Update", "ReportLowPri", (object) "0").ToBool())
        {
          Settings.Update("windows_update", "LastStatus", "2");
          Program.WindowsUpdate.ExitCode = 20;
          if (num2 != 2)
          {
            now = DateTime.Now;
            Settings.Update("windows_update", "LastAlert", now.ToString());
          }
          else if (num1 >= alertAfter && (num2 == 2 || alertAfter == 0))
            Program.WindowsUpdate.ExitCode = 2;
        }
      }
      if (updateCounts.Drivers > 0)
      {
        if (flag)
          summaryString.Append(" and");
        summaryString.Append(" Driver");
        flag = true;
        if (Settings.Load("Windows_Update", "ReportDrivers", (object) "0").ToBool())
        {
          Program.WindowsUpdate.ExitCode = 20;
          Settings.Update("windows_update", "LastStatus", "2");
          if (num2 != 2)
          {
            now = DateTime.Now;
            Settings.Update("windows_update", "LastAlert", now.ToString());
          }
          else if (num1 >= alertAfter && (num2 == 2 || alertAfter == 0))
            Program.WindowsUpdate.ExitCode = 2;
        }
      }
      summaryString.Append(" updates are available.");
      return flag;
    }

    public static void CheckAndResetLastStatus(UpdateResults updateCounts)
    {
      int num = Settings.Load("Windows_Update", "ReportHighPri", (object) "1").ToBool() ? 1 : 0;
      bool flag1 = Settings.Load("Windows_Update", "ReportLowPri", (object) "0").ToBool();
      bool flag2 = Settings.Load("Windows_Update", "ReportDrivers", (object) "0").ToBool();
      if (num != 0 && updateCounts.PriHigh() != 0 || flag1 && updateCounts.PriLow() != 0 || flag2 && updateCounts.Drivers != 0)
        return;
      Settings.Update("windows_update", "LastStatus", "0");
    }
  }
}

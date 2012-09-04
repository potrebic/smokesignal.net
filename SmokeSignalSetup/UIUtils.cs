// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

namespace SmokeSignalSetup
{
    public class UIUtils
    {
        public static void LaunchURL(string url)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(url);
                psi.UseShellExecute = true;
                Process.Start(psi);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("An unexpected error occurred trying to launch your Browser. If the Browser didn't start please try again.");
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show("An unexpected error occurred trying to launch your Browser. If the Browser didn't start please try again.");
            }
        }
    }

    /// <summary>
    /// Utility class that knows how to compare version strings to determine if the latest version is newer than the current version
    /// The class is more complicated than one might expect... that's to allow a little deeper unit testing.
    /// The 2 'VersionGetter' delegate exist so that version string aren't "gotten" until after the time check. That's
    /// in case the getting of the version string is slow/heavy process (e.g. refercing a URL to determine latest version)
    /// </summary>
    class UpdaterChecker
    {
        public delegate string VersionGetter();

        public UpdaterChecker(VersionGetter currentVersionGetter, VersionGetter latestVersionGetter)
        {
            CurrentVersionGetter = currentVersionGetter;
            LatestVersionGetter = latestVersionGetter;
        }

        /// <summary>
        /// Checks to see if an update is available, returning the version string if there is one
        /// </summary>
        /// <param name="now"></param>
        /// <param name="checkTime"></param>
        /// <returns>The version string of the update. Null is there is no update</returns>
        public string Check()
        {
            bool updateAvailable = false;

            // compare version of this executable against the latest version
            string current = CurrentVersionGetter();
            string latest = LatestVersionGetter();
            if (IsNewerVersion(current, latest))
            {
                updateAvailable = true;
            }

            return updateAvailable ? latest : null;
        }

        #region private

        private VersionGetter CurrentVersionGetter;
        private VersionGetter LatestVersionGetter;

        private bool IsNewerVersion(string vCurrent, string vLatest)
        {
            if (string.IsNullOrEmpty(vCurrent) || string.IsNullOrEmpty(vLatest))
            {
                return false;
            }

            IList<int> current = VersionStringToInt(vCurrent);
            IList<int> latest = VersionStringToInt(vLatest);

            for (int i = 0; i < latest.Count; i++)
            {
                if (i >= current.Count)
                {
                    return true;
                }

                if (latest[i] == current[i])
                {
                    continue;
                }

                return latest[i] > current[i];
            }

            return false;
        }

        /// <summary>
        /// Take a string like "1.2.0.5" and contrvert to list of integers. Trailing zeros are removed
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private IList<int> VersionStringToInt(string version)
        {
            List<int> list = new List<int>();
            string[] split = version.Split('.');

            bool processingTrailing = true;
            // go through list in reverse (so that we can drop trailing zeros)
            for (int i = split.Length - 1; i >= 0; i--)
            {
                string s = split[i];
                int val;
                bool validData = false;

                if (Int32.TryParse(s, out val))
                {
                    if (val >= 0)
                    {
                        validData = true;
                        if (processingTrailing)
                        {
                            if (val == 0)
                            {
                                continue;
                            }
                            else
                            {
                                processingTrailing = false;
                            }
                        }
                        list.Insert(0, val);
                    }
                }

                if (!validData)
                {
                    // had a non-integer in the version string. Garbage data. So abort
                    list.Clear();
                    break;
                }
            }
            return list;
        }
        #endregion
    }

    public class ChangeCursor : IDisposable
    {
        Cursor previous;

        public ChangeCursor(Cursor cursor)
        {
            previous = Cursor.Current;
            Cursor.Current = cursor;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Cursor.Current = previous;
        }

        #endregion
    }
}

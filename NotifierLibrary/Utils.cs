// Copyright 2012 Peter Potrebic.  All rights reserved.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using CampfireAPI;

namespace NotifierLibrary
{
    public class Utils
    {
        private static TraceSwitch traceSwitch = new TraceSwitch("TraceLevelSwitch", "Switch in config file");
        private static IList<CampfireState.UserInfo> Special_SmokeSignal_User;

        static Utils()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
 
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Assembly asm = Assembly.GetExecutingAssembly();
            Attribute[] ca = Attribute.GetCustomAttributes(asm, typeof(AssemblyCompanyAttribute));
            path = System.IO.Path.Combine(path, ((AssemblyCompanyAttribute)ca[0]).Company);

            Attribute[] pa = Attribute.GetCustomAttributes(asm, typeof(AssemblyProductAttribute));
            path = System.IO.Path.Combine(path, ((AssemblyProductAttribute)pa[0]).Product);

            DataLocation = path;

            path = System.IO.Path.Combine(path, "trace_log.txt");

            //Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.Listeners.Add(new TextWriterTraceListener(path));
            Trace.AutoFlush = true;

            // This creates the special "Smoke Signal" user which allows commands to communicate with smoke signal. Things like "@SSignal:Help"
            CampfireState.UserInfo smokeSignalUser = new CampfireState.UserInfo(0, "Smoke Signal", "x", "y");
            Special_SmokeSignal_User = new List<CampfireState.UserInfo>();
            Special_SmokeSignal_User.Add(smokeSignalUser);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex _Regex = new System.Text.RegularExpressions.Regex(strRegex);
            if (_Regex.IsMatch(email))
                return (true);
            else
                return (false);
        }

        public static string DataLocation { get; private set; }

        private static EventLogEntryType TraceLevelToLogEntryType(TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Warning:
                    return EventLogEntryType.Warning;
                case TraceLevel.Error:
                    return EventLogEntryType.Error;
                default:
                    return EventLogEntryType.Information;
            }
        }

        public static void TraceException(TraceLevel level, Exception ex, string message)
        {
            if (traceSwitch.Level >= level)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(string.Format("{0}: {1}", DateTime.Now.ToString(), message));
                sb.AppendLine(string.Format("Exception: {0}", ex.ToString()));

                // only write Warning & Error to Window's event log
                if (level <= TraceLevel.Warning)
                {
                    EventLog elog = new EventLog("Application");
                    elog.Source = "SmokeSignal";
                    elog.WriteEntry(sb.ToString(), TraceLevelToLogEntryType(level));
                }

                Trace.Write(sb.ToString());
            } 
        }

        public static void TraceMessage(TraceLevel level, string message)
        {
            if (traceSwitch.Level >= level)
            {
                string completeMessage = string.Format("{0}: {1}", DateTime.Now.ToString(), message);

                // only write Warning & Error to Window's event log
                if (level <= TraceLevel.Warning)
                {
                    EventLog elog = new EventLog("Application");
                    elog.Source = "SmokeSignal";
                    elog.WriteEntry(completeMessage, TraceLevelToLogEntryType(level));
                }
              
                Trace.WriteLine(completeMessage);
            }
        }

        public static void TraceMessage(TraceLevel level, Message msg, string extraText)
        {
            CampfireState campState = CampfireState.Instance;
            IList<CampfireState.UserInfo> allUsers = campState.Users;

            CampfireState.UserInfo user = allUsers.FirstOrDefault(u => u.Id == msg.UserId);
            string userName = (user != null) ? user.NickName : msg.UserId.ToString();

            CampfireState.RoomInfo room = campState.Rooms.FirstOrDefault(r => r.Id == msg.RoomId);
            string roomName = (room != null) ? room.Name : msg.RoomId.ToString();

            TraceMessage(level, string.Format("At: {0}, User: {1}, Room: {2}, {3}{4}{5}",
                msg.PostedAt.ToString(), userName, roomName,
                (extraText ?? ""), (!string.IsNullOrEmpty(extraText)) ? ": " : "", msg.TrimmedBody));
        }
        public static void TraceVerboseMessage(Message msg, string extraText)
        {
            TraceMessage(TraceLevel.Verbose, msg, extraText);
        }
        public static void TraceInfoMessage(Message msg, string extraText)
        {
            TraceMessage(TraceLevel.Info, msg, extraText);
        }
        public static void TraceWarningMessage(Message msg, string extraText)
        {
            TraceMessage(TraceLevel.Warning, msg, extraText);
        }
        public static void TraceErrorMessage(Message msg, string extraText)
        {
            TraceMessage(TraceLevel.Error, msg, extraText);
        }

        public static void TraceVerboseMessage(string msg)
        {
            TraceMessage(TraceLevel.Verbose, msg);
        }
        public static void TraceInfoMessage(string msg)
        {
            TraceMessage(TraceLevel.Info, msg);
        }
        public static void TraceWarningMessage(string msg)
        {
            TraceMessage(TraceLevel.Warning, msg);
        }
        public static void TraceErrorMessage(string msg)
        {
            TraceMessage(TraceLevel.Error, msg);
        }

        private static int SentenceEndIndex(string text, int startPos)
        {
            int index1 = text.IndexOf(". ", startPos);
            int index2 = text.IndexOf("? ", startPos);
            int index3 = text.IndexOf("! ", startPos);

            if (index1 < 0) index1 = text.Length - 1;
            if (index2 < 0) index2 = text.Length - 1;
            if (index3 < 0) index3 = text.Length - 1;

            int eos = Math.Min(index1, Math.Min(index2, index3));
            return eos;
        }

        private static string FindSentenceContaining(string text, string target)
        {
            if (string.IsNullOrEmpty(target) || (target.Trim() == "." || target.Trim() == "!" || target.Trim() == "?"))
            {
                throw new ArgumentException("target must be non null/empty and can't be '.'");
            }

            string str = text;
            int pos = text.IndexOf(target);
            if (pos < 0)
            {
                throw new ArgumentException("target was not found in the given text.");
            }

            // find the start of the sentence containing 'target'
            int i = 0;
            while (true)
            {
                int next = SentenceEndIndex(text, i);
                if (next >= 0 && next < pos)
                {
                    i = next+2;
                }
                else
                {
                    str = text.Substring(i);
                    break;
                }
            }

            // 'str' is now the string starting with the sentance containing 'target'

            // now find the end of the sentence containing 'target'
            i = SentenceEndIndex(str, 0);
            if (i >= 0)
            {
                str = str.Substring(0, i + 1);
            }

            return str.Trim();
        }

        private static CampfireState.UserReference InitReferenceInfo(string text, string target, CampfireState.UserInfo user, bool smokeSignalCommands)
        {
            string refText;

            if (smokeSignalCommands)
            {
                refText = target;
            }
            else if (text.Length < 80)
            {
                refText = text;
            }
            else
            {
                try
                {
                    refText = FindSentenceContaining(text, target);
                }
                catch (ArgumentException ex)
                {
                    refText = string.Empty;
                }
            }

            return new CampfireState.UserReference(refText, user);
        }

        
        /// <summary>
        /// In the give 'text' look for user references (e.g. @PeterP or !Nariman)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="allUsers"></param>
        /// <param name="lazyReferences">list of people references via the "lazy-notification" @Name syntax</param>
        /// <param name="immediateReferences">list of people references via the "immediate-notification" !Name syntax</param>
        public static void FindUserReferences(string text, IList<CampfireState.UserInfo> allUsers, 
            out IList<CampfireState.UserReference> lazyReferences,
            out IList<CampfireState.UserReference> immediateReferences
            )
        {
            IList<CampfireState.UserReference> ignored;
            FindUserReferences(text, allUsers, out lazyReferences, out immediateReferences, out ignored);
        }

        /// <summary>
        /// In the give 'text' look for user references (e.g. @PeterP or !Nariman)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="allUsers"></param>
        /// <param name="lazyReferences">list of people references via the "lazy-notification" @Name syntax</param>
        /// <param name="immediateReferences">list of people references via the "immediate-notification" !Name syntax</param>
        /// <param name="smokeSignalReferences">list of special smoke signal commands like 'help' and 'settings'</param>
        public static void FindUserReferences(string text, IList<CampfireState.UserInfo> allUsers, 
            out IList<CampfireState.UserReference> lazyReferences,
            out IList<CampfireState.UserReference> immediateReferences,
            out IList<CampfireState.UserReference> smokeSignalReferences
            )
        {
            /*
             * The logic here is as follows. A name is composed of X words. To match it must be first letter or
             * whole word. And they must be in order. Minimum chars for a match is 3. And match must be unique
             * (so if 2 people have same initials... then initials will NOT match)
             * 
             * So the name "Peter John Potrebic" would be matched by:
             *      @PJP     @Peter   @PJohn   @PJPotrebic  @Potrebic   @PPotrebic   @JPotrebic etc
             */
            lazyReferences = new List<CampfireState.UserReference>();
            immediateReferences = new List<CampfireState.UserReference>();
            smokeSignalReferences = new List<CampfireState.UserReference>();

            foreach (CampfireState.UserInfo user in allUsers)
            {
                if (text.StartsWith(string.Format("{0}:", user.Name)))
                {
                    // This is the special campfire syntax, users whole name at start of message
                    lazyReferences.Add(InitReferenceInfo(text, user.Name, user, false));
                    break;
                }
            }

            // we're done if there are no '@' or '!' characters
            if (!text.Contains("@") && !text.Contains("!"))
            {
                return;
            }

            FindUserReferencesViaPrefix(text, immediateReferences, allUsers, "!", false);
            FindUserReferencesViaPrefix(text, lazyReferences, allUsers, "@", false);

            FindUserReferencesViaPrefix(text, smokeSignalReferences, Special_SmokeSignal_User, "@", true);
        }

        private static void FindUserReferencesViaPrefix(string text, IList<CampfireState.UserReference> references, IList<CampfireState.UserInfo> allUsers,
            string prefix, bool smokeSignalCommands)
        {
            IList<string> atWords = WordsStartingWithPrefix(text, prefix, smokeSignalCommands);

            foreach (string reference in atWords)
            {
                CampfireState.UserInfo referencedUser = SingleMatch(reference, allUsers, smokeSignalCommands);

                // The extra/second check is to prevent adding the same User multiple times to the list
                if (referencedUser != null && (smokeSignalCommands || !references.Any(ri => ri.TargetUser.Id == referencedUser.Id)))
                {
                    references.Add(InitReferenceInfo(text, reference, referencedUser, smokeSignalCommands));
                }
            }
        }

        private static CampfireState.UserInfo SingleMatch(string target, IList<CampfireState.UserInfo> allUsers, bool smokeSignalCommands)
        {
            // trim off trailing periods so taht references can be at the end of a sentance: "... @Peter."
            string lowered = target.Trim('.').ToLowerInvariant();
            try
            {
                // For reference... if there's only one user who matches... we have a HIT!
                CampfireState.UserInfo referencedUser = allUsers.SingleOrDefault(
                    ui => smokeSignalCommands ? ui.PossibleAbbreviations.Any(abbrv => lowered.StartsWith(abbrv)) : ui.PossibleAbbreviations.Contains(lowered));
                if (referencedUser != null)
                {
                    return referencedUser;
                }
            }
            catch (InvalidOperationException ex)
            {
                // this means we have multiple people that can match the reference found in the text. Oh well... just skip it
            }
            return null;
        }

        public static List<string> BuildAbbreviations(string[] words)
        {
            // each word in name as 3 possiblities: <nothing>, first-letter, entire-word

            string[,] choicesTables = new string[words.Length, 3];

            for (int widx = 0; widx < words.Length; widx++)
            {
                string w = words[widx];
                choicesTables[widx, 0] = "";                            // null
                choicesTables[widx, 1] = w.Substring(0, 1);             // first letter
                choicesTables[widx, 2] = (w.Length == 1) ? null : w;    // whole word (or null of the word is just 1 letter long
            }
            //int wc = choicesTables.GetLength(0);

            List<string> abbrevs = SubAbbrev(choicesTables, 0);

            List<string> filtered = new List<string>();
            // min ength is 3 characters
            foreach (string ab in abbrevs)
            {
                if (ab.Length >= 3)
                    filtered.Add(ab.ToLowerInvariant());
            }

            return filtered;
        }

        private static List<string> SubAbbrev(string[,] choicesTable, int startIdx)
        {
            if (startIdx == choicesTable.GetLength(0) - 1)
            {
                List<string> abbrevs = new List<string>();
                abbrevs.Add(choicesTable[startIdx, 0]);
                abbrevs.Add(choicesTable[startIdx, 1]);
                if (choicesTable[startIdx, 2] != null)
                {
                    abbrevs.Add(choicesTable[startIdx, 2]);
                }
                return abbrevs;
            }
            else
            {
                // recurse
                List<string> subAbbrevs = SubAbbrev(choicesTable, startIdx + 1);

                // now take the subAbrevs and "cross" against the 3 possiblilties at level 'startIdx'
                List<string> composited = new List<string>();
                foreach (string sub in subAbbrevs)
                {
                    composited.Add(string.Format("{0}{1}", choicesTable[startIdx, 0], sub));
                    composited.Add(string.Format("{0}{1}", choicesTable[startIdx, 1], sub));
                    if (choicesTable[startIdx, 2] != null)
                    {
                        composited.Add(string.Format("{0}{1}", choicesTable[startIdx, 2], sub));
                    }
                }
                return composited;
            }
        }

        /// <summary>
        /// Something is wrong with this file. Make a backup and safely delete original
        /// </summary>
        /// <param name="fromPath"></param>
        /// <returns></returns>
        public static bool SafeBackupAndDelete(string fromPath)
        {
            string leaf = System.IO.Path.GetFileNameWithoutExtension(fromPath);
            string directory = System.IO.Path.GetDirectoryName(fromPath);
            string ext = System.IO.Path.GetExtension(fromPath);

            DateTime dt = DateTime.Now;

            // create a backup of the corrupte file
            string newPath = string.Format("{0}\\{1}.{2}{3}", directory, leaf, dt.Ticks.ToString(), ext);
            bool error = false;
            try
            {
                System.IO.File.Move(fromPath, newPath);
            }
            catch (Exception ex)
            {
                error = true;
            }

            if (error)
            {
                try
                {
                    System.IO.File.Delete(fromPath);
                }
                catch (Exception ex)
                {
                    Utils.TraceException(TraceLevel.Error, ex, "Unable to make a backup and/or delete: " + fromPath);
                }
            }
            return error;
        }

        #region private helpers

        private static IList<string> WordsStartingWithPrefix(string text, string prefix, bool smokeSignalCommands)
        {
            List<string> hits = new List<string>();
            char[] delimiterChars =  { ' ', ',', '\t', '\n', '\r', ':', '.' };

            if (smokeSignalCommands) {
                delimiterChars = new char[] { ' ', ',', '\t', '\n', '\r' };
            }

            string[] words = text.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            foreach (string w in words)
            {
                if (w.StartsWith(prefix) && w.Length > 1)
                {
                    hits.Add(w.Substring(1));
                }
            }
            
            return hits;
        }

        #endregion
    }
}

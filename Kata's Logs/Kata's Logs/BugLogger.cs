using System;
using System.Collections.Generic;
using System.Threading;

namespace Kata_s_Logs
{
    class BugLogger
    {

        private List<string> usernames;
        private List<string> bugDescriptions;
        private BugTimer bugTimer = new BugTimer();
        private BugReader reader = new BugReader();
        private Bug newBug = new Bug();

        private Random rnd = new Random();
        private int randnum;

        /// <summary>
        /// Function to be called at start to create the initial logger that will populate the bug list.
        /// </summary>
        public void GenerateLogger()
        {
            usernames = reader.GetUsernames();
            bugDescriptions = reader.GetBugDescriptions();
        }

        /// <summary>
        /// Update funtion that will be called and update the console with new information
        /// </summary>
        public void Update()
        {
            
            if (bugTimer.TimeRemaining() <= 0 || !bugTimer.isEnabled())
            {
                //random timer between 45 seconds to 3 minutes
                randnum = rnd.Next(45000, 180000);
                bugTimer.StartTimer(randnum);
                GenerateBug();
                UpdateLog();
            }
        }

        /// <summary>
        /// creates a new bug based on the entries in the list.
        /// </summary>
        private void GenerateBug()
        {
            randnum = rnd.Next(0, usernames.Count);
            newBug.SetUsername(usernames[randnum]);
            randnum = rnd.Next(0, bugDescriptions.Count);
            newBug.SetBugDescription(bugDescriptions[randnum]);
            randnum = rnd.Next(0, 6);
            newBug.SetBugCategory((Bug.BugTypes)randnum);
        }

        private void UpdateLog()
        {
            TypeLine("    NEW BUG INCOMING TO KATA'S LOGS...\n");
            TypeLine("Bug Category:          " + newBug.GetBugCategory() + "\n");
            TypeLine("Bug Description:       " + newBug.GetBugDescription() + "\n");
            TypeLine("User that reported:    " + newBug.GetUsername() + "\n");
            TypeLine("Time Bug was recieved: " + newBug.GetTimeStamp() + "\n");
            TypeLine("Checking for more bug reports...\n");
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Wrties out text to console slowly.
        /// </summary>
        /// <param name="line"></param>
        private void TypeLine(string line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                Console.Write(line[i]);
                Thread.Sleep(100);
            }
        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace Kata_s_Logs
{
    /// <summary>
    /// Reads whats in files and then returns a string for the logger to use.
    /// </summary>
    class BugReader
    {
        StreamReader reader;
        private List<string> usernames = new List<string>();
        private List<string> bugDescriptions = new List<string>();

        public List<string> GetUsernames()
        {
            reader = new StreamReader("TextFiles/Usernames.txt");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                usernames.Add(line);
            }

            reader.Close();

            return usernames;
        }

        public List<string> GetBugDescriptions()
        {
            reader = new StreamReader("TextFiles/BugDescriptions.txt");

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                bugDescriptions.Add(line);
            }

            reader.Close();
            return bugDescriptions;
        }
    }
}

using System;

namespace Kata_s_Logs
{
    public class Bug
    {
        public enum BugTypes
        {
            Gameplay_Minor,
            Gameplay_Severe,
            UI_Minor,
            UI_Severe,
            Network_Issues,
            Gamplay_Suggestion,
            UI_Suggestion,
        };

        private string Username;
        private BugTypes BugCategory;
        private string bugCategory;
        private string BugDescription;
        private string TimeStamp;

        public string GetUsername()
        {
            return Username;
        }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public string GetBugCategory()
        {
            switch (BugCategory)
            {
                case BugTypes.Gameplay_Minor:
                    bugCategory = "Gameplay Minor";
                    break;
                case BugTypes.Gameplay_Severe:
                    bugCategory = "Gameplay Severe";
                    break;
                case BugTypes.UI_Minor:
                    bugCategory = "UI Minor";
                    break;
                case BugTypes.UI_Severe:
                    bugCategory = "UI Severe";
                    break;
                case BugTypes.Network_Issues:
                    bugCategory = "Network Issues";
                    break;
                case BugTypes.Gamplay_Suggestion:
                    bugCategory = "Gameplay Suggestion";
                    break;
                case BugTypes.UI_Suggestion:
                    bugCategory = "UI Suggestion";
                    break;
                default:
                    break;
            }

            return bugCategory;
        }

        public void SetBugCategory(BugTypes bugCategory)
        {
            BugCategory = bugCategory;
        }

        public string GetBugDescription()
        {
            return BugDescription;
        }

        public void SetBugDescription(string bugDescription)
        {
            BugDescription = bugDescription;
        }

        public string GetTimeStamp()
        {
            TimeStamp = DateTime.Now.ToString();
            return TimeStamp;
        }
    }
}

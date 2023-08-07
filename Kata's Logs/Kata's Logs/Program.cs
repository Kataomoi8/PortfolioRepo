

namespace Kata_s_Logs
{
    class Program
    {
        static void Main(string[] args)
        {
            BugLogger logger = new BugLogger();
            logger.GenerateLogger();
            while (true)
            {
                logger.Update();
            }
        }
    }
}

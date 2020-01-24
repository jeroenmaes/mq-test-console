using System;

namespace MqTestConsole
{
    public static class Logger
    {
        public static void LogMessage(string message)
        {
            Console.WriteLine($"{DateTime.UtcNow}::" + message);
        }
    }
}

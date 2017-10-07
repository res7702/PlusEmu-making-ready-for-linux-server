using System;
using System.Text;
using ConsoleWriter;

namespace Quasar.Core
{
    public static class Logging
     {
        public static bool DisabledState
        {
            get { return Writer.DisabledState; }
            set { Writer.DisabledState = value; }
        }

        public static void WriteLine(string Line, ConsoleColor Colour = ConsoleColor.Gray)
        {
            Writer.WriteLine(Line, Colour);
        }

        public static void LogException(string logText)
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Writer.LogException(CurrentTime + Environment.NewLine + logText + Environment.NewLine);
        }

        public static void LogMySQLError(string logText)
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Writer.LogMySQLError(CurrentTime + Environment.NewLine + logText + Environment.NewLine);
        }

        public static void LogWiredException(string logText)
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Writer.LogWiredException(CurrentTime + logText);
        }

        public static void LogCacheException(string logText)
        {
            Writer.LogCacheException(logText);
        }

        public static void LogPathfinderException(string logText)
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Writer.LogPathfinderException(CurrentTime + logText);
        }

        public static void LogCriticalException(string logText)
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Writer.LogCriticalException(CurrentTime + logText);
        }

        public static void LogThreadException(string Exception, string Threadname)
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Writer.LogThreadException(CurrentTime + Exception, Threadname);
        }

        public static void LogPacketException(string Packet, string Exception)
        {
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss" + " | ");
            Writer.LogPacketException(CurrentTime + Packet, Exception);
        }

        public static void HandleException(Exception pException, string pLocation)
        {
            Writer.HandleException(pException, pLocation);
        }

        public static void DisablePrimaryWriting(bool ClearConsole)
        {
            Writer.DisablePrimaryWriting(ClearConsole);
        }
    }
}
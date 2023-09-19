
namespace HabrPost.LogException
{
    public class ExceptionLogger
    {
        public static void NewException(Exception ex)
        {
            string exception = $"\n\n{DateTime.Now, -15}: {ex.TargetSite, -5}, {ex.Message}\n{ex.StackTrace}\nЗначения: {ex.HResult}";
            File.AppendAllTextAsync("Logs/logs.txt", exception);
        }
    }
}
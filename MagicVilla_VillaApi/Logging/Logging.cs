namespace MagicVilla_VillaApi.Logging;

public class Logging : ILogging
{
    public void LogInformation(string message)
    {
        if (message != "")
        {
            Console.WriteLine("[INFO]: "+ message);
        }
    }

    public void LogError(string message)
    {
        if (message != "")
        {
            Console.WriteLine("[ERROR]: "+ message);
        }
    }
}
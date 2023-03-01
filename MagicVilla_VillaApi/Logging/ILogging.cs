namespace MagicVilla_VillaApi.Logging;

public interface ILogging
{
    public void LogInformation(string message);

    public void LogError(string message);
}
namespace Files;

/// <summary>
/// A singleton holding global app options.
/// </summary>
public class Options
{
    private static Options? _instance;
    
    private Options()
    {
    }

    /// <summary>
    /// Whether or not to show hidden files or not.
    /// </summary>
    public bool ShowHidden { get; set; } = false;
    
    public static Options Instance
    {
        get { return _instance ??= new Options(); }
    }
}
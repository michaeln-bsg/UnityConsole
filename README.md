# UConsole
Easy-to-use developer console built with UGUI, with easy support for other UI solutions.

## Quick-Start Guide
I strongly encourage devs that use UConsole to take a look at the built-in scripts to get a sense of how custom integration functions. However, here's a few example snippets to get started super fast.

Use RuntimeInitializeOnLoadMethod to create a console instance:
```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
private static void AppStart()
{
    _console = ConsoleUtility.Create(ConsolePrefab);
    var commandRegistries = new[]
    {
        typeof(DefaultConsoleCommands),
    };
    _console.ResetConsole(new ConsoleSetupOptions(commandRegistries));
}
```
Make your own CommandRegistry! Optional parameters are supported:
```csharp
/// <summary>
/// Provides game agnostic commands
/// </summary>
public class GameConsoleCommands : AbstractConsoleCommandRegistry
{
    /// <inheritdoc />
    public GameConsoleCommands(Console instance)
        : base(instance) { }

    [CommandDescription("Teleports the player to xyz")]
    [CommandAliases("player_pos", "spp")]
    private void set_player_pos(float x, float y, float z)
    {
        // Get player instance and set transform.position
    }

    // Commands must have at least one CommandAttribute!
    [CommandAliases]
    private void respawn_player(bool kill = false)
    {
        if(kill)
        {
            // Damage player
        }
        else
        {
            // Just teleport player
        }
    }
}
```
Even make custom modules to add additional functionality.
```csharp
using System;

public class ExampleConsoleModule : AbstractConsoleModule
{
    public AutoCompleteConsoleModule(Console console)
        : base(console) { }

    /// <inheritdoc />
    public override void Initialize()
    {
        Console.InputField.AddOnValueChangedListener(OnInputValueChanged);
        Console.ConsoleToggled += OnConsoleToggled;
    }

    /// <inheritdoc />
    public override void Destroy()
    {
        Console.InputField.RemoveOnValueChangedListener(OnInputValueChanged);
        Console.ConsoleToggled -= OnConsoleToggled;
    }

    /// <inheritdoc />
    public override void Update() 
    { 
        // Do something each frame while the console is open
    }

    private void OnConsoleToggled(bool isOpen)
    {
        // Do something when console opens...
    }

    private void OnInputValueChanged(string value)
    {
        // Do something when input field value changes...
    }
}
```
```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
private static void AppStart()
{
    _console = ConsoleUtility.Create(ConsolePrefab);
    var commandRegistries = new[]
    {
        typeof(DefaultConsoleCommands),
    };
    var customModules = new[]
    {
        typeof(ExampleConsoleModule),
    };
    _console.ResetConsole(new ConsoleSetupOptions(commandRegistries, customModules));
}
```

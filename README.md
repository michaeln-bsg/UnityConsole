# PhantomConsole
Easy-to-use developer console built with UGUI, with easy support for other UI solutions such as TextMeshPro.

![alt text](https://github.com/thebeardphantom/PhantomConsole/blob/master/img/console_test.gif "Example")

## Quick-Start Guide
I strongly encourage devs that use PhantomConsole to take a look at the built-in scripts to get a sense of how custom integration functions. However, here's a few example snippets to get started super fast.

Use RuntimeInitializeOnLoadMethod to create a console instance:
```csharp
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
private static void AppStart()
{
    _console = ConsoleUtility.Create(ConsolePrefab);
    _console.ResetConsole(new ConsoleSetupOptions(true));
}
```
Make your own Commands! Optional parameters and params keyword are supported:
```csharp
/// <summary>
/// Provides game agnostic commands
/// </summary>
public class GameConsoleCommands
{
    [ConsoleCommand("set_player_pos", "player_pos", "spp")]
    [CommandDescription("Teleports the player to xyz")]
    private void SetPlayerPosition(float x, float y, float z)
    {
        // Get player instance and set transform.position
    }
	
    [ConsoleCommand("respawn_player")]
    private void RespawnPlayer(bool kill = false)
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
	
    [ConsoleCommand("echo")]
    private string Echo(params string[] echo)
    {
        return string.Join(",", echo);
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
    var customModules = new[]
    {
        typeof(ExampleConsoleModule),
    };
    _console.ResetConsole(new ConsoleSetupOptions(true, customModules));
}
```

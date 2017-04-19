using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public static class UConsoleEditorUtilities
{
    static UConsoleEditorUtilities()
    {
        EditorApplication.update -= CheckAddRemoveDefine;
        EditorApplication.update += CheckAddRemoveDefine;
    }
    private static void CheckAddRemoveDefine()
    {
        var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';'));
        var apiLevel = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
        var didChange = false;
        if (apiLevel == ApiCompatibilityLevel.NET_2_0_Subset)
        {
            didChange = defines.Remove("DOTNET_FULL");
        }
        else if (!defines.Contains("DOTNET_FULL"))
        {
            defines.Add("DOTNET_FULL");
            didChange = true;
        }
        if (didChange)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", defines.ToArray()));
        }
    }
}

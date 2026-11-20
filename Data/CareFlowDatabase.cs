
using System;
using System.IO;

namespace CareFlow.Data;

public static class CareFlowDatabase
{
    public static string GetDatabasePath()
    {
        string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CareFlow");

        Directory.CreateDirectory(appDataPath);

        return Path.Combine(appDataPath, "careflow.db");
    }
}

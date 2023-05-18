// See https://aka.ms/new-console-template for more information

using System.Reflection;

static class Program
{
    public static string TaiwuPath = Path.Join(Environment.CurrentDirectory, "..");
    static Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

    public static void LoadAssemblyReferences(Assembly selectedAssembly)
    {
        foreach (AssemblyName reference in selectedAssembly.GetReferencedAssemblies())
        {
            if (File.Exists(
                    Path.GetDirectoryName(selectedAssembly.Location) +
                    @"\" + reference.Name + ".dll"))
            {
                Assembly.LoadFrom(
                    Path.GetDirectoryName(selectedAssembly.Location) +
                    @"\" + reference.Name + ".dll");
            }
            else
            {
                var taiwuPath = TaiwuPath;

                if (taiwuPath != null)
                {
                    var taiwuDllPath = Path.Join(taiwuPath, "The Scroll of Taiwu_Data", "Managed",
                        reference.Name + ".dll");
                    var taiwuDllPath2 = Path.Join(taiwuPath, "The Scroll of Taiwu_Data", "Managed",
                        reference.Name + ".dll");
                    if (File.Exists(taiwuDllPath))
                    {
                        Assembly.LoadFrom(taiwuDllPath);
                    }
                    else if (File.Exists(taiwuDllPath2))
                    {
                        Assembly.LoadFrom(taiwuDllPath2);
                    }
                }
            }
        }
    }

    // static void Main(string[] args)
    // {
    //     if (args.Length > 1)
    //     {
    //         TaiwuPath = args[1];
    //     }
    //     EventDumper.Dump();
    // }
}


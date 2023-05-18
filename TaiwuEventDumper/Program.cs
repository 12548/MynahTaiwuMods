// See https://aka.ms/new-console-template for more information

using System.Reflection;

static class Program
{
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
                var taiwuPath = Environment.GetEnvironmentVariable("TAIWU_PATH");

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

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }
}


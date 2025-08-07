using System.Reflection;
using System.Runtime.Loader;

namespace Web.Common.Helpers
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            return base.LoadUnmanagedDllFromPath(unmanagedDllName);
        }

        protected override System.Reflection.Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }

}

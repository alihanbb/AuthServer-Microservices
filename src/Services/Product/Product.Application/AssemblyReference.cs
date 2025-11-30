using System.Reflection;

namespace Product.Application
{
    public static class AssemblyReference
    {
        public static Assembly Assembly = typeof(AssemblyReference).Assembly;
    }
}

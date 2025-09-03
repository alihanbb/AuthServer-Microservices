using System.Reflection;

namespace Order.Infrastructures;

public static class AssemblyReference
{
    public static Assembly Assembly = typeof(AssemblyReference).Assembly;
}

using Bam.Console;
using Bam.Shell;

namespace Bam.Application
{
    [Serializable]
    class Program 
    {
        static void Main(string[] args)
        {
            DontMessageSuccess();
            BamConsoleContext.StaticMain(args);
        }

        private static void DontMessageSuccess()
        {
            IMenuItemRunResultRenderer runResultRenderer =
                BamConsoleContext.Current.ServiceRegistry.Get<IMenuItemRunResultRenderer>();
            ((ConsoleMenuItemRunResultRenderer)runResultRenderer).ItemRunSucceeded = (mi) => { };
        }
    }
}
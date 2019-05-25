using Minsk.CodeAnalysis.Syntax.Binding;

namespace Minsk
{

    class Program
    {
        static void Main(string[] args)
        {
            var repl = new MinskRepl();
            repl.Run();
        }
    }
}

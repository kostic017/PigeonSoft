using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.IO;

namespace Kostic017.Pigeon
{
    public static class AntlrExtensions
    {
        public static IParseTree GetRightSibling(this ParserRuleContext context)
        {
            var index = GetNodeIndex(context);
            return index >= 0 && index < context.Parent.ChildCount - 1 
                ? context.Parent.GetChild(index + 1) 
                : null;
        }

        public static int GetNodeIndex(this ParserRuleContext context)
        {
            var parent = context?.Parent;

            if (parent == null)
                return -1;

            for (int i = 0; i < parent.ChildCount; i++)
            {
                if (parent.GetChild(i) == context)
                    return i;
            }

            return -1;
        }

        public static void PrintTree(this IParseTree tree, TextWriter writer, Parser parser, string indent = "", bool isLastChild = true)
        {
            writer.Write(indent + (isLastChild ? "└──" : "├──") + " ");

            SetOutputColor(writer, tree);
            writer.WriteLine(Trees.GetNodeText(tree, parser));
            ResetOutputColor(writer);
            
            for (int i = 0; i < tree.ChildCount; i++)
                PrintTree(tree.GetChild(i), writer, parser, indent + (isLastChild ? "    " : "│   "), i == tree.ChildCount - 1);
        }

        private static void SetOutputColor(TextWriter writer, IParseTree tree)
        {
            if (writer == Console.Out)
            {
                if (tree.Payload is IToken token)
                {
                    if (SyntaxFacts.Keywords.Contains(token.Text))
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    else if (SyntaxFacts.Types.Contains(token.Text))
                        Console.ForegroundColor = ConsoleColor.Blue;
                    else if (token.Type == PigeonParser.ID)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else if (token.Type == PigeonParser.NUMBER)
                        Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
            }
        }

        private static void ResetOutputColor(TextWriter writer)
        {
            if (writer == Console.Out)
                Console.ResetColor();
        }
    }
}

using System.IO;

namespace TabbedEditor.IO
{
    public static class FileChecker
    {
        public const string FileDialogRawFilter = "Text file (*.txt)|*.txt|Config File (*.conf)|*.conf|JSON File (*.json)|*.json|All files (*)|*";

        public static bool IsTooBig(string path)
        {
            return new FileInfo(path).Length > 10000000;
        }
        
        public static bool IsBinary(string path)
        {
            long length = new FileInfo(path).Length;
            if (length == 0) return false;

            using (StreamReader stream = new StreamReader(path))
            {
                int ch;
                while ((ch = stream.Read()) != -1)
                {
                    if (IsControlChar(ch))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsControlChar(int ch)
        {
            return (ch > Chars.NUL && ch < Chars.BS)
                   || (ch > Chars.CR && ch < Chars.SUB);
        }

        private static class Chars
        {
            public static char NUL = (char)0; // Null char
            public static char BS = (char)8; // Back Space
            public static char CR = (char)13; // Carriage Return
            public static char SUB = (char)26; // Substitute
        }
    }
}
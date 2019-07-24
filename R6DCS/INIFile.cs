using System.Text;
using System.Runtime.InteropServices;

namespace R6DCS
{
    class INIFile
    {
        private string _filePath;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public INIFile(string filePath)
        {
            _filePath = filePath;
        }

        public string Read(string section, string key)
        {
            int retValSize = 255;
            var retVal = new StringBuilder(retValSize);

            GetPrivateProfileString(section, key, "", retVal, retValSize, _filePath);

            return retVal.ToString();
        }

        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _filePath);
        }

        public void Delete(string section)
        {
            Write(section, null, null);
        }

        public void Delete(string section, string key)
        {
            Write(section, key, null);
        }
    }
}

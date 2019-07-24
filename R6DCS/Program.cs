using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace R6DCS
{
    class Program
    {
        private static string _iniPathFormat;
        private static string _guidDirPaths;
        private static string[] _dataCenters => new string[] { "default", "eus", "cus", "scus", "wus", "sbr", "neu", "weu", "eas", "seas", "eau", "wja" };

        static void Main(string[] args)
        {
            Initialize();

            // 만약 토큰이 존재하지 않는다면, Ubisoft API 서버에 로그인한다.


            // 계정 선택
            string guid = SelectAccount();
            if (guid == null)
            {
                Console.WriteLine("선택한 계정이 없거나, 계정에 대한 정보가 담긴 폴더가 존재하지 않습니다.");
                return;
            }

            // 데이터 센터 선택
            string dataCenter = SelectDataCenter();
            if (dataCenter == null)
            {
                Console.WriteLine("선택한 데이터 센터가 없습니다.");
                return;
            }

            // 변경할 데이터 센터로 적용
            var originalDataCenter = GetDataCenter(guid);

            SetDataCenter(guid, dataCenter);

            Console.Clear();
            Console.WriteLine("계정 {0}의 데이터 센터를 {1}에서 {2}로 변경하였습니다!", guid, originalDataCenter, dataCenter);
            Console.WriteLine("계속하려면 아무 키나 누르십시오...");
            Console.ReadKey();
        }

        static void Initialize()
        {
            _guidDirPaths = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"My Games\Rainbow Six - Siege");
            _iniPathFormat = Path.Combine(_guidDirPaths, "{0}", "GameSettings.ini");
        }

        static string SelectAccount()
        {
            List<string> guidList = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(_guidDirPaths);

            foreach (var dir in directoryInfo.GetDirectories())
            {
                string guid = dir.Name;
                if (IsGuid(guid))
                    guidList.Add(guid);
            }

            if (guidList.Count > 0)
            {
                Console.Clear();
                Console.WriteLine("계정을 선택해주세요.");

                var cursorTop = Console.CursorTop;

                for (int index = 0; index < guidList.Count; index++)
                    Console.WriteLine("{0:00}. {1}", index + 1, guidList[index]);

                Console.CursorVisible = false;
                var selectedIndex = 0;

                while (true)
                {
                    Console.SetCursorPosition(0, selectedIndex + cursorTop);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine("{0:00}. {1}", selectedIndex + 1, guidList[selectedIndex]);
                    Console.ResetColor();
                    Console.SetCursorPosition(0, selectedIndex + cursorTop);

                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.Escape)
                        break;

                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        return guidList[selectedIndex];
                    }

                    if (keyInfo.Key == ConsoleKey.UpArrow)
                    {
                        if (0 < selectedIndex)
                        {
                            Console.SetCursorPosition(0, selectedIndex + cursorTop);
                            Console.ResetColor();
                            Console.WriteLine("{0:00}. {1}", selectedIndex + 1, guidList[selectedIndex]);

                            selectedIndex--;
                            Console.SetCursorPosition(0, selectedIndex + cursorTop);
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.WriteLine("{0:00}. {1}", selectedIndex + 1, guidList[selectedIndex]);
                            Console.ResetColor();
                            Console.SetCursorPosition(0, selectedIndex + cursorTop);
                        }
                    }
                    else if (keyInfo.Key == ConsoleKey.DownArrow)
                    {
                        if (selectedIndex < guidList.Count - 1)
                        {
                            Console.SetCursorPosition(0, selectedIndex + cursorTop);
                            Console.ResetColor();
                            Console.WriteLine("{0:00}. {1}", selectedIndex + 1, guidList[selectedIndex]);

                            selectedIndex++;
                            Console.SetCursorPosition(0, selectedIndex + cursorTop);
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.WriteLine("{0:00}. {1}", selectedIndex + 1, guidList[selectedIndex]);
                            Console.ResetColor();
                            Console.SetCursorPosition(0, selectedIndex + cursorTop);
                        }
                    }
                }
            }

            Console.Clear();
            return null;
        }

        static string SelectDataCenter()
        {
            Console.Clear();
            Console.WriteLine("데이터 센터를 선택해주세요.");

            var cursorTop = Console.CursorTop;

            for (int index = 0; index < _dataCenters.Length; index++)
                Console.WriteLine("{0:00}. {1}", index + 1, _dataCenters[index]);

            Console.CursorVisible = false;
            var selectedIndex = 0;

            while (true)
            {
                Console.SetCursorPosition(0, selectedIndex + cursorTop);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                Console.WriteLine("{0:00}. {1}", selectedIndex + 1, _dataCenters[selectedIndex]);
                Console.ResetColor();
                Console.SetCursorPosition(0, selectedIndex + cursorTop);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape)
                    break;

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    return _dataCenters[selectedIndex];
                }

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    if (0 < selectedIndex)
                    {
                        Console.SetCursorPosition(0, selectedIndex + cursorTop);
                        Console.ResetColor();
                        Console.WriteLine("{0:00}. {1}", selectedIndex + 1, _dataCenters[selectedIndex]);

                        selectedIndex--;
                        Console.SetCursorPosition(0, selectedIndex + cursorTop);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("{0:00}. {1}", selectedIndex + 1, _dataCenters[selectedIndex]);
                        Console.ResetColor();
                        Console.SetCursorPosition(0, selectedIndex + cursorTop);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    if (selectedIndex < _dataCenters.Length - 1)
                    {
                        Console.SetCursorPosition(0, selectedIndex + cursorTop);
                        Console.ResetColor();
                        Console.WriteLine("{0:00}. {1}", selectedIndex + 1, _dataCenters[selectedIndex]);

                        selectedIndex++;
                        Console.SetCursorPosition(0, selectedIndex + cursorTop);
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("{0:00}. {1}", selectedIndex + 1, _dataCenters[selectedIndex]);
                        Console.ResetColor();
                        Console.SetCursorPosition(0, selectedIndex + cursorTop);
                    }
                }
            }

            Console.Clear();
            return null;
        }

        static bool IsGuid(string guid)
        {
            Regex regex = new Regex(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$");
            return regex.IsMatch(guid);
        }

        static string GetDataCenter(string guid)
        {
            var dataCenter = new INIFile(string.Format(_iniPathFormat, guid)).Read("ONLINE", "DataCenterHint");
            return dataCenter;
        }

        static void SetDataCenter(string guid, string dataCenter)
        {
            new INIFile(string.Format(_iniPathFormat, guid)).Write("ONLINE", "DataCenterHint", dataCenter);
        }
    }
}

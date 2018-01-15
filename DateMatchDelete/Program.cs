using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;

namespace DateMatchDelete
{
    class Program
    {
        static List<string> _listAll = new List<string>();
        static void Main(string[] args)
        {
            Console.WriteLine("磁盘目录按日期清理工具\nRacobit 墨林 2018\n\n使用说明："
            + "\n输入4个命令行参数:\n第1个为监控根目录\n第2个为保留天数\n第3个为每次扫描间隔秒数\n第4个为搜索深度。" 
            + "\n例如 e:\\ 30 3600 5意为删除e盘下所有按日期匹配上的30天以前的目录," 
            + "每隔3600秒扫描一次,搜索深度为5级目录\n当前匹配规则为\\2017\\1120\\即先匹配年份，再匹配月日，匹配成功并且过期则直接整个目录删除。\n\n");

            if (args.Length != 4)
            {
                Console.WriteLine("参数个数错误!\n按任意键退出...");
                Console.ReadKey();
                return;
            }

            Console.Title = "磁盘空间自动清理软件v2.0.0";

            try
            {
                while (true)
                {
                    _listAll.Clear();
                    Console.WriteLine("\n" + DateTime.Now.ToString());
                    Console.WriteLine("当前监控目录为：" + args[0]);
                    Console.WriteLine("删除日期为(天): " + args[1]);
                    Console.WriteLine("扫描间隔为(秒): " + args[2]);
                    Console.WriteLine("扫描深度为: " + args[3]);
                    DateTime dtDel = DateTime.Now - TimeSpan.FromDays(Int32.Parse(args[1]));

                    Console.WriteLine("当前删除匹配日期为" + dtDel.ToString("yyyyMMdd") + "至" + (dtDel - TimeSpan.FromDays(365 * 3)).ToString("yyyyMMdd") + "的文件目录");

                    string[] ssMatchDays = new string[365 * 3];

                    DateTime dt2000 = new DateTime(2000, 1, 1);
                    for (int i = 0; i < 365 * 3; i++)
                    {
                        ssMatchDays[i] = Path.DirectorySeparatorChar + (dtDel - TimeSpan.FromDays(i)).ToString("yyyy") + Path.DirectorySeparatorChar + (dtDel - TimeSpan.FromDays(i)).ToString("MMdd");
                    }

                    GetDirectorys(args[0], 1, Int32.Parse(args[3]));

                    MatchAndDelete(ssMatchDays);

                    Thread.Sleep(Int32.Parse(args[2]) * 1000);
                }

               

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        static void GetDirectorys(string sRoot, int iLevel, int iLevelMax)
        {
            foreach (string sPath in Directory.GetDirectories(sRoot))
            {
                _listAll.Add(sPath);

                if (iLevel < iLevelMax)
                {
                    GetDirectorys(sPath, iLevel + 1, iLevelMax);
                }
            }
        }

        static void MatchAndDelete(string[] ssMatch)
        {
            for (int i = 0; i < _listAll.Count; i++)
            {
                for (int m = 0; m < ssMatch.Length; m++)
                {
                    if (_listAll[i].Contains(ssMatch[m]))
                    {
                        Console.WriteLine("匹配成功，准备删除：" + _listAll[i]);
                        try
                        {
                            Directory.Delete(_listAll[i], true);
                            Console.WriteLine("删除成功：" + _listAll[i]);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("删除失败：" + _listAll[i]);
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
}

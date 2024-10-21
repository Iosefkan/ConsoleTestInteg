using System.Diagnostics;
using System.Text;

namespace TestingIntegralX
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (args.Length == 0)
            {
                Console.WriteLine("Программа требует минимум 1 аргумент: название файла с тест кейсами");
                return;
            }
            Analyze(args[0]);
            Console.WriteLine("Работа программы завершена, нажмите любую клавишу, чтобы продолжить...");
            Console.ReadKey(true);
        }

        static void Analyze(string fileName)
        {
            var input = File.ReadAllLines(fileName);
            int caseCount = input.Length / 4;
            var output = new List<string>();
            for (int i = 0; i < caseCount; i++)
            {
                int l = i * 4;
                output.Add(input[l]);
                try
                {
                    string line = input[l + 1].Split(" = ")[1];
                    using Process cmd = new Process();
                    cmd.StartInfo.FileName = @"Integral3x.exe";
                    cmd.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(1251); ;
                    cmd.StartInfo.StandardInputEncoding = Encoding.GetEncoding(1251); ;
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = true;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.Arguments = line;
                    cmd.Start();

                    var result = cmd.StandardOutput.ReadLine();
                    var resExpected = input[l + 2];
                    var splitExpected = resExpected.Split(" = ");
                    output.Add(result!);
                    if (splitExpected.Length == 1)
                    {
                        output.Add(result != input[l + 2] ? "NOT PASSED" : "PASSED");
                    }
                    else
                    {
                        var splitRes = result.Split(" = ");
                        if (splitRes.Length == 1)
                        {
                            output.Add("NOT PASSED");
                        }
                        else
                        {
                            var expDouble = double.Parse(splitExpected[1]);
                            var resDouble = double.Parse(splitRes[1]);
                            output.Add(expDouble != resDouble ? "NOT PASSED" : "PASSED");
                        }
                    }
                    cmd.StandardInput.Write(' ');
                    cmd.WaitForExit();
                }
                catch
                {
                    output.Add("Ошибка в исходных данных тест кейса");
                    output.Add("NOT PASSED");
                    continue;
                }
                finally
                {
                    output.Add("");
                }
            }
            File.WriteAllLines("report.txt", output);
        }
    }
}

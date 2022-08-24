using Convert.Logger;
using Convert.Moduls.Config;
using Convert.Moduls.Error;
using MLServer_2._1;
using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public class LrdExeFile : ExeFileInfo
    {
        public FileDelete FileDelete;

        public LrdExeFile(string exefile, string filenamr, string command, ref Config0 config)
                                                             : base(exefile, filenamr, command)
        {
            _ = LoggerManager.AddLoggerTask(new LoggerEvent(EnumError.Info, "Загружаем ( Load ) Class LrdExeFile"));

            FileDelete = new FileDelete(ref config);
        }

        public bool Run()
        {
            FileDelete.Run();

            var result = ExeInfo();
            _ = LoggerManager.AddLoggerTask(new LoggerEvent(EnumError.Info, new[] { " LrdExeFile:\n ", 
                $"  Код завершения программы ( Program termination code ) { result.CodeError }  " }));

            if (result.CodeError != 0)
            {
                _ = LoggerManager.AddLoggerTask(new LoggerEvent(EnumError.Info, "  LrdExeFile ->  !!!  Бардак!! ( !!!! Mess !!!  ) "));
            }

            FileDelete.SetExitRepit();

            Guid _guid= Guid.NewGuid();
            bool _isGuid = true;
            void SetFalse() => _isGuid = false;
            ThreadManager.Add(_guid, SetFalse, " LrdExeFile.Run() ");
            TypeDanFromFile0[] __oldFiles = new TypeDanFromFile0[0];
            while ((FileDelete.GetCountFilesName() > 0) && _isGuid)
            {
                var __countRec = FileDelete.GetCountFilesName(); 
                _ = LoggerManager.AddLoggerTask(new LoggerEvent(EnumError.Info, new[] { " LrdExeFile:\n "
                                                            , $"Удаляем файлы, ожидаем завершение, осталось -> \n" +
                                                            $" Delete files, wait for completion, left ->  {__countRec}" }));

                if(__countRec >0)
                {
                    TypeDanFromFile0[] x = FileDelete.MasFiles();
                    foreach (var item_x2 in x)
                    {
                        item_x2.CalcSecDateTime();
                        if (item_x2.SecWait >= 60)
                        {
                            _isGuid = false;
                            break;
                        }
                    }

                    ///////////////////////////
                    ///     Проверить x.время_старта, может быть кол-во повторений или ....
                    ///         1. Если проблема в файле -> Lrd сваливается, то перенести этот файл в новый каталог ErrorFile
                    ///         2. Если блокировка в файле и его нельзя переексти остановить процесс.
                    ///////////////////////////
                }
                
                Thread.Sleep(1000);
            }
            ThreadManager.DelRecInDict(_guid);
            FileDelete.AbortRepit();

            if (result.CodeError == 0) return false;

            _ = ErrorBasa.FError(-5);
            return false;

        }
        public override void CallBackFun(string line)
        {
            if (line.Length <= 0) return;
            _ = LoggerManager.AddLoggerTask(new LoggerEvent(EnumError.Info, $"  LrdExeFile ->  {line}  "));

            if (!line.ToLower().Contains("file")) return;

            try
            {
                var s0 = line.Split('\'');
                Lines.Add(s0[1]);
                FileDelete.Add(s0[1]);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}

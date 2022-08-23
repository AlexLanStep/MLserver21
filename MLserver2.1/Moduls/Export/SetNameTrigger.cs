﻿using Convert.Logger;
using Convert.Moduls.Config;
using Convert.Moduls.FileManager;
using MLServer_2._1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Export
{
    public class SetNameTrigger
    {
        #region data
        private Config0 _config;
        public Task RunTask = null;
        public readonly string PatternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)F";
        private readonly string _ext;
        private readonly string _pathConvert;
        private readonly FileMove _renameFile;
        private readonly string _outdir;
        private readonly string _key;
        #endregion

        #region constructor
        public SetNameTrigger(ref Config0 config, string outdir, string _key, string typeconvert)
        {
            _ = LoggerManager.AddLoggerTask(new LoggerEvent(EnumError.Info, "Загружаем ( Load ) Class SetNameTrigger"));
            _outdir = outdir;
            _config = config;
            var typeconvert1 = typeconvert.ToUpper();
            _pathConvert = _outdir + "\\";
            this._key = _key;
            _ext = "." + _config.ClexportParams[_key]["ext"];

            if (_config.FMem.Count == 0)
            {
                var x0 = _config.DbConfig
                    .Where(x => x.Key.ToLower()
                    .Contains("_m2_"))
                    .Select(y => y.Value);

                foreach (var item in x0)
                    foreach (var (key, value) in item)
                        _config.FMem.AddOrUpdate("M2_" + key, value, (_, _) => value);

                x0 = _config.DbConfig
                    .Where(x => x.Key.ToLower()
                    .Contains("_m1_"))
                    .Select(y => y.Value);
                foreach (var item in x0)
                    foreach (var (key, value) in item)
                        _config.FMem.AddOrUpdate("M1_" + key, value, (_, _) => value);

            }

            _renameFile = new FileMove(_pathConvert, _pathConvert);
            _renameFile.Run();
        }
        #endregion

        #region Public Function Run
        public void Run()
        {
            _config.IsRun.IsExportRename = true;
            Guid _guid = Guid.NewGuid();
            bool _isGuid = true;
            void SetFalse() => _isGuid = false;
            ThreadManager.Add(_guid, SetFalse, " SetNameTrigger.Run() ");

            while ((_config.IsRun.IsExport || _findFileDirClf().Count > 0) && _isGuid)
            {
                var ls = _findFileDirClf();
                if (ls.Count <= 0)
                {
                  Thread.Sleep(1000);
                  continue;
                }
                var lsM1 = ls.Where(x => x.ToLower().Contains("_m1_") && x.ToUpper().Contains(")F")).ToList();
                var lsM2 = ls.Where(x => x.ToLower().Contains("_m2_") && x.ToUpper().Contains(")F")).ToList();
                Task waitM1 = null;
                Task waitM2 = null;

                if (lsM1.Count > 0)
                    waitM1 = Task.Run(() => _rename_m1(lsM1));

                if (lsM2.Count > 0)
                    waitM2 = Task.Run(() => _rename_m2(lsM2));

                waitM1?.Wait();
                waitM2?.Wait();
                Thread.Sleep(1000);
            }

            ThreadManager.DelRecInDict(_guid);
            _config.IsRun.IsExportRename = false;

            while (_renameFile.GetCountFilesNameQueue() > 0)
                Thread.Sleep(300);

            _renameFile.AbortRepit();

        }



        #endregion

        #region Find files
        private List<string> _findFileDirClf()
        {
            var ss = "*" + _ext;
            var path = _pathConvert;
            var x01 = Directory.GetFiles(path, ss);
            return (from item in x01
                    where Regex.Matches(item, PatternFile, RegexOptions.IgnoreCase).Count == 1
                    select Path.GetFileName(item))
                        .ToList();
        }
        private Task _rename_m1(List<string> ls1)
          => Task.Run(() =>
              {
//                  var z = _config.FMem;
                  var ls = new List<string>(ls1);
                  foreach (var item in ls)
                  {
                      var file = Path.GetFileName(item);

                      try
                      {
                          var filePatch = _pathConvert + item;
                          using (File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                          { }
                      }
                      catch (IOException)
                      {
                          continue;
                      }

                      var file0 = Path.GetFileNameWithoutExtension(file).Split("_", 3);
                      string _m1F = "M1_" + Regex.Match(file0[2], @"F\d{3,5}", RegexOptions.IgnoreCase).Value;
                      string _newFile = "";

                      if (_config.FMem.ContainsKey(_m1F))
                          _newFile = file0[0] + "_M1_" + _config.FMem[_m1F].StartEndMem + Path.GetExtension(file);
                      else
                          continue;

                      _renameFile.Add(item, _newFile);

                  }
              });
        
        private  Task _rename_m2(IReadOnlyCollection<string> ls1)
          => Task.Run(() =>
            {
                var z = _config.FMem;

                List<string> ls = new(ls1);
                foreach (var item in ls)
                {
                    var file = Path.GetFileName(item).ToUpper();

                    try
                    {
                        var filePatch = _pathConvert + item;
                        using (File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                        { }
                    }
                    catch (IOException)
                    {
                        continue;
                    }

                    var file0 = Path.GetFileNameWithoutExtension(file).Split("_", 3);
                    string m2Fmem = "M2_" + Regex.Match(file0[2], @"F\d{3,5}", RegexOptions.IgnoreCase).Value;
                    string _newFile = "";

                    if (_config.FMem.ContainsKey(m2Fmem))
                        _newFile = file0[0] + "_M2_" + _config.FMem[m2Fmem].StartEndMem;
                    else
                        continue;

                    var fnum = "";
                    if (_config.FMem.ContainsKey(m2Fmem))
                        fnum = _config.FMem[m2Fmem].GetNameTrigger();

                    _newFile = _newFile + fnum + Path.GetExtension(file); 
                    _renameFile.Add(item, _newFile);
                }
            });
        
        #endregion

    }
}

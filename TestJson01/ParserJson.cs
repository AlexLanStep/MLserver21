using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsJson
{
    
    public class ParserJson
    {
        private readonly string _pathFiles;

        public ParserJson(string pathFiles)
        {
            _pathFiles = pathFiles;
        }

        public string? LoadFileJso(string filejson)
        {
            return !File.Exists(filejson) ? null: File.ReadAllText(filejson);

//                : JsonConvert.DeserializeObject<T>(File.ReadAllText(filejson));
//                : 
        }

        public void Run()
        {
            string? _tsxtJson = LoadFileJso(_pathFiles);
            if (_tsxtJson != null)
                _convertJson(_tsxtJson);
        }

        private void _convertJson(string tsxtJson)
        {
            //            JObject googleSearch = JObject.Parse(tsxtJson);

            JObject googleSearch = JObject.Parse(tsxtJson);
            IList<JToken> results = googleSearch.Children().ToList();

            foreach (JToken item in results)
            {
                string name = ((JProperty)item).Name;
                var _val = ((JProperty)item).Value;
                Console.WriteLine($" name - {name}   item {item.ToString()}  ");

                switch (name)
                {
                    case "clexport":
                        {
                            IList<JToken> results0 = item.Children().ToList();
                            var results1 = googleSearch["clexport"].Children();
                            var results2 = googleSearch["clexport"].Children().ToList();
                            //var value = JsonConvert.DeserializeObject<Dictionary<string, object>>(results2[0]);
                            continue;
                        }
                    case "error":
                        {
                            continue;
                        }
                    case "lrf_dec":
                        {
                            continue;
                        }
                    case "pool":
                        {
                            continue;
                        }
                    case "timewait":
                        {
                            continue;
                        }
                    case "car":
                        {

                            continue;
                        }
                    default:
                        return;
                }

            }

            IList<JToken> _error = googleSearch["error"].Children().ToList();
            var _pool = googleSearch["pool"];
           var _timewait = googleSearch["timewait"];

//            IDictionary<string, JToken> _clexport = googleSearch["clexport"].Children().ToDictionary();
            IList<JToken> _car = googleSearch["car"].Children().ToList();

            int kk = 1;
        }
    }
}

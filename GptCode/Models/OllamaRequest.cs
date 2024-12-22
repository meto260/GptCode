using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GptCode.Models {
    public class OllamaRequest {
        public required string Model { get; set; }
        public required string Prompt { get; set; }
        public string System { get; set; }
        public bool Stream { get; set; }
    }
}

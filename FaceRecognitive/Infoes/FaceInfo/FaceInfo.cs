using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitive.Infoes.FaceInfo
{
    public class FaceInfo
    {
        public byte[] Image { get; set; }

        public int Id { get; set; }

        public String Label { get; set; }

        public int UserId { get; set; }
    }
}

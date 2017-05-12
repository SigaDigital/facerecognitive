using FaceRecognitive.Infoes.FaceInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitive.Engine.Storage
{
    public static class VirtualFaceDatabase
    {
        public static List<FaceInfo> Faces { get; set; } = new List<FaceInfo>();
    }
}

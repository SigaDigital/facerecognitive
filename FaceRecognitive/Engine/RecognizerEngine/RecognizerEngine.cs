using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using FaceRecognitive.Services.FaceService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitive.Engine.RecognizerEngine
{
    public class RecognizerEngine
    {
        private FaceService faceService_;
        private FaceRecognizer faceRecognizer_;
        private string recognizerFilePath_ = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + String.Format("/recognizerEngine/recognizer");

        public RecognizerEngine()
        {
            faceService_ = new FaceService();
            faceRecognizer_ = new EigenFaceRecognizer(80, double.PositiveInfinity);
        }

        public bool Train()
        {
            var allFaces = faceService_.CallFaces("ALL_USERS");
            if (allFaces != null)
            {
                var faceImages = new Image<Gray, byte>[allFaces.Count];
                var faceLabels = new int[allFaces.Count];
                for (int i = 0; i < allFaces.Count; i++)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(allFaces[i].Image, 0, allFaces[i].Image.Length);
                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages[i] = faceImage.Resize(100, 100, Inter.Cubic);
                    faceLabels[i] = allFaces[i].UserId;
                }

                faceRecognizer_.Train(faceImages, faceLabels);
                faceRecognizer_.Save(recognizerFilePath_);
            }

            return true;
        }

        public int Recognize(Image<Gray, byte> userImage)
        {
            faceRecognizer_.Load(recognizerFilePath_);
            var result = faceRecognizer_.Predict(userImage.Resize(100, 100, Inter.Cubic));

            int id = 0;
            if (result.Distance <= 150)
            {
                id = result.Label;
            }

            return id;
        }
    }
}

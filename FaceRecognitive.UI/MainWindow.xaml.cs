using Emgu.CV;
using Emgu.CV.Structure;
using FaceRecognitive.Engine.RecognizerEngine;
using FaceRecognitive.Services.FaceService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FaceRecognitive.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FaceService dataStore = new FaceService();
        RecognizerEngine engine = new RecognizerEngine();

        public MainWindow()
        {
            InitializeComponent();
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void Input_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string filename = dialog.SelectedPath;
                    var imagePaths = Directory.GetFiles(filename);

                    foreach (var imgPath in imagePaths)
                    {
                        Capture capture = new Capture(imgPath);

                        var path = @"C:\opencv\build\etc\haarcascades\haarcascade_frontalface_alt2.xml";
                        try
                        {
                            CascadeClassifier _cascadeClassifier = new CascadeClassifier(path);
                            using (var imageFrame = capture.QueryFrame().ToImage<Bgr, Byte>())
                            {
                                if (imageFrame != null)
                                {
                                    var grayframe = imageFrame.Convert<Gray, byte>();
                                    var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, System.Drawing.Size.Empty);
                                    foreach (var face in faces)
                                    {
                                        imageFrame.Draw(face, new Bgr(Color.Red), 3);

                                    }
                                }

                                var personalFace = BitmapToImageSource(imageFrame.Bitmap);
                                image.Source = personalFace;

                                var faceToSave = new Image<Gray, byte>(imageFrame.Bitmap);
                                Byte[] faceImageInBytes;

                                var username = NameTextBox.Text;
                                var filePath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + String.Format("/recognizer/{0}.jpg", username);
                                faceToSave.ToBitmap().Save(filePath);
                                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                {
                                    using (var reader = new BinaryReader(stream))
                                    {
                                        faceImageInBytes = reader.ReadBytes((int)stream.Length);
                                    }
                                }
                                var saveResult = dataStore.SaveFace(username, faceImageInBytes);
                                engine.Train();
                                System.Windows.Forms.MessageBox.Show(saveResult, "Save Result", MessageBoxButtons.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        private void Recognize_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            string filename;
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;

                Capture capture = new Capture(filename);

                var path = @"C:\opencv\build\etc\haarcascades\haarcascade_frontalface_alt2.xml";
                try
                {
                    CascadeClassifier _cascadeClassifier = new CascadeClassifier(path);
                    using (var imageFrame = capture.QueryFrame().ToImage<Bgr, Byte>())
                    {
                        if (imageFrame != null)
                        {
                            var grayframe = imageFrame.Convert<Gray, byte>();
                            var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, System.Drawing.Size.Empty);
                            foreach (var face in faces)
                            {
                                imageFrame.Draw(face, new Bgr(Color.Red), 3);
                            }
                        }

                        var personalFace = BitmapToImageSource(imageFrame.Bitmap);
                        image.Source = personalFace;

                        var faceToRecognize = new Image<Gray, byte>(imageFrame.Bitmap);
                        var recogResult = dataStore.GetUsername(engine.Recognize(faceToRecognize));

                        if (recogResult == "") recogResult = "No match.";

                        System.Windows.Forms.MessageBox.Show(recogResult, "Recognition Result", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}

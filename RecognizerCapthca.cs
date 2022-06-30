using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp1
{
    class RecognizerCapthca
    {

        Tesseract tesseract = new Tesseract(@"C:\TesseractModels", "eng", OcrEngineMode.TesseractLstmCombined);
        private string directoryPath;
        private static string recognizedText = "";
        public static string neededNumber;

        private Point[,] matrixPoints = new Point[4, 3]
        {
                {new Point(865,690),new Point(968,689),new Point(1074,690) },
                {new Point(865,735),new Point(968,735),new Point(1074,735) },
                {new Point(865,776),new Point(968,776),new Point(1074,776) },
                {new Point(),new Point(968,820),new Point() }
        };
        private string[,] matrixNumbers = new string[4, 3]
        {
            {"","",""},
            {"","",""},
            {"","",""},
            {"","",""},

        };


        public RecognizerCapthca(Point[,] personalPointsOfNumbers)
        {
            matrixPoints = personalPointsOfNumbers;
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
                string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("USER32.DLL", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
      
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dsFlags, int dx, int dy, int cButtons, int dsExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        static void LeftClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }
        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            return destImage;
        }
       
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        
        public void RecognizeCaptcha(string filePath, IWebDriver driver, IWebElement blockOfCaptcha)
        {
            IntPtr chrome = FindWindow("Chrome_RenderWidgetHostHWND", "Chrome Legacy Window");

            Actions action = new Actions(driver);

            SetForegroundWindow(chrome);

 
            for (int i = 0; i <= 3; i++)
            {
               

                blockOfCaptcha = driver.FindElement(By.XPath("//div[@class = 'konvajs-content']"));

                action.MoveToElement(blockOfCaptcha, 234, 60).ContextClick().Build().Perform();

                Thread.Sleep(250);
                SendKeys.SendWait("{Enter}");
                Thread.Sleep(250);
                SendKeys.SendWait("{Enter}");
                Thread.Sleep(1600);
                SendKeys.SendWait("d.bmp");
                Thread.Sleep(250);
                SendKeys.SendWait("{Enter}");
                Thread.Sleep(250);
                SendKeys.SendWait("{Left}");
                Thread.Sleep(250);
                SendKeys.SendWait("{Enter}");

          
                CreateImagesForNeuron(filePath);
       
                tesseract.SetImage(new Image<Bgr, byte>($@"C:\CurrentCaptcha\line1.bmp"));
                tesseract.Recognize();
                recognizedText += tesseract.GetUTF8Text();

                tesseract.SetImage(new Image<Bgr, byte>($@"C:\CurrentCaptcha\line2.bmp"));
                tesseract.Recognize();
                recognizedText += tesseract.GetUTF8Text();

                tesseract.SetImage(new Image<Bgr, byte>($@"C:\CurrentCaptcha\line3.bmp"));
                tesseract.Recognize();
                recognizedText += tesseract.GetUTF8Text();

                tesseract.SetImage(new Image<Bgr, byte>($@"C:\CurrentCaptcha\line4.bmp"));
                tesseract.Recognize();
                recognizedText += tesseract.GetUTF8Text();
                string normalText = GetNormalString(recognizedText);
                FillArray(normalText, matrixNumbers);

                File.Delete($@"C:\CurrentCaptcha\line1.bmp");
                File.Delete($@"C:\CurrentCaptcha\line2.bmp");
                File.Delete($@"C:\CurrentCaptcha\line3.bmp");
                File.Delete($@"C:\CurrentCaptcha\line4.bmp");
     
                Point currentPoint;
                switch (neededNumber[i])
                {
                    case '1':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "1").Item1, CoordinatesOf<string>(matrixNumbers, "1").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '2':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "2").Item1, CoordinatesOf<string>(matrixNumbers, "2").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '3':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "3").Item1, CoordinatesOf<string>(matrixNumbers, "3").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '4':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "4").Item1, CoordinatesOf<string>(matrixNumbers, "4").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '5':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "5").Item1, CoordinatesOf<string>(matrixNumbers, "5").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '6':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "6").Item1, CoordinatesOf<string>(matrixNumbers, "6").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '7':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "7").Item1, CoordinatesOf<string>(matrixNumbers, "7").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '8':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "8").Item1, CoordinatesOf<string>(matrixNumbers, "8").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '9':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "9").Item1, CoordinatesOf<string>(matrixNumbers, "9").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    case '0':
                        currentPoint = matrixPoints[CoordinatesOf<string>(matrixNumbers, "0").Item1, CoordinatesOf<string>(matrixNumbers, "0").Item2];
                        Cursor.Position = currentPoint;
                        LeftClick(currentPoint.X, currentPoint.Y);
                        break;
                    default:
                        break;
                }
                recognizedText = "";
            }
        }
        private string GetNormalString(string str)
        {
            string normalString = "";
            normalString = str.Replace(" ", "").Replace("\n", "").Replace("o", "0").Replace("\r", "");
            return normalString;
        }
        private void CreateImagesForNeuron(string filePath)
        {
            Thread.Sleep(1300);
            directoryPath = Path.GetDirectoryName(filePath);

            for (int imageLine = 1; imageLine <= 4; imageLine++)
            {
                Bitmap img = new Bitmap(filePath);
                Rectangle section;
                Bitmap CroppedImage;

                switch (imageLine)
                {
                    case 1:
                        section = new Rectangle(new Point(0, 62), new Size(280, 37));
                        CroppedImage = CropImage(img, section);
                        break;
                    case 2:
                        section = new Rectangle(new Point(0, 100), new Size(280, 37));
                        CroppedImage = CropImage(img, section);
                        break;
                    case 3:
                        section = new Rectangle(new Point(0, 140), new Size(280, 37));
                        CroppedImage = CropImage(img, section);
                        break;
                    case 4:
                        section = new Rectangle(new Point(93, 178), new Size(90, 35));
                        CroppedImage = CropImage(img, section);
                        break;
                    default:
                        section = new Rectangle(new Point(0, 62), new Size(280, 37));
                        CroppedImage = CropImage(img, section);
                        break;
                }

                img.Dispose();
                Bitmap output = new Bitmap(CroppedImage.Width, CroppedImage.Height);
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < CroppedImage.Height; j++)
                    for (int x = 0; x < CroppedImage.Width; x++)
                    {
                        // получаем (i, j) пиксель
                        UInt32 pixel = (UInt32)(CroppedImage.GetPixel(x, j).ToArgb());
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(x, j, Color.FromArgb((int)newPixel));
                    }
                
                FloodFill(CroppedImage, 0, CroppedImage.Height - 2, Color.White, imageLine);

                img.Dispose();
                CroppedImage.Dispose();
            }
        }
        private void FillArray(string str, string[,] matrix)
        {
            matrix[0, 0] = str[0].ToString();
            matrix[0, 1] = str[1].ToString();
            matrix[0, 2] = str[2].ToString();
            matrix[1, 0] = str[3].ToString();
            matrix[1, 1] = str[4].ToString();
            matrix[1, 2] = str[5].ToString();
            matrix[2, 0] = str[6].ToString();
            matrix[2, 1] = str[7].ToString();
            matrix[2, 2] = str[8].ToString();
            matrix[3, 0] = "";
            matrix[3, 1] = str[9].ToString();
            matrix[3, 2] = "";

        }
        private Tuple<int, int> CoordinatesOf<T>(T[,] matrixNumbers, T value)
        {
            int w = matrixNumbers.GetLength(0); // width
            int h = matrixNumbers.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrixNumbers[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }
        private void FloodFill(Bitmap bitmap, int x, int y, Color color, int imgLine)
        {
            Thread.Sleep(1400);
            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            var bits = new int[data.Stride / 4 * data.Height];
            Marshal.Copy(data.Scan0, bits, 0, bits.Length);

            var check = new LinkedList<Point>();
            var floodTo = color.ToArgb();
            var floodFrom = bits[x + y * data.Stride / 4];
            bits[x + y * data.Stride / 4] = floodTo;

            if (floodTo != floodFrom)
            {
                check.AddLast(new Point(x, y));
                while (check.Count > 0)
                {
                    var cur = check.First.Value;
                    check.RemoveFirst();

                    foreach (var off in new[]
                    {
                        new Point(-1, 0),new Point(-1, -1),
                        new Point(1, 0), new Point(1, 1),  //Точки для просмотриа
                        new Point(0, -1), new Point(0, 1)
                    })
                    {
                        var next = new Point(cur.X + off.X, cur.Y + off.Y);
                        if (next.X < 0 || next.Y < 0 || next.X >= data.Width || next.Y >= data.Height) continue;
                        var col = bits[next.X + next.Y * data.Stride / 4];
                        if (floodFrom != col) continue;
                        check.AddLast(next);
                        bits[next.X + next.Y * data.Stride / 4] = floodTo;
                    }
                }
            }

            Marshal.Copy(bits, 0, data.Scan0, bits.Length);
            bitmap.UnlockBits(data);

            Bitmap orig = bitmap;
            Bitmap clone = new Bitmap(orig.Width, orig.Height, PixelFormat.Format32bppArgb);

            using (Graphics gr = Graphics.FromImage(clone))
            {
                gr.DrawImage(orig, new Rectangle(0, 0, clone.Width, clone.Height));
                bitmap.Dispose();

                clone.Save(directoryPath + $"\\line{imgLine}.bmp", ImageFormat.Bmp);
                bitmap.Dispose();
                orig.Dispose();
                clone.Dispose();
            }
        }
    }
}

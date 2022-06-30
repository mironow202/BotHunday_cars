using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using Resul_Num;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace BotHunday_cars
{
    class Bot_vehicle
    {
        private static ChromeDriver driver;
        private static Actions action;

        private const string comparisonvehicle = "filter-models__checkbox b-checkbox_model b-checkbox_model--disabled";
        private static readonly string writePath = "WriteLines.txt";
        private const string comparisonverificationloadpage = "header-support__title text text-sm family-head dark-grey";
        private const string comparisonauhtorizate = "Мой кабинет";
        private static string neededNumber;

        private static bool authorization = false;
        private static bool stopverificationcycle = true;
        private static bool exception = false;
        private static bool stopcyclvehicle = true;
        public static bool reboot = false;
        private static bool rebootcontunie = false;

        private async static void Write(string log = "")
        {
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                await sw.WriteLineAsync($"{DateTime.Now} {log}");
            }
        }

        private static string GetMethodApi()
        {
            while (true)
            {
                var client = new RestClient("https://api.asknpdmobile.ru/bothyundai/controller/GetMassage");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AlwaysMultipartFormData = true;
                request.AddParameter("phonenumber", "");
                IRestResponse response = client.Execute(request);
                var result = JsonConvert.DeserializeObject<ResultNumber>(response.Content.ToString());//десиарилизация json
                neededNumber = result.Number;
                Console.WriteLine(result.Number);
                if (result.Number == null)
                {
                    Console.WriteLine("null");
                }
                if (result.Number != null)
                {
                    return neededNumber;
                }
            }
        }

        #region number & click
        private static List<int> someList = new List<int>()
         {
              40,
              18,
              34,
              33,
              31,
              36,
              43,
              26,
              42,
              45,
        };

        private static List<Point> pointsOfNumbers = new List<Point>
        {
            //0
            new Point(966,740),
            //1
            new Point(876,622),
            //2
            new Point(968,622),
            //3
            new Point(1061,622),
            //4
            new Point(873,666),
            //5
            new Point(968,666),
            //6
            new Point(1065,666),
            //7
            new Point(871,700),
            //8
            new Point(970,700),
            //9
            new Point(1061,700)

        };

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dsFlags, int dx, int dy, int cButtons, int dsExtraInfo);
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        static void LeftClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }
        private static Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }
        #endregion

        #region verification
        private static void VerificationLoadWeb()
        {
            while (stopverificationcycle)
            {
                Thread.Sleep(1000);
                try
                {
                    var erorwebsite = driver.FindElement(By.XPath("//header[@role='banner']/div[1]/div/div[@class='header-support__title text text-sm family-head dark-grey']"));
                    var atributeerorwebsite = erorwebsite.GetAttribute("class");
                    if (atributeerorwebsite == comparisonverificationloadpage)
                    {
                        Console.WriteLine("САЙТ ЗАГРУЖЕН. ОШИБОК НЕТ.");
                        reboot = false;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    driver.Navigate().Refresh();
                    Write($"{ex.Message} в page.");
                    Console.WriteLine("Страница не была загружена. Перезагрузка." + ex.Message);
                    Thread.Sleep(2000);
                }
            }
        }
        private static void VerificationAuthorization()
        {
            try
            {
                WebDriverWait waits = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
                var verificateelement = waits.Until(e => e.FindElement(By.XPath("//header[@role='banner']/div[1]/div[2]/div/a/span")));
                var verificatetext = verificateelement.Text;
                if (verificatetext == comparisonauhtorizate)
                {
                    Console.WriteLine("Авторизирован. Продолжаем работу");
                    authorization = true;
                }
                else
                {
                    Console.WriteLine("Введите свой номер телефона");
                    string num = Console.ReadLine();
                    driver.Navigate().GoToUrl("https://showroom.hyundai.ru/auth");
                    var click = waits.Until(e => e.FindElement(By.XPath("//main[@role='main']/div[1]/div[1]/form/div[2]/div[1]/input")));
                    click.SendKeys(num);

                    var buttonclick = waits.Until(e => e.FindElement(By.XPath("//main[@role='main']/div[1]/div[1]/form/div[3]/button/span")));
                    buttonclick.Click();

                    Thread.Sleep(1500);
                    IWebElement blockOfCaptcha = waits.Until(e => e.FindElement(By.XPath("//div[@class='konvajs-content']")));
                    Thread.Sleep(25000);//wait call  

                    GetMethodApi();
                    Console.WriteLine(DateTime.Now);
                    CaptureElementScreenShot(blockOfCaptcha);
                    Console.WriteLine(DateTime.Now);

                    var checkauth = waits.Until(e => e.FindElement(By.XPath("//main[@role='main']/div[1]/div/div[3]/div/div/div[contains(text(), 'У вас нет бронирований')]")));
                    if (checkauth.Text == "У вас нет бронирований")
                    {
                        var buttongocheckvehicle = driver.FindElement(By.XPath("//main[@role='main']/div[1]/div/div[3]/div/div/div[3]/a/div"));
                        buttongocheckvehicle.Click();
                        driver.Navigate().GoToUrl("https://showroom.hyundai.ru");
                    }
                }
            }
            catch (Exception ex)
            {
                Write($"{ex.Message}");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
        #endregion

        #region step 1 - 5
        public static void VerificationVehicle()
        {
            try
            {
                WebDriverWait waits = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                Thread.Sleep(2500);

                var verificationtext = waits.Until(e => e.FindElement(By.XPath("//div[@class='sweet-buttons']/button[@class='btn']/span[contains(text(), 'Я соглашаюсь с условиями')]")));//соглашение
                verificationtext.Click();

                Write("Соглашение нажато.");
            }
            catch (Exception ex)
            {
                Write($"{ex.Message} в соглашении.");
                Console.WriteLine(ex.Message + "Соглашение bag");
            }
        }
        public static void AutoServices()
        {
            try
            {
                action = new Actions(driver);
                Thread.Sleep(1500);
                IWebElement multiDiler = driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div[1]/div/form/div[2]/div[1]/div[2]/div[2]/div[1]/div[2]/span"));
                action.MoveToElement(multiDiler).Click().Build().Perform();

                Thread.Sleep(1000);
                SendKeys.SendWait("АвтоСпецЦентр Внуково");
                Thread.Sleep(1000);
                SendKeys.SendWait("{Enter}");
            }
            catch (Exception ex)
            {
                Write($"{ex.Message} в автосервисе.");
                Console.WriteLine(ex.Message);
            }
        }
        public static void MultiCountry()
        {
            try
            {
                var multiCountry = driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div[1]/div/form/div[2]/div[1]/div[2]/div[1]/div[1]/div[2]"));//города
                multiCountry.Click();

                Console.WriteLine("Москва");
                Thread.Sleep(1500);
                SendKeys.SendWait("Москва");
                Thread.Sleep(1000);
                SendKeys.SendWait("{Enter}");
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
                Console.WriteLine(ex.Message);
                Write($"{ex.Message} в мультидиллере города.");
            }
        }
        private static void Continue()
        {
            try
            {
                var contin = driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div/div[2]/div/div/div/div/div[1]/div[3]/div/div[2]/div[1]/div/span"));//подробнее
                contin.Click();
                Console.WriteLine("Выполнено Подробнее " + DateTime.Now);
                Write("выполнено нажатие ПОДРОБНЕЕ");
            }
            catch (Exception ex)
            {
                Write($"{ex.Message} в Continue.");
                rebootcontunie = true;
                Console.WriteLine("Continue " + ex.Message + DateTime.Now);
                return;
            }
        }

        private static void BuyOnlne()
        {
            try
            {
                Thread.Sleep(2000);
                var buyonline = driver.FindElement(By.XPath("/html/body/div[1]/main/div[1]/div/div/div[2]/div[6]/div[2]/div/a/span"));//купить онлайн
                buyonline.Click();
                Console.WriteLine("Выполнено купить онлайн." + DateTime.Now);
                Write("Выполнено нажатие КУПИТЬ ОНЛАЙН");
            }
            catch (Exception ex)
            {
                Write($"{ex.Message} в BuyOnlne.");
                Console.WriteLine($"{ex.Message} BuyOnlne");
                Console.ReadLine();
            }
        }


        private static void CheckBoxInput()
        {
            try
            {
                var checkBox1 = driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div[1]/div/form/div[3]/div[1]/label/span[1]/span"));//чекбоксы
                checkBox1.Click();
                var checkBox2 = driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div[1]/div/form/div[3]/div[2]/label/span[1]/span"));//чекбоксы
                checkBox2.Click();
                var checkBox3 = driver.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div[1]/div/form/div[3]/div[3]/label/span[1]/span"));//чекбоксы
                checkBox3.Click();
                Write("Чекбоксы соглашения поставлены.");
                Console.WriteLine("Чекбоксы проставлены" + DateTime.Now);
            }
            catch (Exception ex)
            {
                Write($"{ex.Message} в чекбоксах.");
                Console.WriteLine($"{ex.Message} чекбоксах" + DateTime.Now);
                reboot = true;
                MainMethod();
            }
        }
        #endregion

        public static void CaptureElementScreenShot(IWebElement element)
        {
            Point currentPoint;
            for (int k = 0; k < 4; k++)
            {
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                Image img;
                using (var ms = new MemoryStream(screenshot.AsByteArray))
                {
                    img = Image.FromStream(ms);
                }

                Rectangle rect = new Rectangle();
                if (element != null)
                {
                    int width = element.Size.Width;
                    int height = element.Size.Height;
                    Point p = element.Location;
                    rect = new Rectangle(p.X, p.Y, width, height);
                }
                Bitmap bmpImage = new Bitmap(img);
                var cropedImag = bmpImage.Clone(rect, bmpImage.PixelFormat);
                for (int imageline = 1; imageline <= 10; imageline++)
                {
                    Bitmap imgage = new Bitmap(cropedImag);
                    Rectangle section;
                    Bitmap CroppedImage;
                    int counter = 0;
                    switch (imageline)
                    {
                        case 1:
                            section = new Rectangle(new Point(7, 65), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 2:
                            section = new Rectangle(new Point(99, 65), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 3:
                            section = new Rectangle(new Point(194, 65), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 4:
                            section = new Rectangle(new Point(7, 105), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 5:
                            section = new Rectangle(new Point(99, 104), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 6:
                            section = new Rectangle(new Point(194, 104), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 7:
                            section = new Rectangle(new Point(4, 142), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 8:
                            section = new Rectangle(new Point(99, 142), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 9:
                            section = new Rectangle(new Point(194, 142), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        case 10:
                            section = new Rectangle(new Point(99, 182), new Size(79, 30));
                            CroppedImage = CropImage(imgage, section);
                            break;
                        default:
                            section = new Rectangle(new Point(0, 65), new Size(308, 44));
                            CroppedImage = CropImage(imgage, section);
                            break;

                    }
                    for (int i = 0; i < CroppedImage.Width; i++)
                    {
                        for (int j = 0; j < CroppedImage.Height; j++)
                        { if (CroppedImage.GetPixel(i, j).GetBrightness() < 0.5) counter++; }
                    }
                    for (int i = 0; i < someList.Count; i++)
                    {
                        if (counter == someList[i])
                        {
                            if (int.Parse(neededNumber[k].ToString()) == i)
                            {

                                Thread.Sleep(100);
                                currentPoint = pointsOfNumbers[imageline];
                                Cursor.Position = currentPoint;
                                Thread.Sleep(100);
                                LeftClick(currentPoint.X, currentPoint.Y);
                            }
                        }
                    }
                }
                img.Dispose();
                cropedImag.Dispose();
            }
        }

        private static void Fingeringcars()
        {
            WebDriverWait waits = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            try
            {
                while (stopcyclvehicle)
                {
                    VerificationLoadWeb();
                    Thread.Sleep(2000);
                    VerificationAuthorization();
                    List<IWebElement> webDrivers = new List<IWebElement>
                    {
                        waits.Until(e => e.FindElement(By.XPath("//div[@class='filter-models__items']/div/input[@id='models-29']"))),
                        waits.Until(e => e.FindElement(By.XPath("//div[@class='filter-models__items']/div/input[@id='models-30']"))),
                        waits.Until(e => e.FindElement(By.XPath("//div[@class='filter-models__items']/div/input[@id='models-34']"))),
                        waits.Until(e => e.FindElement(By.XPath("//div[@class='filter-models__items']/div/input[@id='models-36']"))),
                        waits.Until(e => e.FindElement(By.XPath("//div[@class='filter-models__items']/div/input[@id='models-33']"))),
                        waits.Until(e => e.FindElement(By.XPath("//div[@class='filter-models__items']/div/input[@id='models-35']"))),
                        waits.Until(e => e.FindElement(By.XPath("//div[@class='filter-models__items']/div/input[@id='models-37']")))
                    };

                    foreach (var element in webDrivers)
                    {
                        var checkbox = element.GetAttribute("class");

                        if (checkbox != comparisonvehicle)
                        {
                            stopcyclvehicle = false;
                            Write("чекбокс ИЗМЕНИЛСЯ WARNING.");
                            break;
                        }
                        else Console.WriteLine($"Чекбокс не изменился. Перезагрузка ");
                    }
                    if (stopcyclvehicle == false)
                    {
                        Write("Нашел");
                        Reverse();
                        break;
                    }
                    webDrivers.Clear();
                    Thread.Sleep(4000);
                    driver.Navigate().Refresh();
                }
                if (stopcyclvehicle == false) Reverse();
            }
            catch (Exception ex)
            {
                Console.ReadLine();
                Console.WriteLine(ex.Message);
                driver.Navigate().Refresh();
                driver.Navigate().GoToUrl("https://showroom.hyundai.ru");
                exception = true;
            }
        }

        public static void Reverse()
        {
            Write("Трейтий этап");

            WebDriverWait waits = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            Write("Шаг №1");
            Continue();

            Write("Шаг №2");

            Thread.Sleep(1000);

            BuyOnlne();
            Write("Шаг №3");

            VerificationVehicle();
            Write("Шаг №4");
            Thread.Sleep(1000);
            MultiCountry();

            Write("Шаг №5");

            Thread.Sleep(1000);
            AutoServices();

            Actions actions = new Actions(driver);
            actions.SendKeys(OpenQA.Selenium.Keys.PageDown).Build().Perform();
            actions.SendKeys(OpenQA.Selenium.Keys.PageDown).Build().Perform();
            CheckBoxInput();
            Thread.Sleep(8000);

            var toreserve = waits.Until(e => e.FindElement(By.XPath("/html/body/div[1]/main/div[2]/div[1]/div/form/div[4]/button/span")));
            toreserve.Click();
            Console.WriteLine("Забронировать нажато.");

            Write("Забронировать нажато.");
            Thread.Sleep(3000);

            IWebElement blockOfCaptcha = waits.Until(e => e.FindElement(By.XPath("//div[@class='konvajs-content']")));
            Thread.Sleep(8500);
            GetMethodApi();

            Console.WriteLine("Апишка отработала. Отправляем нейронке");
            CaptureElementScreenShot(blockOfCaptcha);
            Console.ReadLine();
        }

        private static void Load()
        {
            try
            {
                if (reboot == true)
                {
                    driver.Navigate().GoToUrl("https://showroom.hyundai.ru");
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(40);
                    driver.Manage().Window.Maximize();
                }
                else
                {
                    reboot = false;
                    ChromeOptions options = new ChromeOptions();
                    options.AddArguments(@"user-data-dir=C:\Users\cod\AppData\Local\Google\Chrome\User Data\Profile 2");
                    options.AddArguments("--start-maximized");
                    options.AddArguments("--window-size=1920,1080");
                    driver = new ChromeDriver(options);
                    driver.Navigate().GoToUrl("https://showroom.hyundai.ru");
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(40);
                    driver.Manage().Window.Maximize();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Write(ex.Message + "Load");
                driver.Navigate().Refresh();
                reboot = true;
                return;
            }   
        }
        public static void MainMethod()
        {
            Load();
            VerificationLoadWeb();
            if (reboot == true) Load();
            Fingeringcars();
        }
        private static void Main(string[] args)
        {
            MainMethod();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.IO;
using System.Drawing;
using System.Reflection;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;


namespace Selenium
{
    /// <summary>
    /// list of all constant values
    /// </summary>
    public static class Consts
    {
        //log file name and location
        public const string LOG_FILE_NAME = "TestLog";
        public const int WAIT_SECONDS = 5;
        public const string UPDATE_IFRAME_ELEMENT= "news_ticker_iframe";
        public const string CAPTCHA_ELEMENT= "//a[2]";
        public const string REMARKS_TEXT="tufin test";
        public const string FROM_MAIL_ADDRESS = "moiseev_yoav@hotmail.com";
        public const string FROM_NAME= "Yoav Moiseev";
        public const string TO_MAIL_ADDRESS="moiseev.yoav@gmail.com";
        public const string SEND_TO_FRIEND_LINK_ELEMENT= "//article[@id='F_Content']/div/div/div/div/div[2]/div/div/ul/li[5]/a/span[2]";
        public const string ARTICLE_LINK_ELEMENT="//article[@id='F_Content']/div/div[2]/div/div[4]/div/div[2]/a/div/div[2]";
        public const string WEATHER_SELECT_ELEMENT = "cdanwmansrch_weathercitieselect";
        public const string WEATHER_ELEMENT= "//div[2]/div/div[3]/div/div";
        public const string YNET_LINK_PART = "https://www.ynetnews.com/home/";
        public const string YNET_LINK = "https://www.ynetnews.com/home/0,7340,L-3083,00.html";
        public const string YNET_LINK_ELEMENT ="//h3[contains(.,'ynetnews - Homepage')]";
        public const string CHROME_DRIVER_LOCATION = @"\Tufin\Selenium\Selenium\bin\Debug";

        public const string  GOOGLE_LINK= "http:www.google.com";

        public const string GOOGLE_SEARCH_ELEMENT = "//input[@name='q']";




    }
    public class TufinTest
    {
        //Global Variables
        IWebDriver _driver; 
        int _ErrorCounter = 0;
        //Create or overWrite log file
        System.IO.StreamWriter _file;
        string _logsFolderName;

        /// <summary>
        ///  Automatic test Using SELENIUM that performs the steps bellow:
        /// * Search for “Ynetnews” in google
        /// * Enter the website
        /// * Verify the correct webpage is opened
        /// * Weather: the current weather in the homepage print the weather
        /// * Weather: Change the city to Eilat and print the weather
        /// * Open the page in resolution 1920*1080
        /// * open an article of your choice
        /// * Verify “send to a friend” link exists in the article
        /// * Open "send to a friend" and fill the needed data
        /// * Verify there is a validation on the empty “captcha”
        /// </summary>
        /// <returns></returns>
        public int RunTest()
        {
            var options = new ChromeOptions();
            options.AddArgument("-no-sandbox");
            //Create a driver instance for chromedrive
                                                 //get the location of chromeDrive.exe that located with the .exe
            IWebDriver driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options, TimeSpan.FromMinutes(2));

            //Update the globale variable value with current one
            _driver = driver;

            //elemets of web page
            IWebElement e, e2;
            string str;
            try//global try-for unknown exeption
            {
                //Navigate to google  
                driver.Navigate().GoToUrl(Consts.GOOGLE_LINK); //must start with http
                //Maximize the window
                //driver.Manage().Window.Maximize();

                try
                {
                    //Search for “Ynetnews” in google
                    e = driver.FindElement(By.XPath(Consts.GOOGLE_SEARCH_ELEMENT));
                    e.SendKeys("Ynetnews");
                    e.SendKeys(Keys.Enter);
                }
                catch
                {
                    errorHandler("!!!Failed to find *google search* element");
                }
                try
                {
                    //Enter the website  thru the link
                    driver.FindElement(By.XPath(Consts.YNET_LINK_ELEMENT)).Click();
                }
                catch
                {
                    errorHandler("!!!Failed to find *ynetnews - Homepage* link. WorkAround-going to site directly");               
                    try
                    {
                        driver.Close();
                        driver.Navigate().GoToUrl(Consts.YNET_LINK);
                        _driver = driver;
                    }
                    catch
                    {
                        writeToLog("!!!Failed navigating to the link-" + Consts.YNET_LINK);
                    }
                }

                try
                {
                    //wait for page loading
                    //Verify the correct webpage is opened
                    if (driver.Url.Contains(Consts.YNET_LINK_PART))
                        writeToLog("The ynet news home page succesfully opened");
                    else
                        writeToLog("The ynet news home page failed to open!");
                }
                catch
                {
                    errorHandler("!!!Failed opening- ynet news home page. WorkAround-going to site directly");
                    driver.Navigate().GoToUrl(Consts.YNET_LINK);
                }

                try
                {
                    //Weather: the current weather in the homepage 
                    e = driver.FindElement(By.XPath(Consts.WEATHER_ELEMENT));
                    //e = driver.FindElement(By.Id("cdanwmansrch_weathertemps"));
                    //print the weather 
                    str = e.Text.ToString();
                    string[] strArr = str.Split('\r');
                    writeToLog("The current weather in the homepage is-" + strArr[1].Substring(1));

                    //Weather: Change the city to Eilat
                    e2 = driver.FindElement(By.Id(Consts.WEATHER_SELECT_ELEMENT));
                    SelectElement select = new SelectElement(e2);
                    select.SelectByText("Eilat");
                    
                    //print the weather
                    str = e.Text.ToString();
                    strArr = str.Split('\r');
                    writeToLog("The weather in Eilat is-" + strArr[1].Substring(1));
                }
                catch
                {
                    errorHandler("!!!Failed to print the weather");
                }

                try
                {
                    //Open the page in resolution 1920*1080
                    //if the current screen resolution is smaller, will be changed to current screen resolution
                    System.Drawing.Size windowSize = new System.Drawing.Size(1920, 1080);
                    driver.Manage().Window.Size = windowSize;
                }
                catch
                {
                    errorHandler("!!!Failed to open the page in resolution 1920*1080");
                }

                try
                {
                    //open an article of your choice
                    driver.FindElement(By.XPath(Consts.ARTICLE_LINK_ELEMENT)).Click();
                }
                catch
                {
                    errorHandler("!!!Fail to open choosen artcle");
                    //try another article
                }
                try
                {
                    Wait();
                    //Verify “send to a friend” link exists in the article
                    e = driver.FindElement(By.XPath(Consts.SEND_TO_FRIEND_LINK_ELEMENT));
                    //send to friend
                    if (e.Displayed)
                        writeToLog("-send to a friend- link exists in the article");
                    else
                        writeToLog("-send to a friend- link does NOT exist in the article");

                    e.Click();

                }
                catch
                {
                    //throw new System.Exception("Debugging");
                    errorHandler("!!!Fail to find *send to a friend*");
                }

                try
                {
                   
                    //switch to child window-[1], the father window is-[0]
                    String handlewindow = driver.WindowHandles[1];
                    driver.SwitchTo().Window(handlewindow);

                }
                catch
                {
                    errorHandler("!!!Fail swiching to *send to a friend* window ");
                }

                try
                {
                    //... and fill the needed data
                    driver.FindElement(By.Id("txtTo")).SendKeys(Consts.TO_MAIL_ADDRESS);

                    driver.FindElement(By.Id("txtFromName")).SendKeys(Consts.FROM_NAME);

                    driver.FindElement(By.Id("txtFromAddress")).SendKeys(Consts.FROM_MAIL_ADDRESS);

                    driver.FindElement(By.Id("txtRemarks")).SendKeys(Consts.REMARKS_TEXT);
                }
                catch
                {
                    errorHandler("!!!Failed filling *send to a friend* fields");
                }
                //Verify there is a validation on the empty “captcha”
                // תנאים Link under reCAPTCHA
                if (driver.FindElement(By.XPath(Consts.CAPTCHA_ELEMENT)).Displayed)
                    writeToLog("The reCAPTCHA frame exist");
                else
                    writeToLog("The reCAPTCHA frame NOT found");
            }
            catch//global catch-for unknown exeption
            {
                errorHandler("!!!Test aborted with unknown exception");
                //aborted execution
                return (-1);
            }

          
            testCompleted();
            //normal execution
            return (0);
        }


        /// <summary>
        ///converts current time to "Uniq File Name", string without exception signs
        ///to be added to file names to create uniq file names
        /// </summary>
        /// <returns>string </returns>
        public string currentTimeToFileName()
        {
            string str;
            str = DateTime.Now.ToString();
            str = str + " " + DateTime.Now.Millisecond.ToString();
            str = str.Replace( '/' ,'_' );
            str = str.Replace(':', '-');
            return str;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TufinTest(string fileName = Consts.LOG_FILE_NAME)
        {
            string currentTme = currentTimeToFileName();
            string uniqFolderName = fileName + " " + currentTme;
            _logsFolderName = uniqFolderName;
            string uniqFileName = fileName + ".txt";

            System.IO.Directory.CreateDirectory(uniqFolderName);
            string pathString = System.IO.Path.Combine(uniqFolderName, uniqFileName);
            _file = new System.IO.StreamWriter(pathString, false);
            _file.WriteLine(DateTime.Now.ToString() + "   " + "Tufin Test Started!");
        }
   

        /// <summary>
        /// Set of operations that always executed when one of steps fails
        /// 1-Incresing error counter
        /// 2-Writing error to log file
        /// 3-Saving screen print.
        /// </summary>
        /// <param name="errorMessage"> error message that should be writen to thelog file</param>
        /// <returns> 1 on successful execution.</returns>
        int errorHandler(string errorMessage)
        {
            _ErrorCounter++;
            _file.WriteLine(DateTime.Now.ToString() + "   " + errorMessage);
            _file.Flush();

            printScreen(errorMessage);
            return (1);
        }

        /// <summary>
        /// Adds a message to log file
        /// </summary>
        /// <param name="messageToLogFile"></param>
        /// <returns></returns>
        int writeToLog(string messageToLogFile)
        {
            _file.WriteLine(DateTime.Now.ToString() + "   " + messageToLogFile);
            _file.Flush();

            return (1);
        }

        /// <summary>
        /// Waiting until page and element loading, for overloaded internet
        /// </summary>
        /// <param name="Sec"></param>
        void Wait(int Sec = Consts.WAIT_SECONDS)
        {
            //in mili seconds
            System.Threading.Thread.Sleep(Sec * 1000);
        }
        /// <summary>
        /// Creates a picture of current screen
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string printScreen(string fileName )
        {
            Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();
            string screenshot = ss.AsBase64EncodedString;
            byte[] screenshotAsByteArray = ss.AsByteArray;
            string uniqFileName = fileName + currentTimeToFileName() + ".png";
            string pathString = System.IO.Path.Combine(_logsFolderName, uniqFileName);
            ss.SaveAsFile(pathString); //use any of the built in image formating 
            return (uniqFileName);
        }

       
      
        /// <summary>
        /// Bonus test:
        ///Find the “updates” section in the main page חלונית מבזקים -
        ///Verify the “updates” article list is moving
        ///Verify the “updates” article list is not moving on mouse hover
        /// </summary>
        public void bonus()
        {
            //selenium.exe file location
            var options = new ChromeOptions();
            options.AddArgument("-no-sandbox");
            //Create a driver instance for chromedrive
            IWebDriver driver = new ChromeDriver(Consts.CHROME_DRIVER_LOCATION, options, TimeSpan.FromMinutes(2));
            IWebElement e;
            //Update the globale variable value with current one
            _driver = driver;

            //----------Bonus--------------------------------------------
            //Find the “updates” section in the main page חלונית מבזקים -
            try
            {
                driver.Navigate().GoToUrl(Consts.YNET_LINK);
                System.Drawing.Size windowSize = new System.Drawing.Size(1920, 1080);
                driver.Manage().Window.Size = windowSize;
                //Maximize the window
                driver.Manage().Window.Maximize();

                Wait();
                //Verify the “updates” article list is moving
                e = driver.FindElement(By.ClassName(Consts.UPDATE_IFRAME_ELEMENT));

                //move to elemet location
                Actions action = new Actions(driver);
                action.MoveToElement(e).Perform();
                //move to text location
                action.MoveByOffset(5, 5).Perform();
                Wait(1);
                string firstFileName, secondFileName;
                firstFileName = printScreen("first");
                Wait(3);
                secondFileName=printScreen("second");
                Wait(1);
                //compare the pixel in two pictures- if the stoped running it should be equal
                if (IsPixelEqual(firstFileName, secondFileName, e.Location.X + 5, e.Location.Y + 5))
                    writeToLog("article list is NOT moving on mouse hover");
                else
                    writeToLog("article list is moving on mouse hover");
            }
            catch
            {
                errorHandler("!!!Bonus test failed");
            }

         
        }

        /// <summary>
        /// compares a specific pixel of two PNG pictures
        /// </summary>
        /// <param name="firstFileName"></param>
        /// <param name="secondFileName"></param>
        /// <param name="xPosition"></param>
        /// <param name="yPosition"></param>
        /// <returns></returns>
        public bool IsPixelEqual(string firstFileName,string secondFileName, int xPosition, int yPosition)
        {
            // Create a Bitmap object from an image file.
            Bitmap myBitmap = new Bitmap(firstFileName);
            // Get the color of a pixel within myBitmap.
            Color pixelColor = myBitmap.GetPixel(xPosition, yPosition);

            // Create a Bitmap object from an image file.
            Bitmap myBitmap2 = new Bitmap(secondFileName);
            // Get the color of a pixel within myBitmap.
            Color pixelColor2 = myBitmap.GetPixel(xPosition, yPosition);
            if (Color.Equals(pixelColor, pixelColor2))
                return (true);
            else
                return(false);
        }
        /// <summary>
        /// Summarize tests results
        /// </summary>
        void testCompleted()
        {
            writeToLog("Test finished with-" + _ErrorCounter + "  errors");
            _driver.Close();
            //Playing test finished sound
            Console.Beep(); Console.Beep();
    }

}


 

    class Program
    {
        //for parallel execution
        public static void ParalelExecution()
        {
            TufinTest test1= new TufinTest();
            test1.RunTest();
        }
        
        
        static void Main(string[] args)
        {
            
            TufinTest test1;
            
            //single execution
            test1 = new TufinTest();
            test1.RunTest();
            test1.bonus();
            


            /*
            //Sequenced execution
            for (int i = 0; i < 10; i++)
            {
               test1 = new TufinTest(i.ToString());
               test1.RunTest();
            }
            */
         

            //parallel execution
            /*
            for (int i = 0; i < 5; i++)
            {
                Thread thread = new Thread(ParalelExecution);
                thread.Start();
                System.Threading.Thread.Sleep(10000);
            }
            */
            
            



        }

    }

}


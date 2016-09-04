using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
//using Microsoft.ServiceBus.Messaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Slot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public int FirstRandInt;
        public int SecondRandInt;
        public int ThirdRandInt;
        
        private ThreadPoolTimer TPtimer;
        private const int spinTime = 100;

        public int CurrentTPCount;
        public int TPCountMax = 25;

        public string RandIMGString;


        public string AzureColor1;
        public string AzureColor2;
        public string AzureColor3;

        public string AzureResult1;
        public string AzureResult2;
        public string AzureResult3;

        public string ColorWinResult;
        public string IconWinResult;



        public double myMoney;
        public double myCurrentBet;

        public System.Collections.Generic.IReadOnlyList<Windows.Storage.StorageFile> AzureIconFiles;

        public List<AzureColor> AzureColorList;


        //Azure Iot Settings
        public string AzureMode = "Transmit";
        public string internetConnected = "True";

        private const string DeviceConnectionString = "Device Connection String Here";
        private const string iotHubUri = "<replace>"; // ! put in value !
        private const string deviceId = "<replace>"; // ! put in value !
        private const string deviceKey = "<replace>"; // ! put in value

        
        //Game Settings

        public double StartingCredits = 5;

        public double DoubleColorPayOut = 2;
        public double TripleColorPayOut = 5;

        public double DoubleServicePayOut = 10;
        public double TripleServicePayOut = 15;






        public MainPage()
        {
            this.InitializeComponent();
            
        }



        class MySlotData
        {
            public String Name { get; set; }
            public String SensorType { get; set; }
            public String TimeStamp { get; set; }
            public String DataValue { get; set; }
            public String UnitOfMeasure { get; set; }
            public String Location { get; set; }
            public String DataType { get; set; }
            public String MeasurementID { get; set; }
        }





        public class AzureColor
        {
            // Auto-implemented properties.

            public string ColorName { get; set; }
            public SolidColorBrush ColorBrush { get; set; }
            

        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SetupImages();
            myMoney = StartingCredits;

            UpdateCreditsTextBlock(myMoney.ToString());


        }


        private void button_Play_Click(object sender, RoutedEventArgs e)
        {

            // GamePlay Logic
            myMoney = myMoney - 1;
            UpdateCreditsTextBlock(myMoney.ToString());
            Debug.WriteLine("Button Pressed");


       
            ClearWinLoseTextBlock();
            
            TxtDoubleServiceWin.Opacity = 0;
            TxtTripleServiceWin.Opacity = 0;
            TxtLose.Opacity = 0;

            
            button_Play.IsEnabled = false;
            button_Bet.IsEnabled = false;
            CurrentTPCount = 0;

            //Run Win/Lose Logic


            //RUn Probabllitliy Engine


            //Give Percetnages of a Win...


            //if (customer_able_to_win())
            //{
            //calculate_how_to_win();
            //}
            //else
            //{
                //no_win();
            //}


            //Roll must be 1 to Win
            //Increase Range of Random Numbers to determine probability
            //ex max2 = 50/50, max 10 = 10%. max 100 = 1%


            //Roll for Major Win   -- Win CHance x .025
              //Pick a Random Image and set final to each roller to this...
                //Do Anims
            
            // IF NOT MAJOR WIN

                //Roll for Minor Win   -- Win Change x 1
                //Pick a random image and set 2 of the rollers to this (randomly)
                //Do Anims
            
            // IF NO MAJOR OR MINOR WIN

                //Roll random until no 3 rollers match
                //assign
                //do anims.


            //Visual of Pull
            TPtimer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(spinTime));

            var PlayButtonSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                PlayWavSound("SlotPull.wav");
            });

           // SetupImagesOld();

            Debug.WriteLine("Button Press Complete");



        }


        private void Timer_Tick(ThreadPoolTimer timer)
        {

            UpdateDisplay();

        }


        public async Task SetupImages()
        {
            //Load All Images and assign to IReadOnlyList<Windows.Storage.StorageFile... our player function will then later random pick from array and then 


            //Load Image Folders
            StorageFolder appInstalledFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets\\AzureIconsColor");
            AzureIconFiles = await assets.GetFilesAsync();

            //Setup Colors
            AzureColorList = new List<AzureColor>
            {

                new AzureColor(){ ColorName = "Green", ColorBrush = new SolidColorBrush(Colors.Lime) },
                new AzureColor(){ ColorName = "Red", ColorBrush = new SolidColorBrush(Colors.Red) },
                new AzureColor(){ ColorName = "Blue", ColorBrush = new SolidColorBrush(Colors.Blue) },
                new AzureColor(){ ColorName = "Yellow", ColorBrush = new SolidColorBrush(Colors.Yellow) },
                new AzureColor(){ ColorName = "Violet", ColorBrush = new SolidColorBrush(Colors.Magenta) },
                new AzureColor(){ ColorName = "Orange", ColorBrush = new SolidColorBrush(Colors.Orange) },
                new AzureColor(){ ColorName = "Brown", ColorBrush = new SolidColorBrush(Colors.RosyBrown) }
            };



        }





        public async Task SetupImagesOld()
        {



          

            Random Random1 = new Random();
            Random Random2 = new Random();
            Random Random3 = new Random();
            

            /*
            Random RandomColor1 = new Random();
            Random RandomColor2 = new Random();
            Random RandomColor3 = new Random();


            int colindex1 = Random1.Next(0, AzureColorList.Count - 1);
            int colindex2 = Random1.Next(0, AzureColorList.Count - 1);
            int colindex3 = Random1.Next(0, AzureColorList.Count - 1);


            SolidColorBrush RandColorCode1 = (AzureColorList[colindex1].ColorBrush);
            SolidColorBrush RandColorCode2 = (AzureColorList[colindex2].ColorBrush);
            SolidColorBrush RandColorCode3 = (AzureColorList[colindex3].ColorBrush);

            AzureColor1 = (AzureColorList[colindex1].ColorName);
            AzureColor2 = (AzureColorList[colindex2].ColorName);
            AzureColor3 = (AzureColorList[colindex3].ColorName);
            */


            //   AzureBack1.Fill = RandColorCode1;
            //    AzureBack2.Fill = RandColorCode2;
            //  AzureBack3.Fill = RandColorCode3;
            

            try
            {
                int index = Random1.Next(0, AzureIconFiles.Count - 1);
                RandIMGString = (AzureIconFiles[index].Path.ToString());
                AzureResult1 = (AzureIconFiles[index].Name.ToString());
                Uri uri = new Uri(RandIMGString, UriKind.Absolute);
                ImageSource imgSource = new BitmapImage(uri);
                FirstImage.Source = imgSource;



                int index2 = Random1.Next(0, AzureIconFiles.Count - 1);
                RandIMGString = (AzureIconFiles[index2].Path.ToString());
                AzureResult2 = (AzureIconFiles[index2].Name.ToString());
                Uri uri2 = new Uri(RandIMGString, UriKind.Absolute);
                ImageSource imgSource2 = new BitmapImage(uri2);
                SecondImage.Source = imgSource2;


                int index3 = Random1.Next(0, AzureIconFiles.Count - 1);
                RandIMGString = (AzureIconFiles[index3].Path.ToString());
                AzureResult3 = (AzureIconFiles[index3].Name.ToString());
                Uri uri3 = new Uri(RandIMGString, UriKind.Absolute);
                ImageSource imgSource3 = new BitmapImage(uri3);
                ThirdImage.Source = imgSource3;

                //Set Results to Public Strings
                AzureResult1 = AzureResult1.Replace(".png", "");
                AzureResult2 = AzureResult2.Replace(".png", "");
                AzureResult3 = AzureResult3.Replace(".png", "");

                TxtBlockFirst.Text = AzureResult1;
                TxtBlockSecond.Text = AzureResult2;
                TxtBlockThird.Text = AzureResult3;
            }

            catch

            {

            }
                     
            


    




        }

        private async Task UpdateDisplay()
        {

            CurrentTPCount = CurrentTPCount + 1;

            if (CurrentTPCount <= TPCountMax)
            {

                var task = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                {
                    SetupImagesOld();

                });

            }

            else
            {
                //DONE!
                Debug.WriteLine("Random Done");
                TPtimer.Cancel();
                PlayCompleted();



            }

            

        }





        private async void PlayWavSound(string soundeffect)
        {
            MediaElement mysong = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync("Sounds\\" + soundeffect);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mysong.SetSource(stream, file.ContentType);
            mysong.Play();
        }


        private async void UpdateWinLoseTextBlock(string text)
        {
            var WinResultTextBlock = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                string currenttext;
                currenttext = TxtBLockWinLostResult.Text;
                TxtBLockWinLostResult.Text = currenttext + " " + text;

            });
        }




        private async void UpdateCreditsTextBlock(string text)
        {
            var WinResultTextBlock = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                TxtCredits.Text = text;

            });
        }




        private async void ClearWinLoseTextBlock()
        {
            var WinResultTextBlock = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {

                TxtBLockWinLostResult.Text = "";

            });
        }




        private async void ShowWinResultItem(string text)
        {

            var FailSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
             
                if (text == "DoubleService")
                {
                    TxtDoubleServiceWin.Opacity = 1;
                }
                if (text == "TripleService")
                {
                    TxtTripleServiceWin.Opacity = 1;
                }
                if (text == "Lose")
                {
                    TxtLose.Opacity = 1;
                }

            });



      
          
        }






        public async Task UpdateWinLossDisplay()
        {
            TxtBLockWinLostResult.Text = "YOU LOST!";
        }


        public async void PlayCompleted()
        {

            ColorWinResult = "";
            IconWinResult = "";

            Debug.WriteLine("Results ARE....");


   



            // UpdateWinLossDisplay();



            //      public string ColorWinResult;
            //   public string IconWinResult;






            //Debug.WriteLine(AzureColor1 + " " + AzureResult1 + " -- " + AzureColor2 + " " + AzureResult2 + " -- " + AzureColor3 + " " + AzureResult3);
            Debug.WriteLine(AzureResult1 + " -- " + AzureResult2 + " -- " + AzureResult3);






            //Result Review
            /*

            //Color Win Check

            //Triple Color
            if (AzureColor1 == AzureColor2 & AzureColor1 == AzureColor3)
            {
                Debug.WriteLine("Triple Color Win!");
                ColorWinResult = "TripleColorWin";
                UpdateWinLoseTextBlock("Triple Color Win!!!");
                myMoney = (myMoney + TripleColorPayOut);
                ShowWinResultItem("TripleColor");
            }

            if (ColorWinResult != "TripleColorWin")
            {
                //Double Color
                if (AzureColor1 == AzureColor2 | AzureColor1 == AzureColor3 | AzureColor2 == AzureColor3)
                {
                    Debug.WriteLine("Double Color Win!");
                    ColorWinResult = "DoubleColorWin";
                    UpdateWinLoseTextBlock("Double Color Win!!!");
                    myMoney = (myMoney + DoubleColorPayOut);
                    ShowWinResultItem("DoubleColor");
                }
            }
            */

            // Icon Win Check

            //Triple Service Win!
            if (AzureResult1 == AzureResult2 & AzureResult1 == AzureResult3)
            {

                Debug.WriteLine("           !             ");
                Debug.WriteLine("          !!!            ");
                Debug.WriteLine("     !!!!!!!!!!!!!!      ");
                Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
                Debug.WriteLine("Triple Azure Service Win!");
                Debug.WriteLine("Triple Azure Service Win!");
                Debug.WriteLine("Triple Azure Service Win!");
                Debug.WriteLine("Triple Azure Service Win!");
                Debug.WriteLine("Triple Azure Service Win!");
                Debug.WriteLine("Triple Azure Service Win!");
                Debug.WriteLine("     !!!!!!!!!!!!!!      ");
                Debug.WriteLine("          !!!            ");
                Debug.WriteLine("           !             ");



                IconWinResult = "TripleServiceWin";
                UpdateWinLoseTextBlock("Triple Azure Service Win!!!");
                myMoney = (myMoney + TripleServicePayOut);
                ShowWinResultItem("TripleService");

            }

            if (IconWinResult != "TripleServiceWin")
            {
                //Double Color
                if (AzureResult1 == AzureResult2 | AzureResult1 == AzureResult3 | AzureResult2 == AzureResult3)
                {
                    Debug.WriteLine("Double Azure Service Win!");
                    IconWinResult = "Double Azure Service Win";

                    UpdateWinLoseTextBlock("Double Azure Service  Win!");

                    myMoney = (myMoney + DoubleServicePayOut);
                    ShowWinResultItem("DoubleService");


                }
            }



            if (ColorWinResult == "" & IconWinResult == "")
            {
                Debug.WriteLine("You Lose!");
                // PlayWavSound("LosingHorn.wav");
                UpdateWinLoseTextBlock("You Lose!!!");

                var FailSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    PlayWavSound("LosingHorn.wav");
                    ShowWinResultItem("Lose");
                });

             


            }
            else
            {

                var DoublColorWinSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    PlayWavSound("Win95.wav");
                });


            }





            ColorWinResult = "";
            IconWinResult = "";



            UpdateCreditsTextBlock(myMoney.ToString());




            var task = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                if(myMoney > 0)
                {
                    button_Play.IsEnabled = true;
                    button_Bet.IsEnabled = true;

                 

                }
                

            });




            
            //Transmit Result to Azure




    }


   private async Task Log_Event(string DataValue, string Name, string Sensor, string DataType, string UnitOfMeasure)
   {
            Debug.WriteLine("Log!");

            //Debug
            DateTime localDate = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Event: " + Name + "-" + Sensor + "-" + DataValue + "-" + localDate.ToString());

            //Flash LED 
       //     TrasmitLEDPin.Write(GpioPinValue.High);
      //      await Task.Delay(100);
        //    TrasmitLEDPin.Write(GpioPinValue.Low);

            //to Azure
            if (AzureMode == "Transmit")
                if (internetConnected == "True")
                {
                    {

                        //Init httpClinet:
                        //var httpClient = new HttpClient();

                        System.Diagnostics.Debug.WriteLine("Starting Azure Transmit");


                        MySlotData SensorInstance = new MySlotData();
                        SensorInstance.Name = Name;
                        SensorInstance.SensorType = Sensor;
                        SensorInstance.TimeStamp = DateTime.Now.ToString();
                        SensorInstance.DataValue = DataValue;
                        SensorInstance.DataType = DataType;
                        SensorInstance.UnitOfMeasure = UnitOfMeasure;
                        SensorInstance.MeasurementID = Guid.NewGuid().ToString();
                        SensorInstance.Location = "Las Vegas, Nevada";

                        string jsoncontent = JsonConvert.SerializeObject(SensorInstance);




                        DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
                        

                        string dataBuffer;
                        dataBuffer = jsoncontent;

                        System.Diagnostics.Debug.WriteLine(jsoncontent);


                        Message eventMessage = new Message(System.Text.Encoding.UTF8.GetBytes(dataBuffer));
                        await deviceClient.SendEventAsync(eventMessage);


                        System.Diagnostics.Debug.WriteLine("Azure Transmit Done");



                    }
                }


        }










    }
}
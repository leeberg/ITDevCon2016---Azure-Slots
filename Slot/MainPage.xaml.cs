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

        public string CurrentGameID;


        public double currentCredits;
        public double myCurrentBet;

        public System.Collections.Generic.IReadOnlyList<Windows.Storage.StorageFile> AzureIconFiles;
        public System.Collections.Generic.IReadOnlyList<Windows.Storage.StorageFile> SoundFiles;

        //Azure Iot Settings
        public string AzureMode = "Transmit";
        public string internetConnected = "True";

        private const string DeviceConnectionString = "HostName=BergHub1.azure-devices.net;DeviceId=BergDevice2;SharedAccessKey=FkcFl9IwAZEqpPZLyOVeQDUOvacXBYhTh17GwdYqfTQ=";
        
        private const string iotHubUri = "<replace>"; // ! put in value !
        private const string deviceId = "<replace>"; // ! put in value !
        private const string deviceKey = "<replace>"; // ! put in value

        
        //Game Settings

        public double StartingCredits = 5;
        public double currentBet = 1;

        public double DoubleColorPayOut = 2;
        public double TripleColorPayOut = 5;

        public double DoubleServicePayOut = 5;
        public double TripleServicePayOut = 15;


        ///    public MediaElement snd_Chord;
        public MediaElement snd_Chord = new MediaElement();
        public MediaElement snd_HaHa = new MediaElement();
        public MediaElement snd_LosingHorn = new MediaElement();
        public MediaElement snd_SlotPull = new MediaElement();
        public MediaElement snd_Win95 = new MediaElement();
        public MediaElement snd_MarioFail = new MediaElement();
        public MediaElement snd_SadTrom = new MediaElement();

        public Windows.Storage.StorageFolder soundFolder;
        
        public Windows.Storage.StorageFile soundFile_Chord;
        public Windows.Storage.StorageFile soundFile_HaHa;
        public Windows.Storage.StorageFile soundFile_LosingHorn;
        public Windows.Storage.StorageFile soundFile_SlotPull;
        public Windows.Storage.StorageFile soundFile_Win95;
        public Windows.Storage.StorageFile soundFile_MarioFail;
        public Windows.Storage.StorageFile soundFile_SadTrom;

        public ImageSource ActiveDirectory = new BitmapImage();
        public ImageSource Automation = new BitmapImage();
        public ImageSource DataWarehouse = new BitmapImage();
        public ImageSource DevTestLabs = new BitmapImage();
        public ImageSource EventHubs = new BitmapImage();
        public ImageSource IoTHub = new BitmapImage();
        public ImageSource MachineLearning = new BitmapImage();
        public ImageSource MediaService = new BitmapImage();
        public ImageSource NotificationHubs = new BitmapImage();
        public ImageSource OMS = new BitmapImage();
        public ImageSource SQLDatabase = new BitmapImage();
        public ImageSource StreamAnalytics = new BitmapImage();
        public ImageSource VirtualMachine = new BitmapImage();



        public MainPage()
        {
            this.InitializeComponent();
            
        }



        class MySlotData
        {
            public String Name { get; set; }
            public String SensorType { get; set; }
            public String TimeStamp { get; set; }
            public double DataValue { get; set; }
            public String UnitOfMeasure { get; set; }
            public String Location { get; set; }
            public String DataType { get; set; }
            public String MeasurementID { get; set; }
        }



        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {

            await SetupImages();
            await SetupSounds();
            ResetGame();


        }

        private void ResetGame()
        {

            FirstImage.Source = null;
            SecondImage.Source = null;
            ThirdImage.Source = null;

            currentCredits = StartingCredits;
            currentBet = 1;

            CurrentGameID = Guid.NewGuid().ToString();

            UpdateCreditsTextBlock(currentCredits.ToString());
            UpdateCurrentBetTextBlock(currentBet.ToString());

            button_Play.IsEnabled = true;
            button_Bet.IsEnabled = true;
            button_End.IsEnabled = true; 

            TxtBet.Opacity = 100;
            TxtCredits.Opacity = 100;

            var task = Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {

                TxtBlockFirst.Text = "";
                TxtBlockSecond.Text = "";
                TxtBlockThird.Text = "";
                TxtRollResult.Text = "Spin to Start!";

            });
                       

            var taskPrepStart = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {

                {
                 

                    var dialog = new ContentDialog()
                    {
                        Title = "Game Ready!",
                        MaxWidth = this.ActualWidth, // Required for Mobile!
                        Content = "You have 5 Credits, Click OK to Start!"  //YourXamlContent

                    };

                    dialog.PrimaryButtonText = "OK";
                    dialog.IsPrimaryButtonEnabled = true;
                    dialog.PrimaryButtonClick += delegate { Log_Event(currentCredits, "StartGame", "GameEvent", "UserAction", CurrentGameID); };

                    var result = await dialog.ShowAsync();

                   

                }


            });



        }


        private void button_Bet_Click(object sender, RoutedEventArgs e)
        {
            if (currentBet < currentCredits)
            {
                currentBet = currentBet + 1;
                UpdateCurrentBetTextBlock(currentBet.ToString());
                snd_Chord.Play();
            }
            else
            {

            }


            if (currentBet == currentCredits)
            {
                button_Bet.IsEnabled = false;
            }
        }


        private void button_Play_Click(object sender, RoutedEventArgs e)
        {

            // GamePlay Logic
            currentCredits = currentCredits - currentBet;
            UpdateCreditsTextBlock(currentCredits.ToString());
            Debug.WriteLine("Button Pressed");


       
            ClearWinLoseTextBlock();
            ClearBetTextBlock();

            button_Play.IsEnabled = false;
            button_Bet.IsEnabled = false;
            button_End.IsEnabled = false;
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

            var PlayButtonSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //PlayWavSound("SlotPull.wav");
                snd_SlotPull.Play();
            });

           // SetupImagesOld();

            Debug.WriteLine("Button Press Complete");



        }




        private void button_End_Click(object sender, RoutedEventArgs e)
        {
            var task = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
             
                {
                    Log_Event(currentCredits, "UserStop", "SpinEvent", "UserAction", CurrentGameID);

                    button_Bet.IsEnabled = false;
                    button_Play.IsEnabled = false;
                    button_End.IsEnabled = false;
                    UpdateWinLoseTextBlock("END GAME!");
                    TxtBet.Opacity = 0;

                    var dialog = new ContentDialog()
                    {
                        Title = "You Stopped!",
                        MaxWidth = this.ActualWidth, // Required for Mobile!
                        Content = "You have gone home with: " + currentCredits.ToString() + " Credits!"  //YourXamlContent

                    };

                    dialog.PrimaryButtonText = "OK";
                    dialog.IsPrimaryButtonEnabled = true;
                    dialog.PrimaryButtonClick += delegate { ResetGame(); };

                    var result = await dialog.ShowAsync();

                }


            });
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


            Debug.WriteLine("DERP!!!");

            Uri ActiveDirectoryURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\ActiveDirectory.png");
            Uri AutomationURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\Automation.png");
            Uri DataWarehouseURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\DataWarehouse.png");
            Uri DevTestLabsURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\DevTestLabs.png");
            Uri EventHubsURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\EventHubs.png");
            Uri IoTHubURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\IoTHub.png");
            Uri MachineLearningURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\MachineLearning.png");
            Uri MediaServiceURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\MediaService.png");
            Uri NotificationHubsURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\NotificationHubs.png");
            Uri OMSURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\OMS.png");
            Uri SQLDatabaseURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\SQLDatabase.png");
            Uri StreamAnalyticsURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\StreamAnalytics.png");
            Uri VirtualMachineURI = new Uri(appInstalledFolder.Path.ToString() + "\\Assets\\AzureIconsColor\\VirtualMachine.png");




            Debug.WriteLine(ActiveDirectoryURI.ToString());


            ActiveDirectory = new BitmapImage(ActiveDirectoryURI);
            Automation = new BitmapImage(AutomationURI);
            DataWarehouse = new BitmapImage(DataWarehouseURI);
            DevTestLabs = new BitmapImage(DevTestLabsURI);
            EventHubs = new BitmapImage(EventHubsURI);
            IoTHub = new BitmapImage(IoTHubURI);
            MachineLearning = new BitmapImage(MachineLearningURI);
            MediaService = new BitmapImage(MediaServiceURI);
            NotificationHubs = new BitmapImage(NotificationHubsURI);
            OMS = new BitmapImage(OMSURI);
            SQLDatabase = new BitmapImage(SQLDatabaseURI);
            StreamAnalytics = new BitmapImage(StreamAnalyticsURI);
            VirtualMachine = new BitmapImage(VirtualMachineURI);

               //all files established.




        }



    public async Task SetupSounds()
        {
            //Load All Sounds

            //probably could do this programatically but lol

            Debug.WriteLine("Setup Sounds Started");

            try
            {

                snd_Chord.AutoPlay = false;
                snd_HaHa.AutoPlay = false;
                snd_LosingHorn.AutoPlay = false;
                snd_SlotPull.AutoPlay = false;
                snd_Win95.AutoPlay = false;
                snd_MarioFail.AutoPlay = false;
                snd_SadTrom.AutoPlay = false;

                soundFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");



                soundFile_Chord = await soundFolder.GetFileAsync("Sounds\\chord.wav");
                var soundStream_Chord = await soundFile_Chord.OpenAsync(Windows.Storage.FileAccessMode.Read);
                snd_Chord.SetSource(soundStream_Chord, soundFile_Chord.ContentType);
                snd_Chord.Stop();
                Debug.WriteLine("chord Loaded");


                soundFile_HaHa = await soundFolder.GetFileAsync("Sounds\\haha.wav");
                var soundStream_HaHa = await soundFile_HaHa.OpenAsync(Windows.Storage.FileAccessMode.Read);
                snd_HaHa.SetSource(soundStream_HaHa, soundFile_HaHa.ContentType);
                snd_HaHa.Stop();
                Debug.WriteLine("HAha Loaded");

                soundFile_LosingHorn = await soundFolder.GetFileAsync("Sounds\\LosingHorn.wav");
                var soundStream_LosingHorn = await soundFile_LosingHorn.OpenAsync(Windows.Storage.FileAccessMode.Read);
                snd_LosingHorn.SetSource(soundStream_LosingHorn, soundFile_LosingHorn.ContentType);
                snd_LosingHorn.Stop();
                Debug.WriteLine("Losing Horn Loaded");


                soundFile_SlotPull = await soundFolder.GetFileAsync("Sounds\\SlotPull.wav");
                var soundStream_SlotPull = await soundFile_SlotPull.OpenAsync(Windows.Storage.FileAccessMode.Read);
                snd_SlotPull.SetSource(soundStream_SlotPull, soundFile_SlotPull.ContentType);
                snd_SlotPull.Stop();
                Debug.WriteLine("SLot Pull Loaded");


                soundFile_Win95 = await soundFolder.GetFileAsync("Sounds\\Win95.wav");
                var soundStream_Win95 = await soundFile_Win95.OpenAsync(Windows.Storage.FileAccessMode.Read);
                snd_Win95.SetSource(soundStream_Win95, soundFile_Win95.ContentType);
                snd_Win95.Stop();
                Debug.WriteLine("WIn 95 Loaded");


                soundFile_MarioFail = await soundFolder.GetFileAsync("Sounds\\mariofail.wav");
                var soundStream_MarioFail = await soundFile_MarioFail.OpenAsync(Windows.Storage.FileAccessMode.Read);
                snd_MarioFail.SetSource(soundStream_MarioFail, soundFile_MarioFail.ContentType);
                snd_MarioFail.Stop();
                Debug.WriteLine("Mario Fail Loaded");


                soundFile_SadTrom = await soundFolder.GetFileAsync("Sounds\\SadTrom.wav");
                var soundStream_SadTrom = await soundFile_SadTrom.OpenAsync(Windows.Storage.FileAccessMode.Read);
                snd_SadTrom.SetSource(soundStream_SadTrom, soundFile_SadTrom.ContentType);
                snd_SadTrom.Stop();
                Debug.WriteLine("Sad Trombone Loaded");


                Debug.WriteLine("Setup Sounds Completed");

            }

            catch (Exception e)
            {
                Debug.WriteLine("{0} Exception caught." + e.ToString()+ " ---");
            }


        }






        public async Task SetupImagesOld()
        {      

            Random Random1 = new Random();
            Random Random2 = new Random();
            Random Random3 = new Random();
     
            try
            {

                //foreach (Windows.Storage.StorageFile file in AzureIconFiles)

                int index = Random1.Next(0, AzureIconFiles.Count - 1);
                RandIMGString = (AzureIconFiles[index].Path.ToString());
                AzureResult1 = (AzureIconFiles[index].Name.ToString());
                                
                int index2 = Random1.Next(0, AzureIconFiles.Count - 1);
                RandIMGString = (AzureIconFiles[index2].Path.ToString());
                AzureResult2 = (AzureIconFiles[index2].Name.ToString());

                int index3 = Random1.Next(0, AzureIconFiles.Count - 1);
                RandIMGString = (AzureIconFiles[index3].Path.ToString());
                AzureResult3 = (AzureIconFiles[index3].Name.ToString());

                //Set Results to Public Strings
                AzureResult1 = AzureResult1.Replace(".png", "");
                AzureResult2 = AzureResult2.Replace(".png", "");
                AzureResult3 = AzureResult3.Replace(".png", "");
                AzureResult1 = AzureResult1.Replace(".jpg", "");
                AzureResult2 = AzureResult2.Replace(".jpg", "");
                AzureResult3 = AzureResult3.Replace(".jpg", "");

                TxtBlockFirst.Text = AzureResult1;
                TxtBlockSecond.Text = AzureResult2;
                TxtBlockThird.Text = AzureResult3;




                switch (AzureResult1)
                {
                    case "ActiveDirectory":
                        FirstImage.Source = ActiveDirectory;
                        break;
                    case "Automation":
                        FirstImage.Source = Automation;
                        break;
                    case "DataWarehouse":
                        FirstImage.Source = DataWarehouse;
                        break;
                    case "DevTestLabs":
                        FirstImage.Source = DevTestLabs;
                        break;
                    case "EventHubs":
                        FirstImage.Source = EventHubs;
                        break;
                    case "IoTHub":
                        FirstImage.Source = IoTHub;
                        break;
                    case "MediaService":
                        FirstImage.Source = MediaService;
                        break;
                    case "NotificationHubs":
                        FirstImage.Source = NotificationHubs;
                        break;
                    case "OMS":
                        FirstImage.Source = OMS;
                        break;
                    case "SQLDatabase":
                        FirstImage.Source = SQLDatabase;
                        break;
                    case "StreamAnalytics":
                        FirstImage.Source = StreamAnalytics;
                        break;
                    case "VirtualMachine":
                        FirstImage.Source = VirtualMachine;
                        break;
                    case "MachineLearning":
                        FirstImage.Source = MachineLearning;
                        break;

                }

                switch (AzureResult2)
                {
                    case "ActiveDirectory":
                        SecondImage.Source = ActiveDirectory;
                        break;
                    case "Automation":
                        SecondImage.Source = Automation;
                        break;
                    case "DataWarehouse":
                        SecondImage.Source = DataWarehouse;
                        break;
                    case "DevTestLabs":
                        SecondImage.Source = DevTestLabs;
                        break;
                    case "EventHubs":
                        SecondImage.Source = EventHubs;
                        break;
                    case "IoTHub":
                        SecondImage.Source = IoTHub;
                        break;
                    case "MediaService":
                        SecondImage.Source = MediaService;
                        break;
                    case "NotificationHubs":
                        SecondImage.Source = NotificationHubs;
                        break;
                    case "OMS":
                        SecondImage.Source = OMS;
                        break;
                    case "SQLDatabase":
                        SecondImage.Source = SQLDatabase;
                        break;
                    case "StreamAnalytics":
                        SecondImage.Source = StreamAnalytics;
                        break;
                    case "VirtualMachine":
                        SecondImage.Source = VirtualMachine;
                        break;
                    case "MachineLearning":
                        FirstImage.Source = MachineLearning;
                        break;

                }

                switch (AzureResult3)
                {
                    case "ActiveDirectory":
                        ThirdImage.Source = ActiveDirectory;
                        break;
                    case "Automation":
                        ThirdImage.Source = Automation;
                        break;
                    case "DataWarehouse":
                        ThirdImage.Source = DataWarehouse;
                        break;
                    case "DevTestLabs":
                        ThirdImage.Source = DevTestLabs;
                        break;
                    case "EventHubs":
                        ThirdImage.Source = EventHubs;
                        break;
                    case "IoTHub":
                        ThirdImage.Source = IoTHub;
                        break;
                    case "MediaService":
                        ThirdImage.Source = MediaService;
                        break;
                    case "NotificationHubs":
                        ThirdImage.Source = NotificationHubs;
                        break;
                    case "OMS":
                        ThirdImage.Source = OMS;
                        break;
                    case "SQLDatabase":
                        ThirdImage.Source = SQLDatabase;
                        break;
                    case "StreamAnalytics":
                        ThirdImage.Source = StreamAnalytics;
                        break;
                    case "VirtualMachine":
                        ThirdImage.Source = VirtualMachine;
                        break;
                    case "MachineLearning":
                        FirstImage.Source = MachineLearning;
                        break;

                }





                //Uri uri = new Uri(RandIMGString, UriKind.Absolute);
                //ImageSource imgSource = new BitmapImage(uri);
                //FirstImage.Source = imgSource;


                //Uri uri2 = new Uri(RandIMGString, UriKind.Absolute);
                //ImageSource imgSource2 = new BitmapImage(uri2);
                //SecondImage.Source = imgSource2;

                //Uri uri3 = new Uri(RandIMGString, UriKind.Absolute);
                //ImageSource imgSource3 = new BitmapImage(uri3);
//                ThirdImage.Source = imgSource3;




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
                    await SetupImagesOld();

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
            //mysong.Play();
        }


        private async void UpdateWinLoseTextBlock(string text)
        {
            var WinResultTextBlock = Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
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


        private async void UpdateCurrentBetTextBlock(string text)
        {
            var WinResultTextBlock = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                TxtBet.Text = text;

            });
        }


        private async void ClearBetTextBlock()
        {
            var WinResultTextBlock = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {

                TxtBet.Text = "";

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
                    //TxtDoubleServiceWin.Opacity = 1;
                    TxtRollResult.Text = "Double Service Win!";
                }
                if (text == "TripleService")
                {
                    //TxtTripleServiceWin.Opacity = 1;
                    TxtRollResult.Text = "Triple Service Win!";
                }
                if (text == "Lose")
                {
                    TxtRollResult.Text = "You Lose!";
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

            
            //Debug.WriteLine(AzureColor1 + " " + AzureResult1 + " -- " + AzureColor2 + " " + AzureResult2 + " -- " + AzureColor3 + " " + AzureResult3);
            Debug.WriteLine(AzureResult1 + " -- " + AzureResult2 + " -- " + AzureResult3);

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
                currentCredits = (currentCredits + (TripleServicePayOut + currentBet));
                ShowWinResultItem("TripleService");

                Log_Event(1, "Triple Service Win", "SpinEvent", "UserAction", CurrentGameID);
                
            }

            if (IconWinResult != "TripleServiceWin")
            {
                //Double Color
                if (AzureResult1 == AzureResult2 | AzureResult1 == AzureResult3 | AzureResult2 == AzureResult3)
                {
                    Debug.WriteLine("Double Azure Service Win!");
                    IconWinResult = "Double Azure Service Win";

                    UpdateWinLoseTextBlock("Double Azure Service Win!");

                    currentCredits = (currentCredits + (DoubleServicePayOut + currentBet));
                    ShowWinResultItem("DoubleService");

                    //Transmit Result to Azure
                    Log_Event(1, "Double Service Win", "SpinEvent", "UserAction", CurrentGameID);

                }
            }

            if (ColorWinResult == "" & IconWinResult == "")
            {
                Debug.WriteLine("You Lose!");
                UpdateWinLoseTextBlock("You Lose!!!");

                var FailSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //PlayWavSound("LosingHorn.wav");
                    ShowWinResultItem("Lose");
                    snd_Chord.Play();
                });

                Log_Event(1, "Loss", "SpinEvent", "UserAction", CurrentGameID);



            }
            else
            {

                var DoublColorWinSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
                {
                    snd_Win95.Play();
                });


            }

            ColorWinResult = "";
            IconWinResult = "";
            
            UpdateCreditsTextBlock(currentCredits.ToString());




            var task = Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
            if (currentCredits > 0)
            {
                button_Play.IsEnabled = true;
                button_Bet.IsEnabled = true;
                button_End.IsEnabled = true;

                //Reset Bet
                currentBet = 1;
                UpdateCurrentBetTextBlock(currentBet.ToString());

            }
            else
            {
                Log_Event(0, "Bankrupt", "SpinEvent", "UserAction", CurrentGameID);

                button_Bet.IsEnabled = false;
                button_Play.IsEnabled = false;
                button_End.IsEnabled = false;
                UpdateWinLoseTextBlock("BANKRUPT!");
                TxtBet.Opacity = 0;

                var FailSoundTask = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                        //   PlayWavSound("LosingHorn.wav");
                        
                    Random ran = new Random();

                    int index = ran.Next(0,4);

                    if (index == 0)
                    {
                        snd_LosingHorn.Play();
                    }
                    if (index == 1)
                    {
                        snd_HaHa.Play();
                    }
                    if (index == 2)
                    {
                        snd_MarioFail.Play();
                    }
                    if (index == 3)
                    {
                        snd_SadTrom.Play();
                    }



                });



              

                var dialog = new ContentDialog()
                {
                    Title = "You are Bankrupt!",
                    MaxWidth = this.ActualWidth, // Required for Mobile!
                    Content = "Would you like to play again?"  //YourXamlContent

                };

                dialog.PrimaryButtonText = "OK";
                dialog.IsPrimaryButtonEnabled = true;
                dialog.PrimaryButtonClick += delegate {     ResetGame();       };

                var result = await dialog.ShowAsync();

                

                }


            });



                        


        }


       


   private async Task Log_Event(double DataValue, string Name, string Sensor, string DataType, string UnitOfMeasure)
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


                        System.Diagnostics.Debug.WriteLine("Creating Device Client with string:");
                        System.Diagnostics.Debug.WriteLine(DeviceConnectionString);
                        DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
                        

                        string dataBuffer;
                        dataBuffer = jsoncontent;

                        System.Diagnostics.Debug.WriteLine("JSON is:");
                        System.Diagnostics.Debug.WriteLine(jsoncontent);



                        
                        Message eventMessage = new Message(System.Text.Encoding.UTF8.GetBytes(dataBuffer));

                        System.Diagnostics.Debug.WriteLine("event message is:");
                        System.Diagnostics.Debug.WriteLine(eventMessage.ToString());
                    
                        await deviceClient.SendEventAsync(eventMessage);


                        System.Diagnostics.Debug.WriteLine("Azure Transmit Done");



                    }
                }


        }

        
    }
}
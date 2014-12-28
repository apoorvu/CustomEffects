using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using AviarySDK;
using System.Windows.Threading;
using System.Diagnostics;
using Microsoft.Phone.Info;
using System.Windows.Navigation;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using Windows.Storage.Streams;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using Microsoft.Phone.Tasks;
using Windows.Storage.Streams;
using Microsoft.Xna.Framework.Media;

namespace AviaryDemo
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        string path;
        private FilterEffect _cartoonEffect = null;

        // The following  WriteableBitmap contains 
        // The filtered and thumbnail image.
        private WriteableBitmap _cartoonImageBitmap = null;
        private WriteableBitmap _thumbnailImageBitmap = null;
        // Constructor
        AviaryTask aviaryTask;
       // public static int count;
        bool _initialized;
        private readonly DispatcherTimer m_animationTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(10) };

private double m_friction = 0.99;

private Point m_scrollStartPoint;

private Point m_scrollTarget = new Point();

private double m_hvelocity;

private double m_vvelocity;

private double m_angle;

private double _cx, _cy;
        public MainPage()
        {
            InitializeComponent();
            //AviaryImage
            _cartoonImageBitmap = new WriteableBitmap((int)AviaryImage.Width, (int)AviaryImage.Height);
        _thumbnailImageBitmap = new WriteableBitmap((int)AviaryImage.Width, (int)AviaryImage.Height);

            _initialized = false;
            
              m_animationTimer.Tick += new EventHandler(m_animationTimer_Tick);
              //if (App.IsTrial&&count==5)
              //{
              //    button1.Visibility = button2.Visibility = Visibility.Collapsed;
              //    MessageBox.Show("Your Trial Has Expired.You Need To Purchase The App ", "Custom Effects", MessageBoxButton.OK);
              //    MessageBox.Show("To Purchase The App,Use The Application Bar Below", "Custom Effects", MessageBoxButton.OK);

                  
              //    //AviaryFeature.Effects. = Visibility.Collapsed;
                  
                  
              //}
            

            
        }

private void m_animationTimer_Tick(object sender, EventArgs e)
{
 	
    if (m_hvelocity < 0.1)
        m_hvelocity = 0;

    if (m_vvelocity < 0.1)
        m_vvelocity = 0;

    if (m_hvelocity == 0 && m_vvelocity == 0)
    {
        m_animationTimer.Stop();
        return;
    }

    if (m_angle > 0 && m_angle < 180)
        scaleTrans.CenterY -= m_vvelocity;
    else
        scaleTrans.CenterY += m_vvelocity;

    if(m_angle > 90 && m_angle < 270)
        scaleTrans.CenterX += m_hvelocity;
    else
        scaleTrans.CenterX -= m_hvelocity;
            
     m_hvelocity *= m_friction;
     m_vvelocity *= m_friction;

     if ((scaleTrans.CenterX <= 0 || scaleTrans.CenterX >= AviaryImage.Width) || (scaleTrans.CenterY <= 0 || scaleTrans.CenterY >= AviaryImage.Height))
     {
         if (scaleTrans.CenterX < 0)
             scaleTrans.CenterX = 0;

         if (scaleTrans.CenterX > AviaryImage.Width)
             scaleTrans.CenterX = AviaryImage.Width;

         if (scaleTrans.CenterY < 0)
             scaleTrans.CenterY = 0;

          if (scaleTrans.CenterY > AviaryImage.Height)
              scaleTrans.CenterY = AviaryImage.Height;

          m_animationTimer.Stop();
    }
}
        
private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
{
    m_angle = e.Angle;

    m_animationTimer.Stop();

    m_scrollStartPoint = e.GetPosition(this);
    m_scrollTarget.X = m_scrollStartPoint.X + e.HorizontalVelocity;
    m_scrollTarget.Y = m_scrollStartPoint.Y + e.VerticalVelocity;
                
    m_hvelocity = Math.Abs(e.HorizontalVelocity)/100;
    m_vvelocity = Math.Abs(e.VerticalVelocity)/100;
                                
    if (m_scrollTarget.X < 0) 
        m_scrollTarget.X = 0;

    if(m_scrollTarget.X > AviaryImage.Width)
        m_scrollTarget.X = AviaryImage.Width;

    if (m_scrollTarget.Y < 0)
        m_scrollTarget.Y = 0;

    if (m_scrollTarget.Y > AviaryImage.Height)
        m_scrollTarget.Y = AviaryImage.Height;

    m_animationTimer.Start();
}

private void GestureListener_PinchStart(object sender, PinchStartedGestureEventArgs e)
{
    Point p1 = e.GetPosition(AviaryImage, 0);
    Point p2 = e.GetPosition(AviaryImage, 1);

    scaleTrans.CenterX = (p1.X + ((p2.X - p1.X) / 2));
    scaleTrans.CenterY = (p1.Y + ((p2.Y - p1.Y) / 2));

    _cx = scaleTrans.ScaleX;
    _cy = scaleTrans.ScaleY;
}

private void GestureListener_PinchDelta(object sender, PinchGestureEventArgs e)
{         
    // Compute new scaling factors
    double cx = _cx * e.DistanceRatio;
    double cy = _cy * e.DistanceRatio;

    // If they're between 1.0 and 4.0, inclusive, apply them
    if (cx >= 1.0 && cx <= 4.0 && cy >= 1.0 && cy <= 4.0)
    {
        if ((cy - 1) < 0.1 && (cx - 1) < 0.1)
            cx = cy = 1;
            
        scaleTrans.ScaleX = cx;
        scaleTrans.ScaleY = cy;
    }
}

private void GestureListener_PinchComplete(object sender, PinchGestureEventArgs e)
{
}

private void GestureListener_DragStart(object sender, DragStartedGestureEventArgs e)
{
}

private void GestureListener_DragDelta(object sender, DragDeltaGestureEventArgs e)
{
    scaleTrans.CenterX = (scaleTrans.CenterX - e.HorizontalChange);
    scaleTrans.CenterY = (scaleTrans.CenterY - e.VerticalChange);

    if (scaleTrans.CenterX < 0)
        scaleTrans.CenterX = 0;
    else if (scaleTrans.CenterX > (AviaryImage.Height * scaleTrans.ScaleX))
        scaleTrans.CenterX = AviaryImage.Height * scaleTrans.ScaleX;

    if(scaleTrans.CenterY < 0)
        scaleTrans.CenterY = 0;
    else if (scaleTrans.CenterY > (AviaryImage.Height * scaleTrans.ScaleY))
        scaleTrans.CenterY = AviaryImage.Height * scaleTrans.ScaleY;
}

private void GestureListener_DragCompleted(object sender, DragCompletedGestureEventArgs e)
{
    scaleTrans.CenterX = (scaleTrans.CenterX - e.HorizontalChange);
    scaleTrans.CenterY = (scaleTrans.CenterY - e.VerticalChange);

    if (scaleTrans.CenterX < 0)
        scaleTrans.CenterX = 0;
    else if (scaleTrans.CenterX > AviaryImage.Width)
        scaleTrans.CenterX = AviaryImage.Width;

    if (scaleTrans.CenterY < 0)
        scaleTrans.CenterY = 0;
    else if (scaleTrans.CenterY > (AviaryImage.Height))
        scaleTrans.CenterY = AviaryImage.Height;
}

private void GestureListener_DoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
{
    scaleTrans.ScaleX = scaleTrans.ScaleY = 1;
}

private void GestureListener_Tap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
{
    m_animationTimer.Stop();
    m_hvelocity = 0;
    m_vvelocity = 0;
}

    //throw new NotImplementedException();


        #region sir this all i have coded... check out the last lines AviaryTaskCompleted...last 3 lines i added them
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;
            if (queryStrings.ContainsKey("token") && !_initialized)
            {
                _initialized = true;
                //MessageBox.Show("ssss");              
                // Retrieve the picture from the media library using the token passed to the application.
                using (MediaLibrary library = new MediaLibrary())
                {
                    Picture picture = library.GetPictureFromToken(queryStrings["token"]);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.CreateOptions = BitmapCreateOptions.None; //OK
                    bitmap.SetSource(picture.GetImage());

                    AviaryImage.Source = bitmap;
                    Stream stream = picture.GetImage();
                    //ok
                    aviaryTask = new AviaryTask(stream, themeColor: "32A9FF");

                    aviaryTask.Completed += new EventHandler<AviaryTaskResultArgs>(aviaryTask_Completed);

                    aviaryTask.Show();
                }
                
            }
            base.OnNavigatedTo(e);
        }
        #endregion 
        
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //ShowMemory();
        }

        //private void ShowMemory()
        //{
        //    GC.Collect();
        //    var cuMemUsage = (long)DeviceStatus.ApplicationCurrentMemoryUsage;
        //    var maxMemUsage = (long)DeviceStatus.DeviceTotalMemory;
        //    var pkMemUsage = (long)DeviceStatus.ApplicationPeakMemoryUsage;
        //    var ulMemLimit = (long)DeviceStatus.ApplicationMemoryUsageLimit;

        //    cuMemUsage /= 1024 * 1024;
        //    maxMemUsage /= 1024 * 1024;
        //    pkMemUsage /= 1024 * 1024;
        //    ulMemLimit /= 1024 * 1024;

        //    Debug.WriteLine(String.Format("Current Memory : {0} MB, Peak Memory : {1} MB, Usage Limit : {2} MB, Total Memory : {3} MB, ", cuMemUsage, pkMemUsage, ulMemLimit, maxMemUsage));
        //}

        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(photoChooserTask_Completed);
            photoChooserTask.Show();
        }

       async  void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                Stream stream = e.ChosenPhoto;
                e.ChosenPhoto.Position = 0;
                var cartoonFilter = new CartoonFilter();
                _cartoonEffect.Filters = new[] { cartoonFilter };
                var renderer = new WriteableBitmapRenderer(_cartoonEffect, _cartoonImageBitmap);
                _cartoonImageBitmap = await renderer.RenderAsync();
                AviaryImage.Source = _cartoonImageBitmap;
                // ProgressLoading.Visibility = Visibility.Visible;

                AviaryTask aviaryTask = new AviaryTask(stream, themeColor: "FFFFFF");//change the theme color from here
                aviaryTask.Completed += new EventHandler<AviaryTaskResultArgs>(aviaryTask_Completed);
                aviaryTask.Show();
                

                //AviaryPhotoGeniusApply aviaryPhotoGenius = new AviaryPhotoGeniusApply(stream, new float[4]);
                //aviaryPhotoGenius.Completed += new EventHandler<AviaryPhotoGeniusApplyResultArgs>(aviaryPhotoGenius_Completed);
                //aviaryPhotoGenius.Execute();

                //AviaryPhotoGeniusScores aviaryPhotoGenius = new AviaryPhotoGeniusScores(stream);
                //aviaryPhotoGenius.Completed += new EventHandler<AviaryPhotoGeniusScoresResultArgs>(aviaryPhotoGenius_Completed);
                //aviaryPhotoGenius.Execute();
            }
        }

        //void aviaryPhotoGenius_Completed(object sender, AviaryPhotoGeniusScoresResultArgs e)
        //{
        //    //  ProgressLoading.Visibility = Visibility.Collapsed;
        //    ShowMemory();
        //    if (e.AviaryResult == AviaryResult.OK)
        //    {
        //        //e.Predicts;
        //        //e.Scores;
        //        //e.TotalScore;
        //    }
        //    else
        //    {
        //        aviaryTask_Error(e.Exception);
        //    }
        //}

        //void aviaryPhotoGenius_Completed(object sender, AviaryPhotoGeniusApplyResultArgs e)
        //{
        //    // ProgressLoading.Visibility = Visibility.Collapsed;
        //    ShowMemory();
        //    if (e.AviaryResult == AviaryResult.OK)
        //    {
        //        AviaryImage.Source = e.PhotoResult;
        //    }
        //    else
        //    {
        //        aviaryTask_Error(e.Exception);
        //    }
        //}

    public void aviaryTask_Completed(object sender, AviaryTaskResultArgs e)
        {
            //ProgressLoading.Visibility = Visibility.Collapsed;
            //ShowMemory(); //y
            //nahi MessageBox.Show(e.AviaryResult.ToString());
            //Appbar1.IsVisible = true;
            if (e.AviaryResult == AviaryResult.OK) //self added count variable
            {

                    AviaryImage.Source = (ImageSource)e.PhotoResult;

                    var stream = new MemoryStream();
                
                    // Save the picture to the WP7 media library

                    e.PhotoResult.SaveJpeg(stream, e.PhotoResult.PixelWidth, e.PhotoResult.PixelHeight, 0, 100);
                    stream.Seek(0, SeekOrigin.Begin);
                    var lib = new MediaLibrary();
                    var picture = lib.SavePicture(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), stream);
                     path = picture.GetPath();
                   // var abc = lib.MediaSource.ToString();
                   // new MediaLibrary().SavePicture(DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), stream);
                    //count = count + 1;//self added

                //ek sec ruk
            }
            else
            {
               // MessageBox.Show("Your Trial Period Has Expired!,Please Purchase The App", "Custom Effects", MessageBoxButton.OK);//self added
                //aviaryTask_Error(e.Exception); self commented
            }
        }

        void aviaryTask_Error(Exception ex)
        {
            MessageBox.Show("error");
            if (ex == null)
                return;

            if (ex.Message == AviaryError.StreamNull)
            {
                // Input stream can't be null
            }
            else if (ex.Message == AviaryError.FeaturesEmpty)
            {
                // Features list determines which tools are exposed in the Aviary editor and cannot be null or empty
                //
            }
            else if (ex.Message == AviaryError.ImageBig)
            {
                // The image cannot exceed 8 mega pixels
                //
            }
            else if (ex.Message == AviaryError.AdjustmentsEmpty)
            {
                // The adjustment array passed into Photo Genius Apply is not valid.
                // The array must be of 4 float values and the array can't be empty or null
                //
            }
            else
            {
                // This is to handle any error thrown by the system
                //
            }
        }

        private void TakePhoto_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureTask cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
            cameraCaptureTask.Show();
           // ShareStatusTask task = new ShareStatusTask();
            
        }

        void cameraCaptureTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                Stream stream = e.ChosenPhoto;

                AviaryTask aviaryTask = new AviaryTask(stream, themeColor: "004F00");
                aviaryTask.Completed += new EventHandler<AviaryTaskResultArgs>(aviaryTask_Completed);
                aviaryTask.Show();
                //Appbar1.IsVisible = true;

                //OK try deploying now..
                //AviaryPhotoGeniusApply aviaryPhotoGenius = new AviaryPhotoGeniusApply(stream, new float[4]);
                //aviaryPhotoGenius.Completed += new EventHandler<AviaryPhotoGeniusApplyResultArgs>(aviaryPhotoGenius_Completed);
                //aviaryPhotoGenius.Execute();

                //AviaryPhotoGeniusScores aviaryPhotoGenius = new AviaryPhotoGeniusScores(stream);
                //aviaryPhotoGenius.Completed += new EventHandler<AviaryPhotoGeniusScoresResultArgs>(aviaryPhotoGenius_Completed);
                //aviaryPhotoGenius.Execute();
            }
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = new MessageBoxResult();
           // MessageBoxButton abc = new MessageBoxButton();
           
        // MessageBoxResult result=new MessageBox("","",MessageBoxButton.
         //MessageBoxResult result= MessageBox.Show("We Would Recommend You To Connect Your Facebook For Better Experience.Would You Like To Do It Now?","Custom Effects",MessageBoxButton.OKCancel);
         //if (result ==MessageBoxResult.OK)
         //{
         //    WebBrowserTask Task = new WebBrowserTask();
         //    Task.Uri =new Uri( "http://www/google.com");
         //    Task.Show();
         //   // NavigationService.Navigate(new Uri("",UriKind.Relative));
         //}
         //else
         //{
         //    return;
         //}

        }

        

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            WebBrowserTask Task = new WebBrowserTask();
            Task.Uri = new Uri("http://www.facebook.com/customeffectswp");
            Task.Show();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            
            ShareMediaTask Task = new ShareMediaTask();
            Task.FilePath = path;
            Task.Show();
            //MessageBox.Show("Image Shared Successfuly On Your Social Networks connected through the phone", "Effects for Life", MessageBoxButton.OK);

        }
    }
}
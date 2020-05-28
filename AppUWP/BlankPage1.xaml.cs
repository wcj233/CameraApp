using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AppUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BlankPage1 : Page
    {
        public BlankPage1()
        {
            this.InitializeComponent();
        }

        private Rectangle rectangle1;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            rectangle1 = new Rectangle();
            rectangle1.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
            rectangle1.Width = 200;
            rectangle1.Height = 100;
            rectangle1.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            rectangle1.StrokeThickness = 3;
            rectangle1.RadiusX = 50;
            rectangle1.RadiusY = 10;

            // When you create a XAML element in code, you have to add
            // it to the XAML visual tree. This example assumes you have
            // a panel named 'layoutRoot' in your XAML file, like this:
            // <Grid x:Name="layoutRoot>
            layoutRoot.Children.Add(rectangle1);

            await PlayLiveVideo();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Compositor compositor = ElementCompositionPreview.GetElementVisual(rectangle1).Compositor;
            var shadowVisual = compositor.CreateSpriteVisual();
            Vector2 newSize = new Vector2(0, 0);
            if (rectangle1 is FrameworkElement element)
            {
                newSize = new Vector2((float)element.ActualWidth, (float)element.ActualHeight);
            }
            shadowVisual.Size = newSize;

            var dropShadow = compositor.CreateDropShadow();
            dropShadow.BlurRadius = 10;
            dropShadow.Opacity = 0.3f;
            dropShadow.Offset = new Vector3(10,10,10);
            dropShadow.Color = Windows.UI.Colors.Black;

            shadowVisual.Shadow = dropShadow;

            // Sets a Visual as child of the Control’s visual tree.
            ElementCompositionPreview.SetElementChildVisual(rectangle1, shadowVisual);
        }

        private async Task PlayLiveVideo()
        {
            var allGroups = await MediaFrameSourceGroup.FindAllAsync();
            var eligibleGroups = allGroups.Select(g => new
            {
                Group = g,

                // For each source kind, find the source which offers that kind of media frame,
                // or null if there is no such source.
                SourceInfos = new MediaFrameSourceInfo[]
                {
        g.SourceInfos.FirstOrDefault(info => info.DeviceInformation?.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front
            && info.SourceKind == MediaFrameSourceKind.Color),
        g.SourceInfos.FirstOrDefault(info => info.DeviceInformation?.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back
            && info.SourceKind == MediaFrameSourceKind.Color)
                }
            }).Where(g => g.SourceInfos.Any(info => info != null)).ToList();

            if (eligibleGroups.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No source group with front and back-facing camera found.");
                return;
            }

            var selectedGroupIndex = 0; // Select the first eligible group
            MediaFrameSourceGroup selectedGroup = eligibleGroups[selectedGroupIndex].Group;
            MediaFrameSourceInfo frontSourceInfo = selectedGroup.SourceInfos[0];

            MediaCapture mediaCapture = new MediaCapture();
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = selectedGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                StreamingCaptureMode = StreamingCaptureMode.Video,
            };
            try
            {
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MediaCapture initialization failed: " + ex.Message);
                return;
            }
            var frameMediaSource1 = MediaSource.CreateFromMediaFrameSource(mediaCapture.FrameSources[frontSourceInfo.Id]);
            VideoStreamer.SetPlaybackSource(frameMediaSource1);
            VideoStreamer.Play();
        }
    }
}

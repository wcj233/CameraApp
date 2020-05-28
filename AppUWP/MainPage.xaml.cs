using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>


    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MyListViewItemsClass> Items { get; set; }

        private List<string> ItemsFromModel;

        private int _myFontSize;
        public int MyFontSize
        {
            get { return _myFontSize; }
            set
            {
                if (value != _myFontSize)
                {
                    _myFontSize = value;
                    OnPropertyChanged("MyFontSize");
                    // Font size changed, need to refresh ListView items
                    LoadRefreshMyListViewItems();
                }
            }
        }

        public MainViewModel()
        {
            // set default font size
            _myFontSize = 20;

            // example data
            ItemsFromModel = new List<string>()
        {
            "ABC",
            "ABCDEF",
            "ABCDEFGHI",
        };

            LoadRefreshMyListViewItems();
        }

        public void LoadRefreshMyListViewItems()
        {
            int itemMaxTextLength = 0;
            foreach (var modelItem in ItemsFromModel)
            {
                if (modelItem.Length > itemMaxTextLength) { itemMaxTextLength = modelItem.Length; }
            }
            Items = new ObservableCollection<MyListViewItemsClass>();
            // Convert points to pixels, multiply by max character length to determine fixed textblock width
            // This assumes 96 DPI. Search for how to grab system DPI in C# there are answers on SO. 
            double width = MyFontSize * 0.75 * itemMaxTextLength;
            foreach (var itemFromModel in ItemsFromModel)
            {
                var item = new MyListViewItemsClass();
                item.MyText = itemFromModel;
                item.ItemFontSize = MyFontSize;
                item.TextWidth = width;
                Items.Add(item);
            }
            OnPropertyChanged("Items");
        }

        public class MyListViewItemsClass
        {
            public string MyText { get; set; }
            public int ItemFontSize { get; set; }
            public double TextWidth { get; set; }
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        public void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class GridViewCustomPanel : Panel
    {
        private double maxWidth;
        private double maxHeight;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var x = 0.0;
            var y = 0.0;
            foreach (var child in Children)
            {
                var newpos = new Rect(x, y, maxWidth, maxHeight);
                child.Arrange(newpos);
                y += maxHeight;
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);

                var desirtedwidth = child.DesiredSize.Width;
                if (desirtedwidth > maxWidth)
                    maxWidth = desirtedwidth;

                var desiredheight = child.DesiredSize.Height;
                if (desiredheight > maxHeight)
                    maxHeight = desiredheight;
            }
            var itemperrow = Math.Floor(availableSize.Width / maxWidth);
            var rows = Math.Ceiling(Children.Count / itemperrow);
            return new Size(itemperrow * maxWidth, maxHeight * rows);
        }
    }


    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            var lv = new List<string>
            {
                "1", "22", "33", "444", "999999999"
            };
            lview.ItemsSource = lv;
            //int itemMaxTextLength = 0;
            //foreach (var modelItem in lv)
            //{
            //    if (modelItem.Length > itemMaxTextLength)
            //    {
            //        itemMaxTextLength = modelItem.Length;
            //    }
            //}

            mainViewModel = new MainViewModel();
            //double width = 14 * 0.75 * itemMaxTextLength;
            //lview.Width = width;
        }

        private MainViewModel mainViewModel { get; set; }
    }
}

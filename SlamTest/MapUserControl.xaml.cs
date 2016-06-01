using System;
using System.Collections.Generic;
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
using Windows.UI.Composition;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SlamTest
{
    public sealed partial class MapUserControl : UserControl
    {
        public MapUserControl()
        {
            this.InitializeComponent();
            MapGrid.DataContext = this;
        }

        #region MapStatusValues DP

        /// <summary>
        /// Gets or sets the MapStatusValues 
        /// MapStatusValues is a 2 dimensional List of char
        /// where each entry represents the status of the map.
        /// </summary>
        public List<List<char>> MapStatusValues
        {
            get { return (List<List<char>>) GetValue(MapStatusProperty); }
            set { SetValue(MapStatusProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty MapStatusProperty =
            DependencyProperty.Register("MapStatusValues", typeof(List<List<char>>),
              typeof(MapUserControl), new PropertyMetadata(""));

        #endregion


        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identified the Label dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object),
              typeof(MapUserControl), new PropertyMetadata(null));
    }
}

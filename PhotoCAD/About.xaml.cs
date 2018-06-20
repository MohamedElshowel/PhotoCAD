using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhotoCAD
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            // Load '<Archivelopers>' logo from the 'Icons' folder
            AboutIcon.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Icons\\Archivelopers_Black.png"));
        }

        private void Author_blk_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (sender.GetType() != typeof(Hyperlink))  return;        // Check hyperlink validation

            string link = ((Hyperlink)sender).NavigateUri.ToString();
            Process.Start(link);
        }
    }
}

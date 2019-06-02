using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
//
using CONTROLLER;
using MODEL_RESULT;

namespace TestDemoLocator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Author: Alex Akhachynskiy
    /// Date: June 02, 2019
    /// Version 1.0
    /// </summary>
    public partial class MainWindow : Window {
        enum ButtonState { RUN, CLEAR }
        public MainWindow() {
            InitializeComponent();
            //Load default values
            TB_ADDRESS.Text = "18333 Hatteras St";
            TB_CITY.Text = "Tarzana";
            TB_ZIP.Text = "91356";
            TB_LocationName.Text = "Mcdonalds";
            TB_Radius.Text = "3000";

            BTN_RUNCLEAR.Content = ButtonState.RUN.ToString();

        }

        private void BTN_RUNCLEAR_Click(object sender, RoutedEventArgs e) {

            #region valdiate api key
            if (TB_APIKEY.Text == "") {
                MessageBox.Show("Missing API key. Need API key to continue");
                return;
            }
            #endregion

            #region run/clear button two function logic
            if (BTN_RUNCLEAR.Content.ToString() == ButtonState.CLEAR.ToString()) {
                TBLK_RESULTS.Text = "";
                TB_LAT.Text = "";
                TB_LNG.Text = "";
                DG_results.ItemsSource = null;
                DG_results.Items.Refresh();
                BTN_RUNCLEAR.Content = ButtonState.RUN.ToString();
                return;
            }
            BTN_RUNCLEAR.Content = ButtonState.CLEAR.ToString();
            #endregion

            #region Step 1 GeoCode current address
            double lat = -1;
            double lng = -1;
            if (!Controller.GeoCodeCurrentLocation(TB_APIKEY.Text, TB_ADDRESS.Text, TB_CITY.Text, TB_ZIP.Text, ref lat, ref lng)) {
                MessageBox.Show("Sorry error occured during geo location step. Please try again later");
                return;
            }
            TB_LAT.Text = lat.ToString();
            TB_LNG.Text = lng.ToString();

            TBLK_RESULTS.Text += "Your location geocoded successfully...\n";

            #endregion

            #region Step 2 Find name of business near location within given radius
            TBLK_RESULTS.Text += String.Format("Searching for {0} within {1} meters of your location ...\n", TB_LocationName.Text, TB_Radius.Text);


            List<Result> myResults = new List<Result>();
            if (!CONTROLLER.Controller.GetLocationData(lat, lng, TB_Radius.Text, TB_LocationName.Text, TB_APIKEY.Text, ref myResults)) {
                MessageBox.Show("Sorry error occured location searching. Please try again later");
                return;
            }
            TBLK_RESULTS.Text += "Success. Found " + myResults.Count() + " location(s). Printing location(s) ...\n\n";

            DG_results.ItemsSource = myResults.OrderBy(x => x.DistanceInMiles);

            #endregion

        }
    }

}

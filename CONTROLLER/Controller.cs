using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;//system web extensions

using System.Net;
using System.Net.Http;

using System.IO;
//model
using LOCATION = DATA_MODEL_LOCATION;
using PLACES = DATA_MODEL_PLACES;
using MODEL_RESULT;

namespace CONTROLLER {
    /// <summary>
    /// 
    /// </summary>
    public static class Controller {
        static Double PI = Math.Acos(0) * 2;
        static Double PI_180 = PI / 180;
        const Double RADIUS_OF_EARTH_IN_MILES = 3963.1676;

        public static bool GeoCodeCurrentLocation(string apiKey, string address, string city, string zip, ref double lat, ref double lng) {
            LOCATION.LocationResponse myGeoCodedLoc = null;
            string locationURL = String.Format(@"https://maps.googleapis.com/maps/api/geocode/json?address={0}+{1}+{2}&sensor=false&key={3}", address, city, zip, apiKey);

            try {
                MODEL.API_ACCESS.QueryGoogleMapsAPI<LOCATION.LocationResponse>(new Uri(locationURL), ref myGeoCodedLoc);
                lat = myGeoCodedLoc.results.FirstOrDefault().geometry.location.lat;
                lng = myGeoCodedLoc.results.FirstOrDefault().geometry.location.lng;
            }
            catch {
                return false;
            }

            return true;
        }
        public static bool GetLocationData(double myLat, double myLng, string radius, string nameOfBusiess, string apiKey, ref List<Result> results) {

            #region prepare data
            PLACES.DataPlacesResponse closeByPlaces = null;
            string PlacesURL = String.Format(@"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&type={3}&keyword={4}&key={5}"
                             , myLat, myLng, radius, "restaurant", nameOfBusiess, apiKey);
            #endregion

            try {
                #region API call
                MODEL.API_ACCESS.QueryGoogleMapsAPI<PLACES.DataPlacesResponse>(new Uri(PlacesURL), ref closeByPlaces);
                #endregion

                #region calculate distance and preload data
                foreach (var place in closeByPlaces.results) {
                    //calcuate distance to host
                    var distInMiles = calcDistance(place.geometry.location.lat, place.geometry.location.lng, myLat, myLng,
                        true);
                    results.Add(new Result() { DistanceInMiles = distInMiles, LocationName = place.name, CurrentStatus = place.opening_hours.open_now == true ? "OPEN" : "CLOSED"
                        , Address = place.vicinity, Rating = place.rating, LocationLatitude = place.geometry.location.lat, LocationLongitude = place.geometry.location.lng });
                }
                #endregion
            }
            catch {
                return false;
            }
            return true;
        }
        private static double calcDistance(double? a_lat, double? a_lng, double b_lat, double b_lng, bool miles) {
            double dist = -1;
            //
            if (a_lat != null && a_lng != null) {
                dist = RADIUS_OF_EARTH_IN_MILES * Math.Acos(
                       Math.Sin((double)(a_lat * PI_180)) * Math.Sin(b_lat * PI_180)
                     + Math.Cos((double)(a_lat * PI_180)) * Math.Cos(b_lat * PI_180) * Math.Cos((double)(a_lng * PI_180 - b_lng * PI_180)));
            }
            return Math.Round((Double)dist, 1);
        }

    }
}

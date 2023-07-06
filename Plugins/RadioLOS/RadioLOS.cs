using GMap.NET;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using MissionPlanner.Utilities;

namespace RadioLOS
{
    public class RadioLOSOptions
    {
        public double base_height = 0;
        public double clearance_angle = 0;
        public double clearance_terrain = 0;
        public double start_azimuth = 0;
        public double stop_azimuth = 0;
        public double azimuth_step = 0.1;
        public double distance_step = 10;
        public double angle_tolerance = 0.1;
    }
    
    internal class RadioLOS
    {

        private double range;
        private double flight_altitude;
        private PointLatLngAlt home;
        private RadioLOSOptions options;

        private int progress_count;
        private double initial_el_angle;
        private double x_range;
        
        const double RADIUS_OF_EARTH = 6378100.0; // m

        public async Task<List<PointLatLng>> CalculateAllowableFlightZoneAsync(double range, double flight_altitude, PointLatLngAlt home, RadioLOSOptions options = null, IProgress<int> progress = null)
        {
            if (options == null) options = new RadioLOSOptions();

            this.range = range;
            this.flight_altitude = flight_altitude;
            this.home = home;
            this.options = options;

            // Find the elevation for which the line-of-sight intersects the flight altitude,
            // accounting for a round Earth. We need to do this iteratively
            double rel_alt = flight_altitude - home.Alt - options.base_height;
            initial_el_angle = Math.Asin(rel_alt / range);
            double el2;
            int loop_limit = 10;
            x_range = range;
            do
            {
                el2 = initial_el_angle;
                x_range = range * Math.Cos(el2);
                rel_alt = range * Math.Sin(el2) - x_range * x_range / RADIUS_OF_EARTH;
                initial_el_angle = Math.Asin(rel_alt / range);
                if(loop_limit-- < 0)
                {
                    throw new Exception("failed to converge elevation angle");
                }
            } while (Math.Abs(initial_el_angle - el2) > options.angle_tolerance);
            initial_el_angle *= 180 / Math.PI;

            var dist_vs_angle = new SortedList<double, double>();

            // Define the range of azimuth angles
            double start_azimuth = options.start_azimuth;
            double stop_azimuth = options.stop_azimuth;
            if (stop_azimuth <= start_azimuth) stop_azimuth += 360; // Unwrap angle
            double azimuth_step = options.azimuth_step;

            progress_count = 0;

            double total_loops = Math.Ceiling((stop_azimuth - start_azimuth) / azimuth_step);

            int num_loops = (int)Math.Floor(stop_azimuth / azimuth_step) - (int)Math.Ceiling(start_azimuth / azimuth_step) + 1;

            await Task.Run(() =>
            {
                // Parallelize the loop over azimuth angles
                Parallel.For(0, num_loops, i =>
                {
                    double azimuth = start_azimuth + i * azimuth_step;

                    // Call your existing function to calculate the max distance for the given azimuth and flight_altitude
                    double maxDistance = CalculateMaxDistance(azimuth);
                    lock (dist_vs_angle)
                    {
                        dist_vs_angle.Add(azimuth, maxDistance);
                        progress?.Report(100 * progress_count++ / num_loops);
                    }
                });
            });

            // Copy to output list, and account for angle clearance in horizontal direction.
            // Essentailly this means take the minimum distance for a window of +/-clearance_angle
            var allowableFlightZone = new List<PointLatLng>();

            for(int i = 0; i < dist_vs_angle.Count; i++)
            {
                double azimuth = start_azimuth + i * azimuth_step;
                double min_dist = double.PositiveInfinity;

                int n = (int)Math.Round(options.clearance_angle / azimuth_step);
                for(int j = -n; j <= n; j++)
                {
                    double test_az = azimuth + j * azimuth_step;
                    if (dist_vs_angle.ContainsKey(test_az) && dist_vs_angle[test_az] < min_dist)
                    {
                        min_dist = dist_vs_angle[test_az];
                    }
                }

                allowableFlightZone.Add(home.newpos(azimuth, min_dist));
            }

            // If we are displaying a finite arc, add the home point to the end of the list
            if (stop_azimuth - start_azimuth <= 360 - azimuth_step)
            {
                allowableFlightZone.Add(home);
            }

            return allowableFlightZone;
        }

        // Iteratively sweep through elevation angles. At each elevation angle, step from the base out along that
        // line until it crosses through terrain or reaches the aircraft flight_altitude. If it crosses terrain, raise
        // the elevation angle. If it is clear, lower it. Repeat until angle converges or until a terrain flight_altitude
        // is found that equals the aircraft flight flight_altitude.
        private double CalculateMaxDistance(double azimuth)
        {

            double el_angle = initial_el_angle - options.clearance_angle;

            double min = el_angle;
            double max = 90;

            // Resulting horizontal distance from base
            double x_dist;
            while (min + options.angle_tolerance < max)
            {
                // Actual distance from base
                double slant_range = 0;
                // MSL altitude of the test point
                double alt = home.Alt + options.base_height;
                // MSL altitude of terrain at x_dist away from home along azimuth
                double terrain_alt = home.Alt;
                // Cache of trig functions, for speed
                double SinElAngle = Math.Sin(el_angle * Math.PI / 180);
                double CosElAngle = Math.Cos(el_angle * Math.PI / 180);

                while (slant_range < range && alt < flight_altitude)
                {
                    slant_range += options.distance_step;
                    x_dist = slant_range * CosElAngle;
                    alt = home.Alt + options.base_height + slant_range * SinElAngle - x_dist * x_dist / RADIUS_OF_EARTH;
                    var pos = home.newpos(azimuth, x_dist);
                    terrain_alt = srtm.getAltitude(pos.Lat, pos.Lng).alt;

                    // We have found a point where the aircraft would crash into terrain.
                    // We don't need to calculate anything else, return x_dist
                    if (terrain_alt > flight_altitude - options.clearance_terrain) return x_dist;

                    if(terrain_alt > alt) break;
                }

                // Move the test elevation angle depending whether the LOS cleared terrain
                if (terrain_alt > alt) min = el_angle;
                else max = el_angle;
                el_angle = (max + min) / 2;
            }

            // Apply the clearance angle to the elevation
            el_angle += options.clearance_angle;
            el_angle = Math.Min(el_angle, 90);

            // Calculate the x_dist for which the line will reach flight altitude at the given elevation angle
            x_dist = (flight_altitude - home.Alt - options.base_height) / Math.Tan(el_angle * Math.PI / 180);
            // Iteratively solve this distance for curved Earth
            double change = x_dist;
            while (Math.Abs(change) > options.distance_step)
            {
                double rel_alt = flight_altitude - home.Alt - options.base_height - x_dist * x_dist / RADIUS_OF_EARTH;
                change = rel_alt / Math.Tan(el_angle * Math.PI / 180) - x_dist;
                x_dist += change;
            }

            return x_dist;
        }
    }
}

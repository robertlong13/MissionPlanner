using GMap.NET;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using MissionPlanner.Utilities;

namespace RadioLOS
{
    public class RadioLOSOptions
    {
        // Height of the base radio above terrain
        public double base_height = 0;
        // Minimum angle that the line-of-sight must clear the terrain by
        public double clearance_angle = 0;
        // Minimum height above terrain that the aircraft must pass over terrain
        public double clearance_terrain = 0;
        // Start and stop angles to reduce calculation area to only the region of interest
        public double start_azimuth = 0;
        public double stop_azimuth = 0;
        // Azimuth resolution
        public double azimuth_step = 0.1;
        // Terrain sample resolution
        public double distance_step = 10;
        // Convergence angle for calculating minimum elevation angle
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
        
        const double RADIUS_OF_EARTH = 6378100.0; // m

        // Calculate the allowable flight zone to maintain radio line of sight between the base and the aircraft.
        // range - maximum range of aircraft in meters
        // flight_altitude - planned MSL altitude of the flight in meters
        // home - home location of the aircraft in lat, lon, MSL alt (m)
        // options - additional options for the calculation
        // progress - progress callback for drawing progress bar
        public async Task<List<PointLatLng>> CalculateAllowableFlightZoneAsync(double range, double flight_altitude, PointLatLngAlt home, RadioLOSOptions options = null, IProgress<int> progress = null)
        {
            if (options == null) options = new RadioLOSOptions();

            this.range = range;
            this.flight_altitude = flight_altitude;
            this.home = home;
            this.options = options;

            // Calculate and cache the elevation angle from the base to the aircraft from range and flight_altitude
            // Use law of cosines to find elevation angle of a ray of length `range` that connects the circle of
            // (RADIUS_OF_EARTH + base_altitude) to the circle of (RADIUS_OF_EARTH + flight_altitude).
            double R1 = RADIUS_OF_EARTH + home.Alt + options.base_height;
            double R2 = RADIUS_OF_EARTH + flight_altitude;
            initial_el_angle = Math.Asin((R2 * R2 - R1 * R1 - range * range) / (2 * range * R1)) * 180 / Math.PI;

            // Define the range of azimuth angles
            double start_azimuth = options.start_azimuth;
            double stop_azimuth = options.stop_azimuth;
            if (stop_azimuth <= start_azimuth) stop_azimuth += 360; // Unwrap angle
            double azimuth_step = options.azimuth_step;

            // This is the output of polar coordinates of our calculated allowable flight zone
            var dist_vs_angle = new SortedList<double, double>();

            progress_count = 0;
            int num_loops = (int)Math.Floor(stop_azimuth / azimuth_step) - (int)Math.Ceiling(start_azimuth / azimuth_step) + 1;

            await Task.Run(() =>
            {
                // Parallelize the loop over azimuth angles
                Parallel.For(0, num_loops, i =>
                {
                    double azimuth = start_azimuth + i * azimuth_step;

                    // Calculate the maximum distance for this azimuth angle
                    double maxDistance = CalculateMaxDistance(azimuth);

                    // Copy to output list and report progress
                    lock (dist_vs_angle)
                    {
                        dist_vs_angle.Add(azimuth, maxDistance);
                        progress?.Report(100 * progress_count++ / num_loops);
                    }
                });
            });

            // Copy to output list, and account for angle clearance in horizontal direction.
            // Essentailly this means take the minimum distance for a window of +/-clearance_angle
            var allowableFlightZone = new List<PointLatLng>(dist_vs_angle.Count);

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
            // so the sides of the "pie slice" get drawn
            if (stop_azimuth - start_azimuth <= 360 - azimuth_step)
            {
                allowableFlightZone.Add(home);
            }

            return allowableFlightZone;
        }

        // Iteratively sweep through elevation angles. At each elevation angle, step from the base out along that
        // line until it crosses through terrain or reaches the aircrafts' flight altitude. If it crosses terrain,
        // raise the elevation angle. If it is clear, lower it. Repeat until angle converges or until a terrain
        // altitude is found that is too close to the aircraft's flight altitude.
        private double CalculateMaxDistance(double azimuth)
        {

            double el_angle = initial_el_angle - options.clearance_angle;

            double min = el_angle;
            double max = 90;

            // Radius of circle representing the base altitude
            double R1 = RADIUS_OF_EARTH + home.Alt + options.base_height;
            // Radius of cicle representing aircraft altitude
            double R2 = RADIUS_OF_EARTH + flight_altitude;
            while (min + options.angle_tolerance < max)
            {
                // Full 3D distance from base to aircraft
                double slant_range = 0;
                // MSL altitude of the test point
                double alt = home.Alt + options.base_height;
                // MSL altitude of terrain at x_dist away from home along azimuth
                double terrain_alt = home.Alt;
                // Cache of trig functions, for speed
                double SinElAngle = Math.Sin(el_angle * Math.PI / 180);
                double CosElAngle = Math.Cos(el_angle * Math.PI / 180);

                while (alt < flight_altitude && slant_range < range) // The second condition should not be possible, but just in case
                {
                    slant_range += options.distance_step;
                    // Use law of cosines to get altitude of test point
                    alt = Math.Sqrt(R1 * R1 + slant_range * slant_range + 2 * R1 * slant_range * SinElAngle) - RADIUS_OF_EARTH;
                    double x_dist = RADIUS_OF_EARTH * Math.Asin(slant_range * CosElAngle / (RADIUS_OF_EARTH + alt)); // Law of sines
                    var pos = home.newpos(azimuth, x_dist);
                    terrain_alt = srtm.getAltitude(pos.Lat, pos.Lng).alt;

                    // We have found a point where the aircraft would pass too close to terrain.
                    // We don't need to calculate anything else, return x_dist.
                    // The immediate-return is possible only because
                    //      1. We start from home and move out
                    //      2. We know that nothing has broken LOS yet along this ray, so this is
                    //         guaranteed to be a smaller (or equal, if clearance is set to zero)
                    //         to the max LOS range
                    if (terrain_alt > flight_altitude - options.clearance_terrain) return x_dist;

                    // The LOS hits terrain, break and try higher el_angle
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
            // (a form of law of sines again)
            return RADIUS_OF_EARTH * (Math.Acos(R1 / R2 * Math.Cos(el_angle * Math.PI / 180)) - el_angle * Math.PI / 180);

        }
    }
}

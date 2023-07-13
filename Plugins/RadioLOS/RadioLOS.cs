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

            // flight_altitude must be strictly above the base altitude
            if (flight_altitude <= home.Alt + options.base_height)
            {
                throw new ArgumentException("Flight altitude must be above base altitude");
            }

            // Calculate and cache the elevation angle from the base to the aircraft from range and flight_altitude
            // Use law of cosines to find elevation angle of a ray of length `range` that connects the circle of
            // (RADIUS_OF_EARTH + base_altitude) to the circle of (RADIUS_OF_EARTH + flight_altitude).
            double R1 = RADIUS_OF_EARTH + home.Alt + options.base_height;
            double R2 = RADIUS_OF_EARTH + flight_altitude;
            initial_el_angle = Math.Asin((R2 * R2 - R1 * R1 - range * range) / (2 * range * R1));

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

        // Find the max distance we can fly in a given direction before line-of-sight is lost
        // Do this by finding the max elevation angle of every point of terrain within range.
        // From this elevation angle, we calculate the horizontal distance to the point where
        // the LOS reaches the flight altitude.
        private double CalculateMaxDistance(double azimuth)
        {
            // Radius of circle representing the base altitude
            double R1 = RADIUS_OF_EARTH + home.Alt + options.base_height;

            // Clearance angle in radians
            double clearance_angle = options.clearance_angle * Math.PI / 180;

            double el_angle = initial_el_angle - clearance_angle;

            // Calculate the horizontal distance at the max slant range
            double max_x_dist = RADIUS_OF_EARTH * Math.Asin(range / (RADIUS_OF_EARTH + flight_altitude) * Math.Cos(initial_el_angle));

            // Sweep through horizontal distances from the base
            for (double x_dist = options.distance_step; x_dist < max_x_dist; x_dist += options.distance_step)
            {
                // Position of the test point
                PointLatLngAlt pos = home.newpos(azimuth, x_dist);

                // Radius of the circle representing the terrain altitude at this point
                double R2 = RADIUS_OF_EARTH + srtm.getAltitude(pos.Lat, pos.Lng).alt;

                // Solve for elevation angle from the base to this point on the surface
                double test_angle = Math.Atan2(Math.Cos(x_dist / RADIUS_OF_EARTH) - R1 / R2, Math.Sin(x_dist / RADIUS_OF_EARTH));
                
                // Track the terrain point that has the highest elevation angle from the base
                if (test_angle > el_angle)
                {
                    el_angle = test_angle;
                    // Calculate the new horizontal distance to the point where the line of sight
                    // reaches the aircraft's flight altitude, given base altitude and elevation angle
                    // (this changes the for-loop condition check)
                    max_x_dist = RADIUS_OF_EARTH * (Math.Acos(R1 / (RADIUS_OF_EARTH + flight_altitude) * Math.Cos(el_angle + clearance_angle)) - el_angle - clearance_angle);
                }

                if (R2 - RADIUS_OF_EARTH > flight_altitude - options.clearance_terrain)
                {
                    // We have found a point where the aircraft would pass dangerously close to terrain.
                    // We don't need to calculate anything else, return x_dist.
                    // The immediate-return is possible only because
                    //      1. We start from home and move out, so there isn't a nearer terrain risk
                    //      2. We know that nothing has broken LOS yet along this ray, so this is
                    //         guaranteed to be a smaller (or equal, if clearance is set to zero)
                    //         than the max LOS range
                    return x_dist;
                }
            }

            // Calculate the x_dist for which the line will reach flight altitude at the given elevation angle
            // (caclulated using the law of sines)
            return max_x_dist;
        }
    }
}

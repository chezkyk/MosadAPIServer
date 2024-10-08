﻿using MosadAPIServer.Models;

namespace MosadAPIServer.Services
{
    public class LocationService
    {
        // בדיקת וולדטציה לכייונים שצריך ללכת
        public static void VerifyingLocation(Location location, string direction)
        {
            switch (direction)
            {
                case "nw":
                    location.X -= 1;
                    location.Y -= 1;
                    if(location.X <1 )
                    {
                        location.X += 1;
                    }
                    if (location.Y < 1)
                    {
                        location.Y += 1;
                    }
                    break;
                case "n":
                    location.X -= 1;
                    if (location.X < 1)
                    {
                        location.X += 1;
                    }
                    break;
                case "ne":
                    location.X -= 1;
                    location.Y += 1;
                    if (location.X < 1)
                    {
                        location.X += 1;
                    }
                    if (location.Y > 1000)
                    {
                        location.Y -= 1;
                    }
                    break;
                case "w":
                    location.Y -= 1;
                    if (location.Y < 1)
                    {
                        location.Y += 1;
                    }
                    break;
                case "e":
                    location.Y += 1;
                    if (location.Y > 1000)
                    {
                        location.Y -= 1;
                    }
                    break;
                case "sw":
                    location.X += 1;
                    location.Y -= 1;
                    if (location.X > 1000)
                    {
                        location.X -= 1;
                    }
                    if (location.Y < 1)
                    {
                        location.Y += 1;
                    }
                    break;
                case "s":
                    location.X += 1;
                    if (location.X > 1000)
                    {
                        location.X -= 1;
                    }
                    break;
                case "se":
                    location.X += 1;
                    location.Y += 1;
                    if (location.X > 1000)
                    {
                        location.X -= 1;
                    }
                    if (location.Y > 1000)
                    {
                        location.Y -= 1;
                    }
                    break;
            }
        }
    }
}

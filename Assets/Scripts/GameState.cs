using System.Collections.Generic;
using UnityEngine;

namespace TestPerformance
{
    public class GameState
    {
        public const int TERRAIN_WIDTH = 500;
        public const float MAX_DISTANCE_SIGHT = 25;
        public static List<OldSection> oldSections = new List<OldSection>();
        public static List<Quadrant> quadrants = new List<Quadrant>();
        //public static Dictionary<string, List<Transform>> sections = new Dictionary<string, List<Transform>>();
        public static Dictionary<string, Vector3> scenes = new Dictionary<string, Vector3>();
        private static int numberOfScenes = 0;
        public static int numberOfLoadedItems = 0;
        public static int numberOfVisibleItems = 0;
        public static int totalNumberOfItems = 0;

        public static int NumberOfScenes
        {
            get { return numberOfScenes; }
            set
            {
                numberOfScenes = value;
                Debug.Log(string.Format("Scenes Loaded: {0}", numberOfScenes));
            }
        }

        public static string GetPathFromTypeOfObject(string typeOf)
        {
            switch(typeOf)
            {
                case "Coral":return "Items/Cnidarians/Corals/";
            }

            return string.Empty;
        }
    }
}
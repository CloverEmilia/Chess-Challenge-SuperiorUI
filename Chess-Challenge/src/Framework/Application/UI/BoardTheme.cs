using Raylib_cs;

namespace ChessChallenge.Application
{
    public class BoardTheme
    {
        public Color LightCol = new(196, 210, 230, 255);       // Light blue
        public Color DarkCol = new(90, 120, 160, 255);        // Dark blue
        public Color selectedLight = new(190, 170, 230, 255); // Pale purple
        public Color selectedDark = new(140, 100, 200, 255);  // Dark purple

        public Color MoveFromLight = new(200, 160, 205, 155); // Light purple
        public Color MoveFromDark = new(200, 160, 205, 155);  // Dark purple

        public Color MoveToLight = new (220, 120, 255, 155);   // Pale purple
        public Color MoveToDark = new (220, 120, 255, 155);    // Dark pale purple

        public Color LegalLight = new (255, 240, 180, 255);    // Pale yellow
        public Color LegalDark = new (230, 200, 150, 255);     // Dark pale yellow

        public Color CheckLight = new (255, 100, 100, 255);    // Light red
        public Color CheckDark = new (255, 100, 100, 255);       // Dark red

        public Color BorderCol = new (80, 80, 80, 255);        // Dark grey

        public Color LightCoordCol = new (210, 230, 245, 255); // Light blue
        public Color DarkCoordCol = new (100, 140, 180, 255);  // Dark blue

        public Color strongNeutralTextColor = new(200, 200, 200, 255);
        public Color weakNeutralTextColor = new(100, 100, 100, 255);
        public Color positiveTextColor = new(67, 204, 101, 255);
        public Color negativeTextColor = new(200, 0, 0, 255); // [temp, wrong, fix]





        /* Original colors-
        public Color LightCol = new Color(238, 216, 192, 255);
        public Color DarkCol = new Color(171, 121, 101, 255);

        public Color selectedLight = new Color(236, 197, 123, 255);
        public Color selectedDark = new Color(200, 158, 80, 255);

        public Color MoveFromLight = new Color(207, 172, 106, 255);
        public Color MoveFromDark = new Color(197, 158, 54, 255);

        public Color MoveToLight = new Color(221, 208, 124, 255);
        public Color MoveToDark = new Color(197, 173, 96, 255);

        public Color LegalLight = new Color(89, 171, 221, 255);
        public Color LegalDark = new Color(62, 144, 195, 255);

        public Color CheckLight = new Color(234, 74, 74, 255);
        public Color CheckDark = new Color(207, 39, 39, 255);

        public Color BorderCol = new Color(44, 44, 44, 255);

        public Color LightCoordCol = new Color(255, 240, 220, 255);
        public Color DarkCoordCol = new Color(140, 100, 80, 255);*/
    }
}


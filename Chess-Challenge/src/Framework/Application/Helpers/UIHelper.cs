using Raylib_cs;
using System;
using System.Numerics;
using static ChessChallenge.Application.FileHelper;

namespace ChessChallenge.Application
{
    public static class UIHelper
    {
        static readonly bool SDF_Enabled = true;
        const string fontName = "damase_v2.ttf";
        const int referenceResolution = 1920;

        static Font font;
        static Font fontSdf;
        static Shader shader;

        public enum AlignH
        {
            Left,
            Centre,
            Right
        }
        public enum AlignV
        {
            Top,
            Centre,
            Bottom
        }

        static UIHelper()
        {
            if (SDF_Enabled)
            {
                unsafe
                {
                    const int baseSize = 64;
                    uint fileSize = 0;
                    var fileData = Raylib.LoadFileData(GetResourcePath("Fonts", fontName), ref fileSize);
                    Font fontSdf = default;
                    fontSdf.baseSize = baseSize;
                    fontSdf.glyphCount = 95;
                    fontSdf.glyphs = Raylib.LoadFontData(fileData, (int)fileSize, baseSize, null, 0, FontType.FONT_SDF);

                    Image atlas = Raylib.GenImageFontAtlas(fontSdf.glyphs, &fontSdf.recs, 95, baseSize, 0, 1);
                    fontSdf.texture = Raylib.LoadTextureFromImage(atlas);
                    Raylib.UnloadImage(atlas);
                    Raylib.UnloadFileData(fileData);

                    Raylib.SetTextureFilter(fontSdf.texture, TextureFilter.TEXTURE_FILTER_BILINEAR);
                    UIHelper.fontSdf = fontSdf;

                }
                shader = Raylib.LoadShader("", GetResourcePath("Fonts", "sdf.fs"));
            }
            font = Raylib.LoadFontEx(GetResourcePath("Fonts", fontName), 128, null, 0);

        }

        public static void DrawText(string text, Vector2 pos, int size, int spacing, Color col, AlignH alignH = AlignH.Left, AlignV alignV = AlignV.Centre)
        {
            Vector2 boundSize = Raylib.MeasureTextEx(font, text, size, spacing);
            float offsetX = alignH == AlignH.Left ? 0 : (alignH == AlignH.Centre ? -boundSize.X / 2 : -boundSize.X);
            float offsetY = alignV == AlignV.Top ? 0 : (alignV == AlignV.Centre ? -boundSize.Y / 2 : -boundSize.Y);
            Vector2 offset = new(offsetX, offsetY);

            if (SDF_Enabled)
            {
                Raylib.BeginShaderMode(shader);
                Raylib.DrawTextEx(fontSdf, text, pos + offset, size, spacing, col);
                Raylib.EndShaderMode();
            }
            else
            {
                Raylib.DrawTextEx(font, text, pos + offset, size, spacing, col);
            }
        }

        public static bool Button(string text, Vector2 centre, Vector2 size)
        {
            Rectangle rec = new(centre.X - size.X / 2, centre.Y - size.Y / 2, size.X, size.Y);

            Color normalCol = new(40, 40, 40, 255);
            Color hoverCol = new(230, 200, 150, 255);
            Color pressCol = new(250, 230, 130, 255);

            bool mouseOver = MouseInRect(rec);
            bool pressed = mouseOver && Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);
            bool pressedThisFrame = pressed && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
            Color col = mouseOver ? (pressed ? pressCol : hoverCol) : normalCol;
            Color textCol = mouseOver ? Color.WHITE : new Color(130, 130, 130, 205);
            int fontSize = ScaleInt(28);

            if (pressed) { //squishy button interactivity
                SquishButtonWidthAndPosition(ref rec);
                fontSize = ScaleInt(38);
            }

            Raylib.DrawRectangleRec(rec, col);

            DrawText(text, centre, fontSize, 1, textCol, AlignH.Centre);

            return pressedThisFrame;
        }

            //this might be needless by a couple lines, I've been staring at it too long
        static void SquishButtonWidthAndPosition(ref Rectangle rec)
        {
            // Get the original button dimensions
            float originalWidth = rec.width;
            float originalHeight = rec.height;
            Vector2 originalPosition = new(rec.x, rec.y);

            // Calculate the new dimensions;
            float newWidth = originalWidth * 0.75f;
            float newHeight = originalHeight *1.25f;

            // Calculate the change
            float widthChange = originalWidth - newWidth;
            float heightChange = originalHeight - newHeight;

            // Update the dimensions
            rec.width = newWidth;
            rec.height = newHeight;

            // adjust to keep centered, pivot is in the center
            rec.x = originalPosition.X + widthChange / 2;
            rec.y = originalPosition.Y + heightChange / 2;
        }

        static bool MouseInRect(Rectangle rec)
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            return mousePos.X >= rec.x && mousePos.Y >= rec.y && mousePos.X <= rec.x + rec.width && mousePos.Y <= rec.y + rec.height;
        }

        public static float Scale(float val, int referenceResolution = referenceResolution)
        {
            return Raylib.GetScreenWidth() / (float)referenceResolution * val;
        }

        public static int ScaleInt(int val, int referenceResolution = referenceResolution)
        {
            return (int)Math.Round(Raylib.GetScreenWidth() / (float)referenceResolution * val);
        }

        public static Vector2 Scale(Vector2 vec, int referenceResolution = referenceResolution)
        {
            float x = Scale(vec.X, referenceResolution);
            float y = Scale(vec.Y, referenceResolution);
            return new Vector2(x, y);
        }

        public static void Release()
        {
            Raylib.UnloadFont(font);
            if (SDF_Enabled)
            {
                Raylib.UnloadFont(fontSdf);
                Raylib.UnloadShader(shader);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModDataTools.Editor
{
    public static class CoordinateTextureGenerator
    {
        public static Sprite GenerateMultipleCoordinatesSprite(int[][] coords)
        {
            Texture2D tex = GenerateMultipleCoordinatesTexture(coords, 96, 96, 3f, Color.white);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            return sprite;
        }

        public static Texture2D GenerateMultipleCoordinatesTexture(int[][] coords, int width, int height, float lineWidth, Color lineColor)
        {
            if (coords == null || coords.Length == 0)
            {
                return null;
            }
            Texture2D texture = new Texture2D(width * coords.Length, height, TextureFormat.RGBA32, false, false);
            texture.SetPixels(Enumerable.Repeat(Color.clear, texture.width * texture.height).ToArray());
            float x = 0f;
            for (int i = 0; i < coords.Length; i++)
            {
                if (coords[i] == null || coords[i].Length < 2)
                {
                    continue;
                }
                // Remove extra space if coordinate doesn't use left slot
                if (!coords[i].Contains(5))
                {
                    x -= width * 0.175f;
                }
                Rect rect = new Rect(x, 0f, width, height);
                DrawCoordinateLines(texture, rect, coords[i], lineWidth, lineColor);
                // Remove extra space if coordinate doesn't use right slot
                if (!coords[i].Contains(2))
                {
                    x -= width * 0.175f;
                }
                x += width;
            }
            texture.Apply();
            return texture;
        }

        private static Vector2 GetCoordinateCornerPixel(Rect rect, int corner, float margin)
        {
            Vector2 size = rect.size;
            Vector2 center = size * 0.5f;
            float radius = Mathf.Min(size.x, size.y) * 0.475f - margin;
            float angle = Mathf.Deg2Rad * (120f - (60f * corner));
            Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        }

        private static void DrawCoordinateLines(Texture2D texture, Rect rect, int[] coords, float lineWidth, Color lineColor)
        {
            if (coords == null || coords.Length < 2)
            {
                return;
            }
            for (int i = 0; i < coords.Length - 1; i++)
            {
                Vector2 start = rect.position + GetCoordinateCornerPixel(rect, coords[i + 0], lineWidth);
                Vector2 end = rect.position + GetCoordinateCornerPixel(rect, coords[i + 1], lineWidth);

                int x0 = Mathf.FloorToInt(Mathf.Min(start.x, end.x) - lineWidth * 2f);
                int y0 = Mathf.FloorToInt(Mathf.Min(start.y, end.y) - lineWidth * 2f);
                int x1 = Mathf.CeilToInt(Mathf.Max(start.x, end.x) + lineWidth * 2f);
                int y1 = Mathf.CeilToInt(Mathf.Max(start.y, end.y) + lineWidth * 2f);

                Vector2 dir = end - start;
                float length = dir.magnitude;
                dir.Normalize();

                for (int x = x0; x <= x1; x++)
                {
                    for (int y = y0; y <= y1; y++)
                    {
                        Vector2 p = new Vector2(x, y);
                        float dot = Vector2.Dot(p - start, dir);
                        dot = Mathf.Clamp(dot, 0f, length);
                        Vector2 pointOnLine = start + dir * dot;
                        float distToLine = Mathf.Max(0f, Vector2.Distance(p, pointOnLine) - lineWidth);
                        if (distToLine <= 1f)
                        {
                            DrawPixel(texture, x, y, lineColor, 1f - Mathf.Clamp01(distToLine));
                        }
                    }
                }
            }
        }

        private static void DrawPixel(Texture2D texture, int x, int y, Color color, float blend)
        {
            if (color.a * blend < 1f)
            {
                Color existing = texture.GetPixel(x, y);
                if (existing.a > 0f)
                {
                    float colorA = color.a;
                    color.a = 1f;
                    texture.SetPixel(x, y, Color.Lerp(existing, color, Mathf.Clamp01(colorA * blend)));
                    return;
                }
            }
            color.a *= blend;
            texture.SetPixel(x, y, color);
        }
    }
}

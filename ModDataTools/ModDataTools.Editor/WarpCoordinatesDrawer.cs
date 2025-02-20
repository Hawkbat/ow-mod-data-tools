using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;

namespace ModDataTools.Editor
{
    [CustomPropertyDrawer(typeof(StarSystemAsset.Coordinates))]
    public class WarpCoordinatesDrawer : PropertyDrawer
    {
        const int COORD_TEX_SIZE = 50;

        Texture2D cachedTexture;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label) + 2f + COORD_TEX_SIZE;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var currentValue = "";
            var xProp = property.FindPropertyRelative("x");
            for (int i = 0; i < xProp.arraySize; i++)
            {
                var prop = xProp.GetArrayElementAtIndex(i);
                currentValue += prop.intValue;
            }
            var yProp = property.FindPropertyRelative("y");
            if (yProp.arraySize > 0)
                currentValue += " ";
            for (int i = 0; i < yProp.arraySize; i++)
            {
                var prop = yProp.GetArrayElementAtIndex(i);
                currentValue += prop.intValue;
            }
            var zProp = property.FindPropertyRelative("z");
            if (zProp.arraySize > 0)
                currentValue += " ";
            for (int i = 0; i < zProp.arraySize; i++)
            {
                var prop = zProp.GetArrayElementAtIndex(i);
                currentValue += prop.intValue;
            }

            var textHeight = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);

            var parsedValue = EditorGUI.TextField(new Rect(position.x, position.y, position.width, textHeight), label, currentValue);

            var coords = parsedValue.Split(' ').Select(s => s.SelectMany(c =>
            {
                if (int.TryParse("" + c, out int digit) && digit >= 0 && digit < 6)
                {
                    return new int[] { digit % 6 };
                }
                return new int[] { };
            }).Distinct().ToArray()).Where(c => c.Length > 0).ToArray();

            if (coords.Length > 0)
            {
                while (xProp.arraySize > coords[0].Length) xProp.DeleteArrayElementAtIndex(xProp.arraySize - 1);
                while (xProp.arraySize < coords[0].Length) xProp.InsertArrayElementAtIndex(xProp.arraySize);
                for (int i = 0; i < coords[0].Length; i++) xProp.GetArrayElementAtIndex(i).intValue = coords[0][i];
            }
            else
            {
                xProp.ClearArray();
            }

            if (coords.Length > 1)
            {
                while (yProp.arraySize > coords[1].Length) yProp.DeleteArrayElementAtIndex(yProp.arraySize - 1);
                while (yProp.arraySize < coords[1].Length) yProp.InsertArrayElementAtIndex(yProp.arraySize);
                for (int i = 0; i < coords[1].Length; i++) yProp.GetArrayElementAtIndex(i).intValue = coords[1][i];
            }
            else
            {
                yProp.ClearArray();
            }

            if (coords.Length > 2)
            {
                while (zProp.arraySize > coords[2].Length) zProp.DeleteArrayElementAtIndex(zProp.arraySize - 1);
                while (zProp.arraySize < coords[2].Length) zProp.InsertArrayElementAtIndex(zProp.arraySize);
                for (int i = 0; i < coords[2].Length; i++) zProp.GetArrayElementAtIndex(i).intValue = coords[2][i];
            }
            else
            {
                zProp.ClearArray();
            }

            if (!cachedTexture || currentValue != parsedValue)
            {
                if (cachedTexture) Object.DestroyImmediate(cachedTexture);
                cachedTexture = CoordinateTextureGenerator.GenerateMultipleCoordinatesTexture(coords, COORD_TEX_SIZE, COORD_TEX_SIZE, 2f, Color.white);
            }

            if (cachedTexture)
            {
                var textureRect = EditorGUI.PrefixLabel(position, new GUIContent(" "));
                EditorGUI.DrawTextureTransparent(new Rect(textureRect.x, textureRect.y + textHeight + 2f, COORD_TEX_SIZE * coords.Length, COORD_TEX_SIZE), cachedTexture);
            }
        }
    }
}

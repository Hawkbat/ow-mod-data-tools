using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;
using ModDataTools.Utilities;

namespace ModDataTools.Editors
{
    public class ShipLogEditorWindow : EditorWindow
    {
        float zoom = 1f;
        Vector2 areaScrollPosition;
        Rect areaScrollRect;
        Dictionary<EntryAsset, Rect> entryRects = new Dictionary<EntryAsset, Rect>();
        HashSet<EntryAsset> selected = new HashSet<EntryAsset>();
        EntryAsset hovered = null;
        EntryAsset lastHovered = null;
        bool wasAreaClick = false;
        bool wasEntryClick = false;
        bool wasModifiedClick = false;
        Vector2 dragStart = Vector2.zero;
        Vector2 dragOffset = Vector2.zero;

        static GUIStyle entryBackStyle;
        static GUIStyle entryHeadStyle;
        static GUIStyle entryBodyStyle;
        static GUIStyle rumorLineStyle;
        static GUIStyle rumorArrowStyle;

        [MenuItem("Window/Ship Log Editor")]
        public static void Open()
        {
            var window = GetWindow<ShipLogEditorWindow>();
            window.Show();
        }

        void RegenerateStyles()
        {
            Texture2D white = new Texture2D(1, 1);
            white.SetPixel(0, 0, Color.white);
            white.Apply();

            Texture2D black = new Texture2D(1, 1);
            black.SetPixel(0, 0, Color.black);
            black.Apply();

            Texture2D grey = new Texture2D(1, 1);
            black.SetPixel(0, 0, Color.grey);
            black.Apply();

            entryBackStyle = new GUIStyle();
            entryBackStyle.name = nameof(entryBackStyle);
            entryBackStyle.imagePosition = ImagePosition.ImageOnly;
            entryBackStyle.normal.background = white;

            entryHeadStyle = new GUIStyle();
            entryHeadStyle.name = nameof(entryHeadStyle);
            entryHeadStyle.imagePosition = ImagePosition.TextOnly;
            entryHeadStyle.font = Resources.Load<Font>("ModDataTools/Fonts/SpaceMono-Bold");
            entryHeadStyle.fontSize = 14;
            entryHeadStyle.wordWrap = true;
            entryHeadStyle.alignment = TextAnchor.MiddleCenter;
            entryHeadStyle.normal.textColor = Color.black;

            entryBodyStyle = new GUIStyle();
            entryBodyStyle.name = nameof(entryBodyStyle);
            entryBodyStyle.imagePosition = ImagePosition.ImageOnly;
            entryBodyStyle.normal.background = black;

            rumorLineStyle = new GUIStyle();
            rumorLineStyle.name = nameof(rumorLineStyle);
            rumorLineStyle.imagePosition = ImagePosition.ImageOnly;
            rumorLineStyle.normal.background = grey;
            rumorLineStyle.onNormal.background = rumorLineStyle.normal.background;
            rumorLineStyle.hover.background = white;
            rumorLineStyle.onHover.background = rumorLineStyle.hover.background;

            rumorArrowStyle = new GUIStyle();
            rumorArrowStyle.name = nameof(rumorArrowStyle);
            rumorArrowStyle.imagePosition = ImagePosition.ImageOnly;
            rumorArrowStyle.normal.background = Resources.Load<Texture2D>("ModDataTools/Textures/RumorArrowGrey");
            rumorArrowStyle.onNormal.background = rumorArrowStyle.normal.background;
            rumorArrowStyle.hover.background = Resources.Load<Texture2D>("ModDataTools/Textures/RumorArrow");
            rumorArrowStyle.onHover.background = rumorArrowStyle.hover.background;
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Ship Log Editor");
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;

            if (entryBackStyle == null || entryBackStyle.name != nameof(entryHeadStyle))
                RegenerateStyles();

            var entries = AssetRepository.GetAllAssets<EntryAsset>();
            var rumorFacts = AssetRepository.GetAllAssets<RumorFactAsset>();

            EditorGUILayout.BeginVertical();
            var newAreaScrollPosition = EditorGUILayout.BeginScrollView(areaScrollPosition, "TE BoxBackground");
            if (newAreaScrollPosition != areaScrollPosition)
            {
                var delta = newAreaScrollPosition - areaScrollPosition;
                if (Event.current.shift)
                {
                    areaScrollPosition += new Vector2(delta.y, delta.x);
                }
                else
                {
                    areaScrollPosition = newAreaScrollPosition;
                }
            }

            var min = -Vector2.one * 500f;
            foreach (var entry in entries) min = Vector2.Min(min, entry.EditorPosition);

            var max = Vector2.one * 500f;
            foreach (var entry in entries) max = Vector2.Max(max, entry.EditorPosition);

            min -= Vector2.one * 500f;
            max += Vector2.one * 500f;

            GUILayoutUtility.GetRect((max.x - min.x) * zoom, (max.y - min.y) * zoom);

            var offset = -min;

            hovered = null;

            var sortedEntries = new List<EntryAsset>(entries);
            sortedEntries.Sort((a, b) =>
            {
                if (a.IsCuriosity != b.IsCuriosity)
                    return a.IsCuriosity ? -1 : 1;
                if (a is EntryAsset != b is EntryAsset)
                    return a is EntryAsset ? -1 : 1;

                if (a is EntryAsset a2 && b is EntryAsset b2 && a2.Parent != b2.Parent)
                    return a2.Parent == null ? -1 : 1;
                if (a.EditorPosition.y != b.EditorPosition.y)
                    return a.EditorPosition.y < b.EditorPosition.y ? -1 : 1;
                if (a.EditorPosition.x != b.EditorPosition.x)
                    return a.EditorPosition.x < b.EditorPosition.x ? -1 : 1;
                return 0;
            });

            foreach (var fact in rumorFacts)
            {
                if (fact.Entry && fact.Source && entryRects.ContainsKey(fact.Entry) && entryRects.ContainsKey(fact.Source))
                {
                    var isTwoWayRumor = rumorFacts.Any(r => r.Source == fact.Entry && r.Entry == fact.Source);

                    var startRect = entryRects[fact.Source];
                    var start = startRect.center;
                    var endRect = entryRects[fact.Entry];
                    var end = endRect.center;

                    if (RectIntersect(start, end, startRect, out Vector2 intersectStart))
                        start = intersectStart;
                    if (RectIntersect(start, end, endRect, out Vector2 intersectEnd))
                        end = intersectEnd;

                    var dir = (end - start).normalized;
                    var dist = (end - start).magnitude;
                    var mid = Vector2.Lerp(start, end, 0.5f);
                    var matrix = GUI.matrix;
                    GUIUtility.RotateAroundPivot(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, mid);
                    GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                    if (GUI.Button(new Rect(mid.x - dist * 0.5f, mid.y - 5f, dist, 10f), string.Empty, rumorLineStyle))
                    {
                        Selection.activeObject = fact;
                    }
                    GUI.backgroundColor = Color.white;
                    if (GUI.Button(new Rect(mid.x - 20f + (isTwoWayRumor ? 30f : 0f), mid.y - 20f, 40f, 40f), string.Empty, rumorArrowStyle)) {
                        Selection.activeObject = fact;
                    }
                    GUI.matrix = matrix;
                }
            }

            // EntryCard: t:None AnchoredPosition (X, Y) AnchorMin (0.5, 0.5) OffsetMin(X, Y) AnchorMax (0.5, 0.5) OffsetMax(X, Y) Pivot (0.5, 0.5) SizeDelta (110, 145)
            // > EntryCardRoot: t:None AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin(0, 0) AnchorMax (1, 1) OffsetMax(0, 0) Pivot (0.5, 0.5) SizeDelta (0, 0)
            // > > Background: t:Image (solid black) AnchoredPosition (0, 0) AnchorMin (0.5, 0) OffsetMin(-55, 0) AnchorMax (0.5, 0) OffsetMax(55, 110) Pivot (0.5, 0) SizeDelta (110, 110)
            // > > NameBackground: t:Image (solid entry color) AnchoredPosition (0, 110) AnchorMin (0.5, 0) OffsetMin(-55, 110) AnchorMax (0.5, 1) OffsetMax(55, 0) Pivot (0.5, 0) SizeDelta (110, -110)
            // > > > Name: t:Text (entry name, black) AnchoredPosition (0, 2) AnchorMin (0, 0) OffsetMin(2, 2) AnchorMax (1, 1) OffsetMax(-2, -2) Pivot (0.5, 0) SizeDelta (-4, -4)
            // > > EntryCardBackground: t:Image (solid black) AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin(0, 0) AnchorMax (1, 0) OffsetMax(0, 110) Pivot (0.5, 0) SizeDelta (0, 110)
            // > > > PhotoImage: t:Image (entry photo) AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin (0, 0) AnchorMax (1, 1) OffsetMax (0, 0) Pivot (0.5, 0.5) SizeDelta (0, 0)
            // > > Border: t:Image (entry-colored image with solid 4px (of 512px) border and transparent interior) AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin (0, 0) AnchorMax (1, 1) OffsetMax (0, 0) Pivot (0.5, 0.5) SizeDelta (0, 0)
            
            // 32 reference pixels per unit, 15x scale, calculated 3.125 pixels per unit
            // 110x145 base size, increases to 110x151.3333 for two-line text, 110x167.9583 for three-line text

            foreach (var entry in sortedEntries)
            {
                bool isSelected = selected.Contains(entry);
                var parentEntry = entry is EntryAsset e ? e.Parent : null;
                var scale = entry.IsCuriosity ? 2f : parentEntry != null ? parentEntry.IsCuriosity ? 0.8f : 0.6f : 1f;

                var size = new Vector2(110f, 145f);
                var parentCuriosity = entry.IsCuriosity ? entry : entry.Curiosity ? entry.Curiosity : null;
                var label = new GUIContent(entry.FullName);

                var lineCount = Mathf.RoundToInt(entryHeadStyle.CalcHeight(label, size.x) / 21f);
                size.y = Mathf.Max(145f, 134.70833f + (lineCount - 1) * 16.62497f);

                var entryOffset = offset;
                if (isSelected && wasEntryClick) entryOffset += dragOffset;

                var center = entry.EditorPosition + entryOffset;
                var rect = new Rect(center - size * 0.5f, size);
                var scaledRect = new Rect(center - (size * scale * zoom) * 0.5f, size * scale * zoom);
                entryRects[entry] = scaledRect;
                var isHovered = scaledRect.Contains(Event.current.mousePosition);
                if (isHovered)
                    hovered = entry;
                var isDragSelected = !wasEntryClick && scaledRect.Overlaps(GetDragRect());
                EditorGUIUtility.AddCursorRect(scaledRect, MouseCursor.MoveArrow);

                var color = parentCuriosity != null ? isSelected ? parentCuriosity.HighlightColor : isHovered || isDragSelected ? Color.Lerp(parentCuriosity.NormalColor, parentCuriosity.HighlightColor, 0.5f) : parentCuriosity.NormalColor : Color.white;

                var matrix = GUI.matrix;

                GUIUtility.ScaleAroundPivot(Vector2.one * scale * zoom, scaledRect.center);
                GUI.color = color;
                GUI.Box(rect, "", entryBackStyle);
                GUI.color = Color.white;
                GUI.Label(new Rect(rect.position.x + 2f, rect.position.y + 2f, rect.width - 4f, rect.height - rect.width - 4f), label, entryHeadStyle);
                GUI.Box(new Rect(rect.position.x + 1f, rect.position.y + 1f + (rect.height - rect.width), rect.width - 2f, rect.width - 2f), new GUIContent("", entry.Photo), entryBodyStyle);

                GUI.matrix = matrix;
            }

            if (Event.current.type == EventType.MouseDown)
            {
                dragStart = Event.current.mousePosition;
            }
            if (dragOffset.magnitude > 0f && !wasEntryClick)
            {
                GUI.Box(GetDragRect(), "", "window");
            }

            EditorGUILayout.EndScrollView();
            if (Event.current.type == EventType.Repaint) areaScrollRect = GUILayoutUtility.GetLastRect();
            zoom = EditorGUILayout.Slider("Zoom", zoom, 0.25f, 4f);
            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.MouseDown)
            {
                wasAreaClick = areaScrollRect.Contains(Event.current.mousePosition);
                wasEntryClick = wasAreaClick && hovered != null;
                wasModifiedClick = Event.current.control || Event.current.command || Event.current.shift;
                if (Event.current.button == 1 && wasAreaClick)
                {
                    wasAreaClick = false;
                    var menu = new GenericMenu();
                    if (wasEntryClick)
                    {
                        wasEntryClick = false;
                        var sourceEntry = entries.FirstOrDefault(e => e == hovered);
                        var sourcePath = AssetDatabase.GetAssetPath(sourceEntry);
                        menu.AddItem(new GUIContent("Delete Entry"), false, () =>
                        {
                            AssetDatabase.DeleteAsset(sourcePath);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        });
                        menu.AddItem(new GUIContent("Duplicate Entry"), false, () =>
                        {
                            var destPath = AssetDatabase.GenerateUniqueAssetPath(sourcePath);
                            AssetDatabase.CopyAsset(sourcePath, destPath);
                            var newAsset = AssetDatabase.LoadAssetAtPath<EntryAsset>(destPath);
                            newAsset.EditorPosition = sourceEntry.EditorPosition + new Vector2(110f, 110f);
                            if (!string.IsNullOrEmpty(sourceEntry.ID))
                                newAsset.ID = sourceEntry.ID + "_COPY";

                            selected.Clear();
                            selected.Add(newAsset);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        });
                        menu.AddItem(new GUIContent("Swap Entries"), false, () =>
                        {
                            var targetEntry = entries.FirstOrDefault(e => e != sourceEntry && selected.Contains(e));
                            if (targetEntry != null)
                            {
                                Undo.RecordObjects(new[] { sourceEntry, targetEntry }, "Swap Entries");
                                (targetEntry.EditorPosition, sourceEntry.EditorPosition) = (sourceEntry.EditorPosition, targetEntry.EditorPosition);
                            }
                        });
                        menu.AddItem(new GUIContent("Add Rumor Fact"), false, () =>
                        {
                            var fact = CreateInstance<RumorFactAsset>();
                            fact.Entry = sourceEntry;
                            fact.name = "New Rumor";
                            sourceEntry.RumorFacts.Add(fact);
                            AssetDatabase.AddObjectToAsset(fact, sourceEntry);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        });
                        menu.AddItem(new GUIContent("Add Explore Fact"), false, () =>
                        {
                            var fact = CreateInstance<ExploreFactAsset>();
                            fact.Entry = sourceEntry;
                            fact.name = "New Fact";
                            sourceEntry.ExploreFacts.Add(fact);
                            AssetDatabase.AddObjectToAsset(fact, sourceEntry);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        });
                    }
                    menu.ShowAsContext();
                }
            }
            else if (Event.current.type == EventType.MouseDrag && wasAreaClick)
            {
                dragOffset += Event.current.delta / zoom;
                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && wasAreaClick)
            {
                if (dragOffset.magnitude < 10f)
                {
                    if (hovered == null || !wasModifiedClick)
                    {
                        selected.Clear();
                        Selection.activeObject = null;
                    }
                    if (hovered)
                    {
                        if (selected.Contains(hovered))
                        {
                            selected.Remove(hovered);
                            Selection.activeObject = selected.FirstOrDefault();
                        }
                        else
                        {
                            selected.Add(hovered);
                            Selection.activeObject = hovered;
                        }
                    }
                }
                else if (wasEntryClick)
                {
                    dragOffset = new Vector2(Mathf.Round(dragOffset.x / 10f) * 10f, Mathf.Round(dragOffset.y / 10f) * 10f);
                    Undo.RecordObjects(selected.ToArray(), "Move Entries");
                    foreach (var entry in selected)
                    {
                        entry.EditorPosition += dragOffset;
                    }
                }
                else
                {
                    var dragRect = GetDragRect();
                    if (!wasModifiedClick)
                    {
                        selected.Clear();
                        Selection.activeObject = null;
                    }

                    foreach (var entry in entries)
                    {
                        if (entryRects[entry].Overlaps(dragRect) && !selected.Contains(entry))
                        {
                            selected.Add(entry);
                            Selection.activeObject = entry;
                        }
                    }
                }
                dragOffset = Vector2.zero;
                Repaint();
            }
            else if (Event.current.type == EventType.ValidateCommand)
            {
                Repaint();
            }

            if (lastHovered != hovered)
            {
                Repaint();
            }

            lastHovered = hovered;
        }

        bool LineIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 result)
        {
            var denominator = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);
            if (denominator == 0)
            {
                result = Vector2.zero;
                return false;
            }
            var ua = ((b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x)) / denominator;
            var ub = ((a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x)) / denominator;
            if (ua < 0f || ua > 1f || ub < 0f || ub > 1f)
            {
                result = Vector2.zero;
                return false;
            }
            var x = a1.x + ua * (a2.x - a1.x);
            var y = a1.y + ua * (a2.y - a1.y);
            result = new Vector2(x, y);
            return true;
        }

        bool RectIntersect(Vector2 start, Vector2 end, Rect rect, out Vector2 result)
        {
            var tl = new Vector2(rect.xMin, rect.yMin);
            var tr = new Vector2(rect.xMax, rect.yMin);
            var bl = new Vector2(rect.xMin, rect.yMax);
            var br = new Vector2(rect.xMax, rect.yMax);
            if (LineIntersect(start, end, tl, tr, out result)) return true;
            if (LineIntersect(start, end, tr, br, out result)) return true;
            if (LineIntersect(start, end, br, bl, out result)) return true;
            if (LineIntersect(start, end, bl, tl, out result)) return true;
            result = Vector2.zero;
            return false;
        }

        Rect GetDragRect()
        {
            var x = Mathf.Min(dragStart.x + dragOffset.x * zoom, dragStart.x);
            var y = Mathf.Min(dragStart.y + dragOffset.y * zoom, dragStart.y);
            var w = Mathf.Abs(dragOffset.x * zoom);
            var h = Mathf.Abs(dragOffset.y * zoom);
            return new Rect(x, y, w, h);
        }
    }
}

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

            entryBackStyle = new GUIStyle();
            entryBackStyle.name = nameof(entryBackStyle);
            entryBackStyle.imagePosition = ImagePosition.ImageOnly;
            entryBackStyle.normal.background = white;

            entryHeadStyle = new GUIStyle();
            entryHeadStyle.name = nameof(entryHeadStyle);
            entryHeadStyle.imagePosition = ImagePosition.TextOnly;
            entryHeadStyle.font = EditorGUIUtility.Load("Fonts/RobotoMono/RobotoMono-Regular.ttf") as Font;
            entryHeadStyle.wordWrap = true;
            entryHeadStyle.richText = true;
            entryHeadStyle.alignment = TextAnchor.MiddleCenter;
            entryHeadStyle.normal.textColor = Color.black;

            entryBodyStyle = new GUIStyle();
            entryBodyStyle.name = nameof(entryBodyStyle);
            entryBodyStyle.imagePosition = ImagePosition.ImageOnly;
            entryBodyStyle.normal.background = black;
        }

        int GetFontSize(EntryAsset entry, EntryAsset parentEntry)
        {
            var baseSize = 14;
            var multiplier =
                    parentEntry != null ?
                        parentEntry.IsCuriosity ?
                            0.8f
                            : 0.6f
                    : entry.IsCuriosity ?
                        2f
                        : 1f;
            return Mathf.RoundToInt(baseSize * multiplier * zoom);
        }

        private List<T> LoadAllAssetsOfType<T>() where T : Object
        {
            return AssetDatabase.FindAssets("t:" + typeof(T).Name)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<T>(path))
                .ToList();
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
                    var start = entryRects[fact.Source].center;
                    var end = entryRects[fact.Entry].center;
                    var dist = (end - start).magnitude;
                    var count = Mathf.FloorToInt(dist / 30f);
                    for (var i = 1; i < count; i++)
                    {
                        var p = Vector2.Lerp(start, end, Mathf.InverseLerp(0, count, i));
                        if (GUI.Button(new Rect(p.x - 5f, p.y - 5f, 10f, 10f), ""))
                        {
                            Selection.activeObject = fact;
                        }
                    }
                }
            }

            // 3.125 pixels per unit
            // EntryCard: t:None AnchoredPosition (X, Y) AnchorMin (0.5, 0.5) OffsetMin(X, Y) AnchorMax (0.5, 0.5) OffsetMax(X, Y) Pivot (0.5, 0.5) SizeDelta (110, 145)
            // > EntryCardRoot: t:None AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin(0, 0) AnchorMax (1, 1) OffsetMax(0, 0) Pivot (0.5, 0.5) SizeDelta (0, 0)
            // > > Background: t:Image (solid black) AnchoredPosition (0, 0) AnchorMin (0.5, 0) OffsetMin(-55, 0) AnchorMax (0.5, 0) OffsetMax(55, 110) Pivot (0.5, 0) SizeDelta (110, 110)
            // > > NameBackground: t:Image (solid entry color) AnchoredPosition (0, 110) AnchorMin (0.5, 0) OffsetMin(-55, 110) AnchorMax (0.5, 1) OffsetMax(55, 0) Pivot (0.5, 0) SizeDelta (110, -110)
            // > > > Name: t:Text (entry name, black) AnchoredPosition (0, 2) AnchorMin (0, 0) OffsetMin(2, 2) AnchorMax (1, 1) OffsetMax(-2, -2) Pivot (0.5, 0) SizeDelta (-4, -4)
            // > > EntryCardBackground: t:Image (solid black) AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin(0, 0) AnchorMax (1, 0) OffsetMax(0, 110) Pivot (0.5, 0) SizeDelta (0, 110)
            // > > > PhotoImage: t:Image (entry photo) AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin (0, 0) AnchorMax (1, 1) OffsetMax (0, 0) Pivot (0.5, 0.5) SizeDelta (0, 0)
            // > > Border: t:Image (entry-colored image with solid 4px (of 512px) border and transparent interior) AnchoredPosition (0, 0) AnchorMin (0, 0) OffsetMin (0, 0) AnchorMax (1, 1) OffsetMax (0, 0) Pivot (0.5, 0.5) SizeDelta (0, 0)

            foreach (var entry in sortedEntries)
            {
                bool isSelected = selected.Contains(entry);
                var parentEntry = entry is EntryAsset e ? e.Parent : null;
                var scale = entry.IsCuriosity ? 2f : parentEntry != null ? parentEntry.IsCuriosity ? 0.8f : 0.6f : 1f;
                var size = new Vector2(110f, 110f) * scale * zoom;
                var parentCuriosity = entry.IsCuriosity ? entry : entry.Curiosity ? entry.Curiosity : null;
                var label = new GUIContent("<size=" + GetFontSize(entry, parentEntry) + ">" + entry.FullName + "</size>");
                size.y += entryHeadStyle.CalcHeight(label, size.x);

                var entryOffset = offset;
                if (isSelected && wasEntryClick) entryOffset += dragOffset;

                var rect = new Rect((entry.EditorPosition + entryOffset) * zoom - size * 0.5f, size);
                entryRects[entry] = rect;
                var isHovered = rect.Contains(Event.current.mousePosition);
                if (isHovered)
                    hovered = entry;
                var isDragSelected = !wasEntryClick && rect.Overlaps(GetDragRect());

                var color = parentCuriosity != null ? isSelected ? parentCuriosity.HighlightColor : isHovered || isDragSelected ? Color.Lerp(parentCuriosity.NormalColor, parentCuriosity.HighlightColor, 0.5f) : parentCuriosity.NormalColor : Color.white;

                EditorGUIUtility.AddCursorRect(rect, MouseCursor.MoveArrow);
                GUI.color = color;
                GUI.Box(rect, "", entryBackStyle);
                GUI.color = Color.white;
                GUI.Label(new Rect(rect.position, new Vector2(rect.width, rect.height - rect.width)), label, entryHeadStyle);
                GUI.Box(new Rect(rect.position.x + 1f, rect.position.y + 1f + (rect.height - rect.width), rect.width - 2f, rect.width - 2f), new GUIContent("", entry.Photo), entryBodyStyle);
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

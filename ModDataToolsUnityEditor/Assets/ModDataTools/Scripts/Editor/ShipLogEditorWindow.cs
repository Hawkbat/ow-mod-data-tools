using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ModDataTools;
using ModDataTools.Assets;

namespace ModDataTools.Editors
{
    public class ShipLogEditorWindow : EditorWindow
    {
        public static float currentAssetDatabaseTime;

        float savedAssetDatabaseTime;

        List<EntryBase> entries;
        List<RumorFact> rumorFacts;

        float zoom = 1f;
        Vector2 areaScrollPosition;
        Rect areaScrollRect;
        Dictionary<EntryBase, Rect> entryRects = new Dictionary<EntryBase, Rect>();
        HashSet<EntryBase> selected = new HashSet<EntryBase>();
        EntryBase hovered = null;
        EntryBase lastHovered = null;
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

        int GetFontSize(EntryBase entry, EntryBase parentEntry)
        {
            var baseSize = 14;
            var multiplier =
                    parentEntry != null ?
                        parentEntry is Curiosity ?
                            0.8f
                            : 0.6f
                    : entry is Curiosity ?
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

        void ReloadAssets()
        {
            var curiosities = LoadAllAssetsOfType<Curiosity>();
            var otherEntries = LoadAllAssetsOfType<Entry>();
            entries = new List<EntryBase>().Concat(curiosities).Concat(otherEntries).ToList();
            rumorFacts = LoadAllAssetsOfType<RumorFact>();

            savedAssetDatabaseTime = currentAssetDatabaseTime;
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Ship Log Editor");
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;

            if (entryBackStyle == null || entryBackStyle.name != nameof(entryHeadStyle))
                RegenerateStyles();

            if (entries == null || savedAssetDatabaseTime != currentAssetDatabaseTime)
                ReloadAssets();

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

            var sortedEntries = new List<EntryBase>(entries);
            sortedEntries.Sort((a, b) =>
            {
                if (a is Curiosity != b is Curiosity)
                    return a is Curiosity ? -1 : 1;
                if (a is Entry != b is Entry)
                    return a is Entry ? -1 : 1;

                if (a is Entry a2 && b is Entry b2 && a2.Parent != b2.Parent)
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

            foreach (var entry in sortedEntries)
            {
                bool isSelected = selected.Contains(entry);
                var parentEntry = entry is Entry e ? e.Parent : null;
                var scale = entry is Curiosity ? 2f : parentEntry != null ? parentEntry is Curiosity ? 0.8f : 0.6f : 1f;
                var size = new Vector2(110f, 110f) * scale * zoom;
                var parentCuriosity = entry is Curiosity c ? c : entry is Entry e2 ? e2.Curiosity : null;
                var label = new GUIContent("<size=" + GetFontSize(entry, parentEntry) + ">" + entry.GetFullName() + "</size>");
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
                        var sourceEntry = entries.Find(e => e == hovered);
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
                            var newAsset = AssetDatabase.LoadAssetAtPath<EntryBase>(destPath);
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
                            var fact = CreateInstance<RumorFact>();
                            fact.Entry = sourceEntry;
                            fact.name = "New Rumor";
                            sourceEntry.RumorFacts.Add(fact);
                            AssetDatabase.AddObjectToAsset(fact, sourceEntry);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        });
                        menu.AddItem(new GUIContent("Add Explore Fact"), false, () =>
                        {
                            var fact = CreateInstance<ExploreFact>();
                            fact.Entry = sourceEntry;
                            fact.name = "New Fact";
                            sourceEntry.ExploreFacts.Add(fact);
                            AssetDatabase.AddObjectToAsset(fact, sourceEntry);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        });
                        if (sourceEntry is Entry entry)
                        {
                            menu.AddItem(new GUIContent("Convert to Curiosity"), false, () =>
                            {
                                if (!EditorUtility.DisplayDialog("Destructive Edit", "This will delete any facts or rumors associated with this entry. Are you sure you want to continue?", "Continue", "Cancel")) return;
                                var newCuriosity = CreateInstance<Curiosity>();
                                newCuriosity.OverrideFullName = entry.OverrideFullName;
                                newCuriosity.OverrideFullID = entry.OverrideFullID;
                                newCuriosity.ID = entry.ID;
                                newCuriosity.Planet = entry.Planet;
                                newCuriosity.RumorModePosition = entry.RumorModePosition;
                                newCuriosity.IgnoreMoreToExplore = entry.IgnoreMoreToExplore;
                                newCuriosity.IgnoreMoreToExploreCondition = entry.IgnoreMoreToExploreCondition;
                                newCuriosity.Photo = entry.Photo;
                                newCuriosity.AltPhoto = entry.AltPhoto;
                                newCuriosity.AltPhotoCondition = entry.AltPhotoCondition;
                                newCuriosity.Color = Color.white;
                                if (selected.Contains(sourceEntry))
                                {
                                    selected.Remove(sourceEntry);
                                    selected.Add(newCuriosity);
                                }
                                AssetDatabase.DeleteAsset(sourcePath);
                                AssetDatabase.CreateAsset(newCuriosity, sourcePath);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            });
                        }
                        if (sourceEntry is Curiosity curiosity)
                        {
                            menu.AddItem(new GUIContent("Convert to Normal Entry"), false, () =>
                            {
                                if (!EditorUtility.DisplayDialog("Destructive Edit", "This will delete any facts or rumors associated with this curiosity. Are you sure you want to continue?", "Continue", "Cancel")) return;
                                var newEntry = CreateInstance<Entry>();
                                newEntry.OverrideFullName = curiosity.OverrideFullName;
                                newEntry.OverrideFullID = curiosity.OverrideFullID;
                                newEntry.ID = curiosity.ID;
                                newEntry.Planet = curiosity.Planet;
                                newEntry.RumorModePosition = curiosity.RumorModePosition;
                                newEntry.IgnoreMoreToExplore = curiosity.IgnoreMoreToExplore;
                                newEntry.IgnoreMoreToExploreCondition = curiosity.IgnoreMoreToExploreCondition;
                                newEntry.Photo = curiosity.Photo;
                                newEntry.AltPhoto = curiosity.AltPhoto;
                                newEntry.AltPhotoCondition = curiosity.AltPhotoCondition;
                                if (selected.Contains(sourceEntry))
                                {
                                    selected.Remove(sourceEntry);
                                    selected.Add(newEntry);
                                }
                                AssetDatabase.DeleteAsset(sourcePath);
                                AssetDatabase.CreateAsset(newEntry, sourcePath);
                                AssetDatabase.SaveAssets();
                                AssetDatabase.Refresh();
                            });
                        }
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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ModDataTools.Assets;
using ModDataTools.Utilities;

namespace ModDataTools.Editor
{
    public class ShipLogEditorWindow : EditorWindow
    {
        const float MIN_ZOOM = 0.25f;
        const float MAX_ZOOM = 4f;

        [SerializeField]
        StarSystemAsset starSystem;

        [SerializeField]
        Vector2 pan = Vector2.zero;
        [SerializeField]
        float zoom = 1f;
        [SerializeField]
        HashSet<EntryAsset> selected = new HashSet<EntryAsset>();
        [SerializeField]
        bool showFactCounts;

        Rect areaScrollRect;
        Dictionary<EntryAsset, Rect> entryRects = new Dictionary<EntryAsset, Rect>();

        EntryAsset hovered = null;
        EntryAsset lastHovered = null;
        bool wasAreaClick = false;
        bool wasEntryClick = false;
        bool wasModifiedClick = false;
        bool wasDragClick = false;
        bool wasPanClick = false;
        Vector2 dragStart = Vector2.zero;
        Vector2 dragOffset = Vector2.zero;

        static GUIStyle textStyle;
        static GUIStyle entryBackStyle;
        static GUIStyle entryHeadStyle;
        static GUIStyle entryBodyStyle;
        static GUIStyle entryBorderStyle;
        static GUIStyle rumorLineStyle;
        static GUIStyle rumorArrowStyle;
        static GUIStyle factBoxStyle;
        static Texture2D texWhitePixel;
        static Texture2D texBlackPixel;

        [MenuItem("Window/Ship Log Editor")]
        public static void Open() => Open(null);
        public static void Open(StarSystemAsset starSystem)
        {
            var window = GetWindow<ShipLogEditorWindow>();
            if (starSystem) window.starSystem = starSystem;
            window.Show();
        }

        private void OnEnable()
        {
            AssetRepository.Initialize(new ModDataAdapter(false));
            RegenerateStyles();
        }

        void RegenerateStyles()
        {
            if (texWhitePixel == null)
            {
                texWhitePixel = new Texture2D(1, 1);
                texWhitePixel.SetPixel(0, 0, Color.white);
                texWhitePixel.Apply();
            }

            if (texBlackPixel == null)
            {
                texBlackPixel = new Texture2D(1, 1);
                texBlackPixel.SetPixel(0, 0, Color.black);
                texBlackPixel.Apply();
            }

            if (textStyle == null || textStyle.name != nameof(textStyle))
            {
                textStyle = new GUIStyle();
                textStyle.name = nameof(textStyle);
                textStyle.font = Resources.Load<Font>("ModDataTools/Fonts/SpaceMono-Regular");
                textStyle.fontSize = 20;
                textStyle.richText = true;
                textStyle.normal.textColor = Color.white;
            }

            if (entryBackStyle == null || entryBackStyle.name != nameof(entryBackStyle))
            {
                entryBackStyle = new GUIStyle();
                entryBackStyle.name = nameof(entryBackStyle);
                entryBackStyle.imagePosition = ImagePosition.ImageOnly;
                entryBackStyle.normal.background = texWhitePixel;
            }

            if (entryHeadStyle == null || entryHeadStyle.name != nameof(entryHeadStyle))
            {
                entryHeadStyle = new GUIStyle();
                entryHeadStyle.name = nameof(entryHeadStyle);
                entryHeadStyle.imagePosition = ImagePosition.TextOnly;
                entryHeadStyle.font = Resources.Load<Font>("ModDataTools/Fonts/SpaceMono-Bold");
                entryHeadStyle.fontSize = 14;
                entryHeadStyle.wordWrap = true;
                entryHeadStyle.alignment = TextAnchor.MiddleCenter;
                entryHeadStyle.normal.textColor = Color.black;
            }

            if (entryBodyStyle == null || entryBodyStyle.name != nameof(entryBodyStyle))
            {
                entryBodyStyle = new GUIStyle();
                entryBodyStyle.name = nameof(entryBodyStyle);
                entryBodyStyle.imagePosition = ImagePosition.ImageOnly;
                entryBodyStyle.normal.background = texBlackPixel;
            }

            if (entryBorderStyle == null || entryBorderStyle.name != nameof(entryBorderStyle))
            {
                entryBorderStyle = new GUIStyle();
                entryBorderStyle.name = nameof(entryBorderStyle);
                entryBorderStyle.imagePosition = ImagePosition.ImageOnly;
                entryBorderStyle.normal.background = Resources.Load<Texture2D>("ModDataTools/Textures/EntryBorder");
                entryBorderStyle.border = new RectOffset(1, 1, 1, 1);
            }

            if (rumorLineStyle == null || rumorLineStyle.name != nameof(rumorLineStyle))
            {
                rumorLineStyle = new GUIStyle();
                rumorLineStyle.name = nameof(rumorLineStyle);
                rumorLineStyle.imagePosition = ImagePosition.ImageOnly;
                rumorLineStyle.normal.background = texWhitePixel;
            }

            if (rumorArrowStyle == null || rumorArrowStyle.name != nameof(rumorArrowStyle))
            {
                rumorArrowStyle = new GUIStyle();
                rumorArrowStyle.name = nameof(rumorArrowStyle);
                rumorArrowStyle.imagePosition = ImagePosition.ImageOnly;
                rumorArrowStyle.normal.background = Resources.Load<Texture2D>("ModDataTools/Textures/RumorArrow");
            }

            if (factBoxStyle == null || factBoxStyle.name != nameof(factBoxStyle))
            {
                factBoxStyle = new GUIStyle();
                factBoxStyle.name = nameof(factBoxStyle);
                factBoxStyle.imagePosition = ImagePosition.ImageOnly;
                factBoxStyle.normal.background = Resources.Load<Texture2D>("ModDataTools/Textures/FactBoxBorder");
                factBoxStyle.border = new RectOffset(2, 2, 2, 2);
                factBoxStyle.padding = new RectOffset(0, 5, 5, 5);
                factBoxStyle.margin = new RectOffset(25, 25, 25, 25);
            }
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Ship Log Editor");
            wantsMouseMove = true;
            wantsMouseEnterLeaveWindow = true;

            if (entryBackStyle == null || entryBackStyle.name != nameof(entryBackStyle))
                RegenerateStyles();

            if (!starSystem)
            {
                starSystem = Selection.activeObject as StarSystemAsset;
                if (!starSystem)
                {
                    var entry = Selection.activeObject as EntryAsset;
                    if (entry && entry.Planet) starSystem = entry.Planet.StarSystem;
                }
                if (!starSystem)
                {
                    var fact = Selection.activeObject as FactAsset;
                    if (fact && fact.Entry && fact.Entry.Planet) starSystem = fact.Entry.Planet.StarSystem;
                }
                if (!starSystem)
                {
                    var planet = Selection.activeObject as PlanetAsset;
                    if (planet) starSystem = planet.StarSystem;
                }
            }
            if (!starSystem)
            {
                GUILayout.Label($"Select a {nameof(StarSystemAsset)} asset");
                return;
            }
            EditorGUILayout.ObjectField(starSystem, typeof(StarSystemAsset), false);

            var entries = AssetRepository.GetAllAssets<EntryAsset>()
                .Where(e => e.Planet && e.Planet.StarSystem == starSystem);
            var rumorFacts = entries.SelectMany(e => e.RumorFacts);

            hovered = null;
            var offset = areaScrollRect.size * 0.5f;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginScrollView(Vector2.zero, "TE BoxBackground");

            if (Event.current.type == EventType.ScrollWheel)
            {
                var previousPosition = Event.current.mousePosition;
                var zoomDelta = -(Event.current.delta.y / 3f);

                var canvasPos = Event.current.mousePosition - offset;
                var currentPos = canvasPos / zoom;
                zoom += zoom * 0.1f * zoomDelta;
                zoom = Mathf.Clamp(zoom, MIN_ZOOM, MAX_ZOOM);
                var newPos = canvasPos / zoom;
                pan += newPos - currentPos;
                Repaint();
            }

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
                    var lineWidth = 5f * zoom;
                    var arrowSize = 25f * zoom;
                    var arrowSpacing = arrowSize * 0.75f + (isTwoWayRumor ? dist * 0.1875f : 0f);
                    var innerArrowSpacing = arrowSize * -0.5f + (isTwoWayRumor ? dist * 0.1875f : 0f);
                    var matrix = GUI.matrix;
                    GUIUtility.RotateAroundPivot(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg, mid);

                    GUI.color = Selection.activeObject == fact ? Color.white : Color.grey;

                    if (!isTwoWayRumor && GUI.Button(new Rect(mid.x - dist * 0.5f, mid.y - lineWidth * 0.5f, dist * 0.5f - arrowSpacing, lineWidth), string.Empty, rumorLineStyle))
                    {
                        Selection.activeObject = fact;
                    }
                    if (GUI.Button(new Rect(mid.x + arrowSpacing, mid.y - lineWidth * 0.5f, dist * 0.5f - arrowSpacing, lineWidth), string.Empty, rumorLineStyle))
                    {
                        Selection.activeObject = fact;
                    }
                    if (isTwoWayRumor && GUI.Button(new Rect(mid.x, mid.y - lineWidth * 0.5f, innerArrowSpacing, lineWidth), string.Empty, rumorLineStyle))
                    {
                        Selection.activeObject = fact;
                    }

                    if (GUI.Button(new Rect(mid.x - arrowSize * 0.5f + (isTwoWayRumor ? dist * 0.1875f : 0f), mid.y - arrowSize * 0.5f, arrowSize, arrowSize), string.Empty, rumorArrowStyle))
                    {
                        Selection.activeObject = fact;
                    }

                    GUI.color = Color.white;
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

                var center = (entry.EditorPosition + pan) * zoom + offset;
                if (isSelected && wasEntryClick) center += dragOffset;

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
                GUI.Box(new Rect(rect.position.x, rect.position.y + (rect.height - rect.width), rect.width, rect.width), new GUIContent("", entry.Photo), entryBodyStyle);
                GUI.color = color;
                GUI.Box(new Rect(rect.position.x, rect.position.y + (rect.height - rect.width), rect.width, rect.width), string.Empty, entryBorderStyle);
                GUI.color = Color.white;
                if (showFactCounts) GUI.Label(new Rect(rect.position.x + rect.width + 10f, rect.position.y, 100f, 25f), $"{entry.ExploreFacts.Count()}", textStyle);
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

            IEnumerable<FactAsset> factsToDisplay = Enumerable.Empty<FactAsset>();

            if (Selection.activeObject is EntryAsset selectedEntry)
                factsToDisplay = selectedEntry.ExploreFacts;
            if (Selection.activeObject is RumorFactAsset selectedRumorFact)
                factsToDisplay = rumorFacts.Where(r => r.Source == selectedRumorFact.Source && r.Entry == selectedRumorFact.Entry);
            if (Selection.activeObject is ExploreFactAsset selectedExploreFact)
                factsToDisplay = new List<ExploreFactAsset>() { selectedExploreFact };

            if (factsToDisplay.Any())
            {
                GUILayout.BeginArea(new Rect(areaScrollRect.x + 25f, areaScrollRect.y + 25f, areaScrollRect.width - 50f, areaScrollRect.height - 50f));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                GUI.color = new Color(0.3882f, 0.498f, 0.8431f, 1f);
                GUILayout.BeginVertical(factBoxStyle);
                foreach (var fact in factsToDisplay)
                {
                    var hex = ColorUtility.ToHtmlStringRGB(GUI.color);
                    GUI.color = Color.white;
                    GUILayout.Label($"<size=24><b><color=#{hex}>-</color></b></size> {fact.Text}", textStyle);
                    GUI.color = new Color(0.3882f, 0.498f, 0.8431f, 1f);
                }
                GUILayout.EndVertical();
                GUI.color = Color.white;
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

            zoom = EditorGUILayout.Slider("Zoom", zoom, MIN_ZOOM, MAX_ZOOM);
            showFactCounts = EditorGUILayout.Toggle("Show Fact Counts", showFactCounts);
            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.MouseDown)
            {
                wasAreaClick = areaScrollRect.Contains(Event.current.mousePosition);
                wasEntryClick = wasAreaClick && hovered != null;
                wasModifiedClick = Event.current.control || Event.current.command || Event.current.shift;
                wasDragClick = wasAreaClick && Event.current.button == 0;
                wasPanClick = wasAreaClick && Event.current.button != 0 && (!wasEntryClick || Event.current.button != 1);
                if (Event.current.button == 1 && wasEntryClick)
                {
                    wasAreaClick = false;
                    var menu = new GenericMenu();
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
                        EditorUtility.SetDirty(newAsset);
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
                        EditorUtility.SetDirty(fact);
                        EditorUtility.SetDirty(sourceEntry);
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
                        EditorUtility.SetDirty(fact);
                        EditorUtility.SetDirty(sourceEntry);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    });
                    menu.ShowAsContext();
                }
            }
            else if (Event.current.type == EventType.MouseDrag && wasAreaClick)
            {
                if (wasPanClick)
                {
                    pan += Event.current.delta / zoom;
                }
                else if (wasDragClick)
                {
                    dragOffset += Event.current.delta;
                }
                Repaint();
            }
            else if (Event.current.type == EventType.MouseUp && wasAreaClick)
            {
                if (wasDragClick)
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
                        Undo.RecordObjects(selected.ToArray(), "Move Entries");
                        foreach (var entry in selected)
                        {
                            entry.EditorPosition += dragOffset / zoom;
                            entry.EditorPosition = new Vector2(Mathf.Round(entry.EditorPosition.x / 10f) * 10f, Mathf.Round(entry.EditorPosition.y / 10f) * 10f);
                            EditorUtility.SetDirty(entry);
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
                }
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
            var x = Mathf.Min(dragStart.x + dragOffset.x, dragStart.x);
            var y = Mathf.Min(dragStart.y + dragOffset.y, dragStart.y);
            var w = Mathf.Abs(dragOffset.x);
            var h = Mathf.Abs(dragOffset.y);
            return new Rect(x, y, w, h);
        }
    }
}

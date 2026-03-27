#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using BubbleSpinner.Data;

[CustomEditor(typeof(ConversationAsset))]
public class ConversationAssetEditor : Editor
{
    private int tabIndex;
    private string searchFilter;

    private SerializedProperty profileImageProp;
    private SerializedProperty conversationIdProp;

    private ReorderableList cgList;

    // =========================
    // STYLES
    // =========================
    private GUIStyle cardStyle;
    private GUIStyle sectionStyle;
    private GUIStyle headerStyle;
    private GUIStyle subLabelStyle;
    private GUIStyle _styleDividerLabel;
    private Color DIVIDER_LINE;

    private void InitStyles()
    {
        if (cardStyle == null)
        {
            cardStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(12, 12, 10, 10),
                margin = new RectOffset(0, 0, 6, 6)
            };
        }

        if (sectionStyle == null)
        {
            sectionStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };
        }

        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16
            };
        }

        if (subLabelStyle == null)
        {
            subLabelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
            };
        }

        if (_styleDividerLabel == null)
        {
            _styleDividerLabel = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                fontSize = 10,
                normal = { textColor = new Color(0.65f, 0.65f, 0.65f) }
            };
        }

        DIVIDER_LINE = new Color(1f, 1f, 1f, 0.08f);
    }

    private void OnEnable()
    {
        profileImageProp = serializedObject.FindProperty("profileImage");
        conversationIdProp = serializedObject.FindProperty("conversationId");

        var asset = (ConversationAsset)target;

        cgList = new ReorderableList(
            asset.cgAddressableKeys,
            typeof(string),
            true,
            true,
            true,
            true
        );

        cgList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "CG Addressable Keys");
        };

        cgList.drawElementCallback = (rect, index, active, focused) =>
        {
            rect.y += 2;
            asset.cgAddressableKeys[index] =
                EditorGUI.TextField(rect, asset.cgAddressableKeys[index]);
        };
    }

    public override void OnInspectorGUI()
    {
        InitStyles();

        var asset = (ConversationAsset)target;

        serializedObject.Update();

        DrawHeader(asset);
        DrawTabs();

        GUILayout.Space(10);

        switch (tabIndex)
        {
            case 0: DrawChapters(asset); break;
            case 1: DrawProfile(asset); break;
            case 2: DrawCG(asset); break;
            case 3: DrawTools(asset); break;
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            EditorUtility.SetDirty(asset);
    }

    // =========================
    // HEADER
    // =========================
    private void DrawHeader(ConversationAsset asset)
    {
        GUILayout.Space(5);

        EditorGUILayout.LabelField(
            string.IsNullOrEmpty(asset.characterName) ? "Unnamed Character" : asset.characterName,
            headerStyle
        );

        EditorGUILayout.LabelField("Conversation Asset", subLabelStyle);

        GUILayout.Space(8);
        DrawLine();
        GUILayout.Space(8);
    }

    // =========================
    // TABS
    // =========================
    private void DrawTabs()
    {
        tabIndex = GUILayout.Toolbar(
            tabIndex,
            new string[] { "Chapters", "Profile", "CG", "Tools" },
            GUILayout.Height(28)
        );
    }

    // =========================
    // CHAPTERS
    // =========================
    private void DrawChapters(ConversationAsset asset)
    {
        DrawSection("CHARACTER");

        DrawCard(() =>
        {
            asset.characterName = EditorGUILayout.TextField("Character Name", asset.characterName);
            EditorGUILayout.PropertyField(profileImageProp);
            EditorGUILayout.PropertyField(conversationIdProp);
        });

        GUILayout.Space(10);

        DrawSection("CHAPTERS");

        searchFilter = EditorGUILayout.TextField("Search", searchFilter);

        GUILayout.Space(5);

        for (int i = 0; i < asset.chapters.Count; i++)
        {
            var entry = asset.chapters[i];

            if (!MatchesSearch(entry, i))
                continue;

            if (i == 0)
            {
                DrawEntryPoint(() =>
                {
                    DrawChapterRow(asset, entry, i, true);
                });

                GUILayout.Space(8);
                DrawRegistryDivider();
                GUILayout.Space(6);
            }
            else
            {
                DrawCard(() =>
                {
                    DrawChapterRow(asset, entry, i, false);
                });
            }
        }

        if (GUILayout.Button("+ Add Chapter", GUILayout.Height(28)))
        {
            asset.chapters.Add(new ConversationAsset.ChapterEntry());
        }
    }

    private void DrawChapterRow(ConversationAsset asset, ConversationAsset.ChapterEntry entry, int index, bool isEntry)
    {
        EditorGUILayout.BeginVertical(cardStyle);

        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginChangeCheck();

        string newId = EditorGUILayout.TextField("Chapter ID", entry.chapterId);

        if (!isEntry)
        {
            if (GUILayout.Button("✕", GUILayout.Width(24)))
            {
                asset.chapters.RemoveAt(index);
                EditorUtility.SetDirty(asset);
                GUIUtility.ExitGUI();
                return;
            }
        }

        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            entry.chapterId = newId;
            EditorUtility.SetDirty(asset);
        }

        GUILayout.Space(4);

        // ─────────────────────────────
        // FILE FIELD
        // ─────────────────────────────
        EditorGUI.BeginChangeCheck();

        var newFile = (TextAsset)EditorGUILayout.ObjectField(
            "File",
            entry.file,
            typeof(TextAsset),
            false
        );

        if (EditorGUI.EndChangeCheck())
        {
            entry.file = newFile;

            // Auto ID only if empty
            if (entry.file != null && string.IsNullOrEmpty(entry.chapterId))
            {
                entry.chapterId = ReadFirstTitleFromBub(entry.file);
                EditorUtility.SetDirty(asset);
            }

            EditorUtility.SetDirty(asset);
        }

        EditorGUILayout.EndVertical();
    }

    private bool MatchesSearch(ConversationAsset.ChapterEntry entry, int index)
    {
        if (string.IsNullOrEmpty(searchFilter))
            return true;

        string search = searchFilter.ToLower();

        string id = entry.chapterId ?? "";
        string fileName = entry.file != null ? entry.file.name : "";
        string title = ReadFirstTitleFromBub(entry.file);

        return id.ToLower().Contains(search)
            || fileName.ToLower().Contains(search)
            || title.ToLower().Contains(search)
            || index.ToString().Contains(search);
    }

    // =========================
    // PROFILE
    // =========================
    private void DrawProfile(ConversationAsset asset)
    {
        DrawSection("PROFILE");

        DrawCard(() =>
        {
            EditorGUILayout.LabelField("Name", asset.characterName);
            asset.characterAge = EditorGUILayout.TextField("Age", asset.characterAge);
            asset.birthdate = EditorGUILayout.TextField("Birthdate", asset.birthdate);
            asset.occupation = EditorGUILayout.TextField("Occupation", asset.occupation);

            asset.relationshipStatus =
                (RelationshipStatus)EditorGUILayout.EnumPopup("Relationship", asset.relationshipStatus);

            GUILayout.Space(6);

            asset.bio = EditorGUILayout.TextArea(asset.bio, GUILayout.Height(60));

            EditorGUILayout.LabelField("Description");
            asset.description = EditorGUILayout.TextArea(asset.description, GUILayout.Height(90));
        });
    }

    // =========================
    // CG
    // =========================
    private void DrawCG(ConversationAsset asset)
    {
        DrawSection("CG");

        DrawCard(() =>
        {
            cgList.DoLayoutList();

            if (GUILayout.Button("Clear All"))
                asset.cgAddressableKeys.Clear();
        });
    }

    // =========================
    // TOOLS
    // =========================
    private void DrawTools(ConversationAsset asset)
    {
        DrawSection("TOOLS");

        DrawCard(() =>
        {
            if (GUILayout.Button("Auto-fill Chapter IDs"))
            {
                foreach (var entry in asset.chapters)
                {
                    if (entry.file != null && string.IsNullOrEmpty(entry.chapterId))
                        entry.chapterId = ReadFirstTitleFromBub(entry.file);
                }
            }

            if (GUILayout.Button("Auto-Fill CG from Folder"))
            {
                string path = EditorUtility.OpenFolderPanel("Select CG Folder", "Assets", "");

                if (!string.IsNullOrEmpty(path))
                {
                    FillCGFromFolder(asset, path);
                }
            }

            if (GUILayout.Button("Validate"))
                Validate(asset);
        });
    }

    // =========================
    // UI HELPERS
    // =========================
    private void DrawCard(System.Action content)
    {
        Rect rect = EditorGUILayout.BeginVertical(cardStyle);
        EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.18f));

        GUILayout.Space(4);
        content?.Invoke();
        GUILayout.Space(2);

        EditorGUILayout.EndVertical();
    }

    private void DrawEntryPoint(System.Action content)
    {
        Rect rect = EditorGUILayout.BeginVertical(cardStyle);

        GUILayout.Space(4);
        EditorGUILayout.LabelField("Entry Point — Always loads first", EditorStyles.boldLabel);
        GUILayout.Space(2);

        content?.Invoke();

        EditorGUILayout.EndVertical();
    }

    private void DrawSection(string title)
    {
        GUILayout.Space(6);
        EditorGUILayout.LabelField(title, sectionStyle);
        DrawLine();
        GUILayout.Space(4);
    }

    private void DrawLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(rect, new Color(1f, 1f, 1f, 0.05f));
    }

    // ── Registry divider ────────────────────────────

    private void DrawRegistryDivider()
    {
        Rect divRect = EditorGUILayout.GetControlRect(false, 16);

        float lineY = divRect.y + divRect.height / 2;
        float labelW = 230;
        float labelX = divRect.x + (divRect.width - labelW) / 2;

        // Left line
        Rect leftLine = new Rect(divRect.x, lineY, labelX - divRect.x - 4, 1);
        EditorGUI.DrawRect(leftLine, DIVIDER_LINE);

        // Label
        Rect labelRect = new Rect(labelX, divRect.y, labelW, divRect.height);
        EditorGUI.LabelField(labelRect, "JUMP REGISTRY — ORDER DOES NOT MATTER", _styleDividerLabel);

        // Right line
        Rect rightLine = new Rect(labelX + labelW + 4, lineY, divRect.xMax - labelX - labelW - 4, 1);
        EditorGUI.DrawRect(rightLine, DIVIDER_LINE);
    }

    // =========================
    // LOGIC
    // =========================
    private string ReadFirstTitleFromBub(TextAsset file)
    {
        if (file == null) return "";

        foreach (var line in file.text.Split('\n'))
        {
            var t = line.Trim();
            if (t.StartsWith("title:"))
                return t.Substring(6).Trim();
        }

        return file.name;
    }

    private void FillCGFromFolder(ConversationAsset asset, string fullPath)
    {
        if (!fullPath.Contains("Assets"))
        {
            Debug.LogWarning("Select folder inside Assets.");
            return;
        }

        string projectPath = fullPath.Substring(fullPath.IndexOf("Assets"));
        string[] guids = AssetDatabase.FindAssets("t:Texture2D t:Sprite", new[] { projectPath });

        asset.cgAddressableKeys.Clear();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            asset.cgAddressableKeys.Add(assetPath);
        }
    }

    private void Validate(ConversationAsset asset)
    {
        foreach (var c in asset.chapters)
        {
            if (string.IsNullOrEmpty(c.chapterId))
                Debug.LogWarning("Missing chapter ID");

            if (c.file == null)
                Debug.LogWarning("Missing chapter file");
        }

        Debug.Log("Validation complete");
    }
}
#endif
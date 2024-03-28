/*
 * Copyright (c) 2024 JuicyGrape Apps.
 *
 * Licensed under the MIT License, (the "License");
 * you may not use any file by JuicyGrape Apps except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.juicygrapeapps.com/terms
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Creates and reads the GroupTitles asset file located in the Assets folder, this file contains all the title settings.
/// 
/// To change an attribute's properties just click on the object. The Notice, Important and System attribute's
/// change the objects text color to the same as the gradents 100% value. 
/// 
/// Using any attribute makes it harder to access the objects properties, only use on objects where you don't need
/// access to it's properties regulary.  Clicking on an object will an attibute accesses the attributes properties
/// so you can change them, not the actual object properties these are accessed using the keyboard cursor keys.
///
/// </summary>
/// 
/// <remarks>
/// This class and file it's defined in must have identical names otherwise the GroupTitles assets file cannot store a referance the script.
/// </ remarks >
[CreateAssetMenu(fileName = "GroupTitles", menuName = "Assets/GroupTitles", order = 1)]
class GroupTitles : ScriptableObject
{
    public const string k_customSettingPath = "Assets/GroupTitles.asset";

    public Gradient titleGradient;
    public Gradient subtitleGradient;
    public Gradient noticeObject;
    public Gradient importantObject;
    public Gradient systemObject;

    public static void StoreGradient()
    {
        GroupTitles settings = CreateInstance<GroupTitles>();
        settings.titleGradient = TitleObject.gradient;
        settings.subtitleGradient = SubtitleObject.gradient;
        settings.noticeObject = NoticeObject.gradient;
        settings.importantObject = ImportantObject.gradient;
        settings.systemObject = SystemObject.gradient;

        AssetDatabase.CreateAsset(settings, k_customSettingPath);
        AssetDatabase.SaveAssets();
    }

    public static GroupTitles GradientSetting => AssetDatabase.LoadAssetAtPath<GroupTitles>(k_customSettingPath);

    public static Gradient defaultGradient
    {
        get
        {
            Gradient _gradient = new Gradient();
            Color32 liteColor = new Color32(60, 60, 60, 255);
            Color32 darkColor = new Color32(40, 40, 40, 255);
            GradientAlphaKey alpha = new GradientAlphaKey(1.0f, 0.0f);
            GradientColorKey[] gradientColor = new GradientColorKey[4];
            gradientColor[0] = new GradientColorKey(darkColor, 0.0f);
            gradientColor[1] = new GradientColorKey(liteColor, 0.2f);
            gradientColor[2] = new GradientColorKey(liteColor, 0.8f);
            gradientColor[3] = new GradientColorKey(darkColor, 1.0f);
            GradientAlphaKey[] gradientAlpha = new GradientAlphaKey[4];
            gradientAlpha[0] = alpha;
            gradientAlpha[1] = alpha;
            gradientAlpha[2] = alpha;
            gradientAlpha[3] = alpha;
            _gradient.SetKeys(gradientColor, gradientAlpha);
            return _gradient;
        }
    }
}

/// <summary>
/// An editor function to create a titles, subtitles and other highlighted objects in the projects hierarchy.
/// </summary>
[InitializeOnLoad]
public static class HierarchyObject
{
    static HierarchyObject()
    {
        /// Subscribe to hierarchyWindowItemOnGUI events <see cref="HierarchyWindowItemOnGUI(int, Rect)"/>
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
    }

    // Callback method for hierarchyWindowItemOnGUI events
    static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

        if (gameObject)
        {
            if (gameObject.name.StartsWith("[Title]", System.StringComparison.Ordinal))
            {
                EditorGUI.GradientField(selectionRect, TitleObject.gradient);
                EditorGUI.DropShadowLabel(selectionRect, "• " + gameObject.name.Replace("[Title] ", "").Replace("[Title]", "") + " •");
            }
            else if (gameObject.name.StartsWith("[Subtitle]", System.StringComparison.Ordinal))
            {
                EditorGUI.GradientField(selectionRect, SubtitleObject.gradient);
                EditorGUI.DropShadowLabel(selectionRect, "• " + gameObject.name.Replace("[Subtitle] ", "").Replace("[Subtitle]", "") + " •");
            }
            else if (gameObject.name.StartsWith("[Notice]", System.StringComparison.Ordinal))
            {
                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.UpperLeft;
                style.normal.textColor = SystemObject.gradient.Evaluate(1);
                EditorGUI.GradientField(selectionRect, SystemObject.gradient);
                EditorGUI.DropShadowLabel(selectionRect, "  ›  " + gameObject.name.Replace("[Notice] ", "").Replace("[Notice]", ""), style);
            }
            else if (gameObject.name.StartsWith("[Important]", System.StringComparison.Ordinal))
            {
                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.UpperLeft;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = ImportantObject.gradient.Evaluate(1);
                EditorGUI.GradientField(selectionRect, ImportantObject.gradient);
                EditorGUI.DropShadowLabel(selectionRect, "  ›  " + gameObject.name.Replace("[Important] ", "").Replace("[Important]", ""), style);
            }
            else if (gameObject.name.StartsWith("[System]", System.StringComparison.Ordinal))
            {
                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.UpperLeft;
                style.normal.textColor = NoticeObject.gradient.Evaluate(1);
                EditorGUI.GradientField(selectionRect, NoticeObject.gradient);
                EditorGUI.DropShadowLabel(selectionRect, "  ›  " + gameObject.name.Replace("[System] ", "").Replace("[System]", ""), style);
            }
        }
    }
}

/// <summary>
/// An editor function to create a title object for a group of objects.  
/// </summary>
///
/// Usage: Create an empty GameObject and name it with a "[Title]" attribute followed by the group title".
///
/// <example>[Title] Global Object Group</example>
[System.Serializable]
public static class TitleObject
{
    public static Gradient gradient;
    private static Color[] color = new Color[3];

    static TitleObject()
    {
        gradient = GroupTitles.GradientSetting?.titleGradient;
        if (gradient == null) gradient = GroupTitles.defaultGradient;
        color[0] = gradient.Evaluate(0f);
        color[1] = gradient.Evaluate(0.5f);
        color[2] = gradient.Evaluate(1f);
        EditorApplication.quitting += () => { CheckForChange(); };
    }

    private static void CheckForChange()
    {
        if (color[0] != gradient.Evaluate(0) || color[1] != gradient.Evaluate(0.5f) || color[2] != gradient.Evaluate(1))
        {
            color[0] = gradient.Evaluate(0f);
            color[1] = gradient.Evaluate(0.5f);
            color[2] = gradient.Evaluate(1f);
            GroupTitles.StoreGradient();
        }
    }
}

/// <summary>
/// An editor function to create a subtitle object for a sub group of objects.  
/// </summary>
///
/// Usage: Create an empty GameObject and name it with a "[Subtitle]" attribute followed by the group title".
///
/// <example>[Subtitle] Global Object Group</example>
[System.Serializable]
public static class SubtitleObject
{
    public static Gradient gradient;
    private static Color[] color = new Color[3];

    static SubtitleObject()
    {
        gradient = GroupTitles.GradientSetting?.subtitleGradient;
        if (gradient == null) gradient = GroupTitles.defaultGradient;
        color[0] = gradient.Evaluate(0f);
        color[1] = gradient.Evaluate(0.5f);
        color[2] = gradient.Evaluate(1f);
        EditorApplication.quitting += () => { CheckForChange(); };
    }

    private static void CheckForChange()
    {
        if (color[0] != gradient.Evaluate(0) || color[1] != gradient.Evaluate(0.5f) || color[2] != gradient.Evaluate(1))
        {
            color[0] = gradient.Evaluate(0f);
            color[1] = gradient.Evaluate(0.5f);
            color[2] = gradient.Evaluate(1f);
            GroupTitles.StoreGradient();
        }
    }
}

/// <summary>
/// An editor function to create noticeable objects.  
/// </summary>
///
/// Usage: Add a "[Notice]" attribute in front of a GameObjects name".
///
/// <example>[Notice] Noticeable Object</example>
[System.Serializable]
public static class NoticeObject
{
    public static Gradient gradient;
    private static Color[] color = new Color[3];

    static NoticeObject()
    {
        gradient = GroupTitles.GradientSetting?.noticeObject;
        if (gradient == null) gradient = GroupTitles.defaultGradient;
        color[0] = gradient.Evaluate(0f);
        color[1] = gradient.Evaluate(0.5f);
        color[2] = gradient.Evaluate(1f);
        EditorApplication.quitting += () => { CheckForChange(); };
    }

    private static void CheckForChange()
    {
        if (color[0] != gradient.Evaluate(0) || color[1] != gradient.Evaluate(0.5f) || color[2] != gradient.Evaluate(1))
        {
            color[0] = gradient.Evaluate(0f);
            color[1] = gradient.Evaluate(0.5f);
            color[2] = gradient.Evaluate(1f);
            GroupTitles.StoreGradient();
        }
    }
}

/// <summary>
/// An editor function to create important objects.  
/// </summary>
///
/// Usage: Add "[Important]" attribute in front of a GameObjects name".
///
/// <example>[Important] Important Object</example>
[System.Serializable]
public static class ImportantObject
{
    public static Gradient gradient;
    private static Color[] color = new Color[3];

    static ImportantObject()
    {
        gradient = GroupTitles.GradientSetting?.importantObject;
        if (gradient == null) gradient = GroupTitles.defaultGradient;
        color[0] = gradient.Evaluate(0f);
        color[1] = gradient.Evaluate(0.5f);
        color[2] = gradient.Evaluate(1f);
        EditorApplication.quitting += () => { CheckForChange(); };
    }

    private static void CheckForChange()
    {
        if (color[0] != gradient.Evaluate(0) || color[1] != gradient.Evaluate(0.5f) || color[2] != gradient.Evaluate(1))
        {
            color[0] = gradient.Evaluate(0f);
            color[1] = gradient.Evaluate(0.5f);
            color[2] = gradient.Evaluate(1f);
            GroupTitles.StoreGradient();
        }
    }
}

/// <summary>
/// An editor function to define system objects, use for system generated object.  
/// </summary>
///
/// Usage: Add "[System]" attribute in front of a GameObjects name".
///
/// <example>[System] System Object</example>

[System.Serializable]
public static class SystemObject
{
    public static Gradient gradient;
    private static Color[] color = new Color[3];

    static SystemObject()
    {
        gradient = GroupTitles.GradientSetting?.systemObject;
        if (gradient == null) gradient = GroupTitles.defaultGradient;
        color[0] = gradient.Evaluate(0f);
        color[1] = gradient.Evaluate(0.5f);
        color[2] = gradient.Evaluate(1f);
        EditorApplication.quitting += () => { CheckForChange(); };
    }

    private static void CheckForChange()
    {
        if (color[0] != gradient.Evaluate(0) || color[1] != gradient.Evaluate(0.5f) || color[2] != gradient.Evaluate(1))
        {
            color[0] = gradient.Evaluate(0f);
            color[1] = gradient.Evaluate(0.5f);
            color[2] = gradient.Evaluate(1f);
            GroupTitles.StoreGradient();
        }
    }
}

[CustomPropertyDrawer(typeof(Title))]
public class TitleDrawer : DecoratorDrawer
{
    Title title
    {
        get { return (Title)attribute; }
    }

    public override void OnGUI(Rect position)
    {
        EditorGUI.GradientField(position, TitleObject.gradient);
        EditorGUI.DropShadowLabel(position, "• " + title.text + " •");
    }
}

[CustomPropertyDrawer(typeof(Subtitle))]
public class SubtitleDrawer : DecoratorDrawer
{
    Subtitle title
    {
        get { return (Subtitle)attribute; }
    }

    public override void OnGUI(Rect position)
    {
        EditorGUI.GradientField(position, SubtitleObject.gradient);
        EditorGUI.DropShadowLabel(position, "» " + title.text + " «");
    }
}
#endif

/// <summary>
/// An editor function to create a title for groups of inspector fields.  
/// The titles are created by using a DecoratorDrawer <see cref="TitleDrawer"/>
/// </summary>
/// 
/// Usage: Add [Title{"..."}] attribute above group of propities in script".
///
/// <remarks>
/// To create a space before or after a title use a [Space] attribute on lines above or below the title attibute.
/// </remarks>
/// 
/// <example> [Title("Global Propities"] </example>
public class Title : PropertyAttribute
{
    public string text;

    public Title(string title)
    {
        this.text = title;
    }
}

/// <summary>
/// An editor function to create a subtitle for sub groups of inspector fields.  
/// The subtitles are created by using a DecoratorDrawer <see cref="SubtitleDrawer"/>
/// </summary>
/// 
/// Usage: Add the [Subtitle{"..."}] attribute above the sub group of propities in script".
/// 
/// /// <remarks>
/// To create a space before or after a subtitle use a [Space] attribute on lines above or below the subtitle attibute.
/// </remarks>
/// 
/// <example> [Subtitle("Global Group Propities"] </example>
public class Subtitle : PropertyAttribute
{
    public string text;

    public Subtitle(string title)
    {
        this.text = title;
    }
}
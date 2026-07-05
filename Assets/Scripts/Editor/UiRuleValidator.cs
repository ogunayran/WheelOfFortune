using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace WheelGame.EditorTools
{
    /// <summary>
    /// Editor utility enforcing the case's UI technical rules. Run from the menu:
    ///   WheelGame > Validate UI Rules   — reports violations in the console.
    ///   WheelGame > Fix Raycast Targets — disables RaycastTarget on non-interactive graphics.
    /// </summary>
    public static class UiRuleValidator
    {
        [MenuItem("WheelGame/Validate UI Rules")]
        public static void Validate()
        {
            var report = new StringBuilder("UI Rule Validation\n");
            int issues = 0;

            foreach (Graphic graphic in Object.FindObjectsOfType<Graphic>(true))
            {
                GameObject go = graphic.gameObject;

                // Rule: hierarchy names go general -> specific and start with "ui_".
                if (!go.name.StartsWith("ui_") && graphic.GetComponentInParent<Canvas>() != null)
                {
                    report.AppendLine($"[naming] '{GetPath(go.transform)}' should start with 'ui_'.");
                    issues++;
                }

                // Rule: no RaycastTarget on graphics that are not interactive.
                // (popup "_blocker" images are exempt: their job is to swallow clicks)
                if (graphic.raycastTarget && !IsInteractive(graphic) && !go.name.EndsWith("_blocker"))
                {
                    report.AppendLine($"[raycast] '{GetPath(go.transform)}' has RaycastTarget enabled but is not interactive.");
                    issues++;
                }

                // Rule: use TextMeshPro, never legacy Text.
                if (graphic is Text)
                {
                    report.AppendLine($"[tmp] '{GetPath(go.transform)}' uses legacy Text. Use TextMeshPro.");
                    issues++;
                }

                // Rule: images with border sprites should be Sliced, and images should not be stretched raw.
                if (graphic is Image image && image.sprite != null &&
                    image.sprite.border != Vector4.zero && image.type != Image.Type.Sliced)
                {
                    report.AppendLine($"[sliced] '{GetPath(go.transform)}' sprite has 9-slice borders but Image type is {image.type}.");
                    issues++;
                }
            }

            // Rule: TMP texts whose content changes at runtime must end with "_value".
            foreach (TMP_Text text in Object.FindObjectsOfType<TMP_Text>(true))
            {
                if (text.name.Contains("_value") && !text.name.EndsWith("_value"))
                {
                    report.AppendLine($"[naming] '{GetPath(text.transform)}' — '_value' must be the suffix.");
                    issues++;
                }
            }

            report.AppendLine(issues == 0 ? "All UI rules passed. ✔" : $"{issues} issue(s) found.");
            Debug.Log(report.ToString());
        }

        [MenuItem("WheelGame/Fix Raycast Targets")]
        public static void FixRaycastTargets()
        {
            int fixedCount = 0;
            foreach (Graphic graphic in Object.FindObjectsOfType<Graphic>(true))
            {
                if (graphic.raycastTarget && !IsInteractive(graphic))
                {
                    Undo.RecordObject(graphic, "Disable RaycastTarget");
                    graphic.raycastTarget = false;
                    EditorUtility.SetDirty(graphic);
                    fixedCount++;
                }
            }
            Debug.Log($"[UiRuleValidator] RaycastTarget disabled on {fixedCount} non-interactive graphic(s).");
        }

        private static bool IsInteractive(Graphic graphic)
        {
            return graphic.GetComponentInParent<Selectable>(true) != null;
        }

        private static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }
    }
}

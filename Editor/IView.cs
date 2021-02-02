/*
 * 作者：Peter Xiang
 * 联系方式：565067150@qq.com
 * 文档: https://github.com/PxGame
 * 创建时间: 2019/10/28 16:28:13
 */

using UnityEditor;
using UnityEngine;

namespace XMLib.AM
{
    public abstract class IView
    {
        public abstract string title { get; }
        public virtual bool useAre { get; } = true;

        protected abstract void OnGUI(Rect rect);

        public abstract void OnUpdate();

        public abstract IActionEditorWindow baseWindow { get; }

        public EditorWindow popWindow { get; set; }

        public bool isPop => popWindow != null;

        public Rect localRect { get; protected set; }
        public Rect rect => new Rect(baseWindow.position.position + localRect.position, localRect.size);

        public virtual void Draw(Rect rect)
        {
            this.localRect = rect;
            OnGUI(rect);
        }

        public void ShowPopWindow()
        {
            if (popWindow != null)
            {
                popWindow.Focus();
                return;
            }
            ViewWindow.Show(this, rect);
        }

        public void HidePopWindow()
        {
            if (popWindow == null)
            {
                return;
            }
            popWindow.Close();
            popWindow = null;
        }

        public virtual void OnDestroy()
        {
            HidePopWindow();
        }

        public void OnPopDestroy()
        {
            popWindow = null;
        }
    }

    /// <summary>
    /// IView
    /// </summary>
    public abstract class IView<ControllerType, FloatType> : IView where FloatType : struct
    {
        public override IActionEditorWindow baseWindow => win;
        public ActionEditorWindow<ControllerType, FloatType> win { get; set; }
        public virtual bool checkConfig { get; } = true;

        public override void Draw(Rect rect)
        {
            this.localRect = rect;
            Rect contentRect = rect;

            if (!string.IsNullOrEmpty(title))
            {
                float titleHeight = 18f;
                Rect titleRect = new Rect(rect.x, rect.y, rect.width, titleHeight);
                contentRect = new Rect(rect.x, rect.y + titleHeight, rect.width, rect.height - titleHeight);

                GUILayout.BeginArea(titleRect);

                GUILayout.BeginHorizontal();
                GUILayout.Label(title, AEStyles.view_head);

                OnHeaderDraw();

                if (!isPop)
                {
                    if (GUILayout.Button("W", AEStyles.view_head, GUILayout.Width(20)))
                    {
                        GUI.FocusControl(null);
                        ShowPopWindow();
                    }
                }

                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }

            GUI.Box(contentRect, GUIContent.none, AEStyles.view_bg);

            if (!checkConfig || win.isConfigValid)
            {
                if (useAre)
                {
                    GUILayout.BeginArea(contentRect);
                    OnGUI(contentRect);
                    GUILayout.EndArea();
                }
                else
                {
                    OnGUI(contentRect);
                }
            }
        }

        protected virtual void OnHeaderDraw()
        {
        }
    }
}
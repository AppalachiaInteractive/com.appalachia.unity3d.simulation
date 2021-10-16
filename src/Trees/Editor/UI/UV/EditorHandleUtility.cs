using UnityEditor;
using UnityEngine;

namespace Appalachia.Simulation.Trees.UI.UV
{
    internal static class EditorHandleUtility
    {
        private const int HANDLE_PADDING = 8;
        private const int LEFT_MOUSE_BUTTON = 0;
        private const int MIDDLE_MOUSE_BUTTON = 2;

        private static readonly Quaternion QuaternionUp = Quaternion.Euler(Vector3.right * 90f);
        private static readonly Quaternion QuaternionRight = Quaternion.Euler(Vector3.up * 90f);
        private static readonly Vector3 ConeDepth = new Vector3(0f, 0f, 16f);

        private static readonly Color k_HandleColorUp = new Color(0f, .7f, 0f, .8f);
        private static readonly Color k_HandleColorRight = new Color(0f, 0f, .7f, .8f);
        private static readonly Color k_HandleColorRotate = new Color(0f, .7f, 0f, .8f);
        private static readonly Color k_HandleColorScale = new Color(.7f, .7f, .7f, .8f);

        private static Material s_HandleMaterial;
        private static int currentId = -1;

        private static Vector2 handleOffset = Vector2.zero;
        private static Vector2 initialMousePosition = Vector2.zero;

        private static HandleConstraint2D
            axisConstraint = new HandleConstraint2D(0, 0); // Multiply this value by input to mask axis movement.

        public static bool limitToLeftButton = true;

        private static Vector2 s_InitialDirection;

        public static Material handleMaterial
        {
            get
            {
                if (s_HandleMaterial == null)
                {
                    s_HandleMaterial = (Material) EditorGUIUtility.LoadRequired("SceneView/2DHandleLines.mat");
                }

                return s_HandleMaterial;
            }
        }

        public static int CurrentID => currentId;

        /*
         * A 2D GUI view position handle.
         * @param id The Handle id.
         * @param position The position in GUI coordinates.
         * @param size How large in pixels to draw this handle.
         */
        public static Vector2 PositionHandle2d(int id, Vector2 position, int size)
        {
            var w = size / 4;
            var evt = Event.current;

            var handleRectUp = new Rect(
                position.x - (w / 2f),
                position.y - size - HANDLE_PADDING,
                w,
                size + HANDLE_PADDING
            );
            var handleRectRight = new Rect(position.x, position.y - (w / 2f), size, w + HANDLE_PADDING);

            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.CircleHandleCap(-1, position, Quaternion.identity, w / 2f, Event.current.type);
            UnityEditor.Handles.color = k_HandleColorUp;

            // Y Line
            UnityEditor.Handles.DrawLine(position, position - (Vector2.up * size));

            // Y Cone
            if ((position.y - size) > 0f)
            {
                UnityEditor.Handles.ConeHandleCap(
                    0,
                    (Vector3) (position - (Vector2.up * size)) - ConeDepth,
                    QuaternionUp,
                    w / 2f,
                    evt.type
                );
            }

            UnityEditor.Handles.color = k_HandleColorRight;

            // X Line
            UnityEditor.Handles.DrawLine(position, position + (Vector2.right * size));

            // X Cap
            if (position.y > 0f)
            {
                UnityEditor.Handles.ConeHandleCap(
                    0,
                    (Vector3) (position + (Vector2.right * size)) - ConeDepth,
                    QuaternionRight,
                    w / 2f,
                    evt.type
                );
            }

            // If a Tool already is engaged and it's not this one, bail.
            if ((currentId >= 0) && (currentId != id))
            {
                return position;
            }

            var mousePosition = evt.mousePosition;
            var newPosition = position;

            if (currentId == id)
            {
                switch (evt.type)
                {
                    case EventType.MouseDrag:
                        newPosition = axisConstraint.Mask(mousePosition + handleOffset) +
                            axisConstraint.InverseMask(position);
                        break;

                    case EventType.MouseUp:
                    case EventType.Ignore:
                        currentId = -1;
                        break;
                }
            }
            else
            {
                if ((evt.type == EventType.MouseDown) &&
                    ((!limitToLeftButton && (evt.button != MIDDLE_MOUSE_BUTTON)) || (evt.button == LEFT_MOUSE_BUTTON)))
                {
                    if (Vector2.Distance(mousePosition, position) < (w / 2f))
                    {
                        currentId = id;
                        handleOffset = position - mousePosition;
                        axisConstraint = new HandleConstraint2D(1, 1);
                    }
                    else if (handleRectRight.Contains(mousePosition))
                    {
                        currentId = id;
                        handleOffset = position - mousePosition;
                        axisConstraint = new HandleConstraint2D(1, 0);
                    }
                    else if (handleRectUp.Contains(mousePosition))
                    {
                        currentId = id;
                        handleOffset = position - mousePosition;
                        axisConstraint = new HandleConstraint2D(0, 1);
                    }
                }
            }

            return newPosition;
        }

        /// <summary>
        ///     A 2D rotation handle. Behaves like HandleUtility.RotationHandle
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static float RotationHandle2d(int id, Vector2 position, float rotation, int radius)
        {
            var evt = Event.current;
            var mousePosition = evt.mousePosition;
            var newRotation = rotation;

            var currentDirection = (mousePosition - position).normalized;

            // Draw gizmos
            UnityEditor.Handles.color = k_HandleColorRotate;
            UnityEditor.Handles.CircleHandleCap(-1, position, Quaternion.identity, radius, evt.type);

            if (currentId == id)
            {
                UnityEditor.Handles.color = Color.gray;
                UnityEditor.Handles.DrawLine(position, position + ((mousePosition - position).normalized * radius));
                UnityEngine.GUI.Label(new Rect(position.x, position.y, 90f, 30f), newRotation.ToString("F2") + (char) 176);
            }

            // If a Tool already is engaged and it's not this one, bail.
            if ((currentId >= 0) && (currentId != id))
            {
                return rotation;
            }

            if (currentId == id)
            {
                switch (evt.type)
                {
                    case EventType.MouseDrag:

                        newRotation = Vector2.Angle(s_InitialDirection, currentDirection);

                        if (Vector2.Dot(new Vector2(-s_InitialDirection.y, s_InitialDirection.x), currentDirection) < 0)
                        {
                            newRotation = 360f - newRotation;
                        }

                        break;

                    case EventType.MouseUp:
                    case EventType.Ignore:
                        currentId = -1;
                        break;
                }
            }
            else
            {
                if ((evt.type == EventType.MouseDown) &&
                    ((!limitToLeftButton && (evt.button != MIDDLE_MOUSE_BUTTON)) || (evt.button == LEFT_MOUSE_BUTTON)))
                {
                    if (Mathf.Abs(Vector2.Distance(mousePosition, position) - radius) < 8)
                    {
                        currentId = id;
                        initialMousePosition = mousePosition;
                        s_InitialDirection = (initialMousePosition - position).normalized;
                        handleOffset = position - mousePosition;
                    }
                }
            }

            return newRotation;
        }

        /// <summary>
        ///     Scale handle in 2d space.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Vector2 ScaleHandle2d(int id, Vector2 position, Vector2 scale, int size)
        {
            var evt = Event.current;
            var mousePosition = evt.mousePosition;
            var width = size / 4;

            UnityEditor.Handles.color = k_HandleColorUp;
            UnityEditor.Handles.DrawLine(position, position - (Vector2.up * size * scale.y));

            if ((position.y - size) > 0f)
            {
                UnityEditor.Handles.CubeHandleCap(
                    0,
                    (Vector3) (position - (Vector2.up * scale.y * size)) - (Vector3.forward * 16),
                    QuaternionUp,
                    width / 3f,
                    evt.type
                );
            }

            UnityEditor.Handles.color = k_HandleColorRight;
            UnityEditor.Handles.DrawLine(position, position + (Vector2.right * size * scale.x));

            if (position.y > 0f)
            {
                UnityEditor.Handles.CubeHandleCap(
                    0,
                    (Vector3) (position + (Vector2.right * scale.x * size)) - (Vector3.forward * 16),
                    Quaternion.Euler(Vector3.up * 90f),
                    width / 3f,
                    evt.type
                );
            }

            UnityEditor.Handles.color = k_HandleColorScale;

            UnityEditor.Handles.CubeHandleCap(0, (Vector3) position - (Vector3.forward * 16), QuaternionUp, width / 2f, evt.type);

            // If a Tool already is engaged and it's not this one, bail.
            if ((currentId >= 0) && (currentId != id))
            {
                return scale;
            }

            var handleRectUp = new Rect(
                position.x - (width / 2f),
                position.y - size - HANDLE_PADDING,
                width,
                size + HANDLE_PADDING
            );
            var handleRectRight = new Rect(position.x, position.y - (width / 2f), size + 8, width);
            var handleRectCenter = new Rect(position.x - (width / 2f), position.y - (width / 2f), width, width);

            if (currentId == id)
            {
                switch (evt.type)
                {
                    case EventType.MouseDrag:
                        var diff = axisConstraint.Mask(mousePosition - initialMousePosition);
                        diff.x += size;
                        diff.y = -diff.y; // gui space Y is opposite-world
                        diff.y += size;
                        scale = diff / size;
                        if (axisConstraint == HandleConstraint2D.None)
                        {
                            scale.x = Mathf.Min(scale.x, scale.y);
                            scale.y = Mathf.Min(scale.x, scale.y);
                        }

                        break;

                    case EventType.MouseUp:
                    case EventType.Ignore:
                        currentId = -1;
                        break;
                }
            }
            else
            {
                if ((evt.type == EventType.MouseDown) &&
                    ((!limitToLeftButton && (evt.button != MIDDLE_MOUSE_BUTTON)) || (evt.button == LEFT_MOUSE_BUTTON)))
                {
                    if (handleRectCenter.Contains(mousePosition))
                    {
                        currentId = id;
                        handleOffset = position - mousePosition;
                        initialMousePosition = mousePosition;
                        axisConstraint = new HandleConstraint2D(1, 1);
                    }
                    else if (handleRectRight.Contains(mousePosition))
                    {
                        currentId = id;
                        handleOffset = position - mousePosition;
                        initialMousePosition = mousePosition;
                        axisConstraint = new HandleConstraint2D(1, 0);
                    }
                    else if (handleRectUp.Contains(mousePosition))
                    {
                        currentId = id;
                        handleOffset = position - mousePosition;
                        initialMousePosition = mousePosition;
                        axisConstraint = new HandleConstraint2D(0, 1);
                    }
                }
            }

            return scale;
        }
    }
}

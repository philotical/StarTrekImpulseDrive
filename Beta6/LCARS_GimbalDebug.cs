using UnityEngine;

namespace Philotical
{
    class GimbalDebug
    {
        LineRenderer l1 = null;
        LineRenderer l2 = null;
        LineRenderer l3 = null;

        public void drawGimbal(Vector3d position, int length, float width)
        {
            GameObject o = new GameObject("Test");
            o.transform.localPosition = position;
            _drawGimbal(o.transform, length, width);
        }
        public void drawGimbal(GameObject o, int length, float width)
        {
            _drawGimbal(o.transform, length, width);
        }
        public void drawGimbal(Vessel o, int length, float width)
        {
            _drawGimbal(o.transform, length, width);
        }
        public void drawGimbal(Part o, int length, float width)
        {
            _drawGimbal(o.transform, length,  width);
        }
        public void removeGimbal()
        {
            _removeGimbal();
        }
        private void _drawGimbal(Transform o, int length, float width)
        {
            if (l1==null)
            {
                l1 = new LineRenderer();
                l2 = new LineRenderer();
                l3 = new LineRenderer();
            }
            this.l1 = DebugLine(l1, o.transform, o.transform.up, Color.green, length, width);
            this.l2 = DebugLine(l2, o.transform, o.transform.right, Color.red, length, width);
            this.l3 = DebugLine(l3, o.transform, o.transform.forward, Color.blue, length, width);
        }
        private void _removeGimbal()
        {
            this.l1.SetPosition(0, Vector3.zero);
            this.l1.SetPosition(1, Vector3.zero);
            this.l2.SetPosition(0, Vector3.zero);
            this.l2.SetPosition(1, Vector3.zero);
            this.l3.SetPosition(0, Vector3.zero);
            this.l3.SetPosition(1, Vector3.zero);
        }
        private LineRenderer DebugLine(LineRenderer line, Transform origin, Vector3 transformDirection, Color color, int length, float width)
        {
            GameObject o = new GameObject("Test");
            Transform transform = origin;
            o.transform.parent = transform;
            o.transform.localEulerAngles = Vector3.zero;
            line = o.AddComponent<LineRenderer>();
            line.transform.parent = transform;
            line.useWorldSpace = false;
            line.transform.localPosition = origin.localPosition;
            line.transform.localEulerAngles = Vector3.zero;
            line.material = new Material(Shader.Find("Particles/Additive"));
            line.SetWidth(width, width);
            line.SetVertexCount(2);
            line.SetPosition(0, transform.localPosition);
            line.SetPosition(1, transform.InverseTransformDirection(transformDirection) * length);
            //line.SetPosition(1, transform.TransformPoint(transform.InverseTransformDirection(transformDirection)) * length);
            //line.SetPosition(1, transform.TransformPoint(transform.InverseTransformDirection(transformDirection)) * length);
            line.SetColors(color, color);
            return line;
        }

    }
}

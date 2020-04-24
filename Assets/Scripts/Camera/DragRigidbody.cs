using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class DragRigidbody : MonoBehaviour
    {
        const float k_Spring = 50.0f;
        const float k_Damper = 5.0f;
        const float k_Drag = 10.0f;
        const float k_AngularDrag = 5.0f;
        const float k_Distance = 0.2f;
        const bool k_AttachToCenterOfMass = false;
        Vector3 defaultPos;
        public GameObject grabberHand;
        Vector3 hitPointVector;

        private SpringJoint m_SpringJoint;

        private void Awake()
        {
            defaultPos = grabberHand.transform.localPosition;
        }


        private void Update()
        {
            // Make sure the user pressed the mouse down
            if (!Input.GetMouseButtonDown(1))
            {
                grabberHand.transform.localPosition = defaultPos;
                grabberHand.transform.eulerAngles = Camera.main.transform.eulerAngles;
                return;
            }

            var mainCamera = FindCamera();
            int layerMask = (1 << 10);
            layerMask = ~layerMask;

            // We need to actually hit an object
            RaycastHit hit = new RaycastHit();
            if (
                !Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition).origin,
                                 mainCamera.ScreenPointToRay(Input.mousePosition).direction, out hit, 5,
                                 layerMask))
            {
                return;
            }
            // We need to hit a rigidbody that is not kinematic
            if (!hit.rigidbody || hit.rigidbody.isKinematic)
            {
                return;
            }

            if (hit.collider.CompareTag("Player"))
            {
                return;
            }

            if (!m_SpringJoint)
            {
                var go = new GameObject("Rigidbody dragger");
                Rigidbody body = go.AddComponent<Rigidbody>();
                m_SpringJoint = go.AddComponent<SpringJoint>();
                body.isKinematic = true;
            }

            m_SpringJoint.transform.position = hit.point;
            m_SpringJoint.anchor = Vector3.zero;

            m_SpringJoint.spring = k_Spring;
            m_SpringJoint.damper = k_Damper;
            m_SpringJoint.maxDistance = k_Distance;
            m_SpringJoint.connectedBody = hit.rigidbody;

            StartCoroutine("DragObject", hit.distance);
        }


        private IEnumerator DragObject(float distance)
        {
            var oldDrag = m_SpringJoint.connectedBody.drag;
            var oldAngularDrag = m_SpringJoint.connectedBody.angularDrag;
            m_SpringJoint.connectedBody.drag = k_Drag;
            m_SpringJoint.connectedBody.angularDrag = k_AngularDrag;
            var mainCamera = FindCamera();
            bool distanceIsOk = true;
            while (distanceIsOk && Input.GetMouseButton(1))
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                m_SpringJoint.transform.position = ray.GetPoint(distance);
                grabberHand.transform.position = m_SpringJoint.connectedBody.transform.TransformPoint(m_SpringJoint.connectedAnchor);
                grabberHand.transform.eulerAngles = m_SpringJoint.connectedBody.transform.eulerAngles;

                distanceIsOk = (Vector3.Distance(grabberHand.transform.position, mainCamera.transform.position) < 5);

                yield return null;
            }
            if (m_SpringJoint.connectedBody)
            {
                m_SpringJoint.connectedBody.drag = oldDrag;
                m_SpringJoint.connectedBody.angularDrag = oldAngularDrag;
                m_SpringJoint.connectedBody = null;
            }
        }


        private Camera FindCamera()
        {
            if (GetComponent<Camera>())
            {
                return GetComponent<Camera>();
            }

            return Camera.main;
        }
    }
}
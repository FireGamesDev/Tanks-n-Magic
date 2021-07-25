using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevDev.Cameras
{
    public class LD_Camera_Menu : MonoBehaviour
    {
        [MenuItem("LevDev/Cameras/Top Down Camera")]
        public static void CreateTopDownCamera()
        {
            GameObject[] selectedGO = Selection.gameObjects;
            //foreach (var selected in selectedGO)
            //{
            //    Debug.Log(selected.name);
            //}

            if (selectedGO.Length > 0 && selectedGO[0].GetComponent<Camera>())
            {
                if (selectedGO.Length < 2)
                {
                    AttachTopDownScript(selectedGO[0].gameObject, null);
                }
                else if (selectedGO.Length == 2)
                {
                    AttachTopDownScript(selectedGO[0].gameObject, selectedGO[1].transform);
                }
                else if(selectedGO.Length >= 3)
                {
                    EditorUtility.DisplayDialog("Camera Tools", "You can only select two GameObjects in the scene for this to work and the first selection needs to be a camera", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Camera Tools", "You need to select a GameObject in the Scene " +
                    "that has a Camera component assigned to it!", "0K");
            }
        }

        static void AttachTopDownScript(GameObject aCamera, Transform aTarget)
        {
            //assign top down Script to the camera
            LD_TopDown_Camera cameraScript = null;
            if (aCamera)
            {
                cameraScript = aCamera.AddComponent<LD_TopDown_Camera>();
            

                //Check to see if we have a target and we have a script reference
                if(cameraScript && aTarget)
                {
                    cameraScript.m_Target = aTarget;
                }

                Selection.activeGameObject = aCamera;
            }
        }
    }
}

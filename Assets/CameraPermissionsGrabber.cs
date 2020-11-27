using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

using Doublsb.Dialog;
using UnityEngine.UI;

public class CameraPermissionsGrabber : MonoBehaviour
{
    private bool _locationPermissionAsked;
    private bool _microphonePermissionAsked;
    private bool _cameraPermissionAsked;
    private bool _storagePermissionAsked;

    private void Start()
    {
    #if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(AskForPermissions());        
    #else
        /***** Ready to run you app *****/
    #endif
    }

    private IEnumerator AskForPermissions()
    {
        List<bool> permissions = new List<bool>() { false, false, false, false };
        List<bool> permissionsAsked = new List<bool>() { false, false, false, false };
        List<Action> actions = new List<Action>()
        {
        
            new Action(() => {
                permissions[2] = Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);
                if (!permissions[2] && !permissionsAsked[2])
                {
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                    permissionsAsked[2] = true;
                    return;
                }
            })
        };
        for(int i = 0; i < permissionsAsked.Count; )
        {
            actions[i].Invoke();
            if(permissions[i])
            {
                ++i;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
}

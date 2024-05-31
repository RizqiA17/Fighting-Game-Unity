using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HomeCanvasManipulator : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject parrent;
    private float cameraRotationX;
    private Quaternion cameraRotationDefault;
    private bool aboutOpened;
    // Start is called before the first frame update
    void Start()
    {
        cameraRotationX = cameraTransform.localRotation.x;
        cameraRotationDefault = cameraTransform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        ChangeToAbout();
    }

    public void About()
    {
        if (aboutOpened)
        {
            aboutOpened = false;
            anim.CrossFade("Fade In",.1f);
        }
        else
        {
            aboutOpened = true;
            anim.CrossFade("Fade Out", .1f);
        }
    }

    void ChangeToAbout()
    {
        if (aboutOpened)
        {
            Quaternion newRotation = Quaternion.AngleAxis(-180, Vector3.up);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, newRotation, .5f);
            GetComponent<Button>().interactable = false;
            if (cameraTransform.rotation == Quaternion.AngleAxis(-180, Vector3.up))
            {
                parrent.SetActive(false);
            }
        }
        else
        {
            Quaternion newRotation = Quaternion.AngleAxis(-90, Vector3.up);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, newRotation, .5f);
        }

    }
}

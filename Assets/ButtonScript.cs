using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject parrentPanel;
    [SerializeField] private GameObject targetPanel;
    //[SerializeField] private Transform mainCamera;
    //[Range(-360f, 360f)]
    //[SerializeField] private float cameraRotation;
    //[SerializeField] private bool transition3DSelf;
    //[SerializeField] private bool transition3DTarget;
    //private bool openOtherPanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangePanel()
    {
        //Quaternion newRotation = Quaternion.AngleAxis(-180, Vector3.up);
        //mainCamera.rotation = Quaternion.Slerp(mainCamera.rotation, newRotation, .5f);
        GetComponent<Button>().interactable = false;
        anim.CrossFade("Fade Out", .1f);
        //else anim.CrossFade("Fade Out", .1f);
        targetPanel.SetActive(true);
        StartCoroutine(DisableParrent());
    }

    IEnumerator DisableParrent()
    {
        yield return new WaitForSeconds(.5f);
        parrentPanel.SetActive(false);
        targetPanel.GetComponentInChildren<ButtonScript>().OpenPanel();
    }

    public void OpenPanel()
    {
        anim.CrossFade("Fade In", .1f);

        //if (transition3D) anim.CrossFade("Fade In", .1f);
        //else anim.CrossFade("Fade In", .1f);
    }
}

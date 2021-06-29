using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSeletor : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;

    private void Start()
    {
        inputField.ActivateInputField();
    }
}

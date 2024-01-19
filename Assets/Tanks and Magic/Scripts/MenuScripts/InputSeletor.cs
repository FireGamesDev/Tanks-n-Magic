using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSeletor : MonoBehaviour
{
    public InputField inputField;

    private void Start()
    {
        inputField.ActivateInputField();
    }
}

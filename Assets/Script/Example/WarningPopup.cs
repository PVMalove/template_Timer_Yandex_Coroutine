using UnityEngine;
using UnityEngine.UI;

namespace ExampleYGDateTime
{
    public class WarningPopup : MonoBehaviour
    {
        [SerializeField] private Button resetButton;
        
        private void Start() => 
            resetButton.onClick.AddListener(OnClickResetButton);

        private void OnDestroy() => 
            resetButton.onClick.RemoveListener(OnClickResetButton);

        private void OnClickResetButton() => 
            gameObject.SetActive(false);
    }
}
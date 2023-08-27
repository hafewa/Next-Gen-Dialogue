using System;
using Kurisu.NGDS;
using UnityEngine;
using UnityEngine.UI;
namespace Kurisu.NGDT.VITS.Example
{
    public class VITSOptionUI : MonoBehaviour
    {
        private Text optionText;
        private Button button;
        private DialogueOption option;
        private Action<DialogueOption> onClickCallBack;
        private RectTransform rectTransform;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            optionText = GetComponentInChildren<Text>();
            button = GetComponent<Button>();
            button.onClick.AddListener(OnOptionClick);
        }
        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
        public void UpdateOption(DialogueOption option, Action<DialogueOption> callBack)
        {
            this.option = option;
            optionText.text = option.Content;
            onClickCallBack = callBack;
            LayoutRebuilder.ForceRebuildLayoutImmediate(optionText.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        private void OnOptionClick()
        {
            onClickCallBack?.Invoke(option);
        }
    }
}
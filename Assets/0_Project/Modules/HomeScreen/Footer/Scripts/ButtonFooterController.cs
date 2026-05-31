using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



    public class ButtonFooterController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private Button footerBtn;
        [SerializeField] private bool lockOnAwake;
        [SerializeField] private RectTransform _rectTransform;

        [Header("Events")]
        public UnityEvent<ButtonFooterController> OnButtonClickedEvent;

        //Internal
        private bool _selected;
        private bool _locked;

        
        public RectTransform Rect => _rectTransform;
        public bool IsSelected => _selected;

        void Awake()
        {
            SetLock(lockOnAwake);
        }

        void Start()
        {
            footerBtn.onClick.AddListener(() =>
            {
                OnButtonClickedEvent?.Invoke(this);
            });
        }

        public void SetLock(
            bool locked)
        {
            _locked = locked;

            footerBtn.interactable = _locked == false;

            animator.SetBool("Locked", _locked);
        }

        public void SetSelect(
            bool selected)
        {
            _selected = selected;

            animator.SetBool("Selected", _selected);
        }
    }


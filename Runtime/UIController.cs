using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class UIController : MonoBehaviour
{

    public enum State
    {
        None,
        Show,
        OnShow,
        Hide,
        OnHide,
    }

    public State defaultState = State.None;

    [SerializeField] private UnityEvent m_Show = new UnityEvent();
    [SerializeField] private UnityEvent m_OnShow = new UnityEvent();
    [SerializeField] private UnityEvent m_Hide = new UnityEvent();
    [SerializeField] private UnityEvent m_OnHide = new UnityEvent();
    private UnityEvent m_OnShowDisposable = new UnityEvent();
    private UnityEvent m_OnHideDisposable = new UnityEvent();
    private Animator m_Animator;

    public UnityEvent show => m_Show;
    public UnityEvent onShow => m_OnShow;
    public UnityEvent hide => m_Hide;
    public UnityEvent onHide => m_OnHide;
    public State currentState
    {
        get
        {
            if (animator.runtimeAnimatorController == null || !animator.isInitialized)
                return State.None;

            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

            if (currentState.IsName("Show") && currentState.normalizedTime < 1)
                return State.Show;
            else if (currentState.IsName("Hide") && currentState.normalizedTime < 1)
                return State.Hide;
            else
                return animator.GetBool("Is Show") ? State.OnShow : State.OnHide;
        }
    }
    public Animator animator
    {
        get
        {
            if (m_Animator == null)
                m_Animator = GetComponent<Animator>();

            return m_Animator;
        }
    }

    public virtual void Show()
    {
        show.Invoke();

        animator.SetBool("Is Show", true);
        animator.SetTrigger("Play");
    }
    public virtual void Hide()
    {
        hide.Invoke();

        animator.SetBool("Is Show", false);
        animator.SetTrigger("Play");
    }
    public void Show(UnityAction onShow)
    {
        if (onShow != null)
            m_OnShowDisposable.AddListener(onShow);

        Show();
    }
    public void Hide(UnityAction onHide)
    {
        if (onHide != null)
            m_OnHideDisposable.AddListener(onHide);

        Hide();
    }

    protected virtual void OnEnable()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Init"))
        {
            switch (defaultState)
            {
                case State.Show:
                    animator.SetBool("Is Show", true);
                    animator.Play("Show", 0, 0);
                    break;
                case State.OnShow:
                    animator.SetBool("Is Show", true);
                    animator.Play("Show", 0, 1);

                    break;
                case State.Hide:
                    animator.SetBool("Is Show", false);
                    animator.Play("Hide", 0, 0);
                    break;
                case State.OnHide:
                    animator.SetBool("Is Show", false);
                    animator.Play("Hide", 0, 1);
                    break;
            }
        }
    }
    protected virtual void OnShow()
    {
        onShow.Invoke();
        m_OnShowDisposable.Invoke();
        m_OnShowDisposable.RemoveAllListeners();
    }
    protected virtual void OnHide()
    {
        onHide.Invoke();
        m_OnHideDisposable.Invoke();
        m_OnHideDisposable.RemoveAllListeners();
    }
}

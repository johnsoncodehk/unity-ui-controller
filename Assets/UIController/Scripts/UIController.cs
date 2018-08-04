using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class UIController : MonoBehaviour {

	public enum OnHideAction {
		None,
		Disable,
		Destroy,
	}
	public class PlayAsync : CustomYieldInstruction {

		public PlayAsync(UIController controller, bool isShow) {
			this.m_Obj = controller;
			this.m_ObjName = controller.ToString();

			if (isShow) {
				controller.Show(this.OnCompleted);
			}
			else {
				controller.Hide(this.OnCompleted);
			}
		}

		public override bool keepWaiting {
			get {
				if (this.m_Obj == null) {
					throw new System.Exception(this.m_ObjName + " is be destroy, you can't keep waiting.");
				}
				return !this.m_IsDone;
			}
		}

		private UIController m_Obj;
		private string m_ObjName;
		private bool m_IsDone;

		private void OnCompleted() {
			this.m_IsDone = true;
		}
	}

	public bool showOnAwake = true;
	public OnHideAction onHideAction = OnHideAction.Disable;

	[SerializeField] private UnityEvent m_OnShow = new UnityEvent();
	[SerializeField] private UnityEvent m_OnHide = new UnityEvent();
	private UnityEvent m_OnShowDisposable = new UnityEvent();
	private UnityEvent m_OnHideDisposable = new UnityEvent();
	private Animator m_Animator;

	public UnityEvent onShow {
		get { return this.m_OnShow; }
	}
	public UnityEvent onHide {
		get { return this.m_OnHide; }
	}
	public bool isShow {
		get {
			if (this.animator.runtimeAnimatorController == null) {
				return this.isShowWhenNoController;
			}
			if (!this.animator.isInitialized) {
				return false;
			}
			return this.animator.GetBool("Is Show");
		}
		private set {
			if (this.animator.runtimeAnimatorController == null) {
				this.isShowWhenNoController = value;
				if (value) {
					this.OnShow();
				}
				else {
					this.OnHide();
				}
				return;
			}
			this.animator.SetBool("Is Show", value);
			this.animator.SetTrigger("Play");
		}
	}
	public bool isPlaying {
		get {
			if (this.animator.runtimeAnimatorController == null) {
				return false;
			}
			if (!this.animator.isInitialized) {
				return false;
			}
			AnimatorStateInfo currentState = this.animator.GetCurrentAnimatorStateInfo(0);
			return (currentState.IsName("Show") || currentState.IsName("Hide"))
				&& currentState.normalizedTime < 1;
		}
	}
	public Animator animator {
		get {
			if (this.m_Animator == null) {
				this.m_Animator = this.GetComponent<Animator>();
			}
			return this.m_Animator;
		}
	}
	private bool canTransitionToSelf {
		get {
			if (this.animator.runtimeAnimatorController == null) {
				return true;
			}
			if (!this.animator.isInitialized) {
				return true;
			}
			return this.animator.GetBool("Can Transition To Self");
		}
	}
	private bool isShowWhenNoController;
	private int lastShowAtFrame = -1;

	// Show/Hide must fast by Show(UnityAction)Hide(UnityAction), make SendMessage("Show/Hide") working in Inspector
	public virtual void Show() {
		if (!this.canTransitionToSelf && this.isShow) {
			if (!this.isPlaying) {
				this.OnShow();
			}
			return;
		}

		if (!this.gameObject.activeSelf) {
			this.lastShowAtFrame = Time.frameCount;
			this.gameObject.SetActive(true);
		}
		this.isShow = true;
	}
	public virtual void Hide() {
		if (!this.canTransitionToSelf && !this.isShow) {
			if (!this.isPlaying) {
				this.OnHide();
			}
			return;
		}
		this.isShow = false;
	}
	public void Show(UnityAction onShow) {
		if (onShow != null) {
			m_OnShowDisposable.AddListener(onShow);
		}
		this.Show();
	}
	public void Hide(UnityAction onHide) {
		if (onHide != null) {
			m_OnHideDisposable.AddListener(onHide);
		}
		this.Hide();
	}
	public PlayAsync ShowAsync() {
		return new PlayAsync(this, true);
	}
	public PlayAsync HideAsync() {
		return new PlayAsync(this, false);
	}

	protected virtual void OnEnable() {
		// this.animator.Update(0);
		if (this.showOnAwake && Time.frameCount != this.lastShowAtFrame) {
			this.Show();
		}
		this.lastShowAtFrame = -1;
	}
	protected virtual void OnShow() {
		this.onShow.Invoke();
		this.m_OnShowDisposable.Invoke();
		this.m_OnShowDisposable.RemoveAllListeners();
	}
	protected virtual void OnHide() {
		switch (this.onHideAction) {
			case OnHideAction.None:
				break;
			case OnHideAction.Disable:
				this.gameObject.SetActive(false);
				this.animator.Rebind();
				break;
			case OnHideAction.Destroy:
				Destroy(this.gameObject);
				break;
		}
		this.onHide.Invoke();
		this.m_OnHideDisposable.Invoke();
		this.m_OnHideDisposable.RemoveAllListeners();
	}
}

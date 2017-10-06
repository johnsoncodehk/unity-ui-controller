using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class UIController : MonoBehaviour {

	public enum OnHideAction {
		None,
		Disable,
		Destory,
	}

	public bool showOnAwake = true;
	public OnHideAction onHideAction = OnHideAction.Disable;

	[SerializeField] private UnityEvent m_OnShow = new UnityEvent();
	[SerializeField] private UnityEvent m_OnHide = new UnityEvent();
	private UnityEvent m_DisposableOnShow = new UnityEvent();
	private UnityEvent m_DisposableOnHide = new UnityEvent();
	private Animator m_Animator;
	private Vector3 m_TempSaveScale;

	public UnityEvent onShow {
		get { return this.m_OnShow; }
		private set { this.m_OnShow = value; }
	}
	public UnityEvent onHide {
		get { return this.m_OnHide; }
		private set { this.m_OnHide = value; }
	}
	public bool isShow {
		get {
			if (this.animator.runtimeAnimatorController == null) {
				return this.gameObject.activeSelf;
			}
			if (!this.animator.isInitialized) {
				return false;
			}
			return this.animator.GetBool("Is Show");
		}
		private set {
			if (this.animator.runtimeAnimatorController == null) {
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
			if (!this.isValidController) {
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
			if (!this.isValidController) {
				return false;
			}
			return this.animator.GetBool("Can Transition To Self");
		}
	}
	private bool isValidController {
		get {
			return this.animator.runtimeAnimatorController != null && this.animator.isInitialized;
		}
	}

	// Show/Hide must fast by Show(UnityAction)Hide(UnityAction), make SendMessage("Show/Hide") working in Inspector
	public virtual void Show() {
		if (!this.canTransitionToSelf && this.isShow) {
			if (!this.isPlaying) {
				this.OnShow();
			}
			return;
		}
		bool activeSelf = this.gameObject.activeSelf;

		this.gameObject.SetActive(true);
		this.isShow = true;

		if (this.animator.runtimeAnimatorController != null && !activeSelf && this.gameObject.activeInHierarchy) {
			StartCoroutine(this.SaveAndRevertScale());
		}
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
			m_DisposableOnShow.AddListener(onShow);
		}
		this.Show();
	}
	public void Hide(UnityAction onHide) {
		if (onHide != null) {
			m_DisposableOnHide.AddListener(onHide);
		}
		this.Hide();
	}

	protected virtual void OnShow() {
		this.onShow.Invoke();
		this.m_DisposableOnShow.Invoke();
		this.m_DisposableOnShow.RemoveAllListeners();
	}
	protected virtual void OnHide() {
		if (!this.isValidController || !this.isShow) {
			switch (this.onHideAction) {
				case UIController.OnHideAction.None:
					break;
				case UIController.OnHideAction.Disable:
					this.gameObject.SetActive(false);
					break;
				case UIController.OnHideAction.Destory:
					Destroy(this.gameObject);
					break;
			}
		}
		this.onHide.Invoke();
		this.m_DisposableOnHide.Invoke();
		this.m_DisposableOnHide.RemoveAllListeners();
	}

	private IEnumerator SaveAndRevertScale() {
		if (this.transform.localScale != Vector3.zero) {
			this.m_TempSaveScale = this.transform.localScale;
			this.transform.localScale = Vector3.zero;
		}
		yield return new WaitForEndOfFrame();
		if (this.m_TempSaveScale != Vector3.zero) {
			this.transform.localScale = this.m_TempSaveScale;
			this.m_TempSaveScale = Vector3.zero;
		}
	}
}

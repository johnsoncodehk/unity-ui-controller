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
			return this.animator.isInitialized && this.animator.GetBool("isShow");
		}
		private set {
			if (this.animator.runtimeAnimatorController == null) {
				this.gameObject.SetActive(value);
				if (value) {
					this.OnShow();
				}
				else {
					this.OnHide();
				}
				return;
			}
			if (this.animator.isInitialized) {
				this.animator.SetBool("isShow", value);
			}
		}
	}
	public bool isPlaying {
		get {
			var currentState = this.animator.GetCurrentAnimatorStateInfo(0);
			return this.animator.isInitialized
				&& (currentState.IsName("Show") || currentState.IsName("Hide"))
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

	// Show/Hide must fast by Show(UnityAction)Hide(UnityAction), make SendMessage("Show/Hide") working in Inspector
	public virtual void Show() {
		bool active = this.gameObject.activeSelf;

		this.gameObject.SetActive(true);
		this.isShow = true;

		if (!active && this.gameObject.activeInHierarchy) {
			this.TrySaveScale();
			StartCoroutine(this.TryRevertScale());
		}
	}
	public virtual void Hide() {
		this.isShow = false;
	}
	public void Show(UnityAction onShow) {
		this.ShowHideWithAction(true, onShow, this.m_DisposableOnShow, this.Show);
	}
	public void Hide(UnityAction onHide) {
		this.ShowHideWithAction(false, onHide, this.m_DisposableOnHide, this.Hide);
	}

	protected virtual void OnShow() {
		this.onShow.Invoke();
		this.m_DisposableOnShow.Invoke();
		this.m_DisposableOnShow.RemoveAllListeners();
	}
	protected virtual void OnHide() {
		if (!this.animator.GetBool("isShow")) {
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

	private void TrySaveScale() {
		if (this.transform.localScale != Vector3.zero) {
			this.m_TempSaveScale = this.transform.localScale;
			this.transform.localScale = Vector3.zero;
		}
	}
	private IEnumerator TryRevertScale() {
		yield return new WaitForEndOfFrame();
		if (this.m_TempSaveScale != Vector3.zero) {
			this.transform.localScale = this.m_TempSaveScale;
			this.m_TempSaveScale = Vector3.zero;
		}
	}
	private void ShowHideWithAction(bool isShowAction, UnityAction onAction, UnityEvent onActionEvent, System.Action action) {
		if (isShowAction == this.isShow && !this.isPlaying) {
			if (onAction != null) {
				onAction();
			}
		}
		else {
			if (onAction != null) {
				onActionEvent.AddListener(onAction);
			}
			action();
		}
	}
}

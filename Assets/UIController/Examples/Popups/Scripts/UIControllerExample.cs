namespace UIControllerExamples.Popup {

	public class UIControllerExample : UIController {

		// Base public Functions:
		// - void Show, Hide ()
		// - void Show, Hide (UnityAction)

		// Base virtual Functions:
		// - public void Show, Hide ()
		// - protected void OnShow, OnHide ()

		// Base public Pars:
		// - OnHideAction onHideAction
		// - UnityEvent onShow, onHide
		// - bool isShow, isPlaying
		// - Animator animator

		// OnShow, OnHide evect stpes:
		// Override > Listener > Callback

		// Listeners
		void Awake () {
			this.onShow.AddListener (() => {
				print (this + ": OnShow (AddListener in Awake)");
			});
			this.onHide.AddListener (() => {
				print (this + ": OnHide (AddListener in Awake)");
			});
		}

		// Override
		public override void Show () {
			print (this + ": Show (Override)");
			base.Show ();
		}
		public override void Hide () {
			print (this + ": Hide (Override)");
			base.Hide ();
		}
		protected override void OnShow () {
			print (this + ": OnShow (Override)");
			base.OnShow ();
		}
		protected override void OnHide () {
			print (this + ": OnHide (Override)");
			base.OnHide ();
		}

		// Callback
		public void Play () {
			if (!this.isShow) {
				// this.Show (); // No Callback
				this.Show (() => {
					print (this + ": OnShow (Callback)");
				});
			}
			else {
				// this.Hide (); // No Callback
				this.Hide (() => {
					print (this + ": OnHide (Callback)");
				});
			}
		}
	}
}

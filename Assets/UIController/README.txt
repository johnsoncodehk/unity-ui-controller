# UIController Reference

## About
- Support: johnsoncodehk+support@gmail.com
- GitHub: https://github.com/johnsoncodehk/unity-uicontroller

## Videos
- Examples: https://youtu.be/AvHS_WsVhzQ
- Tutorial: https://youtu.be/Y-2yhzTcvFU

## Variables
- onHideAction
	A enum value of action to on hide
	None: No action.
	Disable: Disable GameObject.
	Destory: Destory GameObject.
- isShow
	A bool value of state.
	true: "Show" animation playing or end.
	false: "Hide" animation playing or end.
- isPlaying
	A bool value of animation state.
	true: "Show" or "Hide" animation is playing.
	false: "Show" or "Hide" animation is end.
- animator
	The animator the UI controller is attached to.

## States
	isShow = Show or OnShow
	!isHide = Hide or OnHide
	isPlaying = Show or Hide
	!isPlaying = OnShow or OnHide
	isShow && isPlaying = Show
	isShow && !isPlaying = OnShow
	!isShow && isPlaying = Hide
	!isShow && !isPlaying = OnHide

## Events
- onShow
	A UnityEvent that is invoked when "Show" animation end.
- onHide
	A UnityEvent that is invoked when "Hide" animation end.

## Public Functions
- Show
	Play "Show" animation.
- Hide
	Play "Hide" animation.

## Code Examples
- Play "Show" animation
	[RequireComponent (typeof (UIController))]
	public class MyPanel : MonoBehaviour {
		void Start () {
			this.GetComponent<UIController> ().Show ();
		}
	}
	public class MyPanel : UIController {
		void Start () {
			this.Show ();
		}
	}
- Add "Show" animation end event
	public class MyPanel : UIController {
		void Start () {
			this.onShow.AddListener (() => {
				print ("Show animation end.");
			});
		}
	}

## Tutorial
- Use UIController Component
	1. Add UIController component, UIController and Animator will be create
	2. Create new controller to use of the Animator (See "Create Controller" step)
- Create Controller
	1. In "Project" window > Create > Animator Override Controller
	2. Rename asset
	3. Click "Setup Show->Hide" on Inspector window
	4. Drag controller to UIController Animator "Controller" Variable
	5. Edit the UI animation on Animation window
# UIController Reference

## Links
- [Asset Store](http://u3d.as/B5u)
- [GitHub](https://github.com/johnsoncodehk/unity-ui-controller)
- [Forum](https://github.com/johnsoncodehk/unity-ui-controller/issues)
- [Support Email](mailto:johnsoncodehk+support@gmail.com)

## Videos
- [Examples](https://youtu.be/AvHS_WsVhzQ)
- [Tutorial](https://youtu.be/Y-2yhzTcvFU)

## Tutorial
1. Add UIController component to GameObject, Animator will be create.
1. Create Override Controller. ("Project" window > Create > Animator Override Controller)
1. Click "Quick Setup/Show_Hide" on Inspector window.
1. Drag controller to UIController Animator "Controller" Variable.
1. Edit the UI animation on Animation window.
1. Click "Show / Hide" On UIController component with Playing Mode to test animations.

## Variables
- showOnAwake
	- If enabled, show animation will start playing when GameObject enable.
- onHideAction
	- A enum value of action to on hide
	- None: No action.
	- Disable: Disable GameObject.
	- Destroy: Destroy GameObject.
- isShow
	- A bool value of state.
	- true: "Show" animation playing or end.
	- false: "Hide" animation playing or end.
- isPlaying
	- A bool value of animation state.
	- true: "Show" or "Hide" animation is playing.
	- false: "Show" or "Hide" animation is end.
- animator
	- The animator the UI controller is attached to.

## Events
- onShow
	- A UnityEvent that is invoked when "Show" animation end.
- onHide
	- A UnityEvent that is invoked when "Hide" animation end.

## Public Functions
- Show
	- Play "Show" animation.
- Hide
	- Play "Hide" animation.

## Code Examples

- Play "Show" animation
```csharp
// No Inheritance
[RequireComponent(typeof(UIController))]
public class MyPanel : MonoBehaviour {
	void Start() {
		this.GetComponent<UIController>().Show();
	}
}
```
```csharp
// Inheritance
public class MyPanel : UIController {
	void Start() {
		this.Show();
	}
}
```

- await Animation(need .Net4.6 project)
```csharp
public class MyPanel : UIController {
	async void Start() {
		await this.ShowAsync();
		print("Show animation end.");
	}
}
```

- Add "Show" animation end event
```csharp
public class MyPanel : UIController {
	void Start() {
		this.onShow.AddListener(() => {
			print("Show animation end.");
		});
	}
}
```
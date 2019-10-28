# unity-ui-controller

### Setup

The following lines needs to be added to your Packages/manifest.json file in your Unity Project under the dependencies section:

```json
"com.johnsoncodehk.unity-ui-controller": "https://github.com/johnsoncodehk/unity-ui-controller.git",
"com.johnsoncodehk.unity-animator-override-controller-extras": "https://github.com/johnsoncodehk/unity-animator-override-controller-extras.git",
```

### v3 VS v2(last AssetStore version)

(v2 can still download in the asset-store branch)

The main change in v3 is no longer assumed the use case, such as automatically activating GameObject when Show, or automatically disabling GameObject when OnHide.

This change was made to remove hidden intermediate logic. v3 can still do the equivalent, and examples has show how to do it.

### Example

https://github.com/johnsoncodehk/unity-ui-controller-examples

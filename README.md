# Collision Detector
CollisionDetector improves collision detections in your Unity project.

Delegate version and UniRx version are available.
# Demo
Collision detection on the vibrated ground is unstable as the following movie.

CollisionDetector makes the collision detection more stable as the following movie.
# How to use
## Settings of CollisionDetector
Attach CollisionDetector script to a GameObject you want to detect the smoothed collision using AddComponent.
Settings of CollisionDetector are as follows.
### Check Other Tag
Check tag of collided GameObject or not.
### Target Tag
if Check Other Tag is true, set the tag you want to detect.
### Cut Off Frequency
Stability factor of the smoothed collision detection.
Lower cut-off frequency makes the smoothed collision detection more slowly.
### Detection Threshold Upper/Lower
Threshold of the smoothed collision detection.
Difference between the upper and the lower threshold is a hysteresis.
The larger hysteresis makes the smoothed detection more stable.

# Use of Smoothed Collision Detection
In the script you want to use, declare as follows.
```cs
using NnanaSoft.CollisionDetector;
```
And use GetComponent<CollisionDetector>() in Start or Awake method.

The next step is different between Delegate version and UniRx version.
## Delegate version
Add the method to the delegate as follows.
```cs
private void Start()
}
    var SmoothedCollision = GetComponent<CollisionDetector>();

    // OnCollisionEnter
    SmoothedCollision.onCollisionEnterSmoothed += OnCollisionEnterMethod;
    // OnCollisionStay
    SmoothedCollision.onCollisionStaySmoothed += OnCollisionStayMethod;
    // OnCollisionExit
    SmoothedCollision.OnCollisionEnterSmoothed += OnCollisionExitMethod;
}
// OnCollisionEnter
private void OnCollisionEnterMethod(Collision collision)
{
    // Write a method you want to do
    Debug.Log("OnCollisionEnterSmoothed");
}
// OnCollisionStay
private void OnCollisionStayMethod(Collision collision)
{
    // Write a method you want to do
    Debug.Log("OnCollisionStaySmoothed");
}
// OnCollisionEnter
private void OnCollisionExitMethod(Collision collision)
{
    // Write a method you want to do
    Debug.Log("OnCollisionExitSmoothed");
}
```
## UniRx version
Import [UniRx](https://github.com/neuecc/UniRx) to your Unity project and write using directive as follows.
```cs
using UniRx;
using UniRx.Triggers;
```
Add the method in Start or Awake method as follows
```cs
private void Start()
}
    var SmoothedCollision = GetComponent<CollisionDetector>();

    // OnCollisionEnter
    SmoothedCollision.OnCollisionEnterSmoothedAsObservable
        .Subscribe(other =>
        {
            // Write a method you want to do
            Debug.Log("OnCollisionEnterSmoothed");
        });
    // OnCollisionStay
    SmoothedCollision.OnCollisionStaySmoothedAsObservable
        .Subscribe(other =>
        {
            // Write a method you want to do
            Debug.Log("OnCollisionStaySmoothed");
        });
    // OnCollisionExit
    SmoothedCollision.OnCollisionEnterSmoothedAsObservable
        .Subscribe(other =>
    {
            // Write a method you want to do
            Debug.Log("OnCollisionExitSmoothed");
        });
}
```
